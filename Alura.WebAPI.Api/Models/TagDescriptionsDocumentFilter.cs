using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Alura.ListaLeitura.Api.Models
{
    public class TagDescriptionsDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Tags = new[] 
            {
                new OpenApiTag { Name = "Livros", Description = "Consulta e mantém os livros." },
                new OpenApiTag { Name = "Listas", Description = "Consulta as listas de leitura." }
            };
        }
    }
}
