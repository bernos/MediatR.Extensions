using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Autofac;

namespace MediatR.Extensions.Autofac.Tests
{
    public abstract class MediatorBuilderTests
    {
        protected abstract IMediatorBuilder GetMediatorBuilder();
        protected abstract Assembly GetTestAssembly();
        
        [Fact]
        public void Should_Register_Handler()
        {
            var mediator = GetMediatorBuilder()
                .WithRequestHandler(typeof (PingHandler))
                .Build();

            var pong = mediator.Send(new Ping());
            
            Assert.Equal("Handled", pong.Message);
        }

        [Fact]
        public async Task Should_Register_Async_Handler()
        {
            var mediator = GetMediatorBuilder()
                .WithRequestHandler(typeof(AsyncPingHandler))
                .Build();

            var pong = await mediator.SendAsync(new Ping());

            Assert.Equal("HandledAsync", pong.Message);
        }

        [Fact]
        public async Task Should_Register_All_Handlers_From_Assembly()
        {
            var mediator = GetMediatorBuilder()
                .WithRequestHandlerAssemblies(GetTestAssembly())
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
            var mediator = GetMediatorBuilder()
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
            var mediator = GetMediatorBuilder()
                .WithRequestHandler(typeof(AsyncPingHandler))
                .WithRequestDecorator("decorator_one", typeof(AsyncDecoratorOne<,>))
                .WithRequestDecorator("decorator_two", typeof(AsyncDecoratorTwo<,>))
                .Build();

            var pong = await mediator.SendAsync(new Ping { Message = "Begin" });

            Assert.Equal("BeginDecoratorTwoDecoratorOneHandledAsync", pong.Message);
        }

        [Fact]
        public void Should_Register_NotificationHandler()
        {
            var mediator = GetMediatorBuilder()
                .WithNotificationHandler(typeof (NoteHandler))
                .Build();

            var notification = new Note();

            mediator.Publish(notification);

            Assert.Equal(1, notification.Count);
        }

        [Fact]
        public void Should_Register_Multiple_NotificationHandlers()
        {
            var mediator = GetMediatorBuilder()
                .WithNotificationHandler(typeof (NoteHandler))
                .WithNotificationHandler(typeof (AnotherNoteHandler))
                .Build();

            var notification = new Note();

            mediator.Publish(notification);

            Assert.Equal(2, notification.Count);
        }

        [Fact]
        public async Task Should_Register_Async_NotificationHandler()
        {
            var mediator = GetMediatorBuilder()
                .WithNotificationHandler(typeof (AsyncNoteHandler))
                .Build();

            var notification = new Note();

            await mediator.PublishAsync(notification);

            Assert.Equal(1, notification.Count);
        }

        [Fact]
        public void Generic_Notification_Handler_Does_Handle_All_Inheriting_Notification_Types()
        {
            var mediator = GetMediatorBuilder()
                .WithNotificationHandler(typeof(GenericNotificationHandler))
                .Build();

            var notification = new Note();

            mediator.Publish(notification);

            Assert.Equal(1, notification.Count);
        }

        [Fact]
        public void Should_Register_All_Notification_Handlers_In_Assembly()
        {
            var mediator = GetMediatorBuilder()
                .WithNotificationHandlerAssemblies(GetTestAssembly())
                .Build();

            var notification = new Note();

            mediator.Publish(notification);

            Assert.Equal(3, notification.Count);
        }
    }

    public class AutofacMediatorBuilderTests : MediatorBuilderTests
    {
        private readonly ILifetimeScope container;

        public AutofacMediatorBuilderTests()
        {
            container = new ContainerBuilder().Build();
        }

        protected override IMediatorBuilder GetMediatorBuilder()
        {
            return new AutofacMediatorBuilder(new ContainerBuilder().Build());
        }

        protected override Assembly GetTestAssembly()
        {
            return typeof (AutofacMediatorBuilderTests).Assembly;
        }
    }
}