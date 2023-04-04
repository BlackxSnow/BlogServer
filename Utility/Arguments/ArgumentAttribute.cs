using System.Data.SqlTypes;

namespace Utility.Arguments;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ArgumentAttribute : Attribute
{
    public readonly string Id;
    public readonly Type ContextType;
    
    public ArgumentAttribute(string id, Type contextType)
    {
        Id = id;
        ContextType = contextType;
    }

    public ArgumentAttribute(string id)
    {
        Id = id;
        ContextType = typeof(object);
    }
}