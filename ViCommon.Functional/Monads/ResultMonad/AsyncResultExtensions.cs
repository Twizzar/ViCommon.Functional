using System;
using System.Threading.Tasks;
using ViCommon.Functional.Extensions;

#pragma warning disable SA1202 // Elements should be ordered by access
#pragma warning disable S2436 // Types and methods should not have too many generic parameters

namespace ViCommon.Functional.Monads.ResultMonad
{
    /// <summary>
    /// Async methods for the <see cref="Result{TSuccess,TFailure}"/> monad.
    /// </summary>
    public static class AsyncResultExtensions
    {
        /// <summary>
        /// Gets a value indicating whether the result was a success.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <returns>True if it was a success.</returns>
        public static Task<bool> IsSuccessAsync<TSuccess, TFailure>(this Task<IResult<TSuccess, TFailure>> resultTask)
            where TFailure : Failure =>
            resultTask.Map(result => result.IsSuccess);

        /// <summary>
        /// Gets a value indicating whether the result was a failure.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <returns>True if it was a failure.</returns>
        public static Task<bool> IsFailureAsync<TSuccess, TFailure>(
            this Task<IResult<TSuccess, TFailure>> resultTask)
            where TFailure : Failure =>
            resultTask.Map(result => result.IsFailure);

        /// <summary>
        /// Get the value when <see cref="IsSuccessAsync{TSuccess,TFailure}" /> else throws an <see cref="NullReferenceException" />.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <returns>
        /// The some value.
        /// </returns>
        public static Task<TSuccess> GetSuccessUnsafeAsync<TSuccess, TFailure>(
            this Task<IResult<TSuccess, TFailure>> resultTask)
            where TFailure : Failure =>
            resultTask.Map(result => result.GetSuccessUnsafe());

        /// <summary>
        /// Get the value when <see cref="IsFailureAsync{TSuccess,TFailure}" /> else throws an <see cref="NullReferenceException" />.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <returns>
        /// The some value.
        /// </returns>
        public static Task<TFailure> GetFailureUnsafeAsync<TSuccess, TFailure>(
            this Task<IResult<TSuccess, TFailure>> resultTask)
            where TFailure : Failure =>
            resultTask.Map(result => result.GetFailureUnsafe());

        #region Map

        /// <summary>
        /// Applies the mapFuncSuccess to the result success value if is success and
        /// applies the mapFuncFailure to the result failure value if is failure.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TMapSuccess">New success value type.</typeparam>
        /// <typeparam name="TMapFailure">New failure value type.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <param name="mapFuncSuccess">Function to apply on success.</param>
        /// <param name="mapFuncFailure">Function to apply on failure.</param>
        /// <returns>
        /// A new <see cref="Result{TSuccess,TFailure}" />.
        /// </returns>
        public static Task<IResult<TMapSuccess, TMapFailure>> MapAsync<TSuccess, TFailure, TMapSuccess, TMapFailure>(
            this Task<IResult<TSuccess, TFailure>> resultTask,
            Func<TSuccess, TMapSuccess> mapFuncSuccess,
            Func<TFailure, TMapFailure> mapFuncFailure)
            where TFailure : Failure
            where TMapFailure : Failure
        {
            if (resultTask == null)
            {
                throw new ArgumentNullException(nameof(resultTask));
            }

            if (mapFuncSuccess == null)
            {
                throw new ArgumentNullException(nameof(mapFuncSuccess));
            }

            if (mapFuncFailure == null)
            {
                throw new ArgumentNullException(nameof(mapFuncFailure));
            }

            return resultTask.Map(result => result.Map(mapFuncSuccess, mapFuncFailure));
        }

        /// <summary>
        /// Applies the mapFuncSuccess to the result success value if is success and
        /// applies the mapFuncFailure to the result failure value if is failure.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TMapSuccess">New success value type.</typeparam>
        /// <typeparam name="TMapFailure">New failure value type.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="mapFuncSuccess">Function to apply on success.</param>
        /// <param name="mapFuncFailure">Function to apply on failure.</param>
        /// <returns>
        /// A new <see cref="Result{TSuccess,TFailure}" />.
        /// </returns>
        public static Task<IResult<TMapSuccess, TMapFailure>> MapAsync<TSuccess, TFailure, TMapSuccess, TMapFailure>(
            this IResult<TSuccess, TFailure> result,
            Func<TSuccess, Task<TMapSuccess>> mapFuncSuccess,
            Func<TFailure, Task<TMapFailure>> mapFuncFailure)
            where TFailure : Failure
            where TMapFailure : Failure
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (mapFuncSuccess == null)
            {
                throw new ArgumentNullException(nameof(mapFuncSuccess));
            }

