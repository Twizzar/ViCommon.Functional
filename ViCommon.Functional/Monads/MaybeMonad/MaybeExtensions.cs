using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ViCommon.Functional.Monads.ResultMonad;
using static ViCommon.Functional.FunctionalCommon;

namespace ViCommon.Functional.Monads.MaybeMonad
{
    /// <summary>
    /// Extensions for the maybe monad.
    /// </summary>
    public static class MaybeExtensions
    {
        /// <summary>
        /// Monadic join. Flatten two nested Maybes to one.
        /// </summary>
        /// <typeparam name="TValue">The type value of the inner maybe.</typeparam>
        /// <param name="maybe">The nested maybe.</param>
        /// <returns>One Maybe instead of two nested ones.</returns>
        public static Maybe<TValue> Flatten<TValue>(this Maybe<Maybe<TValue>> maybe) =>
            maybe.Bind(Identity);

        /// <summary>
        /// Get some value of a dictionary by key if key exists else none.
        /// </summary>
        /// <typeparam name="TValue">Type of the dictionary values.</typeparam>
        /// <typeparam name="TKey">Type of the dictionary keys.</typeparam>
        /// <param name="self">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>Maybe a value.</returns>
        public static Maybe<TValue> GetMaybe<TValue, TKey>(this IReadOnlyDictionary<TKey, TValue> self, TKey key)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            return self.ContainsKey(key)
                ? Maybe<TValue>.Some(self[key])
                : Maybe<TValue>.None();
        }

        /// <summary>
        /// Convert to result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TFailure">The type of the failure.</typeparam>
        /// <param name="self">The maybe monad to convert.</param>
        /// <param name="onNone">Failure when the maybe is none.</param>
        /// <returns>A result which is success on some and failure on none.</returns>
        public static IResult<TValue, TFailure> ToResult<TValue, TFailure>(this Maybe<TValue> self, TFailure onNone)
            where TFailure : Failure =>
                self.Match(
                    some: Result.Success<TValue, TFailure>,
                    none: Result.Failure<TValue, TFailure>(onNone));

        /// <summary>
        /// Extracts from a list of `Option` all the `Some` elements.
        /// All the `Some` elements are extracted in order.
        /// </summary>
        /// <typeparam name="TValue">The inner type of the Maybe.</typeparam>
        /// <param name="self">The sequence of Maybes.</param>
        /// <returns>Returns a list of T.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TValue> Somes<TValue>(this IEnumerable<Maybe<TValue>> self)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            return
                from item in self
                where item.IsSome
                select item.GetValueUnsafe();
        }
    }
}
