using AutofacContrib.NSubstitute;
using AutoFixture;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WWTWebservices;
using Xunit;

namespace WWT.Providers.Tests
{
    public abstract class ProviderTests<T>
        where T : RequestProvider
    {
        private readonly Fixture _fixture;

        public ProviderTests()
        {
            _fixture = new Fixture();
            Options = _fixture.Create<FilePathOptions>();
        }

        protected FilePathOptions Options { get; }

        protected abstract int MaxLevel { get; }

        protected virtual Action<IResponse> StreamExceptionResponseHandler { get; } = response =>
        {
            response.Received(1).Write("No image");
            Assert.Equal("text/plain", response.ContentType);
        };

        protected virtual Action<IResponse> NullStreamResponseHandler { get; } = response =>
        {
            response.Received(1).Write("No image");
            Assert.Equal("text/plain", response.ContentType);
        };

        protected abstract Stream GetStreamFromPlateTilePyramid(IPlateTilePyramid plateTiles, int level, int x, int y);

        protected virtual void ExpectedResponseAboveMaxLevel(IResponse response)
        {
            response.Received(1).Write("No image");
            Assert.Equal("text/plain", response.ContentType);
        }

        [Fact]
        public void ExpectedTests()
        {
            for (int level = 0; level < MaxLevel; level++)
            {
                // Arrange
                var data = _fixture.CreateMany<byte>().ToArray();
                var x = _fixture.Create<int>();
                var y = _fixture.Create<int>();

                using var container = AutoSubstitute.Configure()
                    .InitializeProviderTests()
                    .Provide(Options)
                    .ConfigureParameterQ(level, x, y)
                    .Build();

                GetStreamFromPlateTilePyramid(container.Resolve<IPlateTilePyramid>(), level, x, y).Returns(new MemoryStream(data));

                // Act
                container.RunProviderTest<T>();

                // Assert
                GetStreamFromPlateTilePyramid(container.Resolve<IPlateTilePyramid>().Received(1), level, x, y);
                Assert.Equal(data, container.GetOutputData());
            }
        }

        [Fact]
        public void ErrorInStream()
        {
            foreach (var level in Enumerable.Range(0, MaxLevel))
            {
                // Arrange
                var data = _fixture.CreateMany<byte>().ToArray();
                var x = _fixture.Create<int>();
                var y = _fixture.Create<int>();

                using var container = AutoSubstitute.Configure()
                    .InitializeProviderTests()
                    .Provide(Options)
                    .ConfigureParameterQ(level, x, y)
                    .Build();

                GetStreamFromPlateTilePyramid(container.Resolve<IPlateTilePyramid>(), level, x, y).Returns(_ => throw new InvalidOperationException());

                if (StreamExceptionResponseHandler is null)
                {
                    // Act
                    Assert.Throws<InvalidOperationException>(() => container.RunProviderTest<T>());
                }
                else
                {
                    // Act
                    container.RunProviderTest<T>();

                    // Assert
                    GetStreamFromPlateTilePyramid(container.Resolve<IPlateTilePyramid>().Received(1), level, x, y);
                    StreamExceptionResponseHandler(container.Resolve<IResponse>());
                }
            }
        }

        [Fact]
        public void EmptyResult()
        {
            // Arrange
            var level = _fixture.Create<int>() % MaxLevel;
            var x = _fixture.Create<int>();
            var y = _fixture.Create<int>();

            using var container = AutoSubstitute.Configure()
                .Provide(Options)
                .InitializeProviderTests()
                .ConfigureParameterQ(level, x, y)
                .Build();

            var empty = new MemoryStream();
            GetStreamFromPlateTilePyramid(container.Resolve<IPlateTilePyramid>(), level, x, y).Returns(empty);

            // Act
            container.RunProviderTest<T>();

            // Assert
            GetStreamFromPlateTilePyramid(container.Resolve<IPlateTilePyramid>().Received(1), level, x, y);
            Assert.Empty(container.GetOutputData());
        }

        [Fact]
        public void NullResult()
        {
            // Arrange
            var level = _fixture.Create<int>() % MaxLevel;
            var x = _fixture.Create<int>();
            var y = _fixture.Create<int>();

            using var container = AutoSubstitute.Configure()
                .Provide(Options)
                .InitializeProviderTests()
                .ConfigureParameterQ(level, x, y)
                .Build();

            GetStreamFromPlateTilePyramid(container.Resolve<IPlateTilePyramid>(), level, x, y).Returns((Stream)null);

            if (NullStreamResponseHandler is null)
            {
                Assert.Throws<NullReferenceException>(() => container.RunProviderTest<T>());
            }
            else
            {
                // Act
                container.RunProviderTest<T>();

                // Assert
                GetStreamFromPlateTilePyramid(container.Resolve<IPlateTilePyramid>().Received(1), level, x, y);
                NullStreamResponseHandler(container.Resolve<IResponse>());
            }
        }

        [Fact]
        public void AboveMaxLevel()
        {
            foreach (var level in GetLevelsAboveMax())
            {
                // Arrange
                var x = _fixture.Create<int>();
                var y = _fixture.Create<int>();

                using var container = AutoSubstitute.Configure()
                    .Provide(Options)
                    .InitializeProviderTests()
                    .ConfigureParameterQ(level, x, y)
                    .Build();

                // Act
                container.RunProviderTest<T>();

                // Assert
                ExpectedResponseAboveMaxLevel(container.Resolve<IResponse>());
            }

            IEnumerable<int> GetLevelsAboveMax()
            {
                for (int i = 0; i < 10; i++)
                {
                    yield return MaxLevel + _fixture.Create<int>();
                }
            }
        }
    }
}
