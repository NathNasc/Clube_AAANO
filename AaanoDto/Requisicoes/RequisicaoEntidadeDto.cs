using AaanoDto.Base;

namespace AaanoDto.Requisicoes
{
    /// <summary>
    /// Requisição utilizada para incluir/editar uma entidade
    /// </summary>
    public class RequisicaoEntidadeDto<T> : BaseRequisicaoDto where T : BaseEntidadeDto
    {
        /// <summary>
        /// Entidade a ser processada
        /// </summary>
        public T EntidadeDto { get; set; }
    }
}
