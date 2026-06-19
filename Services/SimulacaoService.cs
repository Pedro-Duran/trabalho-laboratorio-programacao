using AgenticContextEngine.Business;
using AgenticContextEngine.Data.Repositories;
using AgenticContextEngine.Models;

namespace AgenticContextEngine.Services
{
    public class SimulacaoService : ISimulacaoService
    {
        private readonly ISimulacaoRepository _repository;

        public SimulacaoService(ISimulacaoRepository repository)
        {
            _repository = repository;
        }

        public async Task<ChatIndexViewModel> ObterIndexAsync()
        {
            return new ChatIndexViewModel
            {
                Agentes = await _repository.GetAgentesAtivosAsync(),
                Canais = await _repository.GetCanaisAtivosAsync()
            };
        }

        public Task<List<SessaoRecenteDto>> ObterSessoesRecentesAsync() =>
            _repository.GetSessoesRecentesAsync(5);

        public async Task<OperationResult<ChatSessaoViewModel>> IniciarOuRetomarChatAsync(int agenteId, int canalOrigemId, int? usuarioId)
        {
            var agente = await _repository.GetAgenteComCategoriaAsync(agenteId);
            var canal = await _repository.GetCanalAsync(canalOrigemId);

            if (agente == null || canal == null)
                return OperationResult<ChatSessaoViewModel>.Falha("Agente ou Canal invalido.");

            var sessao = await _repository.GetSessaoAtivaComMensagensAsync(agenteId, canalOrigemId);

            if (sessao == null)
            {
                sessao = new SessaoAtendimento
                {
                    AgenteId = agenteId,
                    CanalOrigemId = canalOrigemId,
                    UsuarioId = usuarioId is > 0 ? usuarioId : null,
                    Titulo = $"Simulação: {agente.Nome} em {canal.Nome}",
                    Status = "Ativa",
                    DataInicio = DateTime.Now
                };
                _repository.AdicionarSessao(sessao);
                await _repository.SaveChangesAsync();
            }

            var model = new ChatSessaoViewModel
            {
                AgenteId = agenteId,
                CanalOrigemId = canalOrigemId,
                SessaoId = sessao.Id,
                NomeAgente = agente.Nome,
                CategoriaAgente = agente.CategoriaAgente?.Nome ?? "Geral",
                NomeCanal = canal.Nome,
                DataInicio = sessao.DataInicio,
                Mensagens = sessao.Mensagens
                    .OrderBy(m => m.DataEnvio)
                    .Select(m => new MensagemDto
                    {
                        Id = m.Id,
                        Conteudo = m.Conteudo,
                        Remetente = m.Remetente,
                        DataEnvio = m.DataEnvio
                    })
                    .ToList()
            };

            return OperationResult<ChatSessaoViewModel>.Ok(model);
        }

        public async Task<OperationResult<EnviarMensagemResultDto>> EnviarMensagemAsync(int sessaoId, string conteudo)
        {
            var sessao = await _repository.GetSessaoComAgenteECategoriaAsync(sessaoId);
            if (sessao == null || sessao.Agente == null)
                return OperationResult<EnviarMensagemResultDto>.Falha("Sessao nao encontrada.");

            var msgUsuario = new Mensagem
            {
                SessaoAtendimentoId = sessaoId,
                Conteudo = conteudo,
                Remetente = "Usuário",
                DataEnvio = DateTime.Now
            };
            _repository.AdicionarMensagem(msgUsuario);

            AgenteBase agenteNegocio = AgenteFactory.CriarAgente(sessao.Agente);
            string respostaSimulada = agenteNegocio.ProcessarMensagem(conteudo);

            var msgAgente = new Mensagem
            {
                SessaoAtendimentoId = sessaoId,
                Conteudo = respostaSimulada,
                Remetente = sessao.Agente.Nome,
                DataEnvio = DateTime.Now.AddSeconds(1)
            };
            _repository.AdicionarMensagem(msgAgente);

            await _repository.RegistrarOuAtualizarEstatisticaAsync(sessao.AgenteId, sessao.CanalOrigemId);

            await _repository.SaveChangesAsync();

            return OperationResult<EnviarMensagemResultDto>.Ok(new EnviarMensagemResultDto
            {
                AgenteId = sessao.AgenteId,
                CanalOrigemId = sessao.CanalOrigemId,
                ConteudoUsuario = msgUsuario.Conteudo,
                DataEnvioUsuario = msgUsuario.DataEnvio.ToString("dd/MM HH:mm"),
                NomeAgente = sessao.Agente.Nome,
                ConteudoAgente = msgAgente.Conteudo,
                DataEnvioAgente = msgAgente.DataEnvio.ToString("dd/MM HH:mm")
            });
        }

        public async Task<OperationResult<ChatRedirectDto>> LimparContextoAsync(int sessaoId)
        {
            var sessao = await _repository.LimparContextoAsync(sessaoId);
            if (sessao == null)
                return OperationResult<ChatRedirectDto>.Falha("Sessao nao encontrada.");

            return OperationResult<ChatRedirectDto>.Ok(new ChatRedirectDto
            {
                AgenteId = sessao.AgenteId,
                CanalOrigemId = sessao.CanalOrigemId
            });
        }
    }
}
