using System;
using static ViCommon.Functional.FunctionalCommon;

namespace ViCommon.Functional.Monads.MaybeMonad
{
    /// <content>
    /// Contains Match methods for Maybe.
    /// </content>
    public readonly partial struct Maybe<TValue>
    {
        /// <summary>
        /// Gets the value boxed to object if some else returns null.
        /// Can be used for pattern matching.
        /// </summary>
        [Obsolete("use " + nameof(AsMaybeValue) + " instead.")]
        public object Case =>
            this.IsSome ? (object)this._value : null;

        /// <summary>
        /// Pattern match the maybe.
        /// </summary>
        /// <param name="some">Function to execute on some.</param>
        /// <param name="none">Function to execute on none.</param>
        /// <typeparam name="TReturn">The type of the returned value.</typeparam>
        /// <returns>Returns the output of some whe <see cref="IsSome"/> else returns the output of none.</returns>
        public TReturn Match<TReturn>(Func<TValue, TReturn> some, Func<TReturn> none)
        {
            if (some == null)
            {
                throw new ArgumentNullException(nameof(some));
            }

            if (none == null)
            {
                throw new ArgumentNullException(nameof(none));
            }

            return this.IsSome
                ? some(this._value)
                : none();
        }

        /// <summary>
        /// Pattern match the maybe.
        /// </summary>
        /// <typeparam name="TReturn">The type of the returned value.</typeparam>
        /// <param name="some">Function to execute on some.</param>
        /// <param name="none">Value to return on none.</param>
        /// <returns>Returns the output of some when <see cref="IsSome"/> else returns none.</returns>
        public TReturn Match<TReturn>(Func<TValue, TReturn> some, TReturn none)
        {
            if (some == null)
            {
                throw new ArgumentNullException(nameof(some));
            }

            return this.Match(some, () => none);
        }

        /// <summary>
        /// Return value if some else invoke the none function.
        /// </summary>
        /// <param name="none">Invoked when none.</param>
        /// <returns>The value if some else the output of the none function.</returns>
        public TValue SomeOrProvided(Func<TValue> none) =>
            this.Match(Identity, none);

        /// <summary>
        /// Return value if some else the noneValue.
        /// </summary>
        /// <param name="noneValue">Returned when none.</param>
        /// <returns>The value if some else noneValue.</returns>
        public TValue SomeOrProvided(TValue noneValue) =>
            this.Match(Identity, noneValue);
    }
}
