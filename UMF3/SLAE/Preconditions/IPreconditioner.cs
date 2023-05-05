using UMF3.Core.Global;

namespace UMF3.SLAE.Preconditions;

public interface IPreconditioner<out TResult>
{
    public TResult Decompose(SparseMatrix globalMatrix);
}