using AaanoDto.Base;

namespace AaanoDto.ClubeAaano
{
    public class ModeloEmailDto : BaseEntidadeDto
    {
        /// <summary>
        /// Assinto do email
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        public string Assunto { get; set; }

        /// <summary>
        /// Corpo HTML do email
        /// MIN.: 0 / MAX.: 50.000
        /// </summary>
        public string Corpo { get; set; }
    }
}
