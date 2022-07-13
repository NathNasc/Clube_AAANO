using AaanoDal;
using AaanoDal.Base;
using AaanoDto.Base;
using AaanoDto.Requisicoes;
using AaanoDto.Retornos;
using AaanoVo;
using System;

namespace AaanoBll.Base
{
    public abstract class BaseBll<T, Y> : Repositorio<T>, IBaseBll<T, Y> where T : EntidadeBaseVo where Y : BaseEntidadeDto
    {
        /// <summary>
        /// Construtor que iniciar o contexto
        /// </summary>
        public BaseBll()
        {
            aaanoContexto = new AaanoContexto();
        }

        /// <summary>
        /// Construtor que iniciar o contexto
        /// </summary>
        /// <param name="contextoExistente"></param>
        public BaseBll(AaanoContexto contextoExistente)
        {
            aaanoContexto = contextoExistente;
        }

        /// <summary>
        /// Inclui uma entidade no banco de dados
        /// </summary>
        /// <param name="BaseRequisicaoDto"></param>
        /// <param name="retornoDto"></param>
        public virtual bool Incluir(RequisicaoEntidadeDto<Y> requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            if (requisicaoDto.EntidadeDto == null)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Não é possível incluir uma entidade nula.";
                return false;
            }

            if (!requisicaoDto.EntidadeDto.ValidarEntidade(ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Edita uma entidade existente no banco de dados
        /// </summary>
        /// <param name="BaseRequisicaoDto"></param>
        /// <param name="retornoDto"></param>
        public virtual bool Editar(RequisicaoEntidadeDto<Y> requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                Guid idEntidade = (requisicaoDto.EntidadeDto == null) ? Guid.Empty : requisicaoDto.EntidadeDto.Id;
                return false;
            }

            if (requisicaoDto.EntidadeDto == null)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Não é possível editar uma entidade nula.";
                return false;
            }

            if (!requisicaoDto.EntidadeDto.ValidarEntidade(ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Obtém uma entidade do banco de dados
        /// </summary>
        /// <param name="BaseRequisicaoDto"></param>
        /// <param name="retornoDto"></param>
        public virtual bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<Y> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            if (requisicaoDto.Id == Guid.Empty)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "O id é obrigatório para obter uma entidade.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Obtém uma lista de entidades do banco de dados
        /// </summary>
        /// <param name="BaseRequisicaoDto"></param>
        /// <param name="retornoDto"></param>
        public virtual bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<Y> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            // Caso não mande uma página válida, usar a página 1
            if (requisicaoDto.Pagina <= 0)
            {
                requisicaoDto.Pagina = 1;
            }

            // Valida o número de itens por página
            if (requisicaoDto.NumeroItensPorPagina <= 0)
            {
                requisicaoDto.NumeroItensPorPagina = 20;
            }

            return true;
        }

        /// <summary>
        /// Exclui uma entidade do banco de dados
        /// </summary>
        /// <param name="BaseRequisicaoDto"></param>
        /// <param name="retornoDto"></param>
        public virtual bool Excluir(RequisicaoObterDto requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            // Exclui do banco de dados
            if (!ExcluirBd(requisicaoDto.Id, ref mensagemErro))
            {
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Converte uma entidade Dto para uma entidade VO
        /// </summary>
        /// <param name="entidadeDto"></param>
        /// <param name="entidadeVo"></param>
        /// <returns></returns>
        public virtual bool ConverterDtoParaVo(Y entidadeDto, ref T entidadeVo, ref string mensagemErro)
        {
            try
            {
                entidadeVo.Id = entidadeDto.Id;

                return true;
            }
            catch (Exception ex)
            {
                // Logado pela BLL de cada entidade
                mensagemErro = "Falha ao converter de DTO para VO: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte uma entidade VO para uma entidade Dto
        /// </summary>
        /// <param name="entidadeVo"></param>
        /// <param name="entidadeDto"></param>
        /// <returns></returns>
        public virtual bool ConverterVoParaDto(T entidadeVo, ref Y entidadeDto, ref string mensagemErro)
        {
            try
            {
                entidadeDto.Id = entidadeVo.Id;
                entidadeDto.DataAlteracao = entidadeVo.DataAlteracao;
                entidadeDto.DataInclusao = entidadeVo.DataInclusao;

                return true;
            }
            catch (Exception ex)
            {
                // Logado pela BLL de cada entidade
                mensagemErro = "Falha ao converter de VO para DTO: " + ex.Message;
                return false;
            }
        }
    }
}
