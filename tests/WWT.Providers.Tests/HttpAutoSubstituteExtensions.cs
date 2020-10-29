using Autofac;
using AutofacContrib.NSubstitute;
using NSubstitute;
using NSubstitute.Core;
using System;
using System.IO;
using System.Threading.Tasks;
using WWT.PlateFiles;
using WWTWebservices;

namespace WWT.Providers.Tests
{
    internal static class HttpAutoSubstituteExtensions
    {
        public static byte[] GetOutputData(this AutoSubstitute container)
            => container.Resolve<IResponse>().OutputStream.ToArray();

        public static Task RunProviderTestAsync<T>(this AutoSubstitute container)
            where T : RequestProvider
        {
            return container.Resolve<T>().RunAsync(container.Resolve<IWwtContext>(), default);
        }

        public static AutoSubstituteBuilder RegisterAfterBuild<T>(this AutoSubstituteBuilder builder, Action<T, IComponentContext> action)
            => builder.ConfigureBuilder(b => b.RegisterBuildCallback(s => action(s.Resolve<T>(), s)));

        public static AutoSubstituteBuilder InitializeProviderTests(this AutoSubstituteBuilder builder, bool initializeKnownPlateFile = true)
        {
            builder
                .InjectProperties()
                .MakeUnregisteredTypesPerLifetime();

            if (initializeKnownPlateFile)
            {
                builder.ConfigureKnownPlateFile(Arg.Any<string>(), true);
            }

            builder.RegisterAfterBuild<IResponse>((r, _) => r.OutputStream.Returns(new MemoryStream()));

            return builder;
        }

        public static AutoSubstituteBuilder ConfigureKnownPlateFile(this AutoSubstituteBuilder builder, string plateFile, bool result)
            => builder.RegisterAfterBuild<IKnownPlateFiles>((k, _) => k.TryNormalizePlateName(plateFile, out Arg.Any<string>()).Returns(x =>
            {
                x[1] = x[0];
                return result;
            }));

        public static AutoSubstituteBuilder ConfigureParameterQ(this AutoSubstituteBuilder builder, params object[] args)
        {
            var str = string.Join(",", args);
            return builder.RegisterAfterBuild<IParameters>((p, ctx) => p["Q"].Returns(str));
        }
    }
}
