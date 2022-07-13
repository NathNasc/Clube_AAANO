using System;

namespace AaanoVo.Base
{
    public class IntegracaoPagSeguroVo : EntidadeBaseVo
    {
        /// <summary>
        /// Chave qur diz o recurso sincronizado
        /// MIN.: - / MAX.: 100
        /// </summary>
        public string ChaveRecurso { get; set; }

        /// <summary>
        /// Data e hora da ultima sincronização feita
        /// </summary>
        public DateTime UltimaSincronizacao { get; set; }
    }
}
