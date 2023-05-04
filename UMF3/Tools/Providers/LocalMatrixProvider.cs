using KursachOneDim.Models.Local;

namespace KursachOneDim.Tools.Providers
{
    public static class LocalMatrixProvider
    {
        private static readonly LocalMatrix BasicM = new(new[,]
        {
            {2d, 1d},
            {1d, 2d}
        });

        private static readonly LocalMatrix BasicG = new(new[,]
        {
            {1d, -1d},
            {-1d, 1d}
        });

        public static LocalMatrix GetStiffnessMatrix(double hX) => 1 / hX * BasicG;

        public static LocalMatrix GetMassMatrix(double hX) => hX / 6d * BasicM;
    }
}