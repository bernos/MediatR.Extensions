using log4net;

namespace MediatR.Extensions.log4net
{
    public class LoggingRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _innerHander;
        private readonly ILog _log;

        public LoggingRequestHandler(IRequestHandler<TRequest, TResponse> innerHandler)
        {
            _innerHander = innerHandler;
            _log = LogManager.GetLogger(innerHandler.GetType());
        }

        public TResponse Handle(TRequest message)
        {
            _log.Info(string.Format("Request: {0}", message));
            var response = _innerHander.Handle(message);
            _log.Info(string.Format("Response: {0}", response));

            return response;
        }
    }
}