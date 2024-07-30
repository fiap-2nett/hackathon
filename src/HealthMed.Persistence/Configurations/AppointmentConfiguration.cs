using HealthMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthMed.Persistence.Configurations
{
    internal sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("appointments");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("IdAppointment").IsRequired();
            builder.Property(p => p.IdDoctor).IsRequired();
            builder.Property(p => p.IdPatient).IsConcurrencyToken();
            builder.Property(p => p.AppointmentDate).IsRequired();
            builder.Property(p => p.HasBeenNotified).IsRequired();
            builder.Property(p => p.IdAppointmentStatus).IsRequired();
            builder.Property(p => p.CanceledAt);
            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.LastUpdatedAt);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(p => p.IdDoctor).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(p => p.IdPatient).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<AppointmentStatus>()
                .WithMany()
                .HasForeignKey(p => p.IdAppointmentStatus).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
