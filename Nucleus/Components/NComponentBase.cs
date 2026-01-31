namespace Nucleus.Components;

public class NComponentBase : ComponentBase
{
    protected readonly ILogger<ComponentBase> Logger;

    protected readonly IMediator Mediator;

    protected NComponentBase(IMediator mediator, ILogger<ComponentBase> logger)
    {
        Mediator = mediator;
        Logger = logger;
    }
}
