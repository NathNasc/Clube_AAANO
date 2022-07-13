using System;
using System.ComponentModel.DataAnnotations;

namespace ClubeAaanoSite.Models
{
    /// <summary>
    /// Model com as informações da entidade a ser excluída
    /// </summary>
    public class ExclusaoModel
    {
        /// <summary>
        /// Id que identifica a entidade a ser excluída
        /// </summary>
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Descrição de entidade a ser excluída
        /// </summary>
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        /// <summary>
        /// Caso a entidade deletada venha de um cadastro pai
        /// </summary>
        public string IdPai { get; set; }

        /// <summary>
        /// Justificatuva para a ação
        /// </summary>
        [Display(Name = "Justificativa")]
        public string Justificativa { get; set; }

    }
}