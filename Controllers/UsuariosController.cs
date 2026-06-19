using Microsoft.AspNetCore.Mvc;
using AgenticContextEngine.Models;
using AgenticContextEngine.Services;

namespace AgenticContextEngine.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IUsuarioService _service;

        public UsuariosController(IUsuarioService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var resultado = await _service.ListarAsync();

            ViewBag.TotalAdmins = resultado.TotalAdmins;
            ViewBag.TotalPerfis = resultado.TotalPerfis;

            return View(resultado.Usuarios);
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

            ViewBag.Perfis = await _service.ObterPerfisAsync();
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

            await _service.CriarAsync(dto);
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

            var dto = await _service.ObterParaEdicaoAsync(id);
            if (dto == null) return NotFound();

            ViewBag.Perfis = await _service.ObterPerfisAsync();
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

            var resultado = await _service.EditarAsync(dto);
            if (!resultado.Sucesso)
            {
                TempData["Erro"] = resultado.Erro;
                return RedirectToAction("Editar", new { id = dto.Id });
            }

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

            await _service.ExcluirAsync(id);
            return RedirectToAction("Index");
        }
    }
}
