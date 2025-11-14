// --------------------------------------------------------------------------------
// [MODELO DE REQUISIÇÃO] RegisterRequestModel (DTO de Entrada (Data Transfer Object))
// --------------------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;

namespace Olhuz.Api.Models
{
    public class RegisterRequestModel
    {
        [Required(ErrorMessage = "Nome é obrigatório!")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório!")]
        [EmailAddress(ErrorMessage = "O email informado não é válido!")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data de Nascimento é obrigatória!")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória!")]
        public string PassWordHash { get; set; } = string.Empty;
    }
}