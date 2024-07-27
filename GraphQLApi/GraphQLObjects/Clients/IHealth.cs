namespace GraphQLApi.GraphQLObjects.Clients;

public interface IHealth
{
    Task<string> GetHealthAsync(string eTag, string jwtToken);
}
