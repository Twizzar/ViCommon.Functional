using System;

namespace ViCommon.Functional.Monads.ResultMonad
{
    /// <content>
    /// Contains Map methods for Maybe.
    /// </content>
    public readonly partial struct Result<TSuccess, TFailure>
    {
        /// <summary>
        /// Monadic bind.
        /// </summary>
        /// <typeparam name="TBind">A new success type.</typeparam>
        /// <param name="bindFunc">The bind function.</param>
        /// <returns>A new <see cref="Result{TBind,TFailure}"/>.</returns>
        public IResult<TBind, TFailure> Bind<TBind>(Func<TSuccess, IResult<TBind, TFailure>> bindFunc)
        {
            if (bindFunc == null)
            {
                throw new ArgumentNullException(nameof(bindFunc));
            }

            return this.AsResultValue() switch
            {
                SuccessValue<TSuccess> s => bindFunc(s),
                FailureValue<TFailure> f => Result.Failure<TBind, TFailure>(f),
                _ => throw new PatternErrorBuilder(nameof(this.AsResultValue))
                    .IsNotOneOf(nameof(SuccessValue<TSuccess>), nameof(FailureValue<TFailure>)),
            };
        }

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
            where TBindFailure : Failure
        {
            if (bindSuccess == null)
            {
                throw new ArgumentNullException(nameof(bindSuccess));
            }

            if (bindFailure == null)
            {
                throw new ArgumentNullException(nameof(bindFailure));
            }

            return this.IsSuccess
                ? bindSuccess(this._success.GetValueUnsafe())
                : bindFailure(this._failure.GetValueUnsafe());
        }

        /// <summary>
        /// Binds the failure only.
        /// </summary>
        /// <typeparam name="TBind">A new failure type.</typeparam>
        /// <param name="bindFunc">Bind function.</param>
        /// <returns>A new <see cref="Result{TSuccess,TBind}"/>.</returns>
        public IResult<TSuccess, TBind> BindFailure<TBind>(Func<TFailure, IResult<TSuccess, TBind>> bindFunc)
            where TBind : Failure
        {
            if (bindFunc == null)
            {
                throw new ArgumentNullException(nameof(bindFunc));
            }

            return this.IsSuccess
                ? Result.Success<TSuccess, TBind>(this._success.GetValueUnsafe())
                : bindFunc(this._failure.GetValueUnsafe());
        }

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
        /// <typeparam name="TIntermediate">The intermediate type.</typeparam>
        /// <typeparam name="TBind">The result value type.</typeparam>
        /// <param name="mapper">A transform function to apply to the value.</param>
        /// <param name="getResult">A transform function to apply to the intermediate.</param>
        /// <returns>A new result.</returns>
        public IResult<TBind, TFailure> SelectMany<TIntermediate, TBind>(
            Func<TSuccess, IResult<TIntermediate, TFailure>> mapper,
            Func<TSuccess, TIntermediate, TBind> getResult) =>
            this.Bind(value =>
                mapper(value).MapSuccess(
                        intermediate => getResult(value, intermediate)));
    }
}
