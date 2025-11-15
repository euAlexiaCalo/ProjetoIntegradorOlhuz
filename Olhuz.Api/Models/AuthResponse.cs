namespace Olhuz.Api.Models
{
    public class AuthResponse
    {
        // Indica o status geral da requisição. Se a requisição for bem-sucedida, este campo será 'true'. Se houver qualquer falha, este campo será 'false'.
        public bool Erro { get; set; }

        // Contém a mensagem detalhada do resultado.
        public string Message { get; set; } = string.Empty;
    }
}
