using Olhuz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Olhuz.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7058";

        public AuthService()
        {
            // Inicializa o HttpClient com instância única
            _httpClient = new HttpClient();
        }

        // Método para cadastrar um novo usuário com o modelo CadastroRequest e passar dentro dos dadosUsuario
        public async Task<AuthResponse> CadastrarUsuarioAsync(CadastroRequest dadosUsuario)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/Account/register", dadosUsuario);

                AuthResponse? result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return result!;

            }
            catch (Exception erro)
            {
                return new AuthResponse
                {
                    Erro = true,
                    Message = "Seu cadastro não foi realizado. Verifique os dados e tente novamente."
                };
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest dadosLogin)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/Account/login", dadosLogin);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    return result!;
                }

                var erro = await response.Content.ReadAsStringAsync();
                throw new Exception(erro);
            }
            catch (Exception erro)
            {
                return new AuthResponse
                {
                    Erro = true,
                    Message = $"Erro ao realizar login. Verifique os dados digitados"
                };
            }
        }
    }
}