using GridGenerator.Area.Splitting;
using GridGenerator;
using UMF3.Core;
using UMF3.Core.BoundaryConditions;
using UMF3.Core.Converters;
using UMF3.Core.Global;
using UMF3.Core.GridComponents;
using UMF3.SLAE.Preconditions.LU;
using UMF3.SLAE.Solvers;
using UMF3.ThreeDimensional.Assembling;
using UMF3.ThreeDimensional.Assembling.Boundary;
using UMF3.ThreeDimensional.Assembling.Global;
using UMF3.ThreeDimensional.Assembling.Local;
using UMF3.ThreeDimensional.MatrixTemplates;
using UMF3.ThreeDimensional.Parameters;
using static System.Math;

namespace UMF3;

public static class Tests
{
    private static MassMatrixTemplateProvider _massTemplateProvider = new();
    private static StiffnessXMatrixTemplateProvider _stiffnessXTemplateProvider = new();
    private static StiffnessYMatrixTemplateProvider _stiffnessYTemplateProvider = new();
    private static StiffnessZMatrixTemplateProvider _stiffnessZTemplateProvider = new();
    private static MatrixPortraitBuilder _matrixPortraitBuilder = new();
    private static Inserter _inserter = new();
    private static GaussExcluder _gaussExcluder = new();

    private static MaterialFactory _materialFactory = new
    (
        new List<double> { 2d },
        new List<double> { 1d },
        new List<double> { 0.01d },
        new List<double> { 1d }
    );

    private static Func<Node3D, double> FS => p => -1.01 * p.X + 0.99 * p.Y + 0.99 * p.Z;
    private static Func<Node3D, double> FC => p => 0.99 * p.X + 1.01 * p.Y + 1.01 * p.Z;

    public static Grid<Node3D> Grid
    {
        get
        {
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
            return grid;
        }
    }

    public static Grid<Node3D> Grid1000
    {
        get
        {
            var gridBuilder3D = new GridBuilder3D();
            var grid = gridBuilder3D
                .SetXAxis(new AxisSplitParameter(
                        new[] { 0d, 1d },
                        new UniformSplitter(9)
                    )
                )
                .SetYAxis(new AxisSplitParameter(
                        new[] { 0d, 1d },
                        new UniformSplitter(9)
                    )
                )
                .SetZAxis(new AxisSplitParameter(
                        new[] { 0d, 1d },
                        new UniformSplitter(9)
                    )
                )
                .Build();
            return grid;
        }
    }

    public static Grid<Node3D> Grid27000
    {
        get
        {
            var gridBuilder3D = new GridBuilder3D();
            var grid = gridBuilder3D
                .SetXAxis(new AxisSplitParameter(
                        new[] { 0d, 1d },
                        new UniformSplitter(29)
                    )
                )
                .SetYAxis(new AxisSplitParameter(
                        new[] { 0d, 1d },
                        new UniformSplitter(29)
                    )
                )
                .SetZAxis(new AxisSplitParameter(
                        new[] { 0d, 1d },
                        new UniformSplitter(29)
                    )
                )
                .Build();
            return grid;
        }
    }

    private static FirstCondition[] FirstConditions1000
    {
        get
        {
            var grid = Grid1000;

            var firstConditionsProvider =
                new FirstBoundaryProvider
                (
                    grid,
                    US,
                    UC
                );

            return firstConditionsProvider.GetConditions(9, 9, 9);
        }
    }

    private static FirstCondition[] FirstConditions1000Big
    {
        get
        {
            var grid = Grid1000;

            var firstConditionsProvider =
                new FirstBoundaryProvider
                (
                    grid,
                    p => p.X + p.Y + p.Z,
                    p => p.X - p.Y - p.Z
                );

            return firstConditionsProvider.GetConditions(9, 9, 9);
        }
    }

    private static FirstCondition[] FirstConditions27000
    {
        get
        {
            var grid = Grid27000;

            var firstConditionsProvider =
                new FirstBoundaryProvider
                (
                    grid,
                    US,
                    UC
                );

            return firstConditionsProvider.GetConditions(29, 29, 29);
        }
    }

