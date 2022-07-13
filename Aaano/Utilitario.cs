using AaanoBll.Base;
using AaanoBll.ClubeAaano;
using AaanoDto.Base;
using AaanoDto.Requisicoes;
using AaanoDto.Retornos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aaano
{
    internal class Utilitario
    {
        /// <summary>
        /// Preenche uma requição com a identificação e id do usuário
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <returns></returns>
        public static bool RetornarAutenticacaoRequisicaoPreenchida(BaseRequisicaoDto requisicaoDto)
        {
            UsuarioBll usuarioBll = new UsuarioBll(true);
            string senhaCriptografada = "";
            UtilitarioBll.CriptografarSenha(DateTime.Now.AddDays(-2).Date.ToString("dd/MM/yyyy").Replace("/", ""), ref senhaCriptografada);

            RequisicaoFazerLoginDto requisicaoLoginDto = new RequisicaoFazerLoginDto()
            {
                Email = "Suporte",
                Senha = senhaCriptografada
            };

            RetornoFazerLoginDto retornoDto = new RetornoFazerLoginDto();
            if (!usuarioBll.FazerLogin(requisicaoLoginDto, ref retornoDto))
            {
                return false;
            }

            requisicaoDto.Identificacao = retornoDto.Identificacao;
            requisicaoDto.IdUsuario = retornoDto.IdUsuario;

            return true;
        }
    }
}
