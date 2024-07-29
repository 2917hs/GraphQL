using GraphQLApi.GraphQLObjects.Queries;
using GraphQLApi.GraphQLObjects.Services;

namespace GraphQLApi.GraphQLObjects.Types;

[GraphQlType]
public sealed class VisitType : ObjectType<Query>
{
    protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
    {
       descriptor.Field(q => q.GetAllVisits())
            .Name("getAllVisits")
            .ResolveWith<VisitService>(x => x.GetAllVisits(default));
    }
}
