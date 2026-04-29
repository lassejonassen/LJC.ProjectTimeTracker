namespace ProjectTimeTracker.WebAPI.Contracts.Projects;

public sealed record ProjectListResponseDTO(
    IReadOnlyCollection<ProjectResponseDTO> Data
);