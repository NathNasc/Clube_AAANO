using AaanoDto.ClubeAaano;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClubeAaanoSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um usuário
    /// </summary>
    public class BrindeModel : BaseModel
    {
        /// <summary>
        /// Nome do usuário
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        [Required(ErrorMessage = "A descrição do brinde é obrigatório.")]
        [MaxLength(150, ErrorMessage = "A descrição do brinde deve ter até 150 caracteres")]
        [MinLength(3, ErrorMessage = "A descrição do brinde deve ter pelo menos 3 caracteres")]
        [Display(Name = "Descrição")]
        public string Descricao { set; get; }

        /// <summary>
        /// Endereço da loja
        /// MIN.: 20 / MAX.: 200
        /// </summary>
        [Display(Name = "Assinante")]
        public string NomeAssinante { set; get; }

        /// <summary>
        /// Indica se o brinde já foi entregue ao ganhador
        /// </summary>
        [Display(Name = "Entregue")]
        public bool Entregue { set; get; }

        /// <summary>
        /// Data do sorteio
        /// </summary>
        [Display(Name = "Sorteado em")]
        public DateTime? Sorteio { get; set; }

        /// <summary>
        /// Id do ganhador
        /// </summary>
        public Guid? IdAssinaturaPagSeguro { get; set; }

        /// <summary>
        /// Converte um usuário de DTO para Model
        /// </summary>
        /// <param name="brindeDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(BrindeDto brindeDto, ref string mensagemErro)
        {
            try
            {
                this.Descricao = string.IsNullOrWhiteSpace(brindeDto.Descricao) ? "" : brindeDto.Descricao.Trim();
                this.NomeAssinante = string.IsNullOrWhiteSpace(brindeDto.NomeAssinante) ? "" : brindeDto.NomeAssinante.Trim();
                this.IdAssinaturaPagSeguro = brindeDto.IdAssinaturaPagSeguro;
                this.Sorteio = brindeDto.Sorteio;
                this.Entregue = brindeDto.Entregue;
                this.DataAlteracao = brindeDto.DataAlteracao;
                this.DataInclusao = brindeDto.DataInclusao;
                this.Id = brindeDto.Id;

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
        /// <param name="brindeDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref BrindeDto brindeDto, ref string mensagemErro)
        {
            try
            {
                brindeDto.Descricao = string.IsNullOrWhiteSpace(this.Descricao) ? "" : this.Descricao.Trim();
                brindeDto.NomeAssinante = string.IsNullOrWhiteSpace(this.NomeAssinante) ? "" : this.NomeAssinante.Trim();
                brindeDto.IdAssinaturaPagSeguro = this.IdAssinaturaPagSeguro;
                brindeDto.Sorteio = this.Sorteio;
                brindeDto.Entregue = this.Entregue;
                brindeDto.DataAlteracao = this.DataAlteracao;
                brindeDto.DataInclusao = this.DataInclusao;
                brindeDto.Id = this.Id;

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