using Carter;
using ProjectTimeTracker.Application.Abstractions.Messaging;
using ProjectTimeTracker.Application.Projects.Commands;
using ProjectTimeTracker.Application.Projects.Queries;
using ProjectTimeTracker.WebAPI.Contracts.Projects;
using Scalar.AspNetCore;

namespace ProjectTimeTracker.WebAPI.Endpoints;

public class ProjectsModule : ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/projects")
            .WithTags("Projects");

        group.MapGet("/", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var query = new GetAllProjectsQuery();
            var result = await mediator.Send(query, cancellationToken);

            if (result.Count == 0)
                return Results.NoContent();

            var _result = result.Select(x => new ProjectResponseDTO
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Status = x.Status,
                CreatedAtUtc = x.CreatedAtUtc,
                UpdatedAtUtc = x.UpdatedAtUtc,
            }).ToList();

            return Results.Ok(new ProjectListResponseDTO(_result));
        })
            .WithName("GetAllProjects")
            .WithDisplayName("Get all projects")
            .WithDescription("Get all projects").WithBadge("GetAll");

        group.MapGet("/{projectId:guid}", async (Guid projectId, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var query = new GetProjectByIdQuery(projectId);
            var result = await mediator.Send(query, cancellationToken);

            if (result.IsFailure)
                return Results.NotFound();

            var _result = new ProjectResponseDTO
            {
                Id = result.Value.Id,
                Name = result.Value.Name,
                Description = result.Value.Description,
                Status = result.Value.Status,
                CreatedAtUtc = result.Value.CreatedAtUtc,
                UpdatedAtUtc = result.Value.UpdatedAtUtc,
            };

            return Results.Ok(_result);
        })
            .WithName("GetProjectById")
            .WithDisplayName("Get project by Id");

        group.MapPost("/", async (ProjectCreateRequestDTO request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var command = new CreateProjectCommand(request.Name, request.Description);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest();

            return Results.CreatedAtRoute("GetProjectById", new { projectId = result.Value }, result.Value);
        })
            .WithDisplayName("Create Project");

        group.MapPut("/{projectId:guid}", async (Guid projectId, ProjectUpdateRequestDTO request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            if (projectId != request.ProjectId)
                return Results.BadRequest("ProjectId from the route does not match the ProjectId in the payload");

            var command = new UpdateProjectCommand(request.ProjectId, request.Name, request.Description);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest();

            return Results.NoContent();
        });

        group.MapPatch("/{projectId:guid}/close", async (Guid projectId, ProjectCloseRequestDTO request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            if (projectId != request.ProjectId)
                return Results.BadRequest("ProjectId from the route does not match the ProjectId in the payload");

            var command = new CloseProjectCommand(request.ProjectId);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest();

            return Results.NoContent();
        });

        group.MapPatch("/{projectId:guid}/reopen", async (Guid projectId, ProjectReopenRequestDTO request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            if (projectId != request.ProjectId)
                return Results.BadRequest("ProjectId from the route does not match the ProjectId in the payload");

            var command = new ReopenProjectCommand(request.ProjectId);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest();

            return Results.NoContent();
        });

        group.MapDelete("/{projectId:guid}", async (Guid projectId, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var command = new DeleteProjectCommand(projectId);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest();

            return Results.NoContent();
        });
    }
}
