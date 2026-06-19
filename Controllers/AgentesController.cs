using Microsoft.AspNetCore.Mvc;
using AgenticContextEngine.Models;
using AgenticContextEngine.Services;

namespace AgenticContextEngine.Controllers
{
    public class AgentesController : Controller
    {
        private readonly IAgenteService _service;

        public AgentesController(IAgenteService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var resultado = await _service.ListarAsync();

            ViewBag.TotalMensagens = resultado.TotalMensagens;
            ViewBag.TotalMemorias = resultado.TotalMemorias;

            return View(resultado.Agentes);
        }

        public async Task<IActionResult> Criar()
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            if (!AuthHelper.PodeCriar(HttpContext))
            {
                TempData["Erro"] = "Convidados nao podem criar agentes.";
                return RedirectToAction("Index");
            }

            ViewBag.Categorias = await _service.ObterCategoriasAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Criar(AgenteFormDto dto)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            if (!AuthHelper.PodeCriar(HttpContext))
            {
                TempData["Erro"] = "Convidados nao podem criar agentes.";
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

            ViewBag.Categorias = await _service.ObterCategoriasAsync();
            return View(resultado.Dados);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(AgenteFormDto dto)
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
                TempData["Sucesso"] = "Agente excluido com sucesso.";

            return RedirectToAction("Index");
        }
    }
}
