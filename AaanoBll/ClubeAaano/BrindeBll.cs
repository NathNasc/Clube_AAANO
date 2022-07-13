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
    public class BrindeBll : BaseBll<BrindeVo, BrindeDto>
    {
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public BrindeBll(bool salvarAlteracoes) : base()
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public BrindeBll(AaanoContexto contexto, bool salvarAlteracoes) : base(contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui uma brinde no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<BrindeDto> requisicaoDto, ref RetornoDto retornoDto)
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
                retornoDto.Mensagem = "Este usuário não é administrador. Para cadastrar brindes é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            // Converte para VO a ser incluída no banco de dados
            BrindeVo brindeVo = new BrindeVo();
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref brindeVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter a brinde para VO: " + mensagemErro;

                return false;
            }

            // Prepara a inclusão no banco de dados
            if (!IncluirBd(brindeVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao incluir a brinde: " + mensagemErro;
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
        /// Exclui uma brinde do banco de dados a partir do ID
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
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar as brindes é necessário " +
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
        /// Obtém uma brinde pelo ID (não traz a senha no objeto)
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<BrindeDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar as brindes é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            BrindeVo brindeVo;
            if (!ObterPorIdBd(requisicaoDto.Id, out brindeVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter o brinde: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            BrindeDto brindeDto = new BrindeDto();
            if (!ConverterVoParaDto(brindeVo, ref brindeDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter o brinde: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            // Se tiver ganhador, identificar o assinante
            if (brindeDto.IdAssinaturaPagSeguro != null && brindeDto.IdAssinaturaPagSeguro != Guid.Empty)
            {
                AssinaturaPagSeguroBll assinaturaPagSeguroBll = new AssinaturaPagSeguroBll(aaanoContexto, false);
                AssinaturaPagSeguroVo assinaturaPagSeguroVo;

                if (!assinaturaPagSeguroBll.ObterAssinaturaVo(brindeDto.IdAssinaturaPagSeguro.Value, out assinaturaPagSeguroVo, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao obter a loja: " + mensagemErro;
                    retornoDto.Retorno = false;
                    return false;
                }

                brindeDto.NomeAssinante = assinaturaPagSeguroVo.NomeAssinate;
            }
            else
            {
                brindeDto.NomeAssinante = "";

            }

            retornoDto.Entidade = brindeDto;
            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Obtém uma lista de brindes com filtros aplicados, podendo ser paginada (não traz a senha nos objetos)
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<BrindeDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar as brindes é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            // Obter a query primária
            IQueryable<BrindeVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as brindes: {mensagemErro}";
                retornoDto.Retorno = false;

                return false;
            }

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "DESCRICAO":
                        query = query.Where(p => p.Descricao.Contains(filtro.Value));
                        break;

                    case "IDASSINANTE":
                        Guid idAssinante;
                        if (!Guid.TryParse(filtro.Value, out idAssinante))
                        {
                            retornoDto.Mensagem = $"O filtro de assinante não pôde ser convertido.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query = query.Where(p => p.IdAssinaturaPagSeguro == idAssinante);
                        break;

                    case "APENASENTREGUES":
                        query = query.Where(p => p.Entregue == true);
                        break;

                    case "APENASPENDENTESDEENTREGA":
                        query = query.Where(p => p.Entregue == false);
                        break;

                    case "SORTEIOINICIO":
                        DateTime data;
                        if (DateTime.TryParse(filtro.Value, out data))
                        {
                            retornoDto.Mensagem = $"O filtro de sorteio (inicio) não pôde ser convertido.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query = query.Where(p => p.Sorteio >= data);
                        break;

                    case "SORTEIOFIM":
                        DateTime dataFim;
                        if (DateTime.TryParse(filtro.Value, out dataFim))
                        {
                            retornoDto.Mensagem = $"O filtro de sorteio (fim) não pôde ser convertido.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query = query.Where(p => p.Sorteio <= dataFim);
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
                case "DESCRICAO":
                    query = query.OrderBy(p => p.Descricao).ThenBy(p => p.Sorteio);
                    break;

                case "SORTEIO":
                    query = query.OrderBy(p => p.Sorteio).ThenBy(p => p.Descricao);
                    break;

                default:
                    query = query.OrderBy(p => p.Descricao);
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

            AssinaturaPagSeguroBll assinaturaPagSeguroBll = new AssinaturaPagSeguroBll(aaanoContexto, false);
            List<BrindeVo> listaVo = query.ToList();
            foreach (var brinde in listaVo)
            {
                BrindeDto brindeDto = new BrindeDto();
                if (!ConverterVoParaDto(brinde, ref brindeDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;
                    return false;
                }

                if (brindeDto.IdAssinaturaPagSeguro != null && brindeDto.IdAssinaturaPagSeguro != Guid.Empty)
                {
                    AssinaturaPagSeguroVo assinaturaPagSeguroVo;
                    if (!assinaturaPagSeguroBll.ObterAssinaturaVo(brindeDto.IdAssinaturaPagSeguro.Value, out assinaturaPagSeguroVo, ref mensagemErro))
                    {
                        retornoDto.Mensagem = "Erro ao obter a loja: " + mensagemErro;
                        retornoDto.Retorno = false;
                        return false;
                    }

                    brindeDto.NomeAssinante = assinaturaPagSeguroVo.NomeAssinate;
                }
                else
                {
                    brindeDto.NomeAssinante = "";

                }

                retornoDto.ListaEntidades.Add(brindeDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Edita uma brinde
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Editar(RequisicaoEntidadeDto<BrindeDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Editar(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Apenas brindes ADM podem editar brindees
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para editar as brindes é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            BrindeVo brindeVo;
            if (!ObterPorIdBd(requisicaoDto.EntidadeDto.Id, out brindeVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas para encontrar a brinde: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref brindeVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas ao converter a brinde para Vo: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            if (!EditarBd(brindeVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao editar os novos dados da brinde: " + mensagemErro;
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
        /// Converte uma brinde Dto para uma brinde Vo
        /// </summary>
        /// <param name="brindeDto"></param>
        /// <param name="brindeVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(BrindeDto brindeDto, ref BrindeVo brindeVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(brindeDto, ref brindeVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                brindeVo.Descricao = string.IsNullOrWhiteSpace(brindeDto.Descricao) ? "" : brindeDto.Descricao.Trim();
                brindeVo.Sorteio = brindeDto.Sorteio;
                brindeVo.Entregue = brindeDto.Entregue;
                brindeVo.Id = brindeDto.Id;
                brindeVo.IdAssinaturaPagSeguro = brindeDto.IdAssinaturaPagSeguro;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a brinde para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte uma brinde Dto para uma brinde Vo
        /// </summary>
        /// <param name="brindeVo"></param>
        /// <param name="brindeDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(BrindeVo brindeVo, ref BrindeDto brindeDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(brindeVo, ref brindeDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                brindeDto.Descricao = string.IsNullOrWhiteSpace(brindeVo.Descricao) ? "" : brindeVo.Descricao.Trim();
                brindeDto.Sorteio = brindeVo.Sorteio;
                brindeDto.Entregue = brindeVo.Entregue;
                brindeDto.IdAssinaturaPagSeguro = brindeVo.IdAssinaturaPagSeguro;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a brinde para Vo: " + ex.Message;
                return false;
            }
        }

    }
}
