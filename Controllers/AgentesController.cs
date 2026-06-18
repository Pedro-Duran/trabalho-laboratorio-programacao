using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgenticContextEngine.Data;
using AgenticContextEngine.Models;

namespace AgenticContextEngine.Controllers
{
    public class AgentesController : Controller
    {
        private readonly AppDbContext _db;

        public AgentesController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var agentes = await _db.Agente
                .Include(a => a.CategoriaAgente)
                .ToListAsync();

            ViewBag.TotalMensagens = await _db.Mensagem.CountAsync();
            ViewBag.TotalMemorias = await _db.ContextoMemoria.CountAsync();

            return View(agentes);
        }

        public async Task<IActionResult> Criar()
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            ViewBag.Categorias = await _db.CategoriaAgente.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Criar(Agente agente)
        {
            agente.DataCriacao = DateTime.Now;
            _db.Agente.Add(agente);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var agente = await _db.Agente.FindAsync(id);
            if (agente == null) return NotFound();
            ViewBag.Categorias = await _db.CategoriaAgente.ToListAsync();
            return View(agente);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Agente agente)
        {
            _db.Agente.Update(agente);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Excluir(int id)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var agente = await _db.Agente.FindAsync(id);
            if (agente != null)
            {
                bool temSessoes = await _db.SessaoAtendimento.AnyAsync(s => s.AgenteId == id);
                bool temEstatisticas = await _db.EstatisticaAcesso.AnyAsync(e => e.AgenteId == id);
                bool temContexto = await _db.ContextoMemoria.AnyAsync(c => c.AgenteId == id);

                if (temSessoes || temEstatisticas || temContexto)
                {
                    TempData["Erro"] = "Nao foi possivel excluir este agente pois ele possui historico vinculado.";
                }
                else
                {
                    _db.Agente.Remove(agente);
                    await _db.SaveChangesAsync();
                    TempData["Sucesso"] = "Agente excluido com sucesso.";
                }
            }
            return RedirectToAction("Index");
        }
    }
}
