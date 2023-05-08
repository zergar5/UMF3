using UMF3.Core;
using UMF3.Core.Base;
using UMF3.Core.BoundaryConditions;
using UMF3.Core.GridComponents;
using UMF3.Core.Local;
using UMF3.FEM.Assembling;
using UMF3.ThreeDimensional.Parameters;

namespace UMF3.ThreeDimensional.Assembling.Boundary;

public class SecondBoundaryProvider
{
    public readonly Grid<Node3D> Grid;
    public readonly MaterialFactory MaterialFactory;
    private readonly BaseMatrix _templateMatrix;
    private List<SecondCondition> _conditions = new();

    public SecondBoundaryProvider(Grid<Node3D> grid, MaterialFactory materialFactory, ITemplateMatrixProvider templateMatrixProvider)
    {
        Grid = grid;
        MaterialFactory = materialFactory;
        _templateMatrix = templateMatrixProvider.GetMatrix();
    }

    public SecondCondition[] GetConditions()
    {
        var conditions = _conditions.ToArray();
        _conditions.Clear();
        return conditions;
    }

    public SecondBoundaryProvider CreateConditions(int[] elementsIndexes, Bound[] bounds,
        Func<Node3D, double> uS, Func<Node3D, double> uC)
    {
        var conditions = new List<SecondCondition>(elementsIndexes.Length);

        for (var i = 0; i < elementsIndexes.Length; i++)
        {
            var (indexes, hs) = Grid.Elements[elementsIndexes[i]].GetBoundNodeIndexes(bounds[i]);

            var matrix = GetMatrix(hs[0], hs[1]);

            var material = MaterialFactory.GetById(Grid.Elements[elementsIndexes[i]].MaterialId);

            var vector = GetVector(indexes, uS, uC);
            vector = matrix * BaseVector.Multiply(material.Lambda, vector);

            var complexIndexes = GetComplexIndexes(indexes);

            conditions.Add(new SecondCondition(new LocalVector(complexIndexes, vector)));
        }

        _conditions.AddRange(conditions);

        return this;
    }

    public BaseMatrix GetMatrix(double h1, double h2)
    {
        return h1 * h2 / 36d * _templateMatrix;
    }

    public int[] GetComplexIndexes(int[] indexes)
    {
        var complexIndexes = new int[indexes.Length * 2];

        for (var i = 0; i < indexes.Length; i++)
        {
            complexIndexes[i * 2] = indexes[i] * 2;
            complexIndexes[i * 2 + 1] = indexes[i] * 2 + 1;
        }

        return complexIndexes;
    }

    private BaseVector GetVector(int[] indexes, Func<Node3D, double> uS, Func<Node3D, double> uC)
    {
        var vector = new BaseVector(indexes.Length * 2);

        for (var i = 0; i < indexes.Length; i++)
        {
            vector[i * 2] = uS(Grid.Nodes[indexes[i]]);
            vector[i * 2 + 1] = uC(Grid.Nodes[indexes[i]]);
        }

        return vector;
    }
}