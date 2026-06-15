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
                _db.CanalOrigem.Remove(canal);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
