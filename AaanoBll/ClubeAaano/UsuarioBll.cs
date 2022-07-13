using AaanoBll.Base;
using AaanoDal;
using AaanoDto.Base;
using AaanoDto.ClubeAaano;
using AaanoDto.ClubeAaano.Relatorios;
using AaanoDto.Requisicoes;
using AaanoDto.Retornos;
using AaanoVo.ClubeAaano;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using static AaanoEnum.PagSeguroEnum;

namespace AaanoBll.ClubeAaano
{
    public class UsuarioBll : BaseBll<UsuarioVo, UsuarioDto>
    {
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public UsuarioBll(bool salvarAlteracoes) : base()
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public UsuarioBll(AaanoContexto contexto, bool salvarAlteracoes) : base(contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui um usuario no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<UsuarioDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            // Valida a requisição
            if (!base.Incluir(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Converte para VO a ser incluída no banco de dados
            string mensagemErro = "";
            UsuarioVo usuarioVo = new UsuarioVo();
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref usuarioVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o usuario para VO: " + mensagemErro;

                return false;
            }

            // Verifica se o usuario já existe
            UsuarioVo usuarioExistente = null;
            if (!VerificarUsuarioExistente(requisicaoDto.EntidadeDto, ref usuarioExistente, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            // Se existir, não permitir incluir duplicado
            if (usuarioExistente != null)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Esse cadastro (usuário) já existe, não é possível incluir cadastros duplicados.";
                return false;
            }
            else
            {
                // Prepara a inclusão no banco de dados
                if (!IncluirBd(usuarioVo, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao converter o usuario para VO: " + mensagemErro;
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
        /// Exclui um usuário do banco de dados a partir do ID
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
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar os usuários é necessário " +
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
        /// Faz o login com email e senha
        /// </summary>
        public bool FazerLogin(RequisicaoFazerLoginDto requisicaoDto, ref RetornoFazerLoginDto retornoDto)
        {
            //Validar email e senha
            if (string.IsNullOrWhiteSpace(requisicaoDto.Email))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "O email é obrigatório para fazer o login";
                return false;
            }

            if (string.IsNullOrWhiteSpace(requisicaoDto.Senha))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "A senha é obrigatória para fazer o login";
                return false;
            }

            string nomeUsuario = "Suporte";
            Guid idUsuario = UtilitarioBll.RetornarIdUsuarioSuporte();
            bool usuarioAdm = false;

            //Se for o usuário suporte
            if (requisicaoDto.Email.Trim().ToUpper() == "SUPORTE")
            {
                string senhaCriptografada = "";
                UtilitarioBll.CriptografarSenha("chave", ref senhaCriptografada);

                if (requisicaoDto.Senha.Trim() != senhaCriptografada)
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Senha de suporte incorreta.";
                    return false;
                }

                usuarioAdm = true;
            }
            else
            {
                string mensagemErro = "";
                IQueryable<UsuarioVo> query;
                if (!ObterQueryBd(out query, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = $"Falha ao listar os usuários: {mensagemErro}";
                    return false;
                }

                UsuarioVo usuarioVo;

                //Procurar o email com o flag ativo
                query = query.Where(u => u.Email.Trim() == requisicaoDto.Email.Trim());

                try
                {
                    usuarioVo = query.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = $"Falha ao obter o usuário do banco de dados: {ex.Message}";
                    return false;
                }

                if (usuarioVo == null)
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = $"Email ou senha inválidos. ";
                    return false;
                }

                if (!requisicaoDto.Senha.Equals(usuarioVo.Senha))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = $"Email ou senha inválidos ";
                    return false;
                }

                usuarioAdm = usuarioVo.Administrador;
                nomeUsuario = usuarioVo.Nome;
                idUsuario = usuarioVo.Id;
            }

            string identificacao = DateTime.Now.ToString("dd/MM/yyyy HH:mm") + UtilitarioBll.RetornaGuidValidação() + idUsuario.ToString() + $"Adm={(usuarioAdm ? "1" : "0")}";
            string identificacaoCriptografada = "";

            if (!UtilitarioBll.CriptografarString(identificacao, ref identificacaoCriptografada))
            {
                retornoDto.Mensagem = "Falha ao fazer o login: Não foi possível obter a identificação.";
                retornoDto.Retorno = false;
                return false;
            }

            retornoDto.IdUsuario = idUsuario;
            retornoDto.NomeUsuario = nomeUsuario;
            retornoDto.Identificacao = identificacaoCriptografada;
            retornoDto.UsuarioAdministrador = usuarioAdm;

            // Se não for adm, buscar as lojas de permissão
            if (!usuarioAdm)
            {
                RequisicaoObterDto requisicaoObterDto = new RequisicaoObterDto()
                {
                    Id = idUsuario,
                    Identificacao = identificacaoCriptografada,
                    IdUsuario = idUsuario
                };

                PermissaoUsuarioBll permissaoUsuarioBll = new PermissaoUsuarioBll(false);
                RetornoObterListaDto<PermissaoUsuarioDto> retornoObterDto = new RetornoObterListaDto<PermissaoUsuarioDto>();
                if (!permissaoUsuarioBll.ObterPermissoesUsuario(requisicaoObterDto, ref retornoObterDto))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = retornoObterDto.Mensagem;
                    return false;
                }

                retornoDto.ListaLojas = retornoObterDto.ListaEntidades.Select(p => p.IdLojaParceira).ToList();
            }

            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Envia um email com uma nova senha
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        public bool EnviarEmailRecuperacao(RequisicaoFazerLoginDto requisicaoDto, RetornoDto retornoDto)
        {
            if (string.IsNullOrWhiteSpace(requisicaoDto.Email))
            {
                retornoDto.Mensagem = "Informe o email para recuperar a senha.";
                retornoDto.Retorno = false;
                return false;
            }

            string mensagemErro = "";
            IQueryable<UsuarioVo> query;
            if (!ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = $"Erro ao listar os usuários: {mensagemErro}";
                return false;
            }

            //Procurar o email com o flag válido
            UsuarioVo usuarioVo;
            query = query.Where(u => u.Email.Trim() == requisicaoDto.Email.Trim());

            try
            {
                usuarioVo = query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = $"Falha ao obter o usuário do banco de dados: {ex.Message}";
                return false;
            }

            if (usuarioVo == null)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = $"Email não encontrado.";
                return false;
            }

            string opcoes = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@abcdefghijklmnopqrstuvxzwy";
            Random random = new Random();
            string senha = new string(Enumerable.Repeat(opcoes, 8).Select(s => s[random.Next(s.Length)]).ToArray());

            string senhaCriptografada = "";
            UtilitarioBll.CriptografarSenha(senha, ref senhaCriptografada);
            usuarioVo.Senha = senhaCriptografada;

            if (!EditarBd(usuarioVo, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Erro ao editar o usuário: {retornoDto.Mensagem}";
                return false;
            }

            UsuarioDto usuarioDto = new UsuarioDto();
            if (!ConverterVoParaDto(usuarioVo, ref usuarioDto, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Erro ao converter o usuário de Vo para Dto: {mensagemErro}";
                retornoDto.Retorno = false;
                return false;
            }

            string corpoEmail = $"<p> Olá <strong>{usuarioVo.Nome}</strong></p>" +
                                  "<p> Sua senha para acessar o sistema do Clube da AAANO foi recuperada. Você poderá utilizar essa senha para acessar " +
                                  "o sistema e, se desejar, você poderá alterar esta senha editando o seu usuário.</p>" +
                                  $"<p> Sua nova senha é: <strong>{senha}</strong></p><br/>" +
                                  "<p> Por favor não responda este e-mail.</p>";

            if (!UtilitarioBll.EnviarEmail(usuarioVo.Email, "Recuperação de senha - Aaano", corpoEmail, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Problemas para enviar o email com a nova senha. Se o erro persistir, entre em contato com o suporte. Mensagem: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            // Salva as alterações
            if (!aaanoContexto.Salvar(ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Problemas para salvar a nova senha: " + mensagemErro;
                return false;
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Obtém um usuário pelo ID (não traz a senha no objeto)
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<UsuarioDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar os usuários é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            UsuarioVo usuarioVo;
            if (!ObterPorIdBd(requisicaoDto.Id, out usuarioVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter o usuario: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            UsuarioDto usuarioDto = new UsuarioDto();
            if (!ConverterVoParaDto(usuarioVo, ref usuarioDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter o usuário: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            retornoDto.Entidade = usuarioDto;
            retornoDto.Entidade.Senha = "";
            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Converte um usuario Dto para um usuario Vo
        /// </summary>
        /// <param name="usuarioDto"></param>
        /// <param name="usuarioVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(UsuarioDto usuarioDto, ref UsuarioVo usuarioVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(usuarioDto, ref usuarioVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                usuarioVo.Email = string.IsNullOrWhiteSpace(usuarioDto.Email) ? "" : usuarioDto.Email.Trim();
                usuarioVo.Nome = string.IsNullOrWhiteSpace(usuarioDto.Nome) ? "" : usuarioDto.Nome.Trim();
                usuarioVo.Senha = string.IsNullOrWhiteSpace(usuarioDto.Senha) ? "" : usuarioDto.Senha.Trim();
                usuarioVo.Id = usuarioDto.Id;
                usuarioVo.Administrador = usuarioDto.Administrador;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o usuario para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um usuario Dto para um usuario Vo
        /// </summary>
        /// <param name="usuarioVo"></param>
        /// <param name="usuarioDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(UsuarioVo usuarioVo, ref UsuarioDto usuarioDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(usuarioVo, ref usuarioDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                usuarioDto.Email = string.IsNullOrWhiteSpace(usuarioVo.Email) ? "" : usuarioVo.Email.Trim();
                usuarioDto.Nome = string.IsNullOrWhiteSpace(usuarioVo.Nome) ? "" : usuarioVo.Nome.Trim();
                usuarioDto.Senha = string.IsNullOrWhiteSpace(usuarioVo.Senha) ? "" : usuarioVo.Senha.Trim();
                usuarioDto.Administrador = usuarioVo.Administrador;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o usuario para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtém uma lista de usuários com filtros aplicados, podendo ser paginada (não traz a senha nos objetos)
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<UsuarioDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para consultar os usuários é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            // Obter a query primária
            IQueryable<UsuarioVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar os usuários: {mensagemErro}";
                retornoDto.Retorno = false;

                return false;
            }

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "EMAIL":
                        query = query.Where(p => p.Email.Contains(filtro.Value));
                        break;

                    case "NOME":
                        query = query.Where(p => p.Nome.Contains(filtro.Value));
                        break;

                    case "ADMINISTRADOR":

                        bool filtroAdministrador;
                        if (!bool.TryParse(filtro.Value, out filtroAdministrador))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'administrador'.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query = query.Where(p => p.Administrador == filtroAdministrador);
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

                case "EMAIL":
                    query = query.OrderBy(p => p.Email);
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

            double paginas = totalItens <= requisicaoDto.NumeroItensPorPagina ? 1 : totalItens / requisicaoDto.NumeroItensPorPagina;
            retornoDto.NumeroPaginas = (int)Math.Ceiling(paginas);

            int pular = (requisicaoDto.Pagina - 1) * requisicaoDto.NumeroItensPorPagina;
            query = query.Skip(pular).Take(requisicaoDto.NumeroItensPorPagina);

            List<UsuarioVo> listaVo = query.ToList();
            foreach (var usuario in listaVo)
            {
                UsuarioDto usuarioDto = new UsuarioDto();
                if (!ConverterVoParaDto(usuario, ref usuarioDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;
                    return false;
                }

                usuarioDto.Senha = "";
                retornoDto.ListaEntidades.Add(usuarioDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Edita um usuario
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Editar(RequisicaoEntidadeDto<UsuarioDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Editar(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Apenas usuários ADM ou o próprio usuário pode editar
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro) &&
                requisicaoDto.IdUsuario != requisicaoDto.EntidadeDto.Id)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para editar os usuários é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            // Não deixar incluir um email repetido
            UsuarioVo usuarioVo = new UsuarioVo();
            if (!VerificarUsuarioExistente(requisicaoDto.EntidadeDto, ref usuarioVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao validar o Email: " + mensagemErro;
                return false;
            }

            // Se existir
            if (usuarioVo != null)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Esse cadastro (usuário) já existe, não é possível incluir cadastros duplicados";
                return false;
            }

            if (!ObterPorIdBd(requisicaoDto.EntidadeDto.Id, out usuarioVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas para encontrar o usuario: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref usuarioVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas ao converter o usuario para Vo: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            if (!EditarBd(usuarioVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao editar os novos dados do usuario: " + mensagemErro;
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
        /// Valida se o Email já existe
        /// </summary>
        /// <param name="usuarioDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool VerificarUsuarioExistente(UsuarioDto usuarioDto, ref UsuarioVo usuarioVo, ref string mensagemErro)
        {
            if (usuarioDto == null)
            {
                mensagemErro = "É necessário informar o usuario para validar o Email";
                return false;
            }

            if (string.IsNullOrWhiteSpace(usuarioDto.Email))
            {
                return true;
            }

            IQueryable<UsuarioVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                mensagemErro = $"Houve um problema ao listar os usuários: {mensagemErro}";
                return false;
            }

            usuarioDto.Email = usuarioDto.Email;
            query = query.Where(p => p.Email == usuarioDto.Email.Trim() && p.Id != usuarioDto.Id);
            usuarioVo = query.FirstOrDefault();
            return true;

        }

        /// <summary>
        /// Obtem os dados para montar o dashboard da tela inicial
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterDadosDashboard(BaseRequisicaoDto requisicaoDto, ref RetornoObterInformacoesDashboardDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            // Obtem as assinaturas dos ultimos 30 dias
            string queryNovos = "SELECT" +
                " IFNULL(COUNT(Id), 0) AS QuantidadeNovasAssinaturas," +
                " 0 AS QuantidadeAssinaturasCanceladas," +
                " 0 AS QuantidadeAssinaturasPagamentoPendente," +
                " 0 AS QuantidadeAssinaturasAtivas," +
                " 0 AS QuantidadeResgates," +
                " 0.0 AS PercentualAssinaturas15," +
                " 0.0 AS PercentualAssinaturas30," +
                " 0.0 AS PercentualAssinaturas50," +
                " 0.0 AS PercentualAssinaturas100," +
                " 0.0 AS PercentualAssinaturas200," +
                " 0 AS QuantidadeLojas," +
                " 0 AS QuantidadeTotalCancelamento" +
               $" FROM {aaanoContexto.Database.Connection.Database}.AssinaturasPagSeguro" +
                " WHERE CAST(Criacao AS DATE) >= '" + DateTime.Now.AddDays(-30).Date.ToString("yyyy-MM-dd") + "'" +
                " AND CAST(Criacao AS DATE) <=  '" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "'";

            // Cancelamentos dos ultimos 30 dias
            string queryCanceladas = "SELECT" +
                " 0 AS QuantidadeNovasAssinaturas," +
                " IFNULL(COUNT(Id), 0) AS QuantidadeAssinaturasCanceladas," +
                " 0 AS QuantidadeAssinaturasPagamentoPendente," +
                " 0 AS QuantidadeAssinaturasAtivas," +
                " 0 AS QuantidadeResgates," +
                " 0.0 AS PercentualAssinaturas15," +
                " 0.0 AS PercentualAssinaturas30," +
                " 0.0 AS PercentualAssinaturas50," +
                " 0.0 AS PercentualAssinaturas100," +
                " 0.0 AS PercentualAssinaturas200," +
                " 0 AS QuantidadeLojas," +
                " 0 AS QuantidadeTotalCancelamento" +
               $" FROM {aaanoContexto.Database.Connection.Database}.AssinaturasPagSeguro" +
                " WHERE CAST(UltimoEnventoRegistrado AS DATE) >= '" + DateTime.Now.AddDays(-30).Date.ToString("yyyy-MM-dd") + "'" +
                " AND CAST(UltimoEnventoRegistrado AS DATE) <= '" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "'" +
                " AND (Status = " + (int)StatusAssinatura.Cancelada_PagSeguro +
                " OR Status = " + (int)StatusAssinatura.Cancelado_comprador +
                " OR Status = " + (int)StatusAssinatura.Cancelado_vendedor + ")";

            // Pagamentos pendentes
            string queryPagamentoPendente = "SELECT" +
                " 0 AS QuantidadeNovasAssinaturas," +
                " 0 AS QuantidadeAssinaturasCanceladas," +
                " IFNULL(COUNT(Id), 0) AS QuantidadeAssinaturasPagamentoPendente," +
                " 0 AS QuantidadeAssinaturasAtivas," +
                " 0 AS QuantidadeResgates," +
                " 0.0 AS PercentualAssinaturas15," +
                " 0.0 AS PercentualAssinaturas30," +
                " 0.0 AS PercentualAssinaturas50," +
                " 0.0 AS PercentualAssinaturas100," +
                " 0.0 AS PercentualAssinaturas200," +
                " 0 AS QuantidadeLojas," +
                " 0 AS QuantidadeTotalCancelamento" +
               $" FROM {aaanoContexto.Database.Connection.Database}.AssinaturasPagSeguro" +
                " WHERE Status = " + (int)StatusAssinatura.Pagamento_pendente +
                " OR Status = " + (int)StatusAssinatura.Pendente;

            string queryTotalCancelamento = "SELECT" +
                " 0 AS QuantidadeNovasAssinaturas," +
                " 0 AS QuantidadeAssinaturasCanceladas," +
                " 0 AS QuantidadeAssinaturasPagamentoPendente," +
                " 0 AS QuantidadeAssinaturasAtivas," +
                " 0 AS QuantidadeResgates," +
                " 0.0 AS PercentualAssinaturas15," +
                " 0.0 AS PercentualAssinaturas30," +
                " 0.0 AS PercentualAssinaturas50," +
                " 0.0 AS PercentualAssinaturas100," +
                " 0.0 AS PercentualAssinaturas200," +
                " 0 AS QuantidadeLojas," +
                " COUNT(Id) AS QuantidadeTotalCancelamento" +
               $" FROM {aaanoContexto.Database.Connection.Database}.AssinaturasPagSeguro" +
                " WHERE Status = " + (int)StatusAssinatura.Cancelada_PagSeguro +
                " OR Status = " + (int)StatusAssinatura.Cancelado_comprador +
                " OR Status = " + (int)StatusAssinatura.Cancelado_vendedor;

            string queryTotalAssinaturasAtivas = "SELECT" +
                " 0 AS QuantidadeNovasAssinaturas," +
                " 0 AS QuantidadeAssinaturasCanceladas," +
                " 0 AS QuantidadeAssinaturasPagamentoPendente," +
                " COUNT(Id) AS QuantidadeAssinaturasAtivas," +
                " 0 AS QuantidadeResgates," +
                " 0.0 AS PercentualAssinaturas15," +
                " 0.0 AS PercentualAssinaturas30," +
                " 0.0 AS PercentualAssinaturas50," +
                " 0.0 AS PercentualAssinaturas100," +
                " 0.0 AS PercentualAssinaturas200," +
                " 0 AS QuantidadeLojas," +
                " 0 AS QuantidadeTotalCancelamento" +
               $" FROM {aaanoContexto.Database.Connection.Database}.AssinaturasPagSeguro" +
                " WHERE Status = " + (int)StatusAssinatura.Ativo;

            // Pagamentos pendentes
            string queryResgates = "SELECT" +
                " 0 AS QuantidadeNovasAssinaturas," +
                " 0 AS QuantidadeAssinaturasCanceladas," +
                " 0 AS QuantidadeAssinaturasPagamentoPendente," +
                " 0 AS QuantidadeAssinaturasAtivas," +
                " IFNULL(COUNT(Id), 0) AS QuantidadeResgates," +
                " 0.0 AS PercentualAssinaturas15," +
                " 0.0 AS PercentualAssinaturas30," +
                " 0.0 AS PercentualAssinaturas50," +
                " 0.0 AS PercentualAssinaturas100," +
                " 0.0 AS PercentualAssinaturas200," +
                " 0 AS QuantidadeLojas," +
                " 0 AS QuantidadeTotalCancelamento" +
               $" FROM {aaanoContexto.Database.Connection.Database}.ResgatesPromocoes" +
               $" WHERE CAST(Resgate AS DATE) >= '{ DateTime.Now.AddDays(-30).Date.ToString("yyyy-MM-dd") }'" +
               $" AND CAST(Resgate AS DATE) <= '{ DateTime.Now.Date.ToString("yyyy-MM-dd") }'";

            string queryPercentualPlanos = "SELECT" +
                " 0 AS QuantidadeNovasAssinaturas," +
                " 0 AS QuantidadeAssinaturasCanceladas," +
                " 0 AS QuantidadeAssinaturasPagamentoPendente," +
                " 0 AS QuantidadeAssinaturasAtivas," +
                " 0 AS QuantidadeResgates," +
                " CASE WHEN COUNT(Id) = 0 THEN 0 ELSE (CAST(SUM(CASE WHEN ReferenciaPlano = 'AAANO15' THEN 1 ELSE 0 END) AS decimal) * 100)/ COUNT(ID) END AS PercentualAssinaturas15," +
                " CASE WHEN COUNT(Id) = 0 THEN 0 ELSE (CAST(SUM(CASE WHEN ReferenciaPlano = 'AAANO30' THEN 1 ELSE 0 END) AS decimal) * 100)/ COUNT(ID) END AS PercentualAssinaturas30," +
                " CASE WHEN COUNT(Id) = 0 THEN 0 ELSE (CAST(SUM(CASE WHEN ReferenciaPlano = 'AAANO50' THEN 1 ELSE 0 END) AS decimal) * 100)/ COUNT(ID) END AS PercentualAssinaturas50," +
                " CASE WHEN COUNT(Id) = 0 THEN 0 ELSE (CAST(SUM(CASE WHEN ReferenciaPlano = 'AAANO100' THEN 1 ELSE 0 END) AS decimal) * 100)/ COUNT(ID) END AS PercentualAssinaturas100," +
                " CASE WHEN COUNT(Id) = 0 THEN 0 ELSE (CAST(SUM(CASE WHEN ReferenciaPlano = 'AAANO200' THEN 1 ELSE 0 END) AS decimal) * 100)/ COUNT(ID) END AS PercentualAssinaturas200," +
                " 0 AS QuantidadeLojas," +
                " 0 AS QuantidadeTotalCancelamento" +
               $" FROM  {aaanoContexto.Database.Connection.Database}.AssinaturasPagSeguro" +
                " WHERE Status = " + (int)StatusAssinatura.Ativo;

            string queryQuantidadeLojas = $"SELECT" +
                " 0 AS QuantidadeNovasAssinaturas," +
                " 0 AS QuantidadeAssinaturasCanceladas," +
                " 0 AS QuantidadeAssinaturasPagamentoPendente," +
                " 0 AS QuantidadeAssinaturasAtivas," +
                " 0 AS QuantidadeResgates," +
                " 0.0 AS PercentualAssinaturas15," +
                " 0.0 AS PercentualAssinaturas30," +
                " 0.0 AS PercentualAssinaturas50," +
                " 0.0 AS PercentualAssinaturas100," +
                " 0.0 AS PercentualAssinaturas200," +
                $" COUNT(Id) AS QuantidadeLojas," +
                " 0 AS QuantidadeTotalCancelamento" +
                $" FROM {aaanoContexto.Database.Connection.Database}.LojasParceiras";

            string queryAssinaturaCompleta = "SELECT" +
                " SUM(r.QuantidadeNovasAssinaturas) AS QuantidadeNovasAssinaturas," +
                " SUM(r.QuantidadeAssinaturasCanceladas) AS QuantidadeAssinaturasCanceladas," +
                " SUM(r.QuantidadeAssinaturasPagamentoPendente) AS QuantidadeAssinaturasPagamentoPendente," +
                " SUM(r.QuantidadeAssinaturasAtivas) AS QuantidadeAssinaturasAtivas," +
                " SUM(r.QuantidadeResgates) AS QuantidadeResgates," +
                " SUM(r.PercentualAssinaturas15) AS PercentualAssinaturas15," +
                " SUM(r.PercentualAssinaturas30) AS PercentualAssinaturas30," +
                " SUM(r.PercentualAssinaturas50) AS PercentualAssinaturas50," +
                " SUM(r.PercentualAssinaturas100) AS PercentualAssinaturas100," +
                " SUM(r.PercentualAssinaturas200) AS PercentualAssinaturas200," +
                " SUM(r.QuantidadeLojas) AS QuantidadeLojas," +
                " SUM(r.QuantidadeTotalCancelamento) AS QuantidadeTotalCancelamento" +
               $" FROM ({queryNovos} UNION ALL {queryCanceladas} UNION ALL {queryPagamentoPendente} UNION ALL {queryQuantidadeLojas}" +
               $" UNION ALL {queryTotalAssinaturasAtivas} UNION ALL {queryTotalCancelamento} UNION ALL {queryResgates} UNION ALL {queryPercentualPlanos}) AS r";

            string queryAssinaturasMes = "SELECT IFNULL(COUNT(Id), 0) AS Quantidade," +
               " CASE WHEN MONTH(Criacao) = 1 THEN 'Jan'" +
               " WHEN MONTH(Criacao) = 2 THEN 'Fev'" +
               " WHEN MONTH(Criacao) = 4 THEN 'Abr'" +
               " WHEN MONTH(Criacao) = 5 THEN 'Mai'" +
               " WHEN MONTH(Criacao) = 6 THEN 'Jun'" +
               " WHEN MONTH(Criacao) = 7 THEN 'Jul'" +
               " WHEN MONTH(Criacao) = 8 THEN 'Ago'" +
               " WHEN MONTH(Criacao) = 9 THEN 'Set'" +
               " WHEN MONTH(Criacao) = 10 THEN 'Out'" +
               " WHEN MONTH(Criacao) = 11 THEN 'Nov'" +
               " ELSE 'Dez' END AS Mes" +
              $" FROM {aaanoContexto.Database.Connection.Database}.AssinaturasPagSeguro" +
               " WHERE YEAR(Criacao) = " + DateTime.Now.Year + " GROUP BY MONTH(Criacao)";

            string queryResgatesMes = "SELECT IFNULL(COUNT(Id), 0) AS Quantidade," +
               " CASE WHEN MONTH(Resgate) = 1 THEN 'Jan'" +
               " WHEN MONTH(Resgate) = 2 THEN 'Fev'" +
               " WHEN MONTH(Resgate) = 4 THEN 'Abr'" +
               " WHEN MONTH(Resgate) = 5 THEN 'Mai'" +
               " WHEN MONTH(Resgate) = 6 THEN 'Jun'" +
               " WHEN MONTH(Resgate) = 7 THEN 'Jul'" +
               " WHEN MONTH(Resgate) = 8 THEN 'Ago'" +
               " WHEN MONTH(Resgate) = 9 THEN 'Set'" +
               " WHEN MONTH(Resgate) = 10 THEN 'Out'" +
               " WHEN MONTH(Resgate) = 11 THEN 'Nov'" +
               " ELSE 'Dez' END AS Mes" +
              $" FROM {aaanoContexto.Database.Connection.Database}.ResgatesPromocoes" +
               " WHERE YEAR(Resgate) = " + DateTime.Now.Year + " GROUP BY MONTH(Resgate)";

            try
            {
                using (AaanoContexto aaanoContexto = new AaanoContexto())
                {
                    retornoDto = aaanoContexto.Database.SqlQuery<RetornoObterInformacoesDashboardDto>(queryAssinaturaCompleta).FirstOrDefault();

                    retornoDto.PercentualAssinaturas15 = Math.Round(retornoDto.PercentualAssinaturas15, 2);
                    retornoDto.PercentualAssinaturas30 = Math.Round(retornoDto.PercentualAssinaturas30, 2);
                    retornoDto.PercentualAssinaturas50 = Math.Round(retornoDto.PercentualAssinaturas50, 2);
                    retornoDto.PercentualAssinaturas100 = Math.Round(retornoDto.PercentualAssinaturas100, 2);
                    retornoDto.PercentualAssinaturas200 = Math.Round(retornoDto.PercentualAssinaturas200, 2);

                    // Obter as assinaturas por mês
                    var assinaturasMes = aaanoContexto.Database.SqlQuery<InformacaoMensalDto>(queryAssinaturasMes).ToList();
                    for (int i = 0; i < 12; i++)
                    {
                        InformacaoMensalDto mes = assinaturasMes.Where(p => p.Mes.Trim() == UtilitarioBll.RetornaMes(i)).FirstOrDefault();
                        if (mes == null)
                        {
                            retornoDto.ListaAssinaturasPorMes.Add(new InformacaoMensalDto()
                            {
                                Mes = UtilitarioBll.RetornaMes(i),
                                Quantidade = 0
                            });
                        }
                        else
                        {
                            retornoDto.ListaAssinaturasPorMes.Add(mes);
                        }
                    }

                    // Obter os resgates por mês
                    var resgatesMes = aaanoContexto.Database.SqlQuery<InformacaoMensalDto>(queryResgatesMes).ToList();
                    for (int i = 0; i < 12; i++)
                    {
                        InformacaoMensalDto mes = resgatesMes.Where(p => p.Mes.Trim() == UtilitarioBll.RetornaMes(i)).FirstOrDefault();
                        if (mes == null)
                        {
                            retornoDto.ListaResgatesPorMes.Add(new InformacaoMensalDto()
                            {
                                Mes = UtilitarioBll.RetornaMes(i),
                                Quantidade = 0
                            });
                        }
                        else
                        {
                            retornoDto.ListaResgatesPorMes.Add(mes);
                        }
                    }
                }

                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Erro ao obter as informações: " + ex.Message;
                return false;
            }
        }

    }
}
