using Microsoft.AspNetCore.Mvc;
using AgenticContextEngine.Services;

namespace AgenticContextEngine.Controllers
{
    public class SimulacaoController : Controller
    {
        private readonly ISimulacaoService _service;

        public SimulacaoController(ISimulacaoService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var model = await _service.ObterIndexAsync();
            ViewBag.SessoesRecentes = await _service.ObterSessoesRecentesAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Chat(int agenteId, int canalOrigemId)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var usuarioId = AuthHelper.GetUsuarioId(HttpContext);
            var resultado = await _service.IniciarOuRetomarChatAsync(agenteId, canalOrigemId, usuarioId);

            if (!resultado.Sucesso || resultado.Dados == null)
                return NotFound(resultado.Erro);

            return View(resultado.Dados);
        }

        [HttpPost]
        public async Task<IActionResult> EnviarMensagem(int sessaoId, string conteudo)
        {
            if (string.IsNullOrWhiteSpace(conteudo))
                return BadRequest(new { erro = "Mensagem vazia." });

            var resultado = await _service.EnviarMensagemAsync(sessaoId, conteudo);
            if (!resultado.Sucesso || resultado.Dados == null)
                return NotFound();

            var dados = resultado.Dados;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    usuario = new
                    {
                        conteudo = dados.ConteudoUsuario,
                        dataEnvio = dados.DataEnvioUsuario
                    },
                    agente = new
                    {
                        nome = dados.NomeAgente,
                        conteudo = dados.ConteudoAgente,
                        dataEnvio = dados.DataEnvioAgente
                    }
                });
            }

            return RedirectToAction("Chat", new { agenteId = dados.AgenteId, canalOrigemId = dados.CanalOrigemId });
        }
    }
}
