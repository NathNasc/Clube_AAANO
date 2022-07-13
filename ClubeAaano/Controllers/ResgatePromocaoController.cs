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
    public class ResgatePromocaoController : BaseController
    {
        /// <summary>
        /// Chama a tela com a tela de regates
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Chamar a view
            return View();
        }

        /// <summary>
        /// Chama a tela com a listagem dos ultimos resgates
        /// </summary>
        /// <returns></returns>
        public ActionResult UltimosResgates()
        {
            // Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para ver os resgates registrados é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            PesquisaInicialModel model = new PesquisaInicialModel();
            model.Filtros.Add("txtResgateInicial", DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy"));
            model.Filtros.Add("txtResgateFinal", DateTime.Now.ToString("dd/MM/yyyy"));

            //Chamar a view
            return View("ResgatesRegistrados", model);
        }

        /// <summary>
        /// Chama a tela com a tela de regates
        /// </summary>
        /// <returns></returns>
        public ActionResult ResgatesRegistrados()
        {
            // Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para ver os resgates registrados é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            PesquisaInicialModel model = new PesquisaInicialModel();

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Chama a tela com os dados de um regate
        /// </summary>
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
                ViewBag.MensagemErro = "Para visualizar um resgate é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Model a ser populada
            ResgatePromocaoModel model = new ResgatePromocaoModel();

            //Obtem pelo ID
            RetornoObterDto<ResgatePromocaoDto> retornoDto = new RetornoObterDto<ResgatePromocaoDto>();
            RequisicaoObterDto requisicaoDto = new RequisicaoObterDto()
            {
                Id = id,
                LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            ResgatePromocaoBll resgatePromocaoBll = new ResgatePromocaoBll(true);
            resgatePromocaoBll.Obter(requisicaoDto, ref retornoDto);

            //Tratar o retorno
            if (retornoDto.Retorno == false)
            {
                ViewBag.MensagemErro = retornoDto.Mensagem;
                return View("Erro");
            }

            //Converter para Model
            string mensagemErro = "";
            if (!model.ConverterDtoParaModel(retornoDto.Entidade, ref mensagemErro))
            {
                ViewBag.MensagemErro = "Erro ao converter para model: " + retornoDto.Mensagem;
                return View("Erro");
            }

            TempData["Retorno"] = "VISUALIZANDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Faz o resgate de uma promoção
        /// </summary>
        /// <param name="id"></param>
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
            ResgatePromocaoBll resgateBll = new ResgatePromocaoBll(true);
            RetornoObterListaDto<ResgatePromocaoDto> retornoDto = new RetornoObterListaDto<ResgatePromocaoDto>();
            resgateBll.ObterListaFiltrada(requisicaoDto, ref retornoDto);

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }

        /// <summary>
        /// Retorna uma lista de promoções disponíveis
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public string ObterPromocoesPorCodigoAssinatura(RequisicaoObterPorCodigoDto requisicaoDto)
        {
            //Requisição para obter a lista
            requisicaoDto.LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas;
            requisicaoDto.Identificacao = SessaoUsuario.SessaoLogin.Identificacao;
            requisicaoDto.IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario;

            //Consumir o serviço
            ResgatePromocaoBll resgatePromocaoBll = new ResgatePromocaoBll(true);
            RetornoObterInformacoesResgateDto retornoDto = new RetornoObterInformacoesResgateDto();
            resgatePromocaoBll.ObterInformacoesParaResgatePorCodigoSimplificado(requisicaoDto, ref retornoDto);

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }

        /// <summary>
        /// Faz o resgate de uma promoção
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public string ResgatarPromocao(ResgatePromocaoDto resgateDto)
        {
            RetornoDto retornoDto = new RetornoDto();
            if (resgateDto == null)
            {
                retornoDto.Mensagem = "Nenhum resgate informado";
                retornoDto.Retorno = false;
            }
            else
            {
                RequisicaoEntidadeDto<ResgatePromocaoDto> requisicaoDto = new RequisicaoEntidadeDto<ResgatePromocaoDto>()
                {
                    Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                    IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                    LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas,
                    EntidadeDto = resgateDto
                };

                requisicaoDto.EntidadeDto.NomeUsuarioResgate = SessaoUsuario.SessaoLogin.NomeUsuario;

                //Consumir o serviço
                ResgatePromocaoBll resgatePromocaoBll = new ResgatePromocaoBll(true);
                resgatePromocaoBll.ResgatarPromocao(requisicaoDto, ref retornoDto);
            }

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }
    }
}
