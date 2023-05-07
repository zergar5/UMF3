using GridGenerator;
using GridGenerator.Area.Splitting;
using UMF3.Core.Global;
using UMF3.Core.GridComponents;
using UMF3.SLAE.Preconditions.LU;
using UMF3.ThreeDimensional.Assembling;
using UMF3.ThreeDimensional.Assembling.Boundary;
using UMF3.ThreeDimensional.Assembling.Global;
using UMF3.ThreeDimensional.Assembling.Local;
using UMF3.ThreeDimensional.MatrixTemplates;
using UMF3.ThreeDimensional.Parameters;

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

var firstConditions = 
    firstConditionsProvider.GetConditions
    (new[] { 0 }, new[] {Bound.Lower});

var equation = globalAssembler
    .AssembleEquation(grid)
    .ApplyFirstConditions(firstConditions)
    .BuildEquation();

var sparseMatrix = new SparseMatrix
(
    new[] { 0, 0, 1, 2, 4 },
    new[] { 0, 0, 0, 1 },
    new[] { 1d, 1d, 1d, 1d },
    new[] { 2d, 2d, 2d, 2d },
    new[] { 3d, 3d, 3d, 3d }
);
sparseMatrix = new LUPreconditioner().Decompose(sparseMatrix);

Console.WriteLine();