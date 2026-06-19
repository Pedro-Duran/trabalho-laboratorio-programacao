using Microsoft.EntityFrameworkCore;
using AgenticContextEngine.Models;

namespace AgenticContextEngine.Data.Repositories
{
    public class CategoriaRepository : Repository<CategoriaAgente>, ICategoriaRepository
    {
        public CategoriaRepository(AppDbContext context) : base(context) { }

        public async Task<List<CategoriaListItemDto>> GetListItemsAsync()
        {
            return await Context.CategoriaAgente
                .Select(c => new CategoriaListItemDto
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Descricao = c.Descricao,
                    Ativo = c.Ativo,
                    DataCriacao = c.DataCriacao,
                    CriadoPorUsuarioId = c.CriadoPorUsuarioId
                })
                .ToListAsync();
        }

        public async Task<bool> TemAgentesVinculadosAsync(int categoriaId) =>
            await Context.Agente.AnyAsync(a => a.CategoriaAgenteId == categoriaId);
    }
}
