using System;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable S4035 // Classes implementing "IEquatable<T>" should be sealed

namespace ViCommon.Functional.Monads.ResultMonad
{
    /// <summary>
    /// Base Failure class for <see cref="Result{TSuccess,TFailure}"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Failure
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Failure"/> class.
        /// </summary>
        /// <param name="message">The failure message.</param>
        public Failure(string message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets Failure message.
        /// </summary>
        public string Message { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((Failure)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() =>
            this.Message != null ? this.Message.GetHashCode() : 0;

        /// <inheritdoc />
        public override string ToString() =>
            $"Failure: {this.Message}";

        /// <summary>
        /// Check if two Failures are equal.
        /// </summary>
        /// <param name="other">The other Failure.</param>
        /// <returns>True if they are equal.</returns>
        protected bool Equals(Failure other) =>
            this.Message == (other?.Message ?? throw new ArgumentNullException(nameof(other)));
    }
}
