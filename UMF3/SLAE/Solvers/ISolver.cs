using UMF3.Core.Global;

namespace UMF3.SLAE.Solvers;

public interface ISolver<TMatrix>
{
    public GlobalVector Solve(Equation<TMatrix> equation);
}