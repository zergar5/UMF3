using UMF3.Core.Base;
using UMF3.Core.GridComponents;
using UMF3.Core.Local;
using UMF3.FEM.Assembling;
using UMF3.FEM.Parameters;
using UMF3.ThreeDimensional.Parameters;

namespace UMF3.ThreeDimensional.Assembling.Local;

public class LocalAssembler : ILocalAssembler
{
    private readonly ITemplateMatrixProvider _massTemplateProvider;
    private readonly ITemplateMatrixProvider _stiffnessXTemplateProvider;
    private readonly ITemplateMatrixProvider _stiffnessYTemplateProvider;
    private readonly ITemplateMatrixProvider _stiffnessZTemplateProvider;
    private readonly MaterialFactory _materialFactory;
    private readonly IFunctionalParameter _sFunctionalParameter;
    private readonly IFunctionalParameter _cFunctionalParameter;

    public LocalAssembler(
        ITemplateMatrixProvider massTemplateProvider,
        ITemplateMatrixProvider stiffnessXTemplateProvider,
        ITemplateMatrixProvider stiffnessYTemplateProvider,
        ITemplateMatrixProvider stiffnessZTemplateProvider,
        MaterialFactory materialFactory,
        IFunctionalParameter sFunctionalProvider,
        IFunctionalParameter cFunctionalParameter
    )
    {
        _massTemplateProvider = massTemplateProvider;
        _stiffnessXTemplateProvider = stiffnessXTemplateProvider;
        _stiffnessYTemplateProvider = stiffnessYTemplateProvider;
        _stiffnessZTemplateProvider = stiffnessZTemplateProvider;
        _materialFactory = materialFactory;
        _sFunctionalParameter = sFunctionalProvider;
        _cFunctionalParameter = cFunctionalParameter;
    }

    public LocalMatrix AssembleMatrix(Element element)
    {
        var indexes = GetComplexIndexes(element);
        var matrix = GetComplexMatrix(element);

        return new LocalMatrix(indexes, matrix);
    }

    public LocalVector AssembleRightSide(Element element)
    {
        var indexes = GetComplexIndexes(element);
        var vector = GetComplexVector(element);

        return new LocalVector(indexes, vector);
    }

    private BaseMatrix GetStiffnessMatrix(Element element)
    {
        var stiffnessX = _stiffnessXTemplateProvider.GetMatrix();
        var stiffnessY = _stiffnessYTemplateProvider.GetMatrix();
        var stiffnessZ = _stiffnessZTemplateProvider.GetMatrix();

        var stiffness =
            BaseMatrix.Multiply(
                element.Length * element.Width * element.Height / 36d,
                BaseMatrix.Sum(BaseMatrix.Sum(
                    stiffnessX / Math.Pow(element.Length, 2),
                    stiffnessY / Math.Pow(element.Width, 2)),
                    stiffnessZ / Math.Pow(element.Height, 2))
            );

        return stiffness;
    }

    private BaseMatrix GetMassMatrix(Element element)
    {
        var mass = _massTemplateProvider.GetMatrix();

        return element.Length * element.Width * element.Height / 216d * mass;
    }

    private int[] GetComplexIndexes(Element element)
    {
        var indexes = new int[element.NodesIndexes.Length * 2];

        for (var i = 0; i < element.NodesIndexes.Length; i++)
        {
            indexes[i * 2] = 2 * element.NodesIndexes[i];
            indexes[i * 2 + 1] = indexes[i * 2] + 1;
        }

        return indexes;
    }

    private BaseMatrix GetComplexMatrix(Element element)
    {
        var mass = GetMassMatrix(element);
        var stiffness = GetStiffnessMatrix(element);
        var material = _materialFactory.GetById(element.MaterialId);

        stiffness = BaseMatrix.Multiply(material.Lambda, stiffness);

        var matrix = new BaseMatrix(element.NodesIndexes.Length * 2);

        for (var i = 0; i < element.NodesIndexes.Length; i++)
        {
            for (var j = 0; j < element.NodesIndexes.Length; j++)
            {
                matrix[i * 2, j * 2] = stiffness[i, j] - Math.Pow(material.Omega, 2) * material.Chi * mass[i, j];
                matrix[i * 2, j * 2 + 1] = -material.Omega * material.Sigma * mass[i, j];
                matrix[i * 2 + 1, j * 2] = -matrix[i * 2, j * 2 + 1];
                matrix[i * 2 + 1, j * 2 + 1] = matrix[i * 2, j * 2];
            }
        }

        return matrix;
    }

    private BaseVector GetComplexVector(Element element)
    {
        var mass = GetMassMatrix(element);

        var sF = new BaseVector(element.NodesIndexes.Length);
        var cF = new BaseVector(element.NodesIndexes.Length);

        for (var i = 0; i < sF.Count; i++)
        {
            sF[i] = _sFunctionalParameter.Calculate(element.NodesIndexes[i]);
            cF[i] = _cFunctionalParameter.Calculate(element.NodesIndexes[i]);
        }

        sF = mass * sF;
        cF = mass * cF;

        var rightPart = new BaseVector(sF.Count * 2);

        for (var i = 0; i < sF.Count; i++)
        {
            rightPart[i * 2] = sF[i];
            rightPart[i * 2 + 1] = cF[i];
        }

        return rightPart;
    }
}