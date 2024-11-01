using System;
using System.Threading.Tasks;
using ViCommon.Functional.Extensions;
using ViCommon.Functional.Monads.ResultMonad;

namespace ViCommon.Functional.Monads.MaybeMonad
{
    /// <summary>
    /// Async methods for the <see cref="Maybe{TValue}"/> monad.
    /// </summary>
    public static class AsyncMaybeExtensions
    {
        /// <summary>
        /// Get the value when <see cref="Maybe{TValue}.IsSome" /> else throws an <see cref="NullReferenceException" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="maybeTask">The maybe task.</param>
        /// <returns>
        /// The some value.
        /// </returns>
        public static Task<TValue> GetValueUnsafeAsync<TValue>(this Task<Maybe<TValue>> maybeTask)
        {
            if (maybeTask is null)
            {
                throw new ArgumentNullException(nameof(maybeTask));
            }

            return maybeTask.Map(m => m.GetValueUnsafe());
        }

        /// <summary>
        /// Convert the maybe to a result.
        /// </summary>
        /// <typeparam name="TValue">The value type of the maybe.</typeparam>
        /// <typeparam name="TFailure">The failure type.</typeparam>
        /// <param name="maybeTask">The maybe task.</param>
        /// <param name="onNone">The failure to use on none.</param>
        /// <returns>A new <see cref="IResult{TSuccess,TFailure}"/>.</returns>
        public static Task<IResult<TValue, TFailure>> ToResultAsync<TValue, TFailure>(Task<Maybe<TValue>> maybeTask, TFailure onNone)
            where TFailure : Failure
        {
            if (maybeTask is null)
            {
                throw new ArgumentNullException(nameof(maybeTask));
            }

            if (onNone is null)
            {
                throw new ArgumentNullException(nameof(onNone));
            }

            return maybeTask.Map(maybe => maybe.ToResult(onNone));
        }

        #region Map

        /// <summary>
        /// Applies the mapFunction to the value if some
        /// and returns a maybe of the output type of the mapFunction.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TMap">Output type of the mapFunction.</typeparam>
        /// <param name="maybeTask">The maybe task.</param>
        /// <param name="mapFunction">Function to apply.</param>
        /// <returns>
        /// Some(mapFunction(value)) if some else None.
        /// </returns>
        public static Task<Maybe<TMap>> MapAsync<TValue, TMap>(this Task<Maybe<TValue>> maybeTask, Func<TValue, TMap> mapFunction)
        {
            if (maybeTask is null)
            {
                throw new ArgumentNullException(nameof(maybeTask));
            }

            if (mapFunction is null)
            {
                throw new ArgumentNullException(nameof(mapFunction));
            }

            return maybeTask.Map(m => m.Map(mapFunction));
        }

