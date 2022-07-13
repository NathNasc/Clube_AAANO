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
    public class LojaParceiraBll : BaseBll<LojaParceiraVo, LojaParceiraDto>
    {
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public LojaParceiraBll(bool salvarAlteracoes) : base()
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public LojaParceiraBll(AaanoContexto contexto, bool salvarAlteracoes) : base(contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui uma loja parceira no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<LojaParceiraDto> requisicaoDto, ref RetornoDto retornoDto)
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
                retornoDto.Mensagem = "Este usuário não é administrador. Para cadastrar loja parceiras é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            // Converte para VO a ser incluída no banco de dados
            LojaParceiraVo lojaParceiraVo = new LojaParceiraVo();
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref lojaParceiraVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter a loja parceira para VO: " + mensagemErro;

                return false;
            }

            // Prepara a inclusão no banco de dados
            if (!IncluirBd(lojaParceiraVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao incluir a loja: " + mensagemErro;
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
        /// Exclui uma loja parceira do banco de dados a partir do ID
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
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar as lojas parceiras é necessário " +
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
        /// Obtém uma loja parceira pelo ID (não traz a senha no objeto)
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<LojaParceiraDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar as lojas parceiras é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            LojaParceiraVo lojaParceiraVo;
            if (!ObterPorIdBd(requisicaoDto.Id, out lojaParceiraVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter a loja parceira: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            LojaParceiraDto lojaParceiraDto = new LojaParceiraDto();
            if (!ConverterVoParaDto(lojaParceiraVo, ref lojaParceiraDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter a loja parceira: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            retornoDto.Entidade = lojaParceiraDto;
            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Obtem a loja vo para uso interno
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="lojaVo"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        internal bool ObterLojaVo(Guid idLoja, out LojaParceiraVo lojaVo, ref string mensagemErro)
        {
            return ObterPorIdBd(idLoja, out lojaVo, ref mensagemErro);
        }

        /// <summary>
        /// Converte uma loja parceira Dto para uma loja parceira Vo
        /// </summary>
        /// <param name="lojaParceiraDto"></param>
        /// <param name="lojaParceiraVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(LojaParceiraDto lojaParceiraDto, ref LojaParceiraVo lojaParceiraVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(lojaParceiraDto, ref lojaParceiraVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                lojaParceiraVo.Endereco = string.IsNullOrWhiteSpace(lojaParceiraDto.Endereco) ? "" : lojaParceiraDto.Endereco.Trim();
                lojaParceiraVo.Nome = string.IsNullOrWhiteSpace(lojaParceiraDto.Nome) ? "" : lojaParceiraDto.Nome.Trim();
                lojaParceiraVo.Telefone = string.IsNullOrWhiteSpace(lojaParceiraDto.Telefone) ? "" : lojaParceiraDto.Telefone.Trim().Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "");
                lojaParceiraVo.Id = lojaParceiraDto.Id;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a loja parceira para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte uma loja parceira Dto para uma loja parceira Vo
        /// </summary>
        /// <param name="lojaParceiraVo"></param>
        /// <param name="lojaParceiraDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(LojaParceiraVo lojaParceiraVo, ref LojaParceiraDto lojaParceiraDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(lojaParceiraVo, ref lojaParceiraDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                lojaParceiraDto.Endereco = string.IsNullOrWhiteSpace(lojaParceiraVo.Endereco) ? "" : lojaParceiraVo.Endereco.Trim();
                lojaParceiraDto.Nome = string.IsNullOrWhiteSpace(lojaParceiraVo.Nome) ? "" : lojaParceiraVo.Nome.Trim();
                lojaParceiraDto.Telefone = string.IsNullOrWhiteSpace(lojaParceiraVo.Telefone) ? "" : lojaParceiraVo.Telefone.Trim();

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a loja parceira para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtém uma lista de loja parceiras com filtros aplicados, podendo ser paginada (não traz a senha nos objetos)
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<LojaParceiraDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar as lojas parceiras é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            // Obter a query primária
            IQueryable<LojaParceiraVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as lojas parceiras: {mensagemErro}";
                retornoDto.Retorno = false;

                return false;
            }

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "ENDERECO":
                        query = query.Where(p => p.Endereco.Contains(filtro.Value));
                        break;

                    case "NOME":
                        query = query.Where(p => p.Nome.Contains(filtro.Value));
                        break;

                    case "TELEFONE":
                        string telefone = filtro.Value.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "");
                        query = query.Where(p => p.Telefone.Contains(telefone));
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

                case "ENDERECO":
                    query = query.OrderBy(p => p.Endereco);
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

            List<LojaParceiraVo> listaVo = query.ToList();
            foreach (var lojaParceira in listaVo)
            {
                LojaParceiraDto lojaParceiraDto = new LojaParceiraDto();
                if (!ConverterVoParaDto(lojaParceira, ref lojaParceiraDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;
                    return false;
                }

                retornoDto.ListaEntidades.Add(lojaParceiraDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Edita uma loja parceira
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Editar(RequisicaoEntidadeDto<LojaParceiraDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Editar(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Apenas loja parceiras ADM podem editar lojaParceiraes
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para editar as lojas parceiras é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            LojaParceiraVo lojaParceiraVo;
            if (!ObterPorIdBd(requisicaoDto.EntidadeDto.Id, out lojaParceiraVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas para encontrar a loja: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref lojaParceiraVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas ao converter a loja parceira para Vo: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            if (!EditarBd(lojaParceiraVo, ref mensagemErro))
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
    }
}
