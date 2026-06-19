using AgenticContextEngine.Models;

namespace AgenticContextEngine.Data.Repositories
{
    public interface ICategoriaRepository : IRepository<CategoriaAgente>
    {
        Task<List<CategoriaListItemDto>> GetListItemsAsync();
        Task<bool> TemAgentesVinculadosAsync(int categoriaId);
    }
}
