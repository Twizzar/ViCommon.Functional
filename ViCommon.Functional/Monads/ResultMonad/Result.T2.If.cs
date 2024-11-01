using System;

namespace ViCommon.Functional.Monads.ResultMonad
{
    /// <content>
    /// Contains If methods for Result.
    /// </content>
    public readonly partial struct Result<TSuccess, TFailure>
    {
        /// <summary>
        /// Convert the result to <see cref="SuccessValue{TSuccess}"/> or <see cref="FailureValue{TFailure}"/>.
        /// </summary>
        /// <returns>If successful <see cref="SuccessValue{TSuccess}"/> otherwise <see cref="FailureValue{TFailure}"/>.</returns>
        public IResultValue AsResultValue() =>
            this.Match<IResultValue>(
                success => new SuccessValue<TSuccess>(success),
                failure => new FailureValue<TFailure>(failure));

        /// <summary>
        /// Executes an action when <see cref="IsSuccess"/>.
        /// </summary>
        /// <param name="onSuccess">Action to execute.</param>
        public void IfSuccess(Action<TSuccess> onSuccess)
        {
            if (onSuccess == null)
            {
                throw new ArgumentNullException(nameof(onSuccess));
            }

            if (this.IsSuccess)
            {
                onSuccess(this.GetSuccessUnsafe());
            }
        }

        /// <summary>
        /// Executes an action when <see cref="IsFailure"/>.
        /// </summary>
        /// <param name="onFailure">Action to execute.</param>
        public void IfFailure(Action<TFailure> onFailure)
        {
            if (onFailure == null)
            {
                throw new ArgumentNullException(nameof(onFailure));
            }

            if (this.IsFailure)
            {
                onFailure(this.GetFailureUnsafe());
            }
        }

        /// <inheritdoc />
        public void Do(Action<TSuccess> onSuccess, Action<TFailure> onFailure)
        {
            this.IfSuccess(onSuccess);
            this.IfFailure(onFailure);
        }
    }
}
