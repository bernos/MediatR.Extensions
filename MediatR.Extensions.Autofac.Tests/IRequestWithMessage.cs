namespace MediatR.Extensions.Autofac.Tests
{
    public interface IRequestWithMessage<out T> : IRequest<T>, IAsyncRequest<T>
    {
        string Message { get; set; }
    }
}