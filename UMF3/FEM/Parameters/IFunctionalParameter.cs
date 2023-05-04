namespace UMF3.FEM.Parameters;

public interface IFunctionalParameter
{
    public double Calculate(int nodeIndex);
}