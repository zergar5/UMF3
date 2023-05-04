using UMF3.Core.Global;
using UMF3.Core.Local;

namespace UMF3.FEM.Assembling;

public interface Inserter<in TMatrix>
{
    public void InsertVector(GlobalVector vector, BaseVector localVector);
    public void InsertMatrix(TMatrix matrix, LocalMatrix localMatrix);
}