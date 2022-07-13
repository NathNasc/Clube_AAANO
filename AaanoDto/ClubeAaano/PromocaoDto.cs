using AaanoDto.Base;
using System;
using System.Collections.Generic;

namespace AaanoDto.ClubeAaano
{
    public class PromocaoDto : BaseEntidadeDto
    {
        /// <summary>
        /// Loja que oferece a promoção
        /// </summary>
        public Guid IdLojaParceira { get; set; }

        /// <summary>
        /// Breve título da promoção oferecida
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        public string Resumo { get; set; }

        /// <summary>
        /// Detalhes da promoção oferecida
        /// MIN.: 0 / MAX.: 10.000
        /// </summary>
        public string Detalhes { get; set; }

        /// <summary>
        /// Nome da loja
        /// </summary>
        public string NomeLojaParceira { get; set; }

    }
}
