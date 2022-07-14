using AaanoDto.Base;
using AaanoDto.ClubeAaano;
using System.Collections.Generic;

namespace AaanoDto.Retornos
{
    public class RetornoObterInformacoesAssinaturaDto : RetornoDto
    {
        public RetornoObterInformacoesAssinaturaDto()
        {
            this.ListaPromocoes = new List<PromocaoComLojaDto>();
            AssinaturaDto = new AssinaturaPagSeguroDto();
        }

        /// <summary>
        /// Lista de promoções disponíveis
        /// </summary>
        public List<PromocaoComLojaDto> ListaPromocoes { get; set; }

        /// <summary>
        /// Informações da assinatura
        /// </summary>
        public AssinaturaPagSeguroDto AssinaturaDto { get; set; }

    }
}
