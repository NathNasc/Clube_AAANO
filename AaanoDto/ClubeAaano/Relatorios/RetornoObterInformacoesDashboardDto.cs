using AaanoDto.Base;
using AaanoDto.Retornos;
using System.Collections.Generic;

namespace AaanoDto.ClubeAaano.Relatorios
{
    public class RetornoObterInformacoesDashboardDto : RetornoDto
    {
        public RetornoObterInformacoesDashboardDto()
        {
            ListaAssinaturasPorMes = new List<InformacaoMensalDto>();
            ListaResgatesPorMes = new List<InformacaoMensalDto>();
        }

        /// <summary>
        /// Número de assinantes cadastrados no mês
        /// </summary>
        public int QuantidadeNovasAssinaturas { get; set; }

        /// <summary>
        /// Quantidade de resgates do mês
        /// </summary>
        public int QuantidadeResgates { get; set; }

        /// <summary>
        /// Quantidade de assinaturas que foram canceladas
        /// </summary>
        public int QuantidadeAssinaturasCanceladas { get; set; }

        /// <summary>
        /// Assinaturas com pagamento pendente
        /// </summary>
        public int QuantidadeAssinaturasPagamentoPendente { get; set; }

        /// <summary>
        /// Quantidade de assinaturas ativas existentes
        /// </summary>
        public int QuantidadeAssinaturasAtivas { get; set; }

        /// <summary>
        /// Quantidade de lojas parceiras cadastradas
        /// </summary>
        public int QuantidadeLojas { get; set; }

        /// <summary>
        /// Quantidade de brindes não enviados
        /// </summary>
        public int QuantidadeBrindesNaoEnviados { get; set; }

        /// <summary>
        /// Quantidade de total de assinaturas canceladas
        /// </summary>
        public int QuantidadeTotalCancelamento { get; set; }

        /// <summary>
        /// Percentual as assinaturas de 15 reias
        /// </summary>
        public double PercentualAssinaturas15 { get; set; }

        /// <summary>
        /// Percentual as assinaturas de 30 reias
        /// </summary>
        public double PercentualAssinaturas30 { get; set; }

        /// <summary>
        /// Percentual as assinaturas de 50 reias
        /// </summary>
        public double PercentualAssinaturas50 { get; set; }

        /// <summary>
        /// Percentual as assinaturas de 100 reias
        /// </summary>
        public double PercentualAssinaturas100 { get; set; }

        /// <summary>
        /// Percentual as assinaturas de 200 reias
        /// </summary>
        public double PercentualAssinaturas200 { get; set; }

        /// <summary>
        /// Assinaturas feitas a cada mês do ano corrente
        /// </summary>
        public List<InformacaoMensalDto> ListaAssinaturasPorMes { get; set; }

        /// <summary>
        /// Resgates feitos a cada mês do ano corrente
        /// </summary>
        public List<InformacaoMensalDto> ListaResgatesPorMes { get; set; }

    }
}
