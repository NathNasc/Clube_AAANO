using AaanoDto.ClubeAaano;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ClubeAaanoSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um usuário
    /// </summary>
    public class PromocaoModel : BaseModel
    {
        public PromocaoModel()
        {
            ListaPlanos = new List<PlanoPagSeguroModel>();
        }

        /// <summary>
        /// Nome do usuário
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        [Required(ErrorMessage = "O resumo é obrigatório.")]
        [MaxLength(150, ErrorMessage = "O resumo deve ter até 150 caracteres")]
        [MinLength(3, ErrorMessage = "O resumo deve ter pelo menos 3 caracteres")]
        [Display(Name = "Breve descritivo")]
        public string Resumo { set; get; }

        /// <summary>
        /// Endereço da loja
        /// MIN.: 20 / MAX.: 200
        /// </summary>
        [AllowHtml]
        [MaxLength(10000, ErrorMessage = "Os detalhes deve ter até 10.000 caracteres")]
        [Display(Name = "Detalhes exibidos aos clientes")]
        public string Detalhes { set; get; }

        /// <summary>
        /// Loja que oferece a promoção
        /// </summary>
        public Guid IdLojaParceira { get; set; }

        /// <summary>
        /// Telefone de contato da loja
        /// MIN.: 10 / MAX.: 15
        /// </summary>
        [Display(Name = "Loja")]
        public string NomeLojaParceira { set; get; }

        /// <summary>
        /// Lista de planos que tem direito a promoção
        /// </summary>
        public List<PlanoPagSeguroModel> ListaPlanos { get; set; }

        /// <summary>
        /// Converte um modelo de DTO para Model
        /// </summary>
        /// <param name="promocaoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(PromocaoDto promocaoDto, ref string mensagemErro)
        {
            try
            {
                this.Resumo = string.IsNullOrWhiteSpace(promocaoDto.Resumo) ? "" : promocaoDto.Resumo.Trim();
                this.Detalhes = string.IsNullOrWhiteSpace(promocaoDto.Detalhes) ? "" : promocaoDto.Detalhes.Trim();
                this.NomeLojaParceira = string.IsNullOrWhiteSpace(promocaoDto.NomeLojaParceira) ? "" : promocaoDto.NomeLojaParceira.Trim();
                this.DataAlteracao = promocaoDto.DataAlteracao;
                this.DataInclusao = promocaoDto.DataInclusao;
                this.IdLojaParceira = promocaoDto.IdLojaParceira;
                this.Id = promocaoDto.Id;

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
        /// <param name="promocaoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref PromocaoDto promocaoDto, ref string mensagemErro)
        {
            try
            {
                promocaoDto.Resumo = string.IsNullOrWhiteSpace(this.Resumo) ? "" : this.Resumo.Trim();
                promocaoDto.Detalhes = string.IsNullOrWhiteSpace(this.Detalhes) ? "" : this.Detalhes.Trim();
                promocaoDto.NomeLojaParceira = string.IsNullOrWhiteSpace(this.NomeLojaParceira) ? "" : this.NomeLojaParceira.Trim();
                promocaoDto.DataAlteracao = this.DataAlteracao;
                promocaoDto.DataInclusao = this.DataInclusao;
                promocaoDto.IdLojaParceira = this.IdLojaParceira;
                promocaoDto.Id = this.Id;

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