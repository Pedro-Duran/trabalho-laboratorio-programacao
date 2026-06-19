using Microsoft.EntityFrameworkCore;
using AgenticContextEngine.Data;
using AgenticContextEngine.Services;
using AgenticContextEngine.Models;

// Carrega variaveis do .env (credenciais do MySQL do docker-compose) para o ambiente do processo
var envFile = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envFile))
{
    foreach (var line in File.ReadAllLines(envFile))
    {
        var trimmed = line.Trim();
        if (trimmed.Length == 0 || trimmed.StartsWith('#')) continue;
        var idx = trimmed.IndexOf('=');
        if (idx <= 0) continue;
        var key = trimmed[..idx].Trim();
        var value = trimmed[(idx + 1)..].Trim();
        if (Environment.GetEnvironmentVariable(key) == null)
            Environment.SetEnvironmentVariable(key, value);
    }
}

var builder = WebApplication.CreateBuilder(args);

// =======================================
// Services
// =======================================

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IDashboardService, DashboardService>();

var mysqlHost = Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "localhost";
var mysqlPort = Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306";
var mysqlDatabase = Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "agente_contexto";
var mysqlUser = Environment.GetEnvironmentVariable("MYSQL_USER") ?? "agente_user";
var mysqlPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? "";

var connectionString =
    $"Server={mysqlHost};Port={mysqlPort};Database={mysqlDatabase};User={mysqlUser};Password={mysqlPassword};";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

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

        var perfilRegular = new PerfilAcesso
        {
            Nome = "Regular",
            Ativo = true
        };

        context.PerfilAcesso.AddRange(perfilAdmin, perfilRegular);
        context.SaveChanges();

        context.Usuario.Add(new Usuario
        {
            Nome = "Administrador",
            Email = "admin@sistema.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            PerfilAcessoId = perfilAdmin.Id,
            Ativo = true
        });

        context.Usuario.Add(new Usuario
        {
            Nome = "Usuario Regular",
            Email = "regular@sistema.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            PerfilAcessoId = perfilRegular.Id,
            Ativo = true
        });

        context.SaveChanges();
    }

    // ===================================
    // Categorias
    // ===================================

    if (!context.CategoriaAgente.Any())
    {
        var adminId = context.Usuario.First(u => u.Email == "admin@sistema.com").Id;

        var vendas = new CategoriaAgente
        {
            Nome = "Vendas",
            Ativo = true,
            CriadoPorUsuarioId = adminId
        };

        var suporte = new CategoriaAgente
        {
            Nome = "Suporte Tecnico",
            Ativo = true,
            CriadoPorUsuarioId = adminId
        };

        var financeiro = new CategoriaAgente
        {
            Nome = "Financeiro",
            Ativo = true,
            CriadoPorUsuarioId = adminId
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
        var adminId = context.Usuario.First(u => u.Email == "admin@sistema.com").Id;

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
                Ativo = true,
                CriadoPorUsuarioId = adminId
            },

            new Agente
            {
                Nome = "SupportBot",
                CategoriaAgenteId = suporteId,
                Descricao = "Agente especializado em suporte",
                Ativo = true,
                CriadoPorUsuarioId = adminId
            },

            new Agente
            {
                Nome = "FinanceBot",
                CategoriaAgenteId = financeiroId,
                Descricao = "Agente especializado em finanças",
                Ativo = true,
                CriadoPorUsuarioId = adminId
            }
        );

        context.SaveChanges();
    }

    // ===================================
    // Canais
    // ===================================

    if (!context.CanalOrigem.Any())
    {
        var adminId = context.Usuario.First(u => u.Email == "admin@sistema.com").Id;

        context.CanalOrigem.AddRange(

            new CanalOrigem
            {
                Nome = "Site Principal",
                UrlSite = "https://siteprincipal.local",
                Ativo = true,
                CriadoPorUsuarioId = adminId
            },

            new CanalOrigem
            {
                Nome = "App Mobile",
                UrlSite = "https://appmobile.local",
                Ativo = true,
                CriadoPorUsuarioId = adminId
            },

            new CanalOrigem
            {
                Nome = "Loja Virtual",
                UrlSite = "https://lojavirtual.local",
                Ativo = true,
                CriadoPorUsuarioId = adminId
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