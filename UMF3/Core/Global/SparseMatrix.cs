namespace UMF3.Core.Global;

public class SparseMatrix
{
    public double[] Diagonal { get; set; }
    public double[] LowerValues { get; set; }
    public double[] UpperValues { get; set; }
    public int[] RowsIndexes { get; }
    public int[] ColumnsIndexes { get; }

    public SparseMatrix(int[] rowsIndexes, int[] columnsIndexes)
    {
        Diagonal = new double[rowsIndexes.Length - 1];
        LowerValues = new double[rowsIndexes[^1]];
        UpperValues = new double[rowsIndexes[^1]];
        RowsIndexes = rowsIndexes;
        ColumnsIndexes = columnsIndexes;
    }

    public static GlobalVector operator *(SparseMatrix matrix, GlobalVector vector)
    {
        var rowsIndexes = matrix.RowsIndexes;
        var columnsIndexes = matrix.ColumnsIndexes;
        var di = matrix.Diagonal;
        var lowerValues = matrix.LowerValues;
        var upperValues = matrix.UpperValues;
        
        var result = new GlobalVector(matrix.Diagonal.Length);

        for (var i = 0; i < matrix.Diagonal.Length; i++)
        {
            result[i] += di[i] * vector[i];

            for (var j = rowsIndexes[i]; j < rowsIndexes[i + 1]; j++)
            {
                result[i] += lowerValues[j] * vector[columnsIndexes[j]];
                result[columnsIndexes[j]] += upperValues[j] * vector[i];
            }
        }

        return result;
    }
}