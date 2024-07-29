using GraphQLApi.Database.Models;
using GraphQLApi.GraphQLObjects.Clients;

namespace GraphQLApi.GraphQLObjects.Services;

public class PayerService
{
    private readonly IPayer _payer;

    public PayerService(IPayer payer)
    {
       _payer = payer ?? throw new ArgumentNullException(nameof(payer));
    }

    public IQueryable<Payer> GetPayers(int? chhaId = null, string? chhaName = null, string? chhaInitial = null)
    {
        return _payer.GetPayers(chhaId, chhaName, chhaInitial);
    }
}
