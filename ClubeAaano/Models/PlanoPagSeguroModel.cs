using AaanoDto.ClubeAaano;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClubeAaanoSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um usuário
    /// </summary>
    public class PlanoPagSeguroModel : BaseModel
    {
        /// <summary>
        /// Nome do usuário
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        [Required(ErrorMessage = "O nome do plano é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome do plano deve ter até 100 caracteres")]
        [MinLength(3, ErrorMessage = "O nome do plano deve ter pelo menos 3 caracteres")]
        [Display(Name = "Nome")]
        public string Nome { set; get; }

        /// <summary>
        /// Código simplificado informado no cadastro do plano no pagseguro
        /// MIN.: 3 / MAX.: 40
        /// </summary>
        [Required(ErrorMessage = "O código do plano é obrigatório, deve ser igual ao cadastrado no PagSeguro")]
        [MaxLength(40, ErrorMessage = "O código do plano deve ter até 40 caracteres")]
        [MinLength(3, ErrorMessage = "O código do plano deve ter pelo menos 3 caracteres")]
        [Display(Name = "Código PagSeguro")]
        public string CodigoSimplificado { get; set; }

        /// <summary>
        /// Usado para remover o plano de uma promoção
        /// </summary>
        public bool Excluir { get; set; }

        /// <summary>
        /// Converte um usuário de DTO para Model
        /// </summary>
        /// <param name="planoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(PlanoPagSeguroDto planoDto, ref string mensagemErro)
        {
            try
            {
                this.CodigoSimplificado = string.IsNullOrWhiteSpace(planoDto.CodigoSimplificado) ? "" : planoDto.CodigoSimplificado.Trim();
                this.Nome = string.IsNullOrWhiteSpace(planoDto.Nome) ? "" : planoDto.Nome.Trim();
                this.DataAlteracao = planoDto.DataAlteracao;
                this.DataInclusao = planoDto.DataInclusao;
                this.Id = planoDto.Id;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um usuário de Model para Dto
        /// </summary>
        /// <param name="planoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref PlanoPagSeguroDto planoDto, ref string mensagemErro)
        {
            try
            {
                planoDto.CodigoSimplificado = string.IsNullOrWhiteSpace(this.CodigoSimplificado) ? "" : this.CodigoSimplificado.Trim();
                planoDto.Nome = string.IsNullOrWhiteSpace(this.Nome) ? "" : this.Nome.Trim();
                planoDto.DataAlteracao = this.DataAlteracao;
                planoDto.DataInclusao = this.DataInclusao;
                planoDto.Id = this.Id;

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