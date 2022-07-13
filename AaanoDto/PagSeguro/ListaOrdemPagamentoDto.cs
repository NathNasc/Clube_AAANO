using AaanoDto.Base;
using System;
using System.Collections.Generic;

namespace AaanoDto.PagSeguro
{
    public class ListaOrdemPagamentoDto : BaseEntidadeDto
    {
        public ListaOrdemPagamentoDto()
        {
            paymentOrders = new Dictionary<string, Ordem>();
        }

        public DateTime date { get; set; }
        public int resultsInThisPage { get; set; }
        public int currentPage { get; set; }
        public int totalPages { get; set; }
        public Dictionary<string, Ordem> paymentOrders { get; set; }
    }

    public class Ordem
    {
        public string code { get; set; }
        public int status { get; set; }
        public float amount { get; set; }
        public int grossAmount { get; set; }
        public DateTime lastEventDate { get; set; }
        public DateTime schedulingDate { get; set; }
        public Transaction[] transactions { get; set; }
    }

    public class Transaction
    {
        public string code { get; set; }
        public DateTime date { get; set; }
        public int status { get; set; }
    }
}
