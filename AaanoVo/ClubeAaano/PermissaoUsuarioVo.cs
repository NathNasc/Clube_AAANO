using System;

namespace AaanoVo.ClubeAaano
{
    public class PermissaoUsuarioVo : EntidadeBaseVo
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
        /// Referencia a chave estrangeira
        /// </summary>
        public UsuarioVo Usuario { get; set; }

        /// <summary>
        /// Referencia a chave estrangeira
        /// </summary>
        public LojaParceiraVo LojaParceira { get; set; }
    }
}
