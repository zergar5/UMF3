using KursachOneDim.Models.BoundaryConditions;
using KursachOneDim.Models.Global;

namespace KursachOneDim.Tools.Assemblers.BoundaryConditions;

public class GaussExclusion
{
    public static void Exclude(GlobalMatrix globalMatrix, GlobalVector globalVector, FirstCondition firstBoundaryCondition)
    {
        globalVector[firstBoundaryCondition.GlobalNodeNumber] = firstBoundaryCondition.U;
        globalMatrix.DI[firstBoundaryCondition.GlobalNodeNumber] = 1d;

        var rowStartIndex = firstBoundaryCondition.GlobalNodeNumber -
                            (globalMatrix.IG[firstBoundaryCondition.GlobalNodeNumber] + 1 -
                             globalMatrix.IG[firstBoundaryCondition.GlobalNodeNumber]);

        for (var i = globalMatrix.IG[firstBoundaryCondition.GlobalNodeNumber]; i < globalMatrix.IG[firstBoundaryCondition.GlobalNodeNumber + 1]; i++, rowStartIndex++)
        {
            globalVector[rowStartIndex] -= globalMatrix.GG[i] * firstBoundaryCondition.U;
            globalMatrix.GG[i] = 0d;
        }

        for (var i = firstBoundaryCondition.GlobalNodeNumber + 1; i < globalMatrix.N; i++)
        {
            rowStartIndex = i - (globalMatrix.IG[i + 1] - globalMatrix.IG[i]);

            if (rowStartIndex > firstBoundaryCondition.GlobalNodeNumber) break;

            globalVector[i] -= globalMatrix.GG[rowStartIndex] * firstBoundaryCondition.U;
            globalMatrix.GG[rowStartIndex] = 0d;
        }
    }
}