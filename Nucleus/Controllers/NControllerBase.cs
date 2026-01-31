using ValidationException = FluentValidation.ValidationException;

namespace Nucleus.Controllers;

public abstract class NControllerBase
{
    protected readonly ILogger<NControllerBase> Logger;

    protected readonly IMediator Mediator;

    protected NControllerBase(IMediator mediator, ILogger<NControllerBase> logger)
    {
        Mediator = mediator;
        Logger = logger;
    }

    protected async Task<IActionResult> ProcessRequestAsync(IRequest<IActionResult> request)
    {
        try
        {
            return await Mediator.Send(request);
        }
        catch (NExceptionBase cpe)
        {
            return cpe.ToActionResult();
        }
        catch (ValidationException ve)
        {
            return new BadRequestObjectResult(ve.Message);
        }
    }

    protected static async Task<IActionResult> ProcessAsync(Func<Task<IActionResult>> func)
    {
        try
        {
            return await func();
        }
        catch (NExceptionBase cpe)
        {
            return cpe.ToActionResult();
        }
        catch (ValidationException ve)
        {
            return new BadRequestObjectResult(ve.Message);
        }
    }
}