using AaanoDto.ClubeAaano;
using System;
using System.ComponentModel.DataAnnotations;
using static AaanoEnum.PagSeguroEnum;

namespace ClubeAaanoSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um usuário
    /// </summary>
    public class AssinaturaPagSeguroModel : BaseModel
    {
        /// <summary>
        /// Código simplificado da assinatura
        /// PagSeguro: reference
        /// </summary>
        [Display(Name = "Código")]
        public string CodigoSimplificado { get; set; }

        /// <summary>
        /// Criação da assinatura no pagSeguro
        /// PagSeguro: date
        /// </summary>
        [Display(Name = "Data da assinatura")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy hh:mm}")]
        public DateTime Criacao { get; set; }

        /// <summary>
        /// Status da assinatura
        /// PagSeguro: status
        /// </summary>
        [Display(Name = "Status")]
        public StatusAssinatura Status { get; set; }

        /// <summary>
        /// Ultima validação
        /// PagSeguro: lastEventDate
        /// </summary>
        [Display(Name = "Última atualização")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy hh:mm}")]
        public DateTime UltimoEnventoRegistrado { get; set; }

        /// <summary>
        /// Id do plano assinado
        /// </summary>
        [Display(Name = "Código")]
        public Guid IdPlano { get; set; }

        /// <summary>
        /// Código simplificado do plano
        /// PagSeguro: name
        /// </summary>
        [Display(Name = "Referência do plano")]
        public string ReferenciaPlano { get; set; }

        // Dados do assinante

        /// <summary>
        /// Email do assinante
        /// PagSeguro: sender > email
        /// MAX.: 60
        /// </summary>
        [Display(Name = "Email")]
        [MaxLength(60, ErrorMessage = "O email deve ter até 60 caracteres")]
        public string Email { get; set; }

        /// <summary>
        /// Nome do usuário
        /// PagSeguro: sender > name
        /// MAX.: 50
        /// </summary>
        [Display(Name = "Nome")]
        [MaxLength(50, ErrorMessage = "O nome deve ter até 50 caracteres")]
        public string NomeAssinate { get; set; }

        /// <summary>
        /// Telefone do assinante
        /// PagSeguro: sender > phone > areaCode + number
        /// MAX.: 11
        /// </summary>
        [Display(Name = "Telefone")]
        [MaxLength(15, ErrorMessage = "O telefone deve ter até 15 caracteres")]
        public string Telefone { get; set; }

        /// <summary>
        /// Estado de localização
        /// PagSeguro: address > state
        /// MAX.: 2
        /// </summary>
        [Display(Name = "UF")]
        [MaxLength(2, ErrorMessage = "O UF deve ter até 2 caracteres")]
        public string Estado { get; set; }

        /// <summary>
        /// Número da casa
        /// PagSeguro: address > number
        /// MAX.: 20
        /// </summary>
        [Display(Name = "Numero")]
        [MaxLength(20, ErrorMessage = "O número deve ter até 20 caracteres")]
        public string Numero { get; set; }

        /// <summary>
        /// País que mora
        /// PagSeguro: address > country
        /// MAX.: 10
        /// </summary>
        [Display(Name = "País")]
        [MaxLength(10, ErrorMessage = "O complemento deve ter até 10 caracteres")]
        public string Pais { get; set; }

        /// <summary>
        /// Complemento do endereço
        /// PagSeguro: address > complement
        /// MAX.: 40
        /// </summary>
        [Display(Name = "Complemento")]
        [MaxLength(40, ErrorMessage = "O complemento deve ter até 40 caracteres")]
        public string Complemento { get; set; }

        /// <summary>
        /// Bairro que reside
        /// PagSeguro: address > district
        /// MAX.: 60
        /// </summary>
        [Display(Name = "Bairro")]
        [MaxLength(60, ErrorMessage = "O bairro deve ter até 60 caracteres")]
        public string Bairro { get; set; }

        /// <summary>
        /// Cidade que reside
        /// PagSeguro: address > city
        /// MAX.: 60
        /// </summary>
        [Display(Name = "Cidade")]
        [MaxLength(60, ErrorMessage = "A cidade deve ter até 60 caracteres")]
        public string Cidade { get; set; }

        /// <summary>
        /// CEP
        /// PagSeguro: address > postalCode
        /// MAX.: 8
        /// </summary>
        [Display(Name = "CEP")]
        [MaxLength(9, ErrorMessage = "O CEP deve ter até 8 caracteres")]
        public string Cep { get; set; }

        /// <summary>
        /// Rua que reside
        /// PagSeguro: address > street
        /// MAX.: 80
        /// </summary>
        [Display(Name = "Logradouro")]
        [MaxLength(80, ErrorMessage = "O logradouro deve ter até 80 caracteres")]
        public string Logradouro { get; set; }

        /// <summary>
        /// Link para acessar a carteirinha
        /// </summary>
        [Display(Name = "Carteirinha")]
        public string LinkCarteirinha { get; set; }

        /// <summary>
        /// Converte um usuário de DTO para Model
        /// </summary>
        /// <param name="assinaturaDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(AssinaturaPagSeguroDto assinaturaDto, ref string mensagemErro)
        {
            try
            {
                this.CodigoSimplificado = string.IsNullOrWhiteSpace(assinaturaDto.CodigoSimplificado) ? "" : assinaturaDto.CodigoSimplificado.Trim();
                this.ReferenciaPlano = string.IsNullOrWhiteSpace(assinaturaDto.ReferenciaPlano) ? "" : assinaturaDto.ReferenciaPlano.Trim();
                this.Email = string.IsNullOrWhiteSpace(assinaturaDto.Email) ? "" : assinaturaDto.Email.Trim();
                this.NomeAssinate = string.IsNullOrWhiteSpace(assinaturaDto.NomeAssinate) ? "" : assinaturaDto.NomeAssinate.Trim();
                this.Telefone = string.IsNullOrWhiteSpace(assinaturaDto.Telefone) ? "" : assinaturaDto.Telefone.Trim();
                this.Estado = string.IsNullOrWhiteSpace(assinaturaDto.Estado) ? "" : assinaturaDto.Estado.Trim();
                this.Numero = string.IsNullOrWhiteSpace(assinaturaDto.Numero) ? "" : assinaturaDto.Numero.Trim();
                this.Pais = string.IsNullOrWhiteSpace(assinaturaDto.Pais) ? "" : assinaturaDto.Pais.Trim();
                this.Complemento = string.IsNullOrWhiteSpace(assinaturaDto.Complemento) ? "" : assinaturaDto.Complemento.Trim();
                this.Bairro = string.IsNullOrWhiteSpace(assinaturaDto.Bairro) ? "" : assinaturaDto.Bairro.Trim();
                this.Cidade = string.IsNullOrWhiteSpace(assinaturaDto.Cidade) ? "" : assinaturaDto.Cidade.Trim();
                this.Cep = string.IsNullOrWhiteSpace(assinaturaDto.Cep) ? "" : assinaturaDto.Cep.Trim();
                this.Logradouro = string.IsNullOrWhiteSpace(assinaturaDto.Logradouro) ? "" : assinaturaDto.Logradouro.Trim();
                this.LinkCarteirinha = string.IsNullOrWhiteSpace(assinaturaDto.LinkCarteirinha) ? "" : assinaturaDto.LinkCarteirinha.Trim();
                this.Status = assinaturaDto.Status;
                this.UltimoEnventoRegistrado = assinaturaDto.UltimoEnventoRegistrado;
                this.IdPlano = assinaturaDto.IdPlano;
                this.Criacao = assinaturaDto.Criacao;
                this.DataAlteracao = assinaturaDto.DataAlteracao;
                this.DataInclusao = assinaturaDto.DataInclusao;
                this.Id = assinaturaDto.Id;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um usuário de Model para Dto
        /// </summary>
        /// <param name="assinaturaDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref AssinaturaPagSeguroDto assinaturaDto, ref string mensagemErro)
        {
            try
            {
                assinaturaDto.CodigoSimplificado = string.IsNullOrWhiteSpace(this.CodigoSimplificado) ? "" : this.CodigoSimplificado.Trim();
                assinaturaDto.ReferenciaPlano = string.IsNullOrWhiteSpace(this.ReferenciaPlano) ? "" : this.ReferenciaPlano.Trim();
                assinaturaDto.Email = string.IsNullOrWhiteSpace(this.Email) ? "" : this.Email.Trim();
                assinaturaDto.NomeAssinate = string.IsNullOrWhiteSpace(this.NomeAssinate) ? "" : this.NomeAssinate.Trim();
                assinaturaDto.Telefone = string.IsNullOrWhiteSpace(this.Telefone) ? "" : this.Telefone.Trim();
                assinaturaDto.Estado = string.IsNullOrWhiteSpace(this.Estado) ? "" : this.Estado.Trim();
                assinaturaDto.Numero = string.IsNullOrWhiteSpace(this.Numero) ? "" : this.Numero.Trim();
                assinaturaDto.Pais = string.IsNullOrWhiteSpace(this.Pais) ? "" : this.Pais.Trim();
                assinaturaDto.Complemento = string.IsNullOrWhiteSpace(this.Complemento) ? "" : this.Complemento.Trim();
                assinaturaDto.Bairro = string.IsNullOrWhiteSpace(this.Bairro) ? "" : this.Bairro.Trim();
                assinaturaDto.Cidade = string.IsNullOrWhiteSpace(this.Cidade) ? "" : this.Cidade.Trim();
                assinaturaDto.Cep = string.IsNullOrWhiteSpace(this.Cep) ? "" : this.Cep.Trim();
                assinaturaDto.Logradouro = string.IsNullOrWhiteSpace(this.Logradouro) ? "" : this.Logradouro.Trim();
                assinaturaDto.Status = this.Status;
                assinaturaDto.UltimoEnventoRegistrado = this.UltimoEnventoRegistrado;
                assinaturaDto.IdPlano = this.IdPlano;
                assinaturaDto.Criacao = this.Criacao;
                assinaturaDto.DataAlteracao = this.DataAlteracao;
                assinaturaDto.DataInclusao = this.DataInclusao;
                assinaturaDto.Id = this.Id;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                return false;
            }
        }
    }
}