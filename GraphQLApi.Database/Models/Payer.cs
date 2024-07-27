namespace GraphQLApi.Database.Models;

public class Payer
{
    public int ChhaId { get; set; }
    public required string ChhaInitial { get; set; }
    public required string ChhaName { get; set; }
}