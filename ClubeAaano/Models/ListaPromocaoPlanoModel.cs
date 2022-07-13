using AaanoDto.ClubeAaano;
using AaanoDto.Requisicoes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ClubeAaanoSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um usuário
    /// </summary>
    public class ListaPromocaoPlanoModel : BaseModel
    {
        public ListaPromocaoPlanoModel()
        {
            ListaPlanos = new List<PlanoPagSeguroModel>();
        }

        /// <summary>
        /// Informações do usuário das permissões
        /// </summary>
        [Display(Name = "Usuário")]
        public Guid IdPromocao { set; get; }

        /// <summary>
        /// Nome do usuário da permissão 
        /// </summary>
        public string ResumoPromocao { set; get; }

        /// <summary>
        /// Lista com os planos que têm a promoção
        /// </summary>
        public List<PlanoPagSeguroModel> ListaPlanos { set; get; }

        /// <summary>
        /// Converte um usuário de DTO para Model
        /// </summary>
        /// <param name="listaPromocaoPlanoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(List<PromocaoPlanoDto> listaPromocaoPlanoDto, ref string mensagemErro)
        {
            try
            {
                foreach (var plano in listaPromocaoPlanoDto)
                {
                    this.ListaPlanos.Add(new PlanoPagSeguroModel()
                    {
                        Id = plano.IdPlanoPagSeguro,
                        Nome = string.IsNullOrWhiteSpace(plano.NomePlanoPagSeguro) ? "" : plano.NomePlanoPagSeguro.Trim(),
                        CodigoSimplificado = string.IsNullOrWhiteSpace(plano.CodigoSimplificado) ? "" : plano.CodigoSimplificado.Trim()
                    });
                }

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
        public bool ConverterModelParaDto(ref RequisicaoListaEntidadesDto<PromocaoPlanoDto> planoDto, ref string mensagemErro)
        {
            try
            {
                ListaPlanos.RemoveAll(p => p.Excluir == true);

                foreach (var plano in ListaPlanos)
                {
                    // Se a permissão não estiver inclusa
                    if (planoDto.ListaEntidadesDto.Where(p => p.IdPlanoPagSeguro == plano.Id
                                                      && p.IdPromocao == IdPromocao).FirstOrDefault() == null)
                    {
                        planoDto.ListaEntidadesDto.Add(new PromocaoPlanoDto()
                        {
                            IdPlanoPagSeguro = plano.Id,
                            IdPromocao = IdPromocao
                        });
                    }
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