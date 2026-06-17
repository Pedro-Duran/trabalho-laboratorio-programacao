using System.Collections.Generic;

namespace AgenticContextEngine.Models
{
    public class DashboardViewModel
    {
        public int TotalUsuarios { get; set; }
        public int TotalAgentes { get; set; }
        public int TotalSessoes { get; set; }
        public int TotalMensagens { get; set; }

        public List<string> LabelsAgentes { get; set; } = new();
        public List<int> ValoresAgentes { get; set; } = new();

        public List<string> LabelsCanais { get; set; } = new();
        public List<int> ValoresCanais { get; set; } = new();

        public List<string> LabelsDias { get; set; } = new();
        public List<int> ValoresDias { get; set; } = new();
    }
}