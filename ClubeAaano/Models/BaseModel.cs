using System;
using System.ComponentModel.DataAnnotations;

namespace ClubeAaanoSite.Models
{
    /// <summary>
    /// Model com os campos em comum para as entidades
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// Data que a entidade foi incluída no sistema
        /// </summary>
        [Display(Name = "Cadastro")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime DataInclusao { get; set; }

        /// <summary>
        /// Data da última alteração da entidade
        /// </summary>
        [Display(Name = "Última alteração")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime? DataAlteracao { get; set; }

        /// <summary>
        /// Id que identifica unicamente a entidade
        /// </summary>
        [Display(Name = "Id")]
        public Guid Id { get; set; }
    }
}