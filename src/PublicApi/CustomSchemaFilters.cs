using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.eShopWeb.PublicApi;

public class CustomSchemaFilters : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        // Cast to concrete type to access Properties
        if (schema is not OpenApiSchema openApiSchema)
            return;

        var excludeProperties = new[] { "CorrelationId" };

        foreach (var prop in excludeProperties)
            if (openApiSchema.Properties.ContainsKey(prop))
                openApiSchema.Properties.Remove(prop);
    }
}
