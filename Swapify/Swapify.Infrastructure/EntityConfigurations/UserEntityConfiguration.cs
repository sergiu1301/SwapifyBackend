using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Swapify.Infrastructure.Entities;

namespace Swapify.Infrastructure.EntityConfigurations;

internal class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(s => s.UserId);

        builder.HasIndex(s => s.UserId)
            .IsUnique();

        builder.Property(s => s.UserId)
            .HasMaxLength(36);

        builder.Property(s => s.Salt)
            .HasMaxLength(256);

        builder.Property(s => s.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(s => s.Email)
            .IsUnique();

        builder.Property(s => s.UserName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(s => s.PasswordHash)
            .HasMaxLength(int.MaxValue);

        builder.Property(s => s.EmailConfirmed)
            .HasDefaultValue(false);

        builder.Property(s => s.IsBlocked)
            .HasDefaultValue(false);

        builder.Property(s => s.IsOnline)
            .HasDefaultValue(false);

        builder.Property(s => s.IsAdmin)
            .HasDefaultValue(false);

        builder.Property(s => s.FirstName)
            .HasMaxLength(128);

        builder.Property(s => s.LastName)
            .HasMaxLength(128);

        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId);
    }
}