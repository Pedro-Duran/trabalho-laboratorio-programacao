using AgenticContextEngine.Models;

namespace AgenticContextEngine.Services
{
    public interface ICategoriaService
    {
        Task<List<CategoriaListItemDto>> ListarAsync();
        Task CriarAsync(CategoriaFormDto dto, int? criadoPorUsuarioId);
        Task<OperationResult<CategoriaFormDto>> ObterParaEdicaoAsync(int id, int? usuarioId, bool isAdmin);
        Task<OperationResult> EditarAsync(CategoriaFormDto dto, int? usuarioId, bool isAdmin);
        Task<OperationResult<bool>> ExcluirAsync(int id, int? usuarioId, bool isAdmin);
    }
}
