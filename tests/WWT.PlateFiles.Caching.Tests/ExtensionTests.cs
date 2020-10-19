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
            var services = new ServiceCollection();

            var original = Substitute.For<IPlateTilePyramid>();
            services.AddSingleton(original);

            services.AddCaching(options =>
            {
                options.UseCaching = false;
            });

            using var provider = services.BuildServiceProvider();

            // Act
            var resolved = provider.GetRequiredService<IPlateTilePyramid>();

            // Assert
            Assert.Same(original, resolved);
        }

        [Fact]
        public void InMemoryWhenNoRedisConnection()
        {
            // Arrange
            var services = new ServiceCollection();

            var original = Substitute.For<IPlateTilePyramid>();
            services.AddSingleton(original);

            services.AddCaching(options =>
            {
                options.UseCaching = true;
            });

            using var provider = services.BuildServiceProvider();

            // Act
            var resolved = provider.GetRequiredService<IPlateTilePyramid>();

            // Assert
            var inMemory = Assert.IsType<InMemoryCachedPlateTilePyramid>(resolved);

            Assert.Same(original, inMemory.InnerPyramid);
        }

        [Fact]
        public void RedisUsedWithConnectionString()
        {
            // Arrange
            var services = new ServiceCollection();

            var original = Substitute.For<IPlateTilePyramid>();
            services.AddSingleton(original);

            services.AddCaching(options =>
            {
                options.UseCaching = true;
                options.RedisCacheConnectionString = _fixture.Create<string>();
            });

            services.AddSingleton(Substitute.For<IConnectionMultiplexer>());

            using var provider = services.BuildServiceProvider();

            // Act
            var resolved = provider.GetRequiredService<IPlateTilePyramid>();

            // Assert
            var redis = Assert.IsType<RedisCachedPlateTilePyramid>(resolved);

            Assert.Same(original, redis.InnerPyramid);
        }
    }
}
