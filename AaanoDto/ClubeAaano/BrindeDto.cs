using AaanoDto.Base;
using System;

namespace AaanoDto.ClubeAaano
{
    public class BrindeDto : BaseEntidadeDto
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
        /// Nome do assinante
        /// </summary>
        public string NomeAssinante { get; set; }
    }
}
