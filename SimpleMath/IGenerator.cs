using System;
using SimpleMath.Supports;

namespace SimpleMath
{
    public interface IGenerator<T>
    {
        public CalculateMode CalculateMode { get; }
        public ISequence<T> ToSeq();
    }

    internal class CloneGenerator<T> : IGenerator<T>
    {
        private readonly ISequence<T> _seq;
        public CalculateMode CalculateMode { get => CalculateMode.Series; }

        internal CloneGenerator(ISequence<T> seq)
            => _seq = seq;

        public ISequence<T> ToSeq()
            => _seq.Clone() as ISequence<T>;
    }
    
    internal class ParallelGenerator<T> : IGenerator<T>
    {
        private readonly IGenerator<T> _prevGenerator;
        public CalculateMode CalculateMode { get; }
        
        internal ParallelGenerator(ISequence<T> seq, bool force)
        {
            _prevGenerator = seq.GetGenerator();
            CalculateMode = force ? CalculateMode.Parallel : CalculateMode.Auto;
        }

        public ISequence<T> ToSeq()
            => _prevGenerator.ToSeq();
    }

    internal class MapiiGenerator<T, U> : IGenerator<U>
    {
        private readonly IGenerator<T> _prevGenerator;
        private readonly Func<T, int, int, U> _mapper;
        public CalculateMode CalculateMode { get => _prevGenerator.CalculateMode; }

        internal MapiiGenerator(MatrixSeq<T> matrixseq, Func<T, int, int, U> mapper)
        {
            _prevGenerator = matrixseq.GetGenerator();
            _mapper = mapper;
        }

        public ISequence<U> ToSeq()
        {
            var matrix = _prevGenerator.ToSeq() as Matrix<T>;

            var newarray = new U[matrix.RowsNum, matrix.ColumnsNum];
            ParallelArrayProjector.For(newarray, (i, j) =>
                _mapper.Invoke(matrix[i, j], i, j), CalculateMode);

            return new Matrix<U>(newarray);
        }
    }

    internal class TransposeGenerator<T> : IGenerator<T>
    {
        private readonly IGenerator<T> _prevGenerator;
        public CalculateMode CalculateMode { get => _prevGenerator.CalculateMode; }

        internal TransposeGenerator(MatrixSeq<T> matrixseq)
            => _prevGenerator = matrixseq.GetGenerator();
        
        public ISequence<T> ToSeq()
        {
            var matrix = _prevGenerator.ToSeq() as Matrix<T>;

            var newarray = new T[matrix.ColumnsNum, matrix.RowsNum];
            ParallelArrayProjector.For(newarray, (i, j) => matrix[j, i], CalculateMode);

            return new Matrix<T>(newarray);
        }
    }

    internal class ZipGenerator<T, U> : IGenerator<(T, U)>
    {
        private readonly IGenerator<T> _prevGeneratorL;
        private readonly IGenerator<U> _prevGeneratorR;
        public CalculateMode CalculateMode { get => _prevGeneratorL.CalculateMode ; }

        internal ZipGenerator(MatrixSeq<T> matrixseql, MatrixSeq<U> matrixseqr)
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
            ParallelArrayProjector.For(newarray, (i, j) =>
                (matrixl[i, j], matrixr[i, j]), CalculateMode);

            return new Matrix<(T, U)>(newarray);
        }
    }

    internal class CropGenerator<T> : IGenerator<T>
    {
        private readonly IGenerator<T> _prevGenerator;
        private readonly (int, int) _startPos;
        private readonly (int, int) _endPos;
        public CalculateMode CalculateMode { get => _prevGenerator.CalculateMode; }

        internal CropGenerator(MatrixSeq<T> matrixseq, (int, int) startpos, (int, int) endpos)
        {
            _prevGenerator = matrixseq.GetGenerator();

            if (startpos.Item1 > endpos.Item1 || startpos.Item2 > endpos.Item2)
                throw new RankException("Start position must smaller than end position.");

            if (startpos.Item1 < 0 || startpos.Item2 < 0 || endpos.Item1 < 0 || endpos.Item2 < 0)
                throw new IndexOutOfRangeException("The value of position must greater than zero.");

            _startPos = startpos;
            _endPos = endpos;
        }

        internal CropGenerator(MatrixSeq<T> matrixseq, int pos, bool isrow)
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
            ParallelArrayProjector.For(newarray, (i, j) =>
                matrix[i + rowsoffset, j + columnsoffset], CalculateMode);

            return new Matrix<T>(newarray);
        }
    }
}
