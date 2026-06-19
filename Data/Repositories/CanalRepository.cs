using Microsoft.EntityFrameworkCore;
using AgenticContextEngine.Models;

namespace AgenticContextEngine.Data.Repositories
{
    public class CanalRepository : Repository<CanalOrigem>, ICanalRepository
    {
        public CanalRepository(AppDbContext context) : base(context) { }

        public async Task<List<CanalListItemDto>> GetListItemsAsync()
        {
            return await Context.CanalOrigem
                .Select(c => new CanalListItemDto
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    UrlSite = c.UrlSite,
                    Descricao = c.Descricao,
                    Ativo = c.Ativo,
                    DataCriacao = c.DataCriacao,
                    CriadoPorUsuarioId = c.CriadoPorUsuarioId
                })
                .ToListAsync();
        }

        public async Task<int> CountSessoesAsync() => await Context.SessaoAtendimento.CountAsync();

        public async Task<bool> TemSessoesVinculadasAsync(int canalId)
        {
            bool temSessoes = await Context.SessaoAtendimento.AnyAsync(s => s.CanalOrigemId == canalId);
            bool temEstatisticas = await Context.EstatisticaAcesso.AnyAsync(e => e.CanalOrigemId == canalId);
            return temSessoes || temEstatisticas;
        }
    }
}
