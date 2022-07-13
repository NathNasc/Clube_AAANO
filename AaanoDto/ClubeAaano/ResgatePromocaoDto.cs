using AaanoDto.Base;
using System;

namespace AaanoDto.ClubeAaano
{
    public class ResgatePromocaoDto : BaseEntidadeDto
    {
        /// <summary>
        /// Promoção a ser resgatada
        /// </summary>
        public Guid IdPromocao { get; set; }

        /// <summary>
        /// Id do assinante que pode resgatar a promoção
        /// </summary>
        public Guid IdAssinaturaPagSeguro { get; set; }

        /// <summary>
        /// Nome do assinante do resgate
        /// </summary>
        public string NomeAssinante { get; set; }

        /// <summary>
        /// Código simplificado da assinatura
        /// </summary>
        public string CodigoSimplificadoAssinatura { get; set; }

        /// <summary>
        /// Data e hora que foi feito o resgate
        /// </summary>
        public DateTime? Resgate { get; set; }

        /// <summary>
        /// Até quando pode ser resgatado
        /// </summary>
        public DateTime Validade { get; set; }

        /// <summary>
        /// Usuário que fez o resgate
        /// MAX: 150
        /// </summary>
        public string NomeUsuarioResgate { get; set; }

        /// <summary>
        /// Resumo da promoção a ser resgatada
        /// </summary>
        public string ResumoPromocao { get; set; }

        /// <summary>
        /// Usuário que fez o resgate
        /// </summary>
        public string NomeLojaParceira { get; set; }

    }
}
