using AaanoDto.Base;
using System;
using System.Collections.Generic;

namespace AaanoDto.PagSeguro
{
    public class ListaNovasAssinaturasDto : BaseEntidadeDto
    {
        public int resultsInThisPage { get; set; }
        public int currentPage { get; set; }
        public int totalPages { get; set; }
        public DateTime date { get; set; }
        public List<PreAprovados> preApprovalList { get; set; }
    }

    public class PreAprovados
    {
        public string name { get; set; }
        public string code { get; set; }
        public DateTime date { get; set; }
        public string tracker { get; set; }
        public string status { get; set; }
        public string reference { get; set; }
        public DateTime lastEventDate { get; set; }
        public string charge { get; set; }
    }
}
