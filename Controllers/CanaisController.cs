using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgenticContextEngine.Data;
using AgenticContextEngine.Models;

namespace AgenticContextEngine.Controllers
{
    public class CanaisController : Controller
    {
        private readonly AppDbContext _db;

        public CanaisController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var canais = await _db.CanalOrigem.ToListAsync();
            ViewBag.TotalSessoes = await _db.SessaoAtendimento.CountAsync();

            return View(canais);
        }

        public IActionResult Criar()
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Criar(CanalOrigem canal)
        {
            canal.DataCriacao = DateTime.Now;
            _db.CanalOrigem.Add(canal);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");
            var canal = await _db.CanalOrigem.FindAsync(id);
            if (canal == null) return NotFound();
            return View(canal);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(CanalOrigem canal)
        {
            _db.CanalOrigem.Update(canal);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Excluir(int id)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var canal = await _db.CanalOrigem.FindAsync(id);
            if (canal != null)
            {
                bool temSessoes = await _db.SessaoAtendimento.AnyAsync(s => s.CanalOrigemId == id);
                bool temEstatisticas = await _db.EstatisticaAcesso.AnyAsync(e => e.CanalOrigemId == id);

                if (temSessoes || temEstatisticas)
                {
                    TempData["Erro"] = "Nao foi possivel excluir este canal pois ele possui sessoes vinculadas.";
                }
                else
                {
                    _db.CanalOrigem.Remove(canal);
                    await _db.SaveChangesAsync();
                    TempData["Sucesso"] = "Canal excluido com sucesso.";
                }
            }
            return RedirectToAction("Index");
        }
    }
}