using GraphQLApi.GraphQLObjects.Queries;
using GraphQLApi.GraphQLObjects.Services;

namespace GraphQLApi.GraphQLObjects.Types;

[GraphQlType]
public class OpenShiftType : ObjectType<Query>
{
    protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
    {
       descriptor.Field(q => q.GetOpenShiftAsync(default, default, default))
            .Name("getOpenShiftAsync")
            .Argument("globalProviderID", a => a.Type<NonNullType<StringType>>())
            .Argument("globalOfficeID", a => a.Type<NonNullType<StringType>>())
            .ResolveWith<OpenShiftService>(x => x.GetOpenShiftAsync(default, default, default))
            .Authorize();
    }
}
