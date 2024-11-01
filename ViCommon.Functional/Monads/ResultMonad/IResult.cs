using System;

#pragma warning disable CA1716 // Identifiers should not match keywords
#pragma warning disable S2436 // Types and methods should not have too many generic parameters

namespace ViCommon.Functional.Monads.ResultMonad
{
    /// <summary>
    /// Monad which represent a state where there two possible outcomes.
    /// A success and a failure.
    /// The interface has covariant success and failure type but therefore cannot provide
    /// as much functionality as <see cref="Result{TSuccess,TFailure}"/>.
    /// </summary>
    /// <typeparam name="TSuccess">Type of the success success.</typeparam>
    /// <typeparam name="TFailure">Type of the failure success.</typeparam>
    public interface IResult<out TSuccess, out TFailure>
        where TFailure : Failure
    {
        /// <summary>
        /// Gets a value indicating whether the result was a success.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets a value indicating whether the result was a failure.
        /// </summary>
        public bool IsFailure { get; }

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
        public object Case { get; }

        /// <summary>
        /// Get the value when <see cref="IsSuccess"/> else throws an <see cref="NullReferenceException"/>.
        /// </summary>
        /// <returns>The some value.</returns>
        public TSuccess GetSuccessUnsafe();

        /// <summary>
        /// Get the value when <see cref="IsFailure"/> else throws an <see cref="NullReferenceException"/>.
        /// </summary>
        /// <returns>The some value.</returns>
        public TFailure GetFailureUnsafe();

        /// <summary>
        /// Pattern matching on success and failure.
        /// </summary>
        /// <param name="onSuccess">When success.</param>
        /// <param name="onFailure">When failure.</param>
        /// <typeparam name="TRet">the type of the returned value.</typeparam>
        /// <returns>On success the return value of onSuccess else the return value of onFailure.</returns>
        public TRet Match<TRet>(Func<TSuccess, TRet> onSuccess, Func<TFailure, TRet> onFailure);

        /// <summary>
        /// Bi-bind. Allows mapping of both monad states.
        /// </summary>
        /// <typeparam name="TBindSuccess">A new success type.</typeparam>
        /// <typeparam name="TBindFailure">A new failure type.</typeparam>
        /// <param name="bindSuccess">The bind function if success.</param>
        /// <param name="bindFailure">The bind function if failure.</param>
        /// <returns>A new <see cref="Result{TBindSuccess,TBindFailure}"/>.</returns>
        public IResult<TBindSuccess, TBindFailure> BiBind<TBindSuccess, TBindFailure>(
            Func<TSuccess, IResult<TBindSuccess, TBindFailure>> bindSuccess,
            Func<TFailure, IResult<TBindSuccess, TBindFailure>> bindFailure)
            where TBindFailure : Failure;

        /// <summary>
        /// Combines two results to one.
        /// </summary>
        /// <param name="other">The other failure.</param>
        /// <param name="combineSuccessFunc">
        /// Function which takes the Success value of this and other and combines them to a new success value.
        /// </param>
        /// <param name="convertThisFailureFunc">
        /// Function which converts this failure to the new failure type.
        /// </param>
        /// <param name="convertOtherFailureFunc">
        /// Function which converts other failure to the new failure type.
        /// </param>
        /// <param name="combineFailureFunc">
        /// Function which combines both failure to the new failure type.
        /// </param>
        /// <typeparam name="TNewSuccess">The new success type.</typeparam>
        /// <typeparam name="TNewFailure">The new failure type.</typeparam>
        /// <typeparam name="TOtherSuccess">The other success type.</typeparam>
        /// <typeparam name="TOtherFailure">The other failure type.</typeparam>
        /// <returns>A new result which will be success if both are success else a failure.</returns>
        public IResult<TNewSuccess, TNewFailure> Combine<TNewSuccess, TNewFailure, TOtherSuccess, TOtherFailure>(
            IResult<TOtherSuccess, TOtherFailure> other,
            Func<TSuccess, TOtherSuccess, TNewSuccess> combineSuccessFunc,
            Func<TFailure, TNewFailure> convertThisFailureFunc,
            Func<TOtherFailure, TNewFailure> convertOtherFailureFunc,
            Func<TNewFailure, TNewFailure, TNewFailure> combineFailureFunc)
            where TNewFailure : Failure
            where TOtherFailure : Failure;

        /// <summary>
        /// Executes an action when <see cref="IsSuccess"/>.
        /// </summary>
        /// <param name="onSuccess">Action to execute.</param>
        public void IfSuccess(Action<TSuccess> onSuccess);

        /// <summary>
        /// Executes an action when <see cref="IsFailure"/>.
        /// </summary>
        /// <param name="onFailure">Action to execute.</param>
        public void IfFailure(Action<TFailure> onFailure);

        /// <summary>
        /// Executes onSuccess when successful else onFailure.
        /// </summary>
        /// <param name="onSuccess">The on success action.</param>
        /// <param name="onFailure">The on failure action.</param>
        public void Do(Action<TSuccess> onSuccess, Action<TFailure> onFailure);

        /// <summary>
        /// Applies the mapFuncSuccess to the result success value if is success and
        /// applies the mapFuncFailure to the result failure value if is failure.
        /// </summary>
        /// <param name="mapFuncSuccess">Function to apply on success.</param>
        /// <param name="mapFuncFailure">Function to apply on failure.</param>
        /// <typeparam name="TMapSuccess">New success value type.</typeparam>
        /// <typeparam name="TMapFailure">New failure value type.</typeparam>
        /// <returns>A new <see cref="Result{TSuccess,TFailure}"/>.</returns>
        public IResult<TMapSuccess, TMapFailure> Map<TMapSuccess, TMapFailure>(
            Func<TSuccess, TMapSuccess> mapFuncSuccess,
            Func<TFailure, TMapFailure> mapFuncFailure)
            where TMapFailure : Failure;

        /// <summary>
        /// Convert the result to <see cref="SuccessValue{TSuccess}"/> or <see cref="FailureValue{TFailure}"/>.
        /// </summary>
        /// <returns>If successful <see cref="SuccessValue{TSuccess}"/> otherwise <see cref="FailureValue{TFailure}"/>.</returns>
        public IResultValue AsResultValue();
    }
}
