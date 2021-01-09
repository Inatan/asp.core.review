using Alura.ListaLeitura.Seguranca;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.HttpClients
{
    public class LoginResult
    {
        public bool Succeeded { get; set; }
        public string Token { get; set; }

    }
    public class AuthApiClient
    {
        private readonly HttpClient _httpClients;

        public AuthApiClient(HttpClient httpClients)
        {
            _httpClients = httpClients;
        }

        public async Task<LoginResult> PostLoginAsync(LoginModel model)
        {
            var resposta = await _httpClients.PostAsJsonAsync("login", model);
            resposta.EnsureSuccessStatusCode();
            return new LoginResult
            {
                Succeeded = resposta.IsSuccessStatusCode,
                Token = await resposta.Content.ReadAsStringAsync()
            };
        }
    }
}
