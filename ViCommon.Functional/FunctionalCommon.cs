using System.Diagnostics.Contracts;

namespace ViCommon.Functional
{
    /// <summary>
    /// Common functional helper methods.
    /// </summary>
    public static class FunctionalCommon
    {
        /// <summary>
        /// Identity function.
        /// </summary>
        /// <typeparam name="T">Type of the input and output.</typeparam>
        /// <param name="x">The value which will be returned.</param>
        /// <returns>Returns x.</returns>
        [Pure]
        public static T Identity<T>(T x) =>
            x;
    }
}
