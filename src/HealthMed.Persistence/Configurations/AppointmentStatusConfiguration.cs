using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HealthMed.Domain.Entities;

namespace HealthMed.Persistence.Configurations
{
    internal sealed class AppointmentStatusConfiguration : IEntityTypeConfiguration<AppointmentStatus>
    {
        public void Configure(EntityTypeBuilder<AppointmentStatus> builder)
        {
            builder.ToTable("appointmentstatus");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).HasColumnName("IdAppointmentStatus").ValueGeneratedNever();
            builder.Property(p => p.Name).HasMaxLength(150).IsRequired();
        }
    }
}
