using System;

namespace AgenticContextEngine.Models
{
    // Usado na listagem de Usuarios para nao expor SenhaHash nem a entidade completa a view.
    public class UsuarioListItemDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PerfilNome { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
