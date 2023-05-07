using GridGenerator;
using GridGenerator.Area.Splitting;
using UMF3.ThreeDimensional.Assembling;

var gridBuilder3D = new GridBuilder3D();
var grid = gridBuilder3D
    .SetXAxis(new AxisSplitParameter(
            new[] { 0d, 1d },
            new ProportionalSplitter(1, 1.5)
        )
    )
    .SetYAxis(new AxisSplitParameter(
            new[] { 0d, 1d },
            new ProportionalSplitter(1, 1.5)
        )
    )
    .SetZAxis(new AxisSplitParameter(
            new[] { 0d, 1d },
            new ProportionalSplitter(1, 1.5)
        )
    )
    .Build();

var matrixPortraitBuilder = new MatrixPortraitBuilder();
var matrix = matrixPortraitBuilder.Build(grid);

Console.WriteLine();