using GridGenerator;
using GridGenerator.Area.Splitting;
using System.Globalization;
using UMF3.Core.Converters;
using UMF3.Core.Global;
using UMF3.Core.GridComponents;
using UMF3.FEM;
using UMF3.SLAE.Preconditions.LU;
using UMF3.SLAE.Solvers;
using UMF3.ThreeDimensional.Assembling;
using UMF3.ThreeDimensional.Assembling.Boundary;
using UMF3.ThreeDimensional.Assembling.Global;
using UMF3.ThreeDimensional.Assembling.Local;
using UMF3.ThreeDimensional.MatrixTemplates;
using UMF3.ThreeDimensional.Parameters;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

var gridBuilder3D = new GridBuilder3D();
var grid = gridBuilder3D
    .SetXAxis(new AxisSplitParameter(
            new[] { 0d, 1d },
            new UniformSplitter(1)
        )
    )
    .SetYAxis(new AxisSplitParameter(
            new[] { 0d, 1d },
            new UniformSplitter(1)
        )
    )
    .SetZAxis(new AxisSplitParameter(
            new[] { 0d, 1d },
            new UniformSplitter(1)
        )
    )
    .Build();

var massTemplateProvider = new MassMatrixTemplateProvider();
var stiffnessXTemplateProvider = new StiffnessXMatrixTemplateProvider();
var stiffnessYTemplateProvider = new StiffnessYMatrixTemplateProvider();
var stiffnessZTemplateProvider = new StiffnessZMatrixTemplateProvider();

var materialFactory = new MaterialFactory
(
    new List<double> { 2d },
    new List<double> { 1d },
    new List<double> { 0.01d },
    new List<double> { 1d }
);

var fS = new RightPartParameter(p => -1.01 * p.X + 0.99 * p.Y + 0.99 * p.Z, grid);
var fC = new RightPartParameter(p => 0.99 * p.X + 1.01 * p.Y + 1.01 * p.Z, grid);

var localAssembler = new LocalAssembler
(
    massTemplateProvider,
    stiffnessXTemplateProvider,
    stiffnessYTemplateProvider,
    stiffnessZTemplateProvider,
    materialFactory,
    fS,
    fC
);

var globalAssembler = new GlobalAssembler<Node3D, SparseMatrix>
(new MatrixPortraitBuilder(),
    localAssembler,
    new Inserter(),
    new GaussExcluder()
);

var firstConditionsProvider =
    new FirstBoundaryProvider
    (
        grid,
        p => p.X + p.Y + p.Z,
        p => p.X - p.Y - p.Z
    );

var secondConditionsProvider =
    new SecondBoundaryProvider(grid, materialFactory, new SecondConditionMatrixTemplateProvider());

var thirdConditionsProvider =
    new ThirdBoundaryProvider(firstConditionsProvider, secondConditionsProvider);

var firstConditions =
    firstConditionsProvider
        .GetConditions
        (
            new[] { 0 },
            new[] { Bound.Front }
        );

var secondConditions =
    secondConditionsProvider
        .CreateConditions
        (
            new[] { 0 },
            new[] { Bound.Back },
            p => 1,
            p => -1
        )
        .GetConditions();

var thirdConditions =
    thirdConditionsProvider
        .CreateConditions
        (
            new[] { 0 },
            new[] { Bound.Upper },
            p => 1,
            p => -1,
            new[] { 1d }
        )
        .CreateConditions
        (
            new[] { 0 },
            new[] { Bound.Lower },
            p => -1,
            p => 1,
            new[] { 1d }
        )
        .CreateConditions
        (
            new[] { 0 },
            new[] { Bound.Left },
            p => -1,
            p => -1,
            new[] { 1d }
        )
        .CreateConditions
        (
            new[] { 0 },
            new[] { Bound.Right },
            p => 1,
            p => 1,
            new[] { 1d }
        )
        .GetConditions();

var equation = globalAssembler
    .AssembleEquation(grid)
    .ApplySecondConditions(secondConditions)
    .ApplyThirdConditions(thirdConditions)
    .ApplyFirstConditions(firstConditions)
    .BuildEquation();

var profileMatrix = MatricesConverter.Convert(equation.Matrix);
var profileEquation = new Equation<ProfileMatrix>(profileMatrix, equation.Solution.Clone(), equation.RightSide.Clone());

var luPreconditioner = new LUPreconditioner();

var sparseSolver = new BSGSTAB(luPreconditioner, new LUSparse(luPreconditioner));
sparseSolver.Solve(equation);

var femSolution = new FEMSolution(grid, equation.Solution, new LinearFunctionsProvider());

foreach (var node in grid.Nodes)
{
    femSolution.Calculate(node);
}

//var profileSolver = new LUProfile();

//profileSolver.Solve(profileEquation);

Console.WriteLine();