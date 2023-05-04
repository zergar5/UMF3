using UMF3.Core.GridComponents;

namespace UMF3.FEM.Assembling;

public interface IMatrixPortraitBuilder<out TMatrix>
{
    TMatrix Build(IEnumerable<Element> elements, int nodesCount);
}