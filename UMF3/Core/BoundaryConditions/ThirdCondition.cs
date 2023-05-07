using UMF3.Core.Local;

namespace UMF3.Core.BoundaryConditions;

public readonly record struct ThirdCondition(LocalMatrix Matrix, LocalVector Vector);