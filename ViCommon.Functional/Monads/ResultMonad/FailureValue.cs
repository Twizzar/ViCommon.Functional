using System;
using System.Collections.Generic;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ViCommon.Functional.Monads.ResultMonad
{
    /// <summary>
    /// Represents the failure part of a result. Can be implicitly casted to Result.
    /// </summary>
    /// <typeparam name="TFailure">The failure type.</typeparam>
    public readonly struct FailureValue<TFailure> : IEquatable<FailureValue<TFailure>>, IResultValue
        where TFailure : Failure
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureValue{TFailure}"/> struct.
        /// </summary>
        /// <param name="failure">The failure.</param>
        internal FailureValue(TFailure failure)
        {
            this.Value = failure ?? throw new ArgumentNullException(nameof(failure));
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the value.
        /// </summary>
        public TFailure Value { get; }

        #endregion

        #region members

        /// <summary>
        /// Convert ResultFailure to a failure implicitly.
        /// </summary>
        /// <param name="failureValue">The failure result.</param>
        /// <returns>The failure.</returns>
        public static implicit operator TFailure(FailureValue<TFailure> failureValue) => failureValue.Value;

        public static bool operator ==(FailureValue<TFailure> left, FailureValue<TFailure> right) => left.Equals(right);

        public static bool operator !=(FailureValue<TFailure> left, FailureValue<TFailure> right) =>
            !left.Equals(right);

        /// <summary>
        /// Add the success type to a failure value to convert it to a <see cref="IResult{TSuccess,TFailure}"/>.
        /// </summary>
        /// <typeparam name="TSuccess">The success value.</typeparam>
        /// <returns>A new <see cref="IResult{TSuccess,TFailure}"/>.</returns>
        public IResult<TSuccess, TFailure> WithSuccess<TSuccess>() => Result.Failure<TSuccess, TFailure>(this.Value);

        /// <inheritdoc />
        public bool Equals(FailureValue<TFailure> other) =>
            EqualityComparer<TFailure>.Default.Equals(this.Value, other.Value);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is FailureValue<TFailure> other && this.Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => EqualityComparer<TFailure>.Default.GetHashCode(this.Value);

        #endregion
    }
}