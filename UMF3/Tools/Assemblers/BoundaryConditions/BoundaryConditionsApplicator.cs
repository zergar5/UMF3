using KursachOneDim.Models.BoundaryConditions;
using KursachOneDim.Models.Global;

namespace KursachOneDim.Tools.Assemblers.BoundaryConditions;

public class BoundaryConditionsApplicator
{
    public static void ApplyFirstCondition(GlobalMatrix globalMatrix, GlobalVector globalVector, FirstCondition firstBoundaryCondition)
    {
        GaussExclusion.Exclude(globalMatrix, globalVector, firstBoundaryCondition);
    }

    public static void ApplySecondCondition(GlobalVector globalVector, SecondCondition secondBoundaryCondition)
    {
        globalVector[secondBoundaryCondition.GlobalNodeNumber] += secondBoundaryCondition.Theta;
    }

    public static void ApplyThirdCondition(GlobalMatrix globalMatrix, GlobalVector globalVector, ThirdCondition thirdBoundaryCondition)
    {
        globalVector[thirdBoundaryCondition.GlobalNodeNumber] += thirdBoundaryCondition.U * thirdBoundaryCondition.Beta;

        globalMatrix.DI[thirdBoundaryCondition.GlobalNodeNumber] += thirdBoundaryCondition.Beta;
    }
}