    public static Func<Node3D, double> US => p => Exp(p.X * p.Y);
    public static Func<Node3D, double> UC => p => Exp(p.Z * p.Y);

    public static Equation<ProfileMatrix> ProfileTest1000(double lambdaTest, double sigmaTest, double chiTest,
        double omegaTest)
    {
        var grid = Grid1000;

        var materialFactory = new MaterialFactory
        (
            new List<double> { lambdaTest },
            new List<double> { sigmaTest },
            new List<double> { chiTest },
            new List<double> { omegaTest }
        );

        var (lambda, sigma, chi, omega) = materialFactory.GetById(0);

        var fS = new RightPartParameter(
            p => -lambda * US(p) * (Pow(p.X, 2) * Pow(p.Y, 2)) - omega * sigma * UC(p) - Pow(omega, 2) * chi * US(p),
            grid);
        var fC = new RightPartParameter(
            p => -lambda * UC(p) * (Pow(p.Z, 2) * Pow(p.Y, 2)) - omega * sigma * US(p) - Pow(omega, 2) * chi * UC(p),
            grid);

        var localAssembler = new LocalAssembler
        (
            _massTemplateProvider,
            _stiffnessXTemplateProvider,
            _stiffnessYTemplateProvider,
            _stiffnessZTemplateProvider,
            materialFactory,
            fS,
            fC
        );

        var globalAssembler = new GlobalAssembler<Node3D, SparseMatrix>
        (
            _matrixPortraitBuilder,
            localAssembler,
            _inserter,
            _gaussExcluder
        );

        var firstConditions = FirstConditions1000;

        var equation = globalAssembler
            .AssembleEquation(grid)
            .ApplyFirstConditions(firstConditions)
            .BuildEquation();

        var profileMatrix = MatricesConverter.Convert(equation.Matrix);
        return new Equation<ProfileMatrix>(profileMatrix, equation.Solution.Clone(), equation.RightSide.Clone());
    }

    public static Equation<ProfileMatrix> ProfileTest27000(double lambdaTest, double sigmaTest, double chiTest,
        double omegaTest)
    {
        var grid = Grid27000;

        var materialFactory = new MaterialFactory
        (
            new List<double> { lambdaTest },
            new List<double> { sigmaTest },
            new List<double> { chiTest },
            new List<double> { omegaTest }
        );

        var (lambda, sigma, chi, omega) = materialFactory.GetById(0);

        var fS = new RightPartParameter(
            p => -lambda * US(p) * (Pow(p.X, 2) * Pow(p.Y, 2)) - omega * sigma * UC(p) - Pow(omega, 2) * chi * US(p),
            grid);
        var fC = new RightPartParameter(
            p => -lambda * UC(p) * (Pow(p.Z, 2) * Pow(p.Y, 2)) - omega * sigma * US(p) - Pow(omega, 2) * chi * UC(p),
            grid);

        var localAssembler = new LocalAssembler
        (
            _massTemplateProvider,
            _stiffnessXTemplateProvider,
            _stiffnessYTemplateProvider,
            _stiffnessZTemplateProvider,
            materialFactory,
            fS,
            fC
        );

        var globalAssembler = new GlobalAssembler<Node3D, SparseMatrix>
        (
            _matrixPortraitBuilder,
            localAssembler,
            _inserter,
            _gaussExcluder
        );

        var firstConditions = FirstConditions27000;

        var equation = globalAssembler
            .AssembleEquation(grid)
            .ApplyFirstConditions(firstConditions)
            .BuildEquation();

        var profileMatrix = MatricesConverter.Convert(equation.Matrix);
        return new Equation<ProfileMatrix>(profileMatrix, equation.Solution.Clone(), equation.RightSide.Clone());
    }

