using Microsoft.AspNetCore.Mvc;
using AgenticContextEngine.Models;
using AgenticContextEngine.Services;

namespace AgenticContextEngine.Controllers
{
    public class CanaisController : Controller
    {
        private readonly ICanalService _service;

        public CanaisController(ICanalService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var resultado = await _service.ListarAsync();
            ViewBag.TotalSessoes = resultado.TotalSessoes;

            return View(resultado.Canais);
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
        public async Task<IActionResult> Criar(CanalFormDto dto)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            if (!AuthHelper.PodeCriar(HttpContext))
            {
                TempData["Erro"] = "Convidados nao podem criar canais.";
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
        public async Task<IActionResult> Editar(CanalFormDto dto)
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
                TempData["Sucesso"] = "Canal excluido com sucesso.";

            return RedirectToAction("Index");
        }
    }
}
