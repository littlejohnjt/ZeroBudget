using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZeroBudget.Data.EntityClasses
{
    public class BudgetPeriodType
    {
        public byte BudgetPeriodTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
