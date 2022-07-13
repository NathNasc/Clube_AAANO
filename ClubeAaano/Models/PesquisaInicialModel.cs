using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClubeAaanoSite.Models
{
    /// <summary>
    /// Model com os campos em comum para as entidades
    /// </summary>
    public class PesquisaInicialModel
    {
        public PesquisaInicialModel()
        {
            Filtros = new Dictionary<string, string>();
        }

        /// <summary>
        /// Filtros a serem aplicados na tela
        /// </summary>
        public Dictionary<string, string> Filtros { get; set; }
    }
}