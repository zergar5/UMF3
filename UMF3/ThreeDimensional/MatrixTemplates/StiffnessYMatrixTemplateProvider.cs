using UMF3.Core.Base;
using UMF3.FEM.Assembling;

namespace UMF3.ThreeDimensional.MatrixTemplates;

public class StiffnessYMatrixTemplateProvider : ITemplateMatrixProvider
{
    public BaseMatrix GetMatrix()
    {
        return new BaseMatrix(new double[,]
        {
            { 4, 2, -4, -2, 2, 1, -2, -1 },
            { 2, 4, -2, -4, 1, 2, -1, -2 },
            { -4, -2, 4, 2, -2, -1, 2, 1},
            { -2, -4, 2, 4, -1, -2, 1, 2 },
            { 2, 1, -2, -1, 4, 2, -4, -2 },
            { 1, 2, -1, -2, 2, 4, -2, -4 },
            { -2, -1, 2, 1, -4, -2, 4, 2 },
            { -1, -2, 1, 2, -2, -4, 2, 4 }
        });
    }
}