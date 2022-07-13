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
    public class PromocaoPlanoController : BaseController
    {
        /// <summary>
        /// Chama a tela para editar uma loja
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditarPorPromocao(Guid id, string resumoPromocao)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para editar as promoções dos planos é necessário " +
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
            PromocaoPlanoBll promocaoPlanoBll = new PromocaoPlanoBll(true);
            RetornoObterListaDto<PromocaoPlanoDto> retornoDto = new RetornoObterListaDto<PromocaoPlanoDto>();
            promocaoPlanoBll.ObterPlanosPorPromocao(requisicaoDto, ref retornoDto);

            if (!retornoDto.Retorno)
            {
                ViewBag.MensagemErro = retornoDto.Mensagem;
                return View("Erro");
            }

            ListaPromocaoPlanoModel model = new ListaPromocaoPlanoModel();
            string mensagemErro = "";
            if (!model.ConverterDtoParaModel(retornoDto.ListaEntidades, ref mensagemErro))
            {
                ViewBag.MensagemErro = mensagemErro;
                return View("Erro");
            }

            //Chamar a view
            model.ResumoPromocao = resumoPromocao;
            model.IdPromocao = id;
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para salvar os dados da loja
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarPorPromocao(ListaPromocaoPlanoModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para editar as promoções dos planos é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Valida a entidade recebida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            RequisicaoListaEntidadesDto<PromocaoPlanoDto> requisicaoDto = new RequisicaoListaEntidadesDto<PromocaoPlanoDto>()
            {
                LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                IdComum = model.IdPromocao
            };

            //Converte para DTO
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref requisicaoDto, ref mensagemErro))
            {
                ViewBag.MensagemErro = mensagemErro;
                return View("Erro");
            }

            //Consumir o serviço
            RetornoDto retorno = new RetornoDto();
            PromocaoPlanoBll promocaoPlanoBll = new PromocaoPlanoBll(true);
            promocaoPlanoBll.IncluirEditarListaPlanosPorPromocao(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                ModelState.AddModelError("", retorno.Mensagem);
                return View(model);
            }

            TempData["Retorno"] = "ALTERADO";

            //Voltar para a index 
            return RedirectToAction("Index", "Promocao", new { id = model.IdPromocao });
        }
    }
}
