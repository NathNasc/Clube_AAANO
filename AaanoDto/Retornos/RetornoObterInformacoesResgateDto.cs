using AaanoDto.Base;
using AaanoDto.ClubeAaano;
using System.Collections.Generic;

namespace AaanoDto.Retornos
{
    public class RetornoObterInformacoesResgateDto : RetornoDto
    {
        public RetornoObterInformacoesResgateDto()
        {
            this.ListaPromocoesDisponiveis = new List<ResgatePromocaoDto>();
            this.ListaUltimosResgates = new List<ResgatePromocaoDto>();
            this.AssinaturaPagSeguroDto = new AssinaturaPagSeguroDto();
        }

        /// <summary>
        /// Lista de promoções a serem resgatadas
        /// </summary>
        public List<ResgatePromocaoDto> ListaPromocoesDisponiveis { get; set; }

        /// <summary>
        /// Lista das 5 ultimas promoções resgatadas
        /// </summary>
        public List<ResgatePromocaoDto> ListaUltimosResgates { get; set; }

        /// <summary>
        /// Assinatura das promoções 
        /// </summary>
        public AssinaturaPagSeguroDto AssinaturaPagSeguroDto { get; set; }
    }
}
