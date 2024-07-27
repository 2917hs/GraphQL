using GraphQLApi.Database.Models;

namespace GraphQLApi.GraphQLObjects.Clients;

public interface IPayer
{
    IQueryable<Payer> GetPayers(int? chhaId, string? chhaName, string? chhaInitial);
}
