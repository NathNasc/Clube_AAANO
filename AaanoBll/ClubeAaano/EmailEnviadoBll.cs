using AaanoBll.Base;
using AaanoDal;
using AaanoDto.ClubeAaano;
using AaanoDto.Requisicoes;
using AaanoDto.Retornos;
using AaanoVo.ClubeAaano;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace AaanoBll.ClubeAaano
{
    public class EmailEnviadoBll : BaseBll<EmailEnviadoVo, EmailEnviadoDto>
    {
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public EmailEnviadoBll(bool salvarAlteracoes) : base()
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public EmailEnviadoBll(AaanoContexto contexto, bool salvarAlteracoes) : base(contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui umo email no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<EmailEnviadoDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            // Valida a requisição
            if (!base.Incluir(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Converte para VO a ser incluída no banco de dados
            string mensagemErro = "";
            EmailEnviadoVo emailEnviadoVo = new EmailEnviadoVo();
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref emailEnviadoVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o email para VO: " + mensagemErro;

                return false;
            }

            // Prepara a inclusão no banco de dados
            if (!IncluirBd(emailEnviadoVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao incluir o email: " + mensagemErro;
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
        /// Inclui um registro de email enviado para métodos internos
        /// </summary>
        /// <param name="idModelo"></param>
        /// <param name="idAssinante"></param>
        /// <param name="sucesso"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        internal bool IncluirEmailEnviado(Guid idModelo, Guid idAssinante, bool sucesso)
        {
            EmailEnviadoVo emailEnviadoVo = new EmailEnviadoVo()
            {
                Id = Guid.NewGuid(),
                IdAssinaturaPagSeguro = idAssinante,
                IdModeloEmail = idModelo,
                SucessoEnvio = sucesso
            };

            // Prepara a inclusão no banco de dados
            string mensagemErro = "";
            if (!IncluirBd(emailEnviadoVo, ref mensagemErro))
            {
                return false;
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
        /// Exclui umo email do banco de dados a partir do ID
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
        /// Obtém umo email pelo ID (não traz a senha no objeto)
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<EmailEnviadoDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar os emails enviados é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            EmailEnviadoVo emailEnviadoVo;
            if (!ObterPorIdBd(requisicaoDto.Id, out emailEnviadoVo, ref mensagemErro))
            {
                // Se for o email de boas vindas ou o de cobrança e não for encontrado
                if ((requisicaoDto.Id == UtilitarioBll.RetornarIdEmailBoasVindas() ||
                    requisicaoDto.Id == UtilitarioBll.RetornarIdEmailCobranca()) &&
                    mensagemErro.Contains("não encontrado"))
                {
                    // Retorno dto vazia
                    emailEnviadoVo = new EmailEnviadoVo()
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

            EmailEnviadoDto emailEnviadoDto = new EmailEnviadoDto();
            if (!ConverterVoParaDto(emailEnviadoVo, ref emailEnviadoDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter o modelo de email: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            retornoDto.Entidade = emailEnviadoDto;
            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Obtém uma lista de emails enviados com filtros aplicados, podendo ser paginada (não traz a senha nos objetos)
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<EmailEnviadoDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar os emails enviados é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            // Obter a query primária
            string query = "";
            List<MySqlParameter> listaFiltros = new List<MySqlParameter>();

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "IDMODELOEMAIL":
                        Guid idModelo;

                        if (!Guid.TryParse(filtro.Value, out idModelo))
                        {
                            retornoDto.Mensagem = $"O filtro de modelos de email não pôde ser convertido.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query += string.IsNullOrWhiteSpace(query) ? "WHERE " : "AND ";
                        query += "e.IdModeloEmail = @idModelo";

                        listaFiltros.Add(new MySqlParameter("idModelo", idModelo));
                        break;

                    case "SUCESSO":
                        bool sucesso;
                        if (!bool.TryParse(filtro.Value, out sucesso))
                        {
                            retornoDto.Mensagem = $"O filtro de sucesso não pôde ser convertido.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query += string.IsNullOrWhiteSpace(query) ? " WHERE " : " AND ";
                        query += "e.SucessoEnvio = @sucesso";

                        listaFiltros.Add(new MySqlParameter("sucesso", sucesso));
                        break;

                    case "DATAENVIOINICIAL":
                        DateTime dataInicio;

                        if (!DateTime.TryParse(filtro.Value, out dataInicio))
                        {
                            retornoDto.Mensagem = $"O filtro de data (inicio) não pôde ser convertido.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query += string.IsNullOrWhiteSpace(query) ? " WHERE " : " AND ";
                        query += "CAST(e.DataInclusao AS Date) >= @dataInicio";

                        listaFiltros.Add(new MySqlParameter("dataInicio", dataInicio.ToString("yyyy-MM-dd")));
                        break;

                    case "DATAENVIOFINAL":
                        DateTime dataFim;

                        if (!DateTime.TryParse(filtro.Value, out dataFim))
                        {
                            retornoDto.Mensagem = $"O filtro de data (fim) não pôde ser convertido.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query += string.IsNullOrWhiteSpace(query) ? " WHERE " : " AND ";
                        query += "CAST(e.DataInclusao AS Date) <= @dataFim";

                        listaFiltros.Add(new MySqlParameter("dataFim", dataFim.ToString("yyyy-MM-dd")));
                        break;

                    case "IDASSINATURAPAGSEGURO":
                        Guid idAssinatura;

                        if (!Guid.TryParse(filtro.Value, out idAssinatura))
                        {
                            retornoDto.Mensagem = $"O filtro de assinatura não pôde ser convertido.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query += string.IsNullOrWhiteSpace(query) ? " WHERE " : " AND ";
                        query += "e.IdAssinaturaPagSeguro = @idAssinatura";

                        listaFiltros.Add(new MySqlParameter("idAssinatura", idAssinatura));
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
                case "DATAENVIO":
                    query += " ORDER BY e.DataInclusao, a.NomeAssinate";
                    break;

                case "DATAENVIODECRESCENTE":
                    query += " ORDER BY e.DataInclusao DESC, a.NomeAssinate";
                    break;

                case "SUCESSO":
                    query += " ORDER BY e.SucessoEnvio, e.DataInclusao, a.NomeAssinate";
                    break;

                case "NOMEASSINANTE":
                    query += " ORDER BY a.NomeAssinate, e.DataInclusao";
                    break;

                case "ASSUNTOEMAIL":
                    query += " ORDER BY m.Assunto, e.DataInclusao";
                    break;

                default:
                    query += " ORDER BY e.DataInclusao, a.NomeAssinate";
                    break;
            }

            query = "FROM EmailsEnviados AS e " +
                "INNER JOIN AssinaturasPagSeguro AS a ON (e.IdAssinaturaPagSeguro = a.Id) " +
                "INNER JOIN ModelosEmail AS m ON (e.IdModeloEmail = m.Id) " + query;
            double totalItens = 0;

            try
            {
                using (AaanoContexto contexto = new AaanoContexto())
                {
                    string selectCount = "SELECT COUNT(e.Id) " + query;
                    List<MySqlParameter> listaFiltrosCount = listaFiltros.ToList();
                    totalItens = contexto.Database.SqlQuery<double>(selectCount, listaFiltrosCount.ToArray()).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                retornoDto.Mensagem = "Falha ao obter a quantidade de registros: " + ex.Message;
                return false;
            }

            retornoDto.TotalItens = (int)totalItens;
            if (totalItens == 0)
            {
                retornoDto.NumeroPaginas = 0;
                retornoDto.Mensagem = "Nenhum resultado encontrado.";
                retornoDto.Retorno = true;
                return true;
            }

            string select = "SELECT" +
                               " e.Id," +
                               " e.IdAssinaturaPagSeguro," +
                               " e.IdModeloEmail," +
                               " RTRIM(a.NomeAssinate) AS NomeAssinante," +
                               " RTRIM(m.Assunto) AS Assunto," +
                               " e.DataInclusao," +
                               " e.SucessoEnvio " + query;

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
                    retornoDto.ListaEntidades = contexto.Database.SqlQuery<EmailEnviadoDto>(select, listaFiltros.ToArray()).ToList();
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
        /// Não implementado
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Editar(RequisicaoEntidadeDto<EmailEnviadoDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            retornoDto.Mensagem = "Método não implementado";
            retornoDto.Retorno = false;
            return false;
        }

        /// <summary>
        /// Converte umo email Dto para umo email Vo
        /// </summary>
        /// <param name="emailEnviadoDto"></param>
        /// <param name="emailEnviadoVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(EmailEnviadoDto emailEnviadoDto, ref EmailEnviadoVo emailEnviadoVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(emailEnviadoDto, ref emailEnviadoVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                emailEnviadoVo.IdAssinaturaPagSeguro = emailEnviadoDto.IdAssinaturaPagSeguro;
                emailEnviadoVo.IdModeloEmail = emailEnviadoDto.IdModeloEmail;
                emailEnviadoVo.SucessoEnvio = emailEnviadoDto.SucessoEnvio;
                emailEnviadoVo.Id = emailEnviadoDto.Id;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o email para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte umo email Dto para umo email Vo
        /// </summary>
        /// <param name="emailEnviadoVo"></param>
        /// <param name="emailEnviadoDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(EmailEnviadoVo emailEnviadoVo, ref EmailEnviadoDto emailEnviadoDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(emailEnviadoVo, ref emailEnviadoDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                emailEnviadoDto.IdAssinaturaPagSeguro = emailEnviadoVo.IdAssinaturaPagSeguro;
                emailEnviadoDto.IdModeloEmail = emailEnviadoVo.IdModeloEmail;
                emailEnviadoDto.SucessoEnvio = emailEnviadoVo.SucessoEnvio;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o email para Vo: " + ex.Message;
                return false;
            }
        }

    }
}
