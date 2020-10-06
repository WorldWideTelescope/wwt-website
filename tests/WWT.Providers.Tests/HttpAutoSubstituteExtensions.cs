using Autofac;
using AutofacContrib.NSubstitute;
using NSubstitute;
using NSubstitute.Extensions;
using System;
using System.Collections.Specialized;
using System.Web;

namespace WWT.Providers.Tests
{
    internal static class HttpAutoSubstituteExtensions
    {
        public static void RunProviderTest<T>(this AutoSubstitute container)
            where T : RequestProvider
        {
            container.Resolve<T>().Run(container.Resolve<IWwtContext>());
        }

        public static AutoSubstituteBuilder InitializeHttpWrappers(this AutoSubstituteBuilder builder)
        {
            builder.SubstituteFor2<HttpResponseBase>()
                .Configure((b, ctx) =>
                {
                    b.When(bb => bb.Write(Arg.Any<string>())).DoNotCallBase();
                    b.When(bb => bb.Close()).DoNotCallBase();

                    ctx.Resolve<IWwtContext>().Response.Returns(b);
                });

            builder.SubstituteFor2<HttpRequestBase>()
                .Configure((b, ctx) =>
                {
                    ctx.Resolve<IWwtContext>().Request.Returns(b);
                });

            return builder;
        }

        public static AutoSubstituteBuilder ConfigureParameterQ(this AutoSubstituteBuilder builder, int level, int x, int y)
            => builder.ConfigureParameters(c => c.Add("Q", $"{level},{x},{y}"));

        public static AutoSubstituteBuilder ConfigureParameters(this AutoSubstituteBuilder builder, Action<NameValueCollection> action)
        {
            var collection = new NameValueCollection();

            action(collection);

            return builder.SubstituteFor2<HttpRequestBase>()
                .Configure(b => b.Configure().Params.Returns(collection));
        }
    }
}
