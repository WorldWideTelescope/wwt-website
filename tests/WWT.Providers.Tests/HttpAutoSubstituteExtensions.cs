using Autofac;
using AutofacContrib.NSubstitute;
using NSubstitute;
using NSubstitute.Core;
using System;
using System.IO;
using System.Threading.Tasks;

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

        public static AutoSubstituteBuilder InitializeProviderTests(this AutoSubstituteBuilder builder)
            => builder
                // TODO: Add this back once https://github.com/MRCollective/AutofacContrib.NSubstitute/pull/51 is available
                //.InjectProperties()
                .ConfigureOptions(options =>
                {
                    options.MockHandlers.Add(FixedAutoPropertyInjectorMockHandler.Instance);
                })
                .MakeUnregisteredTypesPerLifetime()
                .RegisterAfterBuild<IResponse>((r, _) => r.OutputStream.Returns(new MemoryStream()));

        // TODO: Add this back once https://github.com/MRCollective/AutofacContrib.NSubstitute/pull/51 is available
        private class FixedAutoPropertyInjectorMockHandler : MockHandler
        {
            public static FixedAutoPropertyInjectorMockHandler Instance { get; } = new FixedAutoPropertyInjectorMockHandler();

            private FixedAutoPropertyInjectorMockHandler()
            {
            }

            protected override void OnMockCreated(object instance, Type type, IComponentContext context, ISubstitutionContext substitutionContext)
            {
                var router = substitutionContext.GetCallRouterFor(instance);

                router.RegisterCustomCallHandlerFactory(_ => new AutoPropertyInjectorCallHandler(context));
            }

            private class AutoPropertyInjectorCallHandler : ICallHandler
            {
                private readonly IComponentContext _context;

                public AutoPropertyInjectorCallHandler(IComponentContext context)
                {
                    _context = context;
                }

                public RouteAction Handle(ICall call)
                {
                    var property = call.GetMethodInfo().GetPropertyFromGetterCallOrNull();

                    if (property is null)
                    {
                        return RouteAction.Continue();
                    }

                    var service = _context.ResolveOptional(call.GetReturnType());

                    if (service is null)
                    {
                        return RouteAction.Continue();
                    }

                    return RouteAction.Return(service);
                }
            }
        }

        public static AutoSubstituteBuilder ConfigureParameterQ(this AutoSubstituteBuilder builder, int level, int x, int y)
            => builder.RegisterAfterBuild<IParameters>((p, ctx) => p["Q"].Returns($"{level},{x},{y}"));

        public static AutoSubstituteBuilder ConfigureParameterQ(this AutoSubstituteBuilder builder, object[] args)
        {
            var str = string.Join(",", args);
            return builder.RegisterAfterBuild<IParameters>((p, ctx) => p["Q"].Returns(str));
        }
    }
}
