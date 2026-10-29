using Comments.Application.Attachments;
using Comments.Application.Captcha;
using Comments.Application.Comments;
using Comments.Application.Comments.Text;
using Comments.Application.Comments.Validators;

using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

namespace Comments.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommentService, CommentService>();
        services.AddSingleton<ICommentTextSanitizer, CommentTextSanitizer>();
        services.AddSingleton<ICaptchaService, CaptchaService>();
        services.AddSingleton<AttachmentService>();
        services.AddValidatorsFromAssemblyContaining<CreateCommentRequestValidator>();

        return services;
    }
}