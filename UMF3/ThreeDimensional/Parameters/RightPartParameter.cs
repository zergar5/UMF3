using UMF3.Core;
using UMF3.Core.GridComponents;
using UMF3.FEM.Parameters;

namespace UMF3.ThreeDimensional.Parameters;

public class RightPartParameter : IFunctionalParameter
{
    private readonly Func<Node3D, double> _function;
    private readonly Grid<Node3D> _grid;

    public RightPartParameter(
        Func<Node3D, double> function,
        Grid<Node3D> grid
    )
    {
        _function = function;
        _grid = grid;
    }

    public double Calculate(int nodeNumber)
    {
        var node = _grid.Nodes[nodeNumber];
        return _function(node);
    }
}