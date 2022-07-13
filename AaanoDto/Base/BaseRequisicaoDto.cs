using System;
using System.Collections.Generic;

namespace AaanoDto.Base
{
    /// <summary>
    /// Base de todas as requisição a API
    /// </summary>
    public class BaseRequisicaoDto
    {
        // ------------------> Atributos

        /// <summary>
        /// Autentica as requisições
        /// </summary>
        public string Identificacao = "";

        /// <summary>
        /// Id do usuário que fez a requisição
        /// </summary>
        public Guid IdUsuario = Guid.Empty;

        /// <summary>
        /// Lista das loja que o usuário pode acessar
        /// </summary>
        public List<Guid> LojasPermitidas{ get; set; }
    }
}
