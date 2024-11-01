#pragma warning disable CA1040 // Avoid empty interfaces

namespace ViCommon.Functional.Monads.ResultMonad
{
    /// <summary>
    /// Represents a already evaluated part of a result (success or failure). Can be used for pattern matching.
    /// </summary>
    public interface IResultValue
    {
    }
}