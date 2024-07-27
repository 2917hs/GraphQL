namespace GraphQLApi.ConfigurationOptions;

public class OptionsConfiguration
{
    public required string BaseAddress { get; set; }

    public required string HealthCheckEndPoint { get; set; }

    public required string OpenShiftEndPoint { get; set; }
}
