
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgenticContextEngine.Data;
using AgenticContextEngine.Models;
using AgenticContextEngine.Business;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AgenticContextEngine.Controllers
{
    public class SimulacaoController : Controller
    {
        private readonly AppDbContext _db;

        public SimulacaoController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var model = new ChatIndexViewModel
            {
                Agentes = await _db.Agente
                    .Where(a => a.Ativo)
                    .Select(a => new AgenteOpcaoDto { Id = a.Id, Nome = a.Nome })
                    .ToListAsync(),
                Canais = await _db.CanalOrigem
                    .Where(c => c.Ativo)
                    .Select(c => new CanalOpcaoDto { Id = c.Id, Nome = c.Nome, UrlSite = c.UrlSite })
                    .ToListAsync()
            };

            ViewBag.SessoesRecentes = await _db.SessaoAtendimento
                .Where(s => s.Status == "Ativa")
                .OrderByDescending(s => s.DataInicio)
                .Take(5)
                .Select(s => new SessaoRecenteDto
                {
                    Id = s.Id,
                    NomeAgente = s.Agente != null ? s.Agente.Nome : "-",
                    NomeCanal = s.CanalOrigem != null ? s.CanalOrigem.Nome : "-",
                    DataInicio = s.DataInicio
                })
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Chat(int agenteId, int canalOrigemId)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var agente = await _db.Agente.Include(a => a.CategoriaAgente).FirstOrDefaultAsync(a => a.Id == agenteId);
            var canal = await _db.CanalOrigem.FindAsync(canalOrigemId);

            if (agente == null || canal == null)
                return NotFound("Agente ou Canal invÃ¡lido.");

            var sessao = await _db.SessaoAtendimento
                .Include(s => s.Mensagens)
                .FirstOrDefaultAsync(s => s.AgenteId == agenteId && s.CanalOrigemId == canalOrigemId && s.Status == "Ativa");

            if (sessao == null)
            {
                sessao = new SessaoAtendimento
                {
                    AgenteId = agenteId,
                    CanalOrigemId = canalOrigemId,
                    UsuarioId = int.TryParse(HttpContext.Session.GetString("UsuarioId"), out var uid) && uid > 0 ? uid : null,
                    Titulo = $"SimulaÃ§Ã£o: {agente.Nome} em {canal.Nome}",
                    Status = "Ativa",
                    DataInicio = DateTime.Now
                };
                _db.SessaoAtendimento.Add(sessao);
                await _db.SaveChangesAsync();
            }

            var model = new ChatSessaoViewModel
            {
                AgenteId = agenteId,
                CanalOrigemId = canalOrigemId,
                SessaoId = sessao.Id,
                NomeAgente = agente.Nome,
                CategoriaAgente = agente.CategoriaAgente?.Nome ?? "Geral",
                NomeCanal = canal.Nome,
                Mensagens = sessao.Mensagens
                    .OrderBy(m => m.DataEnvio)
                    .Select(m => new MensagemDto
                    {
                        Id = m.Id,
                        Conteudo = m.Conteudo,
                        Remetente = m.Remetente,
                        DataEnvio = m.DataEnvio
                    })
                    .ToList()
            };

            ViewBag.DataInicio = sessao.DataInicio;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EnviarMensagem(int sessaoId, string conteudo)
        {
            if (string.IsNullOrWhiteSpace(conteudo))
                return BadRequest(new { erro = "Mensagem vazia." });

            var sessao = await _db.SessaoAtendimento
                .Include(s => s.Agente)
                .ThenInclude(a => a!.CategoriaAgente)
                .FirstOrDefaultAsync(s => s.Id == sessaoId);

            if (sessao == null || sessao.Agente == null)
                return NotFound();

            var msgUsuario = new Mensagem
            {
                SessaoAtendimentoId = sessaoId,
                Conteudo = conteudo,
                Remetente = "UsuÃ¡rio",
                DataEnvio = DateTime.Now
            };
            _db.Mensagem.Add(msgUsuario);

            AgenteBase agenteNegocio = AgenteFactory.CriarAgente(sessao.Agente);
            string respostaSimulada = agenteNegocio.ProcessarMensagem(conteudo);

            var msgAgente = new Mensagem
            {
                SessaoAtendimentoId = sessaoId,
                Conteudo = respostaSimulada,
                Remetente = sessao.Agente.Nome,
                DataEnvio = DateTime.Now.AddSeconds(1)
            };
            _db.Mensagem.Add(msgAgente);

            var estatistica = await _db.EstatisticaAcesso
                .FirstOrDefaultAsync(e => e.AgenteId == sessao.AgenteId && e.CanalOrigemId == sessao.CanalOrigemId && e.DataReferencia == DateTime.Today);

            if (estatistica == null)
            {
                _db.EstatisticaAcesso.Add(new EstatisticaAcesso
                {
                    AgenteId = sessao.AgenteId,
                    CanalOrigemId = sessao.CanalOrigemId,
                    TotalSessoes = 1,
                    TotalMensagens = 2,
                    DataReferencia = DateTime.Today
                });
            }
            else
            {
                estatistica.TotalMensagens += 2;
                _db.EstatisticaAcesso.Update(estatistica);
            }

            await _db.SaveChangesAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    usuario = new
                    {
                        conteudo = msgUsuario.Conteudo,
                        dataEnvio = msgUsuario.DataEnvio.ToString("dd/MM HH:mm")
                    },
                    agente = new
                    {
                        nome = sessao.Agente.Nome,
                        conteudo = msgAgente.Conteudo,
                        dataEnvio = msgAgente.DataEnvio.ToString("dd/MM HH:mm")
                    }
                });
            }

            return RedirectToAction("Chat", new { agenteId = sessao.AgenteId, canalOrigemId = sessao.CanalOrigemId });
        }
    }
}