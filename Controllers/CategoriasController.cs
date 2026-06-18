using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgenticContextEngine.Data;
using AgenticContextEngine.Models;
using AgenticContextEngine.Services;

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

            if (!AuthHelper.PodeCriar(HttpContext))
            {
                TempData["Erro"] = "Convidados nao podem criar categorias.";
                return RedirectToAction("Index");
            }

            return View();
        }

        // CREATE POST
        [HttpPost]
        public async Task<IActionResult> Criar(CategoriaAgente categoria)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            if (!AuthHelper.PodeCriar(HttpContext))
            {
                TempData["Erro"] = "Convidados nao podem criar categorias.";
                return RedirectToAction("Index");
            }

            categoria.CriadoPorUsuarioId = AuthHelper.GetUsuarioId(HttpContext);
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

            if (!AuthHelper.PodeGerenciar(HttpContext, categoria.CriadoPorUsuarioId))
            {
                TempData["Erro"] = "Voce so pode editar categorias criadas por voce.";
                return RedirectToAction("Index");
            }

            return View(categoria);
        }

        // EDIT POST
        [HttpPost]
        public async Task<IActionResult> Editar(CategoriaAgente categoria)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var existente = await _db.CategoriaAgente.FindAsync(categoria.Id);
            if (existente == null) return NotFound();

            if (!AuthHelper.PodeGerenciar(HttpContext, existente.CriadoPorUsuarioId))
            {
                TempData["Erro"] = "Voce so pode editar categorias criadas por voce.";
                return RedirectToAction("Index");
            }

            existente.Nome = categoria.Nome;
            existente.Descricao = categoria.Descricao;
            existente.Ativo = categoria.Ativo;

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
                if (!AuthHelper.PodeGerenciar(HttpContext, categoria.CriadoPorUsuarioId))
                {
                    TempData["Erro"] = "Voce so pode excluir categorias criadas por voce.";
                    return RedirectToAction("Index");
                }

                bool temAgentes = await _db.Agente.AnyAsync(a => a.CategoriaAgenteId == id);

                if (temAgentes)
                {
                    TempData["Erro"] = "Nao foi possivel excluir esta categoria pois ela possui agentes vinculados.";
                }
                else
                {
                    _db.CategoriaAgente.Remove(categoria);
                    await _db.SaveChangesAsync();
                    TempData["Sucesso"] = "Categoria excluida com sucesso.";
                }
            }
            return RedirectToAction("Index");
        }
    }
}