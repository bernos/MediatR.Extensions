using System.Threading.Tasks;

namespace MediatR.Extensions.Autofac.Tests
{
    public class PingHandler : IRequestHandler<Ping, Pong>
    {
        public Pong Handle(Ping message)
        {
            return new Pong
            {
                Message = string.Format("{0}Handled", message.Message)
            };
        }
    }

    public class AsyncPingHandler : IAsyncRequestHandler<Ping, Pong>
    {
        public async Task<Pong> Handle(Ping message)
        {
            return await Task.Run(() => new Pong
            {
                Message = string.Format("{0}HandledAsync", message.Message)
            });
        }
    }

    public class Ping : IRequestWithMessage<Pong>
    {
        public string Message { get; set; }
    }

    public class Pong
    {
        public string Message { get; set; }
    }
}