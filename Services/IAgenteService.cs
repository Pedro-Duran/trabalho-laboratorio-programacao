using AgenticContextEngine.Models;

namespace AgenticContextEngine.Services
{
    public interface IAgenteService
    {
        Task<AgentesIndexDto> ListarAsync();
        Task<List<OpcaoSelecaoDto>> ObterCategoriasAsync();
        Task CriarAsync(AgenteFormDto dto, int? criadoPorUsuarioId);
        Task<OperationResult<AgenteFormDto>> ObterParaEdicaoAsync(int id, int? usuarioId, bool isAdmin);
        Task<OperationResult> EditarAsync(AgenteFormDto dto, int? usuarioId, bool isAdmin);
        Task<OperationResult<bool>> ExcluirAsync(int id, int? usuarioId, bool isAdmin);
    }
}
