using Autofac;
using AutofacContrib.NSubstitute;
using NSubstitute;
using NSubstitute.Core;
using System;
using System.IO;

namespace WWT.Providers.Tests
{
    internal static class HttpAutoSubstituteExtensions
    {
        public static byte[] GetOutputData(this AutoSubstitute container)
            => container.Resolve<IResponse>().OutputStream.ToArray();

        public static void RunProviderTest<T>(this AutoSubstitute container)
            where T : RequestProvider
        {
            container.Resolve<T>().Run(container.Resolve<IWwtContext>());
        }

        public static AutoSubstituteBuilder RegisterAfterBuild<T>(this AutoSubstituteBuilder builder, Action<T, IComponentContext> action)
            => builder.ConfigureBuilder(b => b.RegisterBuildCallback(s => action(s.Resolve<T>(), s)));

        public static AutoSubstituteBuilder InitializeProviderTests(this AutoSubstituteBuilder builder)
            => builder
                .InjectProperties()
                .MakeUnregisteredTypesPerLifetime()
                .RegisterAfterBuild<IResponse>((r, _) => r.OutputStream.Returns(new MemoryStream()));

        public static AutoSubstituteBuilder ConfigureParameterQ(this AutoSubstituteBuilder builder, int level, int x, int y)
            => builder.RegisterAfterBuild<IParameters>((p, ctx) => p["Q"].Returns($"{level},{x},{y}"));

        public static AutoSubstituteBuilder ConfigureParameterQ(this AutoSubstituteBuilder builder, object[] args)
        {
            var str = string.Join(",", args);
            return builder.RegisterAfterBuild<IParameters>((p, ctx) => p["Q"].Returns(str));
        }
    }
}
