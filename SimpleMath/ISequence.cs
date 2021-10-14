using System;

namespace SimpleMath
{
    public interface ISequence<T> : ICloneable
    {
        public IGenerator<T> GetGenerator();
    }
}
