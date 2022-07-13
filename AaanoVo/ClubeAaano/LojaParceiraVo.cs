namespace AaanoVo.ClubeAaano
{
    public class LojaParceiraVo : EntidadeBaseVo
    {
        /// <summary>
        /// Nome do usuário
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Endereço da loja
        /// MIN.: 20 / MAX.: 200
        /// </summary>
        public string Endereco { get; set; }

        /// <summary>
        /// Telefone de contato da loja
        /// MIN.: 10 / MAX.: 15
        /// </summary>
        public string Telefone { get; set; }
    }
}
