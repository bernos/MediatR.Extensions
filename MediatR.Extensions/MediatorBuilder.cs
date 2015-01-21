using System;
using System.Linq;
using System.Reflection;

namespace MediatR.Extensions
{
    public abstract class MediatorBuilder : IMediatorBuilder
    {
        private bool _isBuilt;
        private static Func<Type, bool> CreateGenericTypePredicate(Type type)
        {
            return i => i.IsGenericType && i.GetGenericTypeDefinition() == type;
        }

        public IMediatorBuilder WithRequestDecorator(string name, Type decoratorType)
        {
            if (_isBuilt)
            {
                throw new Exception("Cannot call AddRequestDecorator after Build() has been called");
            }

            var interfaces = decoratorType.GetInterfaces();

            if (interfaces.Any(CreateGenericTypePredicate(typeof(IAsyncRequestHandler<,>))))
            {
                RegisterAsyncRequestDecorator(name, decoratorType);
            }
            else if (interfaces.Any(CreateGenericTypePredicate(typeof(IRequestHandler<,>))))
            {
                RegisterRequestDecorator(name, decoratorType);
            }
            else
            {
                throw new ArgumentException("Decorator type must implement IRequestHandler<TRequest,TResponse> or IAsyncRequestHandler<TRequest, TResponse>", "decoratorType");
            }

            return this;
        }
        
        public IMediatorBuilder WithRequestHandler(Type handlerType)
        {
            if (_isBuilt)
            {
                throw new Exception("Cannot call WithRequestHandler after Build() has been called");
            }
            
            var interfaces = handlerType.GetInterfaces();

            if (interfaces.Any(CreateGenericTypePredicate(typeof(IAsyncRequestHandler<,>))))
            {
                RegisterAsyncRequestHandler(handlerType);
            }
            else if (interfaces.Any(CreateGenericTypePredicate(typeof (IRequestHandler<,>))))
            {
                RegisterRequestHandler(handlerType);
            }
            else
            {
                throw new ArgumentException("Handler type must implement IRe IRequestHandler<TRequest,TResponse> or IAsyncRequestHandler<TRequest, TResponse>", "handlerType");
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

        protected abstract void RegisterRequestHandler(Type handlerType);
        protected abstract void RegisterAsyncRequestHandler(Type handlerType);
        protected abstract void RegisterRequestDecorator(string name, Type decoratorType);
        protected abstract void RegisterAsyncRequestDecorator(string name, Type decoratorType);
        protected abstract void RegisterRequestHandlersFromAssembly(Assembly assembly);
        protected abstract void RegisterAsyncRequestHandlersFromAssembly(Assembly assembly);
        
        protected abstract IMediator BuildMediator();
    }
}