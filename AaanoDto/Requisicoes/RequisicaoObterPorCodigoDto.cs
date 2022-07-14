
using AaanoDto.Base;
using System;

namespace AaanoDto.Requisicoes
{
    /// <summary>
    /// Requisição utilizada para obter uma entidade
    /// </summary>
    public class RequisicaoObterPorCodigoDto : BaseRequisicaoDto
    {
        /// <summary>
        /// Código da entidade a ser obtida
        /// </summary>
        public string Codigo { get; set; }
    }
}
