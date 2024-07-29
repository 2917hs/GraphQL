using GraphQLApi.ConfigurationOptions;
using GraphQLApi.Database.Data;
using GraphQLApi.Database.Models;
using Microsoft.Extensions.Options;
using GraphQLApi.GraphQLObjects.Clients;

namespace GraphQLApi.GraphQLObjects.Queries;

public class PayerQuery : IPayer
{
    private readonly InMemoryDbContext _inMemoryDbContext;
    
    public PayerQuery(InMemoryDbContext inMemoryDbContext)
    {
        _inMemoryDbContext = inMemoryDbContext ?? throw new ArgumentNullException(nameof(inMemoryDbContext));
    }
    
    public IQueryable<Payer> GetPayers(int? chhaId = null, string? chhaName = null, string? chhaInitial = null)
    {
        IQueryable<Payer> query = _inMemoryDbContext.Payers;

        if (chhaId.HasValue && !string.IsNullOrEmpty(chhaInitial))
        {
            query = query.Where(p => p.ChhaId == chhaId.Value || p.ChhaInitial.Contains(chhaInitial));
        }
        else if (chhaId.HasValue)
        {
            query = query.Where(p => p.ChhaId == chhaId.Value);
        }
        else if (!string.IsNullOrEmpty(chhaInitial))
        {
            query = query.Where(p => p.ChhaInitial.Contains(chhaInitial));
        }

        return query;
    }
}
