using GraphQLApi.GraphQLObjects.Queries;
using GraphQLApi.GraphQLObjects.Services;

namespace GraphQLApi.GraphQLObjects.Types;

[GraphQlType]
public class PayerType : ObjectType<PayerQuery>
{
    protected override void Configure(IObjectTypeDescriptor<PayerQuery> descriptor)
    {
        descriptor.Field(q => q.GetPayers(default, default, default))
            .Name("getPayers")
            .Argument("chhaId", a => a.Type<IntType>().DefaultValue(null))
            .Argument("chhaName", a => a.Type<StringType>().DefaultValue(null))
            .Argument("chhaInitial", a => a.Type<StringType>().DefaultValue(null))
            .ResolveWith<PayerService>(x => x.GetPayers(default, default, default));
    }
}
