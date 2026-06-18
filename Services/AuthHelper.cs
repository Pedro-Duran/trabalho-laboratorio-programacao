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

        // Convidado nao tem um registro real na tabela Usuario (UsuarioId de sessao = "0"),
        // entao nao pode criar nada que exija um CriadoPorUsuarioId valido.
        public static bool PodeCriar(HttpContext ctx) =>
            GetPerfil(ctx) != "Convidado";

        // Administrador pode gerenciar qualquer registro; demais perfis (Regular, Convidado)
        // só podem gerenciar o que eles mesmos criaram.
        public static bool PodeGerenciar(HttpContext ctx, int? criadoPorUsuarioId)
        {
            if (IsAdmin(ctx)) return true;

            var usuarioId = GetUsuarioId(ctx);
            return usuarioId.HasValue && criadoPorUsuarioId.HasValue && usuarioId == criadoPorUsuarioId;
        }
    }
}
