using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HealthMed.Domain.Entities;

namespace HealthMed.Persistence.Configurations
{
    internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("roles");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).HasColumnName("IdRole").ValueGeneratedNever();
            builder.Property(p => p.Name).HasMaxLength(150).IsRequired();            
        }
    }
}
