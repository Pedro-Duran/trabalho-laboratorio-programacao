using Microsoft.EntityFrameworkCore;
using AgenticContextEngine.Models;

namespace AgenticContextEngine.Data.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(AppDbContext context) : base(context) { }

        public async Task<List<UsuarioListItemDto>> GetListItemsAsync()
        {
            return await Context.Usuario
                .Select(u => new UsuarioListItemDto
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email,
                    PerfilNome = u.PerfilAcesso != null ? u.PerfilAcesso.Nome : "-",
                    Ativo = u.Ativo,
                    DataCriacao = u.DataCriacao
                })
                .ToListAsync();
        }

        public async Task<int> CountPerfisAsync() => await Context.PerfilAcesso.CountAsync();

        public async Task<List<OpcaoSelecaoDto>> GetPerfisAsync()
        {
            return await Context.PerfilAcesso
                .Select(p => new OpcaoSelecaoDto { Id = p.Id, Nome = p.Nome })
                .ToListAsync();
        }

        public async Task<Usuario?> GetByEmailAtivoComPerfilAsync(string email)
        {
            return await Context.Usuario
                .Include(u => u.PerfilAcesso)
                .FirstOrDefaultAsync(u => u.Email == email && u.Ativo);
        }

        public void AdicionarLogAuditoria(LogAuditoria log) => Context.LogAuditoria.Add(log);
    }
}
