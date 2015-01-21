using System;
using System.Reflection;

namespace MediatR.Extensions
{
    public interface IMediatorBuilder
    {
        IMediatorBuilder WithRequestDecorator(string name, Type decoratorType);
        IMediatorBuilder WithRequestHandler(Type requestHandlerType);
        IMediatorBuilder WithRequestHandlerAssemblies(params Assembly[] assemblies);

        IMediatorBuilder WithNotificationHandler(Type notificationHandlerType);
        IMediatorBuilder WithNotificationHandlerAssemblies(params Assembly[] assemblies);

        IMediator Build();
    }
}