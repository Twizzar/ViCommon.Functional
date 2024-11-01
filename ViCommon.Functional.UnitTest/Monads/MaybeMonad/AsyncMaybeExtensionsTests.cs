using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ViCommon.Functional.Monads.MaybeMonad;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;
using static ViCommon.Functional.UnitTest.TestHelper;

namespace ViCommon.Functional.UnitTest.Monads.MaybeMonad
{
    [TestClass()]
    public class AsyncMaybeExtensionsTests
    {
        [TestMethod]
        public async Task GetValueUnsafeAsync_Some_Test()
        {
            // arrange
            var value = RandomInt();
            var some = SomeAsync(Task.FromResult(value));

            // act
            var result = await some.GetValueUnsafeAsync();

            // assert
            result.Should().Be(value);
        }

        [TestMethod]
        public void GetValueUnsafeAsync_None_Test()
        {
            // arrange
            var none = Task.FromResult(None<int>());

            // act
            Func<Task> func = () => none.GetValueUnsafeAsync();

            // assert
            func.Should().Throw<NullReferenceException>();
        }


        [TestMethod()]
        public async Task MapAsyncTest()
        {
            // arrange
            var value = RandomInt();
            var some = Task.FromResult(Some(value));

            // act
            var result = await some.MapAsync(i => i + 1);

            // assert
            result.IsSome.Should().BeTrue();
            result.GetValueUnsafe().Should().Be(value + 1);
        }

        [TestMethod]
        public async Task MapAsyncTest2()
        {
            static async Task<int> Calculate(int x)
            {
                await Task.Yield();
                return x + 1;
            }

                // arrange
            var value = RandomInt();
            var some = Task.FromResult(Some(value));

            // act
            var result = await some.MapAsync(Calculate);

            // assert
            result.IsSome.Should().BeTrue();
            result.GetValueUnsafe().Should().Be(value + 1);
        }

        [TestMethod]
        public async Task MapAsync_None_Test()
        {
            // arrange
            var none = Task.FromResult(None<int>());

            // act
            var result = await none.MapAsync(i => Task.FromResult(i + 1));

            // assert
            result.IsNone.Should().BeTrue();
        }

        [TestMethod]
        public async Task MapAsyncTest3()
        {
            static async Task<int> Calculate(int x)
            {
                await Task.Yield();
                return x + 1;
            }

            // arrange
            var value = RandomInt();
            var some = Some(value);

            // act
            var result = await some.MapAsync(Calculate);

            // assert
            result.IsSome.Should().BeTrue();
            result.GetValueUnsafe().Should().Be(value + 1);
        }

        [TestMethod]
        public void MapAsyncTest_exceptions_in_map_func_is_thrown()
        {
            static async Task<int> Calculate(int x)
            {
                await Task.Delay(1);
                throw new Exception();
            }

            // arrange
            var value = RandomInt();
            var some = Some(value);

            // act
            Func<Task> action = async () => await some.MapAsync(Calculate);

            // assert
            action.Should().Throw<Exception>();
        }

        [TestMethod]
        public async Task BindAsyncTest()
        {
            // arrange
            var value = RandomInt();
            var some = Task.FromResult(Some(value));

            // act
            var result = await some.BindAsync(i => Some(i + 1));

            // assert
            result.IsSome.Should().BeTrue();
            result.GetValueUnsafe().Should().Be(value + 1);
        }

        [TestMethod]
        public async Task BindAsync_None_Test()
        {
            // arrange
            var none = Task.FromResult(Maybe.None<int>());

            // act
            var result = await none.BindAsync(i => Task.FromResult(Maybe.Some(RandomInt())));

            // assert
            result.IsNone.Should().BeTrue();
        }

        [TestMethod]
        public async Task BindAsyncTest2()
        {
            static async Task<Maybe<int>> Calculate(int x)
            {
                await Task.Yield();
                return Some(x + 1);
            }

            // arrange
            var value = RandomInt();
            var some = Task.FromResult(Some(value));

            // act
            var result = await some.BindAsync(Calculate);

            // assert
            result.IsSome.Should().BeTrue();
            result.GetValueUnsafe().Should().Be(value + 1);
        }

