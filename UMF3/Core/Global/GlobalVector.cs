namespace KursachOneDim.Models.Global;

public struct GlobalVector
{
    public double[] Vector { get; set; }

    public GlobalVector()
    {
        Vector = Array.Empty<double>();
    }

    public GlobalVector(int size)
    {
        Vector = new double[size];
    }

    public GlobalVector(double[] vector)
    {
        Vector = vector;
    }

    public double this[int index]
    {
        get => Vector[index];
        set => Vector[index] = value;
    }

    public int Count => Vector.Length;

    public static GlobalVector operator *(double coefficient, GlobalVector localVector)
    {
        var vector = new GlobalVector(localVector.Count);

        for (var i = 0; i < vector.Count; i++)
        {
            vector[i] = coefficient * localVector[i];
        }

        return vector;
    }

    public double CalcNorm() => Math.Sqrt(ScalarProduct(this, this));

    public GlobalVector Clone()
    {
        var clone = new double[Count];
        Array.Copy(Vector, clone, Count);

        return new GlobalVector(clone);
    }

    public static double ScalarProduct(GlobalVector globalVector1, GlobalVector globalVector2)
    {
        var result = 0d;
        for (var i = 0; i < globalVector1.Count; i++)
        {
            result += globalVector1[i] * globalVector2[i];
        }
        return result;
    }

    public double ScalarProduct(GlobalVector vector)
    {
        return ScalarProduct(this, vector);
    }

    public static GlobalVector SumVectors(GlobalVector localVector1, GlobalVector localVector2)
    {
        if (localVector1.Count != localVector2.Count) throw new Exception("Can't sum vectors");

        for (var i = 0; i < localVector1.Count; i++)
        {
            localVector2[i] = localVector1[i] + localVector2[i];
        }

        return localVector2;
    }

    public static GlobalVector SubtractVectors(GlobalVector localVector1, GlobalVector localVector2)
    {
        if (localVector1.Count != localVector2.Count) throw new Exception("Can't sum vectors");

        for (var i = 0; i < localVector1.Count; i++)
        {
            localVector2[i] = localVector1[i] - localVector2[i];
        }

        return localVector2;
    }

    public IEnumerator<double> GetEnumerator() => ((IEnumerable<double>)Vector).GetEnumerator();
}