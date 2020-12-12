using AutofacContrib.NSubstitute;
using AutoFixture;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using WWT.PlateFiles;

namespace WWT.Providers.Tests
{
    public abstract class ProviderTests<T>
        where T : RequestProvider
    {
        public ProviderTests()
        {
            Fixture = new Fixture();
            Options = Fixture.Create<WwtOptions>();
        }

        protected Fixture Fixture { get; }

        protected WwtOptions Options { get; }

        protected virtual int MaxLevel => -1;

        protected virtual Action<IResponse> StreamExceptionResponseHandler { get; } = response =>
        {
            response.Received(1).WriteAsync("No image", default);
            Assert.Equal("text/plain", response.ContentType);
        };

        protected virtual Action<IResponse> NullStreamResponseHandler { get; } = response =>
        {
            response.Received(1).WriteAsync("No image", default);
            Assert.Equal("text/plain", response.ContentType);
        };

        protected abstract Task<Stream> GetStreamFromPlateTilePyramidAsync(IPlateTilePyramid plateTiles, int level, int x, int y);

        protected virtual void ExpectedResponseAboveMaxLevel(IResponse response)
        {
            response.Received(1).WriteAsync("No image", default);
            Assert.Equal("text/plain", response.ContentType);
        }

        protected virtual object[] GetParameterQ(int level, int x, int y)
            => new object[] { level, x, y };

        [Fact]
        public async Task ExpectedTests()
        {
            for (int level = 0; level < MaxLevel; level++)
            {
                // Arrange
                var data = Fixture.CreateMany<byte>().ToArray();
                var x = Fixture.Create<int>();
                var y = Fixture.Create<int>();

                using var container = AutoSubstitute.Configure()
                    .InitializeProviderTests()
                    .Provide(Options)
                    .ConfigureParameterQ(GetParameterQ(level, x, y))
                    .Build();

                GetStreamFromPlateTilePyramidAsync(container.Resolve<IPlateTilePyramid>(), level, x, y).Returns(new MemoryStream(data));

                // Act
                await container.RunProviderTestAsync<T>();

                // Assert
                await GetStreamFromPlateTilePyramidAsync(container.Resolve<IPlateTilePyramid>().Received(1), level, x, y);
                Assert.Equal(data, container.GetOutputData());
            }
        }

        [Fact]
        public async Task ErrorInStream()
        {
            var maxLevel = MaxLevel < 0 ? 10 : MaxLevel;

            foreach (var level in Enumerable.Range(0, maxLevel))
            {
                // Arrange
                var data = Fixture.CreateMany<byte>().ToArray();
                var x = Fixture.Create<int>();
                var y = Fixture.Create<int>();

                using var container = AutoSubstitute.Configure()
                    .InitializeProviderTests()
                    .Provide(Options)
                    .ConfigureParameterQ(GetParameterQ(level, x, y))
                    .Build();

                GetStreamFromPlateTilePyramidAsync(container.Resolve<IPlateTilePyramid>(), level, x, y).Returns<Task<Stream>>(_ => throw new InvalidOperationException());

                if (StreamExceptionResponseHandler is null)
                {
                    // Act
                    await Assert.ThrowsAsync<InvalidOperationException>(() => container.RunProviderTestAsync<T>());
                }
                else
                {
                    // Act
                    await container.RunProviderTestAsync<T>();

                    // Assert
                    await GetStreamFromPlateTilePyramidAsync(container.Resolve<IPlateTilePyramid>().Received(1), level, x, y);
                    StreamExceptionResponseHandler(container.Resolve<IResponse>());
                }
            }
        }

        [Fact]
        public async Task EmptyResult()
        {
            // Arrange
            var level = Fixture.Create<int>() % MaxLevel;
            var x = Fixture.Create<int>();
            var y = Fixture.Create<int>();

            using var container = AutoSubstitute.Configure()
                .Provide(Options)
                .InitializeProviderTests()
                .ConfigureParameterQ(GetParameterQ(level, x, y))
                .Build();

            var empty = Task.FromResult<Stream>(new MemoryStream());
            GetStreamFromPlateTilePyramidAsync(container.Resolve<IPlateTilePyramid>(), level, x, y).Returns(empty, default);

            // Act
            await container.RunProviderTestAsync<T>();

            // Assert
            await GetStreamFromPlateTilePyramidAsync(container.Resolve<IPlateTilePyramid>().Received(1), level, x, y);
            Assert.Empty(container.GetOutputData());
        }

        [Fact]
        public async Task NullResult()
        {
            // Arrange
            var level = Fixture.Create<int>() % MaxLevel;
            var x = Fixture.Create<int>();
            var y = Fixture.Create<int>();

            using var container = AutoSubstitute.Configure()
                .Provide(Options)
                .InitializeProviderTests()
                .ConfigureParameterQ(GetParameterQ(level, x, y))
                .Build();

            GetStreamFromPlateTilePyramidAsync(container.Resolve<IPlateTilePyramid>(), level, x, y).Returns((Stream)null);

            if (NullStreamResponseHandler is null)
            {
                await Assert.ThrowsAsync<NullReferenceException>(() => container.RunProviderTestAsync<T>());
            }
            else
            {
                // Act
                await container.RunProviderTestAsync<T>();

                // Assert
                await GetStreamFromPlateTilePyramidAsync(container.Resolve<IPlateTilePyramid>().Received(1), level, x, y);
                NullStreamResponseHandler(container.Resolve<IResponse>());
            }
        }

        [Fact]
        public async Task AboveMaxLevel()
        {
            if (MaxLevel < 0)
            {
                return;
            }

            foreach (var level in GetLevelsAboveMax())
            {
                // Arrange
                var x = Fixture.Create<int>();
                var y = Fixture.Create<int>();

                using var container = AutoSubstitute.Configure()
                    .Provide(Options)
                    .InitializeProviderTests()
                    .ConfigureParameterQ(GetParameterQ(level, x, y))
                    .Build();

                // Act
                await container.RunProviderTestAsync<T>();

                // Assert
                ExpectedResponseAboveMaxLevel(container.Resolve<IResponse>());
            }

            IEnumerable<int> GetLevelsAboveMax()
            {
                for (int i = 0; i < 10; i++)
                {
                    yield return MaxLevel + Fixture.Create<int>();
                }
            }
        }
    }
}
