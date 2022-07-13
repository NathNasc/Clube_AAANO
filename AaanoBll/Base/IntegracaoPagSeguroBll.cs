using AaanoBll.ClubeAaano;
using AaanoDal;
using AaanoDto.Base;
using AaanoDto.ClubeAaano;
using AaanoDto.PagSeguro;
using AaanoDto.Retornos;
using AaanoVo.Base;
using AaanoVo.ClubeAaano;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using static AaanoEnum.PagSeguroEnum;

namespace AaanoBll.Base
{
    internal class IntegracaoPagSeguroBll : BaseBll<IntegracaoPagSeguroVo, IntegracaoPagSeguroDto>
    {
        internal IntegracaoPagSeguroBll(AaanoContexto contexto)
        {
            this.aaanoContexto = contexto;
        }

        /// <summary>
        /// Retorna o email de integração do pagseguro
        /// </summary>
        /// <returns></returns>
        private static string RetornarEmailPagSeguro()
        {
            return "teste@hotmail.com";
        }

        /// <summary>
        /// Retorna o token de integração do pagseguro
        /// </summary>
        /// <returns></returns>
        private static string RetornarTokenPagSeguro()
        {
            return "d6773482-7631-483e-a308-e416b37084dfd9721d242eaa7302091f4ddba217f0a526d-750b-4ab2-a223-c49bf55443abc";
        }

        /// <summary>
        /// Obtem as chaves de sincronização para saber a data da ultima sincronização
        /// </summary>
        /// <param name="retornoDto"></param>
        /// <param name="listaChaves"></param>
        /// <returns></returns>
        internal bool ObterDataUltimasSincronizacoes(ref RetornoDto retornoDto, ref Dictionary<string, DateTime> listaChaves)
        {
            // Obter a query primária
            string mensagemErro = "";
            IQueryable<IntegracaoPagSeguroVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao obter a data de ultima sincronização: {mensagemErro}";
                retornoDto.Retorno = false;

                return false;
            }

            //Obter chave novas assinaturas
            IntegracaoPagSeguroVo novasAssinaturasVo = query.Where(p => p.ChaveRecurso.Trim() == "NOVASASSINATURASCLUBEAAANO").FirstOrDefault();
            if (novasAssinaturasVo == null)
            {
                listaChaves.Add("NOVASASSINATURASCLUBEAAANO", DateTime.Now.AddMonths(-6));
            }
            else
            {
                listaChaves.Add("NOVASASSINATURASCLUBEAAANO", novasAssinaturasVo.UltimaSincronizacao);
            }

            //Obter chave ordens de pagamento
            IntegracaoPagSeguroVo ordensPagamentoVo = query.Where(p => p.ChaveRecurso.Trim() == "VALIDACAOASSINATURASCLUBEAAANO").FirstOrDefault();
            if (ordensPagamentoVo == null)
            {
                listaChaves.Add("VALIDACAOASSINATURASCLUBEAAANO", DateTime.Now.AddMonths(-6));
            }
            else
            {
                listaChaves.Add("VALIDACAOASSINATURASCLUBEAAANO", ordensPagamentoVo.UltimaSincronizacao);
            }

            return true;
        }

        /// <summary>
        /// Obtem uma lista de assinaturas feitas em um período de data
        /// </summary>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        internal bool ObterNovasAssinaturas(ref RetornoObterListaDto<AssinaturaPagSeguroDto> retornoDto)
        {
            // Obter a query primária
            string mensagemErro = "";
            IQueryable<IntegracaoPagSeguroVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao obter a data de ultima sincronização: {mensagemErro}";
                retornoDto.Retorno = false;

                return false;
            }

            // Obter a ultima data de sincronização, se não tiver, incluir
            IntegracaoPagSeguroVo integracaoPagSeguroVo = query.Where(p => p.ChaveRecurso.Trim() == "NOVASASSINATURASCLUBEAAANO").FirstOrDefault();
            if (integracaoPagSeguroVo == null)
            {
                integracaoPagSeguroVo = new IntegracaoPagSeguroVo()
                {
                    Id = Guid.NewGuid(),
                    ChaveRecurso = "NOVASASSINATURASCLUBEAAANO",
                    UltimaSincronizacao = DateTime.Now.AddMonths(-6)
                };

                if (!IncluirBd(integracaoPagSeguroVo, ref mensagemErro))
                {
                    retornoDto.Mensagem = $"Houve um problema ao obter a data de ultima sincronização: {mensagemErro}";
                    retornoDto.Retorno = false;

                    return false;
                }

                // Salva as alterações
                if (!aaanoContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;
                    return false;
                }
            }

