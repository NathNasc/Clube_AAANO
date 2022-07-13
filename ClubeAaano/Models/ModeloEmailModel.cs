using AaanoDto.ClubeAaano;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ClubeAaanoSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um modelo de email
    /// </summary>
    public class ModeloEmailModel : BaseModel
    {
        /// <summary>
        /// Título do email
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        [Required(ErrorMessage = "O título é obrigatório.")]
        [MaxLength(150, ErrorMessage = "O título deve ter até 100 caracteres")]
        [MinLength(3, ErrorMessage = "O título deve ter pelo menos 3 caracteres")]
        [Display(Name = "Assunto")]
        public string Assunto { set; get; }

        /// <summary>
        /// Corpo do email
        /// </summary>
        [AllowHtml]
        [Display(Name = "Corpo do email")]
        public string Corpo { set; get; }

        /// <summary>
        /// Converte um modelo de DTO para Model
        /// </summary>
        /// <param name="modeloEmailDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(ModeloEmailDto modeloEmailDto, ref string mensagemErro)
        {
            try
            {
                this.Assunto = string.IsNullOrWhiteSpace(modeloEmailDto.Assunto) ? "" : modeloEmailDto.Assunto.Trim();
                this.Corpo = string.IsNullOrWhiteSpace(modeloEmailDto.Corpo) ? "" : modeloEmailDto.Corpo.Trim();
                this.DataAlteracao = modeloEmailDto.DataAlteracao;
                this.DataInclusao = modeloEmailDto.DataInclusao;
                this.Id = modeloEmailDto.Id;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um modelo de Model para Dto
        /// </summary>
        /// <param name="modeloEmailDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref ModeloEmailDto modeloEmailDto, ref string mensagemErro)
        {
            try
            {
                modeloEmailDto.Assunto = string.IsNullOrWhiteSpace(this.Assunto) ? "" : this.Assunto.Trim();
                modeloEmailDto.Corpo = string.IsNullOrWhiteSpace(this.Corpo) ? "" : this.Corpo.Trim();
                modeloEmailDto.DataAlteracao = this.DataAlteracao;
                modeloEmailDto.DataInclusao = this.DataInclusao;
                modeloEmailDto.Id = this.Id;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                return false;
            }
        }
    }
}