using System.Threading.Tasks;

namespace MediatR.Extensions.Autofac.Tests
{
    public class DecoratorTwo<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequestWithMessage<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _innerHander;

        public DecoratorTwo(IRequestHandler<TRequest, TResponse> innerHandler)
        {
            _innerHander = innerHandler;
        }

        public TResponse Handle(TRequest message)
        {
            message.Message += "DecoratorTwo";
            return _innerHander.Handle(message);
        }
    }

    public class AsyncDecoratorTwo<TRequest, TResponse> : IAsyncRequestHandler<TRequest, TResponse> where TRequest : IRequestWithMessage<TResponse>
    {
        private readonly IAsyncRequestHandler<TRequest, TResponse> _innerHander;

        public AsyncDecoratorTwo(IAsyncRequestHandler<TRequest, TResponse> innerHandler)
        {
            _innerHander = innerHandler;
        }

        public async Task<TResponse> Handle(TRequest message)
        {
            message.Message += "DecoratorTwo";
            return await _innerHander.Handle(message);
        }
    }
}