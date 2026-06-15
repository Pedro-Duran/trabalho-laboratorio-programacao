// =====================================================
// Controllers/AuthController.cs
// =====================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgenticContextEngine.Data;

namespace AgenticContextEngine.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UsuarioId") != null)
                return RedirectToAction("Index", "Categorias");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string senha)
        {
            var usuario = await _db.Usuario
                .Include(u => u.PerfilAcesso)
                .FirstOrDefaultAsync(u => u.Email == email && u.Ativo);

            if (usuario == null)
            {
                ViewBag.Erro = "Email ou senha inválidos.";
                return View();
            }

            // Verificação simples de senha (em produção usar hash)
            if (usuario.SenhaHash != senha)
            {
                ViewBag.Erro = "Email ou senha inválidos.";
                return View();
            }

            HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
            HttpContext.Session.SetString("UsuarioNome", usuario.Nome);
            HttpContext.Session.SetString("UsuarioPerfil", usuario.PerfilAcesso?.Nome ?? "");

            // Log de auditoria
            _db.LogAuditoria.Add(new Models.LogAuditoria
            {
                UsuarioId = usuario.Id,
                Acao = "LOGIN",
                Entidade = "Usuario",
                EntidadeId = usuario.Id,
                Detalhes = "Login realizado com sucesso"
            });
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Categorias");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}

