using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZeroBudget.Data.EntityClasses;

namespace ZeroBudget.Data.Services
{
    public class BudgetingService : IBudgetingService
    {
        private ApplicationDbContext _context { get; set; }

        public BudgetingService(ApplicationDbContext context)
        {
            _context = context;
        }

        #region BudgetPeriods

        public Task<BudgetPeriod> GetBudgetPeriod(int budgetPeriodId)
        {
            return _context.budgetPeriods
                .AsNoTracking()
                .FirstOrDefaultAsync(
                bp => bp.BudgetPeriodId == budgetPeriodId);
        }

        public Task<List<BudgetPeriod>> GetBudgetPeriods(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                // No user Id specified, return an empty list
                return Task.FromResult(new List<BudgetPeriod>());

            return _context.budgetPeriods
                .AsNoTracking()
                // Get budget periods for the specified userId
                .Where(bp => bp.UserId == userId)
                .OrderByDescending(bp => bp.StartDate)
                .ThenByDescending(bp => bp.BudgetPeriodId)
                .ToListAsync();
        }

        public Task<BudgetPeriod> GetLatestBudgetPeriod(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                // No user Id specified, return null
                return null;

            return _context.budgetPeriods
                .AsNoTracking()
                // For the specified user
                .Where(bp => bp.UserId == userId)
                // Order by StartDate descending
                .OrderByDescending(bp => bp.StartDate)
                // Get the first item
                .FirstOrDefaultAsync();
        }

        public Task<List<BudgetPeriod>> GetBudgetPeriodsForCurrentMonth(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                // No user Id specified, return an empty list
                return Task.FromResult(new List<BudgetPeriod>());

            return _context.budgetPeriods
                .AsNoTracking()
                // Get budget periods
                .Where(bp => bp.UserId == userId
                && bp.StartDate.Month == DateTime.Now.Date.Month)
                .ToListAsync();
        }

        public bool AddBudgetPeriod(string userId, DateTime startDate,
            byte budgetPeriodTypeId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    // No user Id specified, return false
                    return false;

                if (_context.budgetPeriods
                    .AsNoTracking()
                    .Where(bp => bp.UserId == userId
                    && bp.StartDate == startDate
                    && bp.BudgetPeriodTypeId == budgetPeriodTypeId).Any())
                    // The budget period already exists, don't add another one
                    return true;

                // Add the budget period
                _context.budgetPeriods.Add(new BudgetPeriod
                {
                    UserId = userId,
                    BudgetPeriodTypeId = budgetPeriodTypeId,
                    StartDate = startDate
                });

                // Save the changes
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateBudgetPeriod(string userId, BudgetPeriod budgetPeriod)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    // No user Id specified, return false
                    return false;

                var budgetPeriodToUpdate
                    = _context.budgetPeriods
                    .Where(bp => bp.UserId == userId
                    && bp.BudgetPeriodId == budgetPeriod.BudgetPeriodId)
                    .FirstOrDefault();

                if (budgetPeriodToUpdate == null)
                    // No existing budget period found, return false
                    return false;

                // Else, update the budget period
                budgetPeriodToUpdate.BudgetPeriodTypeId
                    = budgetPeriod.BudgetPeriodTypeId;
                budgetPeriodToUpdate.UserId
                    = budgetPeriod.UserId;
                budgetPeriodToUpdate.StartDate
                    = budgetPeriod.StartDate;

                // Save changes
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region BudgetCategories

        public bool AddBudgetCategory(string userId, string name,
            bool isTaxDeductible = false, int? parentCategoryId = null)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    // You must provide a user Id
                    return false;

                if (_context.budgetCategories.Any(
                    bc => bc.UserId == userId
                    && bc.Name == name
                    && bc.IsTaxDeductible == isTaxDeductible
                    && bc.ParentBudgetCategoryId == parentCategoryId))
                    // It already exists, dont add it again
                    return true;

                // Add the budget category
                _context.budgetCategories.Add(
                    new BudgetCategory
                    {
                        UserId = userId,
                        Name = name,
                        IsTaxDeductible = isTaxDeductible,
                        ParentBudgetCategoryId = parentCategoryId
                    });
                
                // Save the changes
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Task<BudgetCategory> GetBudgetCategory(string userId, int budgetCategoryId)
        {
            return _context.budgetCategories
                .FirstOrDefaultAsync(
                bc => bc.UserId == userId 
                && bc.BudgetCategoryId == budgetCategoryId);
        }

        public Task<List<BudgetCategory>> GetBudgetCategories(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                // Return an empty list, no user Id was supplied
                return Task.FromResult(new List<BudgetCategory>());

            return _context.budgetCategories
                // Get the user defined categories as well as
                // the system defined categories
                .Where(bc => bc.UserId == userId 
                || bc.UserId == null 
                || bc.UserId.Length == 0)
                .ToListAsync();
        }

        public bool UpdateBudgetCategory(string userId, BudgetCategory budgetCategory)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    // No user id specified, return false
                    return false;

                // Find the item to update
                var budgetCategoryToUpdate
                    = _context.budgetCategories
                    .Where(bc => bc.UserId == userId
                    && bc.BudgetCategoryId == budgetCategory.BudgetCategoryId)
                    .FirstOrDefault();

                if (budgetCategoryToUpdate == null)
                    // The item does not exists, return false
                    return false;

                // Update the item
                budgetCategoryToUpdate.UserId = userId;
                budgetCategoryToUpdate.Name = budgetCategory.Name;
                budgetCategoryToUpdate.ParentBudgetCategoryId
                    = budgetCategory.ParentBudgetCategoryId;
                budgetCategoryToUpdate.IsTaxDeductible
                    = budgetCategory.IsTaxDeductible;

                // Save changes
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

    }
}
