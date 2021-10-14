using System;
using SimpleMath.MathQ;
using SimpleMath.Supports;

namespace SimpleMath.Collections
{
    [ContentsType(ContentsType = ContentsType.Numeric)]
    public class IntMatrix : Matrix<int>
    {
        public IntMatrix(int[,] array) : base(array) { }

        public IntMatrix(int[] array) : base(array) { }

        public IntMatrix(int rows, int columns) : base(rows, columns) { }

        private IntMatrix(Matrix<int> matrix) : base(matrix) { }

        public override object Clone()
            => new IntMatrix(this);

        internal static IntMatrix CastFrom(Matrix<int> matrix)
            => new(matrix);

        public static IntMatrix GetMatrixFromString(string matrixstr, ParseFromString parserule)
            => MatrixParser.StringToMatrix<int>(matrixstr, parserule,
                Matrix.IsNumeric(typeof(IntMatrix))).ToIntMatrix();

        public static IntMatrix GetMatrixFromString(string matrixstr)
        {
            var parserule = new ParseFromString();
            parserule.Add(typeof(int), item => int.Parse(item));

            return GetMatrixFromString(matrixstr, parserule);
        }

        public static IntMatrix operator +(IntMatrix left, IntMatrix right)
        {
            if (left.RowsNum != right.RowsNum || left.ColumnsNum != right.ColumnsNum)
                throw new MatrixCalcException("Invalid calculation.");

            var newarray = new int[left.RowsNum, left.ColumnsNum];
            ParallelArrayProjector.AutoFor(newarray, (i, j) => left[i, j] + right[i, j]);

            return new IntMatrix(newarray);
        }

        public static IntMatrix operator -(IntMatrix left, IntMatrix right)
        {
            if (left.RowsNum != right.RowsNum || left.ColumnsNum != right.ColumnsNum)
                throw new MatrixCalcException("Invalid calculation.");

            var newarray = new int[left.RowsNum, left.ColumnsNum];
            ParallelArrayProjector.AutoFor(newarray, (i, j) => left[i, j] - right[i, j]);

            return new IntMatrix(newarray);
        }

        public static IntMatrix operator *(IntMatrix left, IntMatrix right)
        {
            if (left.ColumnsNum != right.RowsNum)
                throw new MatrixCalcException("Invalid calculation.");

            return new IntMatrix(left.RowsNum, right.ColumnsNum)
                .AsParallel()
                .Set((i, j) =>
                {
                    var num = left[i, 0] * right[0, j];

                    for (var k = 1; k < left.ColumnsNum; k++)
                    {
                        num += left[i, k] * right[k, j];
                    }

                    return num;
                }).ToIntMatrix();
        }

        public static IntMatrix operator *(int left, IntMatrix right)
        {
            var newarray = new int[right.RowsNum, right.ColumnsNum];
            ParallelArrayProjector.AutoFor(newarray, (i, j) => left * right[i, j]);

            return new IntMatrix(newarray);
        }

        public static IntMatrix ConcatTopAndBottom(IntMatrix matrixt, IntMatrix matrixb)
            => Matrix.ConcatTopAndBottom(matrixt, matrixb).ToIntMatrix();

        public static IntMatrix ConcatLeftAndRight(IntMatrix matrixl, IntMatrix matrixr)
            => Matrix.ConcatLeftAndRight(matrixl, matrixr).ToIntMatrix();

        public static IntMatrix operator |(IntMatrix matrixt, IntMatrix matrixb)
            => ConcatTopAndBottom(matrixt, matrixb);

        public static IntMatrix operator &(IntMatrix matrixl, IntMatrix matrixr)
            => ConcatLeftAndRight(matrixl, matrixr);
    }
}
