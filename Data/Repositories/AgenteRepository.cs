using Microsoft.EntityFrameworkCore;
using AgenticContextEngine.Models;

namespace AgenticContextEngine.Data.Repositories
{
    public class AgenteRepository : Repository<Agente>, IAgenteRepository
    {
        public AgenteRepository(AppDbContext context) : base(context) { }

        public async Task<List<AgenteListItemDto>> GetListItemsAsync()
        {
            return await Context.Agente
                .Select(a => new AgenteListItemDto
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    CategoriaNome = a.CategoriaAgente != null ? a.CategoriaAgente.Nome : "-",
                    Descricao = a.Descricao,
                    Instrucoes = a.Instrucoes,
                    Ativo = a.Ativo,
                    DataCriacao = a.DataCriacao,
                    CriadoPorUsuarioId = a.CriadoPorUsuarioId
                })
                .ToListAsync();
        }

        public async Task<int> CountMensagensAsync() => await Context.Mensagem.CountAsync();

        public async Task<int> CountMemoriasAsync() => await Context.ContextoMemoria.CountAsync();

        public async Task<List<OpcaoSelecaoDto>> GetCategoriasAsync()
        {
            return await Context.CategoriaAgente
                .Select(c => new OpcaoSelecaoDto { Id = c.Id, Nome = c.Nome })
                .ToListAsync();
        }

        public async Task<bool> TemHistoricoVinculadoAsync(int agenteId)
        {
            bool temSessoes = await Context.SessaoAtendimento.AnyAsync(s => s.AgenteId == agenteId);
            bool temEstatisticas = await Context.EstatisticaAcesso.AnyAsync(e => e.AgenteId == agenteId);
            bool temContexto = await Context.ContextoMemoria.AnyAsync(c => c.AgenteId == agenteId);
            return temSessoes || temEstatisticas || temContexto;
        }
    }
}
