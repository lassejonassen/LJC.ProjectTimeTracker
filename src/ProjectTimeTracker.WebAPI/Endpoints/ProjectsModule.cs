using Carter;
using ProjectTimeTracker.Application.Abstractions.Messaging;
using ProjectTimeTracker.Application.Abstractions.Security;
using ProjectTimeTracker.Application.Projects.Commands;
using ProjectTimeTracker.Application.Projects.Commands.TimeEntries;
using ProjectTimeTracker.Application.Projects.Queries;
using ProjectTimeTracker.WebAPI.Contracts.Projects;
using ProjectTimeTracker.WebAPI.Contracts.Projects.TimeEntries;
using ProjectTimeTracker.WebAPI.Extensions;
using Scalar.AspNetCore;

namespace ProjectTimeTracker.WebAPI.Endpoints;

public class ProjectsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/projects")
            .WithTags("Projects")
            .WithDefaultResponses()
            .RequireAuthorization();

        group.MapGet("/", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var query = new GetAllProjectsQuery();
            var result = await mediator.Send(query, cancellationToken);

            if (result.Count == 0)
                return Results.NoContent();

            var _result = result.Select(x => new ProjectResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Status = x.Status,
                CreatedAtUtc = x.CreatedAtUtc,
                UpdatedAtUtc = x.UpdatedAtUtc,
            }).ToList();

            return Results.Ok(new ProjectListResponse(_result));
        })
            .WithName("GetAllProjects")
            .WithDisplayName("Get all projects")
            .WithDescription("Get all projects").WithBadge("GetAll")
            .Produces<ProjectListResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization(Permissions.ProjectsRead);


        group.MapGet("/{projectId:guid}", async (Guid projectId, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var query = new GetProjectByIdQuery(projectId);
            var result = await mediator.Send(query, cancellationToken);

            if (result.IsFailure)
                return result.Error.Handle();

            var _result = new ProjectResponse
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

            return Results.Ok(_result);
        })
            .WithName("GetProjectById")
            .WithDisplayName("Get project by Id")
            .Produces<ProjectResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(Permissions.ProjectsRead);

        group.MapPost("/", async (ProjectCreateRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var command = new CreateProjectCommand(request.Name, request.Description);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.Handle();

            return Results.CreatedAtRoute("GetProjectById", new { projectId = result.Value }, result.Value);
        })
            .WithDisplayName("Create Project")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Permissions.ProjectsWrite);

        group.MapPut("/{projectId:guid}", async (Guid projectId, ProjectUpdateRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            if (projectId != request.ProjectId)
                return ResultsExtensions.MismatchedId(projectId, request.ProjectId);

            var command = new UpdateProjectCommand(request.ProjectId, request.Name, request.Description);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.Handle();

            return Results.NoContent();
        })
            .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(Permissions.ProjectsWrite);

        group.MapPatch("/{projectId:guid}/close", async (Guid projectId, ProjectCloseRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            if (projectId != request.ProjectId)
                return ResultsExtensions.MismatchedId(projectId, request.ProjectId);

            var command = new CloseProjectCommand(request.ProjectId);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.Handle();

            return Results.NoContent();
        })
            .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .RequireAuthorization(Permissions.ProjectsWrite);

        group.MapPatch("/{projectId:guid}/reopen", async (Guid projectId, ProjectReopenRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            if (projectId != request.ProjectId)
                return ResultsExtensions.MismatchedId(projectId, request.ProjectId);

            var command = new ReopenProjectCommand(request.ProjectId);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.Handle();

            return Results.NoContent();
        })
            .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .RequireAuthorization(Permissions.ProjectsWrite);

        group.MapDelete("/{projectId:guid}", async (Guid projectId, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var command = new DeleteProjectCommand(projectId);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.Handle();

            return Results.NoContent();
        })
            .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);

        var timeEntriesGroup = group.MapGroup("/{projectId:guid}/time-entries")
            .WithTags("Project Time Entries")
            .RequireAuthorization(Permissions.ProjectsDelete);

        timeEntriesGroup.MapPost("/", async (Guid projectId, ProjectTimeEntryCreateRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            if (projectId != request.ProjectId)
                return ResultsExtensions.MismatchedId(projectId, request.ProjectId);

            var command = new CreateProjectTimeEntryCommand(projectId, "SOME USER", request.Notes, request.Hours);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.Handle();

            return Results.NoContent();
        })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(Permissions.ProjectsWrite);


        timeEntriesGroup.MapPut("/{timeEntryId:guid}", async(Guid projectId, Guid timeEntryId, ProjectTimeEntryUpdateRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            if (projectId != request.ProjectId)
                return ResultsExtensions.MismatchedId(projectId, request.ProjectId);

            if (timeEntryId != request.TimeEntryId)
                return ResultsExtensions.MismatchedId(projectId, request.TimeEntryId);

            var command = new UpdateProjectTimeEntryCommand(projectId, timeEntryId, request.Notes, request.Hours);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.Handle();

            return Results.NoContent();
        })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(Permissions.ProjectsWrite);

        timeEntriesGroup.MapPatch("/{timeEntryId:guid}/approve", async (Guid projectId, Guid timeEntryId, ProjectTimeEntryStatusChangeRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            if (projectId != request.ProjectId)
                return ResultsExtensions.MismatchedId(projectId, request.ProjectId);

            if (timeEntryId != request.TimeEntryId)
                return ResultsExtensions.MismatchedId(projectId, request.TimeEntryId);

            var command = new ApproveProjectTimeEntryCommand(projectId, timeEntryId);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.Handle();

            return Results.NoContent();
        })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        timeEntriesGroup.MapPatch("/{timeEntryId:guid}/reject", async (Guid projectId, Guid timeEntryId, ProjectTimeEntryStatusChangeRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            if (projectId != request.ProjectId)
                return ResultsExtensions.MismatchedId(projectId, request.ProjectId);

            if (timeEntryId != request.TimeEntryId)
                return ResultsExtensions.MismatchedId(projectId, request.TimeEntryId);

            var command = new RejectProjectTimeEntryCommand(projectId, timeEntryId);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.Handle();

            return Results.NoContent();
        })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        timeEntriesGroup.MapPatch("/{timeEntryId:guid}/reopen", async (Guid projectId, Guid timeEntryId, ProjectTimeEntryStatusChangeRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            if (projectId != request.ProjectId)
                return ResultsExtensions.MismatchedId(projectId, request.ProjectId);

            if (timeEntryId != request.TimeEntryId)
                return ResultsExtensions.MismatchedId(projectId, request.TimeEntryId);

            var command = new ReopenProjectTimeEntryCommand(projectId, timeEntryId);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.Handle();

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        timeEntriesGroup.MapPatch("/{timeEntryId:guid}/submit", async (Guid projectId, Guid timeEntryId, ProjectTimeEntryStatusChangeRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            if (projectId != request.ProjectId)
                return ResultsExtensions.MismatchedId(projectId, request.ProjectId);

            if (timeEntryId != request.TimeEntryId)
                return ResultsExtensions.MismatchedId(projectId, request.TimeEntryId);

            var command = new SubmitProjectTimeEntryCommand(projectId, timeEntryId);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.Handle();

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
