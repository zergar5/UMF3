using UMF3.Core.Local;

namespace UMF3.Core.Global;

public class ProfileMatrix
{
    public double[] Diagonal { get; }
    public double[] LowerValues { get; }
    public double[] UpperValues { get; }
    public int[] RowsIndexes { get; }

    public int CountRows => Diagonal.Length;
    public int CountColumns => Diagonal.Length;

    public ProfileMatrix(int[] rowsIndexes, double[] diagonal, double[] lowerValues, double[] upperValues)
    {
        Diagonal = diagonal;
        LowerValues = lowerValues;
        UpperValues = upperValues;
        RowsIndexes = rowsIndexes;
    }

    public static GlobalVector operator *(ProfileMatrix matrix, GlobalVector vector)
    {
        var result = new GlobalVector(matrix.CountRows);

        for (var i = 0; i < matrix.CountRows; i++)
        {
            result[i] += matrix.Diagonal[i] * vector[i];

            var k = i - (matrix.RowsIndexes[i + 1] - matrix.RowsIndexes[i]);

            for (var j = matrix.RowsIndexes[i]; j < matrix.RowsIndexes[i + 1]; j++, k++)
            {
                result[i] += matrix.LowerValues[j] * vector[k];
                result[k] += matrix.UpperValues[j] * vector[i];
            }
        }

        return result;
    }

    public ProfileMatrix LU()
    {
        for (var i = 0; i < CountRows; i++)
        {
            var j = i - (RowsIndexes[i + 1] - RowsIndexes[i]);

            var sumD = 0d;

            for (var ij = RowsIndexes[i]; ij < RowsIndexes[i + 1]; ij++, j++)
            {
                var sumL = 0d;
                var sumU = 0d;

                var ik = i - (RowsIndexes[i + 1] - RowsIndexes[i]);
                var jk = j - (RowsIndexes[j + 1] - RowsIndexes[j]);

                var k = Math.Max(ik, jk);

                if (ik >= jk)
                {
                    jk = RowsIndexes[j] + (ik - jk);
                    ik = RowsIndexes[i];
                }
                else
                {
                    jk = RowsIndexes[j];
                    ik = RowsIndexes[i] + (jk - ik);
                }

                for (; k < j; k++, ik++, jk++)
                {
                    sumL += LowerValues[ik] * UpperValues[jk];
                    sumU += LowerValues[jk] * UpperValues[ik];
                }

                LowerValues[ij] -= sumL;
                UpperValues[ij] = (UpperValues[ij] - sumU) / Diagonal[j];

                sumD += LowerValues[ij] * UpperValues[ij];
            }

            Diagonal[i] -= sumD;
        }

        return this;
    }
}