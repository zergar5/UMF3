using UMF3.Core.GridComponents;
using UMF3.Core.Local;

namespace UMF3.FEM.Assembling;

public interface ILocalAssembler
{
    public LocalMatrix AssembleMatrix(Element element);
    public LocalVector AssembleRightSide(Element element);
}