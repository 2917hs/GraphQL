using GraphQLApi.GraphQLObjects.Queries;
using GraphQLApi.GraphQLObjects.Services;

namespace GraphQLApi.GraphQLObjects.Types;

[GraphQlType]
public sealed class VisitType : ObjectType<VisitQuery>
{
    protected override void Configure(IObjectTypeDescriptor<VisitQuery> descriptor)
    {
       descriptor.Field(q => q.GetAllVisits())
            .Name("getAllVisits")
            .ResolveWith<VisitService>(x => x.GetAllVisits(default));
    }
}