        /// <summary>
        /// Applies the mapFunction to the value if some
        /// and returns a maybe of the output type of the mapFunction.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TMap">Output type of the mapFunction.</typeparam>
        /// <param name="maybe">The maybe.</param>
        /// <param name="mapFunction">Function to apply.</param>
        /// <returns>
        /// Some(mapFunction(value)) if some else None.
        /// </returns>
        public static async Task<Maybe<TMap>> MapAsync<TValue, TMap>(this Maybe<TValue> maybe, Func<TValue, Task<TMap>> mapFunction)
        {
            if (maybe.IsNone)
            {
                return Maybe<TMap>.None();
            }
            else
            {
                return await mapFunction(maybe.GetValueUnsafe()).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Applies the mapFunction to the value if some
        /// and returns a maybe of the output type of the mapFunction.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TMap">Output type of the mapFunction.</typeparam>
        /// <param name="maybeTask">The maybe task.</param>
        /// <param name="mapFunction">Function to apply.</param>
        /// <returns>
        /// Some(mapFunction(value)) if some else None.
        /// </returns>
        public static Task<Maybe<TMap>> MapAsync<TValue, TMap>(this Task<Maybe<TValue>> maybeTask, Func<TValue, Task<TMap>> mapFunction)
        {
            if (maybeTask is null)
            {
                throw new ArgumentNullException(nameof(maybeTask));
            }

            if (mapFunction is null)
            {
                throw new ArgumentNullException(nameof(mapFunction));
            }

            return maybeTask.Bind(m => m.MapAsync(mapFunction));
        }

        #endregion

        #region Bind

        /// <summary>
        /// Replaces one contained object with another contained object.
        /// It can be viewed as a combination of <see cref="Maybe{TValue}.Map{TMap}" /> and
        /// <see cref="MaybeExtensions.Flatten{TValue}" />. Useful for chaining many Maybe operations.
        /// <example>For example:
        /// <code>
        /// Maybe&lt;int&gt; GetNumber() =&gt;
        /// Some(5);
        /// Maybe&lt;int&gt; Square(int x) =&gt;
        /// Some(x * x);
        /// Maybe&lt;string&gt; ToString(int x) =&gt;
        /// Some(x.ToString());
        /// var result = GetNumber()
        ///    .Bind(i =&gt; Square(i))
        ///    .Bind(i =&gt; ToString(i));
        /// </code></example>
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TBind">The value type of the new maybe.</typeparam>
        /// <param name="maybeTask">The maybe task.</param>
        /// <param name="bindFunction">A function which takes the value as input and returns a new maybe.</param>
        /// <returns>
        /// A new maybe.
        /// </returns>
        public static Task<Maybe<TBind>> BindAsync<TValue, TBind>(this Task<Maybe<TValue>> maybeTask, Func<TValue, Maybe<TBind>> bindFunction)
        {
            if (maybeTask is null)
            {
                throw new ArgumentNullException(nameof(maybeTask));
            }

            if (bindFunction is null)
            {
                throw new ArgumentNullException(nameof(bindFunction));
            }

            return maybeTask.Map(m => m.Bind(bindFunction));
        }

        /// <summary>
        /// Replaces one contained object with another contained object.
        /// It can be viewed as a combination of <see cref="Maybe{TValue}.Map{TMap}" /> and
        /// <see cref="MaybeExtensions.Flatten{TValue}" />. Useful for chaining many Maybe operations.
        /// <example>For example:
        /// <code>
        /// Maybe&lt;int&gt; GetNumber() =&gt;
        /// Some(5);
        /// Maybe&lt;int&gt; Square(int x) =&gt;
        /// Some(x * x);
        /// Maybe&lt;string&gt; ToString(int x) =&gt;
        /// Some(x.ToString());
        /// var result = GetNumber()
        ///    .Bind(i =&gt; Square(i))
        ///    .Bind(i =&gt; ToString(i));
        /// </code></example>
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TBind">The value type of the new maybe.</typeparam>
        /// <param name="maybe">The maybe task.</param>
        /// <param name="bindFunction">A function which takes the value as input and returns a new maybe.</param>
        /// <returns>
        /// A new maybe.
        /// </returns>
        public static async Task<Maybe<TBind>> BindAsync<TValue, TBind>(this Maybe<TValue> maybe, Func<TValue, Task<Maybe<TBind>>> bindFunction)
        {
            if (maybe.IsNone)
            {
                return Maybe<TBind>.None();
            }
            else
            {
                return await bindFunction(maybe.GetValueUnsafe()).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Replaces one contained object with another contained object.
        /// It can be viewed as a combination of <see cref="Maybe{TValue}.Map{TMap}" /> and
        /// <see cref="MaybeExtensions.Flatten{TValue}" />. Useful for chaining many Maybe operations.
        /// <example>For example:
        /// <code>
        /// Maybe&lt;int&gt; GetNumber() =&gt;
        /// Some(5);
        /// Maybe&lt;int&gt; Square(int x) =&gt;
        /// Some(x * x);
        /// Maybe&lt;string&gt; ToString(int x) =&gt;
        /// Some(x.ToString());
        /// var result = GetNumber()
        ///    .Bind(i =&gt; Square(i))
        ///    .Bind(i =&gt; ToString(i));
        /// </code></example>
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TBind">The value type of the new maybe.</typeparam>
        /// <param name="maybeTask">The maybe task.</param>
        /// <param name="bindFunction">A function which takes the value as input and returns a new maybe.</param>
        /// <returns>
        /// A new maybe.
        /// </returns>
        public static Task<Maybe<TBind>> BindAsync<TValue, TBind>(this Task<Maybe<TValue>> maybeTask, Func<TValue, Task<Maybe<TBind>>> bindFunction)
        {
            if (maybeTask is null)
            {
                throw new ArgumentNullException(nameof(maybeTask));
            }

            if (bindFunction is null)
            {
                throw new ArgumentNullException(nameof(bindFunction));
            }

            return maybeTask.Bind(m => m.BindAsync(bindFunction));
        }

        /// <summary>
        /// Keep the contained object when some; else replace it with another maybe.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="maybeTask">The maybe task.</param>
        /// <param name="bindFunction">The bind function to invoke on none.</param>
        /// <returns>This on some else the output of the bindFunction.</returns>
        public static Task<Maybe<TValue>> BindNoneAsync<TValue>(this Task<Maybe<TValue>> maybeTask, Func<Maybe<TValue>> bindFunction)
        {
            if (maybeTask is null)
            {
                throw new ArgumentNullException(nameof(maybeTask));
            }

            if (bindFunction is null)
            {
                throw new ArgumentNullException(nameof(bindFunction));
            }

            return maybeTask.Map(m => m.BindNone(bindFunction));
        }

        /// <summary>
        /// Keep the contained object when some; else replace it with another maybe.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="maybe">The maybe task.</param>
        /// <param name="bindFunction">The bind function to invoke on none.</param>
        /// <returns>This on some else the output of the bindFunction.</returns>
        public static async Task<Maybe<TValue>> BindNoneAsync<TValue>(this Maybe<TValue> maybe, Func<Task<Maybe<TValue>>> bindFunction)
        {
            if (maybe.IsSome)
            {
                return maybe;
            }
            else
            {
                return await bindFunction().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Keep the contained object when some; else replace it with another maybe.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="maybeTask">The maybe task.</param>
        /// <param name="bindFunction">The bind function to invoke on none.</param>
        /// <returns>This on some else the output of the bindFunction.</returns>
        public static Task<Maybe<TValue>> BindNoneAsync<TValue>(this Task<Maybe<TValue>> maybeTask, Func<Task<Maybe<TValue>>> bindFunction)
        {
            if (maybeTask is null)
            {
                throw new ArgumentNullException(nameof(maybeTask));
            }

            if (bindFunction is null)
            {
                throw new ArgumentNullException(nameof(bindFunction));
            }

            return maybeTask.Bind(m => m.BindNoneAsync(bindFunction));
        }

        /// <summary>
        /// Implementation for the linq select many. Can be used for linq expressions.
        /// <example>
        /// <code>
        ///     var one = Some(1);
        ///     var two = Some(2);
        ///     var three = Some(3);
        ///
        ///     var result = Match(
        ///         from x in one
        ///         from y in two
        ///         from z in three
        ///         select x + y + z,
        ///         some: i =&gt; i * 2,
        ///         none: 0);
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="TValue">The maybe value type.</typeparam>
        /// <typeparam name="TIntermediate">The intermediate type.</typeparam>
        /// <typeparam name="TResult">The result value type.</typeparam>
        /// <param name="self">The maybe task.</param>
        /// <param name="mapper">A transform function to apply to the value.</param>
        /// <param name="getResult">A transform function to apply to the intermediate.</param>
        /// <returns>A new Maybe.</returns>
        public static Task<Maybe<TResult>> SelectMany<TValue, TIntermediate, TResult>(
            this Task<Maybe<TValue>> self,
            Func<TValue, Maybe<TIntermediate>> mapper,
            Func<TValue, TIntermediate, TResult> getResult) =>
            self.BindAsync(value =>
                mapper(value).Bind(
                    intermediate =>
                        Maybe.Some(getResult(value, intermediate))));

        /// <summary>
        /// Implementation for the linq select many. Can be used for linq expressions.
        /// <example>
        /// <code>
        ///     var one = Some(1);
        ///     var two = Some(2);
        ///     var three = Some(3);
        ///
        ///     var result = Match(
        ///         from x in one
        ///         from y in two
        ///         from z in three
        ///         select x + y + z,
        ///         some: i =&gt; i * 2,
        ///         none: 0);
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="TValue">The maybe value type.</typeparam>
        /// <typeparam name="TIntermediate">The intermediate type.</typeparam>
        /// <typeparam name="TResult">The result value type.</typeparam>
        /// <param name="self">The maybe task.</param>
        /// <param name="mapper">A transform function to apply to the value.</param>
        /// <param name="getResult">A transform function to apply to the intermediate.</param>
        /// <returns>A new Maybe.</returns>
        public static Task<Maybe<TResult>> SelectMany<TValue, TIntermediate, TResult>(
            this Task<Maybe<TValue>> self,
            Func<TValue, Task<Maybe<TIntermediate>>> mapper,
            Func<TValue, TIntermediate, TResult> getResult) =>
            self.BindAsync(value =>
                mapper(value).BindAsync(
                    intermediate =>
                        Maybe.Some(getResult(value, intermediate))));

        /// <summary>
        /// Implementation for the linq select many. Can be used for linq expressions.
        /// <example>
        /// <code>
        ///     var one = Some(1);
        ///     var two = Some(2);
        ///     var three = Some(3);
        ///
        ///     var result = Match(
        ///         from x in one
        ///         from y in two
        ///         from z in three
        ///         select x + y + z,
        ///         some: i =&gt; i * 2,
        ///         none: 0);
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="TValue">The maybe value type.</typeparam>
        /// <typeparam name="TIntermediate">The intermediate type.</typeparam>
        /// <typeparam name="TResult">The result value type.</typeparam>
        /// <param name="self">The maybe task.</param>
        /// <param name="mapper">A transform function to apply to the value.</param>
        /// <param name="getResult">A transform function to apply to the intermediate.</param>
        /// <returns>A new Maybe.</returns>
        public static Task<Maybe<TResult>> SelectMany<TValue, TIntermediate, TResult>(
            this Task<Maybe<TValue>> self,
            Func<TValue, Task<Maybe<TIntermediate>>> mapper,
            Func<TValue, TIntermediate, Task<TResult>> getResult) =>
            self.BindAsync(value =>
                mapper(value).BindAsync(
                    intermediate =>
                        Maybe.SomeAsync(getResult(value, intermediate))));
        #endregion

        #region If

        /// <summary>
        /// Convert the maybe to <see cref="SomeValue{TValue}" /> or <see cref="NoneValue" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="maybeTask">The maybe task.</param>
        /// <returns>
        /// If some <see cref="SomeValue{TValue}" /> otherwise <see cref="NoneValue" />.
        /// </returns>
        public static Task<IMaybeValue> AsMaybeValueAsync<TValue>(this Task<Maybe<TValue>> maybeTask) =>
            maybeTask.Map(m => m.AsMaybeValue());

        /// <summary>
        /// Executes an action when <see cref="Maybe{TValue}.IfSome" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="maybeTask">The maybe task.</param>
        /// <param name="onSome">Action to execute.</param>
        /// <returns>A Task.</returns>
        public static Task IfSomeAsync<TValue>(this Task<Maybe<TValue>> maybeTask, Action<TValue> onSome)
        {
            if (maybeTask is null)
            {
                throw new ArgumentNullException(nameof(maybeTask));
            }

            if (onSome is null)
            {
                throw new ArgumentNullException(nameof(onSome));
            }

            return IfSomeInternalAsync(maybeTask, onSome);
        }

        /// <summary>
        /// Executes an action when <see cref="Maybe{TValue}.IfSome" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="maybe">The maybe.</param>
        /// <param name="onSome">Action to execute.</param>
        /// <returns>A Task.</returns>
        public static Task IfSomeAsync<TValue>(this Maybe<TValue> maybe, Func<TValue, Task> onSome)
        {
            if (onSome == null)
            {
                throw new ArgumentNullException(nameof(onSome));
            }

            return IfSomeInternalAsync(maybe, onSome);
        }

        /// <summary>
        /// Executes an action when <see cref="Maybe{TValue}.IfSome" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="maybeTask">The maybe task.</param>
        /// <param name="onSome">Action to execute.</param>
        /// <returns>A Task.</returns>
        public static Task IfSomeAsync<TValue>(this Task<Maybe<TValue>> maybeTask, Func<TValue, Task> onSome)
        {
            if (maybeTask == null)
            {
                throw new ArgumentNullException(nameof(maybeTask));
            }

            if (onSome == null)
            {
                throw new ArgumentNullException(nameof(onSome));
            }

            return IfSomeInternalAsync(maybeTask, onSome);
        }

        /// <summary>
        /// Executes an action when <see cref="Maybe{TValue}.IfNone" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="maybeTask">The maybe task.</param>
        /// <param name="onNone">Action to execute.</param>
        /// <returns>A Task.</returns>
        public static Task IfNoneAsync<TValue>(this Task<Maybe<TValue>> maybeTask, Action onNone)
        {
            if (maybeTask == null)
            {
                throw new ArgumentNullException(nameof(maybeTask));
            }

            if (onNone == null)
            {
                throw new ArgumentNullException(nameof(onNone));
            }

            return IfNoneInternalAsync(maybeTask, onNone);
        }

        /// <summary>
        /// Executes an action when <see cref="Maybe{TValue}.IfNone" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="maybe">The maybe.</param>
        /// <param name="onNone">Action to execute.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public static Task IfNoneAsync<TValue>(this Maybe<TValue> maybe, Task onNone)
        {
            if (onNone == null)
            {
                throw new ArgumentNullException(nameof(onNone));
            }

            return IfNoneInternalAsync(maybe, onNone);
        }

        /// <summary>
        /// Executes an action when <see cref="Maybe{TValue}.IfNone" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="maybeTask">The maybe task.</param>
        /// <param name="onNone">Action to execute.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public static Task IfNoneAsync<TValue>(this Task<Maybe<TValue>> maybeTask, Task onNone)
        {
            if (maybeTask == null)
            {
                throw new ArgumentNullException(nameof(maybeTask));
            }

            if (onNone == null)
            {
                throw new ArgumentNullException(nameof(onNone));
            }

            return IfNoneInternalAsync(maybeTask, onNone);
        }

        /// <summary>
        /// Executes onSome when is some else executes onNone.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="maybeTask">The maybe task.</param>
        /// <param name="onSome">The on some function.</param>
        /// <param name="onNone">The on none function.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public static Task DoAsync<TValue>(this Task<Maybe<TValue>> maybeTask, Action<TValue> onSome, Action onNone)
        {
            if (maybeTask == null)
            {
                throw new ArgumentNullException(nameof(maybeTask));
            }

            if (onSome == null)
            {
                throw new ArgumentNullException(nameof(onSome));
            }

            if (onNone == null)
            {
                throw new ArgumentNullException(nameof(onNone));
            }

            return DoInternalAsync(maybeTask, onSome, onNone);
        }

        /// <summary>
        /// Executes onSome when is some else executes onNone.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="maybe">The maybe.</param>
        /// <param name="onSome">The on some function.</param>
        /// <param name="onNone">The on none function.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public static Task DoAsync<TValue>(this Maybe<TValue> maybe, Func<TValue, Task> onSome, Task onNone)
        {
            if (onSome == null)
            {
                throw new ArgumentNullException(nameof(onSome));
            }

            if (onNone == null)
            {
                throw new ArgumentNullException(nameof(onNone));
            }

            return DoInternalAsync(maybe, onSome, onNone);
        }

        /// <summary>
        /// Executes onSome when is some else executes onNone.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="maybeTask">The maybe task.</param>
        /// <param name="onSome">The on some function.</param>
        /// <param name="onNone">The on none function.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public static Task DoAsync<TValue>(this Task<Maybe<TValue>> maybeTask, Func<TValue, Task> onSome, Task onNone)
        {
            if (maybeTask == null)
            {
                throw new ArgumentNullException(nameof(maybeTask));
            }

            if (onSome == null)
            {
                throw new ArgumentNullException(nameof(onSome));
            }

            if (onNone == null)
            {
                throw new ArgumentNullException(nameof(onNone));
            }

            return DoInternalAsync(maybeTask, onSome, onNone);
        }

        #endregion

        #region Match

        /// <summary>
        /// Pattern match the maybe.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TReturn">The type of the returned value.</typeparam>
        /// <param name="maybeTask">The maybe task.</param>
        /// <param name="some">Function to execute on some.</param>
        /// <param name="none">Function to execute on none.</param>
        /// <returns>
        /// Returns the output of some whe <see cref="Maybe{TValue}.IsSome" /> else returns the output of none.
        /// </returns>
        public static Task<TReturn> MatchAsync<TValue, TReturn>(
            this Task<Maybe<TValue>> maybeTask,
            Func<TValue, TReturn> some,
            Func<TReturn> none) =>
            maybeTask.Map(m => m.Match(some, none));

        /// <summary>
        /// Pattern match the maybe.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TReturn">The type of the returned value.</typeparam>
        /// <param name="maybe">The maybe.</param>
        /// <param name="some">Function to execute on some.</param>
        /// <param name="none">Function to execute on none.</param>
        /// <returns>
        /// Returns the output of some whe <see cref="Maybe{TValue}.IsSome" /> else returns the output of none.
        /// </returns>
        public static Task<TReturn> MatchAsync<TValue, TReturn>(
            this Maybe<TValue> maybe,
            Func<TValue, Task<TReturn>> some,
            Func<Task<TReturn>> none)
        {
            if (some == null)
            {
                throw new ArgumentNullException(nameof(some));
            }

            if (none == null)
            {
                throw new ArgumentNullException(nameof(none));
            }

            return MatchInternalAsync(maybe, some, none);
        }

        /// <summary>
        /// Pattern match the maybe.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TReturn">The type of the returned value.</typeparam>
        /// <param name="maybeTask">The maybe task.</param>
        /// <param name="some">Function to execute on some.</param>
        /// <param name="none">Function to execute on none.</param>
        /// <returns>
        /// Returns the output of some whe <see cref="Maybe{TValue}.IsSome" /> else returns the output of none.
        /// </returns>
        public static Task<TReturn> MatchAsync<TValue, TReturn>(
            this Task<Maybe<TValue>> maybeTask,
            Func<TValue, Task<TReturn>> some,
            Func<Task<TReturn>> none) =>
            maybeTask.Bind(m => m.MatchAsync(some, none));

        /// <summary>
        /// Return value if some else invoke the none function.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="maybeTask">The maybe task.</param>
        /// <param name="none">Invoked when none.</param>
        /// <returns>
        /// The value if some else the output of the none function.
        /// </returns>
        public static Task<TValue> SomeOrProvidedAsync<TValue>(this Task<Maybe<TValue>> maybeTask, Func<TValue> none) =>
            maybeTask.Map(m => m.SomeOrProvided(none));

        /// <summary>
        /// Return value if some else invoke the none function.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="maybe">The maybe.</param>
        /// <param name="none">Invoked when none.</param>
        /// <returns>
        /// The value if some else the output of the none function.
        /// </returns>
        public static Task<TValue> SomeOrProvidedAsync<TValue>(this Maybe<TValue> maybe, Func<Task<TValue>> none)
        {
            if (none == null)
            {
                throw new ArgumentNullException(nameof(none));
            }

            return SomeOrProvidedInternalAsync(maybe, none);
        }

        /// <summary>
        /// Return value if some else invoke the none function.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="maybeTask">The maybe task.</param>
        /// <param name="none">Invoked when none.</param>
        /// <returns>
        /// The value if some else the output of the none function.
        /// </returns>
        public static Task<TValue> SomeOrProvidedAsync<TValue>(this Task<Maybe<TValue>> maybeTask, Func<Task<TValue>> none) =>
            maybeTask.Bind(maybe => maybe.SomeOrProvidedAsync(none));

        #endregion

        #region private helpers

        private static async Task IfSomeInternalAsync<TValue>(this Task<Maybe<TValue>> maybeTask, Func<TValue, Task> onSome)
        {
            var maybe = await maybeTask.ConfigureAwait(false);
            await maybe.IfSomeAsync(onSome).ConfigureAwait(false);
        }

        private static async Task IfSomeInternalAsync<TValue>(this Maybe<TValue> maybe, Func<TValue, Task> onSome)
        {
            if (maybe.IsSome)
            {
                await onSome(maybe.GetValueUnsafe()).ConfigureAwait(false);
            }
        }

        private static async Task IfSomeInternalAsync<TValue>(this Task<Maybe<TValue>> maybeTask, Action<TValue> onSome)
        {
            var maybe = await maybeTask.ConfigureAwait(false);
            maybe.IfSome(onSome);
        }

        private static async Task IfNoneInternalAsync<TValue>(this Task<Maybe<TValue>> maybeTask, Action onNone)
        {
            var maybe = await maybeTask.ConfigureAwait(false);
            maybe.IfNone(onNone);
        }

        private static async Task IfNoneInternalAsync<TValue>(this Maybe<TValue> maybe, Task onNone)
        {
            if (maybe.IsNone)
            {
                await onNone.ConfigureAwait(false);
            }
        }

        private static async Task IfNoneInternalAsync<TValue>(this Task<Maybe<TValue>> maybeTask, Task onNone)
        {
            var maybe = await maybeTask.ConfigureAwait(false);
            await maybe.IfNoneAsync(onNone).ConfigureAwait(false);
        }

        private static async Task DoInternalAsync<TValue>(this Task<Maybe<TValue>> maybeTask, Action<TValue> onSome, Action onNone)
        {
            var maybe = await maybeTask.ConfigureAwait(false);
            maybe.Do(onSome, onNone);
        }

        private static async Task DoInternalAsync<TValue>(this Maybe<TValue> maybe, Func<TValue, Task> onSome, Task onNone)
        {
            if (maybe.IsSome)
            {
                await onSome(maybe.GetValueUnsafe()).ConfigureAwait(false);
            }
            else
            {
                await onNone.ConfigureAwait(false);
            }
        }

        private static async Task DoInternalAsync<TValue>(this Task<Maybe<TValue>> maybeTask, Func<TValue, Task> onSome, Task onNone)
        {
            var maybe = await maybeTask.ConfigureAwait(false);
            await maybe.DoAsync(onSome, onNone).ConfigureAwait(false);
        }

        private static async Task<TReturn> MatchInternalAsync<TValue, TReturn>(
            this Maybe<TValue> maybe,
            Func<TValue, Task<TReturn>> some,
            Func<Task<TReturn>> none)
        {
            if (maybe.AsMaybeValue() is SomeValue<TValue> someValue)
            {
                return await some(someValue.Value).ConfigureAwait(false);
            }

            return await none().ConfigureAwait(false);
        }

        private static async Task<TValue> SomeOrProvidedInternalAsync<TValue>(this Maybe<TValue> maybe, Func<Task<TValue>> none)
        {
            if (maybe.AsMaybeValue() is SomeValue<TValue> someValue)
            {
                return someValue;
            }

            return await none().ConfigureAwait(false);
        }

        #endregion
    }
}
