using System.Collections.Generic;
using mat_process_api.V1.Domain;

namespace mat_process_api.V1.Gateways
{
    public interface ITransactionsGateway
    {
        List<Transaction> GetTransactionsByPropertyRef(string propertyRef);
    }
}
