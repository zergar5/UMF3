namespace KursachOneDim.Tools.Providers;

public class LinearFunctionsProvider
{
    public static Func<double, double> CreateFirstFunction(double rightCoordinate, double h)
    {
        return coordinate => (rightCoordinate - coordinate) / h;
    }

    public static Func<double, double> CreateSecondFunction(double leftCoordinate, double h)
    {
        return coordinate => (coordinate - leftCoordinate) / h;
    }
}