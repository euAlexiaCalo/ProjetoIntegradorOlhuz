// --------------------------------------------------------------------------------
// [CONTROLADOR DE API] AccountController - Versão com Lógica Real e Autenticação
// --------------------------------------------------------------------------------

// Fornece os recursos necessários para criar controladores de endpoints em API
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
// Usado para capturar erros específicos relacionados ao SQL Server
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using Olhuz.Api.Models;
// Importa o repositório para operações relacionadas ao usuário no banco de dados
using Olhuz.Api.Repositories;
// Importa as bibliotecas necessárias com tipos básicos do .NET
using System;
using System.Linq.Expressions;
// ---- Adicionada para a criptografia em hash256
using System.Security.Cryptography;
using System.Text;
// Importa a biblioteca para trabalhar com tarefas assíncronas
using System.Threading.Tasks;
// Cria um apelido para o modelo de dados do usuário para evitar conflitos entre modelos com o mesmo nome
//Faz a referência ao modelo de dados do usuário no banco de dados
using DbUsuario = Olhuz.Api.Data.Models.User;

namespace Olhuz.Api.Controllers
{
    // Define que esta classe é um controlador de API
    [ApiController]

    // Define a rota base para chamar este controlador
    [Route("api/[controller]")]

    // Herda as funcionalidades básicas para controladores de API do proprio framework ASP.NET Core ex: ModelState, BadRequest(), Ok(), NotFound(), etc.
    public class AccountController : ControllerBase
    {
        // Repositório para operações relacionadas ao usuário no banco de dados
        private readonly UserRepository _userRepository;

        // Construtor que recebe o repositório de usuário via injeção de dependência (DI)
        public AccountController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // =================================
        // MÉTODO DE REGISTRO DE USUÁRIO
        // =================================

        // Define um endpoint POST para registrar um novo usuário
        [HttpPost("register")]

        // [FromBody] indica que os dados (model) vêm do corpo da requisição HTTP (geralmente JSON).
        public async Task<IActionResult> Register([FromBody] RegisterRequestModel model)
        {
            // Verificação se email já está em uso
            try
            {
                // Usa o método existente no repositório para verificar se o email já existe
                var existingUser = await _userRepository.GetUserByEmailAsync(model.Email);

                if (existingUser != null)
                {
                    // Se o usuário for encontrado, retorna a resposta de conflito imediatamente (409)
                    return Conflict(new { message = "Este email já está em uso!" });
                }
            }
            catch (Exception ex)
            {
                // Erro ao tentar consultar o banco de dados (ex: problema de conexão)
                return StatusCode(500, new { message = $"Erro ao verificar e-mail: {ex.Message}" });
            }

            // Tenta executar o bloco de código para criar um novo usuário
            try
            {
                // Chave de API fixa (salt/pepper) para aumentar a complexidade e segurança do hash da senha
                // Isso previne ataques de rainbow table, pois o hash nunca é o mesmo entre aplicações.
                string apiKey = "forcaPara_todos";

                // Aplica o Hash SHA-256 (rápido, mas unidirecional) à senha pura recebida.
                // Transforma a senha em um código secreto de letras e números.
                string PassWordHash = ComputeSha256Hash(model.PassWordHash);
                // Aplica o Hash SHA-256 ao e-mail. Isso ajuda a "anonimizar" o e-mail na mistura do hash final.
                // Transforma a senha em um código secreto de letras e números.
                string Email = ComputeSha256Hash(model.Email);

                // Pega: Dia Mês Ano HoraAtual no formato "ddMMyyyyHHmmss" para usar em hash de recuperação de senha futuramente
                string dataString = DateTime.Now.ToString("ddMMyyyyHHmmss");

                // Cria o hash da senha de fato: Combina o hash da senha, o hash do e-mail e a chave fixa (apiKey).
                string PassWordHash2 = PassWordHash + Email + apiKey;

                // Cria uma string base para ser usada futuramente na verificação de tokens de recuperação.
                String HashPass2 = Email + PassWordHash + dataString + apiKey;

                // Criptografia da senha utilizando BCrypt
                // usando somente uma criptografia: string passWordHash = BCrypt.Net.BCrypt.HashPassword(model./PassWordHash);
                // BCrypt é um algoritmo lento por design (função de derivação de chave), o que é ideal.
                // Ele adiciona um salt automaticamente, tornando-o o padrão para hashing de senhas.
                string passWordHash = BCrypt.Net.BCrypt.HashPassword(PassWordHash2);

                // Criptografia do hash para recuperação de senha utilizando BCrypt
                // Esta hash será usada para verificar o token de recuperação, se implementado.
                string HashPass = BCrypt.Net.BCrypt.HashPassword(HashPass2);

                // Cria um novo objeto DbUsuario com os dados recebidos no modelo de requisição para salvar no banco de dados
                var novoUser = new DbUsuario
                {
                    // Nome completo vindo do corpo da requisição
                    NomeCompleto = model.NomeCompleto,
                    // Email informado no registro
                    Email = model.Email,
                    DataNascimento = model.DataNascimento,
                    // Utilizamos no momento de teste: PassWordHash = model.PassWordHash,

                    // Armazenamos o hash da senha para maior segurança
                    /* Se fosse usar Hash256:
                    // Criptografia com Hash256
                    // Esta linha chamará o método ComputeSha256Hash para calcular o hash da senha fornecida
                    PassWordHash = ComputeSha256Hash(model.PassWordHash),
                    */
                    // Senha criptografada com BCrypt (o valor complexo gerado acima)
                    PassWordHash = passWordHash,
                    // Armazena o hash criptografado para recuperação de senha
                    HashPass = HashPass,

                    // Data atual para o controle de criação do usuário
                    CreatedAt = DateTime.Now,
                    // data atual para o controle de atualização do usuário
                    DateUp = DateTime.Now,
                    StatusId = 2
                };

                // Chama o repositório para criar o novo usuário no banco de dados
                // 'await' garante que a thread da API não seja bloqueada enquanto o banco processa a inserção.
                await _userRepository.CreateUserAsync(novoUser);

                // Retorna uma resposta de sucesso (200 OK) com uma mensagem em JSON
                return Ok(new { message = "Usuário registrado com sucesso" });
            }
            // Tratamento de erros específicos de SQL Server de duplicidade (chave única) no email já existente no banco de dados
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                return Conflict(new { message = "Este email já está em uso!" });
            }
            // Captura outros erros genéricos e retorna um status 500 (erro interno do servidor) com a mensagem de erro
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"{ex.Message}" });
            }
        }

        // =================================
        // MÉTODO DE LOGIN DE USUÁRIO
        // =================================

        [HttpPost("login")]

        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            // 1. Repositório busca o usuário no banco de dados pelo email
            var user = await _userRepository.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                // Se o usuário não for encontrado, retorna 401 Unauthorized
                return Unauthorized(new { message = "Email ou senha inválidos" });
            }
            
            // 2. Recria a hash de login exatamente como no registro
            string PassWordHash = ComputeSha256Hash(model.PassWordHash);
            string Email = ComputeSha256Hash(model.Email);
            // Cria a palavra chave para criptografia
            string apiKey = "forcaPara_todos";
            // Criando a string para criptografia
            string PassWordHash2 = PassWordHash + Email + apiKey;
            
            // Variável para armazenar o resultado da verificação da senha
            bool isPasswordValid;

            try
            {
                // Verifica se a senha fornecida corresponde ao hash armazenado usando BCrypt
                isPasswordValid = BCrypt.Net.BCrypt.Verify(PassWordHash2, user.PassWordHash);
            }
            catch (Exception)
            {
                // Em caso de erro na verificação, considera a senha inválida
                isPasswordValid = false;
            }

            // Se a senha for inválida, retorna 401 Unauthorized
            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "Email ou senha inválidos" });
            }

            // Retorna 200 OK se o login for bem-sucedido
            return Ok(new { message = "Login realizado com sucesso" });
        }

        // Se for usar Hash256:
        // ----- Método para calcular o hash SHA-256 de uma string -----
        private string ComputeSha256Hash(string rawData)
        {
            // Cria uma instância do SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Computa o hash de entrada da string
                // Retorna o resultado como um array de bytes
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Converte o array de bytes em uma string hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}




