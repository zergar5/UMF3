using UMF3.Core.Base;

namespace UMF3.FEM.Assembling;

public interface ITemplateMatrixProvider
{
    public BaseMatrix GetMatrix();
}