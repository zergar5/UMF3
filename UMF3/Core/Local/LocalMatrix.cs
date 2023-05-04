﻿using UMF3.Core.Base;

namespace UMF3.Core.Local;

public class LocalMatrix
{
    public int[] Indexes { get; }
    public BaseMatrix Matrix { get; }

    public LocalMatrix(int[] indexes, BaseMatrix matrix)
    {
        Matrix = matrix;
        Indexes = indexes;
    }

    public double this[int i, int j]
    {
        get => Matrix[i, j];
        set => Matrix[i, j] = value;
    }
}