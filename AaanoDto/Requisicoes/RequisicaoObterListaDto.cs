using AaanoDto.Base;
using System.Collections.Generic;

namespace AaanoDto.Requisicoes
{
    /// <summary>
    /// Requisição utilizada para obter uma lista de entidades
    /// </summary>
    public class RequisicaoObterListaDto : BaseRequisicaoDto
    {
        public RequisicaoObterListaDto()
        {
            this.ListaFiltros = new Dictionary<string, string>();
            this.CampoOrdem = "NOME";
        }

        /// <summary>
        /// Filtros disponíveis para a entidade
        /// </summary>
        public Dictionary<string, string> ListaFiltros { get; set; }

        /// <summary>
        /// Campo de ordem dos registros
        /// </summary>
        public string CampoOrdem { get; set; }

        /// <summary>
        /// Número da página de resultados a ser retornada
        /// </summary>
        public int Pagina { get; set; }

        /// <summary>
        /// Número de itens por página
        /// </summary>
        public int NumeroItensPorPagina { get; set; }

        /// <summary>
        /// Indica que a pesquisa não será paginada
        /// </summary>
        public bool NaoPaginarPesquisa { get; set; }
    }
}
