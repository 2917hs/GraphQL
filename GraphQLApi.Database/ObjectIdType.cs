using HotChocolate.Language;
using HotChocolate.Types;
using MongoDB.Bson;

namespace GraphQLApi.Database;

public class ObjectIdType : ScalarType<ObjectId, StringValueNode>
{
    public ObjectIdType() : base("ObjectId", BindingBehavior.Implicit) { }

    protected override ObjectId ParseLiteral(StringValueNode valueSyntax)
    {
        return ObjectId.Parse(valueSyntax.Value);
    }

    protected override StringValueNode ParseValue(ObjectId value)
    {
        return new StringValueNode(value.ToString());
    }

    public override IValueNode ParseResult(object? resultValue)
    {
        if (resultValue is ObjectId objectId)
        {
            return new StringValueNode(objectId.ToString());
        }
        throw new Exception("Invalid ObjectId result value.");
    }
}
