using KursachOneDim.Models.BoundaryConditions;

namespace KursachOneDim.Tools.Providers;

public class BoundaryConditionsListsProvider
{
    public static List<FirstCondition> CreateFirstBoundaryConditionsList(List<int> globalNodesNumbersList, List<double> usList)
    {
        var list = new List<FirstCondition>(globalNodesNumbersList.Count);

        list.AddRange(globalNodesNumbersList.Select((globalNodesNumbers, i) => new FirstCondition(globalNodesNumbers, usList[i])));

        return list;
    }

    public static List<SecondCondition> CreateSecondBoundaryConditionsList(List<int> globalNodesNumbersList, List<double> thetasList)
    {
        var list = new List<SecondCondition>(globalNodesNumbersList.Count);

        list.AddRange(globalNodesNumbersList.Select((globalNodesNumbers, i) => new SecondCondition(globalNodesNumbers, thetasList[i])));

        return list;
    }

    public static List<ThirdCondition> CreateThirdBoundaryConditionsList(List<int> globalNodesNumbersList, List<double> betasList, List<double> usList)
    {
        var list = new List<ThirdCondition>(globalNodesNumbersList.Count);

        list.AddRange(globalNodesNumbersList.Select((globalNodesNumbers, i) => new ThirdCondition(globalNodesNumbers, betasList[i], usList[i])));

        return list;
    }
}