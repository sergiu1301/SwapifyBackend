using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Swapify.Infrastructure.Entities;

namespace Swapify.Infrastructure.EntityConfigurations;

internal class RoleEntityConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.RoleId);

        builder.HasIndex(ur => ur.RoleId )
            .IsUnique();

        builder.Property(ur => ur.RoleId)
            .HasMaxLength(36);

        builder.Property(ur => ur.Name)
            .HasMaxLength(256);

        builder.HasIndex(ur => ur.Name)
            .IsUnique();

        builder.Property(ur => ur.Description)
            .HasMaxLength(256);

        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId);
    }
}