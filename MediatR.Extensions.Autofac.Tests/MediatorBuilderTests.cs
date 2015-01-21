using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Xunit;
using Autofac;

namespace MediatR.Extensions.Autofac.Tests
{
    public class MediatorBuilderTests
    {
        private readonly ILifetimeScope container;

        public MediatorBuilderTests()
        {
            container = new ContainerBuilder().Build();
        }

        [Fact]
        public void Should_Register_Handler()
        {
            var mediator = new AutofacMediatorBuilder(container)
                .WithRequestHandler(typeof (PingHandler))
                .Build();

            var pong = mediator.Send(new Ping());
            
            Assert.Equal("Handled", pong.Message);
        }

        [Fact]
        public async Task Should_Register_Async_Handler()
        {
            var mediator = new AutofacMediatorBuilder(container)
                .WithRequestHandler(typeof(AsyncPingHandler))
                .Build();

            var pong = await mediator.SendAsync(new Ping());

            Assert.Equal("HandledAsync", pong.Message);
        }

        [Fact]
        public async Task Should_Register_All_Handlers_From_Assembly()
        {
            var mediator = new AutofacMediatorBuilder(container)
                .WithRequestHandlerAssemblies(typeof (MediatorBuilderTests).Assembly)
                .Build();

            var result1 = mediator.Send(new Ping {Message = "One"});
            var result2 = mediator.Send(new AnotherPing {Message = "Two"});

            var result3 = await mediator.SendAsync(new Ping { Message = "AsyncOne" });
            var result4 = await mediator.SendAsync(new AnotherPing { Message = "AsyncTwo" });

            Assert.Equal("OneHandled", result1.Message);
            Assert.Equal("TwoHandled", result2.Message);

            Assert.Equal("AsyncOneHandledAsync", result3.Message);
            Assert.Equal("AsyncTwoHandledAsync", result4.Message);
        }

        [Fact]
        public void Should_Register_Decorator()
        {
            var mediator = new AutofacMediatorBuilder(container)
                .WithRequestHandler(typeof(PingHandler))
                .WithRequestDecorator("decorator_one", typeof(DecoratorOne<,>))
                .WithRequestDecorator("decorator_two", typeof(DecoratorTwo<,>))
                .Build();

            var pong = mediator.Send(new Ping());

            Assert.Equal("DecoratorTwoDecoratorOneHandled", pong.Message);
        }

        [Fact]
        public async Task Should_Register_Async_Decorator()
        {
            var mediator = new AutofacMediatorBuilder(container)
                .WithRequestHandler(typeof(AsyncPingHandler))
                .WithRequestDecorator("decorator_one", typeof(AsyncDecoratorOne<,>))
                .WithRequestDecorator("decorator_two", typeof(AsyncDecoratorTwo<,>))
                .Build();

            var pong = await mediator.SendAsync(new Ping { Message = "Begin" });

            Assert.Equal("BeginDecoratorTwoDecoratorOneHandledAsync", pong.Message);
        }
    }
}