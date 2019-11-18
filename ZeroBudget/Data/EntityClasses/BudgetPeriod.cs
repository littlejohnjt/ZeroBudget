using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZeroBudget.Data.EntityClasses
{
    public class BudgetPeriod
    {
        public int BudgetPeriodId { get; set; }
        public string UserId { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Budget Period Type")]
        public int BudgetPeriodTypeId { get; set; }

        public BudgetPeriodType BudgetPeriodType { get; set; }
   }
}
