using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZeroBudget.Data.EntityClasses
{
    public class BudgetCategoryValidator : AbstractValidator<BudgetCategory>
    {
        public BudgetCategoryValidator()
        {
            RuleFor(bc => bc.Name)
                .NotEmpty()
                .WithMessage("You must specify a category name");
        }
    }
}
