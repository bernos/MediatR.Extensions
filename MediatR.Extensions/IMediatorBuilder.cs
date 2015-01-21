using System;
using System.Reflection;

namespace MediatR.Extensions
{
    public interface IMediatorBuilder
    {
        IMediatorBuilder WithRequestDecorator(string name, Type decoratorType);
        IMediatorBuilder WithRequestHandler(Type decoratorType);
        IMediatorBuilder WithRequestHandlerAssemblies(params Assembly[] assemblies);
        IMediator Build();
    }
}