using System;
using System.Linq;
using SimpleMath.Supports;

namespace SimpleMath
{
    [ContentsType(ContentsType = ContentsType.Generic)]
    public class Matrix<T> : MatrixSeq<T>
    {
        private readonly T[,] _sourceArray;
        public int RowsNum { get => _sourceArray.GetUpperBound(0) + 1; }
        public int ColumnsNum { get => _sourceArray.GetUpperBound(1) + 1; }
        public int Length { get => RowsNum * ColumnsNum; }

        public Matrix(T[,] array)
            => _sourceArray = array;

        public Matrix(T[] array)
        {
            var newarray = new T[1, array.Length];

            for (var i = 0; i < array.Length; i++)
            {
                newarray[0, i] = array[i];
            }

            _sourceArray = newarray;
        }

        public Matrix(int rows, int columns)
        {
            var array = new T[rows, columns];
            Array.Clear(array, 0, array.Length);

            _sourceArray = array;
        }

        protected Matrix(Matrix<T> matrix)
            => _sourceArray = matrix._sourceArray;

        public T this[int rows, int columns]
        {
            get => _sourceArray[rows, columns];
            set => _sourceArray[rows, columns] = value;
        }

        public void Iterateii(Action<T, int, int> iterator)
        {
            for (var i = 0; i < RowsNum; i++)
            {
                for (var j = 0; j < ColumnsNum; j++)
                {
                    iterator.Invoke(_sourceArray[i, j], i, j);
                }
            }
        }

        public void Iterate(Action<T> iterator)
            => Iterateii((t, _, _) => iterator.Invoke(t));

        public T[] Convert1DMatrixToArray()
        {
            if (RowsNum != 1 && ColumnsNum != 1)
                throw new MatrixCalcException("The Matrix's dimension is not one.");

            var array = new T[Length];
            var count = 0;
            Iterate(t =>
            {
                array[count] = t;
                count++;
            });

            return array;
        }

        public bool IsNumeric()
            => Matrix.IsNumeric(this.GetType());

        public override object Clone()
            => new Matrix<T>(this);

        public override string ToString()
            => MatrixParser.MatrixToString(this, new ParseToString());

        public string ToString(ParseToString parserule)
            => MatrixParser.MatrixToString(this, parserule);

        public static Matrix<T> operator |(Matrix<T> matrixt, Matrix<T> matrixb)
            => Matrix.ConcatTopAndBottom(matrixt, matrixb);

        public static Matrix<T> operator &(Matrix<T> matrixt, Matrix<T> matrixb)
            => Matrix.ConcatLeftAndRight(matrixt, matrixb);
    }

    public class Matrix
    {
        private Matrix() { }

        public static bool IsNumeric(Type type)
        {
            var hasnum = Attribute.GetCustomAttributes(type).ToList()
                .Find(attr =>
                {
                    if (attr is ContentsTypeAttribute ct)
                    {
                        if (ct.ContentsType == ContentsType.Numeric)
                            return true;
                    }
                    return false;
                });

            return !(hasnum == null);
        }

        public static Matrix<T> GetMatrixFromString<T>(string matrixstr, ParseFromString parserule)
            => MatrixParser.StringToMatrix<T>(matrixstr, parserule, IsNumeric(typeof(Matrix<T>)));

        public static Matrix<T> ConcatTopAndBottom<T>(Matrix<T> matrixt, Matrix<T> matrixb)
        {
            if (matrixt.ColumnsNum != matrixb.ColumnsNum)
                throw new MatrixCalcException("Failed to concat the two matrixes " +
                    "due to the mismatch of the columns numbers.");

            var rowsnum = matrixt.RowsNum + matrixb.RowsNum;
            var columnsnum = matrixt.ColumnsNum;

            var newarray = new T[rowsnum, columnsnum];

            for (var i = 0; i < matrixt.RowsNum; i++)
            {
                for (var j = 0; j < columnsnum; j++)
                {
                    newarray[i, j] = matrixt[i, j];
                }
            }

            for (var i = matrixt.RowsNum; i < rowsnum; i++)
            {
                for (var j = 0; j < columnsnum; j++)
                {
                    newarray[i, j] = matrixb[i - matrixt.RowsNum, j];
                }
            }

            return new(newarray);
        }

        public static Matrix<T> ConcatLeftAndRight<T>(Matrix<T> matrixl, Matrix<T> matrixr)
        {
            if (matrixl.RowsNum != matrixr.RowsNum)
                throw new MatrixCalcException("Failed to concat the two matrixes " +
                    "due to the mismatch of the rows numbers.");

            var rowsnum = matrixl.RowsNum;
            var columnsnum = matrixl.ColumnsNum + matrixr.ColumnsNum;

            var newarray = new T[rowsnum, columnsnum];

            for (var i = 0; i < rowsnum; i++)
            {
                for (var j = 0; j < matrixl.ColumnsNum; j++)
                {
                    newarray[i, j] = matrixl[i, j];
                }
            }

            for (var i = 0; i < rowsnum; i++)
            {
                for (var j = matrixl.ColumnsNum; j < columnsnum; j++)
                {
                    newarray[i, j] = matrixr[i, j - matrixl.ColumnsNum];
                }
            }

            return new(newarray);
        }
    }
}
