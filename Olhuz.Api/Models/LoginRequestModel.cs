// --------------------------------------------------------------------------------
// [MODELO DE REQUISIÇÃO] LoginRequestModel (DTO de Entrada para Login)
// --------------------------------------------------------------------------------
using System;
using System.ComponentModel.DataAnnotations;

namespace Olhuz.Api.Models
{
    public class LoginRequestModel
    {
        [Required(ErrorMessage = "O campo 'email' é obrigatório.")] // Validação para campo obrigatório
        [EmailAddress(ErrorMessage = "O email informado não é válido.")] // Validação para formato de email
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo 'senha' é obrigatório.")] // Validação para campo obrigatório
        public string PassWordHash { get; set; } = string.Empty;
    }
}