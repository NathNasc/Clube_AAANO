using System;

namespace ClubeAaanoSite
{
    public static class Utilidades
    {
        /// <summary>
        /// Retorna o id do email de boas vindas
        /// </summary>
        /// <returns></returns>
        public static Guid RetornarIdEmailBoasVindas()
        {
            return new Guid("9C79E83A-52E0-4D25-9F62-75D3CE2B5E42");
        }

        /// <summary>
        /// Retorna o id do email de cobrança
        /// </summary>
        /// <returns></returns>
        public static Guid RetornarIdEmailCobranca()
        {
            return new Guid("8A9D11F5-851C-4393-B2B5-128586D8A88B");
        }

        public static string RetornaChaveCaptcha()
        {
            return "6LfHttIZAAAAALF5YuYYNp7_YsOp3nsPuzNfkCei";
        }
    }
}