using System;
using SimpleMath.MathQ;
using SimpleMath.Supports;

namespace SimpleMath.Collections
{
    [ContentsType(ContentsType = ContentsType.Numeric)]
    public class DoubleMatrix : Matrix<double>
    {
        public DoubleMatrix(double[,] array) : base(array) { }

        public DoubleMatrix(double[] array) : base(array) { }

        public DoubleMatrix(int rows, int columns) : base(rows, columns) { }

        private DoubleMatrix(Matrix<double> matrix) : base(matrix) { }

        public override object Clone()
            => new DoubleMatrix(this);

        internal static DoubleMatrix CastFrom(Matrix<double> matrix)
            => new(matrix);

        public static DoubleMatrix GetMatrixFromString(string matrixstr, ParseFromString parserule)
            => MatrixParser.StringToMatrix<double>(matrixstr, parserule,
                Matrix.IsNumeric(typeof(DoubleMatrix))).ToDoubleMatrix();

        public static DoubleMatrix GetMatrixFromString(string matrixstr)
        {
            var parserule = new ParseFromString();
            parserule.Add(typeof(double), item => double.Parse(item));

            return GetMatrixFromString(matrixstr, parserule);
        }

        public static DoubleMatrix operator +(DoubleMatrix left, DoubleMatrix right)
        {
            if (left.RowsNum != right.RowsNum || left.ColumnsNum != right.ColumnsNum)
                throw new MatrixCalcException("Invalid calculation.");

            var newarray = new double[left.RowsNum, left.ColumnsNum];
            ParallelArrayProjector.AutoFor(newarray, (i, j) => left[i, j] + right[i, j]);

            return new DoubleMatrix(newarray);
        }

        public static DoubleMatrix operator -(DoubleMatrix left, DoubleMatrix right)
        {
            if (left.RowsNum != right.RowsNum || left.ColumnsNum != right.ColumnsNum)
                throw new MatrixCalcException("Invalid calculation.");

            var newarray = new double[left.RowsNum, left.ColumnsNum];
            ParallelArrayProjector.AutoFor(newarray, (i, j) => left[i, j] - right[i, j]);

            return new DoubleMatrix(newarray);
        }

        public static DoubleMatrix operator *(DoubleMatrix left, DoubleMatrix right)
        {
            if (left.ColumnsNum != right.RowsNum)
                throw new MatrixCalcException("Invalid calculation.");

            var newarray = new double[left.RowsNum, right.ColumnsNum];
            ParallelArrayProjector.AutoFor(newarray, (i, j) => 
            {
                var num = left[i, 0] * right[0, j];

                for (var k = 1; k < left.ColumnsNum; k++)
                {
                    num += left[i, k] * right[k, j];
                }

                return num;
            }, calccost: left.ColumnsNum);

            return new DoubleMatrix(newarray);
        }

        public static DoubleMatrix operator *(double left, DoubleMatrix right)
        {
            var newarray = new double[right.RowsNum, right.ColumnsNum];
            ParallelArrayProjector.AutoFor(newarray, (i, j) => left * right[i, j]);

            return new DoubleMatrix(newarray);
        }

        public static DoubleMatrix ConcatTopAndBottom(DoubleMatrix matrixt, DoubleMatrix matrixb)
            => Matrix.ConcatTopAndBottom(matrixt, matrixb).ToDoubleMatrix();

        public static DoubleMatrix ConcatLeftAndRight(DoubleMatrix matrixl, DoubleMatrix matrixr)
            => Matrix.ConcatLeftAndRight(matrixl, matrixr).ToDoubleMatrix();

        public static DoubleMatrix operator |(DoubleMatrix matrixt, DoubleMatrix matrixb)
            => ConcatTopAndBottom(matrixt, matrixb);

        public static DoubleMatrix operator &(DoubleMatrix matrixl, DoubleMatrix matrixr)
            => ConcatLeftAndRight(matrixl, matrixr);
    }
}
