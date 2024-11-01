using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using ViCommon.Functional.Extensions;

using static ViCommon.Functional.FunctionalCommon;

namespace ViCommon.Functional.Monads.ResultMonad
{
    /// <summary>
    /// Static Methods for the Result monad. Normally we would call this ResultHelper,
    /// but for readability this is called Result.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Result
    {
        /// <summary>
        /// Create a success result.
        /// </summary>
        /// <typeparam name="TSuccess">The success type.</typeparam>
        /// <param name="value">The success value.</param>
        /// <returns>A new success monad.</returns>
        public static SuccessValue<TSuccess> Success<TSuccess>(TSuccess value) =>
            new SuccessValue<TSuccess>(value);

        /// <summary>
        /// Create a success result without a success value.
        /// </summary>
        /// <returns>A new success monad.</returns>
        public static SuccessValue<Unit> Success() =>
            new SuccessValue<Unit>(Unit.New);

        /// <summary>
        /// Create a success result without a success value.
        /// </summary>
        /// <typeparam name="TFailure">The failure type.</typeparam>
        /// <returns>A new success monad.</returns>
        public static Result<Unit, TFailure> Success<TFailure>()
            where TFailure : Failure =>
            new SuccessValue<Unit>(Unit.New);

        /// <summary>
        /// Create a success result.
        /// </summary>
        /// <typeparam name="TSuccess">The success type.</typeparam>
        /// <typeparam name="TFailure">The failure type.</typeparam>
        /// <param name="value">The success value.</param>
        /// <returns>A new success monad.</returns>
        public static IResult<TSuccess, TFailure> Success<TSuccess, TFailure>(TSuccess value)
            where TFailure : Failure =>
            Result<TSuccess, TFailure>.Success(value);

        /// <summary>
        /// Create a success result without a success value.
        /// </summary>
        /// <typeparam name="TFailure">The failure type.</typeparam>
        /// <returns>A new success monad.</returns>
        public static Task<IResult<Unit, TFailure>> SuccessAsync<TFailure>()
            where TFailure : Failure =>
            Task.FromResult(Success<Unit, TFailure>(Unit.New));

        /// <summary>
        /// Create a success result.
        /// </summary>
        /// <typeparam name="TSuccess">The success type.</typeparam>
        /// <typeparam name="TFailure">The failure type.</typeparam>
        /// <param name="valueTask">The success value task.</param>
        /// <returns>A new success monad.</returns>
        public static Task<IResult<TSuccess, TFailure>> SuccessAsync<TSuccess, TFailure>(Task<TSuccess> valueTask)
            where TFailure : Failure =>
            valueTask.Map(Success<TSuccess, TFailure>);

        /// <summary>
        /// Create a success result.
        /// </summary>
        /// <typeparam name="TSuccess">The success type.</typeparam>
        /// <typeparam name="TFailure">The failure type.</typeparam>
        /// <param name="value">The success value.</param>
        /// <returns>A new success monad.</returns>
        public static Task<IResult<TSuccess, TFailure>> SuccessAsync<TSuccess, TFailure>(TSuccess value)
            where TFailure : Failure =>
            Task.FromResult(Success<TSuccess, TFailure>(value));

        /// <summary>
        /// Create a success result.
        /// </summary>
        /// <typeparam name="TSuccess">The success type.</typeparam>
        /// <param name="value">The success value.</param>
        /// <returns>A new success monad.</returns>
        public static IResult<TSuccess, Failure> Ok<TSuccess>(TSuccess value) =>
            Result<TSuccess, Failure>.Success(value);

        /// <summary>
        /// Create a failure result.
        /// </summary>
        /// <typeparam name="TFailure">The failure type.</typeparam>
        /// <param name="failure">The failure value.</param>
        /// <returns>A new success monad.</returns>
        public static FailureValue<TFailure> Failure<TFailure>(TFailure failure)
            where TFailure : Failure =>
            new FailureValue<TFailure>(failure);

        /// <summary>
        /// Create a failure result.
        /// </summary>
        /// <typeparam name="TSuccess">The success type.</typeparam>
        /// <typeparam name="TFailure">The failure type.</typeparam>
        /// <param name="failure">The failure value.</param>
        /// <returns>A new success monad.</returns>
        public static IResult<TSuccess, TFailure> Failure<TSuccess, TFailure>(
            TFailure failure)
            where TFailure : Failure =>
            Result<TSuccess, TFailure>.Failure(failure);

        /// <summary>
        /// Create a failure result.
        /// </summary>
        /// <typeparam name="TSuccess">The success type.</typeparam>
        /// <typeparam name="TFailure">The failure type.</typeparam>
        /// <param name="failure">The failure value.</param>
        /// <returns>A new success monad.</returns>
        public static Task<IResult<TSuccess, TFailure>> FailureAsync<TSuccess, TFailure>(TFailure failure)
            where TFailure : Failure =>
            Task.FromResult(Failure<TSuccess, TFailure>(failure));

        /// <summary>
        /// Create a failure result.
        /// </summary>
        /// <typeparam name="TSuccess">The success type.</typeparam>
        /// <typeparam name="TFailure">The failure type.</typeparam>
        /// <param name="failure">The failure value.</param>
        /// <returns>A new success monad.</returns>
        public static Task<IResult<TSuccess, TFailure>> FailureAsync<TSuccess, TFailure>(Task<TFailure> failure)
            where TFailure : Failure =>
            failure.Map(Failure<TSuccess, TFailure>);

        /// <summary>
        /// Converts a predicate to a Result.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="message">The failure message.</param>
        /// <returns>Success when the predicate is true Failure when the predicate is null or false.</returns>
        public static IResult<Unit, Failure> ToResult(Func<bool> predicate, string message = "The predicate was false.") =>
            predicate != null && predicate()
                ? Success<Unit, Failure>(Unit.New)
                : Failure<Unit, Failure>(new Failure(message));

        /// <summary>
        /// Converts to <see cref="Result{TSuccess,TFailure}"/>.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="iResult">The i result.</param>
        /// <returns>The result class.</returns>
        public static Result<TSuccess, TFailure> ToResult<TSuccess, TFailure>(IResult<TSuccess, TFailure> iResult)
            where TFailure : Failure =>
            iResult.ToResult();

        /// <summary>
        /// Convert <see cref="Result{TSuccess,TFailure}"/> the covariant <see cref="IResult{TSuccess,TFailure}"/>.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="result">The result.</param>
        /// <returns>The result as the interface.</returns>
        public static IResult<TSuccess, TFailure> AsCovariant<TSuccess, TFailure>(Result<TSuccess, TFailure> result)
            where TFailure : Failure =>
            result.AsCovariant();

        /// <summary>
        /// Aggregates all results to one.
        /// </summary>
        /// <typeparam name="TSuccess">The success type.</typeparam>
        /// <typeparam name="TFailure">The failure type.</typeparam>
        /// <param name="results">A sequence of results.</param>
        /// <param name="combineFunc">Function to combine two success values.</param>
        /// <returns>A aggregated result.</returns>
        public static IResult<TSuccess, TFailure> Aggregate<TSuccess, TFailure>(
            IEnumerable<IResult<TSuccess, TFailure>> results,
            Func<TSuccess, TSuccess, TSuccess> combineFunc)
            where TFailure : Failure =>
            results.Aggregate((a, b) => ((Result<TSuccess, TFailure>)a).Combine(b, combineFunc));

        /// <summary>
        /// Applies the mapFunction to the result success value if is success
        /// and returns a result of the output type of the mapFunction.
        /// </summary>
        /// <typeparam name="TSuccess">The success value.</typeparam>
        /// <typeparam name="TFailure">The failure value.</typeparam>
        /// <typeparam name="TMap">Output type of the mapFunction.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="mapFunc">Function to apply.</param>
        /// <returns>Success(mapFunction(value)) if some else Failure.</returns>
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "IResult can only be form the struct type Result")]
        public static IResult<TMap, TFailure> MapSuccess<TSuccess, TFailure, TMap>(
            IResult<TSuccess, TFailure> result,
            Func<TSuccess, TMap> mapFunc)
            where TFailure : Failure =>
            result.Map(mapFunc, Identity);

        /// <summary>
        /// Replaces one contained object with another contained object.
        /// It can be viewed as a combination of <see cref="MapSuccess{TSuccess,TFailure,TMap}"/> and
        /// <see cref="ResultExtensions.Flatten{TSuccess,TFailure}(IResult{IResult{TSuccess,TFailure},TFailure})"/>.
        /// Useful for chaining many Results operations.
        /// </summary>
        /// <typeparam name="TSuccess">The success value type.</typeparam>
        /// <typeparam name="TFailure">The failure value type.</typeparam>
        /// <typeparam name="TBind">The value type of the new result.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="bindFunc">A function which takes the value as input and returns a new result.</param>
        /// <returns>A new result.</returns>
        public static IResult<TBind, TFailure> Bind<TSuccess, TFailure, TBind>(
            IResult<TSuccess, TFailure> result,
            Func<TSuccess, IResult<TBind, TFailure>> bindFunc)
            where TFailure : Failure =>
            result.Bind(bindFunc);

        /// <summary>
        /// Pattern match the result.
        /// </summary>
        /// <typeparam name="TSuccess">The success value type.</typeparam>
        /// <typeparam name="TFailure">The failure value type.</typeparam>
        /// <typeparam name="TRet">The type of the returned value.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="success">Function to execute on success.</param>
        /// <param name="failure">Function to execute on failure.</param>
        /// <returns>Returns the output of some when is some else returns none.</returns>
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "IResult can only be form the struct type Result")]
        public static TRet Match<TSuccess, TFailure, TRet>(
            IResult<TSuccess, TFailure> result,
            Func<TSuccess, TRet> success,
            Func<TFailure, TRet> failure)
            where TFailure : Failure =>
            result.Match(success, failure);

        /// <summary>
        /// Pattern match the result.
        /// </summary>
        /// <typeparam name="TSuccess">The success value type.</typeparam>
        /// <typeparam name="TFailure">The failure value type.</typeparam>
        /// <typeparam name="TRet">The type of the returned value.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="success">Function to execute on success.</param>
        /// <param name="failure">Value to return on failure.</param>
        /// <returns>Returns the output of some when is some else returns none.</returns>
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "IResult can only be form the struct type Result")]
        public static TRet Match<TSuccess, TFailure, TRet>(
            IResult<TSuccess, TFailure> result,
            Func<TSuccess, TRet> success,
            TRet failure)
            where TFailure : Failure =>
            result.Match(success, f => failure);
    }
}