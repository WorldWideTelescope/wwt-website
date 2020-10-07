using Autofac;
using AutofacContrib.NSubstitute;
using NSubstitute;
using NSubstitute.Extensions;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;

namespace WWT.Providers.Tests
{
    internal static class HttpAutoSubstituteExtensions
    {
        public static byte[] GetOutputData(this AutoSubstitute container)
        {
            using var ms = new MemoryStream();

            var stream = container.Resolve<HttpResponseBase>().OutputStream;

            stream.Position = 0;
            stream.CopyTo(ms);

            return ms.ToArray();
        }

        public static void RunProviderTest<T>(this AutoSubstitute container)
            where T : RequestProvider
        {
            container.Resolve<T>().Run(container.Resolve<IWwtContext>());
        }

        public static AutoSubstituteBuilder InitializeProviderTests(this AutoSubstituteBuilder builder)
            => builder
                .InjectProperties()
                .MakeUnregisteredTypesPerLifetime()
                .SubstituteFor<HttpResponseBase>()
                    .ConfigureSubstitute(b =>
                    {
                        b.Configure().OutputStream.Returns(new MemoryStream());
                        b.Configure().ContentType.Returns(string.Empty);
                    })
                .SubstituteFor<HttpRequestBase>();

        public static AutoSubstituteBuilder ConfigureParameterQ(this AutoSubstituteBuilder builder, int level, int x, int y)
            => builder.ConfigureParameters(c => c.Add("Q", $"{level},{x},{y}"));

        public static AutoSubstituteBuilder ConfigureParameters(this AutoSubstituteBuilder builder, Action<NameValueCollection> action)
            => builder.ConfigureBuilder(b =>
            {
                b.RegisterBuildCallback(ctx =>
                {
                    action(ctx.Resolve<NameValueCollection>());
                });
            });
    }
}
