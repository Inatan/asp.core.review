using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.ListaLeitura.HttpClients
{
    public class LivroApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _accessor;

        public LivroApiClient(HttpClient httpClient, IHttpContextAccessor accessor)
        {
            _httpClient = httpClient;
            _accessor = accessor;
        }

        private void AddBaererToken()
        {
            var token = _accessor.HttpContext.User.Claims.First(c => c.Type == "Token").Value;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        }

        public async Task<LivroApi> GetLivroAsync(int id)
        {
            AddBaererToken();
            HttpResponseMessage resposta = await _httpClient.GetAsync($"livros/{id}");

            //Força a resposta for da famiília 200 caso contrário da exceção
            resposta.EnsureSuccessStatusCode();

            return await resposta.Content.ReadAsAsync<LivroApi>();
        }

        public async Task<byte[]> GetCapaLivroAsync(int id)
        {
            AddBaererToken();
            HttpResponseMessage resposta = await _httpClient.GetAsync($"livros/{id}/capa");

            //Força a resposta for da famiília 200 caso contrário da exceção
            resposta.EnsureSuccessStatusCode();

            return await resposta.Content.ReadAsByteArrayAsync();
        }       

        public async Task DeleteLivroAsync(int id)
        {
            AddBaererToken();
            HttpResponseMessage resposta = await _httpClient.DeleteAsync($"livros/{id}");

            //Força a resposta for da famiília 200 caso contrário da exceção
            resposta.EnsureSuccessStatusCode();

            await resposta.Content.ReadAsByteArrayAsync();
        }

        public async Task<Lista> GetListaLeituraAsync(TipoListaLeitura tipo)
        {
            AddBaererToken();

            HttpResponseMessage resposta = await _httpClient.GetAsync($"ListasLeitura/{tipo}");

            //Força a resposta for da famiília 200 caso contrário da exceção
            resposta.EnsureSuccessStatusCode();

            return await resposta.Content.ReadAsAsync<Lista>();
        }

        public async Task PostLivroAsync(LivroUpload model)
        {
            AddBaererToken();
            HttpContent content = CreateMultipartFormDataContent(model);
            HttpResponseMessage resposta = await _httpClient.PostAsync($"livros",content);

            //Força a resposta for da famiília 200 caso contrário da exceção
            resposta.EnsureSuccessStatusCode();

            await resposta.Content.ReadAsByteArrayAsync();
        }

        public async Task PutLivroAsync(LivroUpload model)
        {
            AddBaererToken();
            HttpContent content = CreateMultipartFormDataContent(model);
            HttpResponseMessage resposta = await _httpClient.PutAsync($"livros", content);

            //Força a resposta for da famiília 200 caso contrário da exceção
            resposta.EnsureSuccessStatusCode();

            await resposta.Content.ReadAsByteArrayAsync();
        }

        private string EnvolveComAspasDuplas(string valor)
        {
            return $"\"{valor}\"";
        }

        private HttpContent CreateMultipartFormDataContent(LivroUpload model)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.Titulo), EnvolveComAspasDuplas("titulo")); 
            content.Add(new StringContent(model.Lista.ParaString()), EnvolveComAspasDuplas("lista"));
            if (!string.IsNullOrEmpty(model.Subtitulo))
            {
                content.Add(new StringContent(model.Subtitulo), EnvolveComAspasDuplas("subtitulo")); 
            }
            if (!string.IsNullOrEmpty(model.Autor))
            {
                content.Add(new StringContent(model.Autor), EnvolveComAspasDuplas("autor")); 
            }
            if (!string.IsNullOrEmpty(model.Resumo))
            {
                content.Add(new StringContent(model.Resumo), EnvolveComAspasDuplas("resumo")); 
            }
            
            if(model.Id >0)
            {
                content.Add(new StringContent(model.Id.ToString()), EnvolveComAspasDuplas("id")); 
            }
            if(model.Capa != null)
            {
                var imageContent = new ByteArrayContent(model.Capa.ConvertToBytes());
                imageContent.Headers.Add("content-type", "image/png");
                content.Add(imageContent, EnvolveComAspasDuplas("capa"), EnvolveComAspasDuplas("capa.png"));
            }
            return content;
        }
    }
}