    public static Equation<SparseMatrix> SparseTest1000(double lambdaTest, double sigmaTest, double chiTest,
        double omegaTest)
    {
        var grid = Grid1000;

        var materialFactory = new MaterialFactory
        (
            new List<double> { lambdaTest },
            new List<double> { sigmaTest },
            new List<double> { chiTest },
            new List<double> { omegaTest }
        );

        var (lambda, sigma, chi, omega) = materialFactory.GetById(0);

        var fS = new RightPartParameter(
            p => -lambda * US(p) * (Pow(p.X, 2) * Pow(p.Y, 2)) - omega * sigma * UC(p) - Pow(omega, 2) * chi * US(p),
            grid);
        var fC = new RightPartParameter(
            p => -lambda * UC(p) * (Pow(p.Z, 2) * Pow(p.Y, 2)) - omega * sigma * US(p) - Pow(omega, 2) * chi * UC(p),
            grid);

        var localAssembler = new LocalAssembler
        (
            _massTemplateProvider,
            _stiffnessXTemplateProvider,
            _stiffnessYTemplateProvider,
            _stiffnessZTemplateProvider,
            materialFactory,
            fS,
            fC
        );

        var globalAssembler = new GlobalAssembler<Node3D, SparseMatrix>
        (
            _matrixPortraitBuilder,
            localAssembler,
            _inserter,
            _gaussExcluder
        );

        var firstConditions = FirstConditions1000;

        var equation = globalAssembler
            .AssembleEquation(grid)
            .ApplyFirstConditions(firstConditions)
            .BuildEquation();

        return equation;
    }

    public static Equation<SparseMatrix> SparseTest27000(double lambdaTest, double sigmaTest, double chiTest,
        double omegaTest)
    {
        var grid = Grid27000;

        var materialFactory = new MaterialFactory
        (
            new List<double> { lambdaTest },
            new List<double> { sigmaTest },
            new List<double> { chiTest },
            new List<double> { omegaTest }
        );

        var (lambda, sigma, chi, omega) = materialFactory.GetById(0);

        var fS = new RightPartParameter(
            p => -lambda * US(p) * (Pow(p.X, 2) * Pow(p.Y, 2)) - omega * sigma * UC(p) - Pow(omega, 2) * chi * US(p),
            grid);
        var fC = new RightPartParameter(
            p => -lambda * UC(p) * (Pow(p.Z, 2) * Pow(p.Y, 2)) - omega * sigma * US(p) - Pow(omega, 2) * chi * UC(p),
            grid);

        var localAssembler = new LocalAssembler
        (
            _massTemplateProvider,
            _stiffnessXTemplateProvider,
            _stiffnessYTemplateProvider,
            _stiffnessZTemplateProvider,
            materialFactory,
            fS,
            fC
        );

        var globalAssembler = new GlobalAssembler<Node3D, SparseMatrix>
        (
            _matrixPortraitBuilder,
            localAssembler,
            _inserter,
            _gaussExcluder
        );

        var firstConditions = FirstConditions27000;

        var equation = globalAssembler
            .AssembleEquation(grid)
            .ApplyFirstConditions(firstConditions)
            .BuildEquation();

        return equation;
    }

    public static Equation<SparseMatrix> FirstConditionsSparseTest()
    {
        var grid = Grid;

        var fS = new RightPartParameter(FS, grid);
        var fC = new RightPartParameter(FC, grid);

        var localAssembler = new LocalAssembler
        (
            _massTemplateProvider,
            _stiffnessXTemplateProvider,
            _stiffnessYTemplateProvider,
            _stiffnessZTemplateProvider,
            _materialFactory,
            fS,
            fC
        );

        var globalAssembler = new GlobalAssembler<Node3D, SparseMatrix>
        (
            _matrixPortraitBuilder,
            localAssembler,
            _inserter,
            _gaussExcluder
        );

        var firstConditionsProvider =
            new FirstBoundaryProvider
            (
                grid,
                p => p.X + p.Y + p.Z,
                p => p.X - p.Y - p.Z
            );

        var firstConditions =
            firstConditionsProvider
                .GetConditions
                (
                    new[] { 0, 0, 0, 0, 0, 0 },
                    new[] { Bound.Front, Bound.Back, Bound.Left, Bound.Right, Bound.Upper, Bound.Lower }
                );

        var equation = globalAssembler
            .AssembleEquation(grid)
            .ApplyFirstConditions(firstConditions)
            .BuildEquation();

        return equation;
    }

