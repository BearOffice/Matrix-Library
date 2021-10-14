using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMath
{
    public class MatrixSeq<T> : ISequence<T>
    {
        private readonly IGenerator<T> _generator;

        protected MatrixSeq()
            => _generator = new CloneGenerator<T>(this);

        protected MatrixSeq(IGenerator<T> generator)
            => _generator = generator;

        public virtual object Clone()
            => new MatrixSeq<T>(_generator);

        public IGenerator<T> GetGenerator()
            => _generator;

        internal MatrixSeq<T> AsParallelFunc(bool force)
            => new(new ParallelGenerator<T>(this, force));

        internal MatrixSeq<U> MapiiFunc<U>(Func<T, int, int, U> mapper)
            => new(new MapiiGenerator<T, U>(this, mapper));

        internal MatrixSeq<T> TransposeFunc()
            => new(new TransposeGenerator<T>(this));

        internal MatrixSeq<(T, U)> ZipFunc<U>(MatrixSeq<U> matrixseq)
            => new(new ZipGenerator<T, U>(this, matrixseq));

        internal MatrixSeq<T> GetSubMatrixFunc((int, int) startpos, (int, int) endpos)
            => new(new CropGenerator<T>(this, startpos, endpos));

        internal MatrixSeq<T> GetSubMatrixFunc(int pos, bool isrow)
            => new(new CropGenerator<T>(this, pos, isrow));
    }
}
