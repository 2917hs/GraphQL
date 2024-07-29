using GraphQLApi.GraphQLObjects.Queries;
using GraphQLApi.GraphQLObjects.Services;

namespace GraphQLApi.GraphQLObjects.Types;

[GraphQlType]
public class HealthType : ObjectType<Query>
{
    protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
    {
        descriptor.Field(q => q.GetHealthAsync(default, default))
            .Name("getHealthAsync")
            .Argument("eTag", a => a.Type<StringType>())
            .ResolveWith<HealthService>(x => x.GetHealthAsync(default, default))
            .Authorize();
    }
}