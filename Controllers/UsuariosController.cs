using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgenticContextEngine.Data;
using AgenticContextEngine.Models;
using AgenticContextEngine.Services;

namespace AgenticContextEngine.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _db;

        public UsuariosController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var usuarios = await _db.Usuario
                .Include(u => u.PerfilAcesso)
                .ToListAsync();

            ViewBag.TotalAdmins = usuarios.Count(u => u.PerfilAcesso?.Nome == "Administrador");
            ViewBag.TotalPerfis = await _db.PerfilAcesso.CountAsync();

            return View(usuarios);
        }

        public async Task<IActionResult> Criar()
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Erro"] = "Apenas administradores podem criar usuarios.";
                return RedirectToAction("Index");
            }

            ViewBag.Perfis = await _db.PerfilAcesso.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Criar(Usuario usuario)
        {
            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Erro"] = "Apenas administradores podem criar usuarios.";
                return RedirectToAction("Index");
            }

            usuario.DataCriacao = DateTime.Now;
            _db.Usuario.Add(usuario);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Erro"] = "Apenas administradores podem editar usuarios.";
                return RedirectToAction("Index");
            }

            var usuario = await _db.Usuario.FindAsync(id);
            if (usuario == null) return NotFound();
            ViewBag.Perfis = await _db.PerfilAcesso.ToListAsync();
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Usuario usuario)
        {
            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Erro"] = "Apenas administradores podem editar usuarios.";
                return RedirectToAction("Index");
            }

            _db.Usuario.Update(usuario);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Excluir(int id)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Erro"] = "Apenas administradores podem excluir usuarios.";
                return RedirectToAction("Index");
            }

            var usuario = await _db.Usuario.FindAsync(id);
            if (usuario != null)
            {
                _db.Usuario.Remove(usuario);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}