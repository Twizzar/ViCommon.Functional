using System;
using System.Threading.Tasks;
using ViCommon.Functional.Extensions;

#pragma warning disable SA1129 // Do not use default value type constructor

namespace ViCommon.Functional.Monads.MaybeMonad
{
    /// <summary>
    /// Static Methods for the Maybe monad. Normally we would call this MaybeHelper,
    /// but for readability this is called Maybe.
    /// </summary>
    public static class Maybe
    {
        /// <summary>
        /// Create maybe with some value.
        /// </summary>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="someValue">The value.</param>
        /// <returns>A new maybe.</returns>
        public static Maybe<TValue> Some<TValue>(TValue someValue) =>
            Maybe<TValue>.Some(someValue);

        /// <summary>
        /// Create maybe with some value async.
        /// </summary>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="someValueTask">The value.</param>
        /// <returns>A new maybe.</returns>
        public static Task<Maybe<TValue>> SomeAsync<TValue>(Task<TValue> someValueTask)
        {
            if (someValueTask == null)
            {
                throw new ArgumentNullException(nameof(someValueTask));
            }

            return someValueTask.Map(Some);
        }

        /// <summary>
        /// Create a maybe which is none.
        /// </summary>
        /// <typeparam name="TValue">The inner type of the maybe.</typeparam>
        /// <returns>A new maybe.</returns>
        public static Maybe<TValue> None<TValue>() =>
            Maybe<TValue>.None();

        /// <summary>
        /// Create a MaybeNone which can be implicitly converted to a Maybe.
        /// Useful when you are in a Method with a Maybe return type.
        /// <example>
        /// <code>
        /// Maybe&lt;int&gt; Divide(int a, int b) =>
        ///     (b == 0) ? None() : Some(a / b);
        /// </code>
        /// </example>
        /// </summary>
        /// <returns>A new MaybeNone.</returns>
        public static NoneValue None() =>
            new NoneValue();

        /// <summary>
        /// Convert a nullable value to maybe.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="nullableValue">The nullable value.</param>
        /// <returns>Maybe Some when the value is not null else Maybe none.</returns>
        public static Maybe<TValue> ToMaybe<TValue>(TValue nullableValue) =>
            nullableValue is null
                ? Maybe<TValue>.None()
                : Maybe<TValue>.Some(nullableValue);

        /// <summary>
        /// Convert a nullable value to maybe.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="nullableValue">The nullable value.</param>
        /// <returns>Maybe Some when the value is not null else Maybe none.</returns>
        public static Maybe<TValue> ToMaybe<TValue>(TValue? nullableValue)
            where TValue : struct =>
            nullableValue.HasValue
                ? Maybe<TValue>.Some(nullableValue.Value)
                : Maybe<TValue>.None();

        /// <summary>
        /// Applies the mapFunction to the maybe value if some
        /// and returns a maybe of the output type of the mapFunction.
        /// </summary>
        /// <typeparam name="TValue">The value type of the maybe.</typeparam>
        /// <typeparam name="TMap">Output type of the mapFunction.</typeparam>
        /// <param name="maybe">The maybe.</param>
        /// <param name="mapFunc">Function to apply.</param>
        /// <returns>Some(mapFunction(value)) if some else None.</returns>
        public static Maybe<TMap> Map<TValue, TMap>(Maybe<TValue> maybe, Func<TValue, TMap> mapFunc) =>
            maybe.Map(mapFunc);

        /// <summary>
        /// Replaces one contained object with another contained object.
        /// It can be viewed as a combination of <see cref="Map{TValue,TMap}"/> and
        /// <see cref="MaybeExtensions.Flatten{TValue}"/>. Useful for chaining many Maybe operations.
        /// </summary>
        /// <typeparam name="TValue">The value type of the maybe.</typeparam>
        /// <typeparam name="TBind">The value type of the new maybe.</typeparam>
        /// <param name="maybe">The maybe.</param>
        /// <param name="bindFunc">A function which takes the value as input and returns a new maybe.</param>
        /// <returns>A new maybe.</returns>
        public static Maybe<TBind> Bind<TValue, TBind>(Maybe<TValue> maybe, Func<TValue, Maybe<TBind>> bindFunc) =>
            maybe.Bind(bindFunc);

        /// <summary>
        /// Pattern match the maybe.
        /// </summary>
        /// <typeparam name="TValue">The value type of the maybe.</typeparam>
        /// <typeparam name="TRet">The type of the returned value.</typeparam>
        /// <param name="maybe">The maybe.</param>
        /// <param name="some">Function to execute on some.</param>
        /// <param name="none">Function to execute on none.</param>
        /// <returns>Returns the output of some when is some else returns none.</returns>
        public static TRet Match<TValue, TRet>(
            Maybe<TValue> maybe,
            Func<TValue, TRet> some,
            Func<TRet> none) =>
            maybe.Match(some, none);

        /// <summary>
        /// Pattern match the maybe.
        /// </summary>
        /// <typeparam name="TValue">The value type of the maybe.</typeparam>
        /// <typeparam name="TRet">The type of the returned value.</typeparam>
        /// <param name="maybe">The maybe.</param>
        /// <param name="some">Function to execute on some.</param>
        /// <param name="none">Value to return on none.</param>
        /// <returns>Returns the output of some when is some else returns none.</returns>
        public static TRet Match<TValue, TRet>(
            Maybe<TValue> maybe,
            Func<TValue, TRet> some,
            TRet none) =>
            maybe.Match(some, none);
    }
}
