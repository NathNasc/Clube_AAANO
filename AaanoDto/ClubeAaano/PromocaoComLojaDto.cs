using System;

namespace AaanoDto.ClubeAaano
{
    public class PromocaoComLojaDto : PromocaoDto
    {
        /// <summary>
        /// Endereço da loja
        /// MIN.: 20 / MAX.: 200
        /// </summary>
        public string EnderecoLojaParceira { get; set; }

        /// <summary>
        /// Telefone de contato da loja
        /// MIN.: 10 / MAX.: 15
        /// </summary>
        public string TelefoneLojaParceira { get; set; }
    }
}
