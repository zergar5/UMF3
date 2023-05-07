using UMF3.Core.BoundaryConditions;
using UMF3.Core.Global;

namespace UMF3.FEM.Assembling.Global;

public interface IGaussExcluder<TMatrix>
{
    public void Exclude(Equation<TMatrix> equation, FirstCondition condition);
}