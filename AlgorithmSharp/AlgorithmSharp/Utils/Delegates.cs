namespace AlgorithmSharp.Utils
{
    /// <summary>
    ///     Class to store basic delegates
    /// </summary>
    public static class Delegates
    {
        /// <summary>
        ///     Represents an associative operation
        /// </summary>
        public delegate T AssociativeOperation<T>(T leftOperand, T rightOperand);

        /// <summary>
        ///     Represents commutative operation
        /// </summary>
        public delegate TOut CommutativeOperation<out TOut, in TIn>(TIn leftOperand, TIn rightOperand);

        /// <summary>
        ///     Represents idempotent operation
        /// </summary>
        public delegate T IdempotentOperation<T>(T left, T right);

        /// <summary>
        ///     Represents any operation
        /// </summary>
        public delegate TOut Operation<out TOut, in TLeftOperand, in TRightOperand>(TLeftOperand leftOperand,
            TRightOperand rightOperand);
    }
}