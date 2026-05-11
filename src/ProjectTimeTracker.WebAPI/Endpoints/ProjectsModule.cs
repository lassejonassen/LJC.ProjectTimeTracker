using Carter;
using ProjectTimeTracker.Application.Abstractions.Messaging;
using ProjectTimeTracker.Application.Projects.Commands;
using ProjectTimeTracker.Application.Projects.Commands.TimeEntries;
using ProjectTimeTracker.Application.Projects.Queries;
using ProjectTimeTracker.Domain.Authorization;
using ProjectTimeTracker.WebAPI.Contracts.Projects;
using ProjectTimeTracker.WebAPI.Contracts.Projects.TimeEntries;
using ProjectTimeTracker.WebAPI.Extensions;

namespace ProjectTimeTracker.WebAPI.Endpoints;

public class ProjectsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/projects")
            .WithTags("Projects")
            .WithDefaultResponses()
            .RequireAuthorization();

        #region Project Endpoints

        group.MapGet("/", async (IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetAllProjectsQuery(), ct);

            if (result.Count == 0)
                return Results.NoContent();

            var response = result.Select(x => new ProjectResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Status = x.Status,
                CreatedAtUtc = x.CreatedAtUtc,
                UpdatedAtUtc = x.UpdatedAtUtc,
            }).ToList();

            return Results.Ok(new ProjectListResponse(response));
        })
            .WithName("GetAllProjects")
            .Produces<ProjectListResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization(Permissions.Projects.Read);

        group.MapGet("/{projectId:guid}", async (Guid projectId, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetProjectByIdQuery(projectId), ct);

            if (result.IsFailure)
                return result.Error.Handle();

            var response = new ProjectResponse
            {
                Id = result.Value.Id,
                Name = result.Value.Name,
                Description = result.Value.Description,
                Status = result.Value.Status,
                CreatedAtUtc = result.Value.CreatedAtUtc,
                UpdatedAtUtc = result.Value.UpdatedAtUtc,
                TimeEntries = result.Value.TimeEntries?.Select(x => new TimeEntryResponseDTO
                {
                    Id = x.Id,
                    CreatedAtUtc = x.CreatedAtUtc,
                    Hours = x.Hours,
                    ProjectId = x.ProjectId,
                    Status = x.Status,
                    UpdatedAtUtc = x.UpdatedAtUtc,
                    UserId = x.UserId,
                    Notes = x.Notes
                }).ToList()
            };

            return Results.Ok(response);
        })
            .WithName("GetProjectById")
            .Produces<ProjectResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(Permissions.Projects.Read);

        group.MapPost("/", async (ProjectCreateRequest request, IMediator mediator, CancellationToken ct) =>
        {
            var command = new CreateProjectCommand(request.Name, request.Description);
            var result = await mediator.Send(command, ct);

            return result.IsFailure
                ? result.Error.Handle()
                : Results.CreatedAtRoute("GetProjectById", new { projectId = result.Value }, result.Value);
        })
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Permissions.Projects.Create);

        group.MapPut("/{projectId:guid}", async (Guid projectId, ProjectUpdateRequest request, IMediator mediator, CancellationToken ct) =>
        {
            if (projectId != request.ProjectId)
                return ResultsExtensions.MismatchedId(projectId, request.ProjectId);

            var command = new UpdateProjectCommand(request.ProjectId, request.Name, request.Description);
            var result = await mediator.Send(command, ct);

            return result.IsFailure ? result.Error.Handle() : Results.NoContent();
        })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(Permissions.Projects.Update);

        group.MapPatch("/{projectId:guid}/close", async (Guid projectId, ProjectCloseRequest request, IMediator mediator, CancellationToken ct) =>
        {
            if (projectId != request.ProjectId)
                return ResultsExtensions.MismatchedId(projectId, request.ProjectId);

            var result = await mediator.Send(new CloseProjectCommand(request.ProjectId), ct);
            return result.IsFailure ? result.Error.Handle() : Results.NoContent();
        })
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization(Permissions.Projects.ManageStatus);

        group.MapPatch("/{projectId:guid}/reopen", async (Guid projectId, ProjectReopenRequest request, IMediator mediator, CancellationToken ct) =>
        {
            if (projectId != request.ProjectId)
                return ResultsExtensions.MismatchedId(projectId, request.ProjectId);

            var result = await mediator.Send(new ReopenProjectCommand(request.ProjectId), ct);
            return result.IsFailure ? result.Error.Handle() : Results.NoContent();
        })
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization(Permissions.Projects.ManageStatus);

        group.MapDelete("/{projectId:guid}", async (Guid projectId, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new DeleteProjectCommand(projectId), ct);
            return result.IsFailure ? result.Error.Handle() : Results.NoContent();
        })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(Permissions.Projects.Delete);

        #endregion

        #region Time Entry Endpoints

        var timeEntriesGroup = group.MapGroup("/{projectId:guid}/time-entries")
            .WithTags("Project Time Entries");

        timeEntriesGroup.MapPost("/", async (Guid projectId, ProjectTimeEntryCreateRequest request, IMediator mediator, CancellationToken ct) =>
        {
            if (projectId != request.ProjectId)
                return ResultsExtensions.MismatchedId(projectId, request.ProjectId);

            var command = new CreateProjectTimeEntryCommand(projectId, request.Notes, request.Hours);
            var result = await mediator.Send(command, ct);

            return result.IsFailure ? result.Error.Handle() : Results.NoContent();
        })
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization(Permissions.Projects.ProjectTimeEntries.Create);

        timeEntriesGroup.MapPut("/{timeEntryId:guid}", async (Guid projectId, Guid timeEntryId, ProjectTimeEntryUpdateRequest request, IMediator mediator, CancellationToken ct) =>
        {
            if (projectId != request.ProjectId || timeEntryId != request.TimeEntryId)
                return ResultsExtensions.MismatchedId(projectId, request.TimeEntryId);

            var command = new UpdateProjectTimeEntryCommand(projectId, timeEntryId, request.Notes, request.Hours);
            var result = await mediator.Send(command, ct);

            return result.IsFailure ? result.Error.Handle() : Results.NoContent();
        })
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization(Permissions.Projects.ProjectTimeEntries.Update);

        timeEntriesGroup.MapPatch("/{timeEntryId:guid}/approve", async (Guid projectId, Guid timeEntryId, ProjectTimeEntryStatusChangeRequest request, IMediator mediator, CancellationToken ct) =>
        {
            if (projectId != request.ProjectId || timeEntryId != request.TimeEntryId)
                return ResultsExtensions.MismatchedId(projectId, request.TimeEntryId);

            var result = await mediator.Send(new ApproveProjectTimeEntryCommand(projectId, timeEntryId), ct);
            return result.IsFailure ? result.Error.Handle() : Results.NoContent();
        })
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization(Permissions.Projects.ProjectTimeEntries.Approve);

        timeEntriesGroup.MapPatch("/{timeEntryId:guid}/reject", async (Guid projectId, Guid timeEntryId, ProjectTimeEntryStatusChangeRequest request, IMediator mediator, CancellationToken ct) =>
        {
            if (projectId != request.ProjectId || timeEntryId != request.TimeEntryId)
                return ResultsExtensions.MismatchedId(projectId, request.TimeEntryId);

            var result = await mediator.Send(new RejectProjectTimeEntryCommand(projectId, timeEntryId), ct);
            return result.IsFailure ? result.Error.Handle() : Results.NoContent();
        })
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization(Permissions.Projects.ProjectTimeEntries.Approve);

        timeEntriesGroup.MapPatch("/{timeEntryId:guid}/submit", async (Guid projectId, Guid timeEntryId, ProjectTimeEntryStatusChangeRequest request, IMediator mediator, CancellationToken ct) =>
        {
            if (projectId != request.ProjectId || timeEntryId != request.TimeEntryId)
                return ResultsExtensions.MismatchedId(projectId, request.TimeEntryId);

            var result = await mediator.Send(new SubmitProjectTimeEntryCommand(projectId, timeEntryId), ct);
            return result.IsFailure ? result.Error.Handle() : Results.NoContent();
        })
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization(Permissions.Projects.ProjectTimeEntries.Submit);

        #endregion
    }
}