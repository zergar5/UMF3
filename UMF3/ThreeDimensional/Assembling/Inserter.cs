using UMF3.Core.Base;
using UMF3.Core.Global;
using UMF3.Core.Local;
using UMF3.FEM.Assembling;

namespace UMF3.ThreeDimensional.Assembling;

public class Inserter : IInserter<SparseMatrix>
{
    public void InsertMatrix(SparseMatrix globalMatrix, LocalMatrix localMatrix)
    {
        var nodesIndexes = localMatrix.Indexes;

        for (var i = 0; i < nodesIndexes.Length; i++)
        {
            for (var j = 0; j < i; j++)
            {
                var elementIndex = Array.IndexOf(globalMatrix.ColumnsIndexes, nodesIndexes[j], globalMatrix.RowsIndexes[nodesIndexes[i]],
                    globalMatrix.RowsIndexes[nodesIndexes[i] + 1] - globalMatrix.RowsIndexes[nodesIndexes[i]]);

                globalMatrix.LowerValues[elementIndex] += localMatrix[i, j];
                globalMatrix.UpperValues[elementIndex] += localMatrix[j, i];
            }

            globalMatrix.Diagonal[nodesIndexes[i]] += localMatrix[i, i];
        }
    }

    public void InsertVector(GlobalVector globalVector, LocalVector localVector)
    {
        for (var i = 0; i < localVector.Count; i++)
        {
            globalVector[localVector.Indexes[i]] += localVector[i];
        }
    }
}