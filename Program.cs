using Microsoft.EntityFrameworkCore;
using AgenticContextEngine.Data;

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

builder.Services.AddControllersWithViews();

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

var app = builder.Build();

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

app.MapControllerRoute(name: "default", pattern: "{controller=Auth}/{action=Login}/{id?}");
app.MapControllerRoute(name: "areas", pattern: "{controller}/{action=Index}/{id?}");
app.MapControllerRoute(name: "areas", pattern: "{controller}/{action=Index}/{id?}");


// Carga inicial de dados para teste no localhost
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

    if (!context.PerfilAcesso.Any())
    {
        var perfil = new AgenticContextEngine.Models.PerfilAcesso { Nome = "Administrador", Ativo = true };
        context.PerfilAcesso.Add(perfil);
        context.SaveChanges();

        if (!context.Usuario.Any())
        {
            context.Usuario.Add(new AgenticContextEngine.Models.Usuario
            {
                Nome = "Administrador",
                Email = "admin@sistema.com",
                SenhaHash = "123456", // Senha simples para o seu teste local
                PerfilAcessoId = perfil.Id,
                Ativo = true
            });
        }

        // Adiciona um agente e um canal padrão para o chat não vir vazio
        if (!context.CategoriaAgente.Any())
        {
            var cat = new AgenticContextEngine.Models.CategoriaAgente { Nome = "Vendas", Ativo = true };
            context.CategoriaAgente.Add(cat);
            context.SaveChanges();

            context.Agente.Add(new AgenticContextEngine.Models.Agente { Nome = "SellerBot", CategoriaAgenteId = cat.Id, Ativo = true });
            context.CanalOrigem.Add(new AgenticContextEngine.Models.CanalOrigem { Nome = "Site Principal", UrlSite = "localhost", Ativo = true });
        }

        context.SaveChanges();
    }
}
app.Run();


