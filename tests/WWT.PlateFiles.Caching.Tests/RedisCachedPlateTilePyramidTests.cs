using AutofacContrib.NSubstitute;
using AutoFixture;
using NSubstitute;
using StackExchange.Redis;
using System;
using System.IO;
using System.Linq;
using WWTWebservices;
using Xunit;

namespace WWT.PlateFiles.Caching.Tests
{
    public class RedisCachedPlateTilePyramidTests
    {
        private readonly Fixture _fixture;

        public RedisCachedPlateTilePyramidTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void CacheSanityPlate1()
        {
            // Arrange
            using var mock = AutoSubstitute.Configure()
                .MakeUnregisteredTypesPerLifetime()
                .Build();

            mock.Resolve<IConnectionMultiplexer>().GetDatabase().Returns(mock.Resolve<IDatabase>());

            var level = _fixture.Create<int>();
            var x = _fixture.Create<int>();
            var y = _fixture.Create<int>();
            var prefix = _fixture.Create<string>();
            var name = _fixture.Create<string>();
            var expected = _fixture.CreateMany<byte>(10).ToArray();

            mock.Resolve<IPlateTilePyramid>().GetStream(prefix, name, level, x, y).Returns(new MemoryStream(expected));

            var cached = mock.Resolve<RedisCachedPlateTilePyramid>();
            var context = new CachedPlateTilePyramid.TileContext(null)
            {
                Level = level,
                X = x,
                Y = y,
                PathPrefix = prefix,
                PlateName = name,
            };

            // Act
            using var result1 = cached.GetStream(prefix, name, level, x, y);

            // Assert
            var ms1 = Assert.IsType<MemoryStream>(result1);
            Assert.Equal(expected, ms1.ToArray());

            var key = context.GetKey();

            mock.Resolve<IPlateTilePyramid>().Received(1).GetStream(prefix, name, level, x, y);
            var d = mock.Resolve<IDatabase>();
            d.Received(1).StringSet(key, result1.ToArray(), Arg.Any<TimeSpan>(), When.Always, CommandFlags.FireAndForget);
        }

        [Fact]
        public void ReturnsCachedValue()
        {
            // Arrange
            using var mock = AutoSubstitute.Configure()
                .MakeUnregisteredTypesPerLifetime()
                .Build();

            var level = _fixture.Create<int>();
            var x = _fixture.Create<int>();
            var y = _fixture.Create<int>();
            var prefix = _fixture.Create<string>();
            var name = _fixture.Create<string>();
            var expected = _fixture.CreateMany<byte>(10).ToArray();

            var cached = mock.Resolve<RedisCachedPlateTilePyramid>();
            var context = new CachedPlateTilePyramid.TileContext(null)
            {
                Level = level,
                X = x,
                Y = y,
                PathPrefix = prefix,
                PlateName = name,
            };

            var db = mock.Resolve<IDatabase>();
            mock.Resolve<IConnectionMultiplexer>().GetDatabase().Returns(db);
            var redisValue = RedisValue.CreateFrom(new MemoryStream(expected));
            db.StringGet(context.GetKey()).Returns(redisValue);

            // Act
            using var result1 = cached.GetStream(prefix, name, level, x, y);

            // Assert
            var ms1 = Assert.IsType<MemoryStream>(result1);
            Assert.Equal(expected, ms1.ToArray());

            mock.Resolve<IPlateTilePyramid>().Received(0).GetStream(prefix, name, level, x, y);
            db.DidNotReceiveWithAnyArgs().StringSet(Arg.Any<RedisKey>(), Arg.Any<RedisValue>(), Arg.Any<TimeSpan>(), When.Always, CommandFlags.FireAndForget);
        }

        [Fact]
        public void CacheSanityPlate2()
        {
            // Arrange
            using var mock = AutoSubstitute.Configure()
                .MakeUnregisteredTypesPerLifetime()
                .Build();

            mock.Resolve<IConnectionMultiplexer>().GetDatabase().Returns(mock.Resolve<IDatabase>());

            var level = _fixture.Create<int>();
            var tag = _fixture.Create<int>();
            var x = _fixture.Create<int>();
            var y = _fixture.Create<int>();
            var prefix = _fixture.Create<string>();
            var name = _fixture.Create<string>();
            var expected = _fixture.CreateMany<byte>(10).ToArray();

            mock.Resolve<IPlateTilePyramid>().GetStream(prefix, name, tag, level, x, y).Returns(new MemoryStream(expected));

            var cached = mock.Resolve<RedisCachedPlateTilePyramid>();
            var context = new CachedPlateTilePyramid.TileContext(null)
            {
                Level = level,
                Tag = tag,
                X = x,
                Y = y,
                PathPrefix = prefix,
                PlateName = name,
            };

            // Act
            using var result1 = cached.GetStream(prefix, name, tag, level, x, y);

            // Assert
            var ms1 = Assert.IsType<MemoryStream>(result1);
            Assert.Equal(expected, ms1.ToArray());

            var key = context.GetKey();

            mock.Resolve<IPlateTilePyramid>().Received(1).GetStream(prefix, name, tag, level, x, y);
            var d = mock.Resolve<IDatabase>();
            d.Received(1).StringSet(key, result1.ToArray(), Arg.Any<TimeSpan>(), When.Always, CommandFlags.FireAndForget);
        }
    }
}
