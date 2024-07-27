namespace GraphQLApi.Mutations.Clients;

public interface IJwtHandling
{
    string GenerateJwtToken(string username);
}
