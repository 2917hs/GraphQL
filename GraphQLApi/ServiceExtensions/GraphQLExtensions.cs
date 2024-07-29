using MongoDB.Bson.Serialization;
using System.Reflection;
using GraphQLApi.Database;
using GraphQLApi.Database.Models;
using GraphQLApi.GraphQLObjects;
using GraphQLApi.GraphQLObjects.Queries;
using GraphQLApi.GraphQLObjects.Services;
using GraphQLApi.GraphQLObjects.Types;
using GraphQLApi.Middlewares;

namespace GraphQLApi.ServiceExtensions
{
    public static class GraphQlExtensions
    {
        public static IApplicationBuilder UseETagMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ETagMiddleware>();
        }

        public static IServiceCollection AddCustomGraphQl(this IServiceCollection services)
        {
            var schemaBuilder = services
                .AddGraphQLServer()
                .AddAuthorization(options =>
                {
                    options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
                });

            // var assembly = Assembly.GetExecutingAssembly();
            // var allTypes = assembly.GetTypes()
            //     .Where(t => t.GetCustomAttributes(typeof(GraphQlTypeAttribute), false).Any() &&
            //                 t is { IsAbstract: false, IsInterface: false })
            //     .ToList();
            // allTypes.ForEach(t => schemaBuilder.AddType(t));
            
            schemaBuilder
                .AddQueryType<PayerQuery>()
                .AddMutationType<LoginService>()
                .AddType<PayerType>()
                .AddType<ObjectIdType>()
                .AddType<UInt64Type>();

            BsonClassMap.RegisterClassMap<Visit>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            return services;
        }
    }
}
