using Microsoft.EntityFrameworkCore;
using AgenticContextEngine.Data;
using AgenticContextEngine.Services;
using AgenticContextEngine.Models;

var builder = WebApplication.CreateBuilder(args);

// =======================================
// Services
// =======================================

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=agentic.db"));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

// =======================================
// Build
// =======================================

var app = builder.Build();

// =======================================
// Middleware
// =======================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

// =======================================
// Rotas MVC
// =======================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "mvc",
    pattern: "{controller}/{action=Index}/{id?}");

// =======================================
// Seed Inicial
// =======================================

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    context.Database.EnsureCreated();

    // ===================================
    // Perfil Administrador
    // ===================================

    if (!context.PerfilAcesso.Any())
    {
        var perfilAdmin = new PerfilAcesso
        {
            Nome = "Administrador",
            Ativo = true
        };

        context.PerfilAcesso.Add(perfilAdmin);
        context.SaveChanges();

        context.Usuario.Add(new Usuario
        {
            Nome = "Administrador",
            Email = "admin@sistema.com",
            SenhaHash = "123456",
            PerfilAcessoId = perfilAdmin.Id,
            Ativo = true
        });

        context.SaveChanges();
    }

    // ===================================
    // Categorias
    // ===================================

    if (!context.CategoriaAgente.Any())
    {
        var vendas = new CategoriaAgente
        {
            Nome = "Vendas",
            Ativo = true
        };

        var suporte = new CategoriaAgente
        {
            Nome = "Suporte Tecnico",
            Ativo = true
        };

        var financeiro = new CategoriaAgente
        {
            Nome = "Financeiro",
            Ativo = true
        };

        context.CategoriaAgente.AddRange(
            vendas,
            suporte,
            financeiro);

        context.SaveChanges();
    }

    // ===================================
    // Agentes
    // ===================================

    if (!context.Agente.Any())
    {
        var vendasId = context.CategoriaAgente
            .First(x => x.Nome == "Vendas").Id;

        var suporteId = context.CategoriaAgente
            .First(x => x.Nome == "Suporte Tecnico").Id;

        var financeiroId = context.CategoriaAgente
            .First(x => x.Nome == "Financeiro").Id;

        context.Agente.AddRange(

            new Agente
            {
                Nome = "SellerBot",
                CategoriaAgenteId = vendasId,
                Descricao = "Agente especializado em vendas",
                Ativo = true
            },

            new Agente
            {
                Nome = "SupportBot",
                CategoriaAgenteId = suporteId,
                Descricao = "Agente especializado em suporte",
                Ativo = true
            },

            new Agente
            {
                Nome = "FinanceBot",
                CategoriaAgenteId = financeiroId,
                Descricao = "Agente especializado em finanças",
                Ativo = true
            }
        );

        context.SaveChanges();
    }

    // ===================================
    // Canais
    // ===================================

    if (!context.CanalOrigem.Any())
    {
        context.CanalOrigem.AddRange(

            new CanalOrigem
            {
                Nome = "Site Principal",
                UrlSite = "https://siteprincipal.local",
                Ativo = true
            },

            new CanalOrigem
            {
                Nome = "App Mobile",
                UrlSite = "https://appmobile.local",
                Ativo = true
            },

            new CanalOrigem
            {
                Nome = "Loja Virtual",
                UrlSite = "https://lojavirtual.local",
                Ativo = true
            }
        );

        context.SaveChanges();
    }

    // ===================================
    // Sessões e Mensagens para Dashboard
    // ===================================

    if (!context.SessaoAtendimento.Any())
    {
        var agente = context.Agente.First();
        var canal = context.CanalOrigem.First();

        for (int i = 1; i <= 5; i++)
        {
            var sessao = new SessaoAtendimento
            {
                AgenteId = agente.Id,
                CanalOrigemId = canal.Id,
                Titulo = $"Sessão Teste {i}",
                Status = "Ativa",
                DataInicio = DateTime.Now.AddDays(-i)
            };

            context.SessaoAtendimento.Add(sessao);
            context.SaveChanges();

            for (int j = 1; j <= 4; j++)
            {
                context.Mensagem.Add(new Mensagem
                {
                    SessaoAtendimentoId = sessao.Id,
                    Conteudo = $"Mensagem de teste {j}",
                    Remetente = j % 2 == 0 ? "Agente" : "Usuário",
                    DataEnvio = DateTime.Now.AddDays(-i)
                });
            }

            context.SaveChanges();
        }
    }
}

// =======================================
// Run
// =======================================

app.Run();