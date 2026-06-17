// =====================================================
// Models/ChatViewModel.cs
// =====================================================
using System.Collections.Generic;
using AgenticContextEngine.Models;

namespace AgenticContextEngine.Models
{
    public class ChatIndexViewModel
    {
        public List<Agente> Agentes { get; set; } = new();
        public List<CanalOrigem> Canais { get; set; } = new();
    }

    public class ChatSessaoViewModel
    {
        public int AgenteId { get; set; }
        public int CanalOrigemId { get; set; }
        public int SessaoId { get; set; }
        public string NomeAgente { get; set; } = string.Empty;
        public string CategoriaAgente { get; set; } = string.Empty;
        public string NomeCanal { get; set; } = string.Empty;
        public List<Mensagem> Mensagens { get; set; } = new();
    }
}