﻿using UMF3.Core.Base;
using UMF3.FEM.Assembling;

namespace UMF3.ThreeDimensional.MatrixTemplates;

public class MassMatrixTemplateProvider : ITemplateMatrixProvider
{
    public BaseMatrix GetMatrix()
    {
        return new BaseMatrix(
            new double[,]
            {
                { 8, 4, 4, 2, 4, 2, 2, 1 },
                { 4, 8, 2, 4, 2, 4, 1, 2 },
                { 4, 2, 8, 4, 2, 1, 4, 2 },
                { 2, 4, 4, 8, 1, 2, 2, 4 },
                { 4, 2, 2, 1, 8, 4, 4, 2 },
                { 2, 4, 1, 2, 4, 8, 2, 4 },
                { 2, 1, 4, 2, 4, 2, 8, 4 },
                { 1, 2, 2, 4, 2, 4, 4, 8 }
            }
        );
    }
}