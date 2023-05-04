using GridGenerator;
using GridGenerator.Area.Splitting;

var gridBuilder3D = new GridBuilder3D();
var grid = gridBuilder3D
    .SetXAxis(new AxisSplitParameter(
            new[] { 0d, 1d },
            new ProportionalSplitter(2, 1.5)
        )
    )
    .SetYAxis(new AxisSplitParameter(
            new[] { 0d, 1d },
            new ProportionalSplitter(2, 1.5)
        )
    )
    .SetZAxis(new AxisSplitParameter(
            new[] { 0d, 1d },
            new ProportionalSplitter(2, 1.5)
        )
    )
    .Build();

Console.WriteLine();