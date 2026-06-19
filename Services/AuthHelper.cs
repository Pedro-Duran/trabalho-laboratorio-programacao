using Microsoft.AspNetCore.Http;

namespace AgenticContextEngine.Services
{
    public static class AuthHelper
    {
        public const string PerfilAdministrador = "Administrador";

        public static int? GetUsuarioId(HttpContext ctx)
        {
            var raw = ctx.Session.GetString("UsuarioId");
            return int.TryParse(raw, out var id) ? id : null;
        }

        public static string? GetPerfil(HttpContext ctx) =>
            ctx.Session.GetString("UsuarioPerfil");

        public static bool IsAdmin(HttpContext ctx) =>
            GetPerfil(ctx) == PerfilAdministrador;

    
        public static bool PodeCriar(HttpContext ctx) =>
            GetPerfil(ctx) != "Convidado";

        
        public static bool PodeGerenciar(HttpContext ctx, int? criadoPorUsuarioId)
        {
            if (IsAdmin(ctx)) return true;

            var usuarioId = GetUsuarioId(ctx);
            return usuarioId.HasValue && criadoPorUsuarioId.HasValue && usuarioId == criadoPorUsuarioId;
        }
    }
}
