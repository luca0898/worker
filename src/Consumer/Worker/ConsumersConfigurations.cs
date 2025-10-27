namespace Worker;

public sealed class ConsumersConfigurations
{
    /// <summary>
    /// Default to 5 seconds
    /// </summary>
    public required int ConsumerDelayMs { get; init; } = 5000;
}
