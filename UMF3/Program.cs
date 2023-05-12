using System.Globalization;
using UMF3;
using UMF3.Core.GridComponents;
using UMF3.FEM;
using UMF3.SLAE.Preconditions.LU;
using UMF3.SLAE.Solvers;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

var luPreconditioner = new LUPreconditioner();
var linearFunctionsProvider = new LinearFunctionsProvider();

var sparseSolver1 = new LOS(luPreconditioner, new LUSparse(luPreconditioner));
var sparseSolver2 = new BCGSTAB(luPreconditioner, new LUSparse(luPreconditioner));
var profileSolver = new LUProfile();

var equation = Tests.AllConditionsProfileTest();

var solution = profileSolver.Solve(equation);

var femSolution = new FEMSolution(Tests.Grid, solution, linearFunctionsProvider);
var error = femSolution.CalcError(p => p.X + p.Y + p.Z, p => p.X - p.Y - p.Z);

Console.WriteLine(error);