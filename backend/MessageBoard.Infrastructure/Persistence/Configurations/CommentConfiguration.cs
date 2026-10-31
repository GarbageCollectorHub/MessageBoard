using MessageBoard.Domain.Constants;
using MessageBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessageBoard.Infrastructure.Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments", table =>
        {
            // Enforce the text limit in the database for nvarchar(max).
            // DATALENGTH counts bytes, so multiply the UTF-16 length by two.
            table.HasCheckConstraint(
                "CK_Comments_Text_MaxLength",
                $"DATALENGTH([Text]) <= {CommentLimits.TextMaxLength * 2}");
        });

        builder.HasKey(comment => comment.Id);

        builder.Property(comment => comment.UserName)
            .HasMaxLength(CommentLimits.UserNameMaxLength)
            .IsRequired();

        builder.Property(comment => comment.Email)
            .HasMaxLength(CommentLimits.EmailMaxLength)
            .IsRequired();

        builder.Property(comment => comment.HomePage)
            .HasMaxLength(CommentLimits.HomePageMaxLength);

        builder.Property(comment => comment.Text)
            .HasColumnType("nvarchar(max)")
            .HasMaxLength(CommentLimits.TextMaxLength)
            .IsRequired();

        // Prevent physical deletion of a parent while replies still reference it.
        builder.HasOne(comment => comment.Parent)
            .WithMany(comment => comment.Replies)
            .HasForeignKey(comment => comment.ParentId)
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.HasIndex(comment => new
        {
            comment.ParentId,
            comment.CreatedAtUtc,
            comment.Id
        });
    }
}