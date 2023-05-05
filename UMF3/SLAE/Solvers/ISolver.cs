using UMF3.Core.Global;

namespace UMF3.SLAE.Solvers;

public interface ISolver
{
    public GlobalVector Solve(Equation equation);
}