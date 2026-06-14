// =====================================================
// AGENTIC CONTEXT ENGINE - Models + Estrutura OO
// Herança, Polimorfismo e Interfaces
// =====================================================

// =====================================================
// INTERFACES
// =====================================================

namespace AgenticContextEngine.Core.Interfaces
{
    public interface IAgente
    {
        string Nome { get; set; }
        string ProcessarMensagem(string mensagemUsuario);
        string GerarRespostaPadrao();
    }

    public interface IContextoMemoria
    {
        void SalvarContexto(string chave, string valor);
        string RecuperarContexto(string chave);
        List<KeyValuePair<string, string>> ListarContexto();
    }

    public interface IEstatistica
    {
        int TotalSessoes { get; }
        int TotalMensagens { get; }
        void IncrementarSessao();
        void IncrementarMensagem();
        string GerarResumo();
    }
}

// =====================================================
// MODELS - Entidades do Banco (sem herança, EF Core ok)
// =====================================================

namespace AgenticContextEngine.Models
{
    public class PerfilAcesso
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }

    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public int PerfilAcessoId { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public PerfilAcesso? PerfilAcesso { get; set; }
        public ICollection<SessaoAtendimento> Sessoes { get; set; } = new List<SessaoAtendimento>();
    }

    public class CategoriaAgente
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public ICollection<Agente> Agentes { get; set; } = new List<Agente>();
    }

    // Model simples para o EF Core — sem herança abstrata
    public class Agente
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public int CategoriaAgenteId { get; set; }
        public string? Instrucoes { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public CategoriaAgente? CategoriaAgente { get; set; }
    }

    public class CanalOrigem
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? UrlSite { get; set; }
        public string? Descricao { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }

    public class SessaoAtendimento
    {
        public int Id { get; set; }
        public int AgenteId { get; set; }
        public int CanalOrigemId { get; set; }
        public int? UsuarioId { get; set; }
        public string? Titulo { get; set; }
        public string Status { get; set; } = "Ativa";
        public DateTime DataInicio { get; set; } = DateTime.Now;
        public DateTime? DataFim { get; set; }

        public Agente? Agente { get; set; }
        public CanalOrigem? CanalOrigem { get; set; }
        public Usuario? Usuario { get; set; }
        public ICollection<Mensagem> Mensagens { get; set; } = new List<Mensagem>();
    }

    public class Mensagem
    {
        public int Id { get; set; }
        public int SessaoAtendimentoId { get; set; }
        public string Conteudo { get; set; } = string.Empty;
        public string Remetente { get; set; } = string.Empty;
        public DateTime DataEnvio { get; set; } = DateTime.Now;

        public SessaoAtendimento? SessaoAtendimento { get; set; }
    }

    public class ContextoMemoria
    {
        public int Id { get; set; }
        public int SessaoAtendimentoId { get; set; }
        public int AgenteId { get; set; }
        public string ChaveContexto { get; set; } = string.Empty;
        public string ValorContexto { get; set; } = string.Empty;
        public DateTime DataRegistro { get; set; } = DateTime.Now;

        public SessaoAtendimento? SessaoAtendimento { get; set; }
        public Agente? Agente { get; set; }
    }

    public class LogAuditoria
    {
        public int Id { get; set; }
        public int? UsuarioId { get; set; }
        public string Acao { get; set; } = string.Empty;
        public string? Entidade { get; set; }
        public int? EntidadeId { get; set; }
        public string? Detalhes { get; set; }
        public DateTime DataAcao { get; set; } = DateTime.Now;

        public Usuario? Usuario { get; set; }
    }

    public class EstatisticaAcesso
    {
        public int Id { get; set; }
        public int AgenteId { get; set; }
        public int CanalOrigemId { get; set; }
        public int TotalSessoes { get; set; } = 0;
        public int TotalMensagens { get; set; } = 0;
        public DateTime DataReferencia { get; set; } = DateTime.Today;

        public Agente? Agente { get; set; }
        public CanalOrigem? CanalOrigem { get; set; }
    }
}

// =====================================================
// CAMADA DE NEGÓCIO - Hierarquia OO separada do EF Core
// =====================================================

namespace AgenticContextEngine.Business
{
    using AgenticContextEngine.Core.Interfaces;
    using AgenticContextEngine.Models;

