using AgenticContextEngine.Models;

namespace AgenticContextEngine.Data.Repositories
{
    public interface ICanalRepository : IRepository<CanalOrigem>
    {
        Task<List<CanalListItemDto>> GetListItemsAsync();
        Task<int> CountSessoesAsync();
        Task<bool> TemSessoesVinculadasAsync(int canalId);
    }
}
