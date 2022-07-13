using AaanoBll.Base;
using AaanoBll.ClubeAaano;
using AaanoDto.ClubeAaano;
using AaanoDto.Requisicoes;
using AaanoDto.Retornos;
using ClubeAaanoSite.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ClubeAaanoSite.Controllers
{
    public class UsuarioPermissaoController : BaseController
    {
        /// <summary>
        /// Chama a tela para editar uma loja
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditarPorUsuario(Guid id, string nomeUsuario)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para editar as permissões é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Requisição para obter a lista
            RequisicaoObterDto requisicaoDto = new RequisicaoObterDto()
            {
                Id = id,
                LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            PermissaoUsuarioBll permissaoUsuarioBll = new PermissaoUsuarioBll(true);
            RetornoObterListaDto<PermissaoUsuarioDto> retornoDto = new RetornoObterListaDto<PermissaoUsuarioDto>();
            permissaoUsuarioBll.ObterPermissoesUsuario(requisicaoDto, ref retornoDto);

            if (!retornoDto.Retorno)
            {
                ViewBag.MensagemErro = retornoDto.Mensagem;
                return View("Erro");
            }

            ListaPermissaoUsuarioModel model = new ListaPermissaoUsuarioModel();
            string mensagemErro = "";
            if (!model.ConverterDtoParaModel(retornoDto.ListaEntidades, ref mensagemErro))
            {
                ViewBag.MensagemErro = mensagemErro;
                return View("Erro");
            }

            //Chamar a view
            model.NomeUsuario = nomeUsuario;
            model.IdUsuario = id;
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para salvar os dados da loja
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarPorUsuario(ListaPermissaoUsuarioModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para editar as permissões do usuário é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Valida a entidade recebida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            RequisicaoListaEntidadesDto<PermissaoUsuarioDto> requisicaoDto = new RequisicaoListaEntidadesDto<PermissaoUsuarioDto>()
            {
                LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                IdComum = model.IdUsuario
            };

            //Converte para DTO
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref requisicaoDto, ref mensagemErro))
            {
                ViewBag.MensagemErro = mensagemErro;
                return View("Erro");
            }

            requisicaoDto.IdComum = model.IdUsuario;

            //Consumir o serviço
            RetornoDto retorno = new RetornoDto();
            PermissaoUsuarioBll permissaoUsuarioBll = new PermissaoUsuarioBll(true);
            permissaoUsuarioBll.IncluirEditarListaPermissoesPorUsuario(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                ModelState.AddModelError("", retorno.Mensagem);
                return View(model);
            }

            TempData["Retorno"] = "ALTERADO";

            //Voltar para a index 
            return RedirectToAction("Index", "Usuario", new { id = model.IdUsuario });
        }
    }
}
