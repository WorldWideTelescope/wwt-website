using AutofacContrib.NSubstitute;
using AutoFixture;
using Azure.Storage.Blobs;
using NSubstitute;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace WWT.Azure.Tests
{
    public class AzureKnownPlateFileTests
    {
        private readonly Fixture _fixture;

        public AzureKnownPlateFileTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void BuildsMap()
        {
            // Arrange
            var knownPlates = _fixture.CreateMany<string>(10).ToList();
            var file = CreateFile(knownPlates);
            var options = _fixture.Create<AzurePlateTilePyramidOptions>();

            using var mock = AutoSubstitute.Configure()
                .MakeUnregisteredTypesPerLifetime()
                .Provide(options)
                .SubstituteFor<BlobServiceClient>()
                    .ResolveReturnValue(s => s.GetBlobContainerClient(options.Container))
                .SubstituteFor<BlobContainerClient>()
                    .ResolveReturnValue(c => c.GetBlobClient(options.KnownPlateFile))
                .SubstituteFor<BlobClient>()
                    .ConfigureSubstitute(b => b.OpenRead().Returns(file))
                .Build();

            var known = mock.Resolve<AzureKnownPlateFile>();

            var expected = knownPlates[3];
            var upper = expected.ToUpperInvariant();

            // Act/Assert
            Assert.True(known.TryNormalizePlateName(expected, out var test1));
            Assert.Equal(expected, test1);

            Assert.True(known.TryNormalizePlateName(upper, out var test2));
            Assert.NotEqual(upper, test2);
            Assert.Equal(expected, test2);

            Assert.False(known.TryNormalizePlateName(_fixture.Create<string>(), out _));
        }

        private Stream CreateFile(IEnumerable<string> knownPlates)
        {
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);

            foreach (var plate in knownPlates)
            {
                writer.WriteLine(plate);
            }

            writer.Flush();
            ms.Position = 0;

            return ms;
        }
    }
}
