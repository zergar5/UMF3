using UMF3.Core.Base;
using UMF3.Core.BoundaryConditions;
using UMF3.Core.GridComponents;
using UMF3.Core.Local;

namespace UMF3.ThreeDimensional.Assembling.Boundary;

public class ThirdBoundaryProvider
{
    private readonly FirstBoundaryProvider _firstBoundaryProvider;
    private readonly SecondBoundaryProvider _secondBoundaryProvider;
    private List<ThirdCondition> _conditions = new();

    public ThirdBoundaryProvider(FirstBoundaryProvider firstBoundaryProvider,
        SecondBoundaryProvider secondBoundaryProvider)
    {
        _firstBoundaryProvider = firstBoundaryProvider;
        _secondBoundaryProvider = secondBoundaryProvider;
    }

    public ThirdCondition[] GetConditions()
    {
        var conditions = _conditions.ToArray();
        _conditions.Clear();
        return conditions;
    }

    public ThirdBoundaryProvider CreateConditions(int[] elementsIndexes, Bound[] bounds,
        Func<Node3D, double> uS, Func<Node3D, double> uC, double[] betas)
    {
        var conditions = new List<ThirdCondition>(elementsIndexes.Length);

        for (var i = 0; i < elementsIndexes.Length; i++)
        {
            var (indexes, hs) =
                _secondBoundaryProvider.Grid.Elements[elementsIndexes[i]].GetBoundNodeIndexes(bounds[i]);

            var matrix = BaseMatrix.Multiply(betas[i], _secondBoundaryProvider.GetMatrix(hs[0], hs[1]));

            var material =
                _secondBoundaryProvider.MaterialFactory.GetById(_secondBoundaryProvider.Grid
                    .Elements[elementsIndexes[i]].MaterialId);

            var vector = GetVector(indexes, uS, uC, material, betas[i]);

            vector = matrix * vector;

            var complexIndexes = _secondBoundaryProvider.GetComplexIndexes(indexes);

            conditions.Add(new ThirdCondition(new LocalMatrix(complexIndexes, matrix),
                new LocalVector(complexIndexes, vector)));
        }

        _conditions.AddRange(conditions);

        return this;
    }

    private BaseVector GetVector(int[] indexes, Func<Node3D, double> uS, Func<Node3D, double> uC, Material material, double beta)
    {
        var vector = new BaseVector(indexes.Length * 2);

        for (var i = 0; i < indexes.Length; i++)
        {
            vector[i * 2] = (material.Lambda * uS(_secondBoundaryProvider.Grid.Nodes[indexes[i]]) +
                             beta * _firstBoundaryProvider.GetSValue(indexes[i])) / beta;
            vector[i * 2 + 1] = (material.Lambda * uC(_secondBoundaryProvider.Grid.Nodes[indexes[i]]) +
                             beta * _firstBoundaryProvider.GetCValue(indexes[i])) / beta;
        }

        return vector;
    }

}