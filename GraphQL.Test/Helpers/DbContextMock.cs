using GraphQLApi.Database.Data;
using GraphQLApi.Database.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GraphQLApi.Test.Helpers;

public static class DbContextMock
{
    public static DbSet<T> GetQueryableMockDbSet<T>(this IEnumerable<T> sourceList) where T : class
    {
        var queryable = sourceList.AsQueryable();

        var dbSet = new Mock<DbSet<T>>();
        dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        dbSet.As<IEnumerable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        return dbSet.Object;
    }

    public static InMemoryDbContext GetInMemoryDataContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<InMemoryDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var dbContext = new InMemoryDbContext(options);
        Seed(dbContext);
        return dbContext;
    }

    private static void Seed(InMemoryDbContext dbContext)
    {
        // Seed your in-memory database as needed for testing
        dbContext.Payers.Add(new Payer { ChhaId = 1, ChhaInitial = "ABC", ChhaName = "Name1" });
        dbContext.Payers.Add(new Payer { ChhaId = 2, ChhaInitial = "XYZ", ChhaName = "Name2" });
        dbContext.SaveChanges();
    }
}