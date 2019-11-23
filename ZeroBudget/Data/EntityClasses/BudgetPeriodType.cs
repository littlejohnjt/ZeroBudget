using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZeroBudget.Data.EntityClasses
{
    public class BudgetPeriodType
    {
        public int BudgetPeriodTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
