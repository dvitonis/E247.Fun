using System;
using System.Threading.Tasks;
using Xunit;
using static E247.Fun.Fun;

namespace E247.Fun.UnitTest
{
    public class UnitTests
    {
        [Fact]
        public void SutIsNotNullable()
        {
            Assert.True(typeof(Unit).IsValueType);
        }

        [Fact]
        public void SutIsSingleton()
        {
            var unit1 = Unit.Value;
            var unit2 = Unit.Value;
            var unit3 = new Unit();

            Assert.Equal(unit1, unit2);
            Assert.Equal(unit1, unit3);
        }

        public Result<string, Unit> TryResultSuccess() => "success";

        public Result<string, Unit> TryResultFailure() => Unit.unit;
        
        public Task<Result<string, Unit>> TryAsyncResultSuccess() =>
            Task.FromResult(Result<string, Unit>.Succeed("success"));
        
        public Task<Result<string, Unit>> TryAsyncResultFailure() =>
            Task.FromResult(Result<string, Unit>.Fail(Unit.unit));

        public Task<string> SimpleAsync() => Task.FromResult("simple");
        
        public Result<Tuple<string, string>, Unit> TryResultTuple() => Tuple.Create("success", "success");

        [Fact]
        public void ComputationExpressionsS()
        {
            var result =
                from s in TryResultSuccess()
                from b in TryResultSuccess()
                select s + b;
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void ComputationExpressionsF()
        {
            var result =
                from s in TryResultSuccess()
                from b in TryResultFailure()
                select s + b;
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public void ComputationExpressions2()
        {
            var result =
                from s in TryResultSuccess()
                let s2 = " smth"
                select s + s2;
            Assert.Equal("success smth", result.Success);
        }

        [Fact]
        public async Task ComputationExpressions3()
        {
            var result =
                from s in TryAsyncResultSuccess()
                select s;

            var actual = await result;
            Assert.Equal("success", actual.Success);
        }

        [Fact]
        public async Task ComputationExpressions3S()
        {
            var result =
                from s in TryAsyncResultSuccess()
                from b in TryAsyncResultSuccess()
                select s + b;
            
            var actual = await result;
            Assert.Equal("successsuccess", actual.Success);
        }

        [Fact]
        public async Task ComputationExpressions3F()
        {
            var result =
                from s in TryAsyncResultSuccess()
                from b in TryAsyncResultFailure()
                select s + b;
            
            var actual = await result;
            Assert.False(actual.IsSuccessful);
        }

        [Fact]
        public async Task ComputationExpressions4S()
        {
            var result =
                from s in TryAsyncResultSuccess()
                from b in TryResultSuccess()
                select s + b;
            
            var actual = await result;
            Assert.Equal("successsuccess", actual.Success);
        }

        [Fact]
        public async Task ComputationExpressions4SInverted()
        {
            var result =
                from b in TryResultSuccess().LiftAsync()
                from s in TryAsyncResultSuccess()
                from k in TryResultSuccess()
                select s + b;
            
            var actual = await result;
            Assert.Equal("successsuccess", actual.Success);
        }

        [Fact]
        public async Task ComputationExpressions4SWithLet()
        {
            var result =
                from s in TryAsyncResultSuccess()
                let k = 2
                from b in TryResultSuccess()
                select s + b;
            
            var actual = await result;
            Assert.Equal("successsuccess", actual.Success);
        }
    }
}
