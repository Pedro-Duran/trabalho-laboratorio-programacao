using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgenticContextEngine.Data;
using AgenticContextEngine.Models;
using AgenticContextEngine.Services;

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

            if (!AuthHelper.PodeCriar(HttpContext))
            {
                TempData["Erro"] = "Convidados nao podem criar canais.";
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Criar(CanalOrigem canal)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            if (!AuthHelper.PodeCriar(HttpContext))
            {
                TempData["Erro"] = "Convidados nao podem criar canais.";
                return RedirectToAction("Index");
            }

            canal.DataCriacao = DateTime.Now;
            canal.CriadoPorUsuarioId = AuthHelper.GetUsuarioId(HttpContext);
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

            if (!AuthHelper.PodeGerenciar(HttpContext, canal.CriadoPorUsuarioId))
            {
                TempData["Erro"] = "Voce so pode editar canais criados por voce.";
                return RedirectToAction("Index");
            }

            return View(canal);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(CanalOrigem canal)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var existente = await _db.CanalOrigem.FindAsync(canal.Id);
            if (existente == null) return NotFound();

            if (!AuthHelper.PodeGerenciar(HttpContext, existente.CriadoPorUsuarioId))
            {
                TempData["Erro"] = "Voce so pode editar canais criados por voce.";
                return RedirectToAction("Index");
            }

            existente.Nome = canal.Nome;
            existente.UrlSite = canal.UrlSite;
            existente.Descricao = canal.Descricao;
            existente.Ativo = canal.Ativo;

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
                if (!AuthHelper.PodeGerenciar(HttpContext, canal.CriadoPorUsuarioId))
                {
                    TempData["Erro"] = "Voce so pode excluir canais criados por voce.";
                    return RedirectToAction("Index");
                }

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