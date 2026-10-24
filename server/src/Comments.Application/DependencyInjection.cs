using Comments.Application.Comments;
using Comments.Application.Comments.Validators;

using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

namespace Comments.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommentService, CommentService>();
        services.AddValidatorsFromAssemblyContaining<CreateCommentRequestValidator>();

        return services;
    }
}