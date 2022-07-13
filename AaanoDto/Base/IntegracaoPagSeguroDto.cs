using AaanoDto.Base;
using System;
using System.Collections.Generic;

namespace AaanoDto.Base
{
    public class IntegracaoPagSeguroDto : BaseEntidadeDto
    {
        /// <summary>
        /// Breve título da promoção oferecida
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        public string Chave { get; set; }

        /// <summary>
        /// Detalhes da promoção oferecida
        /// MIN.: 0 / MAX.: 10.000
        /// </summary>
        public DateTime UltimaSincronizacao { get; set; }
    }
}
