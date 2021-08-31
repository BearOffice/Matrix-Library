using System;

namespace SimpleMath.Supports
{
    public class MatrixCalcException : Exception
    {
        public MatrixCalcException() : base() { }

        public MatrixCalcException(string message) : base(message) { }

        public MatrixCalcException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
