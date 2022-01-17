using System.Threading.Tasks;
using NSE.Pagamentos.API.Models;

namespace NSE.Pagamentos.Facade
{
    public interface IPagamentoFacade
    {
        Task<Transacao> AutorizarPagamento(API.Models.Pagamento pagamento);
        Task<Transacao> CapturarPagamento(Transacao transacao);
        Task<Transacao> CancelarAutorizacao(Transacao transacao);
    }
}