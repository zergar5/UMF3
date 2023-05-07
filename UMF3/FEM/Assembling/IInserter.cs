using UMF3.Core.Global;
using UMF3.Core.Local;

namespace UMF3.FEM.Assembling;

public interface IInserter<in TMatrix>
{
    public void InsertMatrix(TMatrix globalMatrix, LocalMatrix localMatrix);
    public void InsertVector(GlobalVector vector, LocalVector localVector);
}