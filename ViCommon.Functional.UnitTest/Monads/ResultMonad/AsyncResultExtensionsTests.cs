using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ViCommon.Functional.Monads.ResultMonad;
using static ViCommon.Functional.UnitTest.TestHelper;

namespace ViCommon.Functional.UnitTest.Monads.ResultMonad
{
    [TestClass]
    public class AsyncResultExtensionsTests
    {
        [TestMethod]
        public async Task MapAsyncTest()
        {
            // arrange
            var value = RandomInt();
            var success = Task.FromResult(Result.Success<int, Failure>(value));


            // act
            var result = await success.MapAsync(i => i + 1, failure => failure);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.GetSuccessUnsafe().Should().Be(value + 1);
        }

        [TestMethod]
        public async Task MapAsyncTest2_failure()
        {
            // arrange
            var msg = RandomString();
            var failure = Task.FromResult(Result.Failure<int, Failure>(new Failure(msg)));


            // act
            var result = await failure.MapAsync(
                i => Task.FromResult(i + 1),
                f => Task.FromResult(new Failure(f.Message + "test")));

            // assert
            result.IsFailure.Should().BeTrue();
            result.GetFailureUnsafe().Message.Should().Be(msg + "test");
        }

        [TestMethod]
        public async Task MapAsyncTest2()
        {
            // arrange
            var value = RandomInt();
            var success = Result.Success<int, Failure>(value);

            // act
            var result = await success.MapAsync(i => Task.FromResult(i + 1), Task.FromResult);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.GetSuccessUnsafe().Should().Be(value + 1);
        }

        [TestMethod()]
        public async Task MapSuccessAsyncTest()
        {
            // arrange
            var value = RandomInt();
            var success = Task.FromResult(Result.Success<int, Failure>(value));

            // act
            var result = await success.MapSuccessAsync(i => i + 1);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.GetSuccessUnsafe().Should().Be(value + 1);
        }


        [TestMethod()]
        public async Task MapSuccessAsyncTest2()
        {
            // arrange
            var value = RandomInt();
            var success = Task.FromResult(Result.Success<int, Failure>(value));

            // act
            var result = await success.MapSuccessAsync(i => Task.FromResult(i + 1));

            // assert
            result.IsSuccess.Should().BeTrue();
            result.GetSuccessUnsafe().Should().Be(value + 1);
        }

        [TestMethod]
        public async Task MapFailureAsyncTest()
        {
            // arrange
            var msg = RandomString();
            var failure = Task.FromResult(Result.Failure<int, Failure>(new Failure(msg)));

            // act
            var result = await failure.MapFailureAsync(f => new Failure(f.Message + "test"));

            // assert
            result.IsFailure.Should().BeTrue();
            result.GetFailureUnsafe().Message.Should().Be(msg + "test");
        }

        [TestMethod]
        public async Task MapFailureAsyncTest2()
        {
            // arrange
            var msg = RandomString();
            var failure = Task.FromResult(Result.Failure<int, Failure>(new Failure(msg)));

            // act
            var result = await failure.MapFailureAsync(f =>
                Task.FromResult(new Failure(f.Message + "test")));

            // assert
            result.IsFailure.Should().BeTrue();
            result.GetFailureUnsafe().Message.Should().Be(msg + "test");
        }

        [TestMethod]
        public async Task BindAsyncTest()
        {
            // arrange
            var value = RandomInt();
            var success = Task.FromResult(Result.Success<int, Failure>(value));

            // act
            var result = await success.BindAsync(i => Result.Success<int, Failure>(i + 1));

            // assert
            result.IsSuccess.Should().BeTrue();
            result.GetSuccessUnsafe().Should().Be(value + 1);
        }

        [TestMethod]
        public async Task BindAsyncTest2()
        {
            // arrange
            var value = RandomInt();
            var success = Task.FromResult(Result.Success<int, Failure>(value));

            // act
            var result = await success.BindAsync(i => Task.FromResult(Result.Success<int, Failure>(i + 1)));

            // assert
            result.IsSuccess.Should().BeTrue();
            result.GetSuccessUnsafe().Should().Be(value + 1);
        }

