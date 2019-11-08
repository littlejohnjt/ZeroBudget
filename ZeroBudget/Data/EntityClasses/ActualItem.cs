using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ZeroBudget.Data.Enumerations;

namespace ZeroBudget.Data.EntityClasses
{
    public class ActualItem
    {
        public int ActualItemId { get; set; }
        public string UserId { get; set; }
        public int BudgetCategoryId { get; set; }
        public BudgetCategory BudgetCategory { get; set; }
        public int BudgetPeriodId { get; set; }
        public BudgetPeriod BudgetPeriod { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}
