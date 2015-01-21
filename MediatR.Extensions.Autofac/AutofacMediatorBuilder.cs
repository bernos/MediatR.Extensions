using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Extras.CommonServiceLocator;
using Autofac.Features.Variance;
using Microsoft.Practices.ServiceLocation;

namespace MediatR.Extensions.Autofac
{
    public class AutofacMediatorBuilder : MediatorBuilder
    {
        private const string HandlerKey = "handler";
        private const string AsyncHandlerKey = "async-handler";
        private readonly ILifetimeScope _container;
        private readonly ContainerBuilder _builder;
        private string _key;
        private string _asyncKey;

        public AutofacMediatorBuilder(ILifetimeScope container)
        {
            _key = HandlerKey;
            _asyncKey = AsyncHandlerKey;
            _container = container;
            _builder = new ContainerBuilder();
        }

        protected override void RegisterRequestDecorator(string name, Type decoratorType)
        {
            _builder.RegisterGenericDecorator(decoratorType, typeof(IRequestHandler<,>),
                    fromKey: _key).Named(name, typeof(IRequestHandler<,>));

            _key = name;
        }

        protected override void RegisterAsyncRequestDecorator(string name, Type decoratorType)
        {
            _builder.RegisterGenericDecorator(decoratorType, typeof(IAsyncRequestHandler<,>),
                fromKey: _asyncKey).Named(name, typeof(IAsyncRequestHandler<,>));

            _asyncKey = name;
        }

        protected override void RegisterRequestHandlersFromAssembly(Assembly assembly)
        {
            _builder.RegisterAssemblyTypes(assembly).As(t => t.GetInterfaces()
                .Where(i => i.IsClosedTypeOf(typeof(IRequestHandler<,>)))
                .Select(i => new KeyedService(HandlerKey, i)));
        }

        protected override void RegisterRequestHandler(Type type)
        {
            var services = type.GetInterfaces()
                .Where(i => i.IsClosedTypeOf(typeof (IRequestHandler<,>)))
                .Select(i => new KeyedService(HandlerKey, i) as Service);

            _builder.RegisterType(type).As(services.ToArray());
        }

        protected override void RegisterAsyncRequestHandler(Type type)
        {
            var services = type.GetInterfaces()
                .Where(i => i.IsClosedTypeOf(typeof(IAsyncRequestHandler<,>)))
                .Select(i => new KeyedService(AsyncHandlerKey, i) as Service);

            _builder.RegisterType(type).As(services.ToArray());
        }

        protected override void RegisterAsyncRequestHandlersFromAssembly(Assembly assembly)
        {
            _builder.RegisterAssemblyTypes(assembly).As(t => t.GetInterfaces()
                .Where(i => i.IsClosedTypeOf(typeof(IAsyncRequestHandler<,>)))
                .Select(i => new KeyedService(AsyncHandlerKey, i)));
        }

        protected override IMediator BuildMediator()
        {
            _builder.RegisterSource(new ContravariantRegistrationSource());
            _builder.RegisterAssemblyTypes(typeof(IMediator).Assembly).AsImplementedInterfaces();

            var lazy = new Lazy<IServiceLocator>(() => new AutofacServiceLocator(_container));
            var serviceLocatorProvider = new ServiceLocatorProvider(() => lazy.Value);

            _builder.RegisterInstance(serviceLocatorProvider);

            _builder.RegisterGenericDecorator(typeof(WrapperRequestHandler<,>), typeof(IRequestHandler<,>),
                fromKey: _key);

            _builder.RegisterGenericDecorator(typeof(AsyncWrapperRequestHandler<,>), typeof(IAsyncRequestHandler<,>),
                fromKey: _asyncKey);

            _builder.Update(_container.ComponentRegistry);

            return serviceLocatorProvider().GetInstance<IMediator>();
        }
    }
}