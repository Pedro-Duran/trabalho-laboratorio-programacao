using Microsoft.EntityFrameworkCore;
using AgenticContextEngine.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=agentic.db")
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


