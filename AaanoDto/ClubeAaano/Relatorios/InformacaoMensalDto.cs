using AaanoDto.Base;
using AaanoDto.Retornos;

namespace AaanoDto.ClubeAaano.Relatorios
{
    public class InformacaoMensalDto 
    {
        /// <summary>
        /// Representa o mês (Jan, Fev, Mar, Abr, Mai, Jun, Jul, Ago, Set, Out, Nov, Dez)
        /// </summary>
        public string Mes { get; set; }

        /// <summary>
        /// Quantidade de registros encontradosmo
        /// </summary>
        public int Quantidade { get; set; }
    }
}
