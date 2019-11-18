using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZeroBudget.Data.EntityClasses;
using static ZeroBudget.Data.Enumerations;

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
        public Task<BudgetPeriod> GetBudgetPeriod(string userId, int budgetPeriodId);

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
        public Task<bool> AddBudgetPeriod(string userId, DateTime startDate,
            int budgetPeriodTypeId);

        /// <summary>
        /// Update the supplied BudgetPeriod
        /// </summary>
        /// <param name="budgetPeriod">The BudgetPeriod to update
        /// in the persistence store</param>
        public Task<bool> UpdateBudgetPeriod(string userId, BudgetPeriod budgetPeriod);

        /// <summary>
        /// DeleteBudgetPeriod - Delete the specified budget period.  This will
        /// not delete the budget period if it is being used elsewhere in the application.
        /// </summary>
        /// <param name="userId">The userId</param>
        /// <param name="budgetPeriodId">The budget period Id</param>
        /// <returns>true if successful and false if not successful</returns>
        public Task<bool> DeleteBudgetPeriod(string userId, int budgetPeriodId);

        #endregion

        #region BudgetCategories

        /// <summary>
        /// AddBudgetCategory - Add a new BudgetCategory
        /// </summary>
        /// <param name="userId">The specified UserId</param>
        /// <param name="name">The BudgetCategory name</param>
        /// <param name="isTaxDeductible">Is the BudgetCategory tax deductible</param>
        /// <param name="parentCategoryId">The parent BudgetCategory if one exists</param>
        public Task<bool> AddBudgetCategory(string userId, string name, 
            bool isTaxDeductible = false, int? parentCategoryId = null);

        public Task<BudgetCategory> GetBudgetCategory(string userId, int budgetCategoryId);

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
        public Task<bool> UpdateBudgetCategory(string userId, BudgetCategory budgetCategory);

        /// <summary>
        /// DeleteBudgetCategory - Deletes the budget category.
        /// The budget category can only be deleted if it is not assigned to any
        /// budget or actual items.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="budgetCategoryId"></param>
        /// <returns></returns>
        public Task<bool> DeleteBudgetCategory(string userId, int budgetCategoryId);

        #endregion

        #region BudgetItems

        /// <summary>
        /// AddBudgetItem - Add a budget item to the collection
        /// </summary>
        /// <param name="userId">The userId creating the budget item</param>
        /// <param name="budgetCategoryId">The budget category associated with 
        /// the budget item</param>
        /// <param name="budgetPeriodId">The budget period associated with the 
        /// budget item</param>
        /// <param name="date">The date of the budget item</param>
        /// <param name="amount">The amount associated with the budget item</param>
        /// <param name="transactionType">The type of transaction associated with 
        /// the budget item (cred/debit)</param>
        /// <param name="isReoccurring">Is the budget item re-occurring</param>
        /// <param name="frequencyTypeId">If the budget item is re-occurring, 
        /// what is the frequency type</param>
        /// <param name="frequencyQuantity">If the budget item is re-occurring
        /// what is the frequency quantity</param>
        /// <returns>True/False based on whether the add was successful</returns>
        public Task<bool> AddBudgetItem(string userId, int budgetCategoryId,
            int budgetPeriodId, DateTime date, decimal amount,
            TransactionType transactionType, bool isReoccurring,
            int? frequencyTypeId = null, int? frequencyQuantity = null);

        /// <summary>
        /// UpdateBudgetItem - Updates the specified budget item
        /// </summary>
        /// <param name="userId">The userId request the update to the budget item</param>
        /// <param name="budgetItem">The updated budget item</param>
        /// <returns>True/False based on whether the update was successful</returns>
        public Task<bool> UpdateBudgetItem(string userId, BudgetItem budgetItem);

        /// <summary>
        /// DeleteBudgetItem - Deletes the specified budget item
        /// </summary>
        /// <param name="userId">The userId requesting the deletion of the budget
        /// item</param>
        /// <param name="budgetItemId">The budgetItemId of the budget item to delete</param>
        /// <returns>True/False based on whether the delete was successful</returns>
        public Task<bool> DeleteBudgetItem(string userId, int budgetItemId);

        /// <summary>
        /// GetBudgetItemsForUser - Gets the budget items for the specified user
        /// </summary>
        /// <param name="userId">The userId for the specfied budget items</param>
        /// <returns>The collection budget items for the specified user</returns>
        public Task<List<BudgetItem>> GetBudgetItemsForUser(string userId);

        /// <summary>
        /// GetBudgetItemsForUserAndBudgetPeriod - Gets the budget items for the
        /// specified user and budget period.
        /// </summary>
        /// <param name="userId">The userId requesting the budget items</param>
        /// <param name="budgetPeriodId">The budget period to retrieve the
        /// budget items for</param>
        /// <returns>The collection of items for the specified user and budget 
        /// period</returns>
        public Task<List<BudgetItem>> GetBudgetItemsForUserAndBudgetPeriod(
            string userId, int budgetPeriodId);

        #endregion

        #region ActualItems

        /// <summary>
        /// AddActualItem - Add an actual item to the collection
        /// </summary>
        /// <param name="userId">The userId creating the actual item</param>
        /// <param name="budgetCategoryId"></param>
        /// <param name="budgetPeriodId"></param>
        /// <param name="date"></param>
        /// <param name="amount"></param>
        /// <param name="transactionType"></param>
        /// <returns></returns>
        public Task<bool> AddActualItem(string userId, int budgetCategoryId,
            int budgetPeriodId, DateTime date, decimal amount,
            TransactionType transactionType);

        /// <summary>
        /// UpdateActualItem - Updates the specified actual item
        /// </summary>
        /// <param name="userId">The userId requesting the updateto the actual item</param>
        /// <param name="actualItem">The updated actual item</param>
        /// <returns>True/False based on whether the update was successful</returns>
        public Task<bool> UpdateActualItem(string userId, ActualItem actualItem);

        /// <summary>
        /// DeleteActualItem - Deletes the specified actual item
        /// </summary>
        /// <param name="userId">The userId requesting the deletion of the actual
        /// item</param>
        /// <param name="budgetItemId">The actualItemId of the actual item to delete</param>
        /// <returns>True/False based on whether the delete was successful</returns>
        public Task<bool> DeleteActualItem(string userId, int actualItemId);

        /// <summary>
        /// GetActualItemsForUser - Gets the actual items for the specified user
        /// </summary>
        /// <param name="userId">The userId for the specfied actual items</param>
        /// <returns>The collection of actual items for the specified user</returns>
        public Task<List<ActualItem>> GetActualItemsForUser(string userId);

        /// <summary>
        /// GetActualItemsForUserAndBudgetPeriod - Gets the actual items for the
        /// specified user and budget period.
        /// </summary>
        /// <param name="userId">The userId requesting the actual items</param>
        /// <param name="budgetPeriodId">The budget period to retrieve the
        /// actual items for</param>
        /// <returns>The collection of actual items for the specified user and 
        /// budget period</returns>
        public Task<List<ActualItem>> GetActualItemsForUserAndBudgetPeriod(
            string userId, int budgetPeriodId);

        #endregion

        #region BudgetPeriodTypes

        public Task<List<BudgetPeriodType>> GetBudgetPeriodTypes();

        #endregion

        #region FrequencyTypes

        public Task<List<FrequencyType>> GetFrequencyTypes();

        #endregion
    }
}
