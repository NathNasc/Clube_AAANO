using System;

namespace AaanoVo.ClubeAaano
{
    public class PromocaoVo : EntidadeBaseVo
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
        /// Referencia a chave estrangeira
        /// </summary>
        public LojaParceiraVo LojaParceira { get; set; }
    }
}
