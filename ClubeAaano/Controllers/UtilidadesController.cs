using AaanoBll.ClubeAaano;
using AaanoDto.Requisicoes;
using AaanoDto.Retornos;
using ClubeAaanoSite.Models;
using QRCoder;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;

namespace ClubeAaanoSite.Controllers
{
    public class UtilidadesController : BaseController
    {
        public ActionResult ConfirmarIdentificacao(string identificacao)
        {
            if (string.IsNullOrWhiteSpace(identificacao))
            {
                ViewBag.MensagemErro = "Identificação inválida para obter as informações da assinatura";
                return View("Erro");
            }

            ConfirmarIdentificacaoModel model = new ConfirmarIdentificacaoModel()
            {
                Identificacao = identificacao,
                EmailConfirmacao = ""
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ConfirmarIdentificacao(ConfirmarIdentificacaoModel model)
        {
            if (string.IsNullOrWhiteSpace(model.EmailConfirmacao))
            {
                ModelState.AddModelError("", "Informe o email que utilizou para assinar o Clube AAANO.");
                return View(model);
            }

            AssinaturaPagSeguroBll assinaturaPagSeguroBll = new AssinaturaPagSeguroBll(false);
            RequisicaoObterInformacoesAssinaturaDto requisicaoDto = new RequisicaoObterInformacoesAssinaturaDto()
            {
                EmailConfirmacao = model.EmailConfirmacao,
                Identificacao = model.Identificacao
            };

            RetornoObterInformacoesAssinaturaDto retornoDto = new RetornoObterInformacoesAssinaturaDto();
            if (!assinaturaPagSeguroBll.ObterInformacoesAssinatura(requisicaoDto, ref retornoDto))
            {
                ModelState.AddModelError("", retornoDto.Mensagem);
                return View(model);
            }

            string mensagemErro = "";
            InformacoesAssinaturaModel informacoesModel = new InformacoesAssinaturaModel();
            if (!informacoesModel.ConverterDtoParaModel(retornoDto, ref mensagemErro))
            {
                ModelState.AddModelError("", "Problemas na conversão das informações: " + mensagemErro);
                return View(model);
            }

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(informacoesModel.AssinaturaPagSeguroModel.CodigoSimplificado, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            MemoryStream ms = new MemoryStream();
            qrCode.GetGraphic(10).Save(ms, ImageFormat.Png);
            byte[] qrCodeBinario = ms.ToArray();

            informacoesModel.QrCodeBase64 = Convert.ToBase64String(qrCodeBinario);

            return View("InformacoesAssinatura", informacoesModel);
        }
    }
}
