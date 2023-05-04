namespace UMF3.Core.BoundaryConditions;

public readonly record struct FirstCondition(int[] GlobalNodeNumbers, double[] Us);