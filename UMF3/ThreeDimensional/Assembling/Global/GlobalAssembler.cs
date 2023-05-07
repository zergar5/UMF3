using UMF3.Core;
using UMF3.Core.BoundaryConditions;
using UMF3.Core.Global;
using UMF3.FEM.Assembling;
using UMF3.FEM.Assembling.Global;

namespace UMF3.ThreeDimensional.Assembling.Global;

public class GlobalAssembler<TNode, TMatrix>
{
    private readonly IMatrixPortraitBuilder<TNode, TMatrix> _matrixPortraitBuilder;
    private readonly ILocalAssembler _localAssembler;
    private readonly IInserter<TMatrix> _inserter;
    private readonly IGaussExcluder<TMatrix> _gaussExcluder;
    private Equation<TMatrix> _equation;

    public GlobalAssembler
    (
        IMatrixPortraitBuilder<TNode, TMatrix> matrixPortraitBuilder,
        ILocalAssembler localAssembler,
        IInserter<TMatrix> inserter,
        IGaussExcluder<TMatrix> gaussExcluder)
    {
        _matrixPortraitBuilder = matrixPortraitBuilder;
        _localAssembler = localAssembler;
        _inserter = inserter;
        _gaussExcluder = gaussExcluder;
    }

    public Equation<TMatrix> BuildEquation()
    {
        return _equation;
    }

    public GlobalAssembler<TNode, TMatrix> AssembleEquation(Grid<TNode> grid)
    {
        var globalMatrix = _matrixPortraitBuilder.Build(grid);
        _equation = new Equation<TMatrix>(
            globalMatrix,
            new GlobalVector(grid.Nodes.Length * 2),
            new GlobalVector(grid.Nodes.Length * 2)
        );

        foreach (var element in grid)
        {
            var localMatrix = _localAssembler.AssembleMatrix(element);
            var localVector = _localAssembler.AssembleRightSide(element);

            _inserter.InsertMatrix(_equation.Matrix, localMatrix);
            _inserter.InsertVector(_equation.RightSide, localVector);
        }

        return this;
    }

    public GlobalAssembler<TNode, TMatrix> ApplySecondConditions(SecondCondition[] conditions)
    {
        foreach (var condition in conditions)
        {
            _inserter.InsertVector(_equation.RightSide, condition.Vector);
        }

        return this;
    }

    public GlobalAssembler<TNode, TMatrix> ApplyThirdConditions(ThirdCondition[] conditions)
    {
        foreach (var condition in conditions)
        {
            _inserter.InsertMatrix(_equation.Matrix, condition.Matrix);
            _inserter.InsertVector(_equation.RightSide, condition.Vector);
        }

        return this;
    }

    public GlobalAssembler<TNode, TMatrix> ApplyFirstConditions(FirstCondition[] conditions)
    {
        foreach (var condition in conditions)
        {
            _gaussExcluder.Exclude(_equation, condition);
        }

        return this;
    }
}