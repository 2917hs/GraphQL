namespace GraphQLApi.GraphQLObjects.Clients;

public interface IOpenShift
{
    Task<string> GetOpenShiftAsync(string globalProviderId, string globalOfficeId, string jwtToken);
}
