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
using System.Data.SqlClient;
using System.Linq;
using static AaanoEnum.PagSeguroEnum;

namespace AaanoBll.ClubeAaano
{
    public class ResgatePromocaoBll : BaseBll<ResgatePromocaoVo, ResgatePromocaoDto>
    {
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public ResgatePromocaoBll(bool salvarAlteracoes) : base()
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public ResgatePromocaoBll(AaanoContexto contexto, bool salvarAlteracoes) : base(contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui as promoções mensais que não existem no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        internal bool IncluirPromocoesDoMes(List<AssinaturaPagSeguroVo> listaAssinaturasValidadas, ref string mensagemErro)
        {
            if (listaAssinaturasValidadas == null || listaAssinaturasValidadas.Count() <= 0)
            {
                mensagemErro = "Não há assinaturas para incluir as promoções";
                return false;
            }

            // Obter a query primária
            IQueryable<ResgatePromocaoVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                mensagemErro = $"Houve um problema ao listar as promoções para planos: {mensagemErro}";
                return false;
            }

            PromocaoPlanoBll promocaoPlanoBll = new PromocaoPlanoBll(false);
            Dictionary<Guid, List<PromocaoPlanoVo>> listaPromocoesPlanos = new Dictionary<Guid, List<PromocaoPlanoVo>>();

            foreach (var assinatura in listaAssinaturasValidadas)
            {
                // Verificar promoções até a próxima validação
                DateTime dataValidade = assinatura.UltimoEnventoRegistrado.AddMonths(1);
                DateTime dataValidadeSeguinte = dataValidade.AddDays(1);
                DateTime dataValidadeAnterior = dataValidade.AddDays(-1);


                // Verificar as promoções existentes neste mês
                List<ResgatePromocaoVo> listaExistentes = query.Where(p => p.IdAssinaturaPagSeguro == assinatura.Id &&
                                        (p.Validade == dataValidade || p.Validade == dataValidadeAnterior || p.Validade == dataValidadeSeguinte)).ToList();

                // Verificar se já obteve as 
                if (!listaPromocoesPlanos.ContainsKey(assinatura.IdPlano))
                {
                    if (!promocaoPlanoBll.ObterPromocoesPorPlano(assinatura.IdPlano, ref listaPromocoesPlanos, ref mensagemErro))
                    {
                        return false;
                    }
                }

                // Se não houver promoção para este plano, ir para a próxima assinatura
                foreach (var promo in listaPromocoesPlanos[assinatura.IdPlano])
                {
                    ResgatePromocaoVo resgatePromocaoVo = listaExistentes.Where(p => p.IdPromocao == promo.IdPromocao).FirstOrDefault();
                    if (resgatePromocaoVo == null)
                    {
                        // Converte para VO a ser incluída no banco de dados
                        resgatePromocaoVo = new ResgatePromocaoVo()
                        {
                            CodigoSimplificadoAssinatura = assinatura.CodigoSimplificado,
                            IdAssinaturaPagSeguro = assinatura.Id,
                            IdPromocao = promo.IdPromocao,
                            NomeUsuarioResgate = "",
                            Resgate = null,
                            Validade = dataValidade
                        };

                        // Prepara a inclusão no banco de dados
                        if (!IncluirBd(resgatePromocaoVo, ref mensagemErro))
                        {
                            mensagemErro = "Falha ao incluir o resgate: " + mensagemErro;
                            return false;
                        }
                    }
                }
            }

            if (salvar)
            {
                // Salva as alterações
                if (!aaanoContexto.Salvar(ref mensagemErro))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Obtem uma lista de promoções para um assinante pelo código
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterInformacoesParaResgatePorCodigoSimplificado(RequisicaoObterPorCodigoDto requisicaoDto, ref RetornoObterInformacoesResgateDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            // Validar o id
            if (string.IsNullOrWhiteSpace(requisicaoDto.Codigo))
            {
                retornoDto.Mensagem = $"Para obter as promoções disponíveis informe o código da assinatura.";
                retornoDto.Retorno = false;

                return false;
            }

            // Verificar se o usuário possui lojas ou é administrador
            if (requisicaoDto.LojasPermitidas.Count() <= 0 && !UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Seu usuário não possui lojas associadas. Entre em contato com o administrador.";
                retornoDto.Retorno = false;

                return false;
            }

            // Obter a assinatura
            AssinaturaPagSeguroDto assinaturaDto = new AssinaturaPagSeguroDto();
            AssinaturaPagSeguroBll assinaturaPagSeguroBll = new AssinaturaPagSeguroBll(false);
            if (!assinaturaPagSeguroBll.ObterPorCodigoSimplificado(requisicaoDto.Codigo, ref assinaturaDto, ref mensagemErro))
            {
                retornoDto.Mensagem = mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            // Se estiver vencido
            if (assinaturaDto.Status == StatusAssinatura.Pendente ||
                assinaturaDto.Status == StatusAssinatura.Pagamento_pendente)
            {
                retornoDto.Mensagem = $"A assinatura está com o pagamento pendente, não é possível resgatar as promoções.";
                retornoDto.Retorno = false;
                return false;
            }

            // Se estiver cancelada
            if (assinaturaDto.Status == StatusAssinatura.Cancelada_PagSeguro ||
                assinaturaDto.Status == StatusAssinatura.Cancelado_vendedor ||
                assinaturaDto.Status == StatusAssinatura.Cancelado_comprador)
            {
                retornoDto.Mensagem = $"A assinatura está cancelada, não é possível resgatar as promoções.";
                retornoDto.Retorno = false;
                return false;
            }

            // Obter as promoções
            List<ResgatePromocaoDto> listaPromocoesDisponiveis;
            if (!ObterPromocoesPorIdAssinatura(requisicaoDto, assinaturaDto.IdPlano, out listaPromocoesDisponiveis, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Problemas para obter as promoções: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            listaPromocoesDisponiveis.ForEach(p => p.IdAssinaturaPagSeguro = assinaturaDto.Id);
            listaPromocoesDisponiveis.ForEach(p => p.CodigoSimplificadoAssinatura = assinaturaDto.CodigoSimplificado);
            listaPromocoesDisponiveis.ForEach(p => p.Validade = assinaturaDto.UltimoEnventoRegistrado.AddMonths(1));

            // Obter os últimos resgates
            List<ResgatePromocaoDto> listaUltimosResgates;
            if (!ObterUltimosResgatesPorIdAssinatura(requisicaoDto, assinaturaDto.Id, out listaUltimosResgates, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Problemas para obter os últimos resgates: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            retornoDto.AssinaturaPagSeguroDto = assinaturaDto;
            retornoDto.ListaPromocoesDisponiveis = listaPromocoesDisponiveis;
            retornoDto.ListaUltimosResgates = listaUltimosResgates;

            retornoDto.Mensagem = $"Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Obtém as  últimas promoções resgatadas pela assinatura
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="idAssinatura"></param>
        /// <param name="listaUltimosResgates"></param>
        /// <returns></returns>
        private bool ObterUltimosResgatesPorIdAssinatura(BaseRequisicaoDto requisicaoDto, Guid idAssinatura, out List<ResgatePromocaoDto> listaUltimosResgates, ref string mensagemErro)
        {
            List<MySqlParameter> listaFiltros = new List<MySqlParameter>();
            listaFiltros.Add(new MySqlParameter("id", idAssinatura));

            string query = "SELECT " +
              "r.Id, " +
              "r.IdPromocao, " +
              "r.CodigoSimplificadoAssinatura, " +
              "r.IdAssinaturaPagSeguro, " +
              "RTRIM(l.Nome) AS NomeLojaParceira, " +
              "RTRIM(p.Resumo) AS ResumoPromocao, " +
              "r.Resgate, " +
              "RTRIM(r.NomeUsuarioResgate) AS NomeUsuarioResgate " +
              "FROM ResgatesPromocoes AS r " +
              "INNER JOIN Promocoes AS p ON r.IdPromocao = p.Id " +
              "INNER JOIN LojasParceiras AS l ON p.IdLojaParceira = l.Id " +
              "WHERE r.IdAssinaturaPagSeguro = @id AND r.Resgate IS NOT NULL ";

            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                string listaLojas = "";
                requisicaoDto.LojasPermitidas.ForEach(p => listaLojas += "'" + p.ToString() + "',");
                listaLojas = listaLojas.Substring(0, listaLojas.Length - 1);

                query += " AND p.IdLojaParceira IN (" + listaLojas + ")";
            }

            query += " ORDER BY r.Resgate DESC";

            try
            {
                // Obter as promoções disponíveis
                using (AaanoContexto contexto = new AaanoContexto())
                {
                    listaUltimosResgates = contexto.Database.SqlQuery<ResgatePromocaoDto>(query, listaFiltros.ToArray()).ToList();
                    return true;
                }
            }
            catch (Exception ex)
            {
                listaUltimosResgates = null;
                mensagemErro = "Falha ao obter os dados: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtém as promoções disponíveis para resgate
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="idPlano"></param>
        /// <param name="listaPromocoes"></param>
        /// <returns></returns>
        private bool ObterPromocoesPorIdAssinatura(BaseRequisicaoDto requisicaoDto, Guid idPlano, out List<ResgatePromocaoDto> listaPromocoes, ref string mensagemErro)
        {
            /*
              List<MySqlParameter> listaFiltros = new List<MySqlParameter>();
            listaFiltros.Add(new MySqlParameter("id", idAssinatura));
            listaFiltros.Add(new MySqlParameter("dataLimite", DateTime.Now.Date));

            string query = "SELECT " +
               "r.Id, " +
               "r.IdPromocao, " +
               "r.CodigoSimplificadoAssinatura, " +
               "r.IdAssinaturaPagSeguro, " +
               "RTRIM(l.Nome) AS NomeLojaParceira, " +
               "RTRIM(p.Resumo) AS ResumoPromocao, " +
               "r.Validade " +
               "FROM ResgatesPromocoes AS r " +
               "INNER JOIN Promocoes AS p ON r.IdPromocao = p.Id " +
               "INNER JOIN LojasParceiras AS l ON p.IdLojaParceira = l.Id " +
               "WHERE r.IdAssinaturaPagSeguro = @id AND r.Resgate IS NULL " +
               "AND r.Validade >= @dataLimite";

             */

            List<MySqlParameter> listaFiltros = new List<MySqlParameter>();
            listaFiltros.Add(new MySqlParameter("id", idPlano));
            listaFiltros.Add(new MySqlParameter("dataLimite", DateTime.Now.Date));

            string query = "SELECT " +
               "pp.IdPromocao, " +
               "RTRIM(l.Nome) AS NomeLojaParceira, " +
               "RTRIM(p.Resumo) AS ResumoPromocao " +
               "FROM PromocoesPlanos AS pp " +
               "INNER JOIN Promocoes AS p ON pp.IdPromocao = p.Id " +
               "INNER JOIN LojasParceiras AS l ON p.IdLojaParceira = l.Id " +
               "WHERE pp.IdPlanoPagSeguro = @id ";

            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                string listaLojas = "";
                requisicaoDto.LojasPermitidas.ForEach(p => listaLojas += "'" + p.ToString() + "',");
                listaLojas = listaLojas.Substring(0, listaLojas.Length - 1);

                query += " AND p.IdLojaParceira IN (" + listaLojas + ")";
            }

            query += " ORDER BY p.Resumo, l.Nome";

            try
            {
                // Obter as promoções disponíveis
                using (AaanoContexto contexto = new AaanoContexto())
                {
                    listaPromocoes = contexto.Database.SqlQuery<ResgatePromocaoDto>(query, listaFiltros.ToArray()).ToList();
                    return true;
                }
            }
            catch (Exception ex)
            {
                listaPromocoes = null;
                mensagemErro = "Falha ao obter os dados: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtem uma lista de promoções para um assinante pelo id
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ResgatarPromocao(RequisicaoEntidadeDto<ResgatePromocaoDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            // Validar o resgate
            if (requisicaoDto.EntidadeDto == null)
            {
                retornoDto.Mensagem = $"Para fazer o resgate, informe os dados do resgate.";
                retornoDto.Retorno = false;

                return false;
            }

            // Verificar se o usuário possui lojas ou é administrador
            if (requisicaoDto.LojasPermitidas.Count() <= 0 && !UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Seu usuário não possui lojas associadas. Entre em contato com o administrador.";
                retornoDto.Retorno = false;

                return false;
            }

            // Se tiver data de resgate
            if (requisicaoDto.EntidadeDto.Validade.Date < DateTime.Today.Date)
            {
                retornoDto.Mensagem = $"Essa promoção está com a validade estogada em: {requisicaoDto.EntidadeDto.Validade.Date.ToString("dd/MM/yyyy")}";
                retornoDto.Retorno = false;
                return false;
            }

            ResgatePromocaoVo resgateVo = new ResgatePromocaoVo();
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref resgateVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter os dados: " + mensagemErro;
                return false;
            }

            resgateVo.Id = Guid.NewGuid();
            resgateVo.Resgate = DateTime.Now;
            resgateVo.NomeUsuarioResgate = string.IsNullOrWhiteSpace(requisicaoDto.EntidadeDto.NomeUsuarioResgate) ? $"NI: {requisicaoDto.IdUsuario.ToString().Replace("-", "")}" : requisicaoDto.EntidadeDto.NomeUsuarioResgate.Trim();

            // Preparar a edição do registro
            if (!IncluirBd(resgateVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao gravar os dados do resgate: " + mensagemErro;
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!aaanoContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Erro ao salvar os dados do resgate: " + mensagemErro;
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        ///// <summary>
        ///// Obtem uma lista de promoções para um assinante pelo id
        ///// </summary>
        ///// <param name="requisicaoDto"></param>
        ///// <param name="retornoDto"></param>
        ///// <returns></returns>
        //public bool ResgatarPromocaoPorId(RequisicaoObterDto requisicaoDto, ref RetornoDto retornoDto)
        //{
        //    string mensagemErro = "";
        //    if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
        //    {
        //        retornoDto.Retorno = false;
        //        retornoDto.Mensagem = mensagemErro;
        //        return false;
        //    }

        //    // Validar o id
        //    if (requisicaoDto.Id == Guid.NewGuid())
        //    {
        //        retornoDto.Mensagem = $"Para fazer o resgate, informe o ID do registro.";
        //        retornoDto.Retorno = false;

        //        return false;
        //    }

        //    // Verificar se o usuário possui lojas ou é administrador
        //    if (requisicaoDto.LojasPermitidas.Count() <= 0 && !UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
        //    {
        //        retornoDto.Mensagem = $"Seu usuário não possui lojas associadas. Entre em contato com o administrador.";
        //        retornoDto.Retorno = false;

        //        return false;
        //    }

        //    ResgatePromocaoVo resgateVo;
        //    if (!ObterPorIdBd(requisicaoDto.Id, out resgateVo, ref mensagemErro))
        //    {
        //        retornoDto.Mensagem = "Erro ao obter o registro do resgate: " + mensagemErro;
        //        retornoDto.Retorno = false;
        //        return false;
        //    }

        //    // Se tiver data de resgate
        //    if (resgateVo.Resgate.HasValue)
        //    {
        //        retornoDto.Mensagem = $"Essa promoção já foi resgatada em {resgateVo.Resgate.Value.ToString("dd/MM/yyyy HH:mm")} pelo usuário {resgateVo.NomeUsuarioResgate}";
        //        retornoDto.Retorno = false;

        //        return false;
        //    }

        //    resgateVo.Resgate = DateTime.Now;
        //    resgateVo.NomeUsuarioResgate = string.IsNullOrWhiteSpace(requisicaoDto.NomeUsuario) ? $"Não Ident.: {requisicaoDto.IdUsuario.ToString().Replace("-", "")}" : requisicaoDto.NomeUsuario.Trim();

        //    // Preparar a edição do registro
        //    if (!EditarBd(resgateVo, ref mensagemErro))
        //    {
        //        retornoDto.Retorno = false;
        //        retornoDto.Mensagem = "Falha ao gravar os dados do resgate: " + mensagemErro;
        //        return false;
        //    }

        //    if (salvar)
        //    {
        //        // Salva as alterações
        //        if (!aaanoContexto.Salvar(ref mensagemErro))
        //        {
        //            retornoDto.Retorno = false;
        //            retornoDto.Mensagem = "Erro ao salvar os dados do resgate: " + mensagemErro;
        //            return false;
        //        }
        //    }

        //    retornoDto.Retorno = true;
        //    retornoDto.Mensagem = "OK";
        //    return true;
        //}

        /// <summary>
        /// Método não implementado
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<ResgatePromocaoDto> requisicaoDto, ref RetornoDto retornoDto)
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
        public override bool Editar(RequisicaoEntidadeDto<ResgatePromocaoDto> requisicaoDto, ref RetornoDto retornoDto)
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
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<ResgatePromocaoDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar os resgates é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            string query = "SELECT" +
                  " r.IdPromocao," +
                  " r.IdAssinaturaPagSeguro," +
                  " r.Resgate," +
                  " r.Validade," +
                  " r.DataInclusao," +
                  " r.DataAlteracao," +
                  " RTRIM(a.NomeAssinate) AS NomeAssinante," +
                  " RTRIM(r.CodigoSimplificadoAssinatura) AS CodigoSimplificadoAssinatura," +
                  " RTRIM(r.NomeUsuarioResgate) AS NomeUsuarioResgate," +
                  " RTRIM(p.Resumo) AS ResumoPromocao," +
                  " RTRIM(l.Nome) AS NomeLojaParceira" +
                  " FROM ResgatesPromocoes AS r" +
                  " INNER JOIN Promocoes AS p ON (r.IdPromocao = p.Id)" +
                  " INNER JOIN LojasParceiras AS l ON (p.IdLojaParceira = l.Id)" +
                  " INNER JOIN AssinaturasPagSeguro AS a ON (r.IdAssinaturaPagSeguro = a.Id)" +
                  " WHERE r.Id = @idResgate";

            List<MySqlParameter> listaFiltros = new List<MySqlParameter>();
            listaFiltros.Add(new MySqlParameter("idResgate", requisicaoDto.Id));

            try
            {
                using (AaanoContexto contexto = new AaanoContexto())
                {
                    retornoDto.Entidade = contexto.Database.SqlQuery<ResgatePromocaoDto>(query, listaFiltros.ToArray()).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                retornoDto.Mensagem = "Falha ao obter a quantidade de registros: " + ex.Message;
                return false;
            }

            // Se não for encontrado
            if (retornoDto.Entidade == null)
            {
                retornoDto.Mensagem = "Erro ao obter a loja parceira: Resgate não encontrado";
                retornoDto.Retorno = false;
                return false;
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
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
        /// Obtém uma lista de promoções com filtros aplicados, podendo ser paginada (não traz a senha nos objetos)
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<ResgatePromocaoDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar as promoções é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            string where = "";
            List<MySqlParameter> listaFiltros = new List<MySqlParameter>();

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "IDASSINATURAPAGUESEGURO":
                        Guid idAssinatura;
                        if (!Guid.TryParse(filtro.Value, out idAssinatura))
                        {
                            retornoDto.Mensagem = $"O filtro de ID assinatura não pôde ser convertido.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        listaFiltros.Add(new MySqlParameter("idAssinatura", idAssinatura));
                        where += " r.IdAssinaturaPagSeguro = @idAssinatura AND ";
                        break;

                    case "IDPROMOCAO":
                        Guid idPromocao;
                        if (!Guid.TryParse(filtro.Value, out idPromocao))
                        {
                            retornoDto.Mensagem = $"O filtro de promoção não pôde ser convertido.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        listaFiltros.Add(new MySqlParameter("idPromocao", idPromocao));
                        where += " r.IdPromocao = @idPromocao AND ";
                        break;

                    case "INICIOVALIDADE":
                        DateTime inicioValidade;
                        if (!DateTime.TryParse(filtro.Value, out inicioValidade))
                        {
                            retornoDto.Mensagem = $"O filtro de validade não pôde ser convertido.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        listaFiltros.Add(new MySqlParameter("validadeInicio", inicioValidade));
                        where += " CAST(r.Validade AS Date) >= @validadeInicio AND ";
                        break;

                    case "FIMVALIDADE":
                        DateTime fimValidade;
                        if (!DateTime.TryParse(filtro.Value, out fimValidade))
                        {
                            retornoDto.Mensagem = $"O filtro de validade não pôde ser convertido.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        listaFiltros.Add(new MySqlParameter("validadeFim", fimValidade));
                        where += " CAST(r.Validade AS Date) <= @validadeFim AND ";
                        break;

                    case "INICIORESGATE":
                        DateTime inicioResgate;
                        if (!DateTime.TryParse(filtro.Value, out inicioResgate))
                        {
                            retornoDto.Mensagem = $"O filtro de resgate não pôde ser convertido.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        listaFiltros.Add(new MySqlParameter("resgateInicio", inicioResgate));
                        where += " CAST(r.Resgate AS Date) >= @resgateInicio AND ";
                        break;

                    case "FIMRESGATE":
                        DateTime fimResgate;
                        if (!DateTime.TryParse(filtro.Value, out fimResgate))
                        {
                            retornoDto.Mensagem = $"O filtro de validade não pôde ser convertido.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        listaFiltros.Add(new MySqlParameter("resgateFim", fimResgate));
                        where += " CAST(r.Resgate AS Date) <= @resgateFim AND ";
                        break;

                    case "IDLOJA":

                        Guid idLoja;
                        if (!Guid.TryParse(filtro.Value, out idLoja))
                        {
                            retornoDto.Mensagem = $"O filtro de loja não pôde ser convertido.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        listaFiltros.Add(new MySqlParameter("idLoja", idLoja));
                        where += " l.Id = @idLoja AND ";

                        break;

                    default:
                        retornoDto.Mensagem = $"O filtro {filtro.Key} não está definido para esta pesquisa.";
                        retornoDto.Retorno = false;
                        return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(where))
            {
                where = " WHERE " + where.Substring(0, where.Length - 4);
            }

            // Juntar os filtros aos Joins
            where = " FROM ResgatesPromocoes AS r" +
                  " INNER JOIN Promocoes AS p ON (r.IdPromocao = p.Id)" +
                  " INNER JOIN LojasParceiras AS l ON (p.IdLojaParceira = l.Id)" +
                  " INNER JOIN AssinaturasPagSeguro AS a ON (r.IdAssinaturaPagSeguro = a.Id)" +
                   where;

            // Ordenar a pesquisa
            requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(requisicaoDto.CampoOrdem) ? "" : requisicaoDto.CampoOrdem.ToUpper().Trim();
            switch (requisicaoDto.CampoOrdem)
            {
                case "RESGATE":
                    where += " ORDER BY r.Resgate, l.Nome, a.NomeAssinate";
                    break;

                case "RESGATEDECRESCENTE":
                    where += " ORDER BY r.Resgate DESC, l.Nome, a.NomeAssinate";
                    break;

                case "VALIDADE":
                    where += " ORDER BY r.Validade, l.Nome, a.NomeAssinate";
                    break;

                case "VALIDADEDECRESCENTE":
                    where += " ORDER BY r.Validade DESC, l.Nome, a.NomeAssinate";
                    break;

                case "RESUMOPROMOCAO":
                    where += " ORDER BY p.Resumo, r.Validade, l.Nome, a.NomeAssinate";
                    break;

                case "NOMEASSINANTE":
                    where += " ORDER BY a.NomeAssinate, r.Validade, l.Nome";
                    break;

                case "NOMELOJA":
                    where += " ORDER BY l.Nome, r.Validade, a.NomeAssinate";
                    break;

                default:
                    where += " ORDER BY r.Validade, l.Nome, a.NomeAssinate";
                    break;
            }

            double totalItens = 0;

            try
            {
                using (AaanoContexto contexto = new AaanoContexto())
                {
                    string selectCount = "SELECT COUNT(r.Id) " + where;
                    List<MySqlParameter> listaFiltrosCount = listaFiltros.ToList();
                    totalItens = contexto.Database.SqlQuery<double>(selectCount, listaFiltrosCount.ToArray()).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                retornoDto.Mensagem = "Falha ao obter a quantidade de registros: " + ex.Message;
                return false;
            }

            // Se não houver resultados
            retornoDto.TotalItens = (int)totalItens;
            if (totalItens == 0)
            {
                retornoDto.NumeroPaginas = 0;
                retornoDto.Mensagem = "Nenhum resultado encontrado.";
                retornoDto.Retorno = true;
                return true;
            }

            string select = "SELECT" +
                               " r.Id," +
                               " r.IdPromocao," +
                               " r.IdAssinaturaPagSeguro," +
                               " RTRIM(a.NomeAssinate) AS NomeAssinante," +
                               " RTRIM(r.CodigoSimplificadoAssinatura) AS CodigoSimplificadoAssinatura," +
                               " r.Resgate," +
                               " r.Validade," +
                               " RTRIM(NomeUsuarioResgate) AS NomeUsuarioResgate," +
                               " RTRIM(p.Resumo) AS ResumoPromocao," +
                               " RTRIM(l.Nome) AS NomeLojaParceira" + where;

            // Paginação da pesquisa
            if (!requisicaoDto.NaoPaginarPesquisa)
            {
                double paginas = totalItens <= requisicaoDto.NumeroItensPorPagina ? 1 : totalItens / requisicaoDto.NumeroItensPorPagina;
                retornoDto.NumeroPaginas = (int)Math.Ceiling(paginas);
                int pular = (requisicaoDto.Pagina - 1) * requisicaoDto.NumeroItensPorPagina;

                select += $" LIMIT {requisicaoDto.NumeroItensPorPagina} OFFSET {pular}";
            }

            try
            {
                using (AaanoContexto contexto = new AaanoContexto())
                {
                    retornoDto.ListaEntidades = contexto.Database.SqlQuery<ResgatePromocaoDto>(select, listaFiltros.ToArray()).ToList();
                }
            }
            catch (Exception ex)
            {
                retornoDto.Mensagem = "Falha ao obter os resgates: " + ex.Message;
                return false;
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Converte uma promoção Dto para uma promoção Vo
        /// </summary>
        /// <param name="resgatePromocaoDto"></param>
        /// <param name="resgatePromocaoVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(ResgatePromocaoDto resgatePromocaoDto, ref ResgatePromocaoVo resgatePromocaoVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(resgatePromocaoDto, ref resgatePromocaoVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                resgatePromocaoVo.CodigoSimplificadoAssinatura = string.IsNullOrWhiteSpace(resgatePromocaoDto.CodigoSimplificadoAssinatura) ? "" : resgatePromocaoDto.CodigoSimplificadoAssinatura.Trim();
                resgatePromocaoVo.NomeUsuarioResgate = string.IsNullOrWhiteSpace(resgatePromocaoDto.NomeUsuarioResgate) ? "" : resgatePromocaoDto.NomeUsuarioResgate.Trim();
                resgatePromocaoVo.IdPromocao = resgatePromocaoDto.IdPromocao;
                resgatePromocaoVo.Resgate = resgatePromocaoDto.Resgate;
                resgatePromocaoVo.Validade = resgatePromocaoDto.Validade;
                resgatePromocaoVo.IdAssinaturaPagSeguro = resgatePromocaoDto.IdAssinaturaPagSeguro;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a promoção para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte uma promoção Dto para uma promoção Vo
        /// </summary>
        /// <param name="resgatePromocaoVo"></param>
        /// <param name="resgatePromocaoDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(ResgatePromocaoVo resgatePromocaoVo, ref ResgatePromocaoDto resgatePromocaoDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(resgatePromocaoVo, ref resgatePromocaoDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                resgatePromocaoDto.CodigoSimplificadoAssinatura = string.IsNullOrWhiteSpace(resgatePromocaoVo.CodigoSimplificadoAssinatura) ? "" : resgatePromocaoVo.CodigoSimplificadoAssinatura.Trim();
                resgatePromocaoDto.NomeUsuarioResgate = string.IsNullOrWhiteSpace(resgatePromocaoVo.NomeUsuarioResgate) ? "" : resgatePromocaoVo.NomeUsuarioResgate.Trim();
                resgatePromocaoDto.IdAssinaturaPagSeguro = resgatePromocaoVo.IdAssinaturaPagSeguro;
                resgatePromocaoDto.IdPromocao = resgatePromocaoVo.IdPromocao;
                resgatePromocaoDto.Resgate = resgatePromocaoVo.Resgate;
                resgatePromocaoDto.Validade = resgatePromocaoVo.Validade;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a promoção para Vo: " + ex.Message;
                return false;
            }
        }

    }
}
