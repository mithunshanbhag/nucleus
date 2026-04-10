namespace Nucleus.Services;

/// <summary>
///     Provides shared configuration, logging, and mapping dependencies for derived service implementations.
/// </summary>
/// <param name="configuration">The application configuration available to the derived service.</param>
/// <param name="logger">The logger used to record service activity and diagnostics.</param>
public abstract class NServiceBase(IConfiguration configuration, ILogger<NServiceBase> logger)
{
    protected readonly IConfiguration Configuration = configuration;

    protected readonly ILogger<NServiceBase> Logger = logger;
}