namespace UMF3.Core.BoundaryConditions;

public readonly record struct ThirdCondition(int[] GlobalNodeNumber, double Beta, double[] U);