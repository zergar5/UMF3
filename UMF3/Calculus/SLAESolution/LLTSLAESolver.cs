using KursachOneDim.Models.Global;

namespace KursachOneDim.Calculus.SLAESolution;

public class LLTSLAESolver
{
    private readonly LLTDecomposer _lltDecomposer;

    public LLTSLAESolver(LLTDecomposer lltDecomposer)
    {
        _lltDecomposer = lltDecomposer;
    }

    public void SolveSLAE(GlobalMatrix globalMatrix, GlobalVector q, GlobalVector b)
    {
        globalMatrix = _lltDecomposer.Decompose(globalMatrix);
        var y = CalcY(globalMatrix, q, b);
        CalcX(globalMatrix, y);
    }

    private static GlobalVector CalcY(GlobalMatrix globalMatrix, GlobalVector q, GlobalVector b)
    {
        var n = globalMatrix.N;
        var ig = globalMatrix.IG;
        var gg = globalMatrix.GG;
        var di = globalMatrix.DI;

        var y = q;

        for (var i = 0; i < n; i++)
        {
            var sum = 0d;

            var k = i - (ig[i + 1] - ig[i]);

            for (var j = ig[i]; j < ig[i + 1]; j++, k++)
            {
                sum += gg[j] * y[k];
            }
            y[i] = (b[i] - sum) / di[i];
        }

        return y;
    }

    private static void CalcX(GlobalMatrix sparseMatrix, GlobalVector y)
    {
        var n = sparseMatrix.N;
        var ig = sparseMatrix.IG;
        var gg = sparseMatrix.GG;
        var di = sparseMatrix.DI;

        var x = y;

        for (var i = n - 1; i >= 0; i--)
        {
            x[i] /= di[i];

            var k = i - 1;

            for (var j = ig[i + 1] - 1; j >= ig[i]; j--, k--)
            {
                x[k] -= gg[j] * x[i];
            }
        }
    }
}