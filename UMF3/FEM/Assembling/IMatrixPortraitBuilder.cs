using UMF3.Core;
using UMF3.Core.GridComponents;

namespace UMF3.FEM.Assembling;

public interface IMatrixPortraitBuilder<TNode, out TMatrix>
{
    TMatrix Build(Grid<TNode> grid);
}