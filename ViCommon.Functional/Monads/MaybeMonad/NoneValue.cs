#pragma warning disable SA1502 // Element should not be on a single line

namespace ViCommon.Functional.Monads.MaybeMonad
{
    /// <summary>
    /// Represents a None of a Maybe. Used for creating None value without the need of
    /// the type of the Maybe value. Maybe declares a implicit cast form MaybeNone to Maybe.
    /// </summary>
    public struct NoneValue : IMaybeValue { }
}
