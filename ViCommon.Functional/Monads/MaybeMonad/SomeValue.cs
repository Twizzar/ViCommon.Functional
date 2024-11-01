using System.Diagnostics.CodeAnalysis;

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ViCommon.Functional.Monads.MaybeMonad
{
    /// <summary>
    /// Represents a maybe which is some.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <seealso cref="IMaybeValue" />
    [ExcludeFromCodeCoverage]
    public readonly struct SomeValue<TValue> : IMaybeValue, System.IEquatable<SomeValue<TValue>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SomeValue{TValue}"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public SomeValue(TValue value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public TValue Value { get; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="SomeValue{TValue}"/> to TValue.
        /// </summary>
        /// <param name="someValue">Some.</param>
        /// <returns>
        /// The result of the conversion. </returns>
        public static implicit operator TValue(SomeValue<TValue> someValue) =>
            someValue.Value;

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(SomeValue<TValue> left, SomeValue<TValue> right) => left.Equals(right);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(SomeValue<TValue> left, SomeValue<TValue> right) => !(left == right);

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => this.Value.Equals(obj);

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
        /// </returns>
        public bool Equals(SomeValue<TValue> other) => this == other;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode() => this.Value.GetHashCode();
    }
}
