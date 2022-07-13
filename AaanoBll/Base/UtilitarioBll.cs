using AaanoDto.Base;
using AaanoDto.Retornos;
using RestSharp;
using System;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using static AaanoEnum.PagSeguroEnum;

namespace AaanoBll.Base
{
    public static class UtilitarioBll
    {
        /// <summary>
        /// Criptografa a senha dos usuários
        /// </summary>
        /// <param name="senha"></param>
        /// <param name="senhaCriptografada"></param>
        public static void CriptografarSenha(string senha, ref string senhaCriptografada)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(senha + ChaveCriptografia()));
                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                senhaCriptografada = sBuilder.ToString();
            }
        }

        /// <summary>
        /// Criptografa a identificação para enviar nas requisições
        /// </summary>
        /// <param name="descriptografado"></param>
        /// <param name="criptografado"></param>
        /// <returns></returns>
        internal static bool CriptografarString(string descriptografado, ref string criptografado)
        {
            byte[] Results;
            UTF8Encoding UTF8 = new UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(ChaveCriptografia()));

            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            byte[] DataToEncrypt = UTF8.GetBytes(descriptografado);
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            criptografado = Convert.ToBase64String(Results);
            return true;
        }

        /// <summary>
        /// Decriptografa a identificação da requisição
        /// </summary>
        /// <param name="criptografado"></param>
        /// <param name="descriotografado"></param>
        /// <returns></returns>
        internal static bool DescriptografarString(string criptografado, ref string descriotografado)
        {
            byte[] Results;
            UTF8Encoding UTF8 = new UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(ChaveCriptografia()));

            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            byte[] DataToDecrypt = Convert.FromBase64String(criptografado);

            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            descriotografado = UTF8.GetString(Results);
            return true;
        }

        /// <summary>
        /// Valida a identificação recebida das requisições e retorna se o usuário é administrador
        /// </summary>
        /// <param name="identificacao"></param>
        /// <param name="idUsuario"></param>
        /// <param name="usuarioAdm"></param>
        /// <param name="mensagemRetorno"></param>
        /// <returns></returns>
        internal static bool ValidarUsuarioAdm(string identificacao, ref string mensagemRetorno)
        {
            string descriptografado = "";
            if (!DescriptografarString(identificacao, ref descriptografado))
            {
                mensagemRetorno = "A identificação da requisição é inválida.";
                return false;
            }

            // Se tiver a configuração de ADM
            if (descriptografado.Contains("Adm="))
            {
                bool adm;
                string indicadorAdm = descriptografado.Substring(descriptografado.LastIndexOf("Adm=") + 4, 1);

                if (!bool.TryParse((indicadorAdm == "0" ? "false" : "true"), out adm))
                {
                    return false;
                }

                // Retornar se o usuário é adm
                return adm;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Valida a identificação recebida das requisições
        /// </summary>
        /// <param name="identificacao"></param>
        /// <param name="idUsuario"></param>
        /// <param name="mensagemRetorno"></param>
        /// <returns></returns>
        internal static bool ValidarIdentificacao(string identificacao, Guid idUsuario, ref string mensagemRetorno)
        {
            string descriptografado = "";
            if (!DescriptografarString(identificacao, ref descriptografado))
            {
                mensagemRetorno = "Acesso  negado: a identificação da requisição é inválida.";
                return false;
            }

            DateTime dataRequisicao;

            try
            {
                string dataGeracao = descriptografado.Substring(0, 16);
                dataRequisicao = Convert.ToDateTime(dataGeracao);
            }
            catch (Exception)
            {
                mensagemRetorno = "Problemas para converter a data de geração da identificação.";
                return false;
            }

            //Se for anterior a 5 dias e maior que a data atual
            if (dataRequisicao < DateTime.Now.AddDays(-2) || dataRequisicao > DateTime.Now)
            {
                mensagemRetorno = "A data da identificação é inválida.";
                return false;
            }

            //Validar o guid 
            string idRequisicao = descriptografado.Substring(16, 36);
            if (idRequisicao.ToUpper() != RetornaGuidValidação().ToString().ToUpper())
            {
                mensagemRetorno = "O digito verificador é inválido.";
                return false;
            }

            //Validar o usuário
            idRequisicao = descriptografado.Substring(52, 36);
            if (idRequisicao.ToUpper() != idUsuario.ToString().ToUpper())
            {
                mensagemRetorno = "O usuário da requisição é diferente da identificação.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Guid de validação das requisições
        /// </summary>
        /// <returns></returns>
        internal static string RetornaGuidValidação()
        {
            return "06DDEE6F-30A9-43CF-A906-802A7C7AF110";
        }

        /// <summary>
        /// Retorna a chave para criptografia e descriptografia
        /// </summary>
        /// <returns></returns>
        private static string ChaveCriptografia()
        {
            return "";
        }

        /// <summary>
        /// Envia um email para o destinatário informado
        /// </summary>
        /// <param name="destinatario"></param>
        /// <param name="assunto"></param>
        /// <param name="corpo"></param>
        /// <returns></returns>
        internal static bool EnviarEmail(string destinatario, string assunto, string corpo, ref string mensagemErro)
        {
            try
            {
                MailMessage mail = new MailMessage("naoresponda@admclubeaaano.com.br", destinatario, assunto, corpo);
                mail.BodyEncoding = Encoding.UTF8;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = true;
                SmtpClient client = new SmtpClient("smtp.admclubeaaano.com.br", 587);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = false;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("naoresponda@admclubeaaano.com.br", "");
                client.Send(mail);

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao enviar o e-mail: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Retorna o id fixo do usuário de suporte
        /// </summary>
        /// <returns></returns>
        internal static Guid RetornarIdUsuarioSuporte()
        {
            return new Guid("3180C46E-9D85-4DDE-A4C0-9835CEAA8656");
        }

        /// <summary>
        /// 0 = Dom, 1 = Seg ... 6 = Sex
        /// </summary>
        /// <param name="dia"></param>
        /// <returns></returns>
        public static string RetornaDiaSemana(int dia)
        {
            switch (dia)
            {
                case 0:
                    return "Dom";
                case 1:
                    return "Seg";
                case 2:
                    return "Ter";
                case 3:
                    return "Qua";
                case 4:
                    return "Qui";
                case 5:
                    return "Sex";
                case 6:
                    return "Sáb";
                default:
                    return "Dia não reconhecido";
            }
        }

        /// <summary>
        /// 0 = Jan, 1 = Fev ... 12 = Dez
        /// </summary>
        /// <param name="dia"></param>
        /// <returns></returns>
        public static string RetornaMes(int mes)
        {
            switch (mes)
            {
                case 0:
                    return "Jan";
                case 1:
                    return "Fev";
                case 2:
                    return "Mar";
                case 3:
                    return "Abr";
                case 4:
                    return "Mai";
                case 5:
                    return "Jun";
                case 6:
                    return "Jul";
                case 7:
                    return "Ago";
                case 8:
                    return "Set";
                case 9:
                    return "Out";
                case 10:
                    return "Nov";
                case 11:
                    return "Dez";
                default:
                    return "Mês não reconhecido";
            }
        }

        /// <summary>
        /// Retorna o telefone preenchido com pontuação
        /// </summary>
        /// <param name="telefone"></param>
        /// <returns></returns>
        public static string RetornarTelefoneFormatado(string telefone)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(telefone))
                {
                    return "( )  -   ";
                }
                else
                {
                    if (telefone.Length == 12)
                    {
                        telefone = "(" + telefone.Substring(0, 2) + ") " + telefone.Substring(2, 5) + "-" + telefone.Substring(7, 4);
                    }
                    else
                    {
                        telefone = "(" + telefone.Substring(0, 2) + ") " + telefone.Substring(2, 4) + "-" + telefone.Substring(6, 4);
                    }

                    return telefone;
                }
            }
            catch (Exception)
            {
                return telefone;
            }

        }

        /// <summary>
        /// Valida uma resposta da chamada de um serviço
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="respostaHttp"></param>
        /// <param name="entidades"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public static bool ValidarRespostaHttpEmLista<T>(IRestResponse respostaHttp, ref T entidades, ref string mensagemErro) where T : BaseEntidadeDto
        {
            if (respostaHttp.StatusCode != HttpStatusCode.OK)
            {
                mensagemErro = "Problemas para consumir o serviço: Código retorno " + respostaHttp.StatusCode.ToString();
                return false;
            }

            try
            {
                JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                entidades = jsonSerializer.Deserialize<T>(respostaHttp.Content);
            }
            catch (Exception ex)
            {
                mensagemErro = "Problemas para consumir o serviço: " + ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Conver o status vindo do pagsegura para um enumerador
        /// </summary>
        /// <param name="statusPagSeguro"></param>
        /// <returns></returns>
        public static StatusAssinatura ConverterStatusPagSeguro(string statusPagSeguro)
        {
            switch (statusPagSeguro)
            {
                case "ACTIVE":
                    return StatusAssinatura.Ativo;

                case "CANCELLED":
                    return StatusAssinatura.Cancelada_PagSeguro;

                case "CANCELLED_BY_SENDER":
                    return StatusAssinatura.Cancelado_comprador;

                case "CANCELLED_BY_RECEIVER":
                    return StatusAssinatura.Cancelado_vendedor;

                case "EXPIRED":
                    return StatusAssinatura.Expirada;

                case "INITIATED":
                    return StatusAssinatura.Iniciado;

                case "PAYMENT_METHOD_CHANGE":
                    return StatusAssinatura.Pagamento_pendente;

                case "PENDING":
                    return StatusAssinatura.Pendente;

                case "SUSPENDED":
                    return StatusAssinatura.Suspensa;

                default:
                    return StatusAssinatura.Não_identificado;
            }
        }

        /// <summary>
        /// Faz a validação da requisição do assinante para obter as informações
        /// </summary>
        /// <param name="identificacao"></param>
        /// <param name="emailConfirmacao"></param>
        /// <param name="idAssinatura"></param>
        /// <param name="mensagemRetorno"></param>
        /// <returns></returns>
        public static bool ValidarIdentificacaoAssinante(string identificacao, string emailConfirmacao, out Guid idAssinatura, ref string mensagemRetorno)
        {
            if (string.IsNullOrWhiteSpace(emailConfirmacao))
            {
                idAssinatura = Guid.NewGuid();
                mensagemRetorno = "Acesso  negado: o email não foi confirmado.";
                return false;
            }

            string descriptografado = "";
            if (!DescriptografarString(identificacao, ref descriptografado))
            {
                idAssinatura = Guid.NewGuid();
                mensagemRetorno = "Acesso  negado: o link recebido é inválido.";
                return false;
            }

            // Validar o guid 
            string idRequisicao = descriptografado.Substring(0, 36);
            if (idRequisicao.ToUpper() != RetornaGuidValidação().ToUpper())
            {
                idAssinatura = Guid.NewGuid();
                mensagemRetorno = "Acesso  negado: o link recebido é inválido.";
                return false;
            }

            // Obter o id da assinatura
            string idAssinaturaRequisicao = descriptografado.Substring(36, 36);
            if (!Guid.TryParse(idAssinaturaRequisicao, out idAssinatura))
            {
                mensagemRetorno = "Não foi possível identificar a assinatura. Confirme o link com a administração.";
                return false;
            }

            // Validar o usuário
            string emailRequisicao = descriptografado.Substring(72, descriptografado.Length - 72);
            if (emailRequisicao.ToUpper() != emailConfirmacao.Trim().ToUpper())
            {
                mensagemRetorno = "O email digitado é diferente do email do link.";
                return false;
            }

            return true;
        }

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

        /// <summary>
        /// Retorna o link para enviar por email
        /// </summary>
        /// <param name="assinaturaVo"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        internal static bool ObterLinkInformacoesAssinatura(Guid id, string email, ref RetornoDto retornoDto)
        {
            // Validar a assinatura
            if (id == null || id == Guid.Empty)
            {
                retornoDto.Mensagem = "Para obter o link com as informações, favor informe a assinatura.";
                retornoDto.Retorno = false;
                return false;
            }

            // Validar o email
            if (string.IsNullOrWhiteSpace(email))
            {
                retornoDto.Mensagem = "Não é possível gerar o link de informações sem o email da assinatura.";
                retornoDto.Retorno = false;
                return false;
            }

            // Indetificação descriptografada
            string identificacaoDescriptografada = $"{UtilitarioBll.RetornaGuidValidação()}{id}{email.Trim()}";
            string identificacaoCriptografada = "";
            if (!UtilitarioBll.CriptografarString(identificacaoDescriptografada, ref identificacaoCriptografada))
            {
                retornoDto.Mensagem = "Não foi possível criptografar a identificação.";
                retornoDto.Retorno = false;
                return false;
            }

            retornoDto.Mensagem = "www.admclubeaaano.com.br/Utilidades/ConfirmarIdentificacao?identificacao=" + HttpUtility.UrlEncode(identificacaoCriptografada);
            retornoDto.Retorno = true;
            return true;
        }

    }
}
