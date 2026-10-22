using Comments.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Comments.Infrastructure.Persistence;

public class CommentsDbContext(DbContextOptions<CommentsDbContext> options) : DbContext(options)
{
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CommentsDbContext).Assembly);
    }
}