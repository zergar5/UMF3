using UMF3.Core.Global;
using UMF3.SLAE;

namespace UMF3.Core.Converters;

public class MatricesConverter
{
    public static ProfileMatrix Convert(SparseMatrix sparseMatrix)
    {
        var matrix = sparseMatrix.Clone();
        var diagonal = matrix.Diagonal;
        var rowsIndexes = matrix.RowsIndexes;
        var columnsIndexes = matrix.ColumnsIndexes;
        var lowerValues = new List<double>();
        var upperValues = new List<double>();

        for (var i = 2; i < rowsIndexes.Length; i++)
        {
            var columns =
                new Span<int>(columnsIndexes, sparseMatrix.RowsIndexes[i - 1], sparseMatrix.RowsIndexes[i] - sparseMatrix.RowsIndexes[i - 1]).ToArray();

            var rowBegin = i - 1;

            for (var j = sparseMatrix.RowsIndexes[i - 1]; j < sparseMatrix.RowsIndexes[i]; j++)
            {
                if(sparseMatrix.LowerValues[j] < MethodsConfig.Eps 
                   && sparseMatrix.UpperValues[j] < MethodsConfig.Eps) continue;
                rowBegin = columnsIndexes[j];
                break;
            }
            
            rowsIndexes[i] = rowsIndexes[i - 1] + (i - 1 - rowBegin);

            for (var j = rowsIndexes[i - 1]; j < rowsIndexes[i]; j++, rowBegin++)
            {
                if (columns.Any(x => x == rowBegin))
                {
                    lowerValues.Add(sparseMatrix.LowerValues[sparseMatrix[i - 1, rowBegin]]);
                    upperValues.Add(sparseMatrix.UpperValues[sparseMatrix[i - 1, rowBegin]]);
                }
                else
                {
                    lowerValues.Add(0d);
                    upperValues.Add(0d);
                }
            }
        }

        return new ProfileMatrix(rowsIndexes, diagonal, lowerValues.ToArray(), upperValues.ToArray());
    }
}