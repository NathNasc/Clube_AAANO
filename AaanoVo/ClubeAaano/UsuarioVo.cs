namespace AaanoVo.ClubeAaano
{
    public class UsuarioVo : EntidadeBaseVo
    {
        /// <summary>
        /// Nome do usuário
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// E-mail para autenticação no software
        /// MIN.: 5 / MAX.: 100
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Senha criptografada de autenticação do usuário
        /// MIN.: Criptografia / MAX.: 50
        /// </summary>
        public string Senha { get; set; }

        /// <summary>
        /// Indica se o usuário é administrador
        /// </summary>
        public bool Administrador { get; set; }
    }
}
