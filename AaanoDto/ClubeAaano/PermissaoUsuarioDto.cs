using AaanoDto.Base;
using System;

namespace AaanoDto.ClubeAaano
{
    public class PermissaoUsuarioDto : BaseEntidadeDto
    {
        /// <summary>
        /// Usuário que a permissão pertence
        /// </summary>
        public Guid IdUsuario { get; set; }

        /// <summary>
        /// Loja que o usuário tem permissão
        /// </summary>
        public Guid IdLojaParceira { get; set; }

        /// <summary>
        /// Nome do usuário da permissão
        /// </summary>
        public string NomeUsuario { get; set; }

        /// <summary>
        /// Nome da loja da permissão
        /// </summary>
        public string NomeLoja { get; set; }
    }
}
