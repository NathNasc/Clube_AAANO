namespace AaanoVo.ClubeAaano
{
    public class PlanoPagSeguroVo : EntidadeBaseVo
    {
        /// <summary>
        /// Nome do plano cadastrado no PagSeguro
        /// MIN.: 3 / MAX.: 100
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Código simplificado informado no cadastro do plano no pagseguro
        /// MIN.: 3 / MAX.: 40
        /// </summary>
        public string CodigoSimplificado { get; set; }
    }
}