        [TestMethod]
        public async Task BindFailureAsyncTest()
        {
            // arrange
            var msg = RandomString();
            var failure = Task.FromResult(Result.Failure<int, Failure>(new Failure(msg)));

            // act
            var result = await failure.BindFailureAsync(f =>
                Result.Failure<int, Failure>(
                    new Failure(f.Message + "test")));

            // assert
            result.IsFailure.Should().BeTrue();
            result.GetFailureUnsafe().Message.Should().Be(msg + "test");
        }

        [TestMethod]
        public async Task BindFailureAsyncTest2()
        {
            // arrange
            var msg = RandomString();
            var failure = Task.FromResult(Result.Failure<int, Failure>(new Failure(msg)));

            // act
            var result = await failure.BindFailureAsync(f =>
                Task.FromResult(Result.Failure<int, Failure>(
                    new Failure(f.Message + "test"))));

            // assert
            result.IsFailure.Should().BeTrue();
            result.GetFailureUnsafe().Message.Should().Be(msg + "test");
        }

        [TestMethod]
        public async Task MatchAsyncSuccessTest()
        {
            // arrange
            var value = RandomInt();
            var successMsg = RandomString();
            var failureMsg = RandomString();
            var success = Task.FromResult(Result.Success<int, Failure>(value));
            var failure = Task.FromResult(Result.Failure<int, Failure>(new Failure(failureMsg)));

            // act
            var resultSuccess = await success.MatchAsync(i => new Failure(successMsg));
            var resultFailure = await failure.MatchAsync(i => new Failure(successMsg));

            // assert
            resultSuccess.Message.Should().Be(successMsg);
            resultFailure.Message.Should().Be(failureMsg);
        }

        [TestMethod]
        public async Task MatchAsyncSuccessTest2()
        {
            // arrange
            var value = RandomInt();
            var successMsg = RandomString();
            var failureMsg = RandomString();
            var success = Task.FromResult(Result.Success<int, Failure>(value));
            var failure = Task.FromResult(Result.Failure<int, Failure>(new Failure(failureMsg)));

            // act
            var resultSuccess = await success.MatchAsync(i => Task.FromResult(new Failure(successMsg)));
            var resultFailure = await failure.MatchAsync(i => Task.FromResult(new Failure(successMsg)));

            // assert
            resultSuccess.Message.Should().Be(successMsg);
            resultFailure.Message.Should().Be(failureMsg);
        }

        [TestMethod]
        public async Task MatchAsyncFailureTest()
        {
            // arrange
            var successValue = RandomInt();
            var failureValue = RandomInt();
            var success = Task.FromResult(Result.Success<int, Failure>(successValue));
            var failure = Task.FromResult(Result.Failure<int, Failure>(new Failure(RandomString())));

            // act
            var resultSuccess = await success.MatchAsync(f => failureValue);
            var resultFailure = await failure.MatchAsync(f => failureValue);

            // assert
            resultSuccess.Should().Be(successValue);
            resultFailure.Should().Be(failureValue);
        }

        [TestMethod]
        public async Task MatchAsyncFailureTest2()
        {
            // arrange
            var successValue = RandomInt();
            var failureValue = RandomInt();
            var success = Task.FromResult(Result.Success<int, Failure>(successValue));
            var failure = Task.FromResult(Result.Failure<int, Failure>(new Failure(RandomString())));

            // act
            var resultSuccess = await success.MatchAsync(f => Task.FromResult(failureValue));
            var resultFailure = await failure.MatchAsync(f => Task.FromResult(failureValue));

            // assert
            resultSuccess.Should().Be(successValue);
            resultFailure.Should().Be(failureValue);
        }

        [TestMethod]
        public async Task AsResultValueTest()
        {
            // arrange
            var successValue = RandomInt();
            var failureValue = new Failure(RandomString());
            var success = Task.FromResult(Result.Success<int, Failure>(successValue));
            var failure = Task.FromResult(Result.Failure<int, Failure>(failureValue));

            // act
            var resultSuccess = await success.AsResultValueAsync();
            var resultFailure = await failure.AsResultValueAsync();

            // assert
            var sVal = resultSuccess.Should().BeAssignableTo<SuccessValue<int>>().Subject;
            sVal.Value.Should().Be(successValue);
            var fVal = resultFailure.Should().BeAssignableTo<FailureValue<Failure>>().Subject;
            fVal.Value.Should().Be(failureValue);
        }
    }
}
