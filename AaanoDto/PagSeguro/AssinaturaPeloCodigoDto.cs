using AaanoDto.Base;
using System;

namespace AaanoDto.PagSeguro
{
    public class AssinaturaPeloCodigoDto : BaseEntidadeDto
    {
        public string name { get; set; }
        public string code { get; set; }
        public DateTime date { get; set; }
        public string tracker { get; set; }
        public string status { get; set; }
        public string reference { get; set; }
        public DateTime lastEventDate { get; set; }
        public string charge { get; set; }
        public Sender sender { get; set; }
    }

    public class Sender
    {
        public string name { get; set; }
        public string email { get; set; }
        public Phone phone { get; set; }
        public Address address { get; set; }
    }

    public class Phone
    {
        public string number { get; set; }
        public string areaCode { get; set; }
    }

    public class Address
    {
        public string state { get; set; }
        public string number { get; set; }
        public string country { get; set; }
        public bool complete { get; set; }
        public string district { get; set; }
        public string city { get; set; }
        public string federationUnit { get; set; }
        public string postalCode { get; set; }
        public string street { get; set; }
        public string complement { get; set; }
        public bool addressRequired { get; set; }
    }

}
