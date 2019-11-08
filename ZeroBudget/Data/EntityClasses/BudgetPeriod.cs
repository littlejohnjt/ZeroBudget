using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZeroBudget.Data.EntityClasses
{
    public class BudgetPeriod
    {
        public int BudgetPeriodId { get; set; }
        public string UserId { get; set; }
        public DateTime StartDate { get; set; }
        public byte BudgetPeriodTypeId { get; set; }
        public BudgetPeriodType BudgetPeriodType { get; set; }
   }
}
