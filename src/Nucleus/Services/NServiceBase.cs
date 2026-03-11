namespace Nucleus.Services;

public abstract class NServiceBase
{
    protected readonly IConfiguration Configuration;

    protected readonly ILogger<NServiceBase> Logger;

    protected readonly IMapper Mapper;

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