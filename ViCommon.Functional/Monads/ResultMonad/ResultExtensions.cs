using System;
using System.Collections.Generic;
using System.Linq;
using static ViCommon.Functional.FunctionalCommon;

namespace ViCommon.Functional.Monads.ResultMonad
{
    /// <summary>
    /// Extension methods for the <see cref="Result{TSuccess,TFailure}"/> type.
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Monadic join. Flatten two nested Results to one.
        /// </summary>
        /// <typeparam name="TSuccess">The success type value.</typeparam>
        /// <typeparam name="TFailure">The failure type value.</typeparam>
        /// <param name="result">The nested result.</param>
        /// <returns>One Result instead of two nested results.</returns>
        public static IResult<TSuccess, TFailure> Flatten<TSuccess, TFailure>(
            this IResult<IResult<TSuccess, TFailure>, TFailure> result)
            where TFailure : Failure =>
            result.Bind(Identity);

        /// <summary>
        /// Monadic join. Flatten two nested Results to one.
        /// </summary>
        /// <typeparam name="TSuccess">The success type value.</typeparam>
        /// <typeparam name="TFailure">The failure type value.</typeparam>
        /// <param name="result">The nested result.</param>
        /// <returns>One Result instead of two nested results.</returns>
        public static IResult<TSuccess, TFailure> Flatten<TSuccess, TFailure>(
            this Result<Result<TSuccess, TFailure>, TFailure> result)
            where TFailure : Failure =>
            result.Bind(r => r);

        /// <summary>
        /// Extracts from a list of Results all the <c>IsSuccess</c> elements.
        /// All the <c>IsSuccess</c> elements are extracted in order.
        /// </summary>
        /// <typeparam name="TSuccess">The success value type.</typeparam>
        /// <typeparam name="TFailure">The failure value type.</typeparam>
        /// <param name="self">The sequence of Results.</param>
        /// <returns>Returns a list of TSuccess.</returns>
        public static IEnumerable<TSuccess> Successes<TSuccess, TFailure>(
            this IEnumerable<IResult<TSuccess, TFailure>> self)
            where TFailure : Failure
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            return
                self
                    .Where(result => result.IsSuccess)
                    .Select(
                        result => result.GetSuccessUnsafe());
        }

        /// <summary>
        /// Combines two failure with no return value.
        /// </summary>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="self">The first result.</param>
        /// <param name="other">The second result.</param>
        /// <returns>Success when both are successful else failure or aggregate failure when both are failure.</returns>
        public static IResult<Unit, Failure> Combine<TFailure>(
            this IResult<Unit, TFailure> self,
            IResult<Unit, TFailure> other)
            where TFailure : Failure =>
            self.Combine(
                other,
                (unit, success) => Unit.New,
                failure => (Failure)failure,
                failure => (Failure)failure,
                (f1, f2) => new AggregateFailure(f1, f2));

        /// <summary>
        /// Combines two failure with no return value.
        /// </summary>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="self">The first result.</param>
        /// <param name="other">The second result.</param>
        /// <param name="combineFailures">Function which describes how to combine two failures.</param>
        /// <returns>Success when both are successful else failure or aggregate failure when both are failure.</returns>
        public static IResult<Unit, Failure> Combine<TFailure>(
            this IResult<Unit, TFailure> self,
            IResult<Unit, TFailure> other,
            Func<Failure, Failure, Failure> combineFailures)
            where TFailure : Failure =>
            self.Combine(
                other,
                (unit, success) => Unit.New,
                failure => (Failure)failure,
                failure => (Failure)failure,
                combineFailures);

        /// <summary>
        /// Aggregates all results.
        /// </summary>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="self">A sequence of results.</param>
        /// <returns>Success or else a failure if one failed or else aggregate failure.</returns>
        public static IResult<Unit, Failure> Aggregate<TFailure>(
            this IEnumerable<IResult<Unit, TFailure>> self)
            where TFailure : Failure =>
                self.Aggregate(
                    Result.Success<Unit, Failure>(Unit.New),
                    (a, b) =>
                        a.Combine(b, CombineFailures));

        private static AggregateFailure CombineFailures(Failure f1, Failure f2)
        {
            if (f1 is AggregateFailure a1)
            {
                return f2 is AggregateFailure a2
                    ? a1.Combine(a2)
                    : a1.Add(f2);
            }
            else
            {
                return new AggregateFailure(f1, f2);
            }
        }
    }
}
