using AaanoBll.Base;
using AaanoBll.ClubeAaano;
using AaanoDto.Base;
using AaanoDto.ClubeAaano;
using AaanoDto.PagSeguro;
using AaanoDto.Retornos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using System;

namespace Aaano
{
    [TestClass]
    public class PagSeguro
    {
        //HttpClient client;

        //string emailTeste = "jlmanfrinato@hotmail.com";
        //string token = "88C882CF25DB47D89CDA82C8504FDA04";

        string email = "junior_cadu@hotmail.com";
        string token = "d6773482-7631-483e-a308-e416b3708a84dfd9721d42eaa7302091f4ddba217f0a526d-750b-4ab2-a223-c49bf55443ab";
        //string sendBox = "https://ws.sandbox.pagseguro.uol.com.br/pre-approvals/";

        [TestMethod]
        public void IntervaloDatas()
        {
            // INTERVALO DE DATAS
            var client = new RestClient($"https://ws.pagseguro.uol.com.br/pre-approvals/?email={email}&token={token}&initialDate=2020-06-05T01:00&finalDate=2020-06-06T20:07&page=1");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/vnd.pagseguro.com.br.v3+json;charset=ISO-8859-1");
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        [TestMethod]
        public void Detalhes()
        {
            // DETALHES
            var client = new RestClient($"https://ws.pagseguro.uol.com.br/pre-approvals/0EE46C547373C02CC41B6FA27586362F?email={email}&token={token}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/vnd.pagseguro.com.br.v3+json;charset=ISO-8859-1");
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        [TestMethod]
        public void OrdensPagamento()
        {
            // ORDENS DE PAGAMENTO
            string assinatura = "a7e53559-fbfb-b093-34b3-cfb89039956e";
            var client = new RestClient($"https://ws.pagseguro.uol.com.br/pre-approvals/{assinatura.ToUpper()}/payment-orders?email={email}&token={token}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/vnd.pagseguro.com.br.v3+json;charset=ISO-8859-1");
            IRestResponse response = client.Execute(request);

            string mensagemErro = "";
            ListaOrdemPagamentoDto ordens = new ListaOrdemPagamentoDto();
            UtilitarioBll.ValidarRespostaHttpEmLista<ListaOrdemPagamentoDto>(response, ref ordens, ref mensagemErro);

            Console.WriteLine(response.Content);

        }

        [TestMethod]
        public void TestarSincronizacaoNovosContratos()
        {
            AssinaturaPagSeguroBll bll = new AssinaturaPagSeguroBll(true);
            BaseRequisicaoDto requisicaoDto = new BaseRequisicaoDto();
            Assert.IsTrue(Utilitario.RetornarAutenticacaoRequisicaoPreenchida(requisicaoDto));

            RetornoDto retorno = new RetornoDto();
            Assert.IsTrue(bll.SincronizarNovasAssinaturas(requisicaoDto, ref retorno));
        }

        [TestMethod]
        public void TestarValidacao()
        {
            AssinaturaPagSeguroBll bll = new AssinaturaPagSeguroBll(true);
            BaseRequisicaoDto requisicaoDto = new BaseRequisicaoDto();
            Assert.IsTrue(Utilitario.RetornarAutenticacaoRequisicaoPreenchida(requisicaoDto));

            RetornoDto retorno = new RetornoDto();
            Assert.IsTrue(bll.ValidarAssinaturasVencidas(requisicaoDto, ref retorno));
        }

    }
}
