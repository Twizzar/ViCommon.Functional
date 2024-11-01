using System;
using static ViCommon.Functional.FunctionalCommon;

namespace ViCommon.Functional.Monads.ResultMonad
{
    /// <content>
    /// Contains Map methods for Maybe.
    /// </content>
    public readonly partial struct Result<TSuccess, TFailure>
    {
        /// <summary>
        /// Applies the mapFuncSuccess to the result success value if is success and
        /// applies the mapFuncFailure to the result failure value if is failure.
        /// </summary>
        /// <param name="mapFuncSuccess">Function to apply on success.</param>
        /// <param name="mapFuncFailure">Function to apply on failure.</param>
        /// <typeparam name="TMapSuccess">New success value type.</typeparam>
        /// <typeparam name="TMapFailure">New failure value type.</typeparam>
        /// <returns>A new <see cref="Result{TSuccess,TFailure}"/>.</returns>
        public IResult<TMapSuccess, TMapFailure> Map<TMapSuccess, TMapFailure>(
            Func<TSuccess, TMapSuccess> mapFuncSuccess,
            Func<TFailure, TMapFailure> mapFuncFailure)
            where TMapFailure : Failure
        {
            if (mapFuncSuccess == null)
            {
                throw new ArgumentNullException(nameof(mapFuncSuccess));
            }

            if (mapFuncFailure == null)
            {
                throw new ArgumentNullException(nameof(mapFuncFailure));
            }

            return this.IsSuccess
                ? Result.Success<TMapSuccess, TMapFailure>(
                    mapFuncSuccess(this._success.GetValueUnsafe()))
                : Result.Failure<TMapSuccess, TMapFailure>(
                    mapFuncFailure(this._failure.GetValueUnsafe()));
        }

        /// <summary>
        /// Applies the mapFunction to the result success value if is success
        /// and returns a result of the output type of the mapFunction.
        /// </summary>
        /// <typeparam name="TMap">Output type of the mapFunction.</typeparam>
        /// <param name="mapFunc">Function to apply.</param>
        /// <returns>Success(mapFunction(SuccessValue)) if success else old failure value.</returns>
        public IResult<TMap, TFailure> MapSuccess<TMap>(Func<TSuccess, TMap> mapFunc) =>
            this.Map(mapFunc, Identity);

        /// <summary>
        /// Applies the mapFunction to the result failure value if is failure
        /// and returns a result of the output type of the mapFunction.
        /// </summary>
        /// <typeparam name="TMap">Output type of the mapFunction.</typeparam>
        /// <param name="mapFunc">Function to apply.</param>
        /// <returns>Failure(mapFunction(failureValue)) if failure else old success value.</returns>
        public IResult<TSuccess, TMap> MapFailure<TMap>(Func<TFailure, TMap> mapFunc)
            where TMap : Failure =>
            this.Map(Identity, mapFunc);

        /// <summary>
        /// Select implementation for using linq expressions. Same as <see cref="MapSuccess{TMap}"/>.
        /// </summary>
        /// <typeparam name="TResult">Output type of the mapFunction.</typeparam>
        /// <param name="mapFunction">Function to apply.</param>
        /// <returns>Success(mapFunction(SuccessValue)) if success else old failure value.</returns>
        public IResult<TResult, TFailure> Select<TResult>(Func<TSuccess, TResult> mapFunction) =>
            this.MapSuccess(mapFunction);
    }
}
