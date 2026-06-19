using AgenticContextEngine.Models;

namespace AgenticContextEngine.Data.Repositories
{
    // Repositorio dedicado ao fluxo de simulacao de chat: nao pertence a uma unica
    // entidade, pois orquestra Agente, CanalOrigem, SessaoAtendimento, Mensagem e EstatisticaAcesso.
    public interface ISimulacaoRepository
    {
        Task<List<AgenteOpcaoDto>> GetAgentesAtivosAsync();
        Task<List<CanalOpcaoDto>> GetCanaisAtivosAsync();
        Task<List<SessaoRecenteDto>> GetSessoesRecentesAsync(int quantidade);
        Task<Agente?> GetAgenteComCategoriaAsync(int agenteId);
        Task<CanalOrigem?> GetCanalAsync(int canalOrigemId);
        Task<SessaoAtendimento?> GetSessaoAtivaComMensagensAsync(int agenteId, int canalOrigemId);
        void AdicionarSessao(SessaoAtendimento sessao);
        Task<SessaoAtendimento?> GetSessaoComAgenteECategoriaAsync(int sessaoId);
        void AdicionarMensagem(Mensagem mensagem);
        Task RegistrarOuAtualizarEstatisticaAsync(int agenteId, int canalOrigemId);
        Task<SessaoAtendimento?> LimparContextoAsync(int sessaoId);
        Task SaveChangesAsync();
    }
}
