// Importa a biblioteca para trabalhar com SQL Server
using Microsoft.Data.SqlClient;

// Importa a classe para configuração de aplicativos dpe .NET (IConfiguration), usada para acessar strings de conexão
using Microsoft.Extensions.Configuration;

// Importa o modelo de dados do usuário
using Olhuz.Api.Data.Models;

// Importa as bibliotecas necessárias para manipulação de datas
using System;

// Importa a biblioteca para trabalhar com tarefas assíncronas
using System.Threading.Tasks;

namespace Olhuz.Api.Repositories
{
    public class UserRepository
    {
        // Armazenará a string de conexão com o banco de dados

        private readonly string _connectionString = string.Empty;

        // Construtor que recebe a string de conexão do banco de dados
        public UserRepository(IConfiguration configuration)
        {
            // Buscar a string de conexão no arquivo de configuração
            // Obtém do arquivo appsettings.json a string de conexão chamada "DefaultConnection"
            // Caso não seja encontrada, lança uma exceção ArgumentNullException, impedindo que o código continue sem uma conexão válida
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Connection string 'DefaultConnection' not found.");
        }

        // Método público assíncrono para criar um novo usuário no banco de dados
        // Recebe um objeto do tipo User contendo os dados do usuário a ser criado
        // Retorna uma tarefa (Task) que representa a operação assíncrona

        public async Task CreateUserAsync(User user)
        {
            // Estabelece uma conexão com o banco de dados usando a string de conexão fornecida
            // O using garante que a conexão será fechada e descartada corretamente após o uso
            using (var connection = new SqlConnection(_connectionString))
            {
                // Abre a conexão com o banco de dados de forma assíncrona (sem travar a thread principal)
                await connection.OpenAsync();

                // Comando SQL para inserir um novo usuário
                // O campo CreatedAt é preenchido automaticamente pelo banco de dados com a data e hora atual
                var commandText = @"INSERT INTO Usuarios (NomeCompleto, Email, DataNascimento, PassWordHash, HashPass, DataCriacao, DataAtualizacao, StatusId)
                                    VALUES (@NomeCompleto, @Email, @DataNascimento, @PassWordHash, @HashPass, @CreatedAt, @DateUp, @StatusId)";



                // Cria um comando SQL com o texto do comando e a conexão aberta
                // Também está dentro de um using para garantir o descarte correto após o uso
                using (var command = new SqlCommand(commandText, connection))
                {
                    // Os protetores contra SQL Injection são automaticamente aplicados ao usar parâmetros
                    // Pega o atributo do objeto user e atribui ao parâmetro correspondente no comando SQL
                    command.Parameters.AddWithValue("@NomeCompleto", user.NomeCompleto);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@DataNascimento", user.DataNascimento);
                    command.Parameters.AddWithValue("@PassWordHash", user.PassWordHash);
                    command.Parameters.AddWithValue("@HashPass", user.HashPass);
                    command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
                    // Trata a possibilidade de DateUp ser nulo, inserindo DBNull.Value se for o caso
                    command.Parameters.AddWithValue("@DateUp", (object)user.DateUp ?? DBNull.Value);
                    command.Parameters.AddWithValue("@StatusId", user.StatusId);

                    // Executa o comando de forma assíncrona para inserir o usuário no banco de dados
                    // Como é um 'INSERT', não retorna resultados, por isso usamos ExecuteNonQueryAsync
                    await command.ExecuteNonQueryAsync();

                }
            }
        }

        // ============================================
        // MISSÃO 2: PROCURAR UM REGISTRO (Login de Usuário)
        // ============================================

        // O Controller de Conta (AccountController) irá chamar este método para buscar um usuário pelo email fornecido durante o login
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            // Estabelece uma conexão com o banco de dados usando a string de conexão fornecida
            using (var connection = new SqlConnection(_connectionString))
            {
                // Abre a conexão com o banco de dados de forma assíncrona
                await connection.OpenAsync();
                // Comando SQL para selecionar um usuário pelo email
                var commandText = @"SELECT TOP 1 * FROM dbo.Usuarios Where Email = @Email";
                using (var command = new SqlCommand(commandText, connection))
                {
                    // Adiciona o parâmetro de email ao comando SQL para prevenir SQL Injection
                    command.Parameters.AddWithValue("@Email", email);
                    // Executa o comando e pede para LER o resultado
                    using (var reader = await command.ExecuteReaderAsync()) // Executa o comando e obtém um leitor de dados
                    {
                        // Tenta ler o primeiro registro retornado pela consulta
                        if (await reader.ReadAsync())
                        {
                            // Mapeia os dados do registro para um objeto User e o retorna com os valores lidos das colunas do banco de dados
                            return new User
                            {
                                UsuarioId = reader.GetInt32(reader.GetOrdinal("UsuarioId")),
                                NomeCompleto = reader.GetString(reader.GetOrdinal("NomeCompleto")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                DataNascimento = reader.GetDateTime(reader.GetOrdinal("DataNascimento")),
                                PassWordHash = reader.GetString(reader.GetOrdinal("PassWordHash")),
                                HashPass = reader.GetString(reader.GetOrdinal("HashPass")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("DataCriacao")),
                                DateUp = reader.IsDBNull(reader.GetOrdinal("DataAtualizacao")) ? null : reader.GetDateTime(reader.GetOrdinal("DataAtualizacao")),
                                StatusId = reader.GetInt32(reader.GetOrdinal("StatusId"))
                            };
                        }
                    }
                }
            }
            // Retorna null se o usuário não for encontrado
            return null;
        }
    }
}