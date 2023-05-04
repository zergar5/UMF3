using GridGenerator.Area.Splitting;
using UMF3.Core;
using UMF3.Core.GridComponents;

namespace GridGenerator;

public class GridBuilder3D : IGridBuilder<Node3D>
{
    private AxisSplitParameter? _xAxisSplitParameter;
    private AxisSplitParameter? _yAxisSplitParameter;
    private AxisSplitParameter? _zAxisSplitParameter;
    private int[]? _materialsId;

    private int GetTotalXElements => _xAxisSplitParameter.Splitters.Sum(x => x.Steps);
    private int GetTotalYElements => _yAxisSplitParameter.Splitters.Sum(y => y.Steps);
    private int GetTotalZElements => _zAxisSplitParameter.Splitters.Sum(z => z.Steps);

    public GridBuilder3D SetXAxis(AxisSplitParameter splitParameter)
    {
        _xAxisSplitParameter = splitParameter;
        return this;
    }

    public GridBuilder3D SetYAxis(AxisSplitParameter splitParameter)
    {
        _yAxisSplitParameter = splitParameter;
        return this;
    }

    public GridBuilder3D SetZAxis(AxisSplitParameter splitParameter)
    {
        _zAxisSplitParameter = splitParameter;
        return this;
    }

    public GridBuilder3D SetMaterials(int[] materialsId)
    {
        _materialsId = materialsId;
        return this;
    }

    public Grid<Node3D> Build()
    {
        if (_xAxisSplitParameter == null || _yAxisSplitParameter == null || _zAxisSplitParameter == null)
            throw new ArgumentNullException();

        var totalXElements = GetTotalXElements;
        var totalYElements = GetTotalYElements;

        var totalNodes = GetTotalNodes();
        var totalElements = GetTotalElements();

        var nodes = new Node3D[totalNodes];
        var elements = new Element[totalElements];

        _materialsId ??= new int[totalElements];

        var i = 0;

        foreach (var (zSection, zSplitter) in _zAxisSplitParameter.SectionWithParameter)
        {
            var zValues = zSplitter.EnumerateValues(zSection);
            if (i > 0) zValues = zValues.Skip(1);

            foreach (var z in zValues)
            {
                var j = 0;

                foreach (var (ySection, ySplitter) in _yAxisSplitParameter.SectionWithParameter)
                {
                    var yValues = ySplitter.EnumerateValues(ySection);
                    if (j > 0) yValues = yValues.Skip(1);

                    foreach (var y in yValues)
                    {
                        var k = 0;

                        foreach (var (xSection, xSplitter) in _xAxisSplitParameter.SectionWithParameter)
                        {
                            var xValues = xSplitter.EnumerateValues(xSection);
                            if (k > 0) xValues = xValues.Skip(1);

                            foreach (var x in xValues)
                            {
                                var nodeIndex = k + j * (totalXElements + 1) +
                                                i * (totalXElements + 1) * (totalYElements + 1);

                                nodes[nodeIndex] = new Node3D(x, y, z);

                                if (i > 0 && j > 0 && k > 0)
                                {
                                    var elementIndex = k - 1 + (j - 1) * totalXElements + (i - 1) * totalXElements * totalYElements;
                                    var nodesIndexes = GetCurrentElementIndexes(i - 1, j - 1, k - 1);

                                    elements[elementIndex] = new Element(
                                        nodesIndexes,
                                        nodes[nodesIndexes[1]].X - nodes[nodesIndexes[0]].X,
                                        nodes[nodesIndexes[2]].Y - nodes[nodesIndexes[0]].Y,
                                        nodes[nodesIndexes[4]].Z - nodes[nodesIndexes[0]].Z,
                                        _materialsId[elementIndex]
                                        );
                                }

                                k++;
                            }
                        }

                        j++;
                    }
                }

                i++;
            }
        }

        return new Grid<Node3D>(nodes, elements);

    }

    private int GetTotalNodes()
    {
        return (GetTotalXElements + 1) * (GetTotalYElements + 1) * (GetTotalZElements + 1);
    }

    private int GetTotalElements()
    {
        return GetTotalXElements * GetTotalYElements * GetTotalZElements;
    }

    private int[] GetCurrentElementIndexes(int i, int j, int k)
    {
        var totalXElements = GetTotalXElements;
        var totalYElements = GetTotalYElements;

        var indexes = new[]
        {
            k + j * (totalXElements + 1) + i * (totalXElements + 1) * (totalYElements + 1),
            k + 1  + j * (totalXElements + 1) + i * (totalXElements + 1) * (totalYElements + 1),
            k + (j + 1) * (totalXElements + 1) + i * (totalXElements + 1) * (totalYElements + 1),
            k + 1 + (j + 1) * (totalXElements + 1) + i * (totalXElements + 1) * (totalYElements + 1),
            k + j * (totalXElements + 1) + (i + 1) * (totalXElements + 1) * (totalYElements + 1),
            k + 1 + j * (totalXElements + 1) + (i + 1) * (totalXElements + 1) * (totalYElements + 1),
            k + (j + 1) * (totalXElements + 1) + (i + 1) * (totalXElements + 1) * (totalYElements + 1),
            k + 1 + (j + 1) * (totalXElements + 1) + (i + 1) * (totalXElements + 1) * (totalYElements + 1)
        };

        return indexes;
    }
}