namespace ProjectTimeTracker.WebAPI.Contracts.Projects;

public sealed record ProjectListResponse(
    IReadOnlyCollection<ProjectResponse> Data
);