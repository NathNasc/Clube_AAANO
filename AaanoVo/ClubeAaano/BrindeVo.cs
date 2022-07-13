using System;

namespace AaanoVo.ClubeAaano
{
    public class BrindeVo : EntidadeBaseVo
    {
        /// <summary>
        /// Breve descrição do brinde
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Id do assinante que ganhou o brinde
        /// </summary>
        public Guid? IdAssinaturaPagSeguro { get; set; }

        /// <summary>
        /// Data do sorteio
        /// </summary>
        public DateTime? Sorteio { get; set; }

        /// <summary>
        /// Indica se o brinde já foi entregue
        /// </summary>
        public bool Entregue { get; set; }

        /// <summary>
        /// Assinante que ganhou o prêmio
        /// </summary>
        public AssinaturaPagSeguroVo Assinante { get; set; }
    }
}
