namespace KursachOneDim.Models.Global;

public record struct Equation(GlobalMatrix A, GlobalVector Q, GlobalVector B);