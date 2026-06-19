using AgenticContextEngine.Models;

namespace AgenticContextEngine.Services
{
    public interface IAuthService
    {
        Task<OperationResult<UsuarioLoginDto>> LoginAsync(string email, string senha);
    }
}
