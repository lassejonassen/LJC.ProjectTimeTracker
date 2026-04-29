namespace ProjectTimeTracker.WebAPI.Extensions;

public static class OpenApiExtensions
{
    public static TBuilder WithDefaultResponses<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
