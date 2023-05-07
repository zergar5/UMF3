using UMF3.Core.Base;
using UMF3.FEM.Assembling;

namespace UMF3.ThreeDimensional.MatrixTemplates;

public class SecondConditionMatrixTemplateProvider : ITemplateMatrixProvider
{
    public BaseMatrix GetMatrix()
    {
        return new BaseMatrix(
            new double[,]
            {
                { 4, 0, 2, 0, 2, 0, 1, 0 },
                { 0, 4, 0, 2, 0, 2, 0, 1 },
                { 2, 0, 4, 0, 1, 0, 2, 0 },
                { 0, 2, 0, 4, 0, 1, 0, 2 },
                { 2, 0, 1, 0, 4, 0, 2, 0 },
                { 0, 2, 0, 1, 0, 4, 0, 2 },
                { 1, 0, 2, 0, 2, 0, 4, 0 },
                { 0, 1, 0, 2, 0, 2, 0, 4 }
            }
        );
    }
}