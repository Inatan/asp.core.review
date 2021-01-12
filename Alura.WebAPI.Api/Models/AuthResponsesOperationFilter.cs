using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Alura.ListaLeitura.Api.Models
{
    public class AuthResponsesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Responses.Add(
                "401",
                new OpenApiResponse { Description = "Unauthorized" });
        }
    }
}
