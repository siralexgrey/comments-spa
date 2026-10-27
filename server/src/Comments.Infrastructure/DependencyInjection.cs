using Comments.Application.Attachments;
using Comments.Application.Captcha;
using Comments.Application.Comments;
using Comments.Infrastructure.Attachments;
using Comments.Infrastructure.Captcha;
using Comments.Infrastructure.Persistence;
using Comments.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Comments.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured");
        var uploadsPath = configuration["Uploads:Path"]
            ?? throw new InvalidOperationException("'Uploads:Path' is not configured");
        var uploadsAbsolutePath = Path.GetFullPath(uploadsPath);

        services.AddDbContext<CommentsDbContext>(options => options.UseSqlServer(connectionString));
        services.AddMemoryCache();

        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddSingleton<ICaptchaImageGenerator, CaptchaImageGenerator>();
        services.AddSingleton<IImageProcessor, ImageProcessor>();
        services.AddSingleton<IFileStorage>(_ => new LocalFileStorage(uploadsAbsolutePath));

        return services;
    }
}