using System;

namespace AaanoVo.ClubeAaano
{
    public class PromocaoPlanoVo : EntidadeBaseVo
    {
        /// <summary>
        /// A promoção que é oferecida
        /// </summary>
        public Guid IdPromocao { get; set; }

        /// <summary>
        /// Plano que a promoção é oferecida
        /// </summary>
        public Guid IdPlanoPagSeguro { get; set; }

        /// <summary>
        /// Referencia a chave estrangeira
        /// </summary>
        public PromocaoVo Promocao { get; set; }

        /// <summary>
        /// Referencia a chave estrangeira
        /// </summary>
        public PlanoPagSeguroVo PlanoPagSeguro { get; set; }
    }
}
