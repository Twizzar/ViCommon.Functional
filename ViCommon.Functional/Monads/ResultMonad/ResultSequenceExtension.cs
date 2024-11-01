using System;
using System.Collections.Generic;
using static ViCommon.Functional.Monads.ResultMonad.Result;

// ReSharper disable PossibleMultipleEnumeration
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented

namespace ViCommon.Functional.Monads.ResultMonad
{
    public static class ResultSequenceExtension
    {
        public static Result<IEnumerable<TValue>, TFailure> ExtractResult<TValue, TFailure>(
            this IEnumerable<Result<TValue, TFailure>> sequence)
            where TFailure : Failure =>
            sequence.ExtractResult(FunctionalCommon.Identity);

        public static Result<TSequence, TFailure> ExtractResult<TValue, TFailure, TSequence>(
            this IEnumerable<Result<TValue, TFailure>> sequence,
            Func<IEnumerable<TValue>, TSequence> convertToSequence)
            where TFailure : Failure
        {
            if (sequence == null)
            {
                throw new ArgumentNullException(nameof(sequence));
            }

            if (convertToSequence == null)
            {
                throw new ArgumentNullException(nameof(convertToSequence));
            }

            var list = new List<TValue>();
            foreach (var result in sequence)
            {
                switch (result.AsResultValue())
                {
                    case SuccessValue<TValue> v:
                        list.Add(v);
                        break;
                    case FailureValue<TFailure> f:
                        return Failure(f.Value);
                }
            }

            return Success(convertToSequence(list));
        }
    }
}
