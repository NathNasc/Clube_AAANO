using System;

namespace AaanoVo.ClubeAaano
{
    public class ResgatePromocaoVo : EntidadeBaseVo
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
        /// Referencia a chave estrangeira
        /// </summary>
        public PromocaoVo Promocao { get; set; }

        /// <summary>
        /// Referencia a chave estrangeira
        /// </summary>
        public AssinaturaPagSeguroVo AssinaturaPagSeguroVo { get; set; }
    }
}
