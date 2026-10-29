using Comments.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comments.Infrastructure.Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserName).HasMaxLength(Comment.UserNameMaxLength);
        builder.Property(c => c.Email).HasMaxLength(Comment.EmailMaxLength);
        builder.Property(c => c.IpAddress).HasMaxLength(Comment.IpAddressMaxLength);
        builder.Property(c => c.HomePage).HasMaxLength(Comment.HomePageMaxLength);
        builder.Property(c => c.UserAgent).HasMaxLength(Comment.UserAgentMaxLength);
        builder.Property(c => c.CreatedAt)
            .HasConversion(
                v => v,
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );

        builder.HasOne<Comment>()
            .WithMany()
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Comment>()
            .WithMany()
            .HasForeignKey(c => c.RootId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(c => c.Attachment)
            .WithOne()
            .HasForeignKey<Attachment>(a => a.CommentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => new { c.ParentId, c.CreatedAt });
        builder.HasIndex(c => new { c.ParentId, c.UserName });
        builder.HasIndex(c => new { c.ParentId, c.Email });
    }
}