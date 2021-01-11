using Alura.ListaLeitura.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.WebAPI.Api.Models
{
    public static class LivroPaginacaoExtensions
    {
        public static IQueryable<Livro> AplicaPaginacao(this IQueryable<Livro> query, LivroPaginacao paginacao)
        {
            if (paginacao != null)
            {
                query = query.Take(paginacao.Tamanho).Skip(paginacao.Pagina);
            }
            return query;
        }
    }

    public static class LivroPaginadoExtensions
    {
        public static LivroPaginado ToLivroPaginado(this IQueryable<LivroApi> query, LivroPaginacao paginacao)
        {
            int totalItens = query.Count();
            int totalPaginas = (int)Math.Ceiling(totalItens / (decimal)paginacao.Tamanho);
            return new LivroPaginado()
            {
                Total = totalItens,
                TotalPaginas = totalPaginas,
                NumeroPagina = paginacao.Pagina,
                TamanhoPagina = paginacao.Tamanho,
                Resultado = query.Skip(paginacao.Tamanho * (paginacao.Pagina -1))
                    .Take(paginacao.Tamanho)
                    .ToList(),
                Anterior = (paginacao.Pagina > 1) ? 
                    $"livros?Tamanho={paginacao.Tamanho}&Pagina={paginacao.Pagina-1}" : "",
                Proximo = (paginacao.Pagina < totalPaginas) ? "" :
                    $"livros?Tamanho={paginacao.Tamanho}&Pagina={paginacao.Pagina + 1}",
            };
        }
    }


    

    public class LivroPaginado
    {
        public int Total { get; set; }
        public int TamanhoPagina { get; set; }
        public int TotalPaginas { get; set; }
        public int NumeroPagina { get; set; }
        public IList<LivroApi> Resultado { get; set; }
        public string Anterior { get; set; }
        public string Proximo { get; set; }
    }


    public class LivroPaginacao
    {
        public int Tamanho { get; set; } = 1;
        public int Pagina { get; set; } = 10;
    }
}
