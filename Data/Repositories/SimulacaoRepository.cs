using Microsoft.EntityFrameworkCore;
using AgenticContextEngine.Models;

namespace AgenticContextEngine.Data.Repositories
{
    public class SimulacaoRepository : ISimulacaoRepository
    {
        private readonly AppDbContext _context;

        public SimulacaoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AgenteOpcaoDto>> GetAgentesAtivosAsync()
        {
            return await _context.Agente
                .Where(a => a.Ativo)
                .Select(a => new AgenteOpcaoDto { Id = a.Id, Nome = a.Nome })
                .ToListAsync();
        }

        public async Task<List<CanalOpcaoDto>> GetCanaisAtivosAsync()
        {
            return await _context.CanalOrigem
                .Where(c => c.Ativo)
                .Select(c => new CanalOpcaoDto { Id = c.Id, Nome = c.Nome, UrlSite = c.UrlSite })
                .ToListAsync();
        }

        public async Task<List<SessaoRecenteDto>> GetSessoesRecentesAsync(int quantidade)
        {
            return await _context.SessaoAtendimento
                .Where(s => s.Status == "Ativa")
                .OrderByDescending(s => s.DataInicio)
                .Take(quantidade)
                .Select(s => new SessaoRecenteDto
                {
                    Id = s.Id,
                    NomeAgente = s.Agente != null ? s.Agente.Nome : "-",
                    NomeCanal = s.CanalOrigem != null ? s.CanalOrigem.Nome : "-",
                    DataInicio = s.DataInicio
                })
                .ToListAsync();
        }

        public async Task<Agente?> GetAgenteComCategoriaAsync(int agenteId) =>
            await _context.Agente.Include(a => a.CategoriaAgente).FirstOrDefaultAsync(a => a.Id == agenteId);

        public async Task<CanalOrigem?> GetCanalAsync(int canalOrigemId) =>
            await _context.CanalOrigem.FindAsync(canalOrigemId);

        public async Task<SessaoAtendimento?> GetSessaoAtivaComMensagensAsync(int agenteId, int canalOrigemId)
        {
            return await _context.SessaoAtendimento
                .Include(s => s.Mensagens)
                .FirstOrDefaultAsync(s => s.AgenteId == agenteId && s.CanalOrigemId == canalOrigemId && s.Status == "Ativa");
        }

        public void AdicionarSessao(SessaoAtendimento sessao) => _context.SessaoAtendimento.Add(sessao);

        public async Task<SessaoAtendimento?> GetSessaoComAgenteECategoriaAsync(int sessaoId)
        {
            return await _context.SessaoAtendimento
                .Include(s => s.Agente)
                .ThenInclude(a => a!.CategoriaAgente)
                .FirstOrDefaultAsync(s => s.Id == sessaoId);
        }

        public void AdicionarMensagem(Mensagem mensagem) => _context.Mensagem.Add(mensagem);

        public async Task RegistrarOuAtualizarEstatisticaAsync(int agenteId, int canalOrigemId)
        {
            var estatistica = await _context.EstatisticaAcesso
                .FirstOrDefaultAsync(e => e.AgenteId == agenteId && e.CanalOrigemId == canalOrigemId && e.DataReferencia == DateTime.Today);

            if (estatistica == null)
            {
                _context.EstatisticaAcesso.Add(new EstatisticaAcesso
                {
                    AgenteId = agenteId,
                    CanalOrigemId = canalOrigemId,
                    TotalSessoes = 1,
                    TotalMensagens = 2,
                    DataReferencia = DateTime.Today
                });
            }
            else
            {
                estatistica.TotalMensagens += 2;
                _context.EstatisticaAcesso.Update(estatistica);
            }
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
