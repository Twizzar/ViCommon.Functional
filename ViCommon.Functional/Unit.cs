using System;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1129 // Do not use default value type constructor
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1801 // Review unused parameters

namespace ViCommon.Functional
{
    /// <summary>
    /// Return value which represents nothing.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public struct Unit : IEquatable<Unit>, IComparable<Unit>
    {
        /// <summary>
        /// Returns a new unit.
        /// </summary>
        public static readonly Unit New =
            new Unit();

        public static implicit operator ValueTuple(Unit _) =>
            default;

        public static implicit operator Unit(ValueTuple _) =>
            default;

        public static bool operator ==(Unit lhs, Unit rhs) =>
            true;

        public static bool operator !=(Unit lhs, Unit rhs) =>
            false;

        public static bool operator >(Unit lhs, Unit rhs) =>
            false;

        public static bool operator >=(Unit lhs, Unit rhs) =>
            true;

        public static bool operator <(Unit lhs, Unit rhs) =>
            false;

        public static bool operator <=(Unit lhs, Unit rhs) =>
            true;

        public static Unit operator +(Unit a, Unit b) =>
            New;

        /// <inheritdoc/>
        public override int GetHashCode() =>
            0;

        /// <inheritdoc/>
        public override string ToString() =>
            "()";

        /// <inheritdoc/>
        public override bool Equals(object obj) =>
            obj is Unit;

        /// <summary>
        /// Check if two units are equal.
        /// </summary>
        /// <param name="other">The other unit value.</param>
        /// <returns>Returns always true.</returns>
        public bool Equals(Unit other) =>
            true;

        /// <summary>
        /// Always equal.
        /// </summary>
        /// <param name="other">The other unit value.</param>
        /// <returns>Always zero.</returns>
        public int CompareTo(Unit other) =>
            0;
    }
}
