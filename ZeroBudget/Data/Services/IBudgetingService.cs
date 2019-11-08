using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZeroBudget.Data.EntityClasses;

namespace ZeroBudget.Data.Services
{
    public interface IBudgetingService
    {
        #region BudgetPeriods

        /// <summary>
        /// GetBudgetPeriod - Retrieves the specified budget period.  Note: the objects
        /// retrieved have EF change tracking turned off.
        /// </summary>
        /// <param name="budgetPeriodId">The budget periodId to retrieve</param>
        /// <returns>The specified budget period</returns>
        public Task<BudgetPeriod> GetBudgetPeriod(int budgetPeriodId);

        /// <summary>
        /// GetBudgetPeriods - Get all budget periods for the specified user.  Note: objects
        /// retrieve have EF change tracking turned off.
        /// </summary>
        /// <param name="userId">The user Id to get the BudgetPeriods for</param>
        /// <returns>The List of resulting BudgetPeriods</returns>
        public Task<List<BudgetPeriod>> GetBudgetPeriods(string userId);
        
        /// <summary>
        /// GetLatestBudgetPeriod - Get the latest BudgetPeriod, based on StartDate.  Note: 
        /// objects retrieved have 
        /// </summary>
        /// <param name="userId">The specified user Id</param>
        /// <returns>The lates BudgetPeriod</returns>
        public Task<BudgetPeriod> GetLatestBudgetPeriod(string userId);

        /// <summary>
        /// GetBudgetPeriodForCurrentMonth - Get the BudgetPeriod for the
        /// current month
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<List<BudgetPeriod>> GetBudgetPeriodsForCurrentMonth(string userId);

        /// <summary>
        /// AddBudgetPeriod - Add a BudgetPeriod to the collection 
        /// of BudgetPeriods for the specified user
        /// </summary>
        /// <param name="userId">The specified user Id, can't be null or empty</param>
        /// <param name="startDate">The start date of the BudgetPeriod</param>
        /// <param name="budgetPeriodTypeId">The type of BudgetPeriod 
        /// (Weekly, Bi-Weekly, etc.)(</param>
        public bool AddBudgetPeriod(string userId, DateTime startDate,
            byte budgetPeriodTypeId);

        /// <summary>
        /// Update the supplied BudgetPeriod
        /// </summary>
        /// <param name="budgetPeriod">The BudgetPeriod to update
        /// in the persistence store</param>
        public bool UpdateBudgetPeriod(string userId, BudgetPeriod budgetPeriod);

        #endregion

        #region BudgetCategories

        /// <summary>
        /// AddBudgetCategory - Add a new BudgetCategory
        /// </summary>
        /// <param name="userId">The specified UserId</param>
        /// <param name="name">The BudgetCategory name</param>
        /// <param name="isTaxDeductible">Is the BudgetCategory tax deductible</param>
        /// <param name="parentCategoryId">The parent BudgetCategory if one exists</param>
        public bool AddBudgetCategory(string userId, string name, 
            bool isTaxDeductible = false, int? parentCategoryId = null);

        public Task<BudgetCategory> GetBudgetCategory(int budgetCategoryId);

        /// <summary>
        /// GetBudgetCategories - Get a list of the user's and the common
        /// BudgetCategories
        /// </summary>
        /// <param name="userId">The specified UserId</param>
        /// <returns>The collection of BudgetCategories</returns>
        public Task<List<BudgetCategory>> GetBudgetCategories(string userId);

        /// <summary>
        /// UpdateBudgetCategory - Update and existing BudgetCategory.  Note
        /// you can only update BudgetCategories you have created.
        /// </summary>
        /// <param name="budgetCategory">The BudgetCategory to update</param>
        public bool UpdateBudgetCategory(string user, BudgetCategory budgetCategory);

        #endregion
    }
}
