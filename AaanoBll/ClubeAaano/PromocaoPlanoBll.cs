using AaanoBll.Base;
using AaanoDal;
using AaanoDto.ClubeAaano;
using AaanoDto.Requisicoes;
using AaanoDto.Retornos;
using AaanoVo.ClubeAaano;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AaanoBll.ClubeAaano
{
    public class PromocaoPlanoBll : BaseBll<PromocaoPlanoVo, PromocaoPlanoDto>
    {
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public PromocaoPlanoBll(bool salvarAlteracoes) : base()
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public PromocaoPlanoBll(AaanoContexto contexto, bool salvarAlteracoes) : base(contexto)
        {
            salvar = salvarAlteracoes;
        }
        /// <summary>
        /// Inclui uma promoção para o plano no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool IncluirEditarListaPlanosPorPromocao(RequisicaoListaEntidadesDto<PromocaoPlanoDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            if (requisicaoDto.ListaEntidadesDto == null)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Não é possível incluir uma lista nula.";
                return false;
            }

            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para cadastrar promoções para planos é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            // Obter a query primária
            IQueryable<PromocaoPlanoVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as promoções para planos: {mensagemErro}";
                retornoDto.Retorno = false;

                return false;
            }

            // Percorrer as promoções para planos adicionadas
            List<PromocaoPlanoVo> listaExclusao = query.Where(p => p.IdPromocao == requisicaoDto.IdComum).ToList();
            foreach (var promocaoPlano in requisicaoDto.ListaEntidadesDto)
            {
                promocaoPlano.IdPromocao = requisicaoDto.IdComum;
                PromocaoPlanoVo promocaoPlanoVo = listaExclusao.Where(p => p.IdPlanoPagSeguro == promocaoPlano.IdPlanoPagSeguro).FirstOrDefault();
                if (promocaoPlanoVo == null)
                {
                    // Converte para VO a ser incluída no banco de dados
                    promocaoPlanoVo = new PromocaoPlanoVo();
                    if (!ConverterDtoParaVo(promocaoPlano, ref promocaoPlanoVo, ref mensagemErro))
                    {
                        retornoDto.Retorno = false;
                        retornoDto.Mensagem = "Falha ao converter a promoção para o plano para VO: " + mensagemErro;

                        return false;
                    }

                    // Prepara a inclusão no banco de dados
                    if (!IncluirBd(promocaoPlanoVo, ref mensagemErro))
                    {
                        retornoDto.Retorno = false;
                        retornoDto.Mensagem = "Falha ao incluir a promoção para o plano: " + mensagemErro;
                        return false;
                    }
                }
                else
                {
                    // Se houver a promoção para o plano, remover da lista de exclusão
                    listaExclusao.Remove(promocaoPlanoVo);
                }
            }

            foreach (var permissao in listaExclusao)
            {
                if (!ExcluirBd(permissao.Id, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao excluir a promoção para o plano: " + mensagemErro;
                    return false;
                }
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
        /// Obtem uma lista de promoções para planos de um usuário
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterPlanosPorPromocao(RequisicaoObterDto requisicaoDto, ref RetornoObterListaDto<PromocaoPlanoDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            // Validar o id
            if (requisicaoDto.Id == Guid.NewGuid())
            {
                retornoDto.Mensagem = $"Para obter as promoções para planos do usuário informe o id.";
                retornoDto.Retorno = false;

                return false;
            }

            // Obter a query primária
            IQueryable<PromocaoPlanoVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as promoções para planos: {mensagemErro}";
                retornoDto.Retorno = false;

                return false;
            }

            PlanoPagSeguroBll planoBll = new PlanoPagSeguroBll(false);
            List<PromocaoPlanoVo> listaVo = query.Where(p => p.IdPromocao == requisicaoDto.Id).ToList();
            foreach (var promocaoPlanoVo in listaVo)
            {
                PlanoPagSeguroVo planoVo;
                if (!planoBll.ObterPlanoVo(promocaoPlanoVo.IdPlanoPagSeguro, out planoVo, ref mensagemErro))
                {
                    planoVo = new PlanoPagSeguroVo()
                    {
                        Nome = "Plano não localizado"
                    };
                }

                PromocaoPlanoDto promocaoPlanoDto = new PromocaoPlanoDto();
                if (!ConverterVoParaDto(promocaoPlanoVo, ref promocaoPlanoDto, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Problemas para converter para DTO: " + mensagemErro;
                    return false;
                }

                promocaoPlanoDto.CodigoSimplificado = planoVo.CodigoSimplificado;
                promocaoPlanoDto.NomePlanoPagSeguro = planoVo.Nome;
                retornoDto.ListaEntidades.Add(promocaoPlanoDto);
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtem as promoções por id do plano
        /// </summary>
        /// <param name="idPlano"></param>
        /// <param name="listaPromocoes"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        internal bool ObterPromocoesPorPlano(Guid idPlano, ref Dictionary<Guid, List<PromocaoPlanoVo>> listaPromocoes, ref string mensagemErro)
        {
            // Validar o id
            if (idPlano == Guid.NewGuid())
            {
                mensagemErro = $"Para obter as promoções dos planos informe o id.";
                return false;
            }

            // Obter a query primária
            IQueryable<PromocaoPlanoVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                mensagemErro = $"Houve um problema ao listar as promoções: {mensagemErro}";
                return false;
            }

            List<PromocaoPlanoVo> listaVo = query.Where(p => p.IdPlanoPagSeguro == idPlano).ToList();
            listaPromocoes.Add(idPlano, listaVo);

            return true;
        }

        /// <summary>
        /// Método não implementado
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<PromocaoPlanoDto> requisicaoDto, ref RetornoDto retornoDto)
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
        public override bool Editar(RequisicaoEntidadeDto<PromocaoPlanoDto> requisicaoDto, ref RetornoDto retornoDto)
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
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<PromocaoPlanoDto> retornoDto)
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
        /// Obtém uma lista de promoções com filtros aplicados, podendo ser paginada (não traz a senha nos objetos)
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<PromocaoPlanoDto> retornoDto)
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

            // Obter a query primária
            IQueryable<PromocaoPlanoVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as promoções: {mensagemErro}";
                retornoDto.Retorno = false;

                return false;
            }

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "IDPLANOPAGSEGURO":
                        Guid idPlano;
                        if (!Guid.TryParse(filtro.Value, out idPlano))
                        {
                            retornoDto.Mensagem = $"O filtro de loja não pôde ser convertido.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query = query.Where(p => p.IdPlanoPagSeguro == idPlano);
                        break;

                    case "IDPROMOCAO":
                        Guid idPromocao;
                        if (!Guid.TryParse(filtro.Value, out idPromocao))
                        {
                            retornoDto.Mensagem = $"O filtro de loja não pôde ser convertido.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query = query.Where(p => p.IdPromocao == idPromocao);
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
                case "IDPLANOPAGSEGURO":
                    query = query.OrderBy(p => p.IdPlanoPagSeguro);
                    break;

                case "IDPROMOCAO":
                    query = query.OrderBy(p => p.IdPromocao).ThenBy(p => p.IdPromocao);
                    break;

                default:
                    query = query.OrderBy(p => p.IdPlanoPagSeguro);
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

            List<PromocaoPlanoVo> listaVo = query.ToList();
            foreach (var promocaoPlano in listaVo)
            {
                PromocaoPlanoDto promocaoPlanoDto = new PromocaoPlanoDto();
                if (!ConverterVoParaDto(promocaoPlano, ref promocaoPlanoDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;
                    return false;
                }

                retornoDto.ListaEntidades.Add(promocaoPlanoDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Converte uma promoção Dto para uma promoção Vo
        /// </summary>
        /// <param name="promocaoPlanoDto"></param>
        /// <param name="promocaoPlanoVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(PromocaoPlanoDto promocaoPlanoDto, ref PromocaoPlanoVo promocaoPlanoVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(promocaoPlanoDto, ref promocaoPlanoVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                promocaoPlanoVo.Id = promocaoPlanoDto.Id;
                promocaoPlanoVo.IdPlanoPagSeguro = promocaoPlanoDto.IdPlanoPagSeguro;
                promocaoPlanoVo.IdPromocao = promocaoPlanoDto.IdPromocao;

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
        /// <param name="promocaoPlanoVo"></param>
        /// <param name="promocaoPlanoDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(PromocaoPlanoVo promocaoPlanoVo, ref PromocaoPlanoDto promocaoPlanoDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(promocaoPlanoVo, ref promocaoPlanoDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                promocaoPlanoDto.IdPlanoPagSeguro = promocaoPlanoVo.IdPlanoPagSeguro;
                promocaoPlanoDto.IdPromocao = promocaoPlanoVo.IdPromocao;

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
