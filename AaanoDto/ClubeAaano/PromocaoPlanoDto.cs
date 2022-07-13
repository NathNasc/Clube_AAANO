using AaanoDto.Base;
using System;

namespace AaanoDto.ClubeAaano
{
    public class PromocaoPlanoDto : BaseEntidadeDto
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
        /// Nome do plano
        /// </summary>
        public string NomePlanoPagSeguro { get; set; }

        /// <summary>
        /// Código do plano
        /// </summary>
        public string CodigoSimplificado { get; set; }
    }
}
