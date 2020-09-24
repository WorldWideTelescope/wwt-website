using AutofacContrib.NSubstitute;

namespace WWT.Azure.Tests
{
    /// <summary>
    /// This can be replaced by https://github.com/MRCollective/AutofacContrib.NSubstitute/pull/41 once it's merged in and available
    /// </summary>
    internal static class AutoSubstituteExtensions
    {
        public static SubstituteForBuilder<T> Provide<T>(this SubstituteForBuilder<T> builder, out T value)
            where T : class
        {
            T result = null;
            builder.Configure(v => result = v);
            value = result;

            return builder;
        }

        public static AutoSubstituteBuilder Configured<T>(this SubstituteForBuilder<T> builder)
            where T : class
            => builder.Configure(_ => { });
    }
}
