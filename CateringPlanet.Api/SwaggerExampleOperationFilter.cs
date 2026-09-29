using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CateringPlanet.Api;

/// <summary>
/// Adds development-time example request bodies to the Swagger UI.
/// </summary>
public class SwaggerExampleOperationFilter : IOperationFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Only the login endpoint gets a pre-filled example (seeded admin account).
        if (!string.Equals(context.ApiDescription.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(context.ApiDescription.RelativePath, "api/auth/login", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var example = new OpenApiObject
        {
            ["email"] = new OpenApiString("luka.popovic@cateringplanet.rs"),
            ["password"] = new OpenApiString("Catering123!")
        };

        operation.RequestBody ??= new OpenApiRequestBody();
        operation.RequestBody.Content ??= new Dictionary<string, OpenApiMediaType>();
        operation.RequestBody.Content["application/json"] ??= new OpenApiMediaType();
        operation.RequestBody.Content["application/json"].Example = example;
    }
}
