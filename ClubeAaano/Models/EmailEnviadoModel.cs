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
    public class EmailEnviadoModel : BaseModel
    {
        /// <summary>
        /// Nome do assinante
        /// </summary>
        [Display(Name = "Assinante")]
        public string NomeAssinante { set; get; }

        /// <summary>
        /// Indica se houve erro para enviar o email
        /// </summary>
        [AllowHtml]
        [Display(Name = "Sucesso")]
        public bool SucessoEnvio { get; set; }

        /// <summary>
        /// Assunto do email
        /// </summary>
        [Display(Name = "Assunto")]
        public string Assunto { set; get; }

        /// <summary>
        /// Corpo do email
        /// </summary>
        [AllowHtml]
        [Display(Name = "Conteúdo enviado")]
        public string Corpo { set; get; }

        /// <summary>
        /// Id do modelo enviado
        /// </summary>
        public Guid IdModeloEmail { get; set; }

        /// <summary>
        /// Email de destino
        /// MAX: 60
        /// </summary>
        public Guid IdAssinaturaPagSeguro { get; set; }

        /// <summary>
        /// Converte um modelo de DTO para Model
        /// </summary>
        /// <param name="emailEnviadoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(EmailEnviadoDto emailEnviadoDto, ref string mensagemErro)
        {
            try
            {
                this.NomeAssinante = string.IsNullOrWhiteSpace(emailEnviadoDto.NomeAssinante) ? "" : emailEnviadoDto.NomeAssinante.Trim();
                this.Assunto = string.IsNullOrWhiteSpace(emailEnviadoDto.Assunto) ? "" : emailEnviadoDto.Assunto.Trim();
                this.Corpo = string.IsNullOrWhiteSpace(emailEnviadoDto.Corpo) ? "" : emailEnviadoDto.Corpo.Trim();
                this.IdModeloEmail = emailEnviadoDto.IdModeloEmail;
                this.SucessoEnvio = emailEnviadoDto.SucessoEnvio;
                this.IdAssinaturaPagSeguro = emailEnviadoDto.IdAssinaturaPagSeguro;
                this.DataAlteracao = emailEnviadoDto.DataAlteracao;
                this.DataInclusao = emailEnviadoDto.DataInclusao;
                this.Id = emailEnviadoDto.Id;

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
        /// <param name="emailEnviadoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref EmailEnviadoDto emailEnviadoDto, ref string mensagemErro)
        {
            try
            {
                emailEnviadoDto.Assunto = string.IsNullOrWhiteSpace(this.Assunto) ? "" : this.Assunto.Trim();
                emailEnviadoDto.Corpo = string.IsNullOrWhiteSpace(this.Corpo) ? "" : this.Corpo.Trim();
                emailEnviadoDto.NomeAssinante = string.IsNullOrWhiteSpace(this.NomeAssinante) ? "" : this.NomeAssinante.Trim();
                emailEnviadoDto.IdModeloEmail = this.IdModeloEmail;
                emailEnviadoDto.SucessoEnvio = this.SucessoEnvio;
                emailEnviadoDto.IdAssinaturaPagSeguro = this.IdAssinaturaPagSeguro;
                emailEnviadoDto.DataAlteracao = this.DataAlteracao;
                emailEnviadoDto.DataInclusao = this.DataInclusao;
                emailEnviadoDto.Id = this.Id;

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