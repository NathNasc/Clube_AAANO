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
    public class BrindeController : BaseController
    {
        /// <summary>
        /// Chama a tela com a listagem de brindes
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
                ViewBag.MensagemErro = "Para consultar os brindes é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            PesquisaInicialModel model = new PesquisaInicialModel();

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Chama a tela para incluir um brinde
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
                ViewBag.MensagemErro = "Para incluir um brinde é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Brinde a ser incluído
            BrindeModel model = new BrindeModel()
            {
                Id = Guid.NewGuid()
            };

            TempData["Retorno"] = "INCLUINDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para incluir um brinde
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Incluir(BrindeModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para incluir um brinde é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Validar a model recebida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Converter para DTO
            BrindeDto brinde = new BrindeDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref brinde, ref mensagemErro))
            {
                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View(model);
            }

            brinde.Id = Guid.NewGuid();

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<BrindeDto> requisicaoDto = new RequisicaoEntidadeDto<BrindeDto>()
            {
                EntidadeDto = brinde,
                LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            BrindeBll brindeBll = new BrindeBll(true);
            brindeBll.Incluir(requisicaoDto, ref retorno);

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
        /// Chama a tela para visualizar um brinde
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
                ViewBag.MensagemErro = "Para visualizar um brinde é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Model a ser populada
            BrindeModel model = new BrindeModel();
            string mensagemRetorno = "";

            //Obtem pelo ID
            if (!this.ObterBrinde(id, ref model, ref mensagemRetorno))
            {
                ViewBag.MensagemErro = mensagemRetorno;
                return View("Erro");
            }

            TempData["Retorno"] = "VISUALIZANDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Chama a tela para editar um brinde
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
                ViewBag.MensagemErro = "Para editar um brinde é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Model a ser populada
            BrindeModel model = new BrindeModel();
            string mensagemRetorno = "";

            //Obtem o pelo ID
            if (!this.ObterBrinde(id, ref model, ref mensagemRetorno))
            {
                ViewBag.MensagemErro = mensagemRetorno;
                return View("Erro");
            }

            TempData["Retorno"] = "EDITANDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para salvar a brinde
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(BrindeModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para editar um brinde é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Valida a entidade recebida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Converte para DTO
            BrindeDto brindeDto = new BrindeDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref brindeDto, ref mensagemErro))
            {
                ViewBag.MensagemErro = mensagemErro;
                return View("Erro");
            }

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<BrindeDto> requisicaoDto = new RequisicaoEntidadeDto<BrindeDto>()
            {
                EntidadeDto = brindeDto,
                LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            BrindeBll brindeBll = new BrindeBll(true);
            brindeBll.Editar(requisicaoDto, ref retorno);

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
        /// Chama a tela para excluir um brinde
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
                ViewBag.MensagemErro = "Para excluir um brinde é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            TempData["Retorno"] = "EXCLUINDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para excluir a brinde
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExcluirBrinde(ExclusaoModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para excluir um brinde é necessário " +
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
            BrindeBll brindeBll = new BrindeBll(true);
            brindeBll.Excluir(requisicaoDto, ref retorno);

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
        /// Sorteia os nomes das assinaturas ativas
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SortearExistente(Guid id)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para sortear um brinde é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Model a ser populada
            BrindeModel model = new BrindeModel();
            string mensagemRetorno = "";

            //Obtem o pelo ID
            if (!this.ObterBrinde(id, ref model, ref mensagemRetorno))
            {
                ViewBag.MensagemErro = mensagemRetorno;
                return View("Erro");
            }

            TempData["Retorno"] = "SORTEANDO";
            model.Sorteio = DateTime.Now;

            //Chamar a view
            return View("Sortear", model);
        }

        /// <summary>
        /// Sorteia os nomes das assinaturas ativas
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SortearNovo()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para sortear um brinde é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Model a ser populada
            BrindeModel model = new BrindeModel()
            {
                Sorteio = DateTime.Now,
                Entregue = false,
                IdAssinaturaPagSeguro = null
            };

            TempData["Retorno"] = "SORTEANDO";

            //Chamar a view
            return View("Sortear", model);
        }

        /// <summary>
        /// Consome o serviço para salvar a brinde
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sortear(BrindeModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para sortear um brinde é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Valida a entidade recebida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Converte para DTO
            BrindeDto brindeDto = new BrindeDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref brindeDto, ref mensagemErro))
            {
                ViewBag.MensagemErro = mensagemErro;
                return View("Erro");
            }

            RetornoDto retorno = new RetornoDto();
            //Preparar requisição e retorno
            RequisicaoEntidadeDto<BrindeDto> requisicaoDto = new RequisicaoEntidadeDto<BrindeDto>()
            {
                EntidadeDto = brindeDto,
                LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            BrindeBll brindeBll = new BrindeBll(true);
            if (model.Id == null || model.Id == Guid.Empty)
            {
                requisicaoDto.EntidadeDto.Id = Guid.NewGuid();
                brindeBll.Incluir(requisicaoDto, ref retorno);
            }
            else
            {
                brindeBll.Editar(requisicaoDto, ref retorno);
            }

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
        /// Obtem um brinde e converte em Model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool ObterBrinde(Guid id, ref BrindeModel model, ref string mensagemErro)
        {
            //Preparar a requisição e o retorno
            RetornoObterDto<BrindeDto> retorno = new RetornoObterDto<BrindeDto>();
            RequisicaoObterDto requisicaoDto = new RequisicaoObterDto()
            {
                Id = id,
                LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            BrindeBll brindeBll = new BrindeBll(true);
            brindeBll.Obter(requisicaoDto, ref retorno);

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
        /// Obtem uma listra filtrada de brindes
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        [HttpPost]
        public string ObterListaFiltradaPaginada(RequisicaoObterListaDto requisicaoDto)
        {
            //Requisição para obter a lista
            requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(requisicaoDto.CampoOrdem) ? "" : requisicaoDto.CampoOrdem.Trim();
            requisicaoDto.IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario;
            requisicaoDto.LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas;
            requisicaoDto.Identificacao = SessaoUsuario.SessaoLogin.Identificacao;
            requisicaoDto.NumeroItensPorPagina = 20;

            //Consumir o serviço
            BrindeBll brindeBll = new BrindeBll(true);
            RetornoObterListaDto<BrindeDto> retornoDto = new RetornoObterListaDto<BrindeDto>();
            brindeBll.ObterListaFiltrada(requisicaoDto, ref retornoDto);

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }
    }
}