    // Classe base abstrata — demonstra HERANÇA e POLIMORFISMO
    public abstract class AgenteBase : IAgente
    {
        public string Nome { get; set; } = string.Empty;
        public string? Instrucoes { get; set; }

        // Método abstrato — cada filho DEVE implementar (polimorfismo)
        public abstract string ProcessarMensagem(string mensagemUsuario);

        // Método virtual — pode ser sobrescrito
        public virtual string GerarRespostaPadrao() =>
            $"Olá! Eu sou o {Nome}. Como posso te ajudar?";
    }

    // Agente de Vendas — HERDA de AgenteBase
    public class AgenteVendas : AgenteBase
    {
        public override string ProcessarMensagem(string mensagemUsuario)
        {
            var msg = mensagemUsuario.ToLower();

            if (msg.Contains("preco") || msg.Contains("valor") || msg.Contains("preço"))
                return "Temos ótimas ofertas! Qual produto você está procurando?";

            if (msg.Contains("desconto"))
                return "Para pedidos acima de 5 unidades, aplicamos 15% de desconto!";

            return GerarRespostaPadrao();
        }

        public override string GerarRespostaPadrao() =>
            $"Olá! Sou o {Nome}, especialista em vendas. Posso te ajudar a encontrar o melhor produto!";
    }

    // Agente de Suporte — HERDA de AgenteBase
    public class AgenteSuporte : AgenteBase
    {
        public override string ProcessarMensagem(string mensagemUsuario)
        {
            var msg = mensagemUsuario.ToLower();

            if (msg.Contains("erro") || msg.Contains("problema"))
                return "Entendo seu problema. Pode me enviar um print da tela de erro?";

            if (msg.Contains("não consigo") || msg.Contains("nao consigo"))
                return "Vou te ajudar! Me informe o que está tentando fazer.";

            return GerarRespostaPadrao();
        }

        public override string GerarRespostaPadrao() =>
            $"Olá! Sou o {Nome}, suporte técnico. Qual problema posso resolver para você?";
    }

    // Agente Financeiro — HERDA de AgenteBase
    public class AgenteFinanceiro : AgenteBase
    {
        public override string ProcessarMensagem(string mensagemUsuario)
        {
            var msg = mensagemUsuario.ToLower();

            if (msg.Contains("fatura") || msg.Contains("boleto"))
                return "Para acessar sua fatura, preciso verificar sua identidade. Qual seu CPF?";

            if (msg.Contains("pagamento"))
                return "Aceitamos PIX, cartão de crédito e boleto bancário.";

            return GerarRespostaPadrao();
        }
    }

    // =====================================================
    // FACTORY — cria o AgenteBase certo com base no Model
    // =====================================================
    public static class AgenteFactory
    {
        public static AgenteBase CriarAgente(Agente agente)
        {
            AgenteBase instancia = agente.CategoriaAgente?.Nome switch
            {
                "Vendas"          => new AgenteVendas(),
                "Suporte Tecnico" => new AgenteSuporte(),
                "Financeiro"      => new AgenteFinanceiro(),
                _                 => new AgenteVendas()
            };

            instancia.Nome       = agente.Nome;
            instancia.Instrucoes = agente.Instrucoes;
            return instancia;
        }
    }

    // =====================================================
    // SERVIÇO DE CONTEXTO — implementa IContextoMemoria
    // =====================================================
    public class ContextoMemoriaService : IContextoMemoria
    {
        private readonly Dictionary<string, string> _memoria = new();

        public void SalvarContexto(string chave, string valor) =>
            _memoria[chave] = valor;

        public string RecuperarContexto(string chave) =>
            _memoria.TryGetValue(chave, out var valor) ? valor : string.Empty;

        public List<KeyValuePair<string, string>> ListarContexto() =>
            new(_memoria);
    }

    // =====================================================
    // SERVIÇO DE ESTATÍSTICA — implementa IEstatistica
    // =====================================================
    public class EstatisticaService : IEstatistica
    {
        public int TotalSessoes { get; private set; } = 0;
        public int TotalMensagens { get; private set; } = 0;

        public void IncrementarSessao() => TotalSessoes++;
        public void IncrementarMensagem() => TotalMensagens++;
        public string GerarResumo() =>
            $"Total de sessões: {TotalSessoes} | Total de mensagens: {TotalMensagens}";
    }
}