            DateTime proximaSincronizacao = DateTime.Now;
            double minutos = (proximaSincronizacao - integracaoPagSeguroVo.UltimaSincronizacao).TotalMinutes;
            if (minutos < 60)
            {
                retornoDto.Mensagem = $"Uma verificação já foi feita há menos de uma hora. Aguarde {(60 - Math.Round(minutos))} minutos para uma nova sincronização.";
                retornoDto.Retorno = false;
                return false;
            }

            // Definir a data inicial da próxima sincronização
            if (!ObterAssinaturasIntervaloDatas(integracaoPagSeguroVo.UltimaSincronizacao, proximaSincronizacao, ref retornoDto))
            {
                retornoDto.Retorno = false;
                return false;
            }

            // Atualizar a data de sincrinização
            integracaoPagSeguroVo.UltimaSincronizacao = proximaSincronizacao;
            if (!EditarBd(integracaoPagSeguroVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao atualizar o parâmetro de sincronização: " + mensagemErro;
                return false;
            }

            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Consome o serviço do pagSeguro e obtem as assinaturas
        /// </summary>
        /// <param name="dataInicio"></param>
        /// <param name="dataFim"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        private bool ObterAssinaturasIntervaloDatas(DateTime dataInicio, DateTime dataFim, ref RetornoObterListaDto<AssinaturaPagSeguroDto> retornoDto)
        {
            int pagina = 1;
            ListaNovasAssinaturasDto listaRetorno;

            do
            {
                listaRetorno = new ListaNovasAssinaturasDto();
                var client = new RestClient($"https://ws.pagseguro.uol.com.br/pre-approvals/?" +
                    $"email={RetornarEmailPagSeguro()}&token={RetornarTokenPagSeguro()}&" +
                    $"initialDate={dataInicio.ToString("yyyy-MM-dd")}T{dataInicio.ToString("HH:mm")}&" +
                    $"finalDate={dataFim.ToString("yyyy-MM-dd")}T{dataFim.ToString("HH:mm")}&page={pagina}");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Accept", "application/vnd.pagseguro.com.br.v3+json;charset=ISO-8859-1");
                IRestResponse response = client.Execute(request);

                string mensagemErro = "";
                if (!UtilitarioBll.ValidarRespostaHttpEmLista<ListaNovasAssinaturasDto>(response, ref listaRetorno, ref mensagemErro))
                {
                    retornoDto.Mensagem = mensagemErro + ": " + response.Content;
                    retornoDto.Retorno = false;

                    return false;
                }

                if (listaRetorno.preApprovalList != null)
                {
                    listaRetorno.preApprovalList.ForEach(p => p.status = string.IsNullOrWhiteSpace(p.status) ? "" : p.status.Trim().ToUpper());
                    foreach (var assinatura in listaRetorno.preApprovalList.Where(p => p.status == "ACTIVE" || p.status == "PENDING" || p.status == "PAYMENT_METHOD_CHANGE").ToList())
                    {
                        AssinaturaPagSeguroDto assinaturaDto = new AssinaturaPagSeguroDto()
                        {
                            CodigoSimplificado = assinatura.reference,
                            Id = Guid.Parse(assinatura.code),
                            Criacao = assinatura.date,
                            Status = UtilitarioBll.ConverterStatusPagSeguro(assinatura.status),
                            UltimoEnventoRegistrado = assinatura.lastEventDate,
                        };

                        retornoDto.ListaEntidades.Add(assinaturaDto);
                    }
                }

                pagina++;
            } while (pagina <= listaRetorno.totalPages);

            return true;
        }

        /// <summary>
        /// Obtém os detalhes das assinaturas
        /// </summary>
        /// <param name="idAssinatura"></param>
        /// <param name="assinaturaDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        internal bool ObterDetalhesAssinatura(string idAssinatura, ref AssinaturaPagSeguroDto assinaturaDto, ref string mensagemErro)
        {
            var client = new RestClient($"https://ws.pagseguro.uol.com.br/pre-approvals/" +
                $"{idAssinatura.ToUpper()}?" +
                $"email={RetornarEmailPagSeguro()}&token={RetornarTokenPagSeguro()}");

            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/vnd.pagseguro.com.br.v3+json;charset=ISO-8859-1");
            IRestResponse response = client.Execute(request);

            AssinaturaPeloCodigoDto assinaturaPagSegudo = new AssinaturaPeloCodigoDto();
            if (!UtilitarioBll.ValidarRespostaHttpEmLista<AssinaturaPeloCodigoDto>(response, ref assinaturaPagSegudo, ref mensagemErro))
            {
                return false;
            }

            if (assinaturaPagSegudo.sender.address != null)
            {
                assinaturaDto.Bairro = assinaturaPagSegudo.sender.address.district;
                assinaturaDto.Cep = assinaturaPagSegudo.sender.address.postalCode;
                assinaturaDto.Cidade = assinaturaPagSegudo.sender.address.city;
                assinaturaDto.Logradouro = assinaturaPagSegudo.sender.address.street;
                assinaturaDto.Estado = assinaturaPagSegudo.sender.address.state;
                assinaturaDto.Complemento = assinaturaPagSegudo.sender.address.complement;
                assinaturaDto.Numero = assinaturaPagSegudo.sender.address.number;
                assinaturaDto.Pais = assinaturaPagSegudo.sender.address.country;
            }

            assinaturaDto.Email = assinaturaPagSegudo.sender.email;
            assinaturaDto.CodigoSimplificado = assinaturaPagSegudo.reference;
            assinaturaDto.NomeAssinate = assinaturaPagSegudo.sender.name;
            assinaturaDto.ReferenciaPlano = assinaturaPagSegudo.name;
            assinaturaDto.Status = UtilitarioBll.ConverterStatusPagSeguro(string.IsNullOrWhiteSpace(assinaturaPagSegudo.status) ? "" : assinaturaPagSegudo.status.Trim().ToUpper());
            if (assinaturaPagSegudo.sender.phone != null)
            {
                assinaturaDto.Telefone = $"{assinaturaPagSegudo.sender.phone.areaCode}{assinaturaPagSegudo.sender.phone.number}";
            }

            return true;
        }

        /// <summary>
        /// Obtem as ordens de pagamento das assinaturas e preenche o status
        /// </summary>
        /// <param name="assinaturaPagSeguroVo"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        internal bool ValidarOrdemPagamentoMesCorrente(ref List<AssinaturaPagSeguroVo> listaAssiturasVo, ref string mensagemErro, bool validacaoIndividual = false)
        {
            // Obter a query primária
            IQueryable<IntegracaoPagSeguroVo> query;
            if (!ObterQueryBd(out query, ref mensagemErro))
            {
                mensagemErro = $"Houve um problema ao obter a data de última sincronização: {mensagemErro}";
                return false;
            }

            // Obter a ultima data de sincronização, se não tiver, incluir
            if (!validacaoIndividual)
            {
                IntegracaoPagSeguroVo integracaoPagSeguroVo = query.Where(p => p.ChaveRecurso.Trim() == "VALIDACAOASSINATURASCLUBEAAANO").FirstOrDefault();
                if (integracaoPagSeguroVo == null)
                {
                    integracaoPagSeguroVo = new IntegracaoPagSeguroVo()
                    {
                        Id = Guid.NewGuid(),
                        ChaveRecurso = "VALIDACAOASSINATURASCLUBEAAANO",
                        UltimaSincronizacao = DateTime.Now
                    };

                    if (!IncluirBd(integracaoPagSeguroVo, ref mensagemErro))
                    {
                        mensagemErro = $"Houve um problema ao obter a data de última sincronização: {mensagemErro}";
                        return false;
                    }
                }
                else
                {
                    // Atualizar a data de sincrinização
                    integracaoPagSeguroVo.UltimaSincronizacao = DateTime.Now;
                    if (!EditarBd(integracaoPagSeguroVo, ref mensagemErro))
                    {
                        mensagemErro = "Falha ao atualizar o parâmetro de sincronização: " + mensagemErro;
                        return false;
                    }
                }
            }

            // Assinaturas que serão geradas as promoções
            List<AssinaturaPagSeguroVo> assinaturasValidadasVo = new List<AssinaturaPagSeguroVo>();

            // Validar as assinaturas
            foreach (var assinaturaPagSeguroVo in listaAssiturasVo)
            {
                int pagina = 1;
                List<Ordem> ordensPagamento = new List<Ordem>();
                ListaOrdemPagamentoDto ordensPagamentoPagina = new ListaOrdemPagamentoDto();

                do
                {
                    var client = new RestClient($"https://ws.pagseguro.uol.com.br/pre-approvals/" +
                        $"{assinaturaPagSeguroVo.Id.ToString().ToUpper()}/payment-orders?" +
                        $"email={RetornarEmailPagSeguro()}&token={RetornarTokenPagSeguro()}&page={pagina}");

                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Accept", "application/vnd.pagseguro.com.br.v3+json;charset=ISO-8859-1");
                    IRestResponse response = client.Execute(request);

                    if (!UtilitarioBll.ValidarRespostaHttpEmLista<ListaOrdemPagamentoDto>(response, ref ordensPagamentoPagina, ref mensagemErro))
                    {
                        return false;
                    }

                    pagina++;

                    foreach (var ordem in ordensPagamentoPagina.paymentOrders)
                    {
                        ordensPagamento.Add(ordem.Value);
                    }
                } while (pagina <= ordensPagamentoPagina.totalPages);

                // Obter a data base do mês corrente
                DateTime dataBase = new DateTime();
                if (assinaturaPagSeguroVo.Criacao.Day == 31)
                {
                    switch (DateTime.Now.Month)
                    {
                        case 2:
                        case 4:
                        case 6:
                        case 9:
                        case 11:
                            // Os meses acima não possuem dia 31, então é validade o dia 01 do mês seguinte
                            dataBase = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                            dataBase = dataBase.AddMonths(1);
                            break;

                        default:
                            // Se o mês tiver 31, prosseguir com a validação normal
                            dataBase = new DateTime(DateTime.Now.Year, DateTime.Now.Month, assinaturaPagSeguroVo.Criacao.Day);
                            break;
                    }
                }
                else
                {
                    if (DateTime.Now.Month == 2 && assinaturaPagSeguroVo.Criacao.Day > 28)
                    {
                        dataBase = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 28);
                    }
                    else
                    {
                        dataBase = new DateTime(DateTime.Now.Year, DateTime.Now.Month, assinaturaPagSeguroVo.Criacao.Day);
                    }
                }

                // Se a data base for maior que hoje, o mês anterior que deve ser validado
                if (dataBase.Date > DateTime.Now.Date)
                {
                    dataBase = dataBase.AddMonths(-1);
                }

                // Zerar horário e obter o pagamento do mês corrente
                ordensPagamento.ForEach(p => p.schedulingDate = p.schedulingDate.Date);

                // Faixa de um dia par frente ou para trás por conta dos assinados no dia 31
                var ordemMesCorrente = ordensPagamento.Where(p => p.schedulingDate == dataBase || p.schedulingDate == dataBase.AddDays(2) || p.schedulingDate == dataBase.AddDays(-2)).FirstOrDefault();

                if (ordemMesCorrente != null)
                {
                    // Se estiver pago, colocar como ativo
                    if (ordemMesCorrente.status == 5)
                    {
                        // Ativar para este mês e atualizar para o próximo mês
                        assinaturaPagSeguroVo.Status = StatusAssinatura.Ativo;
                        assinaturaPagSeguroVo.UltimoEnventoRegistrado = ordemMesCorrente.schedulingDate;

                        // Adicionar para gerar as promoções
                        assinaturasValidadasVo.Add(assinaturaPagSeguroVo);
                    }
                    // Se estiver Agendada, Processando, Não Processada, colocar como aguardando o pagamento
                    else if (ordemMesCorrente.status == 1 || ordemMesCorrente.status == 2 || ordemMesCorrente.status == 3 || ordemMesCorrente.status == 6)
                    {
                        assinaturaPagSeguroVo.Status = StatusAssinatura.Pagamento_pendente;
                    }
                    // Se não, colocar como suspenso
                    else
                    {
                        assinaturaPagSeguroVo.Status = StatusAssinatura.Suspensa;
                        assinaturaPagSeguroVo.UltimoEnventoRegistrado = dataBase;
                    }
                }
                else
                {
                    // Aguardar pagamento, caso não tenho ordens de pagamento
                    assinaturaPagSeguroVo.Status = StatusAssinatura.Pagamento_pendente;
                }
            }

            //if (assinaturasValidadasVo.Count() > 0)
            //{
            //    ResgatePromocaoBll resgatePromocaoBll = new ResgatePromocaoBll(aaanoContexto, false);
            //    if (!resgatePromocaoBll.IncluirPromocoesDoMes(assinaturasValidadasVo, ref mensagemErro))
            //    {
            //        return false;
            //    }
            //}

            return true;
        }
    }
}
