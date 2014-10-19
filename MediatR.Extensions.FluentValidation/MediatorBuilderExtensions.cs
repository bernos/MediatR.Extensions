namespace MediatR.Extensions.FluentValidation
{
    public static class MediatorBuilderExtensions
    {
        public static void UseFluentValidation(this IMediatorBuilder builder)
        {
            builder.WithRequestDecorator("FluentValidation", typeof (ValidationRequestHandler<,>));
            builder.WithRequestDecorator("FluentValidationAsync", typeof (AsyncValidationRequestHandler<,>));
        }
    }
}