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
    public class ModeloEmailBll : BaseBll<ModeloEmailVo, ModeloEmailDto>
    {
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public ModeloEmailBll(bool salvarAlteracoes) : base()
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public ModeloEmailBll(AaanoContexto contexto, bool salvarAlteracoes) : base(contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui uma modelo de email no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<ModeloEmailDto> requisicaoDto, ref RetornoDto retornoDto)
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
                retornoDto.Mensagem = "Este usuário não é administrador. Para cadastrar modelos de email é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            // Converte para VO a ser incluída no banco de dados
            ModeloEmailVo modeloEmailVo = new ModeloEmailVo();
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref modeloEmailVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter a modelo de email para VO: " + mensagemErro;

                return false;
            }

            // Prepara a inclusão no banco de dados
            if (!IncluirBd(modeloEmailVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao incluir a modelo de email: " + mensagemErro;
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
        /// Exclui uma modelo de email do banco de dados a partir do ID
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
                retornoDto.Mensagem = "Este usuário não é administrador. Para excluir os modelos de email é necessário " +
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
        /// Obtém uma modelo de email pelo ID (não traz a senha no objeto)
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<ModeloEmailDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar os modelos de email é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            ModeloEmailVo modeloEmailVo;
            if (!ObterPorIdBd(requisicaoDto.Id, out modeloEmailVo, ref mensagemErro))
            {
                // Se for o email de boas vindas ou o de cobrança e não for encontrado
                if ((requisicaoDto.Id == UtilitarioBll.RetornarIdEmailBoasVindas() ||
                    requisicaoDto.Id == UtilitarioBll.RetornarIdEmailCobranca()) &&
                    mensagemErro.Contains("não encontrado"))
                {
                    // Retorno dto vazia
                    modeloEmailVo = new ModeloEmailVo()
                    {
                        Id = requisicaoDto.Id,
                    };
                }
                else
                {
                    retornoDto.Mensagem = "Erro ao obter o modelo de email: " + mensagemErro;
                    retornoDto.Retorno = false;
                    return false;
                }
            }

            ModeloEmailDto modeloEmailDto = new ModeloEmailDto();
            if (!ConverterVoParaDto(modeloEmailVo, ref modeloEmailDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter o modelo de email: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            retornoDto.Entidade = modeloEmailDto;
            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Obtem a modelo de email vo para uso interno
        /// </summary>
        /// <param name="idModeloEmail"></param>
        /// <param name="modeloEmailVo"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        internal bool ObterModeloEmailVo(Guid idModeloEmail, out ModeloEmailVo modeloEmailVo, ref string mensagemErro)
        {
            return ObterPorIdBd(idModeloEmail, out modeloEmailVo, ref mensagemErro);
        }

        /// <summary>
        /// Obtém uma lista de modelos de email com filtros aplicados, podendo ser paginada (não traz a senha nos objetos)
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<ModeloEmailDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar os modelos de email é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            // Obter a query primária
            IQueryable<ModeloEmailVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar os modelos de email: {mensagemErro}";
                retornoDto.Retorno = false;

                return false;
            }

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "ASSUNTO":
                        query = query.Where(p => p.Assunto.Contains(filtro.Value));
                        break;

                    case "IGNORARFIXOS":
                        Guid idCobranca = UtilitarioBll.RetornarIdEmailCobranca();
                        Guid idBoasVindas = UtilitarioBll.RetornarIdEmailBoasVindas();
                        query = query.Where(p => p.Id != idCobranca && p.Id != idBoasVindas);
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
                case "ASSUNTO":
                    query = query.OrderBy(p => p.Assunto);
                    break;

                default:
                    query = query.OrderBy(p => p.Assunto);
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

            List<ModeloEmailVo> listaVo = query.ToList();
            foreach (var modeloEmail in listaVo)
            {
                ModeloEmailDto modeloEmailDto = new ModeloEmailDto();
                if (!ConverterVoParaDto(modeloEmail, ref modeloEmailDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;
                    return false;
                }

                retornoDto.ListaEntidades.Add(modeloEmailDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Edita uma modelo de email
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Editar(RequisicaoEntidadeDto<ModeloEmailDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Editar(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Apenos modelos de email ADM podem editar modeloEmailes
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para editar os modelos de email é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            ModeloEmailVo modeloEmailVo;
            if (!ObterPorIdBd(requisicaoDto.EntidadeDto.Id, out modeloEmailVo, ref mensagemErro))
            {
                // Se for o email de boas vindas ou o de cobrança e não for encontrado
                if ((requisicaoDto.EntidadeDto.Id == UtilitarioBll.RetornarIdEmailBoasVindas() ||
                    requisicaoDto.EntidadeDto.Id == UtilitarioBll.RetornarIdEmailCobranca()) &&
                    mensagemErro.Contains("não encontrado"))
                {
                    return Incluir(requisicaoDto, ref retornoDto);
                }
                else
                {
                    retornoDto.Mensagem = "Problemas para encontrar a modelo de email: " + mensagemErro;
                    retornoDto.Retorno = false;
                    return false;
                }
            }

            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref modeloEmailVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas ao converter a modelo de email para Vo: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            if (!EditarBd(modeloEmailVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao editar os novos dados da modelo de email: " + mensagemErro;
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
        /// Converte uma modelo de email Dto para uma modelo de email Vo
        /// </summary>
        /// <param name="modeloEmailDto"></param>
        /// <param name="modeloEmailVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(ModeloEmailDto modeloEmailDto, ref ModeloEmailVo modeloEmailVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(modeloEmailDto, ref modeloEmailVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                modeloEmailVo.Assunto = string.IsNullOrWhiteSpace(modeloEmailDto.Assunto) ? "" : modeloEmailDto.Assunto.Trim();
                modeloEmailVo.Corpo = string.IsNullOrWhiteSpace(modeloEmailDto.Corpo) ? "" : modeloEmailDto.Corpo.Trim();
                modeloEmailVo.Id = modeloEmailDto.Id;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a modelo de email para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte uma modelo de email Dto para uma modelo de email Vo
        /// </summary>
        /// <param name="modeloEmailVo"></param>
        /// <param name="modeloEmailDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(ModeloEmailVo modeloEmailVo, ref ModeloEmailDto modeloEmailDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(modeloEmailVo, ref modeloEmailDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                modeloEmailDto.Assunto = string.IsNullOrWhiteSpace(modeloEmailVo.Assunto) ? "" : modeloEmailVo.Assunto.Trim();
                modeloEmailDto.Corpo = string.IsNullOrWhiteSpace(modeloEmailVo.Corpo) ? "" : modeloEmailVo.Corpo.Trim();

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a modelo de email para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Envia um modelo de email para um endereço
        /// </summary>
        /// <param name="assinaturaVo"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        internal bool EnviarModeloEmailIndividual(AssinaturaPagSeguroVo assinaturaVo, Guid idModelo, ref RetornoDto retornoDto)
        {
            ModeloEmailVo modeloEmailVo;
            string mensagemErro = "";
            if (!ObterPorIdBd(idModelo, out modeloEmailVo, ref mensagemErro))
            {
                // Se for o email de boas vindas ou o de cobrança e não for encontrado
                if ((idModelo == UtilitarioBll.RetornarIdEmailBoasVindas() ||
                    idModelo == UtilitarioBll.RetornarIdEmailCobranca()) &&
                    mensagemErro.Contains("não encontrado"))
                {
                    return true;
                }
                else
                {
                    retornoDto.Mensagem = "Problemas para encontrar a modelo de email: " + mensagemErro;
                    return false;
                }
            }

            string corpoEmail = SubstituirTags(assinaturaVo, modeloEmailVo.Corpo, ref retornoDto);
            return UtilitarioBll.EnviarEmail(assinaturaVo.Email, modeloEmailVo.Assunto, corpoEmail, ref mensagemErro);
        }

        /// <summary>
        /// Envia um modelo de email para uma lista de assinaturas
        /// </summary>
        /// <param name="listaAssinaturas"></param>
        /// <param name="idModelo"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        internal bool EnviarModeloEmailListaDestinatario(List<AssinaturaPagSeguroVo> listaAssinaturas, Guid idModelo, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            ModeloEmailVo modeloEmailVo;
            if (!ObterPorIdBd(idModelo, out modeloEmailVo, ref mensagemErro))
            {
                // Se for o email de boas vindas ou o de cobrança e não for encontrado
                if ((idModelo == UtilitarioBll.RetornarIdEmailBoasVindas() ||
                    idModelo == UtilitarioBll.RetornarIdEmailCobranca()) &&
                    mensagemErro.Contains("não encontrado"))
                {
                    return true;
                }
                else
                {
                    retornoDto.Mensagem = "Problemas para encontrar a modelo de email: " + mensagemErro;
                    return false;
                }
            }

            int falha = 0;
            EmailEnviadoBll emailEnviadoBll = new EmailEnviadoBll(aaanoContexto, false);
            foreach (var assinaturaDto in listaAssinaturas)
            {
                string corpoEmail = SubstituirTags(assinaturaDto, modeloEmailVo.Corpo, ref retornoDto);
                if (!string.IsNullOrWhiteSpace(corpoEmail))
                {
                    bool sucesso = UtilitarioBll.EnviarEmail(assinaturaDto.Email, modeloEmailVo.Assunto, corpoEmail, ref mensagemErro);
                    if (!sucesso)
                    {
                        falha++;
                    }

                    emailEnviadoBll.IncluirEmailEnviado(idModelo, assinaturaDto.Id, sucesso);
                }
                else
                {
                    falha++;
                    emailEnviadoBll.IncluirEmailEnviado(idModelo, assinaturaDto.Id, false);
                }
            }

            retornoDto.Mensagem = $"Enviados {listaAssinaturas.Count - falha} de {listaAssinaturas.Count} emails";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Troca as TAGs pelos respectivos valores
        /// </summary>
        /// <param name="assinaturaVo"></param>
        /// <param name="corpo"></param>
        /// <returns></returns>
        private string SubstituirTags(AssinaturaPagSeguroVo assinaturaVo, string corpo, ref RetornoDto retornoDto)
        {
            if (assinaturaVo != null)
            {
                corpo = corpo.Replace("||NOMEASSINANTE||", assinaturaVo.NomeAssinate);
                corpo = corpo.Replace("||ULTIMOPAGAMENTO||", assinaturaVo.UltimoEnventoRegistrado.ToString("dd/MM/yyyy"));

                // Se houver a TAG da carteirinha
                if (corpo.Contains("URLCARTEIRINHA"))
                {
                    if (!UtilitarioBll.ObterLinkInformacoesAssinatura(assinaturaVo.Id, assinaturaVo.Email, ref retornoDto))
                    {
                        return "";
                    }

                    corpo = corpo.Replace("||URLCARTEIRINHA||", "https://" + retornoDto.Mensagem);
                }
            }

            corpo = corpo.Replace("||EMAILAAANO||", "contato@aaano.com.br");
            return corpo;
        }
    }
}
