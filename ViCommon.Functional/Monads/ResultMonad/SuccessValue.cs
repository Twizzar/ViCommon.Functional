using System;
using System.Collections.Generic;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ViCommon.Functional.Monads.ResultMonad
{
    /// <summary>
    /// Represents the success part of a result.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public readonly struct SuccessValue<TValue> : IEquatable<SuccessValue<TValue>>, IResultValue
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessValue{TValue}"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        internal SuccessValue(TValue value)
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the value.
        /// </summary>
        public TValue Value { get; }

        #endregion

        #region members

        /// <summary>
        /// Convert ResultSuccess to a value implicitly.
        /// </summary>
        /// <param name="successValue">The success result.</param>
        /// <returns>The value.</returns>
        public static implicit operator TValue(SuccessValue<TValue> successValue) => successValue.Value;

        public static bool operator ==(SuccessValue<TValue> left, SuccessValue<TValue> right) => left.Equals(right);

        public static bool operator !=(SuccessValue<TValue> left, SuccessValue<TValue> right) => !left.Equals(right);

        /// <summary>
        /// Add the failure type to the success to convert it to a real <see cref="IResult{TSuccess,TFailure}"/>.
        /// </summary>
        /// <typeparam name="TFailure">The failure type.</typeparam>
        /// <returns>A new <see cref="IResult{TSuccess,TFailure}"/>.</returns>
        public IResult<TValue, TFailure> WithFailure<TFailure>()
            where TFailure : Failure =>
            Result.Success<TValue, TFailure>(this.Value);

        /// <summary>
        /// Checks if two ResultSuccess are equal.
        /// </summary>
        /// <param name="other">The other ResultSuccess.</param>
        /// <returns>True when equal.</returns>
        public bool Equals(SuccessValue<TValue> other) =>
            EqualityComparer<TValue>.Default.Equals(this.Value, other.Value);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is SuccessValue<TValue> other && this.Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => EqualityComparer<TValue>.Default.GetHashCode(this.Value);

        #endregion
    }
}