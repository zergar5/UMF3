using UMF3.Core.Base;
using UMF3.FEM.Assembling;

namespace UMF3.ThreeDimensional.MatrixTemplates;

public class StiffnessXMatrixTemplateProvider : ITemplateMatrixProvider
{
    public BaseMatrix GetMatrix()
    {
        return new BaseMatrix(new double[,]
        {
            { 4, -4, 2, -2, 2, -2, 1, -1 },
            { -4, 4, -2, 2, -2, 2, -1, 1 },
            { 2, -2, 4, -4, 1, -1, 2, -2 },
            { -2, 2, -4, 4, -1, 1, -2, 2 },
            { 2, -2, 1, -1, 4, -4, 2, -2 },
            { -2, 2, -1, 1, -4, 4, -2, 2 },
            { 1, -1, 2, -2, 2, -2, 4, -4 },
            { -1, 1, -2, 2, -2, 2, -4, 4 }
        });
    }
}