using System;
using System.Linq;
using System.Reflection;

namespace MediatR.Extensions
{
    public abstract class MediatorBuilder : IMediatorBuilder
    {
        private bool _isBuilt;

        public IMediatorBuilder WithRequestDecorator(string name, Type decoratorType)
        {
            if (_isBuilt)
            {
                throw new Exception("Cannot call AddRequestDecorator after Build() has been called");
            }

            if (decoratorType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncRequestHandler<,>)))
            {
                RegisterAsyncRequestDecorator(name, decoratorType);
            }
            else if (
                decoratorType.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            {
                RegisterRequestDecorator(name, decoratorType);
            }
            else
            {
                throw new ArgumentException("Decorator type must implement IRequestHandler<TRequest,TResponse> or IAsyncRequestHandler<TRequest, TResponse>", "decoratorType");
            }

            return this;
        }

        public IMediatorBuilder WithRequestHandlerAssemblies(params Assembly[] assemblies)
        {
            if (_isBuilt)
            {
                throw new Exception("Cannot call AddRequestDecorator after Build() has been called");
            }

            foreach (var assembly in assemblies)
            {
                RegisterRequestHandlersFromAssembly(assembly);
                RegisterAsyncRequestHandlersFromAssembly(assembly);
            }

            return this;
        }

        public IMediator Build()
        {
            if (_isBuilt)
            {
                throw new Exception("Build() can only be called once");
            }

            var mediator = BuildMediator();

            _isBuilt = true;

            return mediator;
        }

        protected abstract void RegisterRequestDecorator(string name, Type decoratorType);
        protected abstract void RegisterAsyncRequestDecorator(string name, Type decoratorType);
        protected abstract void RegisterRequestHandlersFromAssembly(Assembly assembly);
        protected abstract void RegisterAsyncRequestHandlersFromAssembly(Assembly assembly);
        protected abstract IMediator BuildMediator();
    }
}