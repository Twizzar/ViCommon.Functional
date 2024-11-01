using System;

namespace ViCommon.Functional.Monads.MaybeMonad
{
    /// <content>
    /// Contains Map methods for Maybe.
    /// </content>
    public readonly partial struct Maybe<TValue>
    {
        /// <summary>
        /// Replaces one contained object with another contained object.
        /// It can be viewed as a combination of <see cref="Map{TMap}"/> and
        /// <see cref="MaybeExtensions.Flatten{TValue}"/>. Useful for chaining many Maybe operations.
        /// <example>For example:
        /// <code>
        ///     Maybe&lt;int&gt; GetNumber() =&gt;
        ///         Some(5);
        ///
        ///     Maybe&lt;int&gt; Square(int x) =&gt;
        ///         Some(x * x);
        ///
        ///     Maybe&lt;string&gt; ToString(int x) =&gt;
        ///         Some(x.ToString());
        ///
        ///     var result = GetNumber()
        ///         .Bind(i =&gt; Square(i))
        ///         .Bind(i =&gt; ToString(i));
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="TBind">The value type of the new maybe.</typeparam>
        /// <param name="bindFunction">A function which takes the value as input and returns a new maybe.</param>
        /// <returns>A new maybe.</returns>
        public Maybe<TBind> Bind<TBind>(Func<TValue, Maybe<TBind>> bindFunction)
        {
            if (bindFunction == null)
            {
                throw new ArgumentNullException(nameof(bindFunction));
            }

            return this.IsSome
                ? bindFunction(this._value)
                : Maybe<TBind>.None();
        }

        /// <summary>
        /// Keep the contained object when some else replace it with another maybe.
        /// </summary>
        /// <param name="bindFunction">The bind function to invoke on none.</param>
        /// <returns>This on some else the output of the bindFunction.</returns>
        public Maybe<TValue> BindNone(Func<Maybe<TValue>> bindFunction)
        {
            if (bindFunction == null)
            {
                throw new ArgumentNullException(nameof(bindFunction));
            }

            return this.IsNone
                ? bindFunction()
                : this;
        }

        /// <summary>
        /// Keep the contained object when some else replace it with another maybe.
        /// </summary>
        /// <param name="noneMaybe">The maybe to return on none.</param>
        /// <returns>This on some else the noneMaybe.</returns>
        public Maybe<TValue> BindNone(Maybe<TValue> noneMaybe) =>
            this.BindNone(() => noneMaybe);

        /// <summary>
        /// Implementation for the linq select many. Can be used for linq expressions.
        /// <example>
        /// <code>
        ///     var one = Some(1);
        ///     var two = Some(2);
        ///     var three = Some(3);
        ///
        ///     var result = Match(
        ///         from x in one
        ///         from y in two
        ///         from z in three
        ///         select x + y + z,
        ///         some: i =&gt; i * 2,
        ///         none: 0);
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="TIntermediate">The intermediate type.</typeparam>
        /// <typeparam name="TResult">The result value type.</typeparam>
        /// <param name="mapper">A transform function to apply to the value.</param>
        /// <param name="getResult">A transform function to apply to the intermediate.</param>
        /// <returns>A new Maybe.</returns>
        public Maybe<TResult> SelectMany<TIntermediate, TResult>(
            Func<TValue, Maybe<TIntermediate>> mapper,
            Func<TValue, TIntermediate, TResult> getResult) =>
            this.Bind(value =>
                mapper(value).Bind(
                    intermediate =>
                        Maybe.Some(getResult(value, intermediate))));
    }
}
