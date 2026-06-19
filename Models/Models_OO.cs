using System;
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using AgenticContextEngine.Core.Interfaces;

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
// MODELS
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
        public int? CriadoPorUsuarioId { get; set; }
        public Usuario? CriadoPor { get; set; }
        public ICollection<Agente> Agentes { get; set; } = new List<Agente>();
    }

    public class Agente
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public int CategoriaAgenteId { get; set; }
        public string? Instrucoes { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public int? CriadoPorUsuarioId { get; set; }
        public Usuario? CriadoPor { get; set; }
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
        public int? CriadoPorUsuarioId { get; set; }
        public Usuario? CriadoPor { get; set; }
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
// CAMADA DE NEGOCIO
// =====================================================
namespace AgenticContextEngine.Business
{
    using AgenticContextEngine.Core.Interfaces;
    using AgenticContextEngine.Models;

    public abstract class AgenteBase : IAgente
    {
        public string Nome { get; set; } = string.Empty;
        public string? Instrucoes { get; set; }

        // Remove acentos para comparacoes de texto mais robustas (ex: "preco" == "preÃ§o")
        protected static string RemoverAcentos(string texto)
        {
            var textoNormalizado = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in textoNormalizado)
            {
                var categoria = CharUnicodeInfo.GetUnicodeCategory(c);
                if (categoria != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        // Compara ignorando acentuacao dos dois lados, entao "preço" e "preco" sao equivalentes
        // tanto na palavra-chave escrita no codigo quanto na mensagem digitada pelo usuario.
        protected static bool ContemPalavra(string mensagemNormalizada, params string[] palavras)
        {
            foreach (var palavra in palavras)
            {
                if (mensagemNormalizada.Contains(RemoverAcentos(palavra.ToLower())))
                    return true;
            }
            return false;
        }

        public abstract string ProcessarMensagem(string mensagemUsuario);
        public virtual string GerarRespostaPadrao() =>
            $"Ola! Eu sou o {Nome}. Como posso te ajudar?";
    }

    public class AgenteVendas : AgenteBase
    {
        public override string ProcessarMensagem(string mensagemUsuario)
        {
            var msg = RemoverAcentos(mensagemUsuario.ToLower());
            if (ContemPalavra(msg, "preço", "valor", "quanto custa"))
                return "Encontrei otimas condicoes para voce! Temos esse produto com ate 10% de desconto no PIX. Quer que eu envie o link da oferta?";
            if (ContemPalavra(msg, "desconto", "promoção", "cupom"))
                return "Voce pode usar o cupom BEMVINDO10 e garantir 10% off na primeira compra. Valido para todo o site ate o fim do mes!";
            if (ContemPalavra(msg, "estoque", "disponível", "tem aí"))
                return "Verifiquei aqui no sistema: ainda temos unidades disponiveis! Posso reservar uma para voce finalizar a compra com tranquilidade.";
            if (ContemPalavra(msg, "entrega", "prazo", "frete"))
                return "Para o seu CEP, o prazo estimado e de 3 a 7 dias uteis com frete gratis em compras acima de R$ 199. Quer que eu confirme o prazo exato?";
            if (ContemPalavra(msg, "forma de pagamento", "como pagar", "pagamento", "pagar"))
                return "Temos varias opcoes pra voce! PIX com aprovacao na hora e 5% de desconto extra, cartao de credito em ate 12x sem juros (acima de R$ 100), ou boleto bancario com vencimento em 3 dias uteis. Qual forma prefere usar?";
            if (ContemPalavra(msg, "cancelar", "cancelamento"))
                return "Sem problemas! Pode me informar o numero do pedido? Assim que eu confirmar, o cancelamento e processado em ate 24h.";
            if (ContemPalavra(msg, "troca", "trocar produto"))
                return "Voce tem ate 30 dias para solicitar a troca, sem custo adicional. Posso te enviar o passo a passo da nossa Central de Trocas.";
            if (ContemPalavra(msg, "rastreio", "rastrear", "onde está meu pedido"))
                return "Posso rastrear seu pedido agora mesmo! Me informa o numero do pedido ou o CPF usado na compra.";
            if (ContemPalavra(msg, "obrigado", "obrigada", "valeu"))
                return "Eu que agradeco a confianca! Qualquer coisa, estarei por aqui. Tenha uma otima compra!";
            return GerarRespostaPadrao();
        }
        public override string GerarRespostaPadrao() =>
            $"Ola! Seja bem-vindo(a)! Sou o {Nome}, do time de Vendas. Posso te ajudar com precos, formas de pagamento, prazos de entrega ou disponibilidade de produtos. Como posso ajudar hoje?";
    }

    public class AgenteSuporte : AgenteBase
    {
        public override string ProcessarMensagem(string mensagemUsuario)
        {
            var msg = RemoverAcentos(mensagemUsuario.ToLower());
            if (ContemPalavra(msg, "erro", "problema", "bug"))
                return "Sinto muito pelo transtorno! Para eu te ajudar mais rapido, pode me enviar um print da tela com o erro ou me contar exatamente o que apareceu?";
            if (ContemPalavra(msg, "não consigo", "não funciona"))
                return "Vamos resolver isso juntos! Me conta com calma o que voce esta tentando fazer e em qual etapa o sistema trava.";
            if (ContemPalavra(msg, "senha", "login", "acesso"))
                return "Posso te ajudar a recuperar o acesso agora mesmo! Confirma pra mim o email cadastrado na sua conta, por favor?";
            if (ContemPalavra(msg, "lento", "travando", "trava"))
                return "Entendido! Geralmente isso resolve limpando o cache do navegador ou tentando em uma aba anonima. Quer que eu te guie passo a passo?";
            if (ContemPalavra(msg, "atualizar", "atualização", "versão"))
                return "Lancamos uma atualizacao recente justamente para corrigir instabilidades. Recomendo atualizar o app e reiniciar o dispositivo.";
            if (ContemPalavra(msg, "app", "aplicativo", "celular"))
                return "Sobre o aplicativo: ele esta disponivel na App Store e Google Play. Qual sistema operacional voce esta usando?";
            if (ContemPalavra(msg, "conta", "cadastro", "dados pessoais"))
                return "Posso te ajudar a atualizar seus dados cadastrais. So por seguranca, vou te direcionar para a area de Meu Perfil no site ou app.";
            if (ContemPalavra(msg, "obrigado", "obrigada", "valeu"))
                return "Por nada! Ficamos felizes em ajudar. Se o problema voltar, e so chamar novamente que resolvemos rapidinho.";
            return GerarRespostaPadrao();
        }
        public override string GerarRespostaPadrao() =>
            $"Ola, tudo bem? Sou o {Nome}, do nosso time de Suporte Tecnico. Pode me contar o que esta acontecendo: erro no site, problema de acesso, ou algo lento?";
    }

    public class AgenteFinanceiro : AgenteBase
    {
        public override string ProcessarMensagem(string mensagemUsuario)
        {
            var msg = RemoverAcentos(mensagemUsuario.ToLower());
            if (ContemPalavra(msg, "fatura", "boleto", "cobrança"))
                return "Posso emitir a 2a via da sua fatura agora! Por seguranca, confirma pra mim o CPF ou email cadastrado na conta?";
            if (ContemPalavra(msg, "pagamento", "pagar"))
                return "Voce pode regularizar via PIX (compensacao imediata), cartao de credito ou boleto bancario. Qual prefere usar?";
            if (ContemPalavra(msg, "atraso", "atrasado", "vencido"))
                return "Identifiquei aqui que ha um valor pendente. Posso negociar uma nova data ou gerar a segunda via sem juros se o pagamento for feito em ate 5 dias.";
            if (ContemPalavra(msg, "reembolso", "estorno", "devolução"))
                return "Seu reembolso ja esta em processamento. O prazo costuma ser de ate 5 dias uteis no PIX ou 1 a 2 faturas no cartao de credito.";
            if (ContemPalavra(msg, "nota fiscal", "recibo", "comprovante"))
                return "A nota fiscal e enviada automaticamente para o seu email cadastrado em ate 24h apos a confirmacao do pagamento. Posso reenviar se preferir.";
            if (ContemPalavra(msg, "parcelar", "parcelamento", "parcelas"))
                return "Voce pode parcelar em ate 12x no cartao de credito. Ate 6x sem juros, e da 7a a 12a parcela com taxa reduzida.";
            if (ContemPalavra(msg, "limite", "aumentar limite"))
                return "Para analise de aumento de limite, voce pode acessar a area Meus Limites no app. Costuma levar ate 48h para uma resposta.";
            if (ContemPalavra(msg, "juros", "taxa"))
                return "Nossas taxas variam conforme a forma de pagamento. No PIX e a vista nao ha juros. Posso simular um parcelamento se quiser.";
            if (ContemPalavra(msg, "obrigado", "obrigada", "valeu"))
                return "Disponha! Qualquer duvida sobre pagamentos ou faturas, e so chamar. Tenha um otimo dia!";
            return GerarRespostaPadrao();
        }
        public override string GerarRespostaPadrao() =>
            $"Ola! Sou o {Nome}, do nosso time Financeiro. Posso ajudar com 2a via de fatura, formas de pagamento, parcelamento ou reembolsos. Em que posso ajudar?";
    }

    public class AgenteAgendamento : AgenteBase
    {
        public override string ProcessarMensagem(string mensagemUsuario)
        {
            var msg = RemoverAcentos(mensagemUsuario.ToLower());
            if (ContemPalavra(msg, "agendar", "marcar", "horário"))
                return "Vamos agendar sua assistencia tecnica ou instalacao! Voce prefere um horario pela manha ou tarde, e em qual dia da semana?";
            if (ContemPalavra(msg, "cancelar", "desmarcar"))
                return "Sem problemas! Pode me passar o numero do protocolo ou a data marcada para eu cancelar com a equipe tecnica?";
            if (ContemPalavra(msg, "remarcar", "mudar horário", "trocar data"))
                return "Consigo remarcar para voce agora mesmo. Qual nova data e horario funcionam melhor na sua agenda?";
            if (ContemPalavra(msg, "disponibilidade", "vaga", "tem horário"))
                return "Verificando aqui... temos vagas disponiveis essa semana e na proxima. Prefere atendimento em loja ou visita tecnica em domicilio?";
            if (ContemPalavra(msg, "confirmar", "confirmação"))
                return "Tudo certo! Seu agendamento foi confirmado e voce vai receber um lembrete por SMS um dia antes do horario marcado.";
            if (ContemPalavra(msg, "instalação", "instalar"))
                return "Para instalacao de produtos, nossa equipe tecnica atende em ate 5 dias uteis apos a confirmacao da compra. Posso agendar a data?";
            if (ContemPalavra(msg, "garantia", "assistência"))
                return "Esse atendimento esta dentro do periodo de garantia! Posso agendar uma visita tecnica sem custo adicional para voce.";
            if (ContemPalavra(msg, "técnico", "visita"))
                return "Nossa equipe tecnica atende em horario comercial, de segunda a sabado. Qual endereco devo considerar para o atendimento?";
            if (ContemPalavra(msg, "obrigado", "obrigada", "valeu"))
                return "Por nada! Ate o dia do nosso atendimento agendado. Qualquer mudanca, estamos por aqui.";
            return GerarRespostaPadrao();
        }
        public override string GerarRespostaPadrao() =>
            $"Ola! Sou o {Nome}, do time de Agendamentos. Posso marcar instalacoes, visitas tecnicas ou assistencias. Como posso ajudar?";
    }

    public static class AgenteFactory
    {
        public static AgenteBase CriarAgente(Agente agente)
        {
            AgenteBase instancia = agente.CategoriaAgente?.Nome switch
            {
                "Vendas" => new AgenteVendas(),
                "Suporte Tecnico" => new AgenteSuporte(),
                "Financeiro" => new AgenteFinanceiro(),
                "Agendamento" => new AgenteAgendamento(),
                _ => new AgenteVendas()
            };
            instancia.Nome = agente.Nome;
            instancia.Instrucoes = agente.Instrucoes;
            return instancia;
        }
    }

    public class ContextoMemoriaService : IContextoMemoria
    {
        private readonly Dictionary<string, string> _memoria = new();
        public void SalvarContexto(string chave, string valor) => _memoria[chave] = valor;
        public string RecuperarContexto(string chave) =>
            _memoria.TryGetValue(chave, out var valor) ? valor : string.Empty;
        public List<KeyValuePair<string, string>> ListarContexto() => new(_memoria);
    }

    public class EstatisticaService : IEstatistica
    {
        public int TotalSessoes { get; private set; } = 0;
        public int TotalMensagens { get; private set; } = 0;
        public void IncrementarSessao() => TotalSessoes++;
        public void IncrementarMensagem() => TotalMensagens++;
        public string GerarResumo() =>
            $"Total de sessoes: {TotalSessoes} | Total de mensagens: {TotalMensagens}";
    }
}
