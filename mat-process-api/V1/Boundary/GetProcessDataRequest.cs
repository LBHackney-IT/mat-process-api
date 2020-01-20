using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.V1.Domain
{
    public class GetProcessDataRequest
    {
        [Required] public string processRef { get; set; }
    }
}
