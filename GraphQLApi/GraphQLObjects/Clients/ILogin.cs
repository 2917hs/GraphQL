namespace GraphQLApi.GraphQLObjects.Clients;

public interface ILogin
{
    string Login(string username, string password);
}
