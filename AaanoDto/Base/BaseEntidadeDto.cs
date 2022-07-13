using System;

namespace AaanoDto.Base
{
    public abstract class BaseEntidadeDto
    {
        /// <summary>
        /// Id que identifica unicamente a entidade
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Data que a entidade foi incluída no banco de dados
        /// </summary>
        public DateTime DataInclusao { get; set; }

        /// <summary>
        /// Data da última alteração da entidade
        /// </summary>
        public DateTime? DataAlteracao { get; set; }

        #region Métodos

        /// <summary>
        /// Valida se os dados da entidade estão consistentes
        /// </summary>
        /// <returns></returns>
        public virtual bool ValidarEntidade(ref string mensagemErro)
        {
            bool retorno = true;

            if (Id == null || Id == Guid.Empty)
            {
                mensagemErro = "O ID da entidade é obrigatório!";
                retorno = false;
            }

            return retorno;
        }

        #endregion
    }
}
