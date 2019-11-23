using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZeroBudget.Data.EntityClasses;
using static ZeroBudget.Data.Enumerations;

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

        public async Task<BudgetPeriod> GetBudgetPeriod(string userId, int budgetPeriodId)
        {
            return await _context.budgetPeriods
                .Include(bp => bp.BudgetPeriodType)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                bp => bp.UserId == userId 
                && bp.BudgetPeriodId == budgetPeriodId);
        }

        public async Task<List<BudgetPeriod>> GetBudgetPeriods(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                // No user Id specified, return an empty list
                return new List<BudgetPeriod>();

            return await _context.budgetPeriods
                .Include(bp => bp.BudgetPeriodType)
                .AsNoTracking()
                // Get budget periods for the specified userId
                .Where(bp => bp.UserId == userId)
                .OrderByDescending(bp => bp.StartDate)
                .ThenByDescending(bp => bp.BudgetPeriodId)
                .ToListAsync();
        }

        public async Task<BudgetPeriod> GetLatestBudgetPeriod(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                // No user Id specified, return null
                return null;

            return await _context.budgetPeriods
                .AsNoTracking()
                .Include(bp => bp.BudgetPeriodType)
                // For the specified user
                .Where(bp => bp.UserId == userId)
                // Order by StartDate descending
                .OrderByDescending(bp => bp.StartDate)
                // Get the first item
                .FirstOrDefaultAsync();
        }

        public async Task<List<BudgetPeriod>> GetBudgetPeriodsForCurrentMonth(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                // No user Id specified, return an empty list
                return new List<BudgetPeriod>();

            return await _context.budgetPeriods
                .AsNoTracking()
                // Get budget periods
                .Where(bp => bp.UserId == userId
                && bp.StartDate.Month == DateTime.Now.Date.Month)
                .ToListAsync();
        }

        public async Task<bool> AddBudgetPeriod(string userId, DateTime startDate,
            int budgetPeriodTypeId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    // No user Id specified, return false
                    return false;

                if (await _context.budgetPeriods
                    .AsNoTracking()
                    .Where(bp => bp.UserId == userId
                    && bp.StartDate == startDate
                    && bp.BudgetPeriodTypeId == budgetPeriodTypeId).AnyAsync())
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
                if (await _context.SaveChangesAsync() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateBudgetPeriod(string userId, BudgetPeriod budgetPeriod)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    // No user Id specified, return false
                    return false;

                var budgetPeriodToUpdate
                    = await _context.budgetPeriods
                    .Where(bp => bp.UserId == userId
                    && bp.BudgetPeriodId == budgetPeriod.BudgetPeriodId)
                    .FirstOrDefaultAsync();

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
                if (await _context.SaveChangesAsync() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteBudgetPeriod(string userId, int budgetPeriodId)
        {
            // Find the budget period
            var budgetPeriod
                = await _context.budgetPeriods
                .FirstOrDefaultAsync(
                bp => bp.BudgetPeriodId == budgetPeriodId);

            if (budgetPeriod == null
                || (budgetPeriod.UserId != userId))
                // Either the budget period does not exist or the
                // user does not own the budget period, return false.
                return false;

            // Determine whether the budget period is being used
            if (await _context.budgetItems.AnyAsync(bi => bi.BudgetPeriodId == budgetPeriodId)
                || await _context.actualItems.AnyAsync(ai => ai.BudgetPeriodId == budgetPeriodId))
                // It is being used, return false, it can't be removed
                return false;

            try
            {
                // Else remove the period
                _context.budgetPeriods.Remove(budgetPeriod);

                if (await _context.SaveChangesAsync() > 0)
                    return true;
                else
                    return false;
            }
            catch(Exception)
            {
                // Something bad happened, returned false
                return false;
            }
        }

        #endregion

        #region BudgetCategories

        public async Task<bool> AddBudgetCategory(string userId, string name,
            bool isTaxDeductible = false, int? parentCategoryId = null)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    // You must provide a user Id
                    return false;

                if (await _context.budgetCategories.AnyAsync(
                    bc => bc.UserId == userId
                    && bc.Name == name
                    && bc.IsTaxDeductible == isTaxDeductible
                    && bc.ParentBudgetCategoryId == parentCategoryId) == true)
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
                if (await _context.SaveChangesAsync() > 0 )
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<BudgetCategory> GetBudgetCategory(string userId, int budgetCategoryId)
        {
            return await _context.budgetCategories
                .FirstOrDefaultAsync(
                bc => bc.UserId == userId 
                && bc.BudgetCategoryId == budgetCategoryId);
        }

        public async Task<List<BudgetCategory>> GetBudgetCategories(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                // Return an empty list, no user Id was supplied
                return new List<BudgetCategory>();

            return await _context.budgetCategories
                .Include("ParentBudgetCategory")
                // Get the user defined categories as well as
                // the system defined categories
                .Where(bc => bc.UserId == userId 
                || bc.UserId == null 
                || bc.UserId.Length == 0)
                .ToListAsync();
        }

        public async Task<bool> UpdateBudgetCategory(string userId, BudgetCategory budgetCategory)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    // No user id specified, return false
                    return false;

                // Find the item to update
                var budgetCategoryToUpdate
                    = await _context.budgetCategories
                    .Where(bc => bc.UserId == userId
                    && bc.BudgetCategoryId == budgetCategory.BudgetCategoryId)
                    .FirstOrDefaultAsync();

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
                if (await _context.SaveChangesAsync() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteBudgetCategory(string userId, int budgetCategoryId)
        {
            // Find the budget category
            var budgetCategory
                = await _context.budgetCategories
                .FirstOrDefaultAsync(
                bc => bc.BudgetCategoryId == budgetCategoryId);

            if (budgetCategory == null
                || (budgetCategory != null 
                && budgetCategory.UserId != userId))
                // Either the budget category does not exist or
                // the user does not own the budget category, return
                // false.
                return false;

            // Determine whether the budget category is being used
            if (await _context.budgetItems.AnyAsync(bi => bi.BudgetCategoryId == budgetCategoryId)
                || await _context.actualItems.AnyAsync(ai => ai.BudgetCategoryId == budgetCategoryId))
                // The budget category is in use, it can'tbe removed, return false
                return false;

            try
            {
                // Else remove the category
                _context.budgetCategories.Remove(budgetCategory);

                if (await _context.SaveChangesAsync() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                // Something bad happened, returned false
                return false;
            }
        }

        #endregion

        #region BudgetItem

        public async Task<bool> AddBudgetItem(string userId, int budgetCategoryId,
            int budgetPeriodId, DateTime date, decimal amount,
            TransactionType transactionType, bool isReoccurring,
            int? frequencyTypeId = null, int? frequencyQuantity = null)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return false;

                // The budget category associated with the budget item
                var budgetCategory 
                    = await _context.budgetCategories
                    .FirstOrDefaultAsync(
                    bc => bc.BudgetCategoryId == budgetCategoryId);

                // Does the user own the budget category
                if (budgetCategory == null
                    || budgetCategory.UserId != userId)
                    // return false, the user doesn't own the budget category
                    return false;

                var budgetPeriod
                    = await _context.budgetPeriods
                    .FirstOrDefaultAsync(
                    bp => bp.BudgetPeriodId == budgetPeriodId);

                if (budgetPeriod == null
                    || budgetPeriod.UserId != userId)
                    // return false, the user doesn't own the budget period
                    return false;

                // Everything is good, add the item to the
                // collection
                _context.budgetItems.Add(new BudgetItem
                {
                    UserId = userId,
                    BudgetCategoryId = budgetCategoryId,
                    BudgetPeriodId = budgetPeriodId,
                    Date = date,
                    Amount = amount,
                    TransactionType = transactionType,
                    IsReoccurring = isReoccurring,
                    FrequencyTypeId = frequencyTypeId,
                    FrequencyQuantity = frequencyQuantity
                });

                // Save the item
                if (await _context.SaveChangesAsync() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                // Something bad happened, return false
                return false;
            }
        }

        public async Task<bool> UpdateBudgetItem(string userId, BudgetItem budgetItem)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return false;

                // The budget category associated with the budget item
                var budgetCategory
                    = await _context.budgetCategories
                    .FirstOrDefaultAsync(
                    bc => bc.BudgetCategoryId == budgetItem.BudgetCategoryId);

                // Does the user own the budget category
                if (budgetCategory == null
                    || budgetCategory.UserId != userId)
                    // return false, the user doesn't own the budget category
                    return false;

                var budgetPeriod
                    = await _context.budgetPeriods
                    .FirstOrDefaultAsync(
                    bp => bp.BudgetPeriodId == budgetItem.BudgetPeriodId);

                if (budgetPeriod == null
                    || budgetPeriod.UserId != userId)
                    // return false, the user doesn't own the budget period
                    return false;

                // Everything is good, update the item

                // Get the current item
                var currentBudgetItem 
                    = await _context.budgetItems
                    .FirstOrDefaultAsync(bi => bi.BudgetItemId == budgetItem.BudgetItemId);

                // Update the current item
                currentBudgetItem.UserId = userId;
                currentBudgetItem.BudgetCategoryId = budgetItem.BudgetCategoryId;
                currentBudgetItem.BudgetPeriodId = budgetItem.BudgetPeriodId;
                currentBudgetItem.Date = budgetItem.Date;
                currentBudgetItem.Amount = budgetItem.Amount;
                currentBudgetItem.TransactionType = budgetItem.TransactionType;
                currentBudgetItem.FrequencyTypeId = budgetItem.FrequencyTypeId;
                currentBudgetItem.FrequencyQuantity = budgetItem.FrequencyQuantity;
                _context.budgetItems.Update(currentBudgetItem);

                // Save the item changes
                if (await _context.SaveChangesAsync() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                // Something bad happened, return false
                return false;
            }
        }

        public async Task<List<BudgetItem>> GetBudgetItemsForUser(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    // The user is null or empty, return an empty collection
                    return new List<BudgetItem>();

                return await _context.budgetItems
                    .Where(bi => bi.UserId == userId)
                    .ToListAsync();
            }
            catch (Exception)
            {
                // Something bad happened, return an empty collection
                return new List<BudgetItem>();
            }
        }

        public async Task<List<BudgetItem>> GetBudgetItemsForUserAndBudgetPeriod(string userId, int budgetPeriodId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    // The user is null or empty, return an empty collection
                    return new List<BudgetItem>();

                var budgetPeriod
                    = await _context.budgetPeriods
                    .FirstOrDefaultAsync(bp => bp.BudgetPeriodId == budgetPeriodId);

                if (budgetPeriod == null || budgetPeriod.UserId != userId)
                    // The budget period user does not match the current user, return an empty collection
                    return new List<BudgetItem>();

                return await _context.budgetItems
                    .Where(bi => bi.UserId == userId
                    && bi.BudgetPeriodId == budgetPeriodId)
                    .ToListAsync();
            }
            catch (Exception)
            {
                // Something bad happened, return an empty collection
                return new List<BudgetItem>();
            }
        }

        public async Task<bool> DeleteBudgetItem(string userId, int budgetItemId)
        {
            var budgetItem
                = await _context.budgetItems
                .FirstOrDefaultAsync(bi => bi.BudgetItemId == budgetItemId);

            if (budgetItem == null
                || (budgetItem != null && budgetItem.UserId != userId))
                return false;

            try
            {
                _context.budgetItems.Remove(budgetItem);

                if (await _context.SaveChangesAsync() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                // Something bad happened, return false
                return false;
            }
        }

        #endregion

        #region ActualItem

        public async Task<bool> AddActualItem(string userId, int budgetCategoryId,
            int budgetPeriodId, DateTime date, decimal amount,
            TransactionType transactionType)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return false;

                // The budget category associated with the budget item
                var budgetCategory
                    = await _context.budgetCategories
                    .FirstOrDefaultAsync(
                    bc => bc.BudgetCategoryId == budgetCategoryId);

                // Does the user own the budget category
                if (budgetCategory == null
                    || budgetCategory.UserId != userId)
                    // return false, the user doesn't own the budget category
                    return false;

                var budgetPeriod
                    = await _context.budgetPeriods
                    .FirstOrDefaultAsync(
                    bp => bp.BudgetPeriodId == budgetPeriodId);

                if (budgetPeriod == null
                    || budgetPeriod.UserId != userId)
                    // return false, the user doesn't own the budget period
                    return false;

                // Everything is good, add the item to the
                // collection
                _context.actualItems.Add(new ActualItem
                {
                    UserId = userId,
                    BudgetCategoryId = budgetCategoryId,
                    BudgetPeriodId = budgetPeriodId,
                    Date = date,
                    Amount = amount,
                    TransactionType = transactionType
                });

                // Save the item
                if (await _context.SaveChangesAsync() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                // Something bad happened, return false
                return false;
            }
        }

        public async Task<bool> UpdateActualItem(string userId, ActualItem actualItem)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return false;

                // The budget category associated with the budget item
                var budgetCategory
                    = await _context.budgetCategories
                    .FirstOrDefaultAsync(
                    bc => bc.BudgetCategoryId == actualItem.BudgetCategoryId);

                // Does the user own the budget category
                if (budgetCategory == null
                    || budgetCategory.UserId != userId)
                    // return false, the user doesn't own the budget category
                    return false;

                var budgetPeriod
                    = await _context.budgetPeriods
                    .FirstOrDefaultAsync(
                    bp => bp.BudgetPeriodId == actualItem.BudgetPeriodId);

                if (budgetPeriod == null
                    || budgetPeriod.UserId != userId)
                    // return false, the user doesn't own the budget period
                    return false;

                // Everything is good, update the item

                // Get the current item
                var currentActualItem 
                    = await _context.actualItems
                    .FirstOrDefaultAsync(ai => ai.ActualItemId == actualItem.ActualItemId);

                // Update the current item
                currentActualItem.UserId = userId;
                currentActualItem.BudgetCategoryId = actualItem.BudgetCategoryId;
                currentActualItem.BudgetPeriodId = actualItem.BudgetPeriodId;
                currentActualItem.Date = actualItem.Date;
                currentActualItem.Amount = actualItem.Amount;
                currentActualItem.TransactionType = actualItem.TransactionType;
                _context.actualItems.Update(currentActualItem);

                // Save the item changes
                if (await _context.SaveChangesAsync() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                // Something bad happened, return false
                return false;
            }
        }

        public async Task<List<ActualItem>> GetActualItemsForUser(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    // The user is null or empty, return an empty collection
                    return new List<ActualItem>();

                return await _context.actualItems
                    .Where(ai => ai.UserId == userId)
                    .ToListAsync();
            }
            catch (Exception)
            {
                // Something bad happened, return an empty collection
                return new List<ActualItem>();
            }
        }

        public async Task<List<ActualItem>> GetActualItemsForUserAndBudgetPeriod(
            string userId, int budgetPeriodId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    // The user is null or empty, return an empty collection
                    return new List<ActualItem>();

                var budgetPeriod
                    = await _context.budgetPeriods
                    .FirstOrDefaultAsync(bp => bp.BudgetPeriodId == budgetPeriodId);

                if (budgetPeriod == null || budgetPeriod.UserId != userId)
                    // The budget period user does not match the current user, return an empty collection
                    return new List<ActualItem>();

                return await _context.actualItems
                    .Where(ai => ai.UserId == userId
                    && ai.BudgetPeriodId == budgetPeriodId)
                    .ToListAsync();
            }
            catch (Exception)
            {
                // Something bad happened, return an empty collection
                return new List<ActualItem>();
            }
        }

        public async Task<bool> DeleteActualItem(string userId, int actualItemId)
        {
            var actualItem
                = await _context.actualItems
                .FirstOrDefaultAsync(ai => ai.ActualItemId == actualItemId);

            if (actualItem == null
                || (actualItem != null && actualItem.UserId != userId))
                return false;

            try
            {
                _context.actualItems.Remove(actualItem);

                if (await _context.SaveChangesAsync() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                // Something bad happened, return false
                return false;
            }
        }

        #endregion

        #region BudgetPeriodTypes

        public async Task<List<BudgetPeriodType>> GetBudgetPeriodTypes()
        {
            return await _context.budgetPeriodTypes.ToListAsync();
        }

        #endregion

        public async Task<List<FrequencyType>> GetFrequencyTypes()
        {
            return await _context.frequencyTypes.ToListAsync();
        }
    }
}
