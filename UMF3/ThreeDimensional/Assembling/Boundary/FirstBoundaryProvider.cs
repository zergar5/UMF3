using UMF3.Core;
using UMF3.Core.BoundaryConditions;
using UMF3.Core.GridComponents;

namespace UMF3.ThreeDimensional.Assembling.Boundary;

public class FirstBoundaryProvider
{
    private readonly Grid<Node3D> _grid;
    private readonly Func<Node3D, double> _uS;
    private readonly Func<Node3D, double> _uC;

    public FirstBoundaryProvider(Grid<Node3D> grid, Func<Node3D, double> uS, Func<Node3D, double> uC)
    {
        _grid = grid;
        _uS = uS;
        _uC = uC;
    }

    public FirstCondition[] GetConditions(int[] elementsIndexes, Bound[] bounds)
    {
        var conditions = new List<FirstCondition>(elementsIndexes.Length);

        for (var i = 0; i < elementsIndexes.Length; i++)
        {
            var (indexes, _) = _grid.Elements[elementsIndexes[i]].GetBoundNodeIndexes(bounds[i]);

            var complexIndexes = GetComplexIndexes(indexes);
            var values = new double[complexIndexes.Length];

            for (var j = 0; j < indexes.Length; j++)
            {
                values[j * 2] = _uS(_grid.Nodes[indexes[j]]);
                values[j * 2 + 1] = _uC(_grid.Nodes[indexes[j]]);
            }

            conditions.Add(new FirstCondition(complexIndexes, values));
        }

        return conditions.ToArray();
    }

    public FirstCondition[] GetConditions(int elementsByLength, int elementsByWidth, int elementsByHeight)
    {
        var elementsIndexes = new List<int>();
        var bounds = new List<Bound>();

        for (var i = 0; i < elementsByLength * elementsByWidth; i++)
        {
            elementsIndexes.Add(i);
            bounds.Add(Bound.Lower);
        }

        for (var i = 0; i < elementsByHeight; i++)
        {
            for (var j = 0; j < elementsByLength; j++)
            {
                elementsIndexes.Add(i * elementsByWidth * elementsByLength + j);
                bounds.Add(Bound.Front);
            }
        }

        for (var i = 0; i < elementsByHeight; i++)
        {
            for (var j = 0; j < elementsByWidth; j++)
            {
                elementsIndexes.Add(i * elementsByWidth * elementsByLength + j * elementsByLength + (elementsByWidth - 1));
                bounds.Add(Bound.Right);
            }
        }

        for (var i = 0; i < elementsByHeight; i++)
        {
            for (var j = 0; j < elementsByWidth; j++)
            {
                elementsIndexes.Add(i * elementsByWidth * elementsByLength + j * elementsByLength);
                bounds.Add(Bound.Left);
            }
        }

        for (var i = 0; i < elementsByHeight; i++)
        {
            for (var j = 0; j < elementsByWidth; j++)
            {
                elementsIndexes.Add(i * elementsByWidth * elementsByLength + j + elementsByLength * (elementsByWidth - 1));
                bounds.Add(Bound.Back);
            }
        }

        for (var i = elementsByWidth * elementsByLength * (elementsByHeight - 1); i < elementsByWidth * elementsByLength * elementsByHeight; i++)
        {
            elementsIndexes.Add(i);
            bounds.Add(Bound.Upper);
        }

        return GetConditions(elementsIndexes.ToArray(), bounds.ToArray());
    }

    public double GetSValue(int index)
    {
        return _uS(_grid.Nodes[index]);
    }

    public double GetCValue(int index)
    {
        return _uC(_grid.Nodes[index]);
    }

    private int[] GetComplexIndexes(int[] indexes)
    {
        var complexIndexes = new int[indexes.Length * 2];

        for (var i = 0; i < indexes.Length; i++)
        {
            complexIndexes[i * 2] = indexes[i] * 2;
            complexIndexes[i * 2 + 1] = indexes[i] * 2 + 1;
        }

        return complexIndexes;
    }
}