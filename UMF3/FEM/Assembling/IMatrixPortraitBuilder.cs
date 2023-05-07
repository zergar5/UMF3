using UMF3.Core;

namespace UMF3.FEM.Assembling;

public interface IMatrixPortraitBuilder<TNode, out TMatrix>
{
    TMatrix Build(Grid<TNode> grid);
}