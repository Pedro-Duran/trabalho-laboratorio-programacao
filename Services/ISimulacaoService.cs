using AgenticContextEngine.Models;

namespace AgenticContextEngine.Services
{
    public interface ISimulacaoService
    {
        Task<ChatIndexViewModel> ObterIndexAsync();
        Task<List<SessaoRecenteDto>> ObterSessoesRecentesAsync();
        Task<OperationResult<ChatSessaoViewModel>> IniciarOuRetomarChatAsync(int agenteId, int canalOrigemId, int? usuarioId);
        Task<OperationResult<EnviarMensagemResultDto>> EnviarMensagemAsync(int sessaoId, string conteudo);
    }
}
