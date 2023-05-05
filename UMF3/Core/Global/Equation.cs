namespace UMF3.Core.Global;

public record struct Equation<TMatrix>(TMatrix A, GlobalVector Q, GlobalVector B);