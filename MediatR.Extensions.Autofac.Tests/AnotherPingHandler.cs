using System.Threading.Tasks;

namespace MediatR.Extensions.Autofac.Tests
{
    public class AnotherPingHandler : IRequestHandler<AnotherPing, AnotherPong>
    {
        public AnotherPong Handle(AnotherPing message)
        {
            return new AnotherPong
            {
                Message = string.Format("{0}Handled", message.Message)
            };
        }
    }

    public class AsyncAnotherPingHandler : IAsyncRequestHandler<AnotherPing, AnotherPong>
    {
        public async Task<AnotherPong> Handle(AnotherPing message)
        {
            return await Task.Run(() => new AnotherPong
            {
                 Message = string.Format("{0}HandledAsync", message.Message)
            });
        }
    }

    public class AnotherPing : IRequestWithMessage<AnotherPong>
    {
        public string Message { get; set; }
    }

    public class AnotherPong
    {
        public string Message { get; set; }
    }
}