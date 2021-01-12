using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;

namespace Alura.ListaLeitura.Api.Models
{
    public class ErrorResponse
    {
        public int Codigo { get; set; }
        public string Mensagem { get; set; }
        public ErrorResponse InnerError { get; set; }
        public string[] Detalhes { get; set; }

        public static ErrorResponse From(Exception ex)
        {
            if(ex == null)
            {
                return null;
            }
            return new ErrorResponse
            {
                Codigo = ex.HResult,
                Mensagem = ex.Message,
                InnerError = ErrorResponse.From(ex.InnerException)
            };
        }

        public static object FromModelState(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(m => m.Errors);
            return new ErrorResponse
            {
                Codigo = 100,
                Mensagem = "Houve um erro(s) o envio da Requisição",
                Detalhes = erros.Select(e => e.ErrorMessage).ToArray(),
                InnerError = null,
            };
        }
    }
}
