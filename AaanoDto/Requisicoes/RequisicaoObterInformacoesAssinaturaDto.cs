namespace AaanoDto.Requisicoes
{
    /// <summary>
    /// Requisição utilizada para fazer o login
    /// </summary>
    public class RequisicaoObterInformacoesAssinaturaDto
    {
        /// <summary>
        /// Email confirmado pelo assinante
        /// </summary>
        public string EmailConfirmacao { get; set; }

        /// <summary>
        /// Identificação específica
        /// </summary>
        public string Identificacao { get; set; }
    }
}
