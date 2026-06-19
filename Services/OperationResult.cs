namespace AgenticContextEngine.Services
{
    // Retorno padrao para operacoes de servico que podem falhar por regra de negocio
    // (permissao, validacao), sem precisar lancar excecao nem usar tuplas avulsas.
    public class OperationResult
    {
        public bool Sucesso { get; }
        public string? Erro { get; }

        protected OperationResult(bool sucesso, string? erro)
        {
            Sucesso = sucesso;
            Erro = erro;
        }

        public static OperationResult Ok() => new(true, null);
        public static OperationResult Falha(string erro) => new(false, erro);
    }

    public class OperationResult<T> : OperationResult
    {
        public T? Dados { get; }

        private OperationResult(bool sucesso, string? erro, T? dados) : base(sucesso, erro)
        {
            Dados = dados;
        }

        public static OperationResult<T> Ok(T dados) => new(true, null, dados);
        public static new OperationResult<T> Falha(string erro) => new(false, erro, default);
    }
}
