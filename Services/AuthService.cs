using AgenticContextEngine.Data.Repositories;
using AgenticContextEngine.Models;

namespace AgenticContextEngine.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _repository;

        public AuthService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult<UsuarioLoginDto>> LoginAsync(string email, string senha)
        {
            var usuario = await _repository.GetByEmailAtivoComPerfilAsync(email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash))
                return OperationResult<UsuarioLoginDto>.Falha("Email ou senha inválidos.");

            _repository.AdicionarLogAuditoria(new LogAuditoria
            {
                UsuarioId = usuario.Id,
                Acao = "LOGIN",
                Entidade = "Usuario",
                EntidadeId = usuario.Id,
                Detalhes = "Login realizado com sucesso"
            });
            await _repository.SaveChangesAsync();

            return OperationResult<UsuarioLoginDto>.Ok(new UsuarioLoginDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Perfil = usuario.PerfilAcesso?.Nome ?? ""
            });
        }
    }
}