            if (mapFuncFailure == null)
            {
                throw new ArgumentNullException(nameof(mapFuncFailure));
            }

            return MapAsyncInternal(result, mapFuncSuccess, mapFuncFailure);
        }

        /// <summary>
        /// Applies the mapFuncSuccess to the result success value if is success and
        /// applies the mapFuncFailure to the result failure value if is failure.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TMapSuccess">New success value type.</typeparam>
        /// <typeparam name="TMapFailure">New failure value type.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <param name="mapFuncSuccess">Function to apply on success.</param>
        /// <param name="mapFuncFailure">Function to apply on failure.</param>
        /// <returns>
        /// A new <see cref="Result{TSuccess,TFailure}" />.
        /// </returns>
        public static Task<IResult<TMapSuccess, TMapFailure>> MapAsync<TSuccess, TFailure, TMapSuccess, TMapFailure>(
            this Task<IResult<TSuccess, TFailure>> resultTask,
            Func<TSuccess, Task<TMapSuccess>> mapFuncSuccess,
            Func<TFailure, Task<TMapFailure>> mapFuncFailure)
            where TFailure : Failure
            where TMapFailure : Failure
        {
            if (resultTask == null)
            {
                throw new ArgumentNullException(nameof(resultTask));
            }

            if (mapFuncSuccess == null)
            {
                throw new ArgumentNullException(nameof(mapFuncSuccess));
            }

            if (mapFuncFailure == null)
            {
                throw new ArgumentNullException(nameof(mapFuncFailure));
            }

            return resultTask.Bind(result => result.MapAsync(mapFuncSuccess, mapFuncFailure));
        }

        private static async Task<IResult<TMapSuccess, TMapFailure>> MapAsyncInternal<TSuccess, TFailure, TMapSuccess,
            TMapFailure>(
            this IResult<TSuccess, TFailure> result,
            Func<TSuccess, Task<TMapSuccess>> mapFuncSuccess,
            Func<TFailure, Task<TMapFailure>> mapFuncFailure)
            where TFailure : Failure
            where TMapFailure : Failure =>
            result.AsResultValue() switch
            {
                FailureValue<TFailure> f =>
                    Result<TMapSuccess, TMapFailure>.Failure(
                        await mapFuncFailure(f).ConfigureAwait(false)),
                SuccessValue<TSuccess> s =>
                    Result<TMapSuccess, TMapFailure>.Success(
                        await mapFuncSuccess(s).ConfigureAwait(false)),
                _ => throw new PatternErrorBuilder(nameof(result))
                    .IsNotOneOf(nameof(FailureValue<TFailure>), nameof(SuccessValue<TSuccess>)),
            };

        /// <summary>
        /// Applies the mapFunction to the result success value if is success
        /// and returns a result of the output type of the mapFunction.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TMap">Output type of the mapFunction.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <param name="mapFunc">Function to apply.</param>
        /// <returns>
        /// Success(mapFunction(SuccessValue)) if success else old failure value.
        /// </returns>
        public static Task<IResult<TMap, TFailure>> MapSuccessAsync<TSuccess, TFailure, TMap>(
            this Task<IResult<TSuccess, TFailure>> resultTask,
            Func<TSuccess, TMap> mapFunc)
            where TFailure : Failure
        {
            if (resultTask == null)
            {
                throw new ArgumentNullException(nameof(resultTask));
            }

            if (mapFunc == null)
            {
                throw new ArgumentNullException(nameof(mapFunc));
            }

            return resultTask.Map(result => result.MapSuccess(mapFunc));
        }

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
        /// <exception cref="ArgumentNullException">mapFunc.</exception>
        public static Task<IResult<TMap, TFailure>> MapSuccessAsync<TSuccess, TFailure, TMap>(
            this IResult<TSuccess, TFailure> result,
            Func<TSuccess, Task<TMap>> mapFunc)
            where TFailure : Failure =>
            result.MapAsync(mapFunc, Task.FromResult);

