using UMF3.Core.Global;

namespace KursachOneDim.Calculus.SLAESolution;

public class LLTDecomposer
{
    public GlobalMatrix Decompose(GlobalMatrix globalMatrix)
    {
        var n = globalMatrix.N;
        var ig = globalMatrix.IG;
        var gg = globalMatrix.GG;
        var di = globalMatrix.DI;

        for (var i = 0; i < n; i++)
        {
            var j = i - (ig[i + 1] - ig[i]);

            var sumD = 0d;

            for (var ij = ig[i]; ij < ig[i + 1]; ij++, j++)
            {
                var sumL = 0d;

                var ik = i - (ig[i + 1] - ig[i]);
                var jk = j - (ig[j + 1] - ig[j]);

                var k = Math.Max(ik, jk);

                if (ik >= jk)
                {
                    jk = ig[j] + (ik - jk);
                    ik = ig[i];
                }
                else
                {
                    jk = ig[j];
                    ik = ig[i] + (jk - ik);
                }

                for (; k < j; k++, ik++, jk++)
                {
                    sumL += gg[ik] * gg[jk];
                }

                gg[ij] = (gg[ij] - sumL) / di[j];
                sumD += gg[ij] * gg[ij];
            }

            di[i] = Math.Sqrt(di[i] - sumD);
        }

        return globalMatrix;
    }
}