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

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash))
            {
                ViewBag.Erro = "Email ou senha inválidos.";
                return View();
            }

            HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
            HttpContext.Session.SetString("UsuarioNome", usuario.Nome);
            HttpContext.Session.SetString("UsuarioPerfil", usuario.PerfilAcesso?.Nome ?? "");

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

        [HttpPost]
        public IActionResult Convidado()
        {
            HttpContext.Session.SetString("UsuarioId", "0");
            HttpContext.Session.SetString("UsuarioNome", "Convidado");
            HttpContext.Session.SetString("UsuarioPerfil", "Convidado");
            return RedirectToAction("Index", "Categorias");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}