using Microsoft.EntityFrameworkCore;
using AgenticContextEngine.Models;

namespace AgenticContextEngine.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

      
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
           
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.PerfilAcesso)
                .WithMany(p => p.Usuarios)
                .HasForeignKey(u => u.PerfilAcessoId);

      
            modelBuilder.Entity<Agente>()
                .HasOne(a => a.CategoriaAgente)
                .WithMany(c => c.Agentes)
                .HasForeignKey(a => a.CategoriaAgenteId);

         
            modelBuilder.Entity<CategoriaAgente>()
                .HasOne(c => c.CriadoPor)
                .WithMany()
                .HasForeignKey(c => c.CriadoPorUsuarioId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<Agente>()
                .HasOne(a => a.CriadoPor)
                .WithMany()
                .HasForeignKey(a => a.CriadoPorUsuarioId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

           
            modelBuilder.Entity<CanalOrigem>()
                .HasOne(c => c.CriadoPor)
                .WithMany()
                .HasForeignKey(c => c.CriadoPorUsuarioId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

          
            modelBuilder.Entity<SessaoAtendimento>()
                .HasOne(s => s.Agente)
                .WithMany()
                .HasForeignKey(s => s.AgenteId)
                .OnDelete(DeleteBehavior.Restrict);

    
            modelBuilder.Entity<SessaoAtendimento>()
                .HasOne(s => s.CanalOrigem)
                .WithMany()
                .HasForeignKey(s => s.CanalOrigemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SessaoAtendimento>()
                .HasOne(s => s.Usuario)
                .WithMany(u => u.Sessoes)
                .HasForeignKey(s => s.UsuarioId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Mensagem>()
                .HasOne(m => m.SessaoAtendimento)
                .WithMany(s => s.Mensagens)
                .HasForeignKey(m => m.SessaoAtendimentoId);

            
            modelBuilder.Entity<ContextoMemoria>()
                .HasOne(c => c.SessaoAtendimento)
                .WithMany()
                .HasForeignKey(c => c.SessaoAtendimentoId)
                .OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<ContextoMemoria>()
                .HasOne(c => c.Agente)
                .WithMany()
                .HasForeignKey(c => c.AgenteId)
                .OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<LogAuditoria>()
                .HasOne(l => l.Usuario)
                .WithMany()
                .HasForeignKey(l => l.UsuarioId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            
            modelBuilder.Entity<EstatisticaAcesso>()
                .HasOne(e => e.Agente)
                .WithMany()
                .HasForeignKey(e => e.AgenteId)
                .OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<EstatisticaAcesso>()
                .HasOne(e => e.CanalOrigem)
                .WithMany()
                .HasForeignKey(e => e.CanalOrigemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
