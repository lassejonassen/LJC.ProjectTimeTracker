using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTimeTracker.Domain.Projects.Entities;
using ProjectTimeTracker.Domain.Projects.Enums;

namespace ProjectTimeTracker.Infrastructure.Persistence.Configurations;

internal sealed class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.ToTable("TimeEntries");

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Project)
            .WithMany(x => x.TimeEntries)
            .HasForeignKey(x => x.ProjectId);

        builder.Property(x => x.UserId)
            .IsRequired(true);

        builder.Property(x => x.Notes)
            .HasMaxLength(TimeEntry.NotesMaxLength)
            .IsRequired(false);

        builder.Property(x => x.Hours)
            .HasDefaultValue(0.0)
            //.HasPrecision()
            .IsRequired(true);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasDefaultValue(TimeEntryStatus.Draft)
            .IsRequired(true);

    }
}
