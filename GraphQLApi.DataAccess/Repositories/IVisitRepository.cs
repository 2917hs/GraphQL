using GraphQLApi.Database.Models;

namespace GraphQLApi.DataAccess.Repositories;

public interface IVisitRepository
{
    IEnumerable<Visit> GetAllVisits();
}
