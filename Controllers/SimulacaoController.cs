// =====================================================
// Controllers/SimulacaoController.cs
// =====================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgenticContextEngine.Data;
using AgenticContextEngine.Models;
using AgenticContextEngine.Business;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AgenticContextEngine.Controllers
{
    public class SimulacaoController : Controller
    {
        private readonly AppDbContext _db;

        public SimulacaoController(AppDbContext db)
        {
            _db = db;
        }

        // Tela inicial da simulação: Seleção de Agente e Canal
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var model = new ChatIndexViewModel
            {
                Agentes = await _db.Agente.Where(a => a.Ativo).ToListAsync(),
                Canais = await _db.CanalOrigem.Where(c => c.Ativo).ToListAsync()
            };

            return View(model);
        }

        // Carrega ou inicializa a sessão de chat correspondente ao Agente + Canal
        [HttpGet]
        public async Task<IActionResult> Chat(int agenteId, int canalOrigemId)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
                return RedirectToAction("Login", "Auth");

            var agente = await _db.Agente.Include(a => a.CategoriaAgente).FirstOrDefaultAsync(a => a.Id == agenteId);
            var canal = await _db.CanalOrigem.FindAsync(canalOrigemId);

            if (agente == null || canal == null)
                return NotFound("Agente ou Canal inválido.");

            // Tenta buscar uma sessão ATIVA existente para essa combinação exata
            var sessao = await _db.SessaoAtendimento
                .Include(s => s.Mensagens)
                .FirstOrDefaultAsync(s => s.AgenteId == agenteId && s.CanalOrigemId == canalOrigemId && s.Status == "Ativa");

            // Se não existir, cria uma nova sessão (Engenharia de Contexto Persistente)
            if (sessao == null)
            {
                sessao = new SessaoAtendimento
                {
                    AgenteId = agenteId,
                    CanalOrigemId = canalOrigemId,
                    UsuarioId = int.Parse(HttpContext.Session.GetString("UsuarioId")!),
                    Titulo = $"Simulação: {agente.Nome} em {canal.Nome}",
                    Status = "Ativa",
                    DataInicio = DateTime.Now
                };
                _db.SessaoAtendimento.Add(sessao);
                await _db.SaveChangesAsync();
            }

            var model = new ChatSessaoViewModel
            {
                AgenteId = agenteId,
                CanalOrigemId = canalOrigemId,
                SessaoId = sessao.Id,
                NomeAgente = agente.Nome,
                CategoriaAgente = agente.CategoriaAgente?.Nome ?? "Geral",
                NomeCanal = canal.Nome,
                Mensagens = sessao.Mensagens.OrderBy(m => m.DataEnvio).ToList()
            };

            return View(model);
        }

        // Recebe a mensagem enviada pelo usuário e processa a resposta simulada do Agente
        [HttpPost]
        public async Task<IActionResult> EnviarMensagem(int sessaoId, string conteudo)
        {
            if (string.IsNullOrWhiteSpace(conteudo))
                return BadRequest();

            var sessao = await _db.SessaoAtendimento
                .Include(s => s.Agente)
                .ThenInclude(a => a!.CategoriaAgente)
                .FirstOrDefaultAsync(s => s.Id == sessaoId);

            if (sessao == null || sessao.Agente == null)
                return NotFound();

            // 1. Salva a mensagem do Usuário
            var msgUsuario = new Mensagem
            {
                SessaoAtendimentoId = sessaoId,
                Conteudo = conteudo,
                Remetente = "Usuário",
                DataEnvio = DateTime.Now
            };
            _db.Mensagem.Add(msgUsuario);

            // 2. Aplicação de POO Avançada: Instancia o agente polimórfico pela Factory de Negócio
            AgenteBase agenteNegocio = AgenteFactory.CriarAgente(sessao.Agente);

            // 3. Processa a resposta baseada na estratégia da subclasse do agente
            string respostaSimulada = agenteNegocio.ProcessarMensagem(conteudo);

            // 4. Salva a mensagem de resposta do Agente no Banco de Dados
            var msgAgente = new Mensagem
            {
                SessaoAtendimentoId = sessaoId,
                Conteudo = respostaSimulada,
                Remetente = sessao.Agente.Nome,
                DataEnvio = DateTime.Now.AddSeconds(1) // Pequeno delay visual para ordenação
            };
            _db.Mensagem.Add(msgAgente);

            // Atualiza estatísticas locais na tabela (Mapeamento das 10 tabelas)
            var estatistica = await _db.EstatisticaAcesso
                .FirstOrDefaultAsync(e => e.AgenteId == sessao.AgenteId && e.CanalOrigemId == sessao.CanalOrigemId && e.DataReferencia == DateTime.Today);

            if (estatistica == null)
            {
                _db.EstatisticaAcesso.Add(new EstatisticaAcesso
                {
                    AgenteId = sessao.AgenteId,
                    CanalOrigemId = sessao.CanalOrigemId,
                    TotalSessoes = 1,
                    TotalMensagens = 2,
                    DataReferencia = DateTime.Today
                });
            }
            else
            {
                estatistica.TotalMensagens += 2;
                _db.EstatisticaAcesso.Update(estatistica);
            }

            await _db.SaveChangesAsync();

            // Redireciona de volta para manter o histórico atualizado na tela
            return RedirectToAction("Chat", new { agenteId = sessao.AgenteId, canalOrigemId = sessao.CanalOrigemId });
        }
    }
}