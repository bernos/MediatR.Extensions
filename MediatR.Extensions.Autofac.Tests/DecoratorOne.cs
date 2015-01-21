using System.Threading.Tasks;

namespace MediatR.Extensions.Autofac.Tests
{
    public class DecoratorOne<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequestWithMessage<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _innerHander;

        public DecoratorOne(IRequestHandler<TRequest, TResponse> innerHandler)
        {
            _innerHander = innerHandler;
        }

        public TResponse Handle(TRequest message)
        {
            message.Message += "DecoratorOne";
            return _innerHander.Handle(message);
        }
    }

    public class AsyncDecoratorOne<TRequest, TResponse> : IAsyncRequestHandler<TRequest, TResponse> where TRequest : IRequestWithMessage<TResponse>
    {
        private readonly IAsyncRequestHandler<TRequest, TResponse> _innerHander;

        public AsyncDecoratorOne(IAsyncRequestHandler<TRequest, TResponse> innerHandler)
        {
            _innerHander = innerHandler;
        }

        public async Task<TResponse> Handle(TRequest message)
        {
            message.Message += "DecoratorOne";
            return await _innerHander.Handle(message);
        }
    }
}