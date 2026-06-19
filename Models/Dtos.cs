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

    public class ChatRedirectDto
    {
        public int AgenteId { get; set; }
        public int CanalOrigemId { get; set; }
    }

    public class EnviarMensagemResultDto
    {
        public int AgenteId { get; set; }
        public int CanalOrigemId { get; set; }
        public string ConteudoUsuario { get; set; } = string.Empty;
        public string DataEnvioUsuario { get; set; } = string.Empty;
        public string NomeAgente { get; set; } = string.Empty;
        public string ConteudoAgente { get; set; } = string.Empty;
        public string DataEnvioAgente { get; set; } = string.Empty;
    }

    // ===== Resultados agregados de listagem (Index) =====

    public class UsuariosIndexDto
    {
        public List<UsuarioListItemDto> Usuarios { get; set; } = new();
        public int TotalAdmins { get; set; }
        public int TotalPerfis { get; set; }
    }

    public class AgentesIndexDto
    {
        public List<AgenteListItemDto> Agentes { get; set; } = new();
        public int TotalMensagens { get; set; }
        public int TotalMemorias { get; set; }
    }

    public class CanaisIndexDto
    {
        public List<CanalListItemDto> Canais { get; set; } = new();
        public int TotalSessoes { get; set; }
    }

    public class UsuarioLoginDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Perfil { get; set; } = string.Empty;
    }
}
