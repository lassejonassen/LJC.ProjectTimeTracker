using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTimeTracker.Domain.Projects.Aggregates;
using ProjectTimeTracker.Domain.Projects.Enums;

namespace ProjectTimeTracker.Infrastructure.Persistence.Configurations;

internal sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(Project.NameMaxLength)
            .IsRequired(true);

        builder.Property(x => x.Description)
            .HasMaxLength(Project.DescriptionMaxLength)
            .IsRequired(false);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasDefaultValue(ProjectStatus.Open)
            .IsRequired(true);
    }
}
