using UMF3.Core.BoundaryConditions;
using UMF3.Core.Global;
using UMF3.FEM.Assembling.Global;

namespace UMF3.ThreeDimensional.Assembling.Global;

public class GaussExcluder : IGaussExcluder<SparseMatrix>
{
    public void Exclude(Equation<SparseMatrix> equation, FirstCondition condition)
    {
        for (var i = 0; i < condition.Values.Length; i++)
        {
            equation.RightSide[condition.NodesIndexes[i]] = condition.Values[i];
            equation.Matrix.Diagonal[condition.NodesIndexes[i]] = 1d;

            for (var j = equation.Matrix.RowsIndexes[condition.NodesIndexes[i]];
                 j < equation.Matrix.RowsIndexes[condition.NodesIndexes[i] + 1];
                 j++)
            {
                equation.Matrix.LowerValues[j] = 0d;
            }

            for (var j = condition.NodesIndexes[i] + 1; j < equation.Matrix.CountRows; j++)
            {
                var elementIndex = equation.Matrix[j, condition.NodesIndexes[i]];

                if (elementIndex == -1) continue;
                equation.Matrix.UpperValues[elementIndex] = 0;
            }
        }
    }
}