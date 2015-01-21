namespace MediatR.Extensions.Autofac.Tests
{
    public interface INotificationWithCount : INotification, IAsyncNotification
    {
        int Count { get; set; }
    }
}