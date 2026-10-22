using Comments.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comments.Infrastructure.Persistence.Configurations;

public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.ToTable("Attachments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.OriginalName).HasMaxLength(Attachment.OriginalNameMaxLength);
        builder.Property(a => a.StoredName).HasMaxLength(Attachment.StoredNameMaxLength);
        builder.Property(a => a.ContentType).HasMaxLength(Attachment.ContentTypeMaxLength);
    }
}