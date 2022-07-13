using AaanoDto.Base;
using System;

namespace AaanoDto.ClubeAaano
{
    public class EmailEnviadoDto : BaseEntidadeDto
    {
        /// <summary>
        /// Indica se houve erro para enviar o email
        /// </summary>
        public bool SucessoEnvio { get; set; }

        /// <summary>
        /// Id do modelo enviado
        /// </summary>
        public Guid IdModeloEmail { get; set; }

        /// <summary>
        /// Email de destino
        /// MAX: 60
        /// </summary>
        public Guid IdAssinaturaPagSeguro { get; set; }

        /// <summary>
        /// Corpo do email
        /// </summary>
        public string Corpo { set; get; }

        /// <summary>
        /// Assunto do email
        /// </summary>
        public string Assunto { set; get; }

        /// <summary>
        /// Nome do assinante
        /// </summary>
        public string NomeAssinante { set; get; }

    }
}
