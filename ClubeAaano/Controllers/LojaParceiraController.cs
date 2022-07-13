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
    public class LojaParceiraController : BaseController
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

            //LojaParceira a ser incluído
            LojaParceiraModel model = new LojaParceiraModel()
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
        public ActionResult Incluir(LojaParceiraModel model)
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

            //Converter para DTO
            LojaParceiraDto lojaParceira = new LojaParceiraDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref lojaParceira, ref mensagemErro))
            {
                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View(model);
            }

            lojaParceira.Id = Guid.NewGuid();

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<LojaParceiraDto> requisicaoDto = new RequisicaoEntidadeDto<LojaParceiraDto>()
            {
                EntidadeDto = lojaParceira,
                LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            LojaParceiraBll lojaParceiraBll = new LojaParceiraBll(true);
            lojaParceiraBll.Incluir(requisicaoDto, ref retorno);

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
            LojaParceiraModel model = new LojaParceiraModel();
            string mensagemRetorno = "";

            //Obtem pelo ID
            if (!this.ObterLojaParceira(id, ref model, ref mensagemRetorno))
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
            LojaParceiraModel model = new LojaParceiraModel();
            string mensagemRetorno = "";

            //Obtem o pelo ID
            if (!this.ObterLojaParceira(id, ref model, ref mensagemRetorno))
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
        public ActionResult Editar(LojaParceiraModel model)
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
            LojaParceiraDto lojaParceiraDto = new LojaParceiraDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref lojaParceiraDto, ref mensagemErro))
            {
                ViewBag.MensagemErro = mensagemErro;
                return View("Erro");
            }

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<LojaParceiraDto> requisicaoDto = new RequisicaoEntidadeDto<LojaParceiraDto>()
            {
                EntidadeDto = lojaParceiraDto,
                LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            LojaParceiraBll lojaParceiraBll = new LojaParceiraBll(true);
            lojaParceiraBll.Editar(requisicaoDto, ref retorno);

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
        public ActionResult ExcluirLojaParceira(ExclusaoModel model)
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
            LojaParceiraBll lojaParceiraBll = new LojaParceiraBll(true);
            lojaParceiraBll.Excluir(requisicaoDto, ref retorno);

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
        private bool ObterLojaParceira(Guid id, ref LojaParceiraModel model, ref string mensagemErro)
        {
            //Preparar a requisição e o retorno
            RetornoObterDto<LojaParceiraDto> retorno = new RetornoObterDto<LojaParceiraDto>();
            RequisicaoObterDto requisicaoDto = new RequisicaoObterDto()
            {
                Id = id,
                LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            LojaParceiraBll lojaParceiraBll = new LojaParceiraBll(true);
            lojaParceiraBll.Obter(requisicaoDto, ref retorno);

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
            LojaParceiraBll lojaParceiraBll = new LojaParceiraBll(true);
            RetornoObterListaDto<LojaParceiraDto> retornoDto = new RetornoObterListaDto<LojaParceiraDto>();
            lojaParceiraBll.ObterListaFiltrada(requisicaoDto, ref retornoDto);

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }
    }
}
