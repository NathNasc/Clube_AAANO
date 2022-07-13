using AaanoVo;
using System;
using System.Data.Entity;
using System.Linq;

namespace AaanoDal.Base
{
    /// <summary>
    /// Classe que implementa os métodos básicos de um repositório
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Repositorio<T> : IDisposable where T : EntidadeBaseVo
    {
        /// <summary>
        /// Contexto utilizado nas transições de banco de dados
        /// </summary>
        public AaanoContexto aaanoContexto;

        /// <summary>
        /// Edita uma entidade no banco de dados
        /// </summary>
        /// <param name="entidade">Entidade a ser editada</param>
        /// <param name="mensagemErro">Mensagem de erro, caso ocorra</param>
        /// <returns></returns>
        protected bool EditarBd(T entidade, ref string mensagemErro)
        {
            try
            {
                // Atualizar a data de atualização do registro
                entidade.DataAlteracao = DateTime.Now;

                // Tentar alterar o registro no banco de dados
                aaanoContexto.Entry(entidade).State = EntityState.Modified;
                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao editar a entidade: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Exclui uma entidade do banco de dados
        /// </summary>
        /// <param name="id">Id da entidade a ser excluída</param>
        /// <param name="mensagemErro">Mensagem de erro, caso ocorra</param>
        protected bool ExcluirBd(Guid id, ref string mensagemErro)
        {
            // Validar o ID para excluir a entidade
            if (id == null || id == Guid.Empty)
            {
                mensagemErro = "Para excluir um registro do banco de dados é necessário informar o ID da entidade!";
                return false;
            }

            try
            {
                // Encontrar a entidade por ID e deletar do banco de dados
                T entidade = aaanoContexto.Set<T>().Where(p => p.Id == id).FirstOrDefault();

                if (entidade == null)
                {
                    mensagemErro = "Não foi encontrado nenhum registro com o ID informado. Atualize a página e tente novamente.";
                    return false;
                }
                else
                {
                    // Apagar a entidade
                    aaanoContexto.Entry(entidade).State = EntityState.Deleted;
                    return true;
                }
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao excluir a entidade: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Inclui uma entidade no banco de dados
        /// </summary>
        /// <param name="entidade">Entidade a ser incluída</param>
        /// <param name="mensagemErro">Mensagem de erro, caso ocorra</param>
        /// <returns></returns>
        protected bool IncluirBd(T entidade, ref string mensagemErro)
        {
            try
            {
                // Preencher a data de inclusão e zerar a data de alteração
                entidade.DataInclusao = DateTime.Now;
                entidade.DataAlteracao = null;

                if(entidade.Id == Guid.Empty)
                {
                    entidade.Id = Guid.NewGuid();
                }

                aaanoContexto.Set<T>().Add(entidade);
                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao incluir a entidade: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Prepara uma query da entidade para ser filtrada
        /// </summary>
        /// <param name="query">Query a ser preenchida com uma conexão da entidade</param>
        /// <param name="mensagemErro">Mensagem de erro, caso ocorra</param>
        /// <returns></returns>
        public bool ObterQueryBd(out IQueryable<T> query, ref string mensagemErro)
        {
            try
            {
                query = aaanoContexto.Set<T>();
                return true;
            }
            catch (Exception ex)
            {
                query = null;
                mensagemErro = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtém uma entidade pelo ID
        /// </summary>
        /// <param name="id">Id a ser pesquisado</param>
        /// <param name="entidade">Entidade encontrada</param>
        /// <param name="mensagemErro">Mensagem de erro, caso ocorra</param>
        /// <returns></returns>
        protected bool ObterPorIdBd(Guid id, out T entidade, ref string mensagemErro)
        {
            // Inicializar a entidade
            entidade = null;

            // Validar o ID para obter a entidade
            if (id == null || id == Guid.Empty)
            {
                mensagemErro = "Para obter um registro do banco de dados é necessário informar o ID da entidade!";
                return false;
            }

            try
            {
                // Obter a entidade do banco de dados
                entidade = aaanoContexto.Set<T>().Where(p => p.Id == id).FirstOrDefault();

                if (entidade == null)
                {
                    mensagemErro = "Cadastro não encontrado";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao obter a entidade: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Fecha a conexão do contexto criado
        /// </summary>
        public void Dispose()
        {
            if (aaanoContexto != null)
            {
                aaanoContexto.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}

