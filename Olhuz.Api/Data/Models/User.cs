// --------------------------------------------------------------------------------
// [MODELO DE DADOS] ENTIDADE USER
// --------------------------------------------------------------------------------
// Propósito: Define a estrutura de dados (o "molde", conhecido como Modelagem de Entidade) para um usuário no sistema.
// Ele mapeia a tabela 'User' no banco de dados e garante que a aplicação saiba quais informações compõem um usuário.
// --------------------------------------------------------------------------------

// Importa as bibliotecas necessárias para manipulação de datas
using System;
// Arquivo que define o modelo de dados do usuário
namespace Olhuz.Api.Data.
    Models
{
    // Classe que representa um usuário no sistema
    public class User
    {
        // Identificador único do usuário
        public int UsuarioId { get; set; }

        // Nome completo do usuário que não pode ser nulo e inicia como string vazia
        public string NomeCompleto { get; set; } = string.Empty;

        // Email do usuário que não pode ser nulo e inicia como string vazia
        public string Email { get; set; } = string.Empty;

        // Armazena a senha do usuário
        public string PassWordHash { get; set; } = string.Empty;

        // Armazena a hash temporária para recuperação de senha
        public string HashPass { get; set; } = string.Empty;

        // Data de nascimento do usuário
        public DateTime DataNascimento { get; set; }

        // Data e hora de criação do usuário
        public DateTime CreatedAt { get; set; }

        // Data e hora da última atualização do usuário, pode ser nulo
        public DateTime? DateUp { get; set; }

        // Identificador do status atual do usuário
        public int StatusId { get; set; }
    }
}