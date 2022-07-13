using AaanoDto.Base;
using AaanoDto.Requisicoes;
using AaanoDto.Retornos;
using AaanoVo;

namespace AaanoBll.Base
{
    /// <summary>
    /// Indica o contrato que toda BLL deve seguir para criar o repositório de uma entidade
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseBll<T, Y> where T : EntidadeBaseVo where Y : BaseEntidadeDto
    {
        /// <summary>
        /// Método que retorna uma query para filtrar uma lista de entidades
        /// </summary>
        /// <param name="query">Query a ser preenchida com uma conexão da entidade</param>
        /// <param name="mensagemErro">Mensagem de erro, caso ocorra</param>
        /// <returns></returns>
        bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<Y> retornoDto);

        /// <summary>
        /// Obtém uma entidade pelo ID
        /// </summary>
        /// <param name="id">Id a ser pesquisado</param>
        /// <param name="entidade">Entidade encontrada</param>
        /// <param name="mensagemErro">Mensagem de erro, caso ocorra</param>
        /// <returns></returns>
        bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<Y> retornoDto);

        /// <summary>
        /// Inclui uma entidade no banco de dados
        /// </summary>
        /// <param name="entidade">Entidade a ser incluída</param>
        /// <param name="mensagemErro">Mensagem de erro, caso ocorra</param>
        /// <returns></returns>
        bool Incluir(RequisicaoEntidadeDto<Y> requisicaoDto, ref RetornoDto retornoDto);

        /// <summary>
        /// Edita uma entidade existente no banco de dados
        /// </summary>
        /// <param name="entidade">Entidade a ser editada</param>
        /// <param name="mensagemErro">Mensagem de erro, caso ocorra</param>
        bool Editar(RequisicaoEntidadeDto<Y> requisicaoDto, ref RetornoDto retornoDto);

        /// <summary>
        /// Exclui uma entidade do banco de dados
        /// </summary>
        /// <param name="id">Id da entidade a ser excluída</param>
        /// <param name="mensagemErro">Mensagem de erro, caso ocorra</param>
        bool Excluir(RequisicaoObterDto requisicaoDto, ref RetornoDto retornoDto);

        /// <summary>
        /// Converte uma entidade Dto para uma entidade Vo
        /// </summary>
        /// <param name="entidadeDto"></param>
        /// <param name="entidadeVo"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        bool ConverterDtoParaVo(Y entidadeDto, ref T entidadeVo, ref string mensagemErro);

        /// <summary>
        /// Converte uma entidade Vo para uma entidade Dto
        /// </summary>
        /// <param name="entidadeVo"></param>
        /// <param name="entidadeDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        bool ConverterVoParaDto(T entidadeVo, ref Y entidadeDto, ref string mensagemErro);

    }
}
