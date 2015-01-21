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
        
        public IMediatorBuilder WithRequestHandler(Type requestHandlerType)
        {
            if (_isBuilt)
            {
                throw new Exception("Cannot call WithRequestHandler after Build() has been called");
            }
            
            var interfaces = requestHandlerType.GetInterfaces();

            if (interfaces.Any(CreateGenericTypePredicate(typeof(IAsyncRequestHandler<,>))))
            {
                RegisterAsyncRequestHandler(requestHandlerType);
            }
            else if (interfaces.Any(CreateGenericTypePredicate(typeof (IRequestHandler<,>))))
            {
                RegisterRequestHandler(requestHandlerType);
            }
            else
            {
                throw new ArgumentException("Handler type must implement IRe IRequestHandler<TRequest,TResponse> or IAsyncRequestHandler<TRequest, TResponse>", "requestHandlerType");
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

        public IMediatorBuilder WithNotificationHandler(Type notificationHandlerType)
        {
            if (_isBuilt)
            {
                throw new Exception("Cannot call WithNotificationHandler after Build() has been called");
            }

            var interfaces = notificationHandlerType.GetInterfaces();

            if (interfaces.Any(CreateGenericTypePredicate(typeof(IAsyncNotificationHandler<>))))
            {
                RegisterAsyncNotificationHandler(notificationHandlerType);
            }
            else if (interfaces.Any(CreateGenericTypePredicate(typeof(INotificationHandler<>))))
            {
                RegisterNotificationHandler(notificationHandlerType);
            }
            else
            {
                throw new ArgumentException("Handler type must implement IRe IRequestHandler<TRequest,TResponse> or IAsyncRequestHandler<TRequest, TResponse>", "requestHandlerType");
            }

            return this;
        }

        public IMediatorBuilder WithNotificationHandlerAssemblies(params Assembly[] assemblies)
        {
            if (_isBuilt)
            {
                throw new Exception("Cannot call WithNotificationHandlerAssemblies after Build() has been called");
            }

            foreach (var assembly in assemblies)
            {
                RegisterNotificationHandlersFromAssembly(assembly);
                RegisterAsyncNotificationHandlersFromAssembly(assembly);
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


        protected abstract void RegisterNotificationHandler(Type notificationHandlerType);
        protected abstract void RegisterAsyncNotificationHandler(Type notificationHandlerType);
        protected abstract void RegisterNotificationHandlersFromAssembly(Assembly assembly);
        protected abstract void RegisterAsyncNotificationHandlersFromAssembly(Assembly assembly);


        protected abstract IMediator BuildMediator();
    }
}