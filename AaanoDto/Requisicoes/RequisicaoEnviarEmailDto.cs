using AaanoDto.Base;
using System;
using System.Collections.Generic;

namespace AaanoDto.Requisicoes
{
    /// <summary>
    /// Requisição utilizada para obter uma lista de entidades
    /// </summary>
    public class RequisicaoEnviarEmailDto : BaseRequisicaoDto
    {
        public RequisicaoEnviarEmailDto()
        {
            this.ListaFiltros = new Dictionary<string, string>();
        }

        /// <summary>
        /// Filtros disponíveis para a entidade
        /// </summary>
        public Dictionary<string, string> ListaFiltros { get; set; }

        /// <summary>
        /// Identifica o modelo a ser enviado
        /// </summary>
        public Guid IdModeloEmail { get; set; }
    }
}
