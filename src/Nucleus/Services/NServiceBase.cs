namespace Nucleus.Services;

/// <summary>
///     Provides shared configuration, logging, and mapping dependencies for derived service implementations.
/// </summary>
public abstract class NServiceBase
{
    protected readonly IConfiguration Configuration;

    protected readonly ILogger<NServiceBase> Logger;

    protected readonly IMapper Mapper;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NServiceBase" /> class.
    /// </summary>
    /// <param name="configuration">The application configuration available to the derived service.</param>
    /// <param name="mapper">The mapper used to translate between service-layer and persistence-layer models.</param>
    /// <param name="logger">The logger used to record service activity and diagnostics.</param>
    protected NServiceBase(
        IConfiguration configuration,
        IMapper mapper,
        ILogger<NServiceBase> logger)
    {
        Configuration = configuration;
        Mapper = mapper;
        Logger = logger;
    }
}