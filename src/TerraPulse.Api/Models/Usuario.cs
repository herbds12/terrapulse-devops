using System.ComponentModel.DataAnnotations;
using TerraPulse.Api.Models.Enums;

namespace TerraPulse.Api.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string SenhaHash { get; set; } = string.Empty;

    public PerfilUsuario Perfil { get; set; } = PerfilUsuario.Operador;

    public bool Ativo { get; set; } = true;

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}