namespace Nucleus.Misc.Helpers;

public class NRetryHelper
{
    private readonly ResiliencePipeline _resiliencePipeline;

    public NRetryHelper(int maxRetryAttempts = 3, int delayInMilliSecs = 5,
        DelayBackoffType delayBackoffType = DelayBackoffType.Constant)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxRetryAttempts);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(delayInMilliSecs);

        var retryStrategyOptions = new RetryStrategyOptions
        {
            MaxRetryAttempts = maxRetryAttempts,
            Delay = TimeSpan.FromMilliseconds(delayInMilliSecs),
            BackoffType = delayBackoffType,
            ShouldHandle = new PredicateBuilder()
                .Handle<Exception>()
        };

        _resiliencePipeline = new ResiliencePipelineBuilder()
            .AddRetry(retryStrategyOptions)
            .Build();
    }

    public void Execute(Action action)
    {
        _resiliencePipeline.Execute(action);
    }

    public TResult Execute<TResult>(Func<TResult> func)
    {
        return _resiliencePipeline.Execute(func);
    }
}