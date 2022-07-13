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
    public class ListaPermissaoUsuarioModel : BaseModel
    {
        public ListaPermissaoUsuarioModel()
        {
            ListaPermissoes = new List<PermissaoUsuarioModel>();
        }

        /// <summary>
        /// Informações do usuário das permissões
        /// </summary>
        [Display(Name = "Usuário")]
        public Guid IdUsuario { set; get; }

        /// <summary>
        /// Nome do usuário da permissão 
        /// </summary>
        public string NomeUsuario { set; get; }

        /// <summary>
        /// Lista com as permissões de um usuário
        /// </summary>
        public List<PermissaoUsuarioModel> ListaPermissoes { set; get; }

        /// <summary>
        /// Converte um usuário de DTO para Model
        /// </summary>
        /// <param name="listaPermissoesDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(List<PermissaoUsuarioDto> listaPermissoesDto, ref string mensagemErro)
        {
            try
            {
                foreach (var permissao in listaPermissoesDto)
                {
                    this.ListaPermissoes.Add(new PermissaoUsuarioModel()
                    {
                        IdLojaParceira = permissao.IdLojaParceira,
                        IdUsuario = permissao.IdUsuario,
                        NomeLoja = string.IsNullOrWhiteSpace(permissao.NomeLoja) ? "" : permissao.NomeLoja.Trim(),
                        NomeUsuario = string.IsNullOrWhiteSpace(permissao.NomeUsuario) ? "" : permissao.NomeUsuario.Trim()
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
        public bool ConverterModelParaDto(ref RequisicaoListaEntidadesDto<PermissaoUsuarioDto> planoDto, ref string mensagemErro)
        {
            try
            {
                ListaPermissoes.RemoveAll(p => p.Excluir == true);

                foreach (var permissao in ListaPermissoes)
                {
                    // Se a permissão não estiver inclusa
                    if (planoDto.ListaEntidadesDto.Where(p => p.IdLojaParceira == permissao.IdLojaParceira
                                                      && p.IdUsuario == permissao.IdUsuario).FirstOrDefault() == null)
                    {
                        planoDto.ListaEntidadesDto.Add(new PermissaoUsuarioDto()
                        {
                            IdLojaParceira = permissao.IdLojaParceira,
                            IdUsuario = permissao.IdUsuario
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