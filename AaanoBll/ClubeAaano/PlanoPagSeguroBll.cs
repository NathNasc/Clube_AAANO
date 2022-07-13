using AaanoBll.Base;
using AaanoDal;
using AaanoDto.ClubeAaano;
using AaanoDto.Requisicoes;
using AaanoDto.Retornos;
using AaanoVo.ClubeAaano;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AaanoBll.ClubeAaano
{
    public class PlanoPagSeguroBll : BaseBll<PlanoPagSeguroVo, PlanoPagSeguroDto>
    {
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public PlanoPagSeguroBll(bool salvarAlteracoes) : base()
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public PlanoPagSeguroBll(AaanoContexto contexto, bool salvarAlteracoes) : base(contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui um plano parceira no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<PlanoPagSeguroDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            // Valida a requisição
            if (!base.Incluir(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para cadastrar plano parceiras é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            // Converte para VO a ser incluída no banco de dados
            PlanoPagSeguroVo planoPagSeguroVo = new PlanoPagSeguroVo();
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref planoPagSeguroVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o plano parceira para VO: " + mensagemErro;

                return false;
            }

            // Prepara a inclusão no banco de dados
            if (!IncluirBd(planoPagSeguroVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao incluir o plano: " + mensagemErro;
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!aaanoContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Exclui um plano parceira do banco de dados a partir do ID
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Excluir(RequisicaoObterDto requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Excluir(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar as planos parceiras é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!aaanoContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtém um plano parceira pelo ID (não traz a senha no objeto)
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<PlanoPagSeguroDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar as planos parceiras é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            PlanoPagSeguroVo planoPagSeguroVo;
            if (!ObterPorIdBd(requisicaoDto.Id, out planoPagSeguroVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter o planoPagSeguro: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            PlanoPagSeguroDto planoPagSeguroDto = new PlanoPagSeguroDto();
            if (!ConverterVoParaDto(planoPagSeguroVo, ref planoPagSeguroDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter o plano parceira: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            retornoDto.Entidade = planoPagSeguroDto;
            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Obtem o plano vo para uso interno
        /// </summary>
        /// <param name="idPlano"></param>
        /// <param name="planoVo"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        internal bool ObterPlanoVo(Guid idPlano, out PlanoPagSeguroVo planoVo, ref string mensagemErro)
        {
            return ObterPorIdBd(idPlano, out planoVo, ref mensagemErro);
        }

        /// <summary>
        /// Obtem um dicionário com os planos cadastrados
        /// </summary>
        /// <param name="listaPlanos"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        internal bool ObterPlanosCodigos(out Dictionary<string, Guid> listaPlanos, ref string mensagemErro)
        {
            listaPlanos = new Dictionary<string, Guid>();

            // Obter a query primária
            IQueryable<PlanoPagSeguroVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                mensagemErro = $"Houve um problema ao listar as planos parceiras: {mensagemErro}";
                return false;
            }

            listaPlanos = query.ToDictionary(p => p.CodigoSimplificado, p => p.Id);
            return true;
        }

        /// <summary>
        /// Converte um plano parceira Dto para um plano parceira Vo
        /// </summary>
        /// <param name="planoPagSeguroDto"></param>
        /// <param name="planoPagSeguroVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(PlanoPagSeguroDto planoPagSeguroDto, ref PlanoPagSeguroVo planoPagSeguroVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(planoPagSeguroDto, ref planoPagSeguroVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                planoPagSeguroVo.CodigoSimplificado = string.IsNullOrWhiteSpace(planoPagSeguroDto.CodigoSimplificado) ? "" : planoPagSeguroDto.CodigoSimplificado.Trim();
                planoPagSeguroVo.Nome = string.IsNullOrWhiteSpace(planoPagSeguroDto.Nome) ? "" : planoPagSeguroDto.Nome.Trim();
                planoPagSeguroVo.Id = planoPagSeguroDto.Id;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o plano parceira para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um plano parceira Dto para um plano parceira Vo
        /// </summary>
        /// <param name="planoPagSeguroVo"></param>
        /// <param name="planoPagSeguroDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(PlanoPagSeguroVo planoPagSeguroVo, ref PlanoPagSeguroDto planoPagSeguroDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(planoPagSeguroVo, ref planoPagSeguroDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                planoPagSeguroDto.CodigoSimplificado = string.IsNullOrWhiteSpace(planoPagSeguroVo.CodigoSimplificado) ? "" : planoPagSeguroVo.CodigoSimplificado.Trim();
                planoPagSeguroDto.Nome = string.IsNullOrWhiteSpace(planoPagSeguroVo.Nome) ? "" : planoPagSeguroVo.Nome.Trim();

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o plano parceira para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtém uma lista de plano parceiras com filtros aplicados, podendo ser paginada (não traz a senha nos objetos)
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<PlanoPagSeguroDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar as planos parceiras é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            // Obter a query primária
            IQueryable<PlanoPagSeguroVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as planos parceiras: {mensagemErro}";
                retornoDto.Retorno = false;

                return false;
            }

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "NOME":
                        query = query.Where(p => p.Nome.Contains(filtro.Value));
                        break;

                    case "ID":
                        Guid idPesquisa;
                        if (Guid.TryParse(filtro.Value, out idPesquisa))
                        {
                            retornoDto.Mensagem = $"Ñão foi possível converter o filtro de Id.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query = query.Where(p => p.Id == idPesquisa);
                        break;

                    default:
                        retornoDto.Mensagem = $"O filtro {filtro.Key} não está definido para esta pesquisa.";
                        retornoDto.Retorno = false;
                        return false;
                }
            }

            requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(requisicaoDto.CampoOrdem) ? "" : requisicaoDto.CampoOrdem.ToUpper().Trim();
            switch (requisicaoDto.CampoOrdem)
            {
                case "NOME":
                    query = query.OrderBy(p => p.Nome);
                    break;

                default:
                    query = query.OrderBy(p => p.Nome);
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

            List<PlanoPagSeguroVo> listaVo = query.ToList();
            foreach (var planoPagSeguro in listaVo)
            {
                PlanoPagSeguroDto planoPagSeguroDto = new PlanoPagSeguroDto();
                if (!ConverterVoParaDto(planoPagSeguro, ref planoPagSeguroDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;
                    return false;
                }

                retornoDto.ListaEntidades.Add(planoPagSeguroDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Edita um plano parceira
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Editar(RequisicaoEntidadeDto<PlanoPagSeguroDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Editar(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Apenas plano parceiras ADM podem editar planoPagSeguroes
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para editar as planos parceiras é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            PlanoPagSeguroVo planoPagSeguroVo;
            if (!ObterPorIdBd(requisicaoDto.EntidadeDto.Id, out planoPagSeguroVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas para encontrar o plano: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref planoPagSeguroVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas ao converter o plano parceira para Vo: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            if (!EditarBd(planoPagSeguroVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao editar os novos dados do plano: " + mensagemErro;
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
    }
}
