namespace UMF3.Core.GridComponents;

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

    public (int[], double[]) GetBoundNodeIndexes(Bound bound) =>
        bound switch
        {
            Bound.Lower =>
            (new[]
            {
                NodesIndexes[0],
                NodesIndexes[1],
                NodesIndexes[2],
                NodesIndexes[3]
            },
            new[]
            {
                Length,
                Width
            }),
            Bound.Front =>
            (new[]
            {
                NodesIndexes[0],
                NodesIndexes[1],
                NodesIndexes[4],
                NodesIndexes[5]
            },
            new[]
            {
                Length,
                Height
            }),
            Bound.Back =>
            (new[]
            {
                NodesIndexes[2],
                NodesIndexes[3],
                NodesIndexes[6],
                NodesIndexes[7]
            },
            new[]
            {
                Length,
                Height
            }),
            Bound.Left =>
            (new[]
            {
                NodesIndexes[0],
                NodesIndexes[2],
                NodesIndexes[4],
                NodesIndexes[6]
            },
            new[]
            {
                Width,
                Height
            }),
            Bound.Right =>
            (new[]
            {
                NodesIndexes[1],
                NodesIndexes[3],
                NodesIndexes[5],
                NodesIndexes[7]
            },
            new[]
            {
                Width,
                Height
            }),
            Bound.Upper =>
            (new[]
            {
                NodesIndexes[4],
                NodesIndexes[5],
                NodesIndexes[6],
                NodesIndexes[7]
            },
            new[]
            {
                Length,
                Width
            }),
            _ => throw new ArgumentOutOfRangeException()
        };
}