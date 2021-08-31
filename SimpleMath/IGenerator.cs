using System;
using SimpleMath.Supports;

namespace SimpleMath
{
    public interface IGenerator<T>
    {
        public ISequence<T> ToSeq();
    }

    public class CloneGenerator<T> : IGenerator<T>
    {
        private readonly ISequence<T> _seq;

        public CloneGenerator(ISequence<T> seq)
            => _seq = seq;

        public ISequence<T> ToSeq()
            => _seq.Clone() as ISequence<T>;
    }

    public class MapiiGenerator<T, U> : IGenerator<U>
    {
        private readonly IGenerator<T> _prevGenerator;

        private readonly Func<T, int, int, U> _mapper;

        public MapiiGenerator(MatrixSeq<T> matrixseq, Func<T, int, int, U> mapper)
        {
            _prevGenerator = matrixseq.GetGenerator();
            _mapper = mapper;
        }

        public ISequence<U> ToSeq()
        {
            var matrix = _prevGenerator.ToSeq() as Matrix<T>;

            var newarray = new U[matrix.RowsNum, matrix.ColumnsNum];

            for (var i = 0; i < matrix.RowsNum; i++)
            {
                for (var j = 0; j < matrix.ColumnsNum; j++)
                {
                    newarray[i, j] = _mapper.Invoke(matrix[i, j], i, j);
                }
            }

            return new Matrix<U>(newarray);
        }
    }

    public class TransposeGenerator<T> : IGenerator<T>
    {
        private readonly IGenerator<T> _prevGenerator;
        
        public TransposeGenerator(MatrixSeq<T> matrixseq)
            => _prevGenerator = matrixseq.GetGenerator();
        
        public ISequence<T> ToSeq()
        {
            var matrix = _prevGenerator.ToSeq() as Matrix<T>;

            var newarray = new T[matrix.ColumnsNum, matrix.RowsNum];

            for (var i = 0; i < matrix.ColumnsNum; i++)
            {
                for (var j = 0; j < matrix.RowsNum; j++)
                {
                    newarray[i, j] = matrix[j, i];
                }
            }

            return new Matrix<T>(newarray);
        }
    }
    
    public class ZipGenerator<T, U> : IGenerator<(T, U)>
    {
        private readonly IGenerator<T> _prevGeneratorL;
        private readonly IGenerator<U> _prevGeneratorR;

        public ZipGenerator(MatrixSeq<T> matrixseql, MatrixSeq<U> matrixseqr)
        {
            _prevGeneratorL = matrixseql.GetGenerator();
            _prevGeneratorR = matrixseqr.GetGenerator();
        }

        public ISequence<(T, U)> ToSeq()
        {
            var matrixl = _prevGeneratorL.ToSeq() as Matrix<T>;
            var matrixr = _prevGeneratorR.ToSeq() as Matrix<U>;

            if (matrixl.RowsNum != matrixr.RowsNum || matrixl.ColumnsNum != matrixr.ColumnsNum)
                throw new MatrixCalcException("Invalid calculation.");

            var newarray = new (T, U)[matrixl.RowsNum, matrixl.ColumnsNum];

            for (var i = 0; i < matrixl.RowsNum; i++)
            {
                for (var j = 0; j < matrixl.ColumnsNum; j++)
                {
                    newarray[i, j] = (matrixl[i, j], matrixr[i, j]);
                }
            }

            return new Matrix<(T, U)>(newarray);
        }
    }

    public class CropGenerator<T> : IGenerator<T>
    {
        private readonly IGenerator<T> _prevGenerator;
        private readonly (int, int) _startPos;
        private readonly (int, int) _endPos;

        public CropGenerator(MatrixSeq<T> matrixseq, (int, int) startpos, (int, int) endpos)
        {
            _prevGenerator = matrixseq.GetGenerator();

            if (startpos.Item1 > endpos.Item1 || startpos.Item2 > endpos.Item2)
                throw new RankException("Start position must smaller than end position.");

            if (startpos.Item1 < 0 || startpos.Item2 < 0 || endpos.Item1 < 0 || endpos.Item2 < 0)
                throw new IndexOutOfRangeException("The value of position must greater than zero.");

            _startPos = startpos;
            _endPos = endpos;
        }

        public CropGenerator(MatrixSeq<T> matrixseq, int pos, bool isrow)
        {
            _prevGenerator = matrixseq.GetGenerator();

            if (pos < 0)
                throw new IndexOutOfRangeException("The value of position must greater than zero.");

            if (isrow)
            {
                _startPos = (pos, 0);
                _endPos = (pos, -1);
            }
            else
            {
                _startPos = (0, pos);
                _endPos = (-1, pos);
            }
        }

        public ISequence<T> ToSeq()
        {
            var matrix = _prevGenerator.ToSeq() as Matrix<T>;
            
            var rowsoffset = _startPos.Item1;
            var columnsoffset = _startPos.Item2;

            var rowsendpos = _endPos.Item1 == -1 ? matrix.RowsNum - 1 : _endPos.Item1;
            var columnsendpos = _endPos.Item2 == -1 ? matrix.ColumnsNum - 1 : _endPos.Item2;

            var rowsrange = rowsendpos - rowsoffset + 1;
            var columnsrange = columnsendpos - columnsoffset + 1;

            var newarray = new T[rowsrange, columnsrange];

            for (var i = 0; i < rowsrange; i++)
            {
                for (var j = 0; j < columnsrange; j++)
                {
                    newarray[i, j] = matrix[i + rowsoffset, j + columnsoffset];
                }
            }

            return new Matrix<T>(newarray);
        }
    }
}
