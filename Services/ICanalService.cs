using AgenticContextEngine.Models;

namespace AgenticContextEngine.Services
{
    public interface ICanalService
    {
        Task<CanaisIndexDto> ListarAsync();
        Task CriarAsync(CanalFormDto dto, int? criadoPorUsuarioId);
        Task<OperationResult<CanalFormDto>> ObterParaEdicaoAsync(int id, int? usuarioId, bool isAdmin);
        Task<OperationResult> EditarAsync(CanalFormDto dto, int? usuarioId, bool isAdmin);
        Task<OperationResult<bool>> ExcluirAsync(int id, int? usuarioId, bool isAdmin);
    }
}
