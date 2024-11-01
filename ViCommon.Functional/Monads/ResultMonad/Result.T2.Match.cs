using System;

namespace ViCommon.Functional.Monads.ResultMonad
{
    /// <content>
    /// Contains Match methods for Maybe.
    /// </content>
    public readonly partial struct Result<TSuccess, TFailure>
    {
        /// <summary>
        /// Pattern matching on success and failure.
        /// </summary>
        /// <param name="onSuccess">When success.</param>
        /// <param name="onFailure">When failure.</param>
        /// <typeparam name="TRet">the type of the returned value.</typeparam>
        /// <returns>On success the return value of onSuccess else the return value of onFailure.</returns>
        public TRet Match<TRet>(Func<TSuccess, TRet> onSuccess, Func<TFailure, TRet> onFailure)
        {
            if (onSuccess == null)
            {
                throw new ArgumentNullException(nameof(onSuccess));
            }

            if (onFailure == null)
            {
                throw new ArgumentNullException(nameof(onFailure));
            }

            return this.IsSuccess
                ? onSuccess(this._success.GetValueUnsafe())
                : onFailure(this._failure.GetValueUnsafe());
        }

        /// <summary>
        /// Pattern matching return success or convert failure to success.
        /// </summary>
        /// <param name="onFailure">When failure.</param>
        /// <returns>On success returns success value else execute onFailure.</returns>
        public TSuccess Match(Func<TFailure, TSuccess> onFailure) =>
            this.Match(FunctionalCommon.Identity, onFailure);

        /// <summary>
        /// Pattern matching return failure or convert success to failure.
        /// </summary>
        /// <param name="onSuccess">When success.</param>
        /// <returns>On failure returns failure value else execute onSuccess.</returns>
        public TFailure Match(Func<TSuccess, TFailure> onSuccess) =>
            this.Match(onSuccess, FunctionalCommon.Identity);
    }
}
