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
    public class PlanoPagSeguroController : BaseController
    {
        /// <summary>
        /// Chama a tela com a listagem de lojas
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para consultar as lojas parceiras é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Chamar a view
            return View();
        }

        /// <summary>
        /// Chama a tela para incluir uma loja
        /// </summary>
        /// <returns></returns>
        public ActionResult Incluir()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para incluir uma loja parceira é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //PlanoPagSeguro a ser incluído
            PlanoPagSeguroModel model = new PlanoPagSeguroModel()
            {
                Id = Guid.NewGuid()
            };

            TempData["Retorno"] = "INCLUINDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para incluir uma nova loja
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Incluir(PlanoPagSeguroModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para incluir uma loja parceira é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Validar a model recebida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Id == Guid.Empty || model.Id == null)
            {
                ModelState.AddModelError("Id", "O id do plano é obrigatório.");
                return View(model);
            }

            //Converter para DTO
            PlanoPagSeguroDto planoPagSeguro = new PlanoPagSeguroDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref planoPagSeguro, ref mensagemErro))
            {
                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View(model);
            }

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<PlanoPagSeguroDto> requisicaoDto = new RequisicaoEntidadeDto<PlanoPagSeguroDto>()
            {
                EntidadeDto = planoPagSeguro,
                LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            PlanoPagSeguroBll planoPagSeguroBll = new PlanoPagSeguroBll(true);
            planoPagSeguroBll.Incluir(requisicaoDto, ref retorno);

            //Verificar o retorno 
            if (retorno.Retorno == false)
            {
                //Se houver erro, exibir na tela de inclusão
                ModelState.AddModelError("", retorno.Mensagem);
                return View(model);
            }

            TempData["Retorno"] = "INCLUIDO";

            //Retornar para index
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Chama a tela para visualizar uma loja
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Visualizar(Guid id)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para visualizar uma loja parceira é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Model a ser populada
            PlanoPagSeguroModel model = new PlanoPagSeguroModel();
            string mensagemRetorno = "";

            //Obtem pelo ID
            if (!this.ObterPlanoPagSeguro(id, ref model, ref mensagemRetorno))
            {
                ViewBag.MensagemErro = mensagemRetorno;
                return View("Erro");
            }

            TempData["Retorno"] = "VISUALIZANDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Chama a tela para editar uma loja
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Editar(Guid id)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para editar uma loja parceira é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Model a ser populada
            PlanoPagSeguroModel model = new PlanoPagSeguroModel();
            string mensagemRetorno = "";

            //Obtem o pelo ID
            if (!this.ObterPlanoPagSeguro(id, ref model, ref mensagemRetorno))
            {
                ViewBag.MensagemErro = mensagemRetorno;
                return View("Erro");
            }

            TempData["Retorno"] = "EDITANDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para salvar os dados da loja
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(PlanoPagSeguroModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para editar uma loja parceira é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Valida a entidade recebida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Converte para DTO
            PlanoPagSeguroDto planoPagSeguroDto = new PlanoPagSeguroDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref planoPagSeguroDto, ref mensagemErro))
            {
                ViewBag.MensagemErro = mensagemErro;
                return View("Erro");
            }

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<PlanoPagSeguroDto> requisicaoDto = new RequisicaoEntidadeDto<PlanoPagSeguroDto>()
            {
                EntidadeDto = planoPagSeguroDto,
                LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            PlanoPagSeguroBll planoPagSeguroBll = new PlanoPagSeguroBll(true);
            planoPagSeguroBll.Editar(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                ModelState.AddModelError("", retorno.Mensagem);
                return View(model);
            }

            TempData["Retorno"] = "ALTERADO";

            //Voltar para a index 
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Chama a tela para excluir uma loja
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Excluir(ExclusaoModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para excluir uma loja parceira é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            TempData["Retorno"] = "EXCLUINDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para excluir a loja
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExcluirPlanoPagSeguro(ExclusaoModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para excluir uma loja parceira é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoObterDto requisicaoDto = new RequisicaoObterDto()
            {
                Id = model.Id,
                LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            PlanoPagSeguroBll planoPagSeguroBll = new PlanoPagSeguroBll(true);
            planoPagSeguroBll.Excluir(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                ModelState.AddModelError("", retorno.Mensagem);
                return View("Excluir", model);
            }

            TempData["Retorno"] = "EXCLUIDO";

            //Voltar para a index
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Obtem uma loja e converte em Model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool ObterPlanoPagSeguro(Guid id, ref PlanoPagSeguroModel model, ref string mensagemErro)
        {
            //Preparar a requisição e o retorno
            RetornoObterDto<PlanoPagSeguroDto> retorno = new RetornoObterDto<PlanoPagSeguroDto>();
            RequisicaoObterDto requisicaoDto = new RequisicaoObterDto()
            {
                Id = id,
                LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            PlanoPagSeguroBll planoPagSeguroBll = new PlanoPagSeguroBll(true);
            planoPagSeguroBll.Obter(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                mensagemErro = retorno.Mensagem;
                return false;
            }
            else
            {
                //Converter para Model
                return model.ConverterDtoParaModel(retorno.Entidade, ref mensagemErro);
            }
        }

        /// <summary>
        /// Obtem uma listra filtrada de lojas
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        [HttpPost]
        public string ObterListaFiltradaPaginada(RequisicaoObterListaDto requisicaoDto)
        {
            //Requisição para obter a lista
            requisicaoDto.CampoOrdem = "NOME";
            requisicaoDto.IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario;
            requisicaoDto.LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas;
            requisicaoDto.Identificacao = SessaoUsuario.SessaoLogin.Identificacao;
            requisicaoDto.NumeroItensPorPagina = 20;

            //Consumir o serviço
            PlanoPagSeguroBll planoPagSeguroBll = new PlanoPagSeguroBll(true);
            RetornoObterListaDto<PlanoPagSeguroDto> retornoDto = new RetornoObterListaDto<PlanoPagSeguroDto>();
            planoPagSeguroBll.ObterListaFiltrada(requisicaoDto, ref retornoDto);

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }
    }
}
