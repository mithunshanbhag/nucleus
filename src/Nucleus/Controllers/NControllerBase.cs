using ValidationException = FluentValidation.ValidationException;

namespace Nucleus.Controllers;

public abstract class NControllerBase(ILogger<NControllerBase> logger)
{
    protected readonly ILogger<NControllerBase> Logger = logger;

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