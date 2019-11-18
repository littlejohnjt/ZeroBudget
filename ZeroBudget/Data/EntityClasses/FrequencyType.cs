using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZeroBudget.Data.EntityClasses
{
    public class FrequencyType
    {
        public int FrequencyTypeId { get; set; }

        [Required]
        [Display(Name = "Name")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [DataType(DataType.Text)]
        public string Description { get; set; }
    }
}
