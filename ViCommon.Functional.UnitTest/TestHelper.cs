using System;
using FluentAssertions;
using ViCommon.Functional.Monads.ResultMonad;

namespace ViCommon.Functional.UnitTest
{
    /// <summary>
    /// Helper class for unit tests.
    /// </summary>
    public static class TestHelper
    {
        private static readonly Random Rnd = new Random();

        /// <summary>
        /// Generate a random string.
        /// </summary>
        /// <returns>A random string.</returns>
        public static string RandomString(string prefix = "") =>
            prefix + Guid.NewGuid().ToString();

        /// <summary>
        /// Generate a random int.
        /// </summary>
        /// <param name="start">Inclusive start.</param>
        /// <param name="end">Exclusive end.</param>
        /// <returns>A random int between start and end.</returns>
        public static int RandomInt(int start = int.MinValue, int end = int.MaxValue)
            => Rnd.Next(start, end);

        /// <summary>
        /// Assert that the result should be successful and on success return the success value.
        /// </summary>
        /// <typeparam name="TSuccess">Success type.</typeparam>
        /// <typeparam name="TFailure">Failure type.</typeparam>
        /// <param name="result">The result monad.</param>
        /// <returns>The success value.</returns>
        public static TSuccess AssertAndUnwrapSuccess<TSuccess, TFailure>(
            IResult<TSuccess, TFailure> result)
            where TFailure : Failure
        {
            result.IsSuccess.Should().BeTrue(result.ToString());
            return result.GetSuccessUnsafe();
        }
    }
}
