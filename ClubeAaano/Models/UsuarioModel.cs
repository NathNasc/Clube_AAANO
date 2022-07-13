using AaanoDto.ClubeAaano;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClubeAaanoSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um usuário
    /// </summary>
    public class UsuarioModel : BaseModel
    {
        /// <summary>
        /// Nome do usuário
        /// MIN: 3
        /// MAX: 150
        /// </summary>
        [Required(ErrorMessage = "O nome do usuário é obrigatório.")]
        [MaxLength(150, ErrorMessage = "O nome do usuário deve ter até 150 caracteres")]
        [MinLength(3, ErrorMessage = "O nome do usuário deve ter pelo menos 3 caracteres")]
        [Display(Name = "Nome")]
        public string Nome { set; get; }

        /// <summary>
        /// Email do usuário
        /// MIN: 10
        /// MAX: 100
        /// </summary>
        [Required(ErrorMessage = "O email do usuário é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um email válido para o usuário")]
        [MaxLength(100, ErrorMessage = "O email do usuário deve ter até 100 caracteres")]
        [MinLength(10, ErrorMessage = "O email do usuário deve ter pelo menos 10 caracteres")]
        [Display(Name = "Email")]
        public string Email { set; get; }

        /// <summary>
        /// Senha do usuário
        /// MIN: 5
        /// MAX: 50
        /// </summary>
        [MaxLength(50, ErrorMessage = "A senha deve ter até 50 caracteres")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { set; get; }

        /// <summary>
        /// Confirma se a senha digitada está correta
        /// MIN: 5
        /// MAX: 50
        /// </summary>
        [MaxLength(50, ErrorMessage = "A confirmação da senha deve ter até 50 caracteres")]
        [MinLength(6, ErrorMessage = "A confirmação da senha deve ter pelo menos 6 caracteres")]
        [Compare("Senha", ErrorMessage = "As senhas digitadas não são iguais")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirme a senha")]
        public string ConfirmarSenha { set; get; }

        /// <summary>
        /// Senha usada pelo usuário para confirmar a alteração
        /// MIN: 5
        /// MAX: 50
        /// </summary>
        [MaxLength(50, ErrorMessage = "A senha deve ter até 50 caracteres")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Alterar senha")]
        public string SenhaAntiga { set; get; }

        /// <summary>
        /// Indica se o usuário é administrador
        /// </summary>
        [Display(Name = "Administrador")]
        public bool Administrador { get; set; }

        /// <summary>
        /// Converte um usuário de DTO para Model
        /// </summary>
        /// <param name="usuarioDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(UsuarioDto usuarioDto, ref string mensagemErro)
        {
            try
            {
                this.Nome = string.IsNullOrWhiteSpace(usuarioDto.Nome) ? "" : usuarioDto.Nome.Trim();
                this.Email = string.IsNullOrWhiteSpace(usuarioDto.Email) ? "" : usuarioDto.Email.Trim();
                this.DataAlteracao = usuarioDto.DataAlteracao;
                this.DataInclusao = usuarioDto.DataInclusao;
                this.Id = usuarioDto.Id;
                this.Senha = usuarioDto.Senha;
                this.Administrador = usuarioDto.Administrador;

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
        /// <param name="usuarioDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref UsuarioDto usuarioDto, ref string mensagemErro)
        {
            try
            {
                usuarioDto.Nome = string.IsNullOrWhiteSpace(this.Nome) ? "" : this.Nome.Trim();
                usuarioDto.SenhaAntiga = string.IsNullOrWhiteSpace(this.SenhaAntiga) ? "" : this.SenhaAntiga.Trim();
                usuarioDto.Email = string.IsNullOrWhiteSpace(this.Email) ? "" : this.Email.Trim();
                usuarioDto.DataAlteracao = this.DataAlteracao;
                usuarioDto.DataInclusao = this.DataInclusao;
                usuarioDto.Id = this.Id;
                usuarioDto.Senha = this.Senha;
                usuarioDto.Administrador = this.Administrador;

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