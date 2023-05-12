using UMF3.Core.Global;
using UMF3.FEM;
using UMF3.SLAE.Preconditions.LU;

namespace UMF3.SLAE.Solvers;

public class BCGSTAB : ISolver<SparseMatrix>
{
    private readonly LUPreconditioner _luPreconditioner;
    private readonly LUSparse _luSparse;
    private SparseMatrix _preconditionMatrix;
    private GlobalVector _r0;
    private GlobalVector _r;
    private GlobalVector _z;

    public BCGSTAB(LUPreconditioner luPreconditioner, LUSparse luSparse)
    {
        _luPreconditioner = luPreconditioner;
        _luSparse = luSparse;
    }

    private void PrepareProcess(Equation<SparseMatrix> equation)
    {
        _preconditionMatrix = _luPreconditioner.Decompose(equation.Matrix);
        _r0 = _luSparse.CalcY(_preconditionMatrix, GlobalVector.Subtract(equation.RightSide, equation.Matrix * equation.Solution));
        _z = _r0;
        _r = _r0;
    }

    public GlobalVector Solve(Equation<SparseMatrix> equation)
    {
        PrepareProcess(equation);
        IterationProcess(equation);
        return equation.Solution;
    }

    private void IterationProcess(Equation<SparseMatrix> equation)
    {
        Console.WriteLine("BCGSTAB");

        var residual = _r0.Norm / equation.RightSide.Norm;

        for (var i = 1; i <= MethodsConfig.MaxIterations && residual > Math.Pow(MethodsConfig.Eps, 2); i++)
        {
            var scalarRR = GlobalVector.ScalarProduct(_r, _r0);

            var LAUz = _luSparse.CalcY(_preconditionMatrix, equation.Matrix * _luSparse.CalcX(_preconditionMatrix, _z));

            var alpha = scalarRR / GlobalVector.ScalarProduct(_r0, LAUz);

            var p = GlobalVector.Subtract(_r, alpha * LAUz);

            var LAUp = _luSparse.CalcY(_preconditionMatrix, equation.Matrix * _luSparse.CalcX(_preconditionMatrix, p));

            var gamma = GlobalVector.ScalarProduct(p, LAUp) / GlobalVector.ScalarProduct(LAUp, LAUp);

            GlobalVector.Sum(equation.Solution, GlobalVector.Sum(GlobalVector.Multiply(alpha, _z), gamma * p));

            var rNext = GlobalVector.Subtract(p, gamma * LAUp);

            var beta = alpha * GlobalVector.ScalarProduct(rNext, _r0) / (gamma * scalarRR);

            _z = GlobalVector.Sum
            (
                 GlobalVector.Subtract(GlobalVector.Multiply(beta, _r),
                 GlobalVector.Multiply(beta * gamma, LAUz)),
                 rNext
            );

            _r = rNext;

            residual = _r.Norm / equation.RightSide.Norm;

            CourseHolder.GetInfo(i, residual);
        }

        _luSparse.CalcXWithoutMemory(_preconditionMatrix, equation.Solution);

        Console.WriteLine();
    }
}