using System;

namespace ViCommon.Functional.Monads.MaybeMonad
{
    /// <content>
    /// Contains Map methods for Maybe.
    /// </content>
    public readonly partial struct Maybe<TValue>
    {
        /// <summary>
        /// Applies the mapFunction to the value if some
        /// and returns a maybe of the output type of the mapFunction.
        /// </summary>
        /// <typeparam name="TMap">Output type of the mapFunction.</typeparam>
        /// <param name="mapFunction">Function to apply.</param>
        /// <returns>Some(mapFunction(value)) if some else None.</returns>
        public Maybe<TMap> Map<TMap>(Func<TValue, TMap> mapFunction)
        {
            if (mapFunction == null)
            {
                throw new ArgumentNullException(nameof(mapFunction));
            }

            return this.IsSome
                ? Maybe<TMap>.Some(mapFunction(this._value))
                : Maybe<TMap>.None();
        }

        /// <summary>
        /// Same as <see cref="Map{TMap}"/>. Used for linq expressions.
        /// </summary>
        /// <typeparam name="TResult">Output type of the mapFunction.</typeparam>
        /// <param name="mapFunction">Function to apply.</param>
        /// <returns>Some(mapFunction(value)) if some else None.</returns>
        public Maybe<TResult> Select<TResult>(Func<TValue, TResult> mapFunction) =>
            this.Map(mapFunction);
    }
}
