using System;
using static AaanoEnum.PagSeguroEnum;

namespace AaanoVo.ClubeAaano
{
    public class AssinaturaPagSeguroVo : EntidadeBaseVo
    {
        // Dados gerais da assinatura

        /// <summary>
        /// Criação da assinatura no pagSeguro
        /// PagSeguro: date
        /// </summary>
        public DateTime Criacao { get; set; }

        /// <summary>
        /// Código simplificado da assinatura
        /// PagSeguro: reference
        /// </summary>
        public string CodigoSimplificado { get; set; }

        /// <summary>
        /// Status da assinatura
        /// PagSeguro: status
        /// </summary>
        public StatusAssinatura Status { get; set; }

        /// <summary>
        /// Ultima validação
        /// PagSeguro: lastEventDate
        /// </summary>
        public DateTime UltimoEnventoRegistrado { get; set; }

        /// <summary>
        /// Id do plano assinado
        /// </summary>
        public Guid IdPlano { get; set; }

        /// <summary>
        /// Código simplificado do plano
        /// PagSeguro: name
        /// </summary>
        public string ReferenciaPlano { get; set; }

        // Dados do assinante

        /// <summary>
        /// Email do assinante
        /// PagSeguro: sender > email
        /// MAX.: 60
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Nome do usuário
        /// PagSeguro: sender > name
        /// MAX.: 50
        /// </summary>
        public string NomeAssinate { get; set; }

        /// <summary>
        /// Telefone do assinante
        /// PagSeguro: sender > phone > areaCode + number
        /// MAX.: 11
        /// </summary>
        public string Telefone { get; set; }

        /// <summary>
        /// Estado de localização
        /// PagSeguro: address > state
        /// MAX.: 2
        /// </summary>
        public string Estado { get; set; }

        /// <summary>
        /// Número da casa
        /// PagSeguro: address > number
        /// MAX.: 20
        /// </summary>
        public string Numero { get; set; }

        /// <summary>
        /// País que mora
        /// PagSeguro: address > country
        /// MAX.: 10
        /// </summary>
        public string Pais { get; set; }

        /// <summary>
        /// Complemento do endereço
        /// PagSeguro: address > complement
        /// MAX.: 40
        /// </summary>
        public string Complemento { get; set; }

        /// <summary>
        /// Bairro que reside
        /// PagSeguro: address > district
        /// MAX.: 60
        /// </summary>
        public string Bairro { get; set; }

        /// <summary>
        /// Cidade que reside
        /// PagSeguro: address > city
        /// MAX.: 60
        /// </summary>
        public string Cidade { get; set; }

        /// <summary>
        /// CEP
        /// PagSeguro: address > postalCode
        /// MAX.: 8
        /// </summary>
        public string Cep { get; set; }

        /// <summary>
        /// Rua que reside
        /// PagSeguro: address > street
        /// MAX.: 80
        /// </summary>
        public string Logradouro { get; set; }
    }
}
