using Microsoft.AspNetCore.Mvc;
using AgenticContextEngine.Services;

namespace AgenticContextEngine.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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
            var resultado = await _authService.LoginAsync(email, senha);

            if (!resultado.Sucesso || resultado.Dados == null)
            {
                ViewBag.Erro = resultado.Erro;
                return View();
            }

            HttpContext.Session.SetString("UsuarioId", resultado.Dados.Id.ToString());
            HttpContext.Session.SetString("UsuarioNome", resultado.Dados.Nome);
            HttpContext.Session.SetString("UsuarioPerfil", resultado.Dados.Perfil);

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
