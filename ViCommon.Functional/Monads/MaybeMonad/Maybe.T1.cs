using System;

namespace ViCommon.Functional.Monads.MaybeMonad
{
    /// <summary>
    /// Monad which represent a value which can be not declared (none).
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public readonly partial struct Maybe<TValue> : IEquatable<Maybe<TValue>>
    {
        private readonly TValue _value;

        private Maybe(TValue someValue, bool isSome)
        {
            if (isSome && someValue is null)
            {
                throw new ArgumentNullException(nameof(someValue));
            }

            this._value = someValue;
            this.IsSome = isSome;
        }

        /// <summary>
        /// Gets a value indicating whether the value is declared.
        /// </summary>
        public bool IsSome { get; }

        /// <summary>
        /// Gets a value indicating whether the value is not declared.
        /// </summary>
        public bool IsNone => !this.IsSome;

        /// <summary>
        /// Convert value implicit to Maybe.
        /// </summary>
        /// <param name="value">The type of the value.</param>
        public static implicit operator Maybe<TValue>(TValue value) =>
            value switch
            {
                Maybe<TValue> maybe => maybe,
                null => throw new ArgumentNullException(nameof(value)),
                _ => Some(value),
            };

        /// <summary>
        /// Convert MaybeNone to a Maybe.
        /// </summary>
        /// <param name="noneValue">The MaybeNone.</param>
        public static implicit operator Maybe<TValue>(NoneValue noneValue) =>
            None();

        /// <summary>
        /// Check if the inner value of the maybe is equal with a value.
        /// </summary>
        /// <param name="maybe">The maybe.</param>
        /// <param name="value">The value.</param>
        /// <returns>True when maybe.value is equals value.</returns>
        public static bool operator ==(Maybe<TValue> maybe, TValue value)
        {
            if (value is Maybe<TValue>)
            {
                return maybe.Equals(value);
            }

            return maybe.Match(
                x => x.Equals(value),
                () => false);
        }

        /// <summary>
        /// Check if the inner value of the maybe is not equal with a value.
        /// </summary>
        /// <param name="maybe">The maybe.</param>
        /// <param name="value">The value.</param>
        /// <returns>False when maybe.value is equals value.</returns>
        public static bool operator !=(Maybe<TValue> maybe, TValue value) =>
            !(maybe == value);

        /// <summary>
        /// Check if the inner values of two maybes are equals.
        /// </summary>
        /// <param name="first">First maybe.</param>
        /// <param name="second">Second maybe.</param>
        /// <returns>True when they are equal.</returns>
        public static bool operator ==(Maybe<TValue> first, Maybe<TValue> second) =>
            first.Equals(second);

        /// <summary>
        /// Check if the inner values of two maybes are not equals.
        /// </summary>
        /// <param name="first">First maybe.</param>
        /// <param name="second">Second maybe.</param>
        /// <returns>False when they are equal.</returns>
        public static bool operator !=(Maybe<TValue> first, Maybe<TValue> second) =>
            !(first == second);

        /// <summary>
        /// Get the value when <see cref="IsSome"/> else throws an <see cref="NullReferenceException"/>.
        /// </summary>
        /// <returns>The some value.</returns>
        public TValue GetValueUnsafe() =>
            this.IsNone
                ? throw new NullReferenceException(nameof(this._value))
                : this._value ?? throw new NullReferenceException(nameof(this._value));

        /// <inheritdoc/>
        public bool Equals(Maybe<TValue> other)
        {
            if (this.IsNone && other.IsNone)
            {
                return true;
            }

            if (this.IsNone || other.IsNone)
            {
                return false;
            }

            return Maybe.Match(
                from x in this
                from y in other
                select x.Equals(y),
                some: b => b,
                none: false);
        }

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is Maybe<TValue> maybe && this.Equals(maybe);

        /// <inheritdoc/>
        public override int GetHashCode() =>
            this.IsNone ? 0 : this._value.GetHashCode();

        /// <inheritdoc/>
        public override string ToString() =>
            this.Match(
                value => $"Some({value})",
                "None()");

        /// <summary>
        /// Create new <see cref="Maybe{TValue}"/> with some value.
        /// </summary>
        /// <param name="someValue">The value.</param>
        /// <returns>A new maybe.</returns>
        internal static Maybe<TValue> Some(TValue someValue) =>
            new Maybe<TValue>(someValue, true);

        /// <summary>
        /// Create a <see cref="Maybe{TValue}"/> with none value.
        /// </summary>
        /// <returns>A new maybe.</returns>
        internal static Maybe<TValue> None() =>
            new Maybe<TValue>(default, false);
    }
}
