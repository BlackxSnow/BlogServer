using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Auth.Service;

public class SnakeCaseConvention : IPropertyAddedConvention
{
    public void ProcessPropertyAdded(IConventionPropertyBuilder propertyBuilder, IConventionContext<IConventionPropertyBuilder> context)
    {
        string columnName = string.Concat(propertyBuilder.Metadata.GetColumnName()
            .Select((c, i) => i > 0 && char.IsUpper(c) ? $"_{c}" : c.ToString())).ToLower();
        propertyBuilder.Metadata.SetColumnName(columnName);
    }
}