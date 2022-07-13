using System;

namespace AaanoVo.ClubeAaano
{
    public class EmailEnviadoVo : EntidadeBaseVo
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
        /// Referencia a chave estrangeira
        /// </summary>
        public ModeloEmailVo ModeloEmail { get; set; }

        /// <summary>
        /// Referencia a chave estrangeira
        /// </summary>
        public AssinaturaPagSeguroVo Assinatura { get; set; }

    }
}
