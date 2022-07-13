using AaanoBll.Base;
using AaanoDal;
using AaanoDto.ClubeAaano;
using AaanoDto.Requisicoes;
using AaanoDto.Retornos;
using AaanoVo.ClubeAaano;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace AaanoBll.ClubeAaano
{
    public class PromocaoBll : BaseBll<PromocaoVo, PromocaoDto>
    {
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public PromocaoBll(bool salvarAlteracoes) : base()
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public PromocaoBll(AaanoContexto contexto, bool salvarAlteracoes) : base(contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui uma promoção no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<PromocaoDto> requisicaoDto, ref RetornoDto retornoDto)
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
                retornoDto.Mensagem = "Este usuário não é administrador. Para cadastrar promoções é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            // Converte para VO a ser incluída no banco de dados
            PromocaoVo promocaoVo = new PromocaoVo();
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref promocaoVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter a promoção para VO: " + mensagemErro;

                return false;
            }

            // Prepara a inclusão no banco de dados
            if (!IncluirBd(promocaoVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao incluir a promoção: " + mensagemErro;
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
        /// Exclui uma promoção do banco de dados a partir do ID
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
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar as promoções é necessário " +
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
        /// Obtém uma promoção pelo ID (não traz a senha no objeto)
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<PromocaoDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
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

            PromocaoVo promocaoVo;
            if (!ObterPorIdBd(requisicaoDto.Id, out promocaoVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter o promocao: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            PromocaoDto promocaoDto = new PromocaoDto();
            if (!ConverterVoParaDto(promocaoVo, ref promocaoDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter o promoção: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            LojaParceiraBll lojaParceiraBll = new LojaParceiraBll(aaanoContexto, false);
            LojaParceiraVo lojaParceiraVo;

            if (!lojaParceiraBll.ObterLojaVo(promocaoDto.IdLojaParceira, out lojaParceiraVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter a loja: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            promocaoDto.NomeLojaParceira = lojaParceiraVo.Nome;

            retornoDto.Entidade = promocaoDto;
            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Obtem a promoção vo para uso interno
        /// </summary>
        /// <param name="idPromocao"></param>
        /// <param name="promocaoVo"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        internal bool ObterPromocaoVo(Guid idPromocao, out PromocaoVo promocaoVo, ref string mensagemErro)
        {
            return ObterPorIdBd(idPromocao, out promocaoVo, ref mensagemErro);
        }

        /// <summary>
        /// Obtém uma lista de promoções com filtros aplicados, podendo ser paginada (não traz a senha nos objetos)
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<PromocaoDto> retornoDto)
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
            IQueryable<PromocaoVo> query;
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
                    case "RESUMO":
                        query = query.Where(p => p.Resumo.Contains(filtro.Value));
                        break;

                    case "IDLOJAPARCEIRA":
                        Guid idLoja;
                        if (!Guid.TryParse(filtro.Value, out idLoja))
                        {
                            retornoDto.Mensagem = $"O filtro de loja não pôde ser convertido.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query = query.Where(p => p.IdLojaParceira == idLoja);
                        break;

                    case "DETALHES":
                        query = query.Where(p => p.Detalhes.Contains(filtro.Value));
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
                case "RESUMO":
                    query = query.OrderBy(p => p.Resumo);
                    break;

                case "IDLOJPARCEIRA":
                    query = query.OrderBy(p => p.IdLojaParceira).ThenBy(p => p.Resumo);
                    break;

                default:
                    query = query.OrderBy(p => p.Resumo);
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

            LojaParceiraBll lojaParceiraBll = new LojaParceiraBll(aaanoContexto, false);
            List<PromocaoVo> listaVo = query.ToList();
            foreach (var promocao in listaVo)
            {
                PromocaoDto promocaoDto = new PromocaoDto();
                if (!ConverterVoParaDto(promocao, ref promocaoDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;
                    return false;
                }

                LojaParceiraVo lojaParceiraVo;
                if (!lojaParceiraBll.ObterLojaVo(promocaoDto.IdLojaParceira, out lojaParceiraVo, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao obter a loja: " + mensagemErro;
                    retornoDto.Retorno = false;
                    return false;
                }

                promocaoDto.NomeLojaParceira = lojaParceiraVo.Nome;
                retornoDto.ListaEntidades.Add(promocaoDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Edita uma promoção
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Editar(RequisicaoEntidadeDto<PromocaoDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Editar(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Apenas promoções ADM podem editar promocaoes
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para editar as promoções é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            PromocaoVo promocaoVo;
            if (!ObterPorIdBd(requisicaoDto.EntidadeDto.Id, out promocaoVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas para encontrar a promoção: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref promocaoVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas ao converter a promoção para Vo: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            if (!EditarBd(promocaoVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao editar os novos dados da promoção: " + mensagemErro;
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
        /// Converte uma promoção Dto para uma promoção Vo
        /// </summary>
        /// <param name="promocaoDto"></param>
        /// <param name="promocaoVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(PromocaoDto promocaoDto, ref PromocaoVo promocaoVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(promocaoDto, ref promocaoVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                promocaoVo.Resumo = string.IsNullOrWhiteSpace(promocaoDto.Resumo) ? "" : promocaoDto.Resumo.Trim();
                promocaoVo.Detalhes = string.IsNullOrWhiteSpace(promocaoDto.Detalhes) ? "" : promocaoDto.Detalhes.Trim();
                promocaoVo.Id = promocaoDto.Id;
                promocaoVo.IdLojaParceira = promocaoDto.IdLojaParceira;

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
        /// <param name="promocaoVo"></param>
        /// <param name="promocaoDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(PromocaoVo promocaoVo, ref PromocaoDto promocaoDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(promocaoVo, ref promocaoDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                promocaoDto.Resumo = string.IsNullOrWhiteSpace(promocaoVo.Resumo) ? "" : promocaoVo.Resumo.Trim();
                promocaoDto.Detalhes = string.IsNullOrWhiteSpace(promocaoVo.Detalhes) ? "" : promocaoVo.Detalhes.Trim();
                promocaoDto.IdLojaParceira = promocaoVo.IdLojaParceira;

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