        [TestMethod]
        public async Task BindAsyncTest3()
        {
            static async Task<Maybe<int>> Calculate(int x)
            {
                await Task.Yield();
                return Some(x + 1);
            }

            // arrange
            var value = RandomInt();
            var some = Some(value);

            // act
            var result = await some.BindAsync(Calculate);

            // assert
            result.IsSome.Should().BeTrue();
            result.GetValueUnsafe().Should().Be(value + 1);
        }

        [TestMethod]
        public async Task BindNoneAsyncTest()
        {
            // arrange
            var newValue = RandomInt();
            var none = Task.FromResult(None<int>());

            // act
            var result = await none.BindNoneAsync(() => Some(newValue));

            // assert
            result.IsSome.Should().BeTrue();
            result.GetValueUnsafe().Should().Be(newValue);
        }

        [TestMethod]
        public async Task BindNoneAsyncTest2()
        {
            // arrange
            var newValue = RandomInt();
            var none = Task.FromResult(None<int>());

            // act
            var result = await none.BindNoneAsync(() => Task.FromResult(Some(newValue)));

            // assert
            result.IsSome.Should().BeTrue();
            result.GetValueUnsafe().Should().Be(newValue);
        }

        [TestMethod]
        public async Task BindNoneAsync_Some_Test()
        {
            // arrange
            var newValue = RandomInt();
            var oldValue = RandomInt();
            var some = Task.FromResult(Some(oldValue));

            // act
            var result = await some.BindNoneAsync(() => Task.FromResult(Some(newValue)));

            // assert
            result.IsSome.Should().BeTrue();
            result.GetValueUnsafe().Should().Be(oldValue);
        }

        [TestMethod]
        public async Task Match_some_Test()
        {
            // arrange
            var value = RandomInt(0);
            var some = Task.FromResult(Some(value));

            // act
            var result = await some.MatchAsync(i => i + 1, () => RandomInt(int.MinValue, 0));

            // assert
            result.Should().Be(value + 1);
        }

        [TestMethod]
        public async Task Match_none_Test()
        {
            // arrange
            var matchValue = RandomString();
            var none = Task.FromResult(None<int>());

            // act
            var result = await none.MatchAsync(i => RandomString(), () => matchValue);

            // assert
            result.Should().Be(matchValue);
        }

        [TestMethod]
        public async Task Match2_some_Test()
        {
            // arrange
            var value = RandomInt(0);
            var some = Task.FromResult(Some(value));

            // act
            var result = await some.MatchAsync(i => Task.FromResult(i + 1), () => Task.FromResult(RandomInt(int.MinValue, 0)));

            // assert
            result.Should().Be(value + 1);
        }

        [TestMethod]
        public async Task Match2_none_Test()
        {
            // arrange
            var matchValue = RandomString();
            var none = Task.FromResult(None<int>());

            // act
            var result = await none.MatchAsync(i => Task.FromResult(RandomString()), () => Task.FromResult(matchValue));

            // assert
            result.Should().Be(matchValue);
        }

        [TestMethod]
        public async Task SomeOrProvidedAsync_some_Test()
        {
            // arrange
            var value = RandomInt();
            var some = Task.FromResult(Some(value));

            // act
            var result = await some.SomeOrProvidedAsync(() => RandomInt());

            // assert
            result.Should().Be(value);
        }

        [TestMethod]
        public async Task SomeOrProvidedAsync_none_Test()
        {
            // arrange
            var value = RandomInt();
            var some = Task.FromResult(None<int>());

            // act
            var result = await some.SomeOrProvidedAsync(() => value);

            // assert
            result.Should().Be(value);
        }

        [TestMethod]
        public async Task SomeOrProvidedAsync2_some_Test()
        {
            // arrange
            var value = RandomInt();
            var some = Task.FromResult(Some(value));

            // act
            var result = await some.SomeOrProvidedAsync(() => Task.FromResult(RandomInt()));

            // assert
            result.Should().Be(value);
        }

        [TestMethod]
        public async Task SomeOrProvidedAsync2_none_Test()
        {
            // arrange
            var value = RandomInt();
            var some = Task.FromResult(None<int>());

            // act
            var result = await some.SomeOrProvidedAsync(() => Task.FromResult(value));

            // assert
            result.Should().Be(value);
        }
    }
}