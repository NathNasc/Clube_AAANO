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
    public class PermissaoUsuarioBll : BaseBll<PermissaoUsuarioVo, PermissaoUsuarioDto>
    {
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public PermissaoUsuarioBll(bool salvarAlteracoes) : base()
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public PermissaoUsuarioBll(AaanoContexto contexto, bool salvarAlteracoes) : base(contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui uma permissão no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool IncluirEditarListaPermissoesPorUsuario(RequisicaoListaEntidadesDto<PermissaoUsuarioDto> requisicaoDto, ref RetornoDto retornoDto)
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
                retornoDto.Mensagem = "Este usuário não é administrador. Para cadastrar permissões é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                return false;
            }

            // Obter a query primária
            IQueryable<PermissaoUsuarioVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as permissões: {mensagemErro}";
                retornoDto.Retorno = false;

                return false;
            }

            // Percorrer as permissões adicionadas
            List<PermissaoUsuarioVo> listaExclusao = query.Where(p => p.IdUsuario == requisicaoDto.IdComum).ToList();
            foreach (var permissao in requisicaoDto.ListaEntidadesDto)
            {
                permissao.IdUsuario = requisicaoDto.IdComum;
                PermissaoUsuarioVo permissaoUsuarioVo = listaExclusao.Where(p => p.IdLojaParceira == permissao.IdLojaParceira).FirstOrDefault();
                if (permissaoUsuarioVo == null)
                {
                    // Converte para VO a ser incluída no banco de dados
                    permissaoUsuarioVo = new PermissaoUsuarioVo();
                    if (!ConverterDtoParaVo(permissao, ref permissaoUsuarioVo, ref mensagemErro))
                    {
                        retornoDto.Retorno = false;
                        retornoDto.Mensagem = "Falha ao converter a permissão para VO: " + mensagemErro;

                        return false;
                    }

                    // Prepara a inclusão no banco de dados
                    if (!IncluirBd(permissaoUsuarioVo, ref mensagemErro))
                    {
                        retornoDto.Retorno = false;
                        retornoDto.Mensagem = "Falha ao incluir a permissão: " + mensagemErro;
                        return false;
                    }
                }
                else
                {
                    // Se houver a permissão, remover da lista de exclusão
                    listaExclusao.Remove(permissaoUsuarioVo);
                }
            }

            foreach (var permissao in listaExclusao)
            {
                if (!ExcluirBd(permissao.Id, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao excluir a permissão: " + mensagemErro;
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
        /// Prepara a exclusão das permissões
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        internal bool ExcluirPermissoesPorIdUsuario(Guid idUsuario, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (idUsuario == Guid.Empty)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Informe o id do usuário para excluir suas permissões.";
                return false;
            }

            // Obter a query primária
            IQueryable<PermissaoUsuarioVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as permissões: {mensagemErro}";
                retornoDto.Retorno = false;

                return false;
            }

            List<PermissaoUsuarioVo> listaPermissoes = query.Where(p => p.IdUsuario == idUsuario).ToList();
            foreach (var permissao in listaPermissoes)
            {
                if (!ExcluirBd(permissao.Id, ref mensagemErro))
                {
                    retornoDto.Mensagem = $"Houve um problema ao excluir as permissões: {mensagemErro}";
                    retornoDto.Retorno = false;

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Obtem uma lista de permissões de um usuário
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterPermissoesUsuario(RequisicaoObterDto requisicaoDto, ref RetornoObterListaDto<PermissaoUsuarioDto> retornoDto)
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
                retornoDto.Mensagem = $"Para obter as permissões do usuário informe o id.";
                retornoDto.Retorno = false;

                return false;
            }

            // Obter a query primária
            IQueryable<PermissaoUsuarioVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as permissões: {mensagemErro}";
                retornoDto.Retorno = false;

                return false;
            }

            LojaParceiraBll lojaParceiraBll = new LojaParceiraBll(false);
            List<PermissaoUsuarioVo> listaVo = query.Where(p => p.IdUsuario == requisicaoDto.Id).ToList();
            foreach (var permissao in listaVo)
            {
                LojaParceiraVo lojaVo;
                if (!lojaParceiraBll.ObterLojaVo(permissao.IdLojaParceira, out lojaVo, ref mensagemErro))
                {
                    lojaVo = new LojaParceiraVo()
                    {
                        Nome = "Loja não localizada"
                    };
                }

                retornoDto.ListaEntidades.Add(new PermissaoUsuarioDto()
                {
                    DataAlteracao = permissao.DataAlteracao,
                    DataInclusao = permissao.DataInclusao,
                    Id = permissao.Id,
                    IdLojaParceira = permissao.IdLojaParceira,
                    IdUsuario = permissao.IdUsuario,
                    NomeLoja = lojaVo.Nome.Trim()
                });
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Método não implementado
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<PermissaoUsuarioDto> requisicaoDto, ref RetornoDto retornoDto)
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
        public override bool Editar(RequisicaoEntidadeDto<PermissaoUsuarioDto> requisicaoDto, ref RetornoDto retornoDto)
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
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<PermissaoUsuarioDto> retornoDto)
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
        /// Método não implementado
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<PermissaoUsuarioDto> retornoDto)
        {
            retornoDto.Mensagem = "Método não implementado";
            retornoDto.Retorno = false;
            return false;
        }

        /// <summary>
        /// Converte uma permissão Dto para uma permissão Vo
        /// </summary>
        /// <param name="permissaoUsuarioDto"></param>
        /// <param name="permissaoUsuarioVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(PermissaoUsuarioDto permissaoUsuarioDto, ref PermissaoUsuarioVo permissaoUsuarioVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(permissaoUsuarioDto, ref permissaoUsuarioVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                permissaoUsuarioVo.Id = permissaoUsuarioDto.Id;
                permissaoUsuarioVo.IdLojaParceira = permissaoUsuarioDto.IdLojaParceira;
                permissaoUsuarioVo.IdUsuario = permissaoUsuarioDto.IdUsuario;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a permissão para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte uma permissão Dto para uma permissão Vo
        /// </summary>
        /// <param name="permissaoUsuarioVo"></param>
        /// <param name="permissaoUsuarioDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(PermissaoUsuarioVo permissaoUsuarioVo, ref PermissaoUsuarioDto permissaoUsuarioDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(permissaoUsuarioVo, ref permissaoUsuarioDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                permissaoUsuarioDto.IdLojaParceira = permissaoUsuarioVo.IdLojaParceira;
                permissaoUsuarioDto.IdUsuario = permissaoUsuarioVo.IdUsuario;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a permissão para Vo: " + ex.Message;
                return false;
            }
        }

    }
}
