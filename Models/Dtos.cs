using System;

namespace AgenticContextEngine.Models
{
  
    public class OpcaoSelecaoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }

    

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

        
        public string? NovaSenha { get; set; }

        
        public string? SenhaAtual { get; set; }
    }


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
