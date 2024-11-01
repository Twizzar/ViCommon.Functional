using System;
using static ViCommon.Functional.FunctionalCommon;

#pragma warning disable S2436 // Types and methods should not have too many generic parameters

namespace ViCommon.Functional.Monads.ResultMonad
{
    /// <summary>
    /// Extension methods for implementing the missing method form <see cref="IResult{TSuccess,TFailure}"/>
    /// because the covariant type parameters are not allowed to be used as return value in the interface.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IResultExtensions
    {
        /// <summary>
        /// Converts a <see cref="IResult{TSuccess,TFailure}"/> to a <see cref="Result"/>.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="result">The result.</param>
        /// <returns>A result.</returns>
        public static Result<TSuccess, TFailure> ToResult<TSuccess, TFailure>(this IResult<TSuccess, TFailure> result)
            where TFailure : Failure =>
            result.IsSuccess
                ? Result.Success(result.GetSuccessUnsafe())
                : Result.Failure(result.GetFailureUnsafe());

        /// <summary>
        /// Convert to success when the isSuccess predicate returns true;
        /// Else false.
        /// </summary>
        /// <typeparam name="TSuccess">The success type.</typeparam>
        /// <typeparam name="TFailure">The failure type.</typeparam>
        /// <param name="value">The success vale.</param>
        /// <param name="failure">The failure value.</param>
        /// <param name="isSuccessful">Predicate to determine if successful.</param>
        /// <returns>A new <see cref="IResult{TSuccess,TFailure}"/>.</returns>
        public static IResult<TSuccess, TFailure> ToResult<TSuccess, TFailure>(
            this TSuccess value,
            TFailure failure,
            Func<TSuccess, bool> isSuccessful)
            where TFailure : Failure =>
            isSuccessful(value)
                ? Result.Success<TSuccess, TFailure>(value)
                : Result.Failure<TSuccess, TFailure>(failure);

        /// <summary>
        /// Convert a failure to a result failure.
        /// </summary>
        /// <typeparam name="TSuccess">The success type.</typeparam>
        /// <typeparam name="TFailure">The failure type.</typeparam>
        /// <param name="failure">The failure.</param>
        /// <returns>A new <see cref="IResult{TSuccess,TFailure}"/>.</returns>
        public static IResult<TSuccess, TFailure> ToResult<TSuccess, TFailure>(this TFailure failure)
            where TFailure : Failure =>
            Result.Failure<TSuccess, TFailure>(failure);

        /// <summary>
        /// Converts a <see cref="IResult{TSuccess,TFailure}"/> to a <see cref="Result"/>.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="result">The result.</param>
        /// <returns>A result.</returns>
        public static IResult<TSuccess, TFailure> AsCovariant<TSuccess, TFailure>(this Result<TSuccess, TFailure> result)
            where TFailure : Failure =>
            result;

        /// <summary>
        /// Monadic bind.
        /// </summary>
        /// <typeparam name="TBind">A new success type.</typeparam>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="bindFunc">The bind function.</param>
        /// <returns>
        /// A new <see cref="Result{TBind,TFailure}" />.
        /// </returns>
        public static IResult<TBind, TFailure> Bind<TBind, TSuccess, TFailure>(
            this IResult<TSuccess, TFailure> result,
            Func<TSuccess, IResult<TBind, TFailure>> bindFunc)
            where TFailure : Failure =>
            result.ToResult().Bind(bindFunc);

        /// <summary>
        /// Binds the failure only.
        /// </summary>
        /// <typeparam name="TBind">A new failure type.</typeparam>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="bindFunc">Bind function.</param>
        /// <returns>
        /// A new <see cref="Result{TSuccess,TBind}" />.
        /// </returns>
        public static IResult<TSuccess, TBind> BindFailure<TBind, TSuccess, TFailure>(
            this IResult<TSuccess, TFailure> result,
            Func<TFailure, IResult<TSuccess, TBind>> bindFunc)
            where TFailure : Failure
            where TBind : Failure =>
            result.ToResult().BindFailure(bindFunc);

        /// <summary>
        /// Combines two results to one.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TNewSuccess">The new success type.</typeparam>
        /// <typeparam name="TOtherSuccess">The other success type.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="other">The other failure.</param>
        /// <param name="combineSuccessFunc">Function which takes the Success value of this and other and combines them to a new success value.</param>
        /// <param name="combineFailureFunc">Function which combines both failure to the new failure type.</param>
        /// <returns>
        /// A new result which will be success if both are success else a failure.
        /// </returns>
        public static IResult<TNewSuccess, TFailure> Combine<TSuccess, TFailure, TNewSuccess, TOtherSuccess>(
            this IResult<TSuccess, TFailure> result,
            IResult<TOtherSuccess, TFailure> other,
            Func<TSuccess, TOtherSuccess, TNewSuccess> combineSuccessFunc,
            Func<TFailure, TFailure, TFailure> combineFailureFunc)
            where TFailure : Failure =>
            result.ToResult().Combine(
                other,
                combineSuccessFunc,
                Identity,
                Identity,
                combineFailureFunc);

        /// <summary>
        /// Combines two failures to one.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="other">The other failure.</param>
        /// <param name="combineSuccessFunc">Function which takes the Success value of this and other and combines them to a new success value.</param>
        /// <returns>
        /// A new result which will be success if both are success else a failure.
        /// </returns>
        public static IResult<TSuccess, TFailure> Combine<TSuccess, TFailure>(
            this IResult<TSuccess, TFailure> result,
            IResult<TSuccess, TFailure> other,
            Func<TSuccess, TSuccess, TSuccess> combineSuccessFunc)
            where TFailure : Failure =>
            result.ToResult().Combine(
                other,
                combineSuccessFunc,
                Identity,
                Identity,
                (failure, failure1) => failure);

        /// <summary>
        /// Implementation for the linq select many. Can be used for linq expressions.
        /// <example><code>
        /// var one = Success&lt;int, Failure&gt;(1);
        /// var two = Success&lt;int, Failure&gt;(2);
        /// var three = Failure&lt;int, Failure&gt;(new Failure("Error"));
        /// var result = Match(
        /// from x in one
        /// from y in two
        /// from z in three
        /// select x + y + z,
        /// success: i =&gt; i * 2,
        /// failure: 0);
        /// </code></example>
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TIntermediate">The intermediate type.</typeparam>
        /// <typeparam name="TBind">The result value type.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="mapper">A transform function to apply to the value.</param>
        /// <param name="getResult">A transform function to apply to the intermediate.</param>
        /// <returns>
        /// A new Maybe.
        /// </returns>
        public static IResult<TBind, TFailure> SelectMany<TSuccess, TFailure, TIntermediate, TBind>(
            this IResult<TSuccess, TFailure> result,
            Func<TSuccess, IResult<TIntermediate, TFailure>> mapper,
            Func<TSuccess, TIntermediate, TBind> getResult)
            where TFailure : Failure =>
            result.ToResult().SelectMany(mapper, getResult);

        /// <summary>
        /// Applies the mapFunction to the result success value if is success
        /// and returns a result of the output type of the mapFunction.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TMap">Output type of the mapFunction.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="mapFunc">Function to apply.</param>
        /// <returns>
        /// Success(mapFunction(SuccessValue)) if success else old failure value.
        /// </returns>
        public static IResult<TMap, TFailure> MapSuccess<TSuccess, TFailure, TMap>(
            this IResult<TSuccess, TFailure> result,
            Func<TSuccess, TMap> mapFunc)
            where TFailure : Failure =>
            result.ToResult().MapSuccess(mapFunc);

        /// <summary>
        /// Applies the mapFunction to the result failure value if is failure
        /// and returns a result of the output type of the mapFunction.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TMap">Output type of the mapFunction.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="mapFunc">Function to apply.</param>
        /// <returns>
        /// Failure(mapFunction(failureValue)) if failure else old success value.
        /// </returns>
        public static IResult<TSuccess, TMap> MapFailure<TSuccess, TFailure, TMap>(
            this IResult<TSuccess, TFailure> result,
            Func<TFailure, TMap> mapFunc)
            where TMap : Failure
            where TFailure : Failure =>
            result.ToResult().MapFailure(mapFunc);

        /// <summary>
        /// Select implementation for using linq expressions. Same as <see cref="MapSuccess{TSuccess,TFailure,TMap}" />.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TResult">Output type of the mapFunction.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="mapFunction">Function to apply.</param>
        /// <returns>
        /// Success(mapFunction(SuccessValue)) if success else old failure value.
        /// </returns>
        public static IResult<TResult, TFailure> Select<TSuccess, TFailure, TResult>(
            this IResult<TSuccess, TFailure> result,
            Func<TSuccess, TResult> mapFunction)
            where TFailure : Failure =>
            result.ToResult().Select(mapFunction);

        /// <summary>
        /// Pattern matching return success or convert failure to success.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="onFailure">When failure.</param>
        /// <returns>
        /// On success returns success value else execute onFailure.
        /// </returns>
        public static TSuccess Match<TSuccess, TFailure>(
            this IResult<TSuccess, TFailure> result,
            Func<TFailure, TSuccess> onFailure)
            where TFailure : Failure =>
            result.ToResult().Match(Identity, onFailure);

        /// <summary>
        /// Pattern matching return failure or convert success to failure.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="onSuccess">When success.</param>
        /// <returns>
        /// On failure returns failure value else execute onSuccess.
        /// </returns>
        public static TFailure Match<TSuccess, TFailure>(
            this IResult<TSuccess, TFailure> result,
            Func<TSuccess, TFailure> onSuccess)
            where TFailure : Failure =>
            result.ToResult().Match(onSuccess, Identity);

        /// <summary>
        /// Convert a value to a success result.
        /// </summary>
        /// <typeparam name="TSuccess">The success value type.</typeparam>
        /// <typeparam name="TFailure">The failure type.</typeparam>
        /// <param name="value">The success value.</param>
        /// <returns>A new <see cref="IResult{TSuccess,TFailure}"/>.</returns>
        public static IResult<TSuccess, TFailure> ToSuccess<TSuccess, TFailure>(this TSuccess value)
            where TFailure : Failure =>
            Result.Success<TSuccess, TFailure>(value);
    }
}