    public static Equation<SparseMatrix> FirstSecondConditionsSparseTest()
    {
        var grid = Grid;

        var fS = new RightPartParameter(FS, grid);
        var fC = new RightPartParameter(FC, grid);

        var localAssembler = new LocalAssembler
        (
            _massTemplateProvider,
            _stiffnessXTemplateProvider,
            _stiffnessYTemplateProvider,
            _stiffnessZTemplateProvider,
            _materialFactory,
            fS,
            fC
        );

        var globalAssembler = new GlobalAssembler<Node3D, SparseMatrix>
        (
            _matrixPortraitBuilder,
            localAssembler,
            _inserter,
            _gaussExcluder
        );

        var firstConditionsProvider =
            new FirstBoundaryProvider
            (
                grid,
                p => p.X + p.Y + p.Z,
                p => p.X - p.Y - p.Z
            );

        var secondConditionsProvider =
            new SecondBoundaryProvider(grid, _materialFactory, new SecondConditionMatrixTemplateProvider());

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
                .CreateConditions
                (
                    new[] { 0 },
                    new[] { Bound.Left },
                    p => -1,
                    p => -1
                )
                .CreateConditions
                (
                    new[] { 0 },
                    new[] { Bound.Right },
                    p => 1,
                    p => 1
                )
                .CreateConditions
                (
                    new[] { 0 },
                    new[] { Bound.Upper },
                    p => 1,
                    p => -1
                )
                .CreateConditions
                (
                    new[] { 0 },
                    new[] { Bound.Lower },
                    p => -1,
                    p => 1
                )
                .GetConditions();


        var equation = globalAssembler
            .AssembleEquation(grid)
            .ApplySecondConditions(secondConditions)
            .ApplyFirstConditions(firstConditions)
            .BuildEquation();

        return equation;
    }

