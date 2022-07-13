using System;
using System.Collections.Generic;
using System.Web;

namespace ClubeAaanoSite
{
    public class SessaoUsuario
    {
        public SessaoUsuario()
        {
            Identificacao = "";
            NomeUsuario = "Não logado";
            IdUsuario = Guid.Empty;
            Administrador = false;
            LojasPermitidas = new List<Guid>();
        }

        // Propriedades da sessão
        public string NomeUsuario { get; set; }
        public Guid IdUsuario { get; set; }
        public string Identificacao { get; set; }
        public bool Administrador { get; set; }
        public List<Guid> LojasPermitidas { get; set; }

        /// <summary>
        /// Prepra a sessão
        /// </summary>
        public static SessaoUsuario SessaoLogin
        {
            get
            {
                SessaoUsuario sessao = (SessaoUsuario)HttpContext.Current.Session["_Sessao"];
                if (sessao == null)
                {
                    sessao = new SessaoUsuario();
                    HttpContext.Current.Session["_Sessao"] = sessao;
                }

                return sessao;
            }
        }
    }
}