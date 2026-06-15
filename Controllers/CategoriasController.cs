using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgenticContextEngine.Data;
using AgenticContextEngine.Models;

namespace AgenticContextEngine.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly AppDbContext _db;

        public CategoriasController(AppDbContext db)
        {
            _db = db;
        }

        // LIST
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var categorias = await _db.CategoriaAgente.ToListAsync();
            return View(categorias);
        }

        // CREATE GET
        public IActionResult Criar()
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");
            return View();
        }

        // CREATE POST
        [HttpPost]
        public async Task<IActionResult> Criar(CategoriaAgente categoria)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            _db.CategoriaAgente.Add(categoria);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // EDIT GET
        public async Task<IActionResult> Editar(int id)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var categoria = await _db.CategoriaAgente.FindAsync(id);
            if (categoria == null) return NotFound();
            return View(categoria);
        }

        // EDIT POST
        [HttpPost]
        public async Task<IActionResult> Editar(CategoriaAgente categoria)
        {
            _db.CategoriaAgente.Update(categoria);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // DELETE
        public async Task<IActionResult> Excluir(int id)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var categoria = await _db.CategoriaAgente.FindAsync(id);
            if (categoria != null)
            {
                _db.CategoriaAgente.Remove(categoria);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
