using UMF3.Core;

namespace GridGenerator;

public interface IGridBuilder<TPoint>
{
    public Grid<TPoint> Build();
}