using System;
using ViCommon.Functional.Monads.MaybeMonad;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;

#pragma warning disable CA1000 // Do not declare static members on generic types
#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ViCommon.Functional.Monads.ResultMonad
{
    /// <summary>
    /// Monad which represent a state where there two possible outcomes.
    /// A success and a failure.
    /// </summary>
    /// <typeparam name="TSuccess">Type of the success success.</typeparam>
    /// <typeparam name="TFailure">Type of the failure success.</typeparam>
    public readonly partial struct Result<TSuccess, TFailure> : IResult<TSuccess, TFailure>, IEquatable<Result<TSuccess, TFailure>>
        where TFailure : Failure
    {
        private readonly Maybe<TSuccess> _success;
        private readonly Maybe<TFailure> _failure;

        private Result(TSuccess success)
        {
            this._success = success ?? throw new ArgumentNullException(nameof(success));
            this._failure = None();
            this.IsSuccess = true;
        }

        private Result(TFailure failure)
        {
            this._success = None();
            this._failure = failure ?? throw new ArgumentNullException(nameof(failure));
            this.IsSuccess = false;
        }

        /// <summary>
        /// Gets a value indicating whether the result was a success.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets a value indicating whether the result was a failure.
        /// </summary>
        public bool IsFailure =>
            !this.IsSuccess;

        /// <summary>
        /// Gets the success when <see cref="IsSuccess"/> else the failure.
        /// Can be used for pattern matching.
        /// <example>
        /// For example:
        /// <code>
        /// Result&lt;int, Failure&gt; result = Result.Success(5);
        /// switch (result.Case)
        /// {
        ///     case int i:
        ///         // Whe success
        ///         break;
        ///     case Failure f:
        ///         // When Failure
        ///         break;
        ///     }
        /// </code>
        /// </example>
        /// </summary>
        [Obsolete("Use " + nameof(AsResultValue) + " instead.")]
        public object Case =>
            this.IsSuccess switch
            {
                true => this._success.GetValueUnsafe(),
                false => this._failure.GetValueUnsafe(),
            };

        /// <summary>
        /// Converts ResultSuccess to Result implicitly.
        /// </summary>
        /// <param name="successValue">The ResultSuccess.</param>
        public static implicit operator Result<TSuccess, TFailure>(SuccessValue<TSuccess> successValue) =>
            new Result<TSuccess, TFailure>(successValue);

        /// <summary>
        /// Converts ResultFailure to Result implicitly.
        /// </summary>
        /// <param name="failureValue">The ResultFailure.</param>
        public static implicit operator Result<TSuccess, TFailure>(
            FailureValue<TFailure> failureValue) =>
            new Result<TSuccess, TFailure>(failureValue);

        /// <summary>
        /// Converts a Value to a Success Result implicitly.
        /// </summary>
        /// <param name="success">The success value.</param>
        public static implicit operator Result<TSuccess, TFailure>(TSuccess success) =>
            new Result<TSuccess, TFailure>(success);

        /// <summary>
        /// Converts a Failure type to a Failure Result implicitly.
        /// </summary>
        /// <param name="failure">The failure.</param>
        public static implicit operator Result<TSuccess, TFailure>(TFailure failure) =>
            new Result<TSuccess, TFailure>(failure);

        /// <summary>
        /// Checks if two results are equal.
        /// </summary>
        /// <param name="left">First result.</param>
        /// <param name="right">Second result.</param>
        /// <returns>True if they are equal.</returns>
        public static bool operator ==(Result<TSuccess, TFailure> left, Result<TSuccess, TFailure> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks if two results are not equal.
        /// </summary>
        /// <param name="left">First result.</param>
        /// <param name="right">Second result.</param>
        /// <returns>True if they are not equal.</returns>
        public static bool operator !=(Result<TSuccess, TFailure> left, Result<TSuccess, TFailure> right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Overloads the Bitwise or operator for chaining Results with Bind.
        /// Unfortunately this only works when both results have the same success and failure type.
        /// </summary>
        /// <param name="a">Result a.</param>
        /// <param name="b">Result b.</param>
        /// <returns>A new result a.Bind(b).</returns>
        public static IResult<TSuccess, TFailure> operator |(Result<TSuccess, TFailure> a, Func<TSuccess, IResult<TSuccess, TFailure>> b) =>
            a.Bind(b);

        /// <summary>
        /// Overloads the Bitwise or operator for chaining Results with Bind.
        /// Unfortunately this only works when both results have the same success and failure type.
        /// </summary>
        /// <param name="a">Result a.</param>
        /// <param name="b">Result b.</param>
        /// <returns>A new result a.Bind(b).</returns>
        public static IResult<TSuccess, TFailure> operator |(Result<TSuccess, TFailure> a, Func<TSuccess, SuccessValue<TSuccess>> b) =>
            a.Bind(success => (Result<TSuccess, TFailure>)b(success));

        /// <summary>
        /// Create a success result.
        /// </summary>
        /// <param name="value">The success.</param>
        /// <returns>A new <see cref="Result{TSuccess,TFailure}"/>.</returns>
        public static IResult<TSuccess, TFailure> Success(TSuccess value) =>
            new Result<TSuccess, TFailure>(value);

        /// <summary>
        /// Create a failure result.
        /// </summary>
        /// <param name="failure">The failure.</param>
        /// <returns>A new <see cref="Result{TSuccess,TFailure}"/>.</returns>
        public static IResult<TSuccess, TFailure> Failure(TFailure failure) =>
            new Result<TSuccess, TFailure>(failure);

        /// <summary>
        /// Get the value when <see cref="IsSuccess"/> else throws an <see cref="NullReferenceException"/>.
        /// </summary>
        /// <returns>The some value.</returns>
        public TSuccess GetSuccessUnsafe() =>
            this._success.GetValueUnsafe();

        /// <summary>
        /// Get the value when <see cref="IsFailure"/> else throws an <see cref="NullReferenceException"/>.
        /// </summary>
        /// <returns>The some value.</returns>
        public TFailure GetFailureUnsafe() =>
            this._failure.GetValueUnsafe();

        /// <inheritdoc />
        public override int GetHashCode() =>
            this.Match(
                success => success.GetHashCode(),
                failure => failure.GetHashCode());

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj switch
            {
                Result<TSuccess, TFailure> x => this.Equals(x),
                _ => false,
            };

        /// <inheritdoc />
        public bool Equals(Result<TSuccess, TFailure> other) =>
            this.AsResultValue() switch
            {
                SuccessValue<TSuccess> thisSuccess => other.AsResultValue() switch
                {
                    SuccessValue<TSuccess> otherSuccess => thisSuccess.Equals(otherSuccess),
                    _ => false,
                },

                FailureValue<TFailure> thisFailure => other.AsResultValue() switch
                {
                    FailureValue<TFailure> otherFailure => thisFailure.Equals(otherFailure),
                    _ => false,
                },

                _ => false,
            };

        /// <inheritdoc />
        public override string ToString() =>
            this.IsSuccess
                ? $"Success({this._success.GetValueUnsafe()})"
                : $"Failure({this._failure.GetValueUnsafe()})";
    }
}
