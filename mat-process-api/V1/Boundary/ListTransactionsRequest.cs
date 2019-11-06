

using System.ComponentModel.DataAnnotations;

namespace mat_process_api.V1.Boundary
{
    public class ListTransactionsRequest
    {
        [Required] public string PropertyRef { get; set; }
    }
}
