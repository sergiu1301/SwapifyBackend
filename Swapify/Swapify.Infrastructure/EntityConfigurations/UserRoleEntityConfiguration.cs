using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Swapify.Infrastructure.Entities;

namespace Swapify.Infrastructure.EntityConfigurations;

internal class UserRoleEntityConfiguration : IEntityTypeConfiguration<UserRoleEntity>
{
    public void Configure(EntityTypeBuilder<UserRoleEntity> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasIndex(ur => new { ur.UserId })
            .IsUnique();

        builder.HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique();

        builder.Property(ur => ur.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(ur => ur.UpdatedAt)
            .IsRequired(false);

        builder.Property(ur => ur.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ur => ur.UpdatedBy)
            .HasMaxLength(100);
    }
}