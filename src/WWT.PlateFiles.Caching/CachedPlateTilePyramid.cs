using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using WWTWebservices;

namespace WWT.PlateFiles.Caching
{
    public abstract class CachedPlateTilePyramid : IPlateTilePyramid
    {
        public CachedPlateTilePyramid(IPlateTilePyramid other)
        {
            InnerPyramid = other;
        }

        public IPlateTilePyramid InnerPyramid { get; }

        public IAsyncEnumerable<string> GetPlateNames(CancellationToken token)
            => InnerPyramid.GetPlateNames(token);

        public Stream GetStream(string pathPrefix, string plateName, int level, int x, int y)
        {
            var context = new TileContext(InnerPyramid)
            {
                PathPrefix = pathPrefix,
                PlateName = plateName,
                Level = level,
                X = x,
                Y = y
            };

            return new MemoryStream(GetOrUpdateCache(context));
        }

        public Stream GetStream(string pathPrefix, string plateName, int tag, int level, int x, int y)
        {
            var context = new TileContext(InnerPyramid)
            {
                PathPrefix = pathPrefix,
                PlateName = plateName,
                Tag = tag,
                Level = level,
                X = x,
                Y = y
            };

            return new MemoryStream(GetOrUpdateCache(context));
        }

        protected abstract byte[] GetOrUpdateCache(TileContext context);

        public class TileContext
        {
            private readonly IPlateTilePyramid _other;

            public TileContext(IPlateTilePyramid other)
            {
                _other = other;
            }

            public string GetKey()
            {
                var sb = new StringBuilder(PathPrefix.Length + PlateName.Length + 20);

                sb.Append(PathPrefix);
                sb.Append("_");
                sb.Append(PlateName);
                sb.Append("_");

                if (Tag.HasValue)
                {
                    sb.Append(Tag.Value);
                    sb.Append("_");
                }

                sb.Append("L");
                sb.Append(Level.ToString());

                sb.Append("X");
                sb.Append(X.ToString());

                sb.Append("Y");
                sb.Append(Y.ToString());

                return sb.ToString();
            }

            public string PathPrefix { get; set; }

            public string PlateName { get; set; }

            public int? Tag { get; set; }

            public int Level { get; set; }

            public int X { get; set; }

            public int Y { get; set; }

            public byte[] GetResult()
            {
                var result = Tag.HasValue
                    ? _other.GetStream(PathPrefix, PlateName, Tag.Value, Level, X, Y)
                    : _other.GetStream(PathPrefix, PlateName, Level, X, Y);

                using (var ms = new MemoryStream())
                {
                    result.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}
