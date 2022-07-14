using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace AaanoDto.Retornos
{
    /// <summary>
    /// Retorna a identificação com as informações criptografadas do login
    /// </summary>
    public class RetornoFazerLoginDto : RetornoDto
    {
        public RetornoFazerLoginDto()
        {
            ListaLojas = new List<Guid>();
        }

        /// <summary>
        /// String com as informações criptografadas do login
        /// </summary>
        public string Identificacao { get; set; }

        /// <summary>
        /// Nome do usuário do login
        /// </summary>
        public string NomeUsuario { get; set; }

        /// <summary>
        /// Id do usuário de login
        /// </summary>
        public Guid IdUsuario { get; set; }

        /// <summary>
        /// Retorna se o usuário é adm ou não
        /// </summary>
        public bool UsuarioAdministrador { get; set; }

        /// <summary>
        /// Listas de lojas permitidas
        /// </summary>
        public List<Guid> ListaLojas { get; set; }
    }
}
