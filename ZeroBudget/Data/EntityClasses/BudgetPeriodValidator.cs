using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZeroBudget.Data.EntityClasses
{
    public class BudgetPeriodValidator : AbstractValidator<BudgetPeriod>
    {
        public BudgetPeriodValidator()
        {
            RuleFor(bp => bp.BudgetPeriodTypeId)
                .GreaterThan(0)
                .WithMessage("You must specify the type of budget period");
            RuleFor(bp => bp.StartDate)
                .NotEmpty()
                .WithMessage("You must specify a date");
        }
    }
}
