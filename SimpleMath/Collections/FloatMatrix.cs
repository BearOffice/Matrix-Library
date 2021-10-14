using System;
using SimpleMath.MathQ;
using SimpleMath.Supports;

namespace SimpleMath.Collections
{
    [ContentsType(ContentsType = ContentsType.Numeric)]
    public class FloatMatrix : Matrix<float>
    {
        public FloatMatrix(float[,] array) : base(array) { }

        public FloatMatrix(float[] array) : base(array) { }

        public FloatMatrix(int rows, int columns) : base(rows, columns) { }

        private FloatMatrix(Matrix<float> matrix) : base(matrix) { }

        public override object Clone()
            => new FloatMatrix(this);

        internal static FloatMatrix CastFrom(Matrix<float> matrix)
            => new(matrix);

        public static FloatMatrix GetMatrixFromString(string matrixstr, ParseFromString parserule)
            => MatrixParser.StringToMatrix<float>(matrixstr, parserule,
                Matrix.IsNumeric(typeof(FloatMatrix))).ToFloatMatrix();

        public static FloatMatrix GetMatrixFromString(string matrixstr)
        {
            var parserule = new ParseFromString();
            parserule.Add(typeof(float), item => float.Parse(item));

            return GetMatrixFromString(matrixstr, parserule);
        }

        public static FloatMatrix operator +(FloatMatrix left, FloatMatrix right)
        {
            if (left.RowsNum != right.RowsNum || left.ColumnsNum != right.ColumnsNum)
                throw new MatrixCalcException("Invalid calculation.");

            var newarray = new float[left.RowsNum, left.ColumnsNum];
            ParallelArrayProjector.AutoFor(newarray, (i, j) => left[i, j] + right[i, j]);

            return new FloatMatrix(newarray);
        }

        public static FloatMatrix operator -(FloatMatrix left, FloatMatrix right)
        {
            if (left.RowsNum != right.RowsNum || left.ColumnsNum != right.ColumnsNum)
                throw new MatrixCalcException("Invalid calculation.");

            var newarray = new float[left.RowsNum, left.ColumnsNum];
            ParallelArrayProjector.AutoFor(newarray, (i, j) => left[i, j] - right[i, j]);

            return new FloatMatrix(newarray);
        }

        public static FloatMatrix operator *(FloatMatrix left, FloatMatrix right)
        {
            if (left.ColumnsNum != right.RowsNum)
                throw new MatrixCalcException("Invalid calculation.");

            var newarray = new float[left.RowsNum, right.ColumnsNum];
            ParallelArrayProjector.AutoFor(newarray, (i, j) =>
            {
                var num = left[i, 0] * right[0, j];

                for (var k = 1; k < left.ColumnsNum; k++)
                {
                    num += left[i, k] * right[k, j];
                }

                return num;
            }, calccost: left.ColumnsNum);

            return new FloatMatrix(newarray);
        }

        public static FloatMatrix operator *(float left, FloatMatrix right)
        {
            var newarray = new float[right.RowsNum, right.ColumnsNum];
            ParallelArrayProjector.AutoFor(newarray, (i, j) => left * right[i, j]);

            return new FloatMatrix(newarray);
        }

        public static FloatMatrix ConcatTopAndBottom(FloatMatrix matrixt, FloatMatrix matrixb)
            => Matrix.ConcatTopAndBottom(matrixt, matrixb).ToFloatMatrix();

        public static FloatMatrix ConcatLeftAndRight(FloatMatrix matrixl, FloatMatrix matrixr)
            => Matrix.ConcatLeftAndRight(matrixl, matrixr).ToFloatMatrix();

        public static FloatMatrix operator |(FloatMatrix matrixt, FloatMatrix matrixb)
            => ConcatTopAndBottom(matrixt, matrixb);

        public static FloatMatrix operator &(FloatMatrix matrixl, FloatMatrix matrixr)
            => ConcatLeftAndRight(matrixl, matrixr);
    }
}
