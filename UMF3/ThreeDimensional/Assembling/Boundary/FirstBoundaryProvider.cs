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
        _uC=uC;
    }

    public FirstCondition[] GetConditions(int[] nodesIndexes)
    {
        var conditions = new List<FirstCondition>(nodesIndexes.Length * 2);

        conditions.
            AddRange(nodesIndexes.
            Select(index => new FirstCondition(index * 2, GetSValue(index))));

        conditions.
            AddRange(nodesIndexes.
                Select(index => new FirstCondition(index * 2 + 1, GetCValue(index))));

        return conditions.ToArray();
    }

    public double GetSValue(int index)
    {
        return _uS(_grid.Nodes[index]);
    }

    public double GetCValue(int index)
    {
        return _uC(_grid.Nodes[index]);
    }
}