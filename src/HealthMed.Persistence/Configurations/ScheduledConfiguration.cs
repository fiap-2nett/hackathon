using HealthMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthMed.Persistence.Configurations
{
    internal sealed class ScheduledConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.ToTable("schedules");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("IdSchedule").IsRequired();
            builder.Property(p => p.IdDoctor).IsRequired();
            builder.Property(p => p.StartDate).IsRequired();
            builder.Property(p => p.EndDate).IsRequired();
            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.LastUpdatedAt);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(p => p.IdDoctor).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
