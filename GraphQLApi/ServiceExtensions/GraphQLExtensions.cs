using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using System;
using System.Linq;
using System.Reflection;
using GraphQLApi.Database;
using GraphQLApi.Database.Models;
using GraphQLApi.GraphQLObjects;
using GraphQLApi.GraphQLObjects.Queries;
using GraphQLApi.GraphQLObjects.Services;
using GraphQLApi.GraphQLObjects.Types;
using GraphQLApi.Middlewares;
using HotChocolate.Execution.Configuration;

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

            var assembly = Assembly.GetExecutingAssembly();
            var allTypes = assembly.GetTypes()
                .Where(t => t.GetCustomAttributes(typeof(GraphQlTypeAttribute), false).Any() &&
                            t is { IsAbstract: false, IsInterface: false })
                .ToList();
            allTypes.ForEach(t => schemaBuilder.AddType(t));
            
            schemaBuilder.AddGraphQLTypes();
                // .AddQueryType<HealthQuery>()
                // .AddTypeExtension<PayerQuery>()
                // .AddMutationType<LoginService>()
                // .AddType<ObjectIdType>()
                // .AddType<UInt64Type>();

            BsonClassMap.RegisterClassMap<Visit>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            return services;
        }
    }
}

public static class IRequestExecutorBuilderExtensions
{
    public static void AddGraphQLTypes(this IRequestExecutorBuilder requestExecutorBuilder)
    {
        requestExecutorBuilder
            // .AddMutationType(d => d.Name("Mutation"))
            .AddQueryType<Query>();

        // var mutations = typeof(EntityMutation).GetAllSubTypes().ToList();
        // mutations.ForEach(t => requestExecutorBuilder.AddTypeExtension(t));

        var queries = typeof(Query).GetAllSubTypes().ToList();
        queries.ForEach(t => requestExecutorBuilder.AddTypeExtension(t));
    }
}

public static class TypeExtensions
{
    public static IEnumerable<Type> GetAllSubTypes(this Type type)
    {
        var allTypes = type.Assembly.GetTypes();

        var types = from x in allTypes
            where x.BaseType != null && x.BaseType.Name == type.Name
            select x;

        return types;
    }
}
