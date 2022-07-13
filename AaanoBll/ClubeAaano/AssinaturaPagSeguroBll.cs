using AaanoBll.Base;
using AaanoDal;
using AaanoDto.Base;
using AaanoDto.ClubeAaano;
using AaanoDto.Requisicoes;
using AaanoDto.Retornos;
using AaanoVo.ClubeAaano;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using static AaanoEnum.PagSeguroEnum;

namespace AaanoBll.ClubeAaano
{
    public class AssinaturaPagSeguroBll : BaseBll<AssinaturaPagSeguroVo, AssinaturaPagSeguroDto>
    {
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public AssinaturaPagSeguroBll(bool salvarAlteracoes) : base()
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public AssinaturaPagSeguroBll(AaanoContexto contexto, bool salvarAlteracoes) : base(contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Verifica se foi sincronizado há mais de 6h
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool VerificarSincronizacaoDiaria(BaseRequisicaoDto requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            // Obter as ultimas datas de sincronização
            Dictionary<string, DateTime> listaChaves = new Dictionary<string, DateTime>();
            IntegracaoPagSeguroBll integracaoPagSeguroBll = new IntegracaoPagSeguroBll(aaanoContexto);
            if (!integracaoPagSeguroBll.ObterDataUltimasSincronizacoes(ref retornoDto, ref listaChaves))
            {
                return false;
            }

            // Se foi sincronizado há mais de 6h
            if (listaChaves["NOVASASSINATURASCLUBEAAANO"] < DateTime.Now.AddHours(-6))
            {
                if (!SincronizarNovasAssinaturas(requisicaoDto, ref retornoDto))
                {
                    return false;
                }
            }

            // Se foi sincronizado há mais de 6h
            if (listaChaves["VALIDACAOASSINATURASCLUBEAAANO"] < DateTime.Now.AddHours(-6))
            {
                if (!ValidarAssinaturasVencidas(requisicaoDto, ref retornoDto))
                {
                    return false;
                }
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Sincroniza as novas assinaturas
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool SincronizarNovasAssinaturas(BaseRequisicaoDto requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            //Obter as assinaturas feitas desde a ultima verificação
            RetornoObterListaDto<AssinaturaPagSeguroDto> retornoListaDto = new RetornoObterListaDto<AssinaturaPagSeguroDto>();
            IntegracaoPagSeguroBll pagSeguroBll = new IntegracaoPagSeguroBll(this.aaanoContexto);
            if (!pagSeguroBll.ObterNovasAssinaturas(ref retornoListaDto))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = retornoListaDto.Mensagem;
                return false;
            }

            //Se houve alguma assinatura
            if (retornoListaDto.ListaEntidades.Count > 0)
            {
                //Obter os planos cadastrados
                Dictionary<string, Guid> listaPlanos;
                PlanoPagSeguroBll planosBll = new PlanoPagSeguroBll(false);
                if (!planosBll.ObterPlanosCodigos(out listaPlanos, ref mensagemErro))
                {
                    retornoDto.Mensagem = $"Houve um problema ao preencher os planos: {mensagemErro}";
                    retornoDto.Retorno = false;

                    return false;
                }

                // Obter a query primária
                IQueryable<AssinaturaPagSeguroVo> query;
                if (!this.ObterQueryBd(out query, ref mensagemErro))
                {
                    retornoDto.Mensagem = $"Houve um problema ao listar as assinaturas: {mensagemErro}";
                    retornoDto.Retorno = false;

                    return false;
                }

                //Verificar se as assinaturas encontradas já estão cadastradas
                List<AssinaturaPagSeguroVo> assinaturasIncluidas = new List<AssinaturaPagSeguroVo>();
                foreach (var assinaturaDto in retornoListaDto.ListaEntidades)
                {
                    AssinaturaPagSeguroDto assinaturaPagSeguroDto = assinaturaDto;
                    AssinaturaPagSeguroVo assinaturaPagSeguroVo = query.Where(p => p.Id == assinaturaDto.Id).FirstOrDefault();

                    // Se não existir, incluir
                    if (assinaturaPagSeguroVo == null)
                    {
                        // Obter os dados da assinatura
                        if (!pagSeguroBll.ObterDetalhesAssinatura(assinaturaDto.Id.ToString().Replace("-", ""), ref assinaturaPagSeguroDto, ref mensagemErro))
                        {
                            retornoDto.Mensagem = $"Houve um problema ao obter os detalhes da assinatura: {mensagemErro}";
                            retornoDto.Retorno = false;

                            return false;
                        }

                        //Converter para VO
                        assinaturaPagSeguroVo = new AssinaturaPagSeguroVo();
                        if (!ConverterDtoParaVo(assinaturaPagSeguroDto, ref assinaturaPagSeguroVo, ref mensagemErro))
                        {
                            retornoDto.Mensagem = $"Houve um problema ao listar as assinaturas: {mensagemErro}";
                            retornoDto.Retorno = false;

                            return false;
                        }

                        //Preencher o id do plano
                        if (!listaPlanos.ContainsKey(assinaturaPagSeguroVo.ReferenciaPlano.Trim().ToUpper()))
                        {
                            retornoDto.Mensagem = $"O plano vindo do PagSeguro não foi encontrado no sistema: {assinaturaPagSeguroVo.ReferenciaPlano.Trim().ToUpper()}";
                            retornoDto.Retorno = false;

                            return false;
                        }

                        assinaturaPagSeguroVo.IdPlano = listaPlanos[assinaturaPagSeguroVo.ReferenciaPlano.Trim().ToUpper()];

                        // Prepara a inclusão no banco de dados
                        if (!IncluirBd(assinaturaPagSeguroVo, ref mensagemErro))
                        {
                            retornoDto.Retorno = false;
                            retornoDto.Mensagem = "Falha ao incluir a assinatura: " + mensagemErro;
                            return false;
                        }

                        assinaturasIncluidas.Add(assinaturaPagSeguroVo);
                    }
                }

                ModeloEmailBll modeloEmailBll = new ModeloEmailBll(aaanoContexto, false);
                modeloEmailBll.EnviarModeloEmailListaDestinatario(assinaturasIncluidas, UtilitarioBll.RetornarIdEmailBoasVindas(), ref retornoDto);
            }

            // Salva as alterações
            if (!aaanoContexto.Salvar(ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Validar uma assinatura individualmente
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ValidarAssinaturaPorId(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<AssinaturaPagSeguroDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            AssinaturaPagSeguroVo assinaturaPagSeguroVo;
            if (!ObterPorIdBd(requisicaoDto.Id, out assinaturaPagSeguroVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao validar a assinatura: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            List<AssinaturaPagSeguroVo> listaAssinaturas = new List<AssinaturaPagSeguroVo>();
            listaAssinaturas.Add(assinaturaPagSeguroVo);

            IntegracaoPagSeguroBll integracaoPagSeguroBll = new IntegracaoPagSeguroBll(this.aaanoContexto);
            if (!integracaoPagSeguroBll.ValidarOrdemPagamentoMesCorrente(ref listaAssinaturas, ref mensagemErro, true))
            {
                retornoDto.Mensagem = $"Houve um problema para obter as ordens de pagamento: {mensagemErro}";
                retornoDto.Retorno = false;

                return false;
            }

            // Se estiver pendente, verificar o cancelamento
            if (listaAssinaturas.First().Status == StatusAssinatura.Pagamento_pendente)
            {
                AssinaturaPagSeguroDto assinaturaDto = new AssinaturaPagSeguroDto();
                if (!integracaoPagSeguroBll.ObterDetalhesAssinatura(listaAssinaturas.First().Id.ToString(), ref assinaturaDto, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao editar os novos dados da assinatura: " + mensagemErro;
                    return false;
                }


                listaAssinaturas.First().Status = (assinaturaDto.Status != StatusAssinatura.Ativo) ? assinaturaDto.Status : listaAssinaturas.First().Status;
            }

            if (!EditarBd(listaAssinaturas.First(), ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao editar os novos dados da loja: " + mensagemErro;
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!aaanoContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Erro ao salvar os novos dados: " + mensagemErro;
                    return false;
                }
            }

            AssinaturaPagSeguroDto assinaturaPagSeguroDto = new AssinaturaPagSeguroDto();
            if (!ConverterVoParaDto(assinaturaPagSeguroVo, ref assinaturaPagSeguroDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter a assinatura: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            retornoDto.Entidade = assinaturaPagSeguroDto;
            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Verifica se a ordem de pagamento foi paga e atualiza o status
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ValidarAssinaturasVencidas(BaseRequisicaoDto requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            // Obter a query primária
            IQueryable<AssinaturaPagSeguroVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as assinaturas: {mensagemErro}";
                retornoDto.Retorno = false;

                return false;
            }

            //Pesquisar apenas as com a validade esgotada (validado há mais de um mês)
            DateTime dataBase = DateTime.Now.AddMonths(-1).Date;

            // Obter as assinaturas ativas/pendentes que não foram validadas no mês
            List<AssinaturaPagSeguroVo> listaVencidas = query.Where(p =>
                             (p.Status == StatusAssinatura.Ativo ||
                              p.Status == StatusAssinatura.Pendente ||
                              p.Status == StatusAssinatura.Pagamento_pendente) &&
                              p.UltimoEnventoRegistrado < dataBase).ToList();

            //Validar assinaturas
            IntegracaoPagSeguroBll integracaoPagSeguroBll = new IntegracaoPagSeguroBll(this.aaanoContexto);
            if (!integracaoPagSeguroBll.ValidarOrdemPagamentoMesCorrente(ref listaVencidas, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema para obter as ordens de pagamento: {mensagemErro}";
                retornoDto.Retorno = false;

                return false;
            }

            // Gravar e verificar se não foi cancelada
            foreach (var assinatura in listaVencidas)
            {
                if (assinatura.Status == StatusAssinatura.Pagamento_pendente)
                {
                    AssinaturaPagSeguroDto assinaturaDto = new AssinaturaPagSeguroDto();
                    if (!integracaoPagSeguroBll.ObterDetalhesAssinatura(assinatura.Id.ToString(), ref assinaturaDto, ref mensagemErro))
                    {
                        retornoDto.Retorno = false;
                        retornoDto.Mensagem = "Falha ao editar os novos dados da assinatura: " + mensagemErro;
                        return false;
                    }

                    if (assinaturaDto.Status == StatusAssinatura.Cancelada_PagSeguro ||
                        assinaturaDto.Status == StatusAssinatura.Cancelado_comprador ||
                        assinaturaDto.Status == StatusAssinatura.Cancelado_vendedor)
                    {
                        assinatura.Status = assinaturaDto.Status;
                    }
                }

                if (!EditarBd(assinatura, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao editar os novos dados da assinatura: " + mensagemErro;
                    return false;
                }
            }

            // Salva as alterações
            if (!aaanoContexto.Salvar(ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Erro ao salvar os novos dados: " + mensagemErro;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Obtém uma lista de assinaturas com filtros aplicados, podendo ser paginada (não traz a senha nos objetos)
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<AssinaturaPagSeguroDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar as assinaturas é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Obter a query primária
            IQueryable<AssinaturaPagSeguroVo> query;
            if (!FiltrarLista(requisicaoDto, out query, ref retornoDto))
            {
                retornoDto.Mensagem = $"Houve um problema ao filtrar as assinaturas: {mensagemErro}";
                retornoDto.Retorno = false;

                return false;
            }

            List<AssinaturaPagSeguroVo> listaVo = query.ToList();
            foreach (var assinaturaPagSeguro in listaVo)
            {
                AssinaturaPagSeguroDto assinaturaPagSeguroDto = new AssinaturaPagSeguroDto();
                if (!ConverterVoParaDto(assinaturaPagSeguro, ref assinaturaPagSeguroDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;
                    return false;
                }

                retornoDto.ListaEntidades.Add(assinaturaPagSeguroDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Obtem uma lista simplificada apenas para seleção
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterListaParaSelecao(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<AssinaturaPagSeguroDto> retornoDto)
        {
            string mensagemErro = "";
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Obter a query primária
            IQueryable<AssinaturaPagSeguroVo> query;
            if (!FiltrarLista(requisicaoDto, out query, ref retornoDto))
            {
                retornoDto.Mensagem = $"Houve um problema ao filtrar as assinaturas: {mensagemErro}";
                retornoDto.Retorno = false;

                return false;
            }

            retornoDto.ListaEntidades = query.Select(p => new AssinaturaPagSeguroDto()
            {
                Id = p.Id,
                NomeAssinate = p.NomeAssinate,
                CodigoSimplificado = p.CodigoSimplificado,
                Telefone = p.Telefone,
                IdPlano = p.IdPlano,
                Status = p.Status,
                Criacao = p.Criacao,
                Email = p.Email,
                ReferenciaPlano = p.ReferenciaPlano
            }).ToList();

            retornoDto.ListaEntidades.ForEach(p => p.NomeAssinate = string.IsNullOrWhiteSpace(p.NomeAssinate) ? "" : p.NomeAssinate.Trim());
            retornoDto.ListaEntidades.ForEach(p => p.CodigoSimplificado = string.IsNullOrWhiteSpace(p.CodigoSimplificado) ? "" : p.CodigoSimplificado.Trim());
            retornoDto.ListaEntidades.ForEach(p => p.Telefone = string.IsNullOrWhiteSpace(p.Telefone) ? "" : p.Telefone.Trim());
            retornoDto.ListaEntidades.ForEach(p => p.ReferenciaPlano = string.IsNullOrWhiteSpace(p.ReferenciaPlano) ? "" : p.ReferenciaPlano.Trim());
            retornoDto.ListaEntidades.ForEach(p => p.Email = string.IsNullOrWhiteSpace(p.Email) ? "" : p.Email.Trim());

            // Se não for adm, anonimizar o telefone
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.ListaEntidades.ForEach(p => p.Telefone = p.Telefone.Length > 0 ? p.Telefone.Substring(0, 6).PadRight(p.Telefone.Length, '*') : p.Telefone);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Filtra uma lista de assinaturas
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="query"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        private bool FiltrarLista(RequisicaoObterListaDto requisicaoDto, out IQueryable<AssinaturaPagSeguroVo> query, ref RetornoObterListaDto<AssinaturaPagSeguroDto> retornoDto)
        {
            string mensagemErro = "";
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as assinaturas: {mensagemErro}";
                retornoDto.Retorno = false;

                return false;
            }

            // Aplicar os filtros
            if (!AplicarFiltros(requisicaoDto.ListaFiltros, ref query, ref mensagemErro))
            {
                retornoDto.Mensagem = mensagemErro;
                retornoDto.Retorno = false;

                return false;
            }

            requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(requisicaoDto.CampoOrdem) ? "" : requisicaoDto.CampoOrdem.ToUpper().Trim();
            switch (requisicaoDto.CampoOrdem)
            {
                case "NOMEASSINANTE":
                    query = query.OrderBy(p => p.NomeAssinate).ThenBy(p => p.Criacao);
                    break;

                case "DATACRIACAO":
                    query = query.OrderBy(p => p.Criacao).ThenBy(p => p.NomeAssinate);
                    break;

                case "REFERENCIAPLANO":
                    query = query.OrderBy(p => p.ReferenciaPlano).ThenBy(p => p.NomeAssinate);
                    break;

                default:
                    query = query.OrderBy(p => p.NomeAssinate);
                    break;
            }

            double totalItens = query.Count();
            retornoDto.TotalItens = (int)totalItens;
            if (totalItens == 0)
            {
                retornoDto.NumeroPaginas = 0;
                retornoDto.Mensagem = "Nenhum resultado encontrado.";
                retornoDto.Retorno = true;
                return true;
            }

            if (!requisicaoDto.NaoPaginarPesquisa)
            {
                double paginas = totalItens <= requisicaoDto.NumeroItensPorPagina ? 1 : totalItens / requisicaoDto.NumeroItensPorPagina;
                retornoDto.NumeroPaginas = (int)Math.Ceiling(paginas);

                int pular = (requisicaoDto.Pagina - 1) * requisicaoDto.NumeroItensPorPagina;
                query = query.Skip(pular).Take(requisicaoDto.NumeroItensPorPagina);
            }

            return true;
        }

        /// <summary>
        /// Aplica os filtros de uma lista na query
        /// </summary>
        /// <param name="listaFiltros"></param>
        /// <param name="query"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool AplicarFiltros(Dictionary<string, string> listaFiltros, ref IQueryable<AssinaturaPagSeguroVo> query, ref string mensagemErro)
        {
            foreach (var filtro in listaFiltros)
            {
                switch (filtro.Key)
                {
                    case "EMAILASSINANTE":
                        query = query.Where(p => p.Email.Contains(filtro.Value));
                        break;

                    case "TELEFONEASSINANTE":
                        string telefenoSemFormatacao = filtro.Value.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                        query = query.Where(p => p.Telefone.Contains(telefenoSemFormatacao));
                        break;

                    case "NOMEASSINANTE":
                        query = query.Where(p => p.NomeAssinate.Contains(filtro.Value));
                        break;

                    case "CODIGOSIMPLIFICADO":
                        query = query.Where(p => p.CodigoSimplificado.Contains(filtro.Value));
                        break;

                    case "DATACRIACAOINICIO":
                        DateTime dataInicio;
                        if (!DateTime.TryParse(filtro.Value, out dataInicio))
                        {
                            mensagemErro = $"Falha ao converter o filtro de data de criação (início)";
                            return false;
                        }

                        dataInicio = dataInicio.Date;
                        query = query.Where(p => p.Criacao >= dataInicio);
                        break;

                    case "DATACRIACAOFIM":
                        DateTime dataFim;
                        if (!DateTime.TryParse(filtro.Value, out dataFim))
                        {
                            mensagemErro = $"Falha ao converter o filtro de data de criação (fim)";
                            return false;
                        }

                        dataFim = dataFim.AddHours(23).AddMinutes(59);
                        query = query.Where(p => p.Criacao <= dataFim);
                        break;

                    case "IDPLANO":
                        Guid idPlanota;
                        if (!Guid.TryParse(filtro.Value, out idPlanota))
                        {
                            mensagemErro = $"Falha ao converter o filtro de data de criação (fim)";
                            return false;
                        }

                        query = query.Where(p => p.IdPlano == idPlanota);
                        break;

                    case "STATUS":
                        int status;
                        if (!int.TryParse(filtro.Value, out status))
                        {
                            mensagemErro = $"Falha ao converter o filtro de status";
                            return false;
                        }

                        query = query.Where(p => p.Status == (StatusAssinatura)status);

                        break;

                    case "CLUBEDESCONTO":
                        query = query.Where(p => p.ReferenciaPlano == "AAANO30" || p.ReferenciaPlano == "AAANO50" || p.ReferenciaPlano == "AAANO100" || p.ReferenciaPlano == "AAANO200");
                        break;

                    default:
                        mensagemErro = $"O filtro {filtro.Key} não está definido para esta pesquisa.";
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Método não implementado
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<AssinaturaPagSeguroDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            retornoDto.Mensagem = "Método não implementado";
            retornoDto.Retorno = false;
            return false;
        }

        /// <summary>
        /// Método não implementado
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Excluir(RequisicaoObterDto requisicaoDto, ref RetornoDto retornoDto)
        {
            retornoDto.Mensagem = "Método não implementado";
            retornoDto.Retorno = false;
            return false;
        }

        /// <summary>
        /// Edita uma assinatura
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Editar(RequisicaoEntidadeDto<AssinaturaPagSeguroDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Editar(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Apenas assinaturas ADM podem editar assinaturaPagSeguroes
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para editar as assinaturas é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            AssinaturaPagSeguroVo assinaturaPagSeguroVo;
            if (!ObterPorIdBd(requisicaoDto.EntidadeDto.Id, out assinaturaPagSeguroVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas para encontrar a loja: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            // Forçar os dados que não podem ser alterados
            requisicaoDto.EntidadeDto.UltimoEnventoRegistrado = assinaturaPagSeguroVo.UltimoEnventoRegistrado;
            requisicaoDto.EntidadeDto.ReferenciaPlano = assinaturaPagSeguroVo.ReferenciaPlano;
            requisicaoDto.EntidadeDto.CodigoSimplificado = assinaturaPagSeguroVo.CodigoSimplificado;
            requisicaoDto.EntidadeDto.IdPlano = assinaturaPagSeguroVo.IdPlano;

            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref assinaturaPagSeguroVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas ao converter a assinatura para Vo: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            if (!EditarBd(assinaturaPagSeguroVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao editar os novos dados da loja: " + mensagemErro;
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!aaanoContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Erro ao salvar os novos dados: " + mensagemErro;
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtém uma assinatura parceira pelo ID
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<AssinaturaPagSeguroDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            AssinaturaPagSeguroVo assinaturaPagSeguroVo;
            if (!ObterPorIdBd(requisicaoDto.Id, out assinaturaPagSeguroVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter a assinatura: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            AssinaturaPagSeguroDto assinaturaPagSeguroDto = new AssinaturaPagSeguroDto();
            if (!ConverterVoParaDto(assinaturaPagSeguroVo, ref assinaturaPagSeguroDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter a assinatura: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            RetornoDto retornoLinkDto = new RetornoDto();
            if (UtilitarioBll.ObterLinkInformacoesAssinatura(assinaturaPagSeguroVo.Id, assinaturaPagSeguroVo.Email, ref retornoLinkDto))
            {
                assinaturaPagSeguroDto.LinkCarteirinha = retornoLinkDto.Mensagem;
            }

            retornoDto.Entidade = assinaturaPagSeguroDto;
            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Obtém uma assinatura parceira pelo ID
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        internal bool ObterPorCodigoSimplificado(string codigoAssinatura, ref AssinaturaPagSeguroDto assinaturaDto, ref string mensagemErro)
        {
            if (string.IsNullOrWhiteSpace(codigoAssinatura))
            {
                mensagemErro = "O código é obrigatório para obter uma assinatura.";
                return false;
            }

            // Obter a query primária
            IQueryable<AssinaturaPagSeguroVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                mensagemErro = $"Houve um problema ao listar as assinaturas: {mensagemErro}";
                return false;
            }

            // Filtrar pelo código
            AssinaturaPagSeguroVo assinaturaPagSeguroVo;
            codigoAssinatura = codigoAssinatura.Trim().ToUpper();
            query = query.Where(p => p.CodigoSimplificado.ToUpper() == codigoAssinatura);

            try
            {
                assinaturaPagSeguroVo = query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                mensagemErro = $"Houve um problema ao obter a assinatura: {ex.Message}";
                return false;
            }

            // Se não encontrar a assinatura
            if (assinaturaPagSeguroVo == null)
            {
                mensagemErro = "Assinatura não encontrada.";
                return false;
            }

            AssinaturaPagSeguroDto assinaturaPagSeguroDto = new AssinaturaPagSeguroDto();
            if (!ConverterVoParaDto(assinaturaPagSeguroVo, ref assinaturaPagSeguroDto, ref mensagemErro))
            {
                mensagemErro = "Erro ao converter a assinatura: " + mensagemErro;
                return false;
            }

            assinaturaDto = assinaturaPagSeguroDto;
            return true;
        }

        /// <summary>
        /// Obter a assinatura VO
        /// </summary>
        /// <param name="idAssinatura"></param>
        /// <param name="assinaturaPagSeguroVo"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        internal bool ObterAssinaturaVo(Guid idAssinatura, out AssinaturaPagSeguroVo assinaturaPagSeguroVo, ref string mensagemErro)
        {
            return ObterPorIdBd(idAssinatura, out assinaturaPagSeguroVo, ref mensagemErro);
        }

        /// <summary>
        /// Valida a requisição e retorna as informações da assinatura se estiver válido
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterInformacoesAssinatura(RequisicaoObterInformacoesAssinaturaDto requisicaoDto, ref RetornoObterInformacoesAssinaturaDto retornoDto)
        {
            Guid idAssinatura;
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacaoAssinante(requisicaoDto.Identificacao, requisicaoDto.EmailConfirmacao, out idAssinatura, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            // Obter a assinatura
            AssinaturaPagSeguroVo assinaturaPagSeguroVo;
            if (!ObterPorIdBd(idAssinatura, out assinaturaPagSeguroVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter a assinatura: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            AssinaturaPagSeguroDto assinaturaPagSeguroDto = new AssinaturaPagSeguroDto();
            if (!ConverterVoParaDto(assinaturaPagSeguroVo, ref assinaturaPagSeguroDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter a assinatura: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            // Adicinar ao retorno
            retornoDto.AssinaturaDto = assinaturaPagSeguroDto;

            // Obter as promoções do plano
            string queryPromocoes = "SELECT" +
                                        " RTRIM(p.Resumo) AS Resumo," +
                                        " RTRIM(p.Detalhes) AS Detalhes," +
                                        " p.IdLojaParceira," +
                                        " RTRIM(l.Nome) AS NomeLojaParceira," +
                                        " RTRIM(l.Endereco) AS EnderecoLojaParceira," +
                                        " RTRIM(l.Telefone) AS TelefoneLojaParceira" +
                                        $" FROM {aaanoContexto.Database.Connection.Database}.Promocoes AS p" +
                                        $" INNER JOIN {aaanoContexto.Database.Connection.Database}.PromocoesPlanos as pp" +
                                        $" ON p.Id = pp.IdPromocao AND pp.IdPlanoPagSeguro = @idPlano" +
                                        $" LEFT JOIN {aaanoContexto.Database.Connection.Database}.LojasParceiras AS l" +
                                        $" ON p.IdLojaParceira = l.Id";

            List<MySqlParameter> listaFiltros = new List<MySqlParameter>();
            listaFiltros.Add(new MySqlParameter("idPlano", assinaturaPagSeguroDto.IdPlano));

            try
            {
                // Obter as promoções disponíveis
                using (AaanoContexto contexto = new AaanoContexto())
                {
                    retornoDto.ListaPromocoes = contexto.Database.SqlQuery<PromocaoComLojaDto>(queryPromocoes, listaFiltros.ToArray()).ToList();
                    retornoDto.ListaPromocoes = retornoDto.ListaPromocoes.OrderBy(m => m.NomeLojaParceira).ToList();

                    retornoDto.Mensagem = "Ok";
                    retornoDto.Retorno = true;
                    return true;
                }
            }
            catch (Exception ex)
            {
                mensagemErro = "Falha ao obter os dados: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte uma assinatura Dto para uma assinatura Vo
        /// </summary>
        /// <param name="AssinaturaDto"></param>
        /// <param name="assinaturaVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(AssinaturaPagSeguroDto AssinaturaDto, ref AssinaturaPagSeguroVo assinaturaVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(AssinaturaDto, ref assinaturaVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                assinaturaVo.Bairro = string.IsNullOrWhiteSpace(AssinaturaDto.Bairro) ? "" : AssinaturaDto.Bairro.Trim();
                assinaturaVo.Cep = string.IsNullOrWhiteSpace(AssinaturaDto.Cep) ? "" : AssinaturaDto.Cep.Trim().Replace("-", "");
                assinaturaVo.Cidade = string.IsNullOrWhiteSpace(AssinaturaDto.Cidade) ? "" : AssinaturaDto.Cidade.Trim();
                assinaturaVo.CodigoSimplificado = string.IsNullOrWhiteSpace(AssinaturaDto.CodigoSimplificado) ? "" : AssinaturaDto.CodigoSimplificado.Trim();
                assinaturaVo.Complemento = string.IsNullOrWhiteSpace(AssinaturaDto.Complemento) ? "" : AssinaturaDto.Complemento.Trim();
                assinaturaVo.Criacao = AssinaturaDto.Criacao;
                assinaturaVo.Email = string.IsNullOrWhiteSpace(AssinaturaDto.Email) ? "" : AssinaturaDto.Email.Trim();
                assinaturaVo.Estado = string.IsNullOrWhiteSpace(AssinaturaDto.Estado) ? "" : AssinaturaDto.Estado.Trim();
                assinaturaVo.IdPlano = AssinaturaDto.IdPlano;
                assinaturaVo.Logradouro = string.IsNullOrWhiteSpace(AssinaturaDto.Logradouro) ? "" : AssinaturaDto.Logradouro.Trim();
                assinaturaVo.NomeAssinate = string.IsNullOrWhiteSpace(AssinaturaDto.NomeAssinate) ? "" : AssinaturaDto.NomeAssinate.Trim();
                assinaturaVo.Numero = string.IsNullOrWhiteSpace(AssinaturaDto.Numero) ? "" : AssinaturaDto.Numero.Trim();
                assinaturaVo.Pais = string.IsNullOrWhiteSpace(AssinaturaDto.Pais) ? "" : AssinaturaDto.Pais.Trim();
                assinaturaVo.ReferenciaPlano = string.IsNullOrWhiteSpace(AssinaturaDto.ReferenciaPlano) ? "" : AssinaturaDto.ReferenciaPlano.Trim();
                assinaturaVo.Status = AssinaturaDto.Status;
                assinaturaVo.Telefone = string.IsNullOrWhiteSpace(AssinaturaDto.Telefone) ? "" : AssinaturaDto.Telefone.Trim().Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "");
                assinaturaVo.UltimoEnventoRegistrado = AssinaturaDto.UltimoEnventoRegistrado;
                assinaturaVo.Id = AssinaturaDto.Id;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a assinatura para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte uma assinatura Dto para uma assinatura Vo
        /// </summary>
        /// <param name="assinaturaVo"></param>
        /// <param name="assinaturaDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(AssinaturaPagSeguroVo assinaturaVo, ref AssinaturaPagSeguroDto assinaturaDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(assinaturaVo, ref assinaturaDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                assinaturaDto.Bairro = string.IsNullOrWhiteSpace(assinaturaVo.Bairro) ? "" : assinaturaVo.Bairro.Trim();
                assinaturaDto.Cep = string.IsNullOrWhiteSpace(assinaturaVo.Cep) ? "" : assinaturaVo.Cep.Trim();
                assinaturaDto.Cidade = string.IsNullOrWhiteSpace(assinaturaVo.Cidade) ? "" : assinaturaVo.Cidade.Trim();
                assinaturaDto.CodigoSimplificado = string.IsNullOrWhiteSpace(assinaturaVo.CodigoSimplificado) ? "" : assinaturaVo.CodigoSimplificado.Trim();
                assinaturaDto.Complemento = string.IsNullOrWhiteSpace(assinaturaVo.Complemento) ? "" : assinaturaVo.Complemento.Trim();
                assinaturaDto.Criacao = assinaturaVo.Criacao;
                assinaturaDto.Email = string.IsNullOrWhiteSpace(assinaturaVo.Email) ? "" : assinaturaVo.Email.Trim();
                assinaturaDto.Estado = string.IsNullOrWhiteSpace(assinaturaVo.Estado) ? "" : assinaturaVo.Estado.Trim();
                assinaturaDto.IdPlano = assinaturaVo.IdPlano;
                assinaturaDto.Logradouro = string.IsNullOrWhiteSpace(assinaturaVo.Logradouro) ? "" : assinaturaVo.Logradouro.Trim();
                assinaturaDto.NomeAssinate = string.IsNullOrWhiteSpace(assinaturaVo.NomeAssinate) ? "" : assinaturaVo.NomeAssinate.Trim();
                assinaturaDto.Numero = string.IsNullOrWhiteSpace(assinaturaVo.Numero) ? "" : assinaturaVo.Numero.Trim();
                assinaturaDto.Pais = string.IsNullOrWhiteSpace(assinaturaVo.Pais) ? "" : assinaturaVo.Pais.Trim();
                assinaturaDto.ReferenciaPlano = string.IsNullOrWhiteSpace(assinaturaVo.ReferenciaPlano) ? "" : assinaturaVo.ReferenciaPlano.Trim();
                assinaturaDto.Status = assinaturaVo.Status;
                assinaturaDto.Telefone = string.IsNullOrWhiteSpace(assinaturaVo.Telefone) ? "" : assinaturaVo.Telefone.Trim();
                assinaturaDto.UltimoEnventoRegistrado = assinaturaVo.UltimoEnventoRegistrado;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a assinatura para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtem a lista de assinaturas e envia um modelo de email
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool EnviarEmailMassa(RequisicaoEnviarEmailDto requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            // Obter a query primária
            IQueryable<AssinaturaPagSeguroVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as assinaturas: {mensagemErro}";
                retornoDto.Retorno = false;

                return false;
            }

            //Aplicar os filtros
            if (!AplicarFiltros(requisicaoDto.ListaFiltros, ref query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao filtrar as assinaturas: {mensagemErro}";
                retornoDto.Retorno = false;

                return false;
            }

            List<AssinaturaPagSeguroVo> listaDestinatarios = query.ToList();
            ModeloEmailBll modeloEmailBll = new ModeloEmailBll(false);
            return modeloEmailBll.EnviarModeloEmailListaDestinatario(listaDestinatarios, requisicaoDto.IdModeloEmail, ref retornoDto);
        }
    }
}
