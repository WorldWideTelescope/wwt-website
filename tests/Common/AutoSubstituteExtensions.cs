using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutofacContrib.NSubstitute;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace WWT
{
    internal static class AutoSubstituteExtensions
    {
        public static AutoSubstituteBuilder ConfigureServices(this AutoSubstituteBuilder builder, Action<IServiceCollection> configure)
            => builder.ConfigureBuilder(b =>
            {
                var services = new ServiceCollection();

                configure(services);

                b.Populate(services);
            });

        public static SubstituteForBuilder<T> ResolveReturnValue<T, TResult>(this SubstituteForBuilder<T> builder, Func<T, TResult> action)
            where T : class
            => builder.ConfigureSubstitute((t, ctx) =>
            {
                var s = ctx.Resolve<TResult>();
                action(t.Configure()).Returns(s);
            });

        public static byte[] ToArray(this Stream stream)
        {
            if (stream is MemoryStream msInput)
            {
                return msInput.ToArray();
            }

            using var ms = new MemoryStream();

            stream.CopyTo(ms);

            return ms.ToArray();
        }
    }
}