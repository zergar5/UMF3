using UMF3.Core.Global;
using UMF3.Core.Local;

namespace UMF3.SLAE.Preconditions.LU;

public class LUPreconditioner : IPreconditioner<SparseMatrix>
{
    public SparseMatrix Decompose(SparseMatrix globalMatrix)
    {
        //var n = globalMatrix.N;
        //var cIG = new int[n + 1];
        //Array.Copy(globalMatrix.IG, cIG, n + 1);
        //var cJG = new int[cIG[n]];
        //Array.Copy(globalMatrix.JG, cJG, cIG[n]);
        //var cGG = new double[cIG[n]];
        //Array.Copy(globalMatrix.GG, cGG, cIG[n]);
        //var cDI = new double[n];
        //Array.Copy(globalMatrix.DI, cDI, n);

        //for (var i = 0; i < n; i++)
        //{
        //    var sumD = 0.0;
        //    for (var j = cIG[i]; j < cIG[i + 1]; j++)
        //    {
        //        var sumIPrev = 0d;
        //        for (var k = cIG[i]; k < j; k++)
        //        {
        //            var iPrev = i - cJG[j];
        //            var kPrev = Array.IndexOf(cJG, cJG[k], cIG[i - iPrev], cIG[i - iPrev + 1] - cIG[i - iPrev]);
        //            if (kPrev != -1)
        //            {
        //                sumIPrev += cGG[k] * cGG[kPrev];
        //            }
        //        }
        //        cGG[j] = (cGG[j] - sumIPrev) / cDI[cJG[j]];
        //        sumD += cGG[j] * cGG[j];
        //    }
        //    cDI[i] = Math.Sqrt(cDI[i] - sumD);
        //}

        //return new GlobalMatrix(n, cDI, cGG, cJG, cIG);
        throw new NotImplementedException();
    }
}