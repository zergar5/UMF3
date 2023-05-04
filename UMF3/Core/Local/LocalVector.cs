namespace KursachOneDim.Models.Local;

public record struct LocalVector(double[] Vector)
{
    public LocalVector() : this(Array.Empty<double>()) { }

    public LocalVector(int size) : this(new double[size]) { }

    public double this[int index]
    {
        get => Vector[index];
        set => Vector[index] = value;
    }

    public int Count => Vector.Length;

    public static LocalVector MultiplyVectorOnNumber(double number, LocalVector localVector)
    {
        for (var i = 0; i < localVector.Count; i++)
        {
            localVector[i] *= number;
        }

        return localVector;
    }

    public IEnumerator<double> GetEnumerator() => ((IEnumerable<double>)Vector).GetEnumerator();
}