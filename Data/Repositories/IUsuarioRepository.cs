using AgenticContextEngine.Models;

namespace AgenticContextEngine.Data.Repositories
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<List<UsuarioListItemDto>> GetListItemsAsync();
        Task<int> CountPerfisAsync();
        Task<List<OpcaoSelecaoDto>> GetPerfisAsync();
        Task<Usuario?> GetByEmailAtivoComPerfilAsync(string email);
        void AdicionarLogAuditoria(LogAuditoria log);
    }
}
