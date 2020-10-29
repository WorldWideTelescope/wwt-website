using AutofacContrib.NSubstitute;
using AutofacContrib.NSubstitute.MockHandlers;
using AutoFixture;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using StackExchange.Redis;
using System.Threading.Tasks;
using WWTWebservices;
using Xunit;

namespace WWT.Caching.Tests
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
            var original = Substitute.For<ITestService>();
            var mock = AutoSubstitute.Configure()
                .ConfigureServices(services =>
                {
                    services.AddSingleton(original);
                    services
                        .AddCaching(options =>
                        {
                            options.UseCaching = false;
                        })
                        .CacheType<ITestService>(_ => { });
                })
                .SubstituteFor<IConnectionMultiplexer>()
                .Build();

            // Act
            var resolved = mock.Resolve<ITestService>();

            // Assert
            Assert.Same(original, resolved);
        }

        [Fact]
        public void InMemoryWhenNoRedisConnection()
        {
            // Arrange
            var original = Substitute.For<ITestService>();
            var mock = AutoSubstitute.Configure()
                .InjectProperties()
                .MakeUnregisteredTypesPerLifetime()
                .ConfigureOptions(options =>
                {
                    options.MockHandlers.Add(SkipTypeMockHandler.Create(typeof(IValidateOptions<>)));
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton(original);
                    services
                        .AddCaching(options =>
                        {
                            options.UseCaching = true;
                        })
                        .CacheType<ITestService>(_ => { });
                })
                .SubstituteFor<IConnectionMultiplexer>()
                .Build();

            // Act
            var resolved = mock.Resolve<IDistributedCache>();

            // Assert
            var inMemory = Assert.IsType<MemoryDistributedCache>(resolved);
        }

        [Fact]
        public void RedisUsedWithConnectionString()
        {
            // Arrange
            var original = Substitute.For<ITestService>();
            var mock = AutoSubstitute.Configure()
                .InjectProperties()
                .MakeUnregisteredTypesPerLifetime()
                .ConfigureOptions(options =>
                {
                    options.MockHandlers.Add(SkipTypeMockHandler.Create(typeof(IValidateOptions<>)));
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton(original);
                    services
                        .AddCaching(options =>
                        {
                            options.UseCaching = true;
                            options.RedisCacheConnectionString = _fixture.Create<string>();
                        })
                        .CacheType<ITestService>(_ => { });
                })
                .SubstituteFor<IConnectionMultiplexer>()
                .Build();

            // Act
            var resolved = mock.Resolve<IDistributedCache>();

            // Assert
            var inMemory = Assert.IsType<RedisCache>(resolved);
        }

        public interface ITestService
        {
            Task<string> GetAsync();
        }
    }
}
