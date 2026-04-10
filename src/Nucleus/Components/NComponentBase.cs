namespace Nucleus.Components;

public abstract class NComponentBase(ILogger<ComponentBase> logger) : ComponentBase
{
    protected readonly ILogger<ComponentBase> Logger = logger;
}