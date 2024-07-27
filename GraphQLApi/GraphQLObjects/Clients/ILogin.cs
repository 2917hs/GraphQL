namespace GraphQLApi.Mutations.Clients;

public interface ILogin
{
    string Login(string username, string password);
}
