using UMF3.Core.GridComponents;

namespace UMF3.ThreeDimensional.Assembling.Local;

public class LocalBasisFunction
{
    private readonly Func<double, double> _xFunction;
    private readonly Func<double, double> _yFunction;
    private readonly Func<double, double> _zFunction;

    public LocalBasisFunction(Func<double, double> xFunction, Func<double, double> yFunction, Func<double, double> zFunction)
    {
        _xFunction = xFunction;
        _yFunction = yFunction;
        _zFunction = zFunction;
    }

    public double CalcFunction(Node3D node)
    {
        return _xFunction(node.X) * _yFunction(node.Y) * _zFunction(node.Z);
    }
}