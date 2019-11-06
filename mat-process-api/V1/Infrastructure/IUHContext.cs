using Microsoft.EntityFrameworkCore;
using mat_process_api.V1.Domain;

namespace mat_process_api.V1.Infrastructure
{
    public interface IUHContext
    {
        DbSet<UhTransaction> UTransactions { get; set; }
    }
}
