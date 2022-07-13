using AaanoBll.ClubeAaano;
using AaanoDto.Base;
using AaanoDto.ClubeAaano;
using AaanoDto.Requisicoes;
using AaanoDto.Retornos;
using ClubeAaanoSite.Models;
using OfficeOpenXml;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ClubeAaanoSite.Controllers
{
    public class AssinaturaPagSeguroController : BaseController
    {
        /// <summary>
        /// Chama a tela com a listagem de assinaturas
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
                ViewBag.MensagemErro = "Para consultar as assinaturas é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            PesquisaInicialModel model = new PesquisaInicialModel();

            //Chamar a view
            return View("Index", model);
        }

        /// <summary>
        /// Chama a tela com a listagem das ultimas assinaturas
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexUltimasAssinaturas()
        {
            // Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para consultar as assinaturas é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            PesquisaInicialModel model = new PesquisaInicialModel();
            model.Filtros.Add("txtCriacaoInicial", DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy"));
            model.Filtros.Add("txtCriacaoFinal", DateTime.Now.ToString("dd/MM/yyyy"));

            //Chamar a view
            return View("Index", model);
        }

        /// <summary>
        /// Chama a tela com a listagem das assinaturas com pagamento pendente
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexPagamentosPendentes()
        {
            // Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para consultar as assinaturas é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            PesquisaInicialModel model = new PesquisaInicialModel();
            model.Filtros.Add("optStatus", "3");

            //Chamar a view
            return View("Index", model);
        }

        /// <summary>
        /// Chama a tela com a listagem das assinaturas com pagamento pendente
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexCanceladas()
        {
            // Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para consultar as assinaturas é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            PesquisaInicialModel model = new PesquisaInicialModel();
            model.Filtros.Add("optStatus", "7");

            //Chamar a view
            return View("Index", model);
        }

        /// <summary>
        /// Chama a tela para visualizar uma assinatura
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
                ViewBag.MensagemErro = "Para visualizar uma assinatura é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Model a ser populada
            AssinaturaPagSeguroModel model = new AssinaturaPagSeguroModel();
            string mensagemRetorno = "";

            //Obtem pelo ID
            if (!this.ObterAssinaturaPagSeguro(id, ref model, ref mensagemRetorno))
            {
                ViewBag.MensagemErro = mensagemRetorno;
                return View("Erro");
            }

            TempData["Retorno"] = "VISUALIZANDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Chama a tela para editar uma assinatura
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
                ViewBag.MensagemErro = "Para editar uma assinatura é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Model a ser populada
            AssinaturaPagSeguroModel model = new AssinaturaPagSeguroModel();
            string mensagemRetorno = "";

            //Obtem o pelo ID
            if (!this.ObterAssinaturaPagSeguro(id, ref model, ref mensagemRetorno))
            {
                ViewBag.MensagemErro = mensagemRetorno;
                return View("Erro");
            }

            TempData["Retorno"] = "EDITANDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para salvar a assinatura
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(AssinaturaPagSeguroModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para editar uma assinatura é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Valida a entidade recebida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Converte para DTO
            AssinaturaPagSeguroDto assinaturaPagSeguroDto = new AssinaturaPagSeguroDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref assinaturaPagSeguroDto, ref mensagemErro))
            {
                ViewBag.MensagemErro = mensagemErro;
                return View("Erro");
            }

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<AssinaturaPagSeguroDto> requisicaoDto = new RequisicaoEntidadeDto<AssinaturaPagSeguroDto>()
            {
                EntidadeDto = assinaturaPagSeguroDto,
                LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
            };

            //Consumir o serviço
            AssinaturaPagSeguroBll assinaturaPagSeguroBll = new AssinaturaPagSeguroBll(true);
            assinaturaPagSeguroBll.Editar(requisicaoDto, ref retorno);

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
        /// Obtem uma assinatura e converte em Model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool ObterAssinaturaPagSeguro(Guid id, ref AssinaturaPagSeguroModel model, ref string mensagemErro)
        {
            //Preparar a requisição e o retorno
            RetornoObterDto<AssinaturaPagSeguroDto> retorno = new RetornoObterDto<AssinaturaPagSeguroDto>();
            RequisicaoObterDto requisicaoDto = new RequisicaoObterDto()
            {
                Id = id,
                LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            AssinaturaPagSeguroBll assinaturaPagSeguroBll = new AssinaturaPagSeguroBll(true);
            assinaturaPagSeguroBll.Obter(requisicaoDto, ref retorno);

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
        /// Obtem uma lista filtrada de assinaturas
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        [HttpPost]
        public string ObterListaFiltradaSimplificadaPaginada(RequisicaoObterListaDto requisicaoDto)
        {
            //Requisição para obter a lista
            requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(requisicaoDto.CampoOrdem) ? "NOMEASSINANTE" : requisicaoDto.CampoOrdem;
            requisicaoDto.IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario;
            requisicaoDto.LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas;
            requisicaoDto.Identificacao = SessaoUsuario.SessaoLogin.Identificacao;

            //Consumir o serviço
            AssinaturaPagSeguroBll assinaturaPagSeguroBll = new AssinaturaPagSeguroBll(true);
            RetornoObterListaDto<AssinaturaPagSeguroDto> retornoDto = new RetornoObterListaDto<AssinaturaPagSeguroDto>();
            assinaturaPagSeguroBll.ObterListaParaSelecao(requisicaoDto, ref retornoDto);

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }

        /// <summary>
        /// Sincroniza as assinaturas com o Pagseguro
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string SincronizarAssinaturas()
        {
            //Requisição para obter a lista
            BaseRequisicaoDto requisicaoDto = new BaseRequisicaoDto();
            requisicaoDto.LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas;
            requisicaoDto.Identificacao = SessaoUsuario.SessaoLogin.Identificacao;
            requisicaoDto.IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario;

            //Consumir o serviço
            AssinaturaPagSeguroBll assinaturaPagSeguroBll = new AssinaturaPagSeguroBll(true);
            RetornoDto retornoDto = new RetornoDto();
            assinaturaPagSeguroBll.SincronizarNovasAssinaturas(requisicaoDto, ref retornoDto);

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }

        /// <summary>
        /// Sincroniza os pagamentos da assinatura com o Pagseguro
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string ValidarAssinatura(Guid id)
        {
            RetornoObterDto<AssinaturaPagSeguroDto> retornoDto = new RetornoObterDto<AssinaturaPagSeguroDto>();
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Login expirado. Atualize a página e entre novamente.";

                return new JavaScriptSerializer().Serialize(retornoDto);
            }

            if (id == Guid.Empty)
            {
                retornoDto.Mensagem = "Id da assinatura não informado";
                retornoDto.Retorno = false;
            }
            else
            {

                //Requisição para obter a lista
                RequisicaoObterDto requisicaoDto = new RequisicaoObterDto();
                requisicaoDto.LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas;
                requisicaoDto.Identificacao = SessaoUsuario.SessaoLogin.Identificacao;
                requisicaoDto.IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario;
                requisicaoDto.Id = id;

                //Consumir o serviço
                AssinaturaPagSeguroBll assinaturaPagSeguroBll = new AssinaturaPagSeguroBll(true);
                assinaturaPagSeguroBll.ValidarAssinaturaPorId(requisicaoDto, ref retornoDto);
            }

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }

        /// <summary>
        /// Gera um Excel para download
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <returns></returns>
        [HttpPost]
        public string GerarExcelAssinaturas(RequisicaoObterListaDto requisicaoDto)
        {
            //Se não tiver login, encaminhar para a tela de login
            RetornoObterListaDto<AssinaturaPagSeguroDto> retornoDto = new RetornoObterListaDto<AssinaturaPagSeguroDto>();
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Login expirado. Atualize a página e entre novamente.";

                string retorno = new JavaScriptSerializer().Serialize(retornoDto);
                return retorno;
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage Ep = new ExcelPackage())
            {
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
                Sheet.Cells["A1"].Value = "Nome";
                Sheet.Cells["B1"].Value = "Cód Simplificado";
                Sheet.Cells["C1"].Value = "Assinatura";
                Sheet.Cells["D1"].Value = "Status";
                Sheet.Cells["E1"].Value = "Email";
                Sheet.Cells["F1"].Value = "Telefone";
                Sheet.Cells["G1"].Value = "Logradouro";
                Sheet.Cells["H1"].Value = "Numero";
                Sheet.Cells["I1"].Value = "Bairro";
                Sheet.Cells["J1"].Value = "Cidade";
                Sheet.Cells["K1"].Value = "Complemento";
                Sheet.Cells["L1"].Value = "Estado";
                Sheet.Cells["M1"].Value = "Cep";
                Sheet.Cells["N1"].Value = "Plano";
                Sheet.Cells["O1"].Value = "Última validação";

                //Requisição para obter a lista
                requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(requisicaoDto.CampoOrdem) ? "NOMEASSINANTE" : requisicaoDto.CampoOrdem;
                requisicaoDto.IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario;
                requisicaoDto.LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas;
                requisicaoDto.Identificacao = SessaoUsuario.SessaoLogin.Identificacao;
                requisicaoDto.NaoPaginarPesquisa = true;

                //Consumir o serviço
                AssinaturaPagSeguroBll assinaturaPagSeguroBll = new AssinaturaPagSeguroBll(true);
                assinaturaPagSeguroBll.ObterListaFiltrada(requisicaoDto, ref retornoDto);

                int row = 2;
                foreach (var item in retornoDto.ListaEntidades)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.NomeAssinate;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.CodigoSimplificado;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Criacao.ToString("dd/MM/yyyy hh:MM");
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Status.ToString();
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Email;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Telefone;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Logradouro;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Numero;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.Bairro;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.Cidade;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.Complemento;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.Estado;
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.Cep;
                    Sheet.Cells[string.Format("N{0}", row)].Value = item.ReferenciaPlano;
                    Sheet.Cells[string.Format("O{0}", row)].Value = item.UltimoEnventoRegistrado.ToString("dd/MM/yyyy hh:MM");
                    row++;
                }

                Sheet.Cells["A:AZ"].AutoFitColumns();
                TempData["ArquivoDownload"] = Ep.GetAsByteArray();

                string retorno = new JavaScriptSerializer().Serialize(retornoDto);
                return retorno;
            }
        }

        /// <summary>
        /// Faz o download do excel com as assinaturas
        /// </summary>
        public void DownloadArquivo()
        {
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + $"Assinaturas {(DateTime.Now.ToString("dd-MM-yyyy"))}.xlsx");
            Response.BinaryWrite((byte[])TempData["ArquivoDownload"]);
            Response.End();
        }

        /// <summary>
        /// Envia email para uma lista de assinaturas
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        [HttpPost]
        public string EnviarEmailMassa(RequisicaoEnviarEmailDto requisicaoDto)
        {
            //Requisição para obter a lista
            requisicaoDto.IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario;
            requisicaoDto.LojasPermitidas = SessaoUsuario.SessaoLogin.LojasPermitidas;
            requisicaoDto.Identificacao = SessaoUsuario.SessaoLogin.Identificacao;

            //Consumir o serviço
            AssinaturaPagSeguroBll assinaturaPagSeguroBll = new AssinaturaPagSeguroBll(true);
            RetornoDto retornoDto = new RetornoDto();
            assinaturaPagSeguroBll.EnviarEmailMassa(requisicaoDto, ref retornoDto);

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }
    }
}
