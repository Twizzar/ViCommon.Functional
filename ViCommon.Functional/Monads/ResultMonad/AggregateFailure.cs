using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ViCommon.Functional.Monads.ResultMonad
{
    /// <summary>
    /// Represents one or more failures that occur during application execution.
    /// </summary>
    /// <seealso cref="Failure" />
    [ExcludeFromCodeCoverage] // only holds data.
    public class AggregateFailure : Failure
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateFailure"/> class.
        /// </summary>
        /// <param name="message">The failure message.</param>
        public AggregateFailure(string message)
            : base(message)
        {
            this.Failures = new List<Failure>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateFailure"/> class.
        /// </summary>
        /// <param name="failures">The failures.</param>
        public AggregateFailure(params Failure[] failures)
            : this((IEnumerable<Failure>)failures)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateFailure"/> class.
        /// </summary>
        /// <param name="failures">The failures.</param>
        public AggregateFailure(IEnumerable<Failure> failures)
            : base(ToString(failures))
        {
            this.Failures = failures.ToList();
        }

        /// <summary>
        /// Gets the failures.
        /// </summary>
        public IReadOnlyList<Failure> Failures { get; }

        /// <summary>
        /// Creates a new <see cref="AggregateFailure"/> with this failure and the other failure combined.
        /// </summary>
        /// <param name="other">The other failure.</param>
        /// <returns>A new aggregate failure.</returns>
        public AggregateFailure Combine(AggregateFailure other) =>
            new AggregateFailure(this.Failures.Concat(other?.Failures ?? throw new ArgumentNullException(nameof(other))));

        /// <summary>
        /// Adds the specified failure to the aggregate and returns a new <see cref="AggregateFailure"/>.
        /// </summary>
        /// <param name="failure">The failure to add cannot be a <see cref="AggregateFailure"/>.</param>
        /// <returns>A new <see cref="AggregateFailure"/>.</returns>
        public AggregateFailure Add(Failure failure) =>
            failure switch
            {
                null => throw new ArgumentNullException(nameof(failure)),
                AggregateFailure => throw new ArgumentException(
                    $"Use {nameof(this.Combine)} to add a aggregate failure to an aggregate failure."),
                _ => new AggregateFailure(this.Failures.Append(failure)),
            };

        private static string ToString(IEnumerable<Failure> failures) =>
            failures.Aggregate(string.Empty, (s, failure) => s + ", " + failure.Message);
    }
}
