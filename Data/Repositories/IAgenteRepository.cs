using AgenticContextEngine.Models;

namespace AgenticContextEngine.Data.Repositories
{
    public interface IAgenteRepository : IRepository<Agente>
    {
        Task<List<AgenteListItemDto>> GetListItemsAsync();
        Task<int> CountMensagensAsync();
        Task<int> CountMemoriasAsync();
        Task<List<OpcaoSelecaoDto>> GetCategoriasAsync();
        Task<bool> TemHistoricoVinculadoAsync(int agenteId);
    }
}
