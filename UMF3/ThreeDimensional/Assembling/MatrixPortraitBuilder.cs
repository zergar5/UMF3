using UMF3.Core;
using UMF3.Core.Global;
using UMF3.Core.GridComponents;
using UMF3.FEM.Assembling;

namespace UMF3.ThreeDimensional.Assembling;

public class MatrixPortraitBuilder : IMatrixPortraitBuilder<Node3D, SparseMatrix>
{
    private List<SortedSet<int>> _adjacencyList = null!;

    public SparseMatrix Build(Grid<Node3D> grid)
    {
        BuildAdjacencyList(grid);

        var amount = 0;
        var buf = _adjacencyList.Select(nodeSet => amount += nodeSet.Count).ToList();
        buf.Insert(0, 0);

        var rowsIndexes = buf.ToArray();
        var columnsIndexes = _adjacencyList.SelectMany(nodeList => nodeList).ToArray();

        return new SparseMatrix(rowsIndexes, columnsIndexes);
    }

    private void BuildAdjacencyList(Grid<Node3D> grid)
    {
        _adjacencyList = new List<SortedSet<int>>(grid.Nodes.Length * 2);

        for (var i = 0; i < grid.Nodes.Length * 2; i++)
        {
            _adjacencyList.Add(new SortedSet<int>());
        }

        foreach (var element in grid)
        {
            var nodesIndexes = element.NodesIndexes;

            foreach (var currentNode in nodesIndexes)
            {
                for (var i = 0; i < 2; i++)
                {
                    var currentComplexNode = currentNode * 2 + i;

                    foreach (var nodeIndex in nodesIndexes)
                    {
                        for (var j = 0; j < 2; j++)
                        {
                            var complexNodeIndex = nodeIndex * 2 + j;
                            if (currentComplexNode > complexNodeIndex)
                                _adjacencyList[currentComplexNode].Add(complexNodeIndex);
                        }
                    }
                }
            }
        }
    }
}