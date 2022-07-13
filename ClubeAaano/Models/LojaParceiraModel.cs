using AaanoDto.ClubeAaano;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClubeAaanoSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um usuário
    /// </summary>
    public class LojaParceiraModel : BaseModel
    {
        /// <summary>
        /// Nome do usuário
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        [Required(ErrorMessage = "O nome da loja é obrigatório.")]
        [MaxLength(150, ErrorMessage = "O nome da loja deve ter até 150 caracteres")]
        [MinLength(3, ErrorMessage = "O nome da loja deve ter pelo menos 3 caracteres")]
        [Display(Name = "Nome")]
        public string Nome { set; get; }

        /// <summary>
        /// Endereço da loja
        /// MIN.: 20 / MAX.: 200
        /// </summary>
        [MaxLength(200, ErrorMessage = "O endereço da loja deve ter até 200 caracteres")]
        [MinLength(20, ErrorMessage = "O endereço da loja deve ter pelo menos 20 caracteres")]
        [Display(Name = "Endereço")]
        public string Endereco { set; get; }

        /// <summary>
        /// Telefone de contato da loja
        /// MIN.: 10 / MAX.: 15
        /// </summary>
        [MaxLength(15, ErrorMessage = "O telefone deve ter até 15 caracteres")]
        [MinLength(10, ErrorMessage = "O telefone deve ter pelo menos 10 caracteres")]
        [Display(Name = "Telefone")]
        public string Telefone { set; get; }

        /// <summary>
        /// Converte um usuário de DTO para Model
        /// </summary>
        /// <param name="lojaDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(LojaParceiraDto lojaDto, ref string mensagemErro)
        {
            try
            {
                this.Nome = string.IsNullOrWhiteSpace(lojaDto.Nome) ? "" : lojaDto.Nome.Trim();
                this.Endereco = string.IsNullOrWhiteSpace(lojaDto.Endereco) ? "" : lojaDto.Endereco.Trim();
                this.Telefone = string.IsNullOrWhiteSpace(lojaDto.Telefone) ? "" : lojaDto.Telefone.Trim();
                this.DataAlteracao = lojaDto.DataAlteracao;
                this.DataInclusao = lojaDto.DataInclusao;
                this.Id = lojaDto.Id;

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
        /// <param name="lojaDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref LojaParceiraDto lojaDto, ref string mensagemErro)
        {
            try
            {
                lojaDto.Nome = string.IsNullOrWhiteSpace(this.Nome) ? "" : this.Nome.Trim();
                lojaDto.Endereco = string.IsNullOrWhiteSpace(this.Endereco) ? "" : this.Endereco.Trim();
                lojaDto.Telefone = string.IsNullOrWhiteSpace(this.Telefone) ? "" : this.Telefone.Trim();
                lojaDto.DataAlteracao = this.DataAlteracao;
                lojaDto.DataInclusao = this.DataInclusao;
                lojaDto.Id = this.Id;

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