// --------------------------------------------------------------------------------
// [CONTROLADOR DE API] AccountController - Versão de Teste (Hardcoded)
// --------------------------------------------------------------------------------
// Propósito:
// Este controlador é o 'ponto de entrada' da API responsável por gerenciar
// operações de conta do usuário, como Login, Registro e recuperação de senha.
//
// Detalhes de Roteamento (Rotas):
// - [ApiController]: Identifica a classe como um controlador de API do ASP.NET Core.
// - [Route("api/[controller]")]: Define a rota base como "/api/Account".
//
// Método Implementado (Login):
// - [HttpPost("Login")]: Mapeia o método para receber requisições POST no caminho
//   "/api/Account/Login".
// - [FromBody]: Indica que os dados do usuário (Email e Senha) virão no corpo
//   (body) da requisição HTTP.
//
// ATENÇÃO: A lógica de login atual é HARDCODED (fixa) e serve apenas para DEMONSTRAÇÃO
// do endpoint. No futuro, ela será substituída pela lógica de autenticação real
// (consulta ao banco de dados, verificação de hash de senha e geração de Token JWT).
// --------------------------------------------------------------------------------

/* Teste para testar se a estrutura do controlador de API está funcionando corretamente
/// Importa as bibliotecas necessárias para controladores de API
using Microsoft.AspNetCore.Mvc;

// Importa o modelo de requisição de login
using ProjetoMakers.Api.Models;

namespace ProjetoMakers.Api.Controllers
{
    // Está classe é um controlador de API
    [ApiController]

    // Define a rota base para este controlador
    [Route("api/[controller]")]

    // Herdar de ControllerBase para funcionalidades de API
    // Controlador responsável por gerenciar operações relacionadas à conta do usuário, como login e registro
    public class AccountController : ControllerBase
    {
        // Define que este método responde a requisições POST(criar) em "api/Account/Login"
        [HttpPost("Login")]

        // Recebe o modelo de requisição no corpo da requisição
        public IActionResult Login([FromBody] LoginRequestModel model)
        {
            // por enquanto, vamos utilizar um usuário fixo para demonstração de endpoint
            if(model.Email == "usuario@teste.com.br" && model.Password == "Senha@123")
            {
                // Se a autenticação for bem-sucedida, retorna um status 200 OK com uma mensagem de sucesso
                // Poderiámos retornar um token JWT (autenticação do usuário no sistema), na aplicação no futuro
                return Ok(new { message = "Login realizado com sucesso" });
            }

            // Retorna um status 401 Unauthorized com uma mensagem de erro
            return Unauthorized(new { message = "Email ou senha inválidos" });
        }
    }
}*/