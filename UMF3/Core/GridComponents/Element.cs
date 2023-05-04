using KursachOneDim.Models.Local;
using KursachOneDim.Tools.Providers;

namespace KursachOneDim.Models.GridComponents;

public class Element
{
    public int[] NodesIndexes { get; }
    public int MaterialId { get; }
    public double Length { get; }
    public double Width { get; }
    public double Height { get; }

    public IEnumerator<int> GetEnumerator() => ((IEnumerable<int>)NodesIndexes).GetEnumerator();

    public Element(int[] nodesIndexes, double length, double width, double height, int materialId)
    {
        NodesIndexes = nodesIndexes;
        Length = length;
        Width = width;
        Height = height;
        MaterialId = materialId;
    }
}