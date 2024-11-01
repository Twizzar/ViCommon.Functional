using System;
using static ViCommon.Functional.FunctionalCommon;

#pragma warning disable S2436 // Types and methods should not have too many generic parameters

namespace ViCommon.Functional.Monads.ResultMonad
{
    /// <content>
    /// Contains Combine methods for Result.
    /// </content>
    public readonly partial struct Result<TSuccess, TFailure>
    {
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
            where TOtherFailure : Failure =>
                this.BiBind(
                    bindSuccess: thisSuccess =>
                        other.BiBind(
                            otherSuccess => Result.Success<TNewSuccess, TNewFailure>(combineSuccessFunc(thisSuccess, otherSuccess)),
                            failure => Result.Failure<TNewSuccess, TNewFailure>(convertOtherFailureFunc(failure))),
                    bindFailure: thisFailure =>
                        other.BiBind(
                            otherSuccess => Result.Failure<TNewSuccess, TNewFailure>(convertThisFailureFunc(thisFailure)),
                            otherFailure => Result.Failure<TNewSuccess, TNewFailure>(
                                combineFailureFunc(
                                    convertThisFailureFunc(thisFailure),
                                    convertOtherFailureFunc(otherFailure)))));

        /// <summary>
        /// Combines two results to one.
        /// </summary>
        /// <typeparam name="TNewSuccess">The new success type.</typeparam>
        /// <typeparam name="TOtherSuccess">The other success type.</typeparam>
        /// <param name="other">The other failure.</param>
        /// <param name="combineSuccessFunc">
        /// Function which takes the Success value of this and other and combines them to a new success value.
        /// </param>
        /// <param name="combineFailureFunc">
        /// Function which combines both failure to the new failure type.
        /// </param>
        /// <returns>A new result which will be success if both are success else a failure.</returns>
        public IResult<TNewSuccess, TFailure> Combine<TNewSuccess, TOtherSuccess>(
            IResult<TOtherSuccess, TFailure> other,
            Func<TSuccess, TOtherSuccess, TNewSuccess> combineSuccessFunc,
            Func<TFailure, TFailure, TFailure> combineFailureFunc) =>
            this.Combine(
                other,
                combineSuccessFunc,
                Identity,
                Identity,
                combineFailureFunc);

        /// <summary>
        /// Combines two failures to one.
        /// </summary>
        /// <param name="other">The other failure.</param>
        /// <param name="combineSuccessFunc">
        /// Function which takes the Success value of this and other and combines them to a new success value.
        /// </param>
        /// <returns>A new result which will be success if both are success else a failure.</returns>
        public IResult<TSuccess, TFailure> Combine(
            IResult<TSuccess, TFailure> other,
            Func<TSuccess, TSuccess, TSuccess> combineSuccessFunc) =>
            this.Combine(
                other,
                combineSuccessFunc,
                Identity,
                Identity,
                (failure, failure1) => failure);
    }
}