        /// <summary>
        /// Applies the mapFunction to the result success value if is success
        /// and returns a result of the output type of the mapFunction.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TMap">Output type of the mapFunction.</typeparam>
        /// <param name="resultTask">the result.</param>
        /// <param name="mapFunc">Function to apply.</param>
        /// <returns>
        /// Success(mapFunction(SuccessValue)) if success else old failure value.
        /// </returns>
        /// <exception cref="ArgumentNullException">mapFunc.</exception>
        public static Task<IResult<TMap, TFailure>> MapSuccessAsync<TSuccess, TFailure, TMap>(
            this Task<IResult<TSuccess, TFailure>> resultTask,
            Func<TSuccess, Task<TMap>> mapFunc)
            where TFailure : Failure =>
            resultTask.Bind(result => result.MapSuccessAsync(mapFunc));

        /// <summary>
        /// Applies the mapFunction to the result failure value if is failure
        /// and returns a result of the output type of the mapFunction.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TMap">Output type of the mapFunction.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <param name="mapFunc">Function to apply.</param>
        /// <returns>
        /// Failure(mapFunction(failureValue)) if failure else old success value.
        /// </returns>
        public static Task<IResult<TSuccess, TMap>> MapFailureAsync<TSuccess, TFailure, TMap>(
            this Task<IResult<TSuccess, TFailure>> resultTask,
            Func<TFailure, TMap> mapFunc)
            where TMap : Failure
            where TFailure : Failure =>
            resultTask.Map(result => result.MapFailure(mapFunc));

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
        /// <exception cref="ArgumentNullException">
        /// mapFunc
        /// or
        /// result.
        /// </exception>
        public static Task<IResult<TSuccess, TMap>> MapFailureAsync<TSuccess, TFailure, TMap>(
            this IResult<TSuccess, TFailure> result,
            Func<TFailure, Task<TMap>> mapFunc)
            where TMap : Failure
            where TFailure : Failure =>
            result.MapAsync(Task.FromResult, mapFunc);

        /// <summary>
        /// Applies the mapFunction to the result failure value if is failure
        /// and returns a result of the output type of the mapFunction.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TMap">Output type of the mapFunction.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <param name="mapFunc">Function to apply.</param>
        /// <returns>
        /// Failure(mapFunction(failureValue)) if failure else old success value.
        /// </returns>
        public static Task<IResult<TSuccess, TMap>> MapFailureAsync<TSuccess, TFailure, TMap>(
            this Task<IResult<TSuccess, TFailure>> resultTask,
            Func<TFailure, Task<TMap>> mapFunc)
            where TMap : Failure
            where TFailure : Failure =>
            resultTask.Bind(result => result.MapFailureAsync(mapFunc));

        #endregion

        #region Bind

        /// <summary>
        /// Monadic bind.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TBind">A new success type.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <param name="bindFunc">The bind function.</param>
        /// <returns>
        /// A new <see cref="Result{TBind,TFailure}" />.
        /// </returns>
        public static Task<IResult<TBind, TFailure>> BindAsync<TSuccess, TFailure, TBind>(
            this Task<IResult<TSuccess, TFailure>> resultTask,
            Func<TSuccess, IResult<TBind, TFailure>> bindFunc)
            where TFailure : Failure
        {
            if (resultTask == null)
            {
                throw new ArgumentNullException(nameof(resultTask));
            }

            if (bindFunc == null)
            {
                throw new ArgumentNullException(nameof(bindFunc));
            }

            return resultTask.Map(result => result.Bind(bindFunc));
        }

        /// <summary>
        /// Monadic bind.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TBind">A new success type.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="bindFunc">The bind function.</param>
        /// <returns>
        /// A new <see cref="Result{TBind,TFailure}" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// result
        /// or
        /// bindFunc.
        /// </exception>
        public static Task<IResult<TBind, TFailure>> BindAsync<TSuccess, TFailure, TBind>(
            this IResult<TSuccess, TFailure> result,
            Func<TSuccess, Task<IResult<TBind, TFailure>>> bindFunc)
            where TFailure : Failure
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (bindFunc == null)
            {
                throw new ArgumentNullException(nameof(bindFunc));
            }

