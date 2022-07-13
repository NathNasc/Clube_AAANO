using AaanoDto.ClubeAaano;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClubeAaanoSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um usuário
    /// </summary>
    public class PermissaoUsuarioModel : BaseModel
    {
        /// <summary>
        /// Usuário que a permissão pertence
        /// </summary>
        [Display(Name = "Usuário")]
        public Guid IdUsuario { get; set; }

        /// <summary>
        /// Loja que o usuário tem permissão
        /// </summary>
        [Display(Name = "Loja")]
        public Guid IdLojaParceira { get; set; }

        /// <summary>
        /// Nome do usuário da permissão
        /// </summary>
        public string NomeUsuario { get; set; }

        /// <summary>
        /// Nome da loja da permissão
        /// </summary>
        public string NomeLoja { get; set; }

        /// <summary>
        /// Indica se é para excluir a permissão
        /// </summary>
        public bool Excluir { get; set; }

        /// <summary>
        /// Converte um usuário de DTO para Model
        /// </summary>
        /// <param name="permissaoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(PermissaoUsuarioDto permissaoDto, ref string mensagemErro)
        {
            try
            {
                this.IdLojaParceira = permissaoDto.IdLojaParceira;
                this.IdUsuario = permissaoDto.IdUsuario;
                this.DataAlteracao = permissaoDto.DataAlteracao;
                this.DataInclusao = permissaoDto.DataInclusao;
                this.Id = permissaoDto.Id;

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
        /// <param name="permissaoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref PermissaoUsuarioDto permissaoDto, ref string mensagemErro)
        {
            try
            {
                permissaoDto.IdLojaParceira = this.IdLojaParceira;
                permissaoDto.IdUsuario = this.IdUsuario;
                permissaoDto.DataAlteracao = this.DataAlteracao;
                permissaoDto.DataInclusao = this.DataInclusao;
                permissaoDto.Id = this.Id;

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