using AaanoDto.ClubeAaano;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClubeAaanoSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um usuário
    /// </summary>
    public class ConfirmarIdentificacaoModel : BaseModel
    {
        /// <summary>
        /// Identificação criptografada
        /// </summary>
        public string Identificacao { set; get; }

        /// <summary>
        /// Confirmação do email da identificação
        /// </summary>
        [Display(Name = "Confirme seu email")]
        public string EmailConfirmacao { set; get; }

    }
}