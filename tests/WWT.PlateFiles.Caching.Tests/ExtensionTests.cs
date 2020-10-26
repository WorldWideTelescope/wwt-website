using AutofacContrib.NSubstitute;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using StackExchange.Redis;
using WWTWebservices;
using Xunit;

namespace WWT.PlateFiles.Caching.Tests
{
    public class ExtensionTests
    {
        private readonly Fixture _fixture;

        public ExtensionTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void NoCachingByDefault()
        {
            // Arrange
            var original = Substitute.For<IPlateTilePyramid>();
            var mock = AutoSubstitute.Configure()
                .ConfigureServices(services =>
                {
                    services.AddSingleton(original);
                    services.AddCaching(options =>
                    {
                        options.UseCaching = false;
                    });
                })
                .SubstituteFor<IConnectionMultiplexer>()
                .Build();

            // Act
            var resolved = mock.Resolve<IPlateTilePyramid>();

            // Assert
            Assert.Same(original, resolved);
        }

        [Fact]
        public void InMemoryWhenNoRedisConnection()
        {
            // Arrange
            var original = Substitute.For<IPlateTilePyramid>();
            var mock = AutoSubstitute.Configure()
                .ConfigureServices(services =>
                {
                    services.AddSingleton(original);
                    services.AddCaching(options =>
                    {
                        options.UseCaching = true;
                    });
                })
                .SubstituteFor<IConnectionMultiplexer>()
                .Build();

            // Act
            var resolved = mock.Resolve<IPlateTilePyramid>();

            // Assert
            var inMemory = Assert.IsType<InMemoryCachedPlateTilePyramid>(resolved);

            Assert.Same(original, inMemory.InnerPyramid);
        }

        [Fact]
        public void RedisUsedWithConnectionString()
        {
            // Arrange
            var original = Substitute.For<IPlateTilePyramid>();
            var mock = AutoSubstitute.Configure()
                .ConfigureServices(services =>
                {
                    services.AddSingleton(original);
                    services.AddCaching(options =>
                    {
                        options.UseCaching = true;
                        options.RedisCacheConnectionString = _fixture.Create<string>();
                    });
                })
                .SubstituteFor<IConnectionMultiplexer>()
                .Build();

            // Act
            var resolved = mock.Resolve<IPlateTilePyramid>();

            // Assert
            var redis = Assert.IsType<RedisCachedPlateTilePyramid>(resolved);

            Assert.Same(original, redis.InnerPyramid);
        }
    }
}
