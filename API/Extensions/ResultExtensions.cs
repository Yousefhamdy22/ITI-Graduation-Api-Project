using Ardalis.Result;
using Microsoft.AspNetCore.Mvc;

namespace API.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        var response = new
        {
            success = result.Status == ResultStatus.Ok || result.Status == ResultStatus.Created,
            status = result.Status.ToString(),
            data = result.Value,
            errors = result.Errors != null && result.Errors.Any() ? result.Errors : null
        };

        return result.Status switch
        {
            ResultStatus.Ok => new OkObjectResult(response),
            ResultStatus.Created => new CreatedResult("", response),
            ResultStatus.NotFound => new NotFoundObjectResult(response),
            ResultStatus.Invalid => new BadRequestObjectResult(response),
            ResultStatus.Error => new BadRequestObjectResult(response),
            ResultStatus.Unauthorized => new UnauthorizedObjectResult(response),
            ResultStatus.Forbidden => new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status403Forbidden
            },
            _ => new BadRequestObjectResult(response)
        };
    }

    public static IActionResult ToActionResult(this Result result)
    {
        var response = new
        {
            success = result.Status == ResultStatus.Ok || result.Status == ResultStatus.Created,
            status = result.Status.ToString(),
            data = (object)null,
            errors = result.Errors != null && result.Errors.Any() ? result.Errors : null
        };

        return result.Status switch
        {
            ResultStatus.Ok => new OkObjectResult(response),
            ResultStatus.NotFound => new NotFoundObjectResult(response),
            ResultStatus.Invalid => new BadRequestObjectResult(response),
            ResultStatus.Error => new BadRequestObjectResult(response),
            ResultStatus.Unauthorized => new UnauthorizedObjectResult(response),
            ResultStatus.Forbidden => new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status403Forbidden
            },
            _ => new BadRequestObjectResult(response)
        };
    }
}