    public static Equation<SparseMatrix> FirstThirdConditionsSparseTest()
    {
        var grid = Grid;

        var fS = new RightPartParameter(FS, grid);
        var fC = new RightPartParameter(FC, grid);

        var localAssembler = new LocalAssembler
        (
            _massTemplateProvider,
            _stiffnessXTemplateProvider,
            _stiffnessYTemplateProvider,
            _stiffnessZTemplateProvider,
            _materialFactory,
            fS,
            fC
        );

        var globalAssembler = new GlobalAssembler<Node3D, SparseMatrix>
        (
            _matrixPortraitBuilder,
            localAssembler,
            _inserter,
            _gaussExcluder
        );

        var firstConditionsProvider =
            new FirstBoundaryProvider
            (
                grid,
                p => p.X + p.Y + p.Z,
                p => p.X - p.Y - p.Z
            );

        var secondConditionsProvider =
            new SecondBoundaryProvider(grid, _materialFactory, new SecondConditionMatrixTemplateProvider());

        var thirdConditionsProvider = new ThirdBoundaryProvider(firstConditionsProvider, secondConditionsProvider);

        var firstConditions =
            firstConditionsProvider
                .GetConditions
                (
                    new[] { 0 },
                    new[] { Bound.Front }
                );

        var thirdConditions =
            thirdConditionsProvider
                .CreateConditions
                (
                    new[] { 0 },
                    new[] { Bound.Back },
                    p => 1,
                    p => -1,
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
                .GetConditions();


        var equation = globalAssembler
            .AssembleEquation(grid)
            .ApplyThirdConditions(thirdConditions)
            .ApplyFirstConditions(firstConditions)
            .BuildEquation();

        return equation;
    }

    public static Equation<SparseMatrix> AllConditionsSparseTest()
    {
        var grid = Grid;

        var fS = new RightPartParameter(FS, grid);
        var fC = new RightPartParameter(FC, grid);

        var localAssembler = new LocalAssembler
        (
            _massTemplateProvider,
            _stiffnessXTemplateProvider,
            _stiffnessYTemplateProvider,
            _stiffnessZTemplateProvider,
            _materialFactory,
            fS,
            fC
        );

        var globalAssembler = new GlobalAssembler<Node3D, SparseMatrix>
        (
            _matrixPortraitBuilder,
            localAssembler,
            _inserter,
            _gaussExcluder
        );

        var firstConditionsProvider =
            new FirstBoundaryProvider
            (
                grid,
                p => p.X + p.Y + p.Z,
                p => p.X - p.Y - p.Z
            );

        var secondConditionsProvider =
            new SecondBoundaryProvider(grid, _materialFactory, new SecondConditionMatrixTemplateProvider());

        var thirdConditionsProvider = new ThirdBoundaryProvider(firstConditionsProvider, secondConditionsProvider);

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

        return equation;
    }

    public static Equation<ProfileMatrix> FirstConditionsProfileTest()
    {
        var grid = Grid;

        var fS = new RightPartParameter(FS, grid);
        var fC = new RightPartParameter(FC, grid);

        var localAssembler = new LocalAssembler
        (
            _massTemplateProvider,
            _stiffnessXTemplateProvider,
            _stiffnessYTemplateProvider,
            _stiffnessZTemplateProvider,
            _materialFactory,
            fS,
            fC
        );

        var globalAssembler = new GlobalAssembler<Node3D, SparseMatrix>
        (
            _matrixPortraitBuilder,
            localAssembler,
            _inserter,
            _gaussExcluder
        );

        var firstConditionsProvider =
            new FirstBoundaryProvider
            (
                grid,
                p => p.X + p.Y + p.Z,
                p => p.X - p.Y - p.Z
            );

        var firstConditions =
            firstConditionsProvider
                .GetConditions
                (
                    new[] { 0, 0, 0, 0, 0, 0, },
                    new[] { Bound.Front, Bound.Back, Bound.Left, Bound.Right, Bound.Upper, Bound.Lower }
                );

        var equation = globalAssembler
            .AssembleEquation(grid)
            .ApplyFirstConditions(firstConditions)
            .BuildEquation();

        var profileMatrix = MatricesConverter.Convert(equation.Matrix);
        return new Equation<ProfileMatrix>(profileMatrix, equation.Solution.Clone(), equation.RightSide.Clone());
    }

    public static Equation<ProfileMatrix> FirstSecondConditionsProfileTest()
    {
        var grid = Grid;

        var fS = new RightPartParameter(FS, grid);
        var fC = new RightPartParameter(FC, grid);

        var localAssembler = new LocalAssembler
        (
            _massTemplateProvider,
            _stiffnessXTemplateProvider,
            _stiffnessYTemplateProvider,
            _stiffnessZTemplateProvider,
            _materialFactory,
            fS,
            fC
        );

        var globalAssembler = new GlobalAssembler<Node3D, SparseMatrix>
        (
            _matrixPortraitBuilder,
            localAssembler,
            _inserter,
            _gaussExcluder
        );

        var firstConditionsProvider =
            new FirstBoundaryProvider
            (
                grid,
                p => p.X + p.Y + p.Z,
                p => p.X - p.Y - p.Z
            );

        var secondConditionsProvider =
            new SecondBoundaryProvider(grid, _materialFactory, new SecondConditionMatrixTemplateProvider());

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
                .CreateConditions
                (
                    new[] { 0 },
                    new[] { Bound.Left },
                    p => -1,
                    p => -1
                )
                .CreateConditions
                (
                    new[] { 0 },
                    new[] { Bound.Right },
                    p => 1,
                    p => 1
                )
                .CreateConditions
                (
                    new[] { 0 },
                    new[] { Bound.Upper },
                    p => 1,
                    p => -1
                )
                .CreateConditions
                (
                    new[] { 0 },
                    new[] { Bound.Lower },
                    p => -1,
                    p => 1
                )
                .GetConditions();


        var equation = globalAssembler
            .AssembleEquation(grid)
            .ApplySecondConditions(secondConditions)
            .ApplyFirstConditions(firstConditions)
            .BuildEquation();

        var profileMatrix = MatricesConverter.Convert(equation.Matrix);
        return new Equation<ProfileMatrix>(profileMatrix, equation.Solution.Clone(), equation.RightSide.Clone());
    }

    public static Equation<ProfileMatrix> FirstThirdConditionsProfileTest()
    {
        var grid = Grid;

        var fS = new RightPartParameter(FS, grid);
        var fC = new RightPartParameter(FC, grid);

        var localAssembler = new LocalAssembler
        (
            _massTemplateProvider,
            _stiffnessXTemplateProvider,
            _stiffnessYTemplateProvider,
            _stiffnessZTemplateProvider,
            _materialFactory,
            fS,
            fC
        );

        var globalAssembler = new GlobalAssembler<Node3D, SparseMatrix>
        (
            _matrixPortraitBuilder,
            localAssembler,
            _inserter,
            _gaussExcluder
        );

        var firstConditionsProvider =
            new FirstBoundaryProvider
            (
                grid,
                p => p.X + p.Y + p.Z,
                p => p.X - p.Y - p.Z
            );

        var secondConditionsProvider =
            new SecondBoundaryProvider(grid, _materialFactory, new SecondConditionMatrixTemplateProvider());

        var thirdConditionsProvider = new ThirdBoundaryProvider(firstConditionsProvider, secondConditionsProvider);

        var firstConditions =
            firstConditionsProvider
                .GetConditions
                (
                    new[] { 0 },
                    new[] { Bound.Front }
                );

        var thirdConditions =
            thirdConditionsProvider
                .CreateConditions
                (
                    new[] { 0 },
                    new[] { Bound.Back },
                    p => 1,
                    p => -1,
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
                .GetConditions();


        var equation = globalAssembler
            .AssembleEquation(grid)
            .ApplyThirdConditions(thirdConditions)
            .ApplyFirstConditions(firstConditions)
            .BuildEquation();

        var profileMatrix = MatricesConverter.Convert(equation.Matrix);
        return new Equation<ProfileMatrix>(profileMatrix, equation.Solution.Clone(), equation.RightSide.Clone());
    }

    public static Equation<ProfileMatrix> AllConditionsProfileTest()
    {
        var grid = Grid;

        var fS = new RightPartParameter(FS, grid);
        var fC = new RightPartParameter(FC, grid);

        var localAssembler = new LocalAssembler
        (
            _massTemplateProvider,
            _stiffnessXTemplateProvider,
            _stiffnessYTemplateProvider,
            _stiffnessZTemplateProvider,
            _materialFactory,
            fS,
            fC
        );

        var globalAssembler = new GlobalAssembler<Node3D, SparseMatrix>
        (
            _matrixPortraitBuilder,
            localAssembler,
            _inserter,
            _gaussExcluder
        );

        var firstConditionsProvider =
            new FirstBoundaryProvider
            (
                grid,
                p => p.X + p.Y + p.Z,
                p => p.X - p.Y - p.Z
            );

        var secondConditionsProvider =
            new SecondBoundaryProvider(grid, _materialFactory, new SecondConditionMatrixTemplateProvider());

        var thirdConditionsProvider = new ThirdBoundaryProvider(firstConditionsProvider, secondConditionsProvider);

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
        return new Equation<ProfileMatrix>(profileMatrix, equation.Solution.Clone(), equation.RightSide.Clone());
    }

    public static Equation<SparseMatrix> BigTest()
    {
        var grid = Grid1000;

        var fS = new RightPartParameter(FS, grid);
        var fC = new RightPartParameter(FC, grid);

        var localAssembler = new LocalAssembler
        (
            _massTemplateProvider,
            _stiffnessXTemplateProvider,
            _stiffnessYTemplateProvider,
            _stiffnessZTemplateProvider,
            _materialFactory,
            fS,
            fC
        );

        var globalAssembler = new GlobalAssembler<Node3D, SparseMatrix>
        (
            _matrixPortraitBuilder,
            localAssembler,
            _inserter,
            _gaussExcluder
        );

        var firstConditions = FirstConditions1000Big;

        var equation = globalAssembler
            .AssembleEquation(grid)
            .ApplyFirstConditions(firstConditions)
            .BuildEquation();

        return equation;
    }
}