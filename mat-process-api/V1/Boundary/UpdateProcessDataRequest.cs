using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using mat_process_api.V1.Domain;

namespace mat_process_api.V1.Boundary
{
    public class UpdateProcessDataRequest
    {
        [Required] public string processRef { get; set; }
        public MatUpdateProcessData processDataToUpdate { get; set; }
    }
}
