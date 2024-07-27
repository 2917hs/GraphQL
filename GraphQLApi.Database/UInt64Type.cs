using HotChocolate.Language;
using HotChocolate.Types;
namespace GraphQLApi.Database;

public class UInt64Type : ScalarType<ulong, IntValueNode>
{
    public UInt64Type() : base("UInt64", BindingBehavior.Explicit) { }

    protected override ulong ParseLiteral(IntValueNode valueSyntax)
    {
        if (valueSyntax is null)
        {
            throw new ArgumentNullException(nameof(valueSyntax));
        }

        if (ulong.TryParse(valueSyntax.Value, out var result))
        {
            return result;
        }

        throw new Exception($"Unable to parse '{valueSyntax.Value}' as UInt64.");
    }

    protected override IntValueNode ParseValue(ulong value)
    {
        return new IntValueNode(value);
    }

    public override IValueNode ParseResult(object? resultValue)
    {
        if (resultValue is ulong ulongValue)
        {
            return new IntValueNode(ulongValue);
        }
        throw new Exception("Invalid UInt64 result value.");
    }
}
