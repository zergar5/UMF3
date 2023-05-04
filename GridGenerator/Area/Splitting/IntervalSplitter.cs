using GridGenerator.Area.Core;

namespace GridGenerator.Area.Splitting;

public interface IntervalSplitter
{
    public IEnumerable<double> EnumerateValues(Interval interval);
    public int Steps { get; }
}