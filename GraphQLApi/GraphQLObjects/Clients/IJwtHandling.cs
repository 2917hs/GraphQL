namespace GraphQLApi.GraphQLObjects.Clients;

public interface IJwtHandling
{
    string GenerateJwtToken(string username);
}
