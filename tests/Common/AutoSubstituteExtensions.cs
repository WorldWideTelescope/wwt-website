using Autofac;
using NSubstitute;
using NSubstitute.Extensions;
using System;

namespace AutofacContrib.NSubstitute
{
    internal static class AutoSubstituteExtensions
    {
        public static SubstituteForBuilder<T> ResolveReturnValue<T, TResult>(this SubstituteForBuilder<T> builder, Func<T, TResult> action)
            where T : class
            => builder.ConfigureSubstitute((t, ctx) =>
            {
                var s = ctx.Resolve<TResult>();
                action(t.Configure()).Returns(s);
            });
    }
}