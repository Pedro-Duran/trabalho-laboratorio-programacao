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

        public static bool PodeCriar(string? perfil) => perfil != "Convidado";

        public static bool PodeCriar(HttpContext ctx) =>
            PodeCriar(GetPerfil(ctx));

        // Versao pura, sem dependencia de HttpContext, para uso na camada de servico.
        public static bool PodeGerenciar(bool isAdmin, int? usuarioId, int? criadoPorUsuarioId)
        {
            if (isAdmin) return true;
            return usuarioId.HasValue && criadoPorUsuarioId.HasValue && usuarioId == criadoPorUsuarioId;
        }

        public static bool PodeGerenciar(HttpContext ctx, int? criadoPorUsuarioId) =>
            PodeGerenciar(IsAdmin(ctx), GetUsuarioId(ctx), criadoPorUsuarioId);
    }
}
