using UMF3.Core;
using UMF3.Core.Global;
using UMF3.Core.GridComponents;

namespace UMF3.FEM;

public class FEMSolution
{
    private readonly Grid<Node3D> _grid;
    private readonly GlobalVector _solution;

    public FEMSolution(Grid<Node3D> grid, GlobalVector solution)
    {
        _grid = grid;
        _solution = solution;
    }

    public double Calculate(Node3D point)
    {
        throw new NotImplementedException();
    }
}