using AaanoDto.ClubeAaano;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ClubeAaanoSite.Models
{
    /// <summary>
    /// Classe que representa os dados de uma promoção e sua loja
    /// </summary>
    public class PromocaoComLojaModel : PromocaoModel
    {
        /// <summary>
        /// Telefone de contato da loja
        /// </summary>
        [Display(Name = "Telefone")]
        public string TelefoneLojaParceira { set; get; }

        /// <summary>
        /// Telefone de contato da loja
        /// </summary>
        [Display(Name = "Endereço")]
        public string EnderecoLojaParceira { set; get; }

        /// <summary>
        /// Converte um usuário de DTO para Model
        /// </summary>
        /// <param name="promocaoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(PromocaoComLojaDto promocaoDto, ref string mensagemErro)
        {
            try
            {
                if (!base.ConverterDtoParaModel(promocaoDto, ref mensagemErro))
                {
                    return false;
                }

                this.TelefoneLojaParceira = string.IsNullOrWhiteSpace(promocaoDto.TelefoneLojaParceira) ? "" : promocaoDto.TelefoneLojaParceira.Trim();
                this.EnderecoLojaParceira = string.IsNullOrWhiteSpace(promocaoDto.EnderecoLojaParceira) ? "" : promocaoDto.EnderecoLojaParceira.Trim();

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
        /// <param name="promocaoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref PromocaoComLojaDto promocaoDto, ref string mensagemErro)
        {
            try
            {
                if (!ConverterModelParaDto(ref promocaoDto, ref mensagemErro))
                {
                    return false;
                }

                promocaoDto.TelefoneLojaParceira = string.IsNullOrWhiteSpace(this.TelefoneLojaParceira) ? "" : this.TelefoneLojaParceira.Trim();
                promocaoDto.EnderecoLojaParceira = string.IsNullOrWhiteSpace(this.EnderecoLojaParceira) ? "" : this.EnderecoLojaParceira.Trim();

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