            return BindAsyncInternal(result, bindFunc);
        }

        /// <summary>
        /// Monadic bind.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TBind">A new success type.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <param name="bindFunc">The bind function.</param>
        /// <returns>
        /// A new <see cref="Result{TBind,TFailure}" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">result
        /// or
        /// bindFunc.</exception>
        public static Task<IResult<TBind, TFailure>> BindAsync<TSuccess, TFailure, TBind>(
            this Task<IResult<TSuccess, TFailure>> resultTask,
            Func<TSuccess, Task<IResult<TBind, TFailure>>> bindFunc)
            where TFailure : Failure =>
            resultTask.Bind(result => result.BindAsync(bindFunc));

        private static async Task<IResult<TBind, TFailure>> BindAsyncInternal<TSuccess, TFailure, TBind>(
            this IResult<TSuccess, TFailure> result,
            Func<TSuccess, Task<IResult<TBind, TFailure>>> bindFunc)
            where TFailure : Failure =>
            result.AsResultValue() switch
            {
                SuccessValue<TSuccess> successValue => await bindFunc(successValue).ConfigureAwait(false),
                FailureValue<TFailure> failureValue => Result.Failure<TBind, TFailure>(failureValue),
                _ => throw new PatternErrorBuilder(nameof(result))
                    .IsNotOneOf(nameof(FailureValue<TFailure>), nameof(SuccessValue<TSuccess>)),
            };

        /// <summary>
        /// Bi-bind. Allows mapping of both monad states.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TBindSuccess">A new success type.</typeparam>
        /// <typeparam name="TBindFailure">A new failure type.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <param name="bindSuccess">The bind function if success.</param>
        /// <param name="bindFailure">The bind function if failure.</param>
        /// <returns>
        /// A new <see cref="Result{TBindSuccess,TBindFailure}" />.
        /// </returns>
        public static Task<IResult<TBindSuccess, TBindFailure>> BiBindAsync<TSuccess, TFailure, TBindSuccess, TBindFailure>(
            this Task<IResult<TSuccess, TFailure>> resultTask,
            Func<TSuccess, IResult<TBindSuccess, TBindFailure>> bindSuccess,
            Func<TFailure, IResult<TBindSuccess, TBindFailure>> bindFailure)
            where TBindFailure : Failure
            where TFailure : Failure =>
            resultTask.Map(result => result.BiBind(bindSuccess, bindFailure));

        /// <summary>
        /// Bi-bind. Allows mapping of both monad states.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TBindSuccess">A new success type.</typeparam>
        /// <typeparam name="TBindFailure">A new failure type.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="bindSuccess">The bind function if success.</param>
        /// <param name="bindFailure">The bind function if failure.</param>
        /// <returns>
        /// A new <see cref="Result{TBindSuccess,TBindFailure}" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// result
        /// or
        /// bindSuccess
        /// or
        /// bindFailure.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Major Code Smell",
            "S2436:Types and methods should not have too many generic parameters",
            Justification = "Extention Methods")]
        public static Task<IResult<TBindSuccess, TBindFailure>> BiBindAsync<TSuccess, TFailure, TBindSuccess,
            TBindFailure>(
            this IResult<TSuccess, TFailure> result,
            Func<TSuccess, Task<IResult<TBindSuccess, TBindFailure>>> bindSuccess,
            Func<TFailure, Task<IResult<TBindSuccess, TBindFailure>>> bindFailure)
            where TBindFailure : Failure
            where TFailure : Failure
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (bindSuccess == null)
            {
                throw new ArgumentNullException(nameof(bindSuccess));
            }

            if (bindFailure == null)
            {
                throw new ArgumentNullException(nameof(bindFailure));
            }

            return BiBindAsyncInternal(result, bindSuccess, bindFailure);
        }

        /// <summary>
        /// Bi-bind. Allows mapping of both monad states.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TBindSuccess">A new success type.</typeparam>
        /// <typeparam name="TBindFailure">A new failure type.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <param name="bindSuccess">The bind function if success.</param>
        /// <param name="bindFailure">The bind function if failure.</param>
        /// <returns>
        /// A new <see cref="Result{TBindSuccess,TBindFailure}" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">result
        /// or
        /// bindSuccess
        /// or
        /// bindFailure.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Major Code Smell",
            "S2436:Types and methods should not have too many generic parameters",
            Justification = "Extention Methods")]
        public static Task<IResult<TBindSuccess, TBindFailure>> BiBindAsync<TSuccess, TFailure, TBindSuccess,
            TBindFailure>(
            this Task<IResult<TSuccess, TFailure>> resultTask,
            Func<TSuccess, Task<IResult<TBindSuccess, TBindFailure>>> bindSuccess,
            Func<TFailure, Task<IResult<TBindSuccess, TBindFailure>>> bindFailure)
            where TBindFailure : Failure
            where TFailure : Failure =>
            resultTask.Bind(result => result.BiBindAsync(bindSuccess, bindFailure));

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Major Code Smell",
            "S2436:Types and methods should not have too many generic parameters",
            Justification = "Extention Method")]
        private static async Task<IResult<TBindSuccess, TBindFailure>> BiBindAsyncInternal<TSuccess, TFailure,
            TBindSuccess,
            TBindFailure>(
            IResult<TSuccess, TFailure> result,
            Func<TSuccess, Task<IResult<TBindSuccess, TBindFailure>>> bindSuccess,
            Func<TFailure, Task<IResult<TBindSuccess, TBindFailure>>> bindFailure)
            where TBindFailure : Failure
            where TFailure : Failure =>
            result.AsResultValue() switch
            {
                FailureValue<TFailure> f => await bindFailure(f).ConfigureAwait(false),
                SuccessValue<TSuccess> s => await bindSuccess(s).ConfigureAwait(false),
                _ => throw new PatternErrorBuilder(nameof(result))
                    .IsNotOneOf(nameof(FailureValue<TFailure>), nameof(SuccessValue<TSuccess>)),
            };

        /// <summary>
        /// Binds the failure only.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TBind">A new failure type.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <param name="bindFunc">Bind function.</param>
        /// <returns>
        /// A new <see cref="Result{TSuccess,TBind}" />.
        /// </returns>
        public static Task<IResult<TSuccess, TBind>> BindFailureAsync<TSuccess, TFailure, TBind>(
            this Task<IResult<TSuccess, TFailure>> resultTask,
            Func<TFailure, IResult<TSuccess, TBind>> bindFunc)
            where TBind : Failure
            where TFailure : Failure =>
            resultTask.BiBindAsync(
                Result.Success<TSuccess, TBind>,
                bindFunc);

        /// <summary>
        /// Binds the failure only.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TBind">A new failure type.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="bindFunc">Bind function.</param>
        /// <returns>
        /// A new <see cref="Result{TSuccess,TBind}" />.
        /// </returns>
        public static Task<IResult<TSuccess, TBind>> BindFailureAsync<TSuccess, TFailure, TBind>(
            this IResult<TSuccess, TFailure> result,
            Func<TFailure, Task<IResult<TSuccess, TBind>>> bindFunc)
            where TBind : Failure
            where TFailure : Failure =>
            result.BiBindAsync(
                s => Task.FromResult(Result.Success<TSuccess, TBind>(s)),
                bindFunc);

        /// <summary>
        /// Binds the failure only.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TBind">A new failure type.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <param name="bindFunc">Bind function.</param>
        /// <returns>
        /// A new <see cref="Result{TSuccess,TBind}" />.
        /// </returns>
        public static Task<IResult<TSuccess, TBind>> BindFailureAsync<TSuccess, TFailure, TBind>(
            this Task<IResult<TSuccess, TFailure>> resultTask,
            Func<TFailure, Task<IResult<TSuccess, TBind>>> bindFunc)
            where TBind : Failure
            where TFailure : Failure =>
            resultTask.Bind(result => result.BindFailureAsync(bindFunc));

        /// <summary>
        /// Implementation for the linq select many. Can be used for linq expressions.
        /// <example>
        /// <code>
        /// var one = Success&lt;int, Failure&gt;(1);
        /// var two = Success&lt;int, Failure&gt;(2);
        /// var three = Failure&lt;int, Failure&gt;(new Failure("Error"));
        ///
        /// var result = Match(
        ///     from x in one
        ///     from y in two
        ///     from z in three
        ///     select x + y + z,
        ///     success: i =&gt; i * 2,
        ///     failure: 0);
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="TSuccess">The initial success value.</typeparam>
        /// <typeparam name="TFailure">The failure value.</typeparam>
        /// <typeparam name="TIntermediate">The intermediate type.</typeparam>
        /// <typeparam name="TBind">The result value type.</typeparam>
        /// <param name="self">A task of a result monad.</param>
        /// <param name="mapper">A transform function to apply to the value.</param>
        /// <param name="getResult">A transform function to apply to the intermediate.</param>
        /// <returns>A new result.</returns>
        public static Task<IResult<TBind, TFailure>> SelectMany<TSuccess, TFailure, TIntermediate, TBind>(
            this Task<IResult<TSuccess, TFailure>> self,
            Func<TSuccess, Task<IResult<TIntermediate, TFailure>>> mapper,
            Func<TSuccess, TIntermediate, TBind> getResult)
            where TFailure : Failure =>
            self.BindAsync(value =>
                mapper(value).MapSuccessAsync(
                    intermediate => getResult(value, intermediate)));

        /// <summary>
        /// Implementation for the linq select many. Can be used for linq expressions.
        /// <example>
        /// <code>
        /// var one = Success&lt;int, Failure&gt;(1);
        /// var two = Success&lt;int, Failure&gt;(2);
        /// var three = Failure&lt;int, Failure&gt;(new Failure("Error"));
        ///
        /// var result = Match(
        ///     from x in one
        ///     from y in two
        ///     from z in three
        ///     select x + y + z,
        ///     success: i =&gt; i * 2,
        ///     failure: 0);
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="TSuccess">The initial success value.</typeparam>
        /// <typeparam name="TFailure">The failure value.</typeparam>
        /// <typeparam name="TIntermediate">The intermediate type.</typeparam>
        /// <typeparam name="TBind">The result value type.</typeparam>
        /// <param name="self">The result monad.</param>
        /// <param name="mapper">A transform function to apply to the value.</param>
        /// <param name="getResult">A transform function to apply to the intermediate.</param>
        /// <returns>A new result.</returns>
        public static Task<IResult<TBind, TFailure>> SelectMany<TSuccess, TFailure, TIntermediate, TBind>(
            this IResult<TSuccess, TFailure> self,
            Func<TSuccess, Task<IResult<TIntermediate, TFailure>>> mapper,
            Func<TSuccess, TIntermediate, TBind> getResult)
            where TFailure : Failure =>
            self.BindAsync(value =>
                mapper(value).MapSuccessAsync(
                    intermediate => getResult(value, intermediate)));

        /// <summary>
        /// Implementation for the linq select many. Can be used for linq expressions.
        /// <example>
        /// <code>
        /// var one = Success&lt;int, Failure&gt;(1);
        /// var two = Success&lt;int, Failure&gt;(2);
        /// var three = Failure&lt;int, Failure&gt;(new Failure("Error"));
        ///
        /// var result = Match(
        ///     from x in one
        ///     from y in two
        ///     from z in three
        ///     select x + y + z,
        ///     success: i =&gt; i * 2,
        ///     failure: 0);
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="TSuccess">The initial success value.</typeparam>
        /// <typeparam name="TFailure">The failure value.</typeparam>
        /// <typeparam name="TIntermediate">The intermediate type.</typeparam>
        /// <typeparam name="TBind">The result value type.</typeparam>
        /// <param name="self">The result monad.</param>
        /// <param name="mapper">A transform function to apply to the value.</param>
        /// <param name="getResult">A transform function to apply to the intermediate.</param>
        /// <returns>A new result.</returns>
        public static Task<IResult<TBind, TFailure>> SelectMany<TSuccess, TFailure, TIntermediate, TBind>(
            this Task<IResult<TSuccess, TFailure>> self,
            Func<TSuccess, IResult<TIntermediate, TFailure>> mapper,
            Func<TSuccess, TIntermediate, TBind> getResult)
            where TFailure : Failure =>
            self.BindAsync(value =>
                mapper(value).MapSuccess(
                    intermediate => getResult(value, intermediate)));

        #endregion

        #region If

        /// <summary>
        /// Convert the result to <see cref="SuccessValue{TSuccess}" /> or <see cref="FailureValue{TFailure}" />.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <returns>
        /// If successful <see cref="SuccessValue{TSuccess}" /> otherwise <see cref="FailureValue{TFailure}" />.
        /// </returns>
        public static Task<IResultValue> AsResultValueAsync<TSuccess, TFailure>(
            this Task<IResult<TSuccess, TFailure>> resultTask)
            where TFailure : Failure =>
            resultTask.Map(result => result.AsResultValue());

        /// <summary>
        /// Executes an action when <see cref="IsSuccessAsync{TSuccess,TFailure}" />.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <param name="onSuccess">Action to execute.</param>
        /// <returns>A task.</returns>
        public static async Task IfSuccessAsync<TSuccess, TFailure>(
            this Task<IResult<TSuccess, TFailure>> resultTask,
            Action<TSuccess> onSuccess)
            where TFailure : Failure
        {
            var result = await resultTask.ConfigureAwait(false);
            result.IfSuccess(onSuccess);
        }

        /// <summary>
        /// Executes an action when <see cref="IsFailureAsync{TSuccess,TFailure}" />.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <param name="onFailure">Action to execute.</param>
        /// <returns>A task.</returns>
        public static async Task IfFailureAsync<TSuccess, TFailure>(
            this Task<IResult<TSuccess, TFailure>> resultTask,
            Action<TFailure> onFailure)
            where TFailure : Failure
        {
            var result = await resultTask.ConfigureAwait(false);
            result.IfFailure(onFailure);
        }

        /// <summary>
        /// Executes onSuccess when successful else onFailure.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <param name="onSuccess">The on success action.</param>
        /// <param name="onFailure">The on failure action.</param>
        /// <returns>a Task.</returns>
        public static async Task DoAsync<TSuccess, TFailure>(
            this Task<IResult<TSuccess, TFailure>> resultTask,
            Action<TSuccess> onSuccess,
            Action<TFailure> onFailure)
            where TFailure : Failure
        {
            var result = await resultTask.ConfigureAwait(false);
            result.Do(onSuccess, onFailure);
        }

        /// <summary>
        /// Executes onSuccess when successful else onFailure.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="result">The result task.</param>
        /// <param name="onSuccess">The on success action.</param>
        /// <param name="onFailure">The on failure action.</param>
        /// <returns>a task.</returns>
        /// <exception cref="ArgumentOutOfRangeException">resultTask.AsResultValue.</exception>
        public static Task DoAsync<TSuccess, TFailure>(
            this IResult<TSuccess, TFailure> result,
            Func<TSuccess, Task> onSuccess,
            Func<TFailure, Task> onFailure)
            where TFailure : Failure
        {
            if (onSuccess == null)
            {
                throw new ArgumentNullException(nameof(onSuccess));
            }

            if (onFailure == null)
            {
                throw new ArgumentNullException(nameof(onFailure));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return DoAsyncInternal(result, onSuccess, onFailure);
        }

        /// <summary>
        /// Executes onSuccess when successful else onFailure.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <param name="onSuccess">The on success action.</param>
        /// <param name="onFailure">The on failure action.</param>
        /// <returns>a task.</returns>
        /// <exception cref="ArgumentOutOfRangeException">resultTask.AsResultValue.</exception>
        public static async Task DoAsync<TSuccess, TFailure>(
                this Task<IResult<TSuccess, TFailure>> resultTask,
                Func<TSuccess, Task> onSuccess,
                Func<TFailure, Task> onFailure)
                where TFailure : Failure
            {
                var result = await resultTask.ConfigureAwait(false);
                await result.DoAsync(onSuccess, onFailure).ConfigureAwait(false);
            }

        private static async Task DoAsyncInternal<TSuccess, TFailure>(
            IResult<TSuccess, TFailure> result,
            Func<TSuccess, Task> onSuccess,
            Func<TFailure, Task> onFailure)
            where TFailure : Failure
        {
            switch (result.AsResultValue())
            {
                case FailureValue<TFailure> f:
                    await onFailure(f).ConfigureAwait(false);
                    break;
                case SuccessValue<TSuccess> s:
                    await onSuccess(s).ConfigureAwait(false);
                    break;
                default:
                    throw new PatternErrorBuilder(nameof(result.AsResultValue))
                        .IsNotOneOf(nameof(FailureValue<TFailure>), nameof(SuccessValue<TSuccess>));
            }
        }

        #endregion

        #region Match

        /// <summary>
        /// Pattern matching on success and failure.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TRet">the type of the returned value.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <param name="onSuccess">When success.</param>
        /// <param name="onFailure">When failure.</param>
        /// <returns>
        /// On success the return value of onSuccess else the return value of onFailure.
        /// </returns>
        public static Task<TRet> MatchAsync<TSuccess, TFailure, TRet>(
            this Task<IResult<TSuccess, TFailure>> resultTask,
            Func<TSuccess, TRet> onSuccess,
            Func<TFailure, TRet> onFailure)
            where TFailure : Failure =>
            resultTask.Map(result => result.Match(onSuccess, onFailure));

        /// <summary>
        /// Pattern matching on success and failure.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TRet">the type of the returned value.</typeparam>
        /// <param name="result">The result task.</param>
        /// <param name="onSuccess">When success.</param>
        /// <param name="onFailure">When failure.</param>
        /// <returns>
        /// On success the return value of onSuccess else the return value of onFailure.
        /// </returns>
        public static Task<TRet> MatchAsync<TSuccess, TFailure, TRet>(
            this IResult<TSuccess, TFailure> result,
            Func<TSuccess, Task<TRet>> onSuccess,
            Func<TFailure, Task<TRet>> onFailure)
                where TFailure : Failure
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (onSuccess == null)
            {
                throw new ArgumentNullException(nameof(onSuccess));
            }

            if (onFailure == null)
            {
                throw new ArgumentNullException(nameof(onFailure));
            }

            return MatchAsyncInternal(result, onSuccess, onFailure);
        }

        /// <summary>
        /// Pattern matching on success and failure.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <typeparam name="TRet">the type of the returned value.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <param name="onSuccess">When success.</param>
        /// <param name="onFailure">When failure.</param>
        /// <returns>
        /// On success the return value of onSuccess else the return value of onFailure.
        /// </returns>
        public static Task<TRet> MatchAsync<TSuccess, TFailure, TRet>(
            this Task<IResult<TSuccess, TFailure>> resultTask,
            Func<TSuccess, Task<TRet>> onSuccess,
            Func<TFailure, Task<TRet>> onFailure)
            where TFailure : Failure =>
                resultTask.Bind(result => result.MatchAsync(onSuccess, onFailure));

        /// <summary>
        /// Pattern matching return success or convert failure to success.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="resultTask">The result.</param>
        /// <param name="onFailure">When failure.</param>
        /// <returns>
        /// On success returns success value else execute onFailure.
        /// </returns>
        public static Task<TSuccess> MatchAsync<TSuccess, TFailure>(
            this Task<IResult<TSuccess, TFailure>> resultTask,
            Func<TFailure, TSuccess> onFailure)
            where TFailure : Failure =>
                resultTask.MatchAsync(FunctionalCommon.Identity, onFailure);

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
        public static Task<TSuccess> MatchAsync<TSuccess, TFailure>(
            this IResult<TSuccess, TFailure> result,
            Func<TFailure, Task<TSuccess>> onFailure)
            where TFailure : Failure =>
                result.MatchAsync(Task.FromResult, onFailure);

        /// <summary>
        /// Pattern matching return success or convert failure to success.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="resultTask">The result.</param>
        /// <param name="onFailure">When failure.</param>
        /// <returns>
        /// On success returns success value else execute onFailure.
        /// </returns>
        public static Task<TSuccess> MatchAsync<TSuccess, TFailure>(
            this Task<IResult<TSuccess, TFailure>> resultTask,
            Func<TFailure, Task<TSuccess>> onFailure)
            where TFailure : Failure =>
                resultTask.Bind(result => result.MatchAsync(onFailure));

        /// <summary>
        /// Pattern matching return failure or convert success to failure.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <param name="onSuccess">When success.</param>
        /// <returns>
        /// On failure returns failure value else execute onSuccess.
        /// </returns>
        public static Task<TFailure> MatchAsync<TSuccess, TFailure>(
            this Task<IResult<TSuccess, TFailure>> resultTask,
            Func<TSuccess, TFailure> onSuccess)
            where TFailure : Failure =>
            resultTask.MatchAsync(onSuccess, FunctionalCommon.Identity);

        /// <summary>
        /// Pattern matching return failure or convert success to failure.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="result">The result task.</param>
        /// <param name="onSuccess">When success.</param>
        /// <returns>
        /// On failure returns failure value else execute onSuccess.
        /// </returns>
        public static Task<TFailure> MatchAsync<TSuccess, TFailure>(
            this IResult<TSuccess, TFailure> result,
            Func<TSuccess, Task<TFailure>> onSuccess)
            where TFailure : Failure =>
                result.MatchAsync(onSuccess, Task.FromResult);

        /// <summary>
        /// Pattern matching return failure or convert success to failure.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="resultTask">The result task.</param>
        /// <param name="onSuccess">When success.</param>
        /// <returns>
        /// On failure returns failure value else execute onSuccess.
        /// </returns>
        public static Task<TFailure> MatchAsync<TSuccess, TFailure>(
            this Task<IResult<TSuccess, TFailure>> resultTask,
            Func<TSuccess, Task<TFailure>> onSuccess)
            where TFailure : Failure =>
                resultTask.Bind(result => result.MatchAsync(onSuccess));

        private static async Task<TRet> MatchAsyncInternal<TSuccess, TFailure, TRet>(
            this IResult<TSuccess, TFailure> result,
            Func<TSuccess, Task<TRet>> onSuccess,
            Func<TFailure, Task<TRet>> onFailure)
            where TFailure : Failure =>
            result.AsResultValue() switch
            {
                FailureValue<TFailure> f => await onFailure(f).ConfigureAwait(false),
                SuccessValue<TSuccess> s => await onSuccess(s).ConfigureAwait(false),
                _ => throw new PatternErrorBuilder(nameof(result.AsResultValue))
                    .IsNotOneOf(nameof(FailureValue<TFailure>), nameof(SuccessValue<TSuccess>)),
            };

        #endregion
    }
}
