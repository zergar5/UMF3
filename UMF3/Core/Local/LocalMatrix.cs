namespace KursachOneDim.Models.Local;

public record struct LocalMatrix(double[,] Matrix)
{
    public LocalMatrix() : this(new double[0, 0]) { }

    public LocalMatrix(int n) : this(new double[n, n]) { }

    public double this[int i, int j]
    {
        get => Matrix[i, j];
        set => Matrix[i, j] = value;
    }

    public int CountRows()
    {
        return Matrix.GetLength(0);
    }

    public int CountColumns()
    {
        return Matrix.GetLength(1);
    }

    public static LocalMatrix operator *(double coefficient, LocalMatrix matrix)
    {
        var localMatrix = new LocalMatrix(matrix.CountRows());

        for (var i = 0; i < localMatrix.CountRows(); i++)
        {
            for (var j = 0; j < localMatrix.CountColumns(); j++)
            {
                localMatrix[i, j] = coefficient * matrix[i, j];
            }
        }

        return localMatrix;
    }

    public static LocalVector operator *(LocalMatrix matrix, LocalVector vector)
    {
        var localVector = new LocalVector(vector.Count);

        if (matrix.CountRows() != vector.Count)
        {
            throw new Exception("Can't multiply matrix");
        }

        for (var i = 0; i < matrix.CountRows(); i++)
        {
            for (var j = 0; j < matrix.CountColumns(); j++)
            {
                localVector[i] += matrix[i, j] * vector[j];
            }
        }

        return localVector;
    }

    public static LocalMatrix SumLocalMatrix(LocalMatrix localMatrix1, LocalMatrix localMatrix2)
    {
        if (localMatrix1.CountRows() != localMatrix2.CountRows() && localMatrix1.CountColumns() != localMatrix2.CountColumns())
        {
            throw new Exception("Can't sum matrix");
        }

        for (var i = 0; i < localMatrix1.CountRows(); i++)
        {
            for (var j = 0; j < localMatrix1.CountColumns(); j++)
            {
                localMatrix1[i, j] += localMatrix2[i, j];
            }
        }

        return localMatrix1;
    }
}