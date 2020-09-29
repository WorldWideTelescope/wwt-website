using Autofac;
using NSubstitute;
using System;

namespace AutofacContrib.NSubstitute
{
    /// <summary>
    /// This can be replaced by https://github.com/MRCollective/AutofacContrib.NSubstitute/pull/41 once it's merged in and available
    /// </summary>
    internal static class AutoSubstituteExtensions
    {
        public static SubstituteForBuilder2<T> SubstituteFor2<T>(this AutoSubstituteBuilder builder)
            where T : class
        {
            builder.ConfigureBuilder(b =>
            {
                b.Register(_ => Substitute.For<T>())
                    .As<T>()
                    .InstancePerLifetimeScope();
            });

            return new SubstituteForBuilder2<T>(builder);
        }

        public class SubstituteForBuilder2<TService>
            where TService : class
        {
            private readonly AutoSubstituteBuilder _builder;

            internal SubstituteForBuilder2(AutoSubstituteBuilder builder)
            {
                _builder = builder;
            }

            /// <summary>
            /// Allows for configuration of the service.
            /// </summary>
            /// <param name="action">The delegate to configure the service.</param>
            /// <returns>The original <see cref="AutoSubstituteBuilder"/>.</returns>
            public AutoSubstituteBuilder Configure(Action<TService> action)
                => Configure((s, _) => action(s));

            /// <summary>
            /// Allows for configuration of the service with access to the resolved components.
            /// </summary>
            /// <param name="action">The delegate to configure the service.</param>
            /// <returns>The original <see cref="AutoSubstituteBuilder"/>.</returns>
            public AutoSubstituteBuilder Configure(Action<TService, IComponentContext> action)
                => _builder.ConfigureBuilder(builder =>
                {
                    builder.RegisterBuildCallback(context =>
                    {
                        action(context.Resolve<TService>(), context);
                    });
                });

            /// <summary>
            /// Completes the configuration of the substitute.
            /// </summary>
            /// <returns>The original <see cref="AutoSubstituteBuilder"/>.</returns>
            public AutoSubstituteBuilder Configured()
                => _builder;

            /// <summary>
            /// Allows a way to access the services being configured that the container will provide.
            /// </summary>
            /// <param name="service">Parameter to obtain the substituted value.</param>
            /// <returns></returns>
            public SubstituteForBuilder2<TService> Provide(out IProvidedValue<TService> service)
            {
                var provided = new ProvidedValue<TService>();

                _builder.ConfigureBuilder(b =>
                {
                    b.RegisterBuildCallback(scope =>
                    {
                        provided.Value = scope.Resolve<TService>();
                    });
                });

                service = provided;
                return this;
            }

            private class ProvidedValue<T> : IProvidedValue<T>
            {
                public T Value { get; set; }
            }
        }
    }
}