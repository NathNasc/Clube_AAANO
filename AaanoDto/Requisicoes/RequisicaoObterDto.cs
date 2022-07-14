
using AaanoDto.Base;
using System;

namespace AaanoDto.Requisicoes
{
    /// <summary>
    /// Requisição utilizada para obter uma entidade
    /// </summary>
    public class RequisicaoObterDto : BaseRequisicaoDto
    {
        /// <summary>
        /// Id a ser obtido
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome do usuário da requisição quando for necessário logar
        /// </summary>
        public string NomeUsuario{ get; set; }
    }
}
