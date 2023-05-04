using KursachOneDim.Models.GridComponents;

namespace KursachOneDim.Models.Global;

public record struct GlobalMatrix(int N, double[] DI, double[] GG, int[] IG)
{


    public static GlobalVector operator *(GlobalMatrix globalMatrix, GlobalVector globalVector)
    {
        var n = globalMatrix.N;
        var ig = globalMatrix.IG;
        var gg = globalMatrix.GG;
        var di = globalMatrix.DI;

        var result = new GlobalVector(n);

        for (var i = 0; i < n; i++)
        {
            result[i] += di[i] * globalVector[i];

            var k = i - (ig[i + 1] - ig[i]);

            for (var j = ig[i]; j < ig[i + 1]; j++, k++)
            {
                result[i] += gg[j] * globalVector[k];
                result[k] += gg[j] * globalVector[i];
            }
        }

        return result;
    }
}