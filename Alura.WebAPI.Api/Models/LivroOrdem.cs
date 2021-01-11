using Alura.ListaLeitura.Modelos;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Alura.WebAPI.Api.Models
{
    public static class LivroOrdemExtensions
    {
        public static IQueryable<Livro> AplicaOrdem(this IQueryable<Livro> query, LivroOrdem ordem)
        {
            if (ordem != null)
            {
                if (!string.IsNullOrEmpty(ordem.OrdernarPor))
                    query = query.OrderBy(ordem.OrdernarPor);
            }
            return query;
        }
    }

    public class LivroOrdem
    {
        public string OrdernarPor { get; set; }
    }
}
