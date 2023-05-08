using UMF3.Core.GridComponents;

namespace UMF3.FEM;

public class CourseHolder
{
    public static void GetInfo(int iteration, double residual)
    {
        Console.Write($"Iteration: {iteration}, residual: {residual:E14}                                   \r");
    }

    public static void WriteSolution(Node3D point, double sValue, double cValue)
    {
        Console.WriteLine($"({point.X},{point.Y},{point.Z}) {sValue:E14} {cValue:E14}");
    }

    public static void WriteAreaInfo()
    {
        Console.WriteLine("Point not in area");
    }
}