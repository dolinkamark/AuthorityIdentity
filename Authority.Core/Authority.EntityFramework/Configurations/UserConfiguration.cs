﻿using System.Data.Entity.ModelConfiguration;
using Authority.DomainModel;

namespace Authority.EntityFramework.Configurations
{
    public sealed class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            ToTable(User.TableName);

            HasKey(e => new { e.DomainId, e.Email });

            Property(u => u.DomainId).IsRequired();
            Property(u => u.Email).IsRequired().HasMaxLength(128);
            Property(u => u.Username).IsRequired().HasMaxLength(64);
            Property(u => u.LastLogin).IsRequired();
            Property(d => d.PasswordHash).IsRequired().HasMaxLength(128);
            Property(d => d.Salt).IsRequired().HasMaxLength(128);
            Property(d => d.IsPending).IsRequired();
            Property(d => d.IsActive).IsRequired();
            Property(d => d.PendingRegistrationId).IsRequired();

            HasMany(u => u.Policies).WithMany(p => p.Users);
        }
    }
}
