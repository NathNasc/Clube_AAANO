using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaanoEnum
{
    public class PagSeguroEnum
    {
        /// <summary>
        /// Status das assinaturas
        /// </summary>
        public enum StatusAssinatura
        {
            /// <summary>
            /// INITIATED: O comprador iniciou o processo de pagamento, 
            /// mas abandonou o checkout e não concluiu a compra.
            /// </summary>
            Iniciado = 0,
            /// <summary>
            /// PENDING: O processo de pagamento foi concluído e transação está 
            /// em análise ou aguardando a confirmação da operadora.
            /// </summary>
            Pendente = 1,
            /// <summary>
            /// ACTIVE: A criação da recorrência, transação validadora 
            /// ou transação recorrente foi aprovada.
            /// </summary>
            Ativo = 2,
            /// <summary>
            /// PAYMENT_METHOD_CHANGE: Uma transação retornou como "Cartão Expirado, 
            /// Cancelado ou Bloqueado" e o cartão da recorrência precisa ser substituído pelo comprador.
            /// </summary>
            Pagamento_pendente = 3,
            /// <summary>
            /// SUSPENDED: A recorrência foi suspensa pelo vendedor.
            /// </summary>
            Suspensa = 4,
            /// <summary>
            /// CANCELLED: A criação da recorrência foi cancelada pelo PagSeguro
            /// </summary>
            Cancelada_PagSeguro = 5,
            /// <summary>
            /// CANCELLED_BY_RECEIVER: A recorrência foi cancelada a pedido do vendedor.
            /// </summary>
            Cancelado_vendedor = 6,
            /// <summary>
            /// CANCELLED_BY_SENDER: A recorrência foi cancelada a pedido do comprador.
            /// </summary>
            Cancelado_comprador = 7,
            /// <summary>
            /// EXPIRED: A recorrência expirou por atingir a data limite da vigência ou por 
            /// ter atingido o valor máximo de cobrança definido na cobrança do plano.
            /// </summary>
            Expirada = 8,
            /// <summary>
            /// Quando o status não for reconhecido
            /// </summary>
            Não_identificado = 99
        }

        /// <summary>
        /// Status das ordens de pagamento
        /// </summary>
        public enum StatusPagamento
        {
            /// <summary>
            /// A ordem de pagamento está aguardando a data agendada para processamento.
            /// </summary>
            Agendada = 1,
            /// <summary>
            /// A ordem de pagamento está sendo processada pelo sistema.
            /// </summary>
            Processando = 2,
            /// <summary>
            /// A ordem de pagamento não pôde ser processada por alguma falha interna, a equipe do 
            /// PagSeguro é notificada imediatamente assim que isso ocorre.
            /// </summary>
            Não_Processada = 3,
            /// <summary>
            /// A ordem de pagamento foi desconsiderada pois a recorrência estava 
            /// suspensa na data agendada para processamento.
            /// </summary>
            Suspensa = 4,
            /// <summary>
            /// A ordem de pagamento foi paga, ou seja, a última transação vinculada 
            /// à ordem de pagamento foi paga.
            /// </summary>
            Paga = 5,
            /// <summary>
            /// A ordem de pagamento não pôde ser paga, ou seja, nenhuma transação 
            /// associada apresentou sucesso no pagamento.
            /// </summary>
            Não_Paga = 6
        }
    }
}
