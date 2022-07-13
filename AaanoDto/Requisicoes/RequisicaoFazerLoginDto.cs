namespace AaanoDto.Requisicoes
{
    /// <summary>
    /// Requisição utilizada para fazer o login
    /// </summary>
    public class RequisicaoFazerLoginDto
    {
        /// <summary>
        /// Email do usuário
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Senha do usuário
        /// </summary>
        public string Senha { get; set; }
    }
}
