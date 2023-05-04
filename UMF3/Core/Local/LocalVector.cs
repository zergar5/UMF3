namespace UMF3.Core.Local;

public class LocalVector
{
    public int[] Indexes { get; }
    public BaseVector Vector { get; }

    public LocalVector(int[] indexes, BaseVector vector)
    {
        Indexes = indexes;
        Vector = vector;
    }

    public double this[int index]
    {
        get => Vector[index];
        set => Vector[index] = value;
    }

    public IEnumerator<double> GetEnumerator() => ((IEnumerable<double>)Vector).GetEnumerator();
}