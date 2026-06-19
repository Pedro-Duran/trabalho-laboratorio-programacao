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
                .Select(u => new UsuarioListItemDto
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email,
                    PerfilNome = u.PerfilAcesso != null ? u.PerfilAcesso.Nome : "-",
                    Ativo = u.Ativo,
                    DataCriacao = u.DataCriacao
                })
                .ToListAsync();

            ViewBag.TotalAdmins = usuarios.Count(u => u.PerfilNome == "Administrador");
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

            ViewBag.Perfis = await _db.PerfilAcesso
                .Select(p => new OpcaoSelecaoDto { Id = p.Id, Nome = p.Nome })
                .ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Criar(UsuarioCreateDto dto)
        {
            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Erro"] = "Apenas administradores podem criar usuarios.";
                return RedirectToAction("Index");
            }

            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = dto.SenhaHash,
                PerfilAcessoId = dto.PerfilAcessoId,
                DataCriacao = DateTime.Now
            };
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

            var dto = new UsuarioEditDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                PerfilAcessoId = usuario.PerfilAcessoId,
                Ativo = usuario.Ativo
            };

            ViewBag.Perfis = await _db.PerfilAcesso
                .Select(p => new OpcaoSelecaoDto { Id = p.Id, Nome = p.Nome })
                .ToListAsync();
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(UsuarioEditDto dto)
        {
            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Erro"] = "Apenas administradores podem editar usuarios.";
                return RedirectToAction("Index");
            }

            var usuario = await _db.Usuario.FindAsync(dto.Id);
            if (usuario == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.NovaSenha))
            {
                if (dto.SenhaAtual != usuario.SenhaHash)
                {
                    TempData["Erro"] = "Senha atual incorreta. A senha nao foi alterada.";
                    return RedirectToAction("Editar", new { id = dto.Id });
                }
                usuario.SenhaHash = dto.NovaSenha;
            }

            usuario.Nome = dto.Nome;
            usuario.Email = dto.Email;
            usuario.PerfilAcessoId = dto.PerfilAcessoId;
            usuario.Ativo = dto.Ativo;

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