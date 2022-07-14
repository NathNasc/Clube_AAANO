using AaanoDto.Base;
using System;
using System.Collections.Generic;

namespace AaanoDto.Requisicoes
{
    /// <summary>
    /// Requisição utilizada para incluir/editar uma lista de entidades
    /// </summary>
    public class RequisicaoListaEntidadesDto<T> : BaseRequisicaoDto where T : BaseEntidadeDto
    {
        public RequisicaoListaEntidadesDto()
        {
            ListaEntidadesDto = new List<T>();
        }

        /// <summary>
        /// Lista de entidades a ser processada
        /// </summary>
        public List<T> ListaEntidadesDto { get; set; }

        /// <summary>
        /// Id que une todas as entidades de lista
        /// </summary>
        public Guid IdComum { get; set; }
    }
}
