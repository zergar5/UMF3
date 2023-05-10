using UMF3.Core;
using UMF3.Core.Global;
using UMF3.Core.GridComponents;
using UMF3.ThreeDimensional.Assembling.Local;

namespace UMF3.FEM;

public class FEMSolution
{
    private readonly Grid<Node3D> _grid;
    private readonly GlobalVector _solution;
    private readonly LinearFunctionsProvider _linearFunctionsProvider;

    public FEMSolution(Grid<Node3D> grid, GlobalVector solution, LinearFunctionsProvider linearFunctionsProvider)
    {
        _grid = grid;
        _solution = solution;
        _linearFunctionsProvider = linearFunctionsProvider;
    }

    public (double, double) Calculate(Node3D point)
    {
        if (AreaHas(point))
        {
            var element = _grid.Elements.First(x => ElementHas(x, point));

            var basisFunctions = CreateBasisFunction(element, point);

            var sumS = 0d;
            var sumC = 0d;

            for (var i = 0; i < element.NodesIndexes.Length; i++)
            {
                sumS += _solution[element.NodesIndexes[i] * 2] * basisFunctions[i].CalcFunction(point);
                sumC += _solution[element.NodesIndexes[i] * 2 + 1] * basisFunctions[i].CalcFunction(point);
            }

            CourseHolder.WriteSolution(point, sumS, sumC);

            return (sumS, sumC);
        }

        CourseHolder.WriteAreaInfo();
        CourseHolder.WriteSolution(point, double.NaN, double.NaN);
        return (double.NaN, double.NaN);
    }

    public double CalcError(Func<Node3D, double> uS, Func<Node3D, double> uC)
    {
        var trueSolution = new GlobalVector(_solution.Count);

        for (var i = 0; i < _solution.Count / 2; i++)
        {
            trueSolution[i * 2] = uS(_grid.Nodes[i]);
            trueSolution[i * 2 + 1] = uC(_grid.Nodes[i]);
        }

        GlobalVector.Subtract(_solution, trueSolution);

        return trueSolution.Norm;
    }

    private bool ElementHas(Element element, Node3D node)
    {
        var leftCornerNode = _grid.Nodes[element.NodesIndexes[0]];
        var rightCornerNode = _grid.Nodes[element.NodesIndexes[^1]];
        return node.X >= leftCornerNode.X && node.Y >= leftCornerNode.Y && node.Z >= leftCornerNode.Z &&
               node.X <= rightCornerNode.X && node.Y <= rightCornerNode.Y && node.Z <= rightCornerNode.Z;
    }

    private bool AreaHas(Node3D node)
    {
        var leftCornerNode = _grid.Nodes[0];
        var rightCornerNode = _grid.Nodes[^1];
        return node.X >= leftCornerNode.X && node.Y >= leftCornerNode.Y && node.Z >= leftCornerNode.Z &&
               node.X <= rightCornerNode.X && node.Y <= rightCornerNode.Y && node.Z <= rightCornerNode.Z;
    }

    private LocalBasisFunction[] CreateBasisFunction(Element element, Node3D point)
    {
        var firstXFunction =
            _linearFunctionsProvider.CreateFirstFunction(_grid.Nodes[element.NodesIndexes[1]].X, element.Length);
        var secondXFunction =
            _linearFunctionsProvider.CreateSecondFunction(_grid.Nodes[element.NodesIndexes[0]].X, element.Length);

        var firstYFunction =
            _linearFunctionsProvider.CreateFirstFunction(_grid.Nodes[element.NodesIndexes[2]].Y, element.Width);
        var secondYFunction =
            _linearFunctionsProvider.CreateSecondFunction(_grid.Nodes[element.NodesIndexes[0]].Y, element.Width);

        var firstZFunction =
            _linearFunctionsProvider.CreateFirstFunction(_grid.Nodes[element.NodesIndexes[4]].Z, element.Height);
        var secondZFunction =
            _linearFunctionsProvider.CreateSecondFunction(_grid.Nodes[element.NodesIndexes[0]].Z, element.Height);

        var basisFunctions = new LocalBasisFunction[]
        {
            new (firstXFunction, firstYFunction, firstZFunction),
            new (secondXFunction, firstYFunction, firstZFunction),
            new (firstXFunction, secondYFunction, firstZFunction),
            new (secondXFunction, secondYFunction, firstZFunction),
            new (firstXFunction, firstYFunction, secondZFunction),
            new (secondXFunction, firstYFunction, secondZFunction),
            new (firstXFunction, secondYFunction, secondZFunction),
            new (secondXFunction, secondYFunction, secondZFunction),
        };

        return basisFunctions;
    }
}