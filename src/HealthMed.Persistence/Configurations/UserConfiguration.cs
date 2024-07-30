using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Enumerations;
using HealthMed.Domain.ValueObjects;
using HealthMed.Infrastructure.Cryptography;

namespace HealthMed.Persistence.Configurations
{
    internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        private readonly PasswordHasher _passwordHasher = new PasswordHasher();

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("IdUser").IsRequired();
            builder.Property(p => p.IdRole).IsRequired();
            builder.Property(p => p.Name).HasMaxLength(180).IsRequired();
            builder.Property(p => p.CPF).HasMaxLength(11).IsRequired();
            builder.Property(p => p.CRM).HasMaxLength(10);
            builder.OwnsOne(p => p.Email, builder =>
            {
                builder.WithOwner();
                builder.Property(email => email.Value)
                    .HasColumnName(nameof(User.Email))
                    .HasMaxLength(Email.MaxLength)
                    .IsRequired();
            });
            builder.Property<string>("_passwordHash").HasField("_passwordHash").HasColumnName("PasswordHash").IsRequired();
            
            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.LastUpdatedAt);

            builder.HasOne<Role>()
                .WithMany()
                .HasForeignKey(p => p.IdRole).OnDelete(DeleteBehavior.NoAction);

            SeedBuiltInUsers(builder);
        }

        private void SeedBuiltInUsers(EntityTypeBuilder<User> builder)
        {
            var users = new List<(int Id, string Name, string CPF, string CRM, Email Email, UserRoles Role, string PasswordHash)>()
            {
                new (10_000, "Dr. Doctor 1",    "16058582008",  "1605858-BR",   Email.Create("dr.doctor1@healthmed.app"),   UserRoles.Doctor,   @"MUKOsLOjfoh4YY1ZZLlp+CTyODjmgHhvPAp7PxFiCAWgXo1wibTbOrqht1UhnQi1"), //Password: Admin@123
                new (10_001, "Dr. Doctor 2",    "80084320044",  "8008432-BR",   Email.Create("dr.doctor2@healthmed.app"),   UserRoles.Doctor,   @"MUKOsLOjfoh4YY1ZZLlp+CTyODjmgHhvPAp7PxFiCAWgXo1wibTbOrqht1UhnQi1"), //Password: Admin@123

                new (10_002, "Ailton",          "04953908015",  null,           Email.Create("ailton@techchallenge.app"),   UserRoles.Patient,  @"LFhLAgFT8oinF3iXkk63ccZhEllpvGtr/OHG28On+hqniGeX+AIYe8UhNnqztEIm"), //Password: Ailton@123
                new (10_003, "Bruno",           "30050470086",  null,           Email.Create("bruno@techchallenge.app"),    UserRoles.Patient,  @"yobUq3aH9/R2x//xYdfaxqX2+FVBBLKzLipbFZILjsTo2sJ9cU/f2F4q6vvwIRzs"), //Password: Bruno@123
                new (10_004, "Cecília",         "80738939080",  null,           Email.Create("cecilia@techchallenge.app"),  UserRoles.Patient,  @"LSHTSlFvEBDMS0tjoK2po682H7rLfgL2sXssgm/djzWWouzW4lIydGie7PbmX/1P"), //Password: Cecilia@123
                new (10_005, "Cesar",           "53900451060",  null,           Email.Create("cesar@techchallenge.app"),    UserRoles.Patient,  @"q1EyG7yB1S6Cwm7DGuDo3P8ZraEvVHTdBbKHZ1QW3TMG5JWVCnb3EO3UslYiiGeL"), //Password: Cesar@123
                new (10_006, "Paulo",           "41739149033",  null,           Email.Create("paulo@techchallenge.app"),    UserRoles.Patient,  @"XAro1VAlABuvkw5sxcSPEUdCeuTZRcM+9qLOumd79674Ro2V0bvvnlgb3zIkA7Yt"), //Password: Paulo@123
            };

            builder.HasData(users.Select(user => new
            {
                user.Id,
                user.Name,
                user.CPF,
                user.CRM,
                IdRole = (byte)user.Role,
                CreatedAt = DateTime.MinValue.Date,                
                _passwordHash = user.PasswordHash
            }));

            builder.OwnsOne(p => p.Email).HasData(users.Select(user => new
            {
                UserId = user.Id,
                user.Email.Value
            }));
        }
    }
}
