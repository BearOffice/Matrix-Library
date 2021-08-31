using System;

namespace SimpleMath.MathQ
{
    public static class MatrixQueryExtensions
    {
        public static Matrix<T> ToMatrix<T>(this MatrixSeq<T> matrixseq)
             => matrixseq.GetGenerator().ToSeq() as Matrix<T>;

        public static MatrixSeq<U> Mapii<T, U>(this MatrixSeq<T> matrixseq, 
            Func<T, int, int, U> mapper)
             => matrixseq.MapiiFunc(mapper);

        public static MatrixSeq<U> Map<T, U>(this MatrixSeq<T> matrixseq, 
            Func<T, U> mapper)
             => matrixseq.MapiiFunc((t, _, _) => mapper.Invoke(t));
        
        public static MatrixSeq<T> Set<T>(this MatrixSeq<T> matrixseq, 
            Func<int, int, T> setter)
             => matrixseq.MapiiFunc((_, i, j) => setter.Invoke(i, j));

        public static MatrixSeq<T> Transpose<T>(this MatrixSeq<T> matrixseq)
             => matrixseq.TransposeFunc();

        public static MatrixSeq<(T, U)> Zip<T, U>(this MatrixSeq<T> matrixseql, 
            MatrixSeq<U> matrixseqr)
             => matrixseql.ZipFunc(matrixseqr);

        public static MatrixSeq<T> GetSubMatrix<T>(this MatrixSeq<T> matrixseq,
            (int, int) startpos, (int, int) endpos)
             => matrixseq.GetSubMatrixFunc(startpos, endpos);

        public static MatrixSeq<T> GetRow<T>(this MatrixSeq<T> matrixseq, int pos)
             => matrixseq.GetSubMatrixFunc(pos, isrow: true);

        public static MatrixSeq<T> GetColumn<T>(this MatrixSeq<T> matrixseq, int pos)
             => matrixseq.GetSubMatrixFunc(pos, isrow: false);
    }
}
