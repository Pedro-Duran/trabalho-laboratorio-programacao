using AgenticContextEngine.Data;
using AgenticContextEngine.Models;
using Microsoft.EntityFrameworkCore;

namespace AgenticContextEngine.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public DashboardViewModel ObterDashboard()
        {
            var model = new DashboardViewModel();

            model.TotalUsuarios = _context.Usuario.Count();
            model.TotalAgentes = _context.Agente.Count();
            model.TotalSessoes = _context.SessaoAtendimento.Count();
            model.TotalMensagens = _context.Mensagem.Count();

            var mensagensPorAgente = _context.Mensagem
                .Include(m => m.SessaoAtendimento)
                .ThenInclude(s => s!.Agente)
                .GroupBy(m => m.SessaoAtendimento!.Agente!.Nome)
                .Select(g => new
                {
                    Nome = g.Key,
                    Total = g.Count()
                })
                .ToList();

            model.LabelsAgentes = mensagensPorAgente.Select(x => x.Nome).ToList();
            model.ValoresAgentes = mensagensPorAgente.Select(x => x.Total).ToList();

            var sessoesPorCanal = _context.SessaoAtendimento
                .Include(s => s.CanalOrigem)
                .GroupBy(s => s.CanalOrigem!.Nome)
                .Select(g => new
                {
                    Nome = g.Key,
                    Total = g.Count()
                })
                .ToList();

            model.LabelsCanais = sessoesPorCanal.Select(x => x.Nome).ToList();
            model.ValoresCanais = sessoesPorCanal.Select(x => x.Total).ToList();

            var mensagensPorDia = _context.Mensagem
                .GroupBy(m => m.DataEnvio.Date)
                .Select(g => new
                {
                    Dia = g.Key,
                    Total = g.Count()
                })
                .OrderBy(x => x.Dia)
                .ToList();

            model.LabelsDias = mensagensPorDia
                .Select(x => x.Dia.ToString("dd/MM"))
                .ToList();

            model.ValoresDias = mensagensPorDia
                .Select(x => x.Total)
                .ToList();

            return model;
        }
    }
}