using Microsoft.AspNetCore.Mvc;
using AgenticContextEngine.Models;
using AgenticContextEngine.Services;

namespace AgenticContextEngine.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly ICategoriaService _service;

        public CategoriasController(ICategoriaService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var categorias = await _service.ListarAsync();
            return View(categorias);
        }

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

        [HttpPost]
        public async Task<IActionResult> Criar(CategoriaFormDto dto)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            if (!AuthHelper.PodeCriar(HttpContext))
            {
                TempData["Erro"] = "Convidados nao podem criar categorias.";
                return RedirectToAction("Index");
            }

            await _service.CriarAsync(dto, AuthHelper.GetUsuarioId(HttpContext));
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var resultado = await _service.ObterParaEdicaoAsync(id, AuthHelper.GetUsuarioId(HttpContext), AuthHelper.IsAdmin(HttpContext));
            if (!resultado.Sucesso || resultado.Dados == null)
            {
                TempData["Erro"] = resultado.Erro;
                return RedirectToAction("Index");
            }

            return View(resultado.Dados);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(CategoriaFormDto dto)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var resultado = await _service.EditarAsync(dto, AuthHelper.GetUsuarioId(HttpContext), AuthHelper.IsAdmin(HttpContext));
            if (!resultado.Sucesso)
                TempData["Erro"] = resultado.Erro;

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Excluir(int id)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var resultado = await _service.ExcluirAsync(id, AuthHelper.GetUsuarioId(HttpContext), AuthHelper.IsAdmin(HttpContext));
            if (!resultado.Sucesso)
                TempData["Erro"] = resultado.Erro;
            else if (resultado.Dados)
                TempData["Sucesso"] = "Categoria excluida com sucesso.";

            return RedirectToAction("Index");
        }
    }
}
