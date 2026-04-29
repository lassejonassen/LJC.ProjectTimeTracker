using ProjectTimeTracker.SharedKernel;

namespace ProjectTimeTracker.WebAPI.Extensions;

public static class ResultsExtensions
{
    public static IResult Handle(this Error error)
    {
        return error.Type switch
        {
            ErrorType.NotFound => Results.Problem(
                detail: error.Description,
                statusCode: StatusCodes.Status404NotFound,
                title: "Not Found",
                extensions: new Dictionary<string, object?> { { "code", error.Code } }),

            ErrorType.Validation => Results.Problem(
                detail: error.Description,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Validation Error",
                extensions: new Dictionary<string, object?> { { "code", error.Code } }),

            ErrorType.Conflict => Results.Problem(
                detail: error.Description,
                statusCode: StatusCodes.Status409Conflict,
                title: "Conflict",
                extensions: new Dictionary<string, object?> { { "code", error.Code } }),

            _ => Results.Problem(
                detail: error.Description,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Server Error",
                extensions: new Dictionary<string, object?> { { "code", error.Code } })
        };
    }

    public static IResult MismatchedId(Guid routeId, Guid payloadId)
    {
        return Results.Problem(
            detail: $"Route ID {routeId} and Payload ID {payloadId} do not match.",
            statusCode: StatusCodes.Status400BadRequest,
            title: "Bad Request");
    }
}
