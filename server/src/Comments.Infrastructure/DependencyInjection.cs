using Comments.Application.Comments;
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

        services.AddDbContext<CommentsDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<ICommentRepository, CommentRepository>();

        return services;
    }
}