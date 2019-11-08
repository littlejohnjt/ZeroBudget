using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZeroBudget.Data.EntityClasses
{
    public class BudgetCategory
    {
        public int BudgetCategoryId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public int? ParentBudgetCategoryId { get; set; }
        public bool IsTaxDeductible { get; set; }
    }
}
