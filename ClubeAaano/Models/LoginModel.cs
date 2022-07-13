using AaanoDto.Requisicoes;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClubeAaanoSite.Models
{
    /// <summary>
    /// Classe que representa os dados necessários para fazer o login
    /// </summary>
    public class LoginModel : BaseModel
    {
        /// <summary>
        /// Email do usuário
        /// MIN: 10
        /// MAX: 100
        /// </summary>
        public string EmailLogin { set; get; }

        /// <summary>
        /// Senha do usuário
        /// MIN: 5
        /// MAX: 50
        /// </summary>
        [DataType(DataType.Password)]
        public string SenhaLogin { set; get; }

        /// <summary>
        /// Converte o login de Model para Dto
        /// </summary>
        /// <param name="requisicaoLoginDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref RequisicaoFazerLoginDto requisicaoLoginDto, ref string mensagemErro)
        {
            try
            {
                requisicaoLoginDto.Email = this.EmailLogin;
                requisicaoLoginDto.Senha = this.SenhaLogin;

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