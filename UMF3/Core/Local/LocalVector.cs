﻿using UMF3.Core.Base;

namespace UMF3.Core.Local;

public class LocalVector
{
    public int[] Indexes { get; }
    public BaseVector Vector { get; }

    public int Count => Vector.Count;

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