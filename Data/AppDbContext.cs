// =====================================================
// AGENTIC CONTEXT ENGINE - AppDbContext
// Entity Framework Core — SQL Server
// =====================================================

using Microsoft.EntityFrameworkCore;
using AgenticContextEngine.Models;

namespace AgenticContextEngine.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // 10 tabelas mapeadas
        public DbSet<PerfilAcesso>        PerfilAcesso        { get; set; }
        public DbSet<Usuario>             Usuario             { get; set; }
        public DbSet<CategoriaAgente>     CategoriaAgente     { get; set; }
        public DbSet<Agente>              Agente              { get; set; }
        public DbSet<CanalOrigem>         CanalOrigem         { get; set; }
        public DbSet<SessaoAtendimento>   SessaoAtendimento   { get; set; }
        public DbSet<Mensagem>            Mensagem            { get; set; }
        public DbSet<ContextoMemoria>     ContextoMemoria     { get; set; }
        public DbSet<LogAuditoria>        LogAuditoria        { get; set; }
        public DbSet<EstatisticaAcesso>   EstatisticaAcesso   { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Usuario -> PerfilAcesso
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.PerfilAcesso)
                .WithMany(p => p.Usuarios)
                .HasForeignKey(u => u.PerfilAcessoId);

            // Agente -> CategoriaAgente
            modelBuilder.Entity<Agente>()
                .HasOne(a => a.CategoriaAgente)
                .WithMany(c => c.Agentes)
                .HasForeignKey(a => a.CategoriaAgenteId);

            // SessaoAtendimento -> Agente
            modelBuilder.Entity<SessaoAtendimento>()
                .HasOne(s => s.Agente)
                .WithMany()
                .HasForeignKey(s => s.AgenteId)
                .OnDelete(DeleteBehavior.Restrict);

            // SessaoAtendimento -> CanalOrigem
            modelBuilder.Entity<SessaoAtendimento>()
                .HasOne(s => s.CanalOrigem)
                .WithMany()
                .HasForeignKey(s => s.CanalOrigemId)
                .OnDelete(DeleteBehavior.Restrict);

            // SessaoAtendimento -> Usuario (opcional)
            modelBuilder.Entity<SessaoAtendimento>()
                .HasOne(s => s.Usuario)
                .WithMany(u => u.Sessoes)
                .HasForeignKey(s => s.UsuarioId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Mensagem -> SessaoAtendimento
            modelBuilder.Entity<Mensagem>()
                .HasOne(m => m.SessaoAtendimento)
                .WithMany(s => s.Mensagens)
                .HasForeignKey(m => m.SessaoAtendimentoId);

            // ContextoMemoria -> SessaoAtendimento
            modelBuilder.Entity<ContextoMemoria>()
                .HasOne(c => c.SessaoAtendimento)
                .WithMany()
                .HasForeignKey(c => c.SessaoAtendimentoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ContextoMemoria -> Agente
            modelBuilder.Entity<ContextoMemoria>()
                .HasOne(c => c.Agente)
                .WithMany()
                .HasForeignKey(c => c.AgenteId)
                .OnDelete(DeleteBehavior.Restrict);

            // LogAuditoria -> Usuario (opcional)
            modelBuilder.Entity<LogAuditoria>()
                .HasOne(l => l.Usuario)
                .WithMany()
                .HasForeignKey(l => l.UsuarioId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // EstatisticaAcesso -> Agente
            modelBuilder.Entity<EstatisticaAcesso>()
                .HasOne(e => e.Agente)
                .WithMany()
                .HasForeignKey(e => e.AgenteId)
                .OnDelete(DeleteBehavior.Restrict);

            // EstatisticaAcesso -> CanalOrigem
            modelBuilder.Entity<EstatisticaAcesso>()
                .HasOne(e => e.CanalOrigem)
                .WithMany()
                .HasForeignKey(e => e.CanalOrigemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
