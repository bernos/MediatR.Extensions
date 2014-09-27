using System.Threading.Tasks;
using log4net;

namespace MediatR.Extensions.log4net
{
    public class AsyncLoggingRequestHandler<TRequest, TResponse> : IAsyncRequestHandler<TRequest, TResponse> where TRequest : IAsyncRequest<TResponse>
    {
        private readonly IAsyncRequestHandler<TRequest, TResponse> _innerHander;
        private readonly ILog _log;

        public AsyncLoggingRequestHandler(IAsyncRequestHandler<TRequest, TResponse> innerHandler)
        {
            _innerHander = innerHandler;
            _log = LogManager.GetLogger(innerHandler.GetType());
        }

        public async Task<TResponse> Handle(TRequest message)
        {
            _log.Info(string.Format("Request: {0}", message));
            var response = await _innerHander.Handle(message);
            _log.Info(string.Format("Response: {0}", response));

            return response;
        }
    }
}