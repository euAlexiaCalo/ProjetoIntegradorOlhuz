using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olhuz.Models
{
    public class Usuario
    {
        // Chave Primária
        public int Id { get; set; }

        // Dados Pessoais
        public string NomeCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string PassWordHash { get; set; } = string.Empty;
        public string HashPass { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DateUp { get; set; }

    }
}