using AaanoDto.ClubeAaano;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClubeAaanoSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um resgate
    /// </summary>
    public class ResgatePromocaoModel : BaseModel
    {
        /// <summary>
        /// Nome do usuário
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        [Display(Name = "Promoção")]
        /// <summary>
        /// Promoção a ser resgatada
        /// </summary>
        public Guid IdPromocao { get; set; }

        /// <summary>
        /// Id do assinante que pode resgatar a promoção
        /// </summary>
        [Display(Name = "Assinatura")]
        public Guid IdAssinaturaPagSeguro { get; set; }

        /// <summary>
        /// Código simplificado da assinatura
        /// </summary>
        [Display(Name = "Código simplificado")]
        public string CodigoSimplificadoAssinatura { get; set; }

        /// <summary>
        /// Data e hora que foi feito o resgate
        /// </summary>
        [Display(Name = "Resgate")]
        public DateTime? Resgate { get; set; }

        /// <summary>
        /// Até quando pode ser resgatado
        /// </summary>
        [Display(Name = "Validade")]
        public DateTime Validade { get; set; }

        /// <summary>
        /// Usuário que fez o resgate
        /// MAX: 150
        /// </summary>
        [MaxLength(150, ErrorMessage = "O usuário do resgate deve ter até 150 caracteres")]
        [Display(Name = "Usuário responsável")]
        public string NomeUsuarioResgate { get; set; }

        /// <summary>
        /// Resumo da promoção a ser resgatada
        /// </summary>
        [Display(Name = "Promoção")]
        public string ResumoPromocao { get; set; }

        /// <summary>
        /// Nome da loja parceira
        /// </summary>
        [Display(Name = "Loja parceira")]
        public string NomeLojaParceira { get; set; }

        /// <summary>
        /// Nome da loja parceira
        /// </summary>
        [Display(Name = "Assinante")]
        public string NomeAssinante { get; set; }

        /// <summary>
        /// Converte um usuário de DTO para Model
        /// </summary>
        /// <param name="resgateDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(ResgatePromocaoDto resgateDto, ref string mensagemErro)
        {
            try
            {
                CodigoSimplificadoAssinatura = string.IsNullOrWhiteSpace(resgateDto.CodigoSimplificadoAssinatura) ? "" : resgateDto.CodigoSimplificadoAssinatura.Trim();
                NomeUsuarioResgate = string.IsNullOrWhiteSpace(resgateDto.NomeUsuarioResgate) ? "" : resgateDto.NomeUsuarioResgate.Trim();
                ResumoPromocao = string.IsNullOrWhiteSpace(resgateDto.ResumoPromocao) ? "" : resgateDto.ResumoPromocao.Trim();
                NomeLojaParceira = string.IsNullOrWhiteSpace(resgateDto.NomeLojaParceira) ? "" : resgateDto.NomeLojaParceira.Trim();
                NomeAssinante = string.IsNullOrWhiteSpace(resgateDto.NomeAssinante) ? "" : resgateDto.NomeAssinante.Trim();
                IdAssinaturaPagSeguro = resgateDto.IdAssinaturaPagSeguro;
                IdPromocao = resgateDto.IdPromocao;
                Resgate = resgateDto.Resgate;
                Validade = resgateDto.Validade;
                DataAlteracao = resgateDto.DataAlteracao;
                DataInclusao = resgateDto.DataInclusao;
                Id = resgateDto.Id;

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
        /// <param name="resgateDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref ResgatePromocaoDto resgateDto, ref string mensagemErro)
        {
            try
            {
                resgateDto.CodigoSimplificadoAssinatura = string.IsNullOrWhiteSpace(CodigoSimplificadoAssinatura) ? "" : CodigoSimplificadoAssinatura.Trim();
                resgateDto.NomeUsuarioResgate = string.IsNullOrWhiteSpace(NomeUsuarioResgate) ? "" : NomeUsuarioResgate.Trim();
                resgateDto.ResumoPromocao = string.IsNullOrWhiteSpace(ResumoPromocao) ? "" : ResumoPromocao.Trim();
                resgateDto.IdAssinaturaPagSeguro = IdAssinaturaPagSeguro;
                resgateDto.IdPromocao = IdPromocao;
                resgateDto.Resgate = Resgate;
                resgateDto.Validade = Validade;
                resgateDto.DataAlteracao = DataAlteracao;
                resgateDto.DataInclusao = DataInclusao;
                resgateDto.Id = Id;

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