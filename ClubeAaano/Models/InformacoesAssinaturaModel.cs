using AaanoDto.ClubeAaano;
using AaanoDto.Retornos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClubeAaanoSite.Models
{
    /// <summary>
    /// Classe que representa os dados referentes a uma assinatura
    /// </summary>
    public class InformacoesAssinaturaModel : BaseModel
    {
        public InformacoesAssinaturaModel()
        {
            ListaPromocoes = new List<PromocaoComLojaModel>();
            AssinaturaPagSeguroModel = new AssinaturaPagSeguroModel();
        }

        /// <summary>
        /// Identificação criptografada
        /// </summary>
        public AssinaturaPagSeguroModel AssinaturaPagSeguroModel { set; get; }

        /// <summary>
        /// Confirmação do email da identificação
        /// </summary>
        public List<PromocaoComLojaModel> ListaPromocoes { set; get; }

        /// <summary>
        /// QR Code do código da assinatura
        /// </summary>
        public string QrCodeBase64 { get; set; }

        /// <summary>
        /// Converte um usuário de DTO para Model
        /// </summary>
        /// <param name="retornoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(RetornoObterInformacoesAssinaturaDto retornoDto, ref string mensagemErro)
        {
            try
            {
                if (!AssinaturaPagSeguroModel.ConverterDtoParaModel(retornoDto.AssinaturaDto, ref mensagemErro))
                {
                    return false;
                }

                foreach (var promocao in retornoDto.ListaPromocoes)
                {
                    PromocaoComLojaModel promocaoModel = new PromocaoComLojaModel();
                    if (!promocaoModel.ConverterDtoParaModel(promocao, ref mensagemErro))
                    {
                        return false;
                    }

                    ListaPromocoes.Add(promocaoModel);
                }

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