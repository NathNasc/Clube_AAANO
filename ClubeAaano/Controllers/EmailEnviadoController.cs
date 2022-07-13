using AaanoBll.ClubeAaano;
using AaanoDto.ClubeAaano;
using AaanoDto.Requisicoes;
using AaanoDto.Retornos;
using ClubeAaanoSite.Models;
using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ClubeAaanoSite.Controllers
{
    public class EmailEnviadoController : BaseController
    {
        /// <summary>
        /// Chama a tela com a listagem de emails enviados
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
                ViewBag.MensagemErro = "Para consultar os emails enviados é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Chamar a view
            return View();
        }

        /// <summary>
        /// Chama a tela para visualizar um email enviado
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
                ViewBag.MensagemErro = "Para visualizar emails enviados é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Model a ser populada
            EmailEnviadoModel model = new EmailEnviadoModel();
            string mensagemRetorno = "";

            //Obtem pelo ID
            if (!this.ObterEmailEnviado(id, ref model, ref mensagemRetorno))
            {
                ViewBag.MensagemErro = mensagemRetorno;
                return View("Erro");
            }

            TempData["Retorno"] = "VISUALIZANDO";

            //Chamar a view
            return View(model);
        }
        
        /// <summary>
        /// Obtem emails enviados e converte em Model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool ObterEmailEnviado(Guid id, ref EmailEnviadoModel model, ref string mensagemErro)
        {
            //Preparar a requisição e o retorno
            RetornoObterDto<EmailEnviadoDto> retorno = new RetornoObterDto<EmailEnviadoDto>();
            RequisicaoObterDto requisicaoDto = new RequisicaoObterDto()
            {
                Id = id,
                LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            EmailEnviadoBll modeloEmailBll = new EmailEnviadoBll(true);
            modeloEmailBll.Obter(requisicaoDto, ref retorno);

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
        /// Obtem uma listra filtrada de emails enviados
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        [HttpPost]
        public string ObterListaFiltradaPaginada(RequisicaoObterListaDto requisicaoDto)
        {
            //Requisição para obter a lista
            requisicaoDto.IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario;
            requisicaoDto.LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas;
            requisicaoDto.Identificacao = SessaoUsuario.SessaoLogin.Identificacao;
            requisicaoDto.NumeroItensPorPagina = 20;

            //Consumir o serviço
            EmailEnviadoBll bll = new EmailEnviadoBll(true);
            RetornoObterListaDto<EmailEnviadoDto> retornoDto = new RetornoObterListaDto<EmailEnviadoDto>();
            bll.ObterListaFiltrada(requisicaoDto, ref retornoDto);

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }
    }
}
