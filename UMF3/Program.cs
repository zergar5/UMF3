using GridGenerator;
using GridGenerator.Area.Splitting;
using System.Globalization;
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
            new UniformSplitter(2)
        )
    )
    .SetYAxis(new AxisSplitParameter(
            new[] { 0d, 1d },
            new UniformSplitter(2)
        )
    )
    .SetZAxis(new AxisSplitParameter(
            new[] { 0d, 1d },
            new UniformSplitter(2)
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

var firstConditions =
    firstConditionsProvider.GetConditions
    (
        new[] { 0, 0, 1, 2, 3, 4, 5, 6, 7, 7 },
        new[] { Bound.Lower, Bound.Front, Bound.Right, Bound.Left, Bound.Back, Bound.Front, Bound.Right, Bound.Left, Bound.Back, Bound.Upper });

var equation = globalAssembler
    .AssembleEquation(grid)
    .ApplyFirstConditions(firstConditions)
    .BuildEquation();

var luPreconditioner = new LUPreconditioner();

var solver = new LUProfile();
var sparseSolver = new BSGSTAB(luPreconditioner, new LUSparse(luPreconditioner));
sparseSolver.Solve(equation);

var femSolution = new FEMSolution(grid, equation.Solution, new LinearFunctionsProvider());

foreach (var node in grid.Nodes)
{
    femSolution.Calculate(node);
}