using System;
using SimpleMath.MathQ;

namespace SimpleMath.Collections
{
    public static class MatrixConvertExtensions
    {
        public static IntMatrix ToIntMatrix(this Matrix<int> matrix)
            => IntMatrix.CastFrom(matrix);

        public static IntMatrix ToIntMatrix(this MatrixSeq<int> matrixseq)
            => IntMatrix.CastFrom(matrixseq.ToMatrix());

        public static DoubleMatrix ToDoubleMatrix(this Matrix<double> matrix)
            => DoubleMatrix.CastFrom(matrix);

        public static DoubleMatrix ToDoubleMatrix(this MatrixSeq<double> matrixseq)
            => DoubleMatrix.CastFrom(matrixseq.ToMatrix());

        public static FloatMatrix ToFloatMatrix(this Matrix<float> matrix)
            => FloatMatrix.CastFrom(matrix);
        
        public static FloatMatrix ToFloatMatrix(this MatrixSeq<float> matrixseq)
            => FloatMatrix.CastFrom(matrixseq.ToMatrix());
    }
}
