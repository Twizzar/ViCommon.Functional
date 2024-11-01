using System;

namespace ViCommon.Functional.Monads.MaybeMonad
{
    /// <content>
    /// Contains If methods for Maybe.
    /// </content>
    public readonly partial struct Maybe<TValue>
    {
        /// <summary>
        /// Convert the maybe to <see cref="SomeValue{TValue}"/> or <see cref="NoneValue"/>.
        /// </summary>
        /// <returns>If some <see cref="SomeValue{TValue}"/> otherwise <see cref="NoneValue"/>.</returns>
        public IMaybeValue AsMaybeValue() =>
            this.Match<IMaybeValue>(
            value => new SomeValue<TValue>(value),
            () => default(NoneValue));

        /// <summary>
        /// Executes an action when <see cref="IsSome"/>.
        /// </summary>
        /// <param name="onSome">Action to execute.</param>
        public void IfSome(Action<TValue> onSome)
        {
            if (onSome == null)
            {
                throw new ArgumentNullException(nameof(onSome));
            }

            if (this.IsSome)
            {
                onSome(this._value);
            }
        }

        /// <summary>
        /// Executes an action when <see cref="IsNone"/>.
        /// </summary>
        /// <param name="onNone">Action to execute.</param>
        public void IfNone(Action onNone)
        {
            if (this.IsNone)
            {
                onNone();
            }
        }

        /// <summary>
        /// Executes onSome when some else onNone.
        /// </summary>
        /// <param name="onSome">The on some action.</param>
        /// <param name="onNone">The on none action.</param>
        public void Do(Action<TValue> onSome, Action onNone)
        {
            this.IfSome(onSome);
            this.IfNone(onNone);
        }
    }
}
