using System;

namespace AgenticContextEngine.Models
{
    // Opcao generica usada em <select> de formularios (ex: perfis, categorias),
    // para nao precisar passar a entidade completa so para listar Id/Nome.
    public class OpcaoSelecaoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }

    // ===== Usuario =====

    public class UsuarioCreateDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public int PerfilAcessoId { get; set; }
    }

    public class UsuarioEditDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int PerfilAcessoId { get; set; }
        public bool Ativo { get; set; } = true;

        // Preenchido apenas quando o usuario quer trocar a senha; vazio = mantem a atual.
        public string? NovaSenha { get; set; }

        // Confirmacao obrigatoria da senha atual da conta sendo editada quando ha troca de senha.
        public string? SenhaAtual { get; set; }
    }

    // ===== Agente =====

    public class AgenteListItemDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string CategoriaNome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string? Instrucoes { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? CriadoPorUsuarioId { get; set; }
    }

    public class AgenteFormDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int CategoriaAgenteId { get; set; }
        public string? Descricao { get; set; }
        public string? Instrucoes { get; set; }
        public bool Ativo { get; set; } = true;
    }

    // ===== Canal =====

    public class CanalListItemDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? UrlSite { get; set; }
        public string? Descricao { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? CriadoPorUsuarioId { get; set; }
    }

    public class CanalFormDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? UrlSite { get; set; }
        public string? Descricao { get; set; }
        public bool Ativo { get; set; } = true;
    }

    // ===== Categoria =====

    public class CategoriaListItemDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? CriadoPorUsuarioId { get; set; }
    }

    public class CategoriaFormDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public bool Ativo { get; set; } = true;
    }

    // ===== Simulacao / Chat =====

    public class AgenteOpcaoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }

    public class CanalOpcaoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? UrlSite { get; set; }
    }

    public class MensagemDto
    {
        public int Id { get; set; }
        public string Conteudo { get; set; } = string.Empty;
        public string Remetente { get; set; } = string.Empty;
        public DateTime DataEnvio { get; set; }
    }

    public class SessaoRecenteDto
    {
        public int Id { get; set; }
        public string NomeAgente { get; set; } = string.Empty;
        public string NomeCanal { get; set; } = string.Empty;
        public DateTime DataInicio { get; set; }
    }
}
