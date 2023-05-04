namespace UMF3.Core.BoundaryConditions;

public readonly record struct SecondCondition(int[] GlobalNodeNumbers, double[] Theta);