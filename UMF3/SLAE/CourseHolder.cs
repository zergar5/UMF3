using System.Text;
using System.Xml.Linq;

namespace UMF3.SLAE;

public class CourseHolder
{
    public static void GetInfo(int iteration, double residual)
    {
        Console.Write($"Iteration: {iteration}, residual: {residual:E14}                                   \r");
    }

    //public static void WriteSolution(Node node, double result)
    //{
    //    Console.WriteLine($"Function value at the point ({node.X}, {node.Y}) = {result.ToString("0.00000000000000e+00", _culture)}");
    //}

    //public static void WriteAreaInfo()
    //{
    //    Console.WriteLine("Point not in area");
    //}
}