using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;
using ZeroBudget.Data;
using ZeroBudget.Data.EntityClasses;
using ZeroBudget.Data.Services;

namespace ZeroBudget.Test
{
    public class ZeroBudgetTest
    {
        private ApplicationDbContext _context;

        private BudgetingService _budgetingService;

        public ZeroBudgetTest() { }

        public void InitializeContextAndService(string databaseName)
        {
            // Instantiate the DB context with the in-memory option
            _context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName).Options);

            // Instantiate the budgeting service with
            _budgetingService = new BudgetingService(_context);
        }

        #region BudgetPeriods

        [Fact]
        public void AddBudgetPeriodTest()
        {
            InitializeContextAndService("AddBudgetPeriodTest");

            var user = "abc123";
            var date1 = new DateTime(2019, 11, 1);
            var date2 = new DateTime(2019, 11, 2);

            // Add a budget period
            var results = _budgetingService.AddBudgetPeriod(user, date1, 1);

            // Get the budget period just added
            var budgetPeriod = _context.budgetPeriods.Where(bp => bp.UserId == user).FirstOrDefault();

            // Insure the budget period was added
            Assert.True(results ==  true && budgetPeriod.BudgetPeriodTypeId == 1 
                && budgetPeriod.StartDate == date1);

            // Add another budget period
            results = _budgetingService.AddBudgetPeriod(user, date2, 1);

            // Get the budget period just added
            budgetPeriod = _context.budgetPeriods
                .Where(bp => bp.UserId == user)
                .OrderByDescending(bp => bp.BudgetPeriodId)
                .FirstOrDefault();

            // Insure the budget period was added
            Assert.True(results == true && budgetPeriod.BudgetPeriodTypeId == 1
                && budgetPeriod.StartDate == date2);

            // Attempt to add the same buget period again
            results = _budgetingService.AddBudgetPeriod(user, date2, 1);

            // Insure another budget period was not added to the collection
            Assert.True(results == true && _context.budgetPeriods.Count() == 2);

            // Attempt to add a budget period and NOT specify a user Id
            results = _budgetingService.AddBudgetPeriod(string.Empty, date1, 1);

            Assert.True(results == false && _context.budgetPeriods.Count() == 2);

            // Attempt to add a budget period and specify null for the user Id
            results = _budgetingService.AddBudgetPeriod(null, date1, 1);

            Assert.True(results == false && _context.budgetPeriods.Count() == 2);
        }

        [Fact]
        public async void GetBudgetPeriodTest()
        {
            InitializeContextAndService("GetBudgetPeriodTest");

            var user = "abc123";
            var date = new DateTime(2019, 11, 1);

            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user, BudgetPeriodTypeId = 1, StartDate = date });
            _context.SaveChanges();
            var budgetPeriodId = _context.budgetPeriods.FirstOrDefault().BudgetPeriodId;

            var budgetPeriod = await _budgetingService.GetBudgetPeriod(budgetPeriodId);

            Assert.True(budgetPeriod.BudgetPeriodTypeId == 1 && budgetPeriod.StartDate == date);
        }

        [Fact]
        public async void GetBudgetPeriodsTest()
        {
            InitializeContextAndService("GetBudgetPeriodsTest");

            var user1 = "abc123";
            var user2 = "xyz789";
            var userDoesntExist = "999999";
            var date = new DateTime(2019, 11, 1);

            // There are no items in the list, we should get an empty list
            var budgetPeriods = await _budgetingService.GetBudgetPeriods(user1);
            Assert.True(budgetPeriods.Count() == 0);

            // Get a colletion of BudgetPeriods
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user1, BudgetPeriodTypeId = 1, StartDate = date });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user1, BudgetPeriodTypeId = 2, StartDate = date.AddDays(1) });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user2, BudgetPeriodTypeId = 1, StartDate = date });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user2, BudgetPeriodTypeId = 2, StartDate = date.AddDays(1) });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user2, BudgetPeriodTypeId = 3, StartDate = date.AddDays(2) });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user1, BudgetPeriodTypeId = 3, StartDate = date });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user1, BudgetPeriodTypeId = 4, StartDate = date.AddDays(1) });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user1, BudgetPeriodTypeId = 5, StartDate = date.AddDays(2) });
            _context.SaveChanges();

            // Get all BudgetPeriods for the specified user
            budgetPeriods = await _budgetingService.GetBudgetPeriods(user1);

            // Get the first BudgetPeriod
            BudgetPeriod previousBudgetPeriod = budgetPeriods.FirstOrDefault();
            // For all other BudgetPeriods, skip the first
            foreach (var budgetPeriod in budgetPeriods.Skip(1))
            {
                // Each successive BudgetPeriod's StartDate should be less than or equal to the previous one.
                // i.e., in descending StartDate order.
                Assert.True(budgetPeriod.StartDate <= previousBudgetPeriod.StartDate);
                previousBudgetPeriod = budgetPeriod;
            }

            // If no userId is specified, it should return an empty list
            budgetPeriods = await _budgetingService.GetBudgetPeriods(string.Empty);
            Assert.True(budgetPeriods.Count() == 0);

            budgetPeriods = await _budgetingService.GetBudgetPeriods(null);
            Assert.True(budgetPeriods.Count() == 0);

            budgetPeriods = await _budgetingService.GetBudgetPeriods(userDoesntExist);
            Assert.True(budgetPeriods.Count() == 0);
        }

        [Fact]
        public async void GetLatestBudgetPeriodTest()
        {
            InitializeContextAndService("GetLatestBudgetPeriodTest");

            var user1 = "abc123";
            var user2 = "xyz789";
            var userDoesntExist = "999999";
            var date = new DateTime(2019, 11, 1);

            // There are no items in the list, we should get an empty list
            var budgetPeriod = await _budgetingService.GetLatestBudgetPeriod(user1);
            Assert.True(budgetPeriod == null);

            // Get a colletion of BudgetPeriods
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user1, BudgetPeriodTypeId = 1, StartDate = date });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user1, BudgetPeriodTypeId = 2, StartDate = date.AddDays(1) });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user2, BudgetPeriodTypeId = 1, StartDate = date });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user2, BudgetPeriodTypeId = 2, StartDate = date.AddDays(1) });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user2, BudgetPeriodTypeId = 3, StartDate = date.AddDays(2) });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user1, BudgetPeriodTypeId = 3, StartDate = date });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user1, BudgetPeriodTypeId = 4, StartDate = date.AddDays(1) });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user1, BudgetPeriodTypeId = 5, StartDate = date.AddDays(2) });
            _context.SaveChanges();

            budgetPeriod = await _budgetingService.GetLatestBudgetPeriod(user1);

            Assert.True(budgetPeriod.StartDate == date.AddDays(2));

            budgetPeriod = await _budgetingService.GetLatestBudgetPeriod(userDoesntExist);
            Assert.True(budgetPeriod == null);
        }

        [Fact]
        public async void GetBudgetPeriodForCurrentMonthTest()
        {
            InitializeContextAndService("GetBudgetPeriodForCurrentMonthTest");

            var user1 = "abc123";
            var user2 = "xyz789";
            var userDoesNotExist = "999999";
            var dateNotThisMonth = new DateTime(2000, 10, 1);
            var dateCurrentMonth = DateTime.Now.Date;

            // There are no items in the list, we should get an empty list
            var budgetPeriods = await _budgetingService.GetBudgetPeriodsForCurrentMonth(user1);
            Assert.True(budgetPeriods.Count() == 0);

            // Add a period along time ago
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user1, BudgetPeriodTypeId = 1, StartDate = dateNotThisMonth });
            // There are non periods this month, should be empty list
            budgetPeriods = await _budgetingService.GetBudgetPeriodsForCurrentMonth(user1);
            Assert.True(budgetPeriods.Count() == 0);

            // Get a colletion of BudgetPeriods
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user1, BudgetPeriodTypeId = 1, StartDate = dateCurrentMonth });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user1, BudgetPeriodTypeId = 2, StartDate = dateCurrentMonth.AddDays(1) });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user2, BudgetPeriodTypeId = 1, StartDate = dateCurrentMonth });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user2, BudgetPeriodTypeId = 2, StartDate = dateCurrentMonth.AddDays(1) });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user2, BudgetPeriodTypeId = 3, StartDate = dateCurrentMonth.AddDays(2) });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user1, BudgetPeriodTypeId = 3, StartDate = dateCurrentMonth });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user1, BudgetPeriodTypeId = 4, StartDate = dateCurrentMonth });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user1, BudgetPeriodTypeId = 5, StartDate = dateCurrentMonth });
            _context.SaveChanges();

            // All periods should be in this month
            budgetPeriods = await _budgetingService.GetBudgetPeriodsForCurrentMonth(user1);
            foreach (var budgetPeriod in budgetPeriods)
            {
                Assert.True(budgetPeriod.StartDate.Month == dateCurrentMonth.Month);
            }

            // User does not exist, the list should be empty
            budgetPeriods = await _budgetingService.GetBudgetPeriodsForCurrentMonth(userDoesNotExist);
            Assert.True(budgetPeriods.Count() == 0);
        }

        [Fact]
        public void UpdateBudgetPeriodTest()
        {
            InitializeContextAndService("UpdateBudgetPeriodTest");

            var user1 = "abc123";
            var userDoesNotExist = "999999";
            var date = new DateTime(2019, 11, 1);

            // Create a BudgetPeriod
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user1, BudgetPeriodTypeId = 1, StartDate = date });
            _context.SaveChanges();

            // Retrieve the created BudgetPeriod, disable tracking
            var budgetPeriod = _context.budgetPeriods.AsNoTracking().FirstOrDefault();

            // Update the BudgetPeriod
            budgetPeriod.BudgetPeriodTypeId = 2;
            var results = _budgetingService.UpdateBudgetPeriod(user1, budgetPeriod);

            // Insure the BudgetPeriod was updated
            Assert.True(results == true && _context.budgetPeriods.FirstOrDefault().BudgetPeriodTypeId == 2);

            // Budget period does not exist for the user, no changes should be made
            budgetPeriod.BudgetPeriodTypeId = 3;
            results = _budgetingService.UpdateBudgetPeriod(userDoesNotExist, budgetPeriod);
            Assert.True(results == false && _context.budgetPeriods.FirstOrDefault().BudgetPeriodTypeId == 2);

            // No user specified, no changes should be made
            results = _budgetingService.UpdateBudgetPeriod(string.Empty, budgetPeriod);
            Assert.True(results == false && _context.budgetPeriods.FirstOrDefault().BudgetPeriodTypeId == 2);

            // No user specified, no changes should be made
            results = _budgetingService.UpdateBudgetPeriod(null, budgetPeriod);
            Assert.True(results == false && _context.budgetPeriods.FirstOrDefault().BudgetPeriodTypeId == 2);
        }

        #endregion

        #region BudgetCategory

        [Fact]
        public void AddBudgetCategoryTest()
        {
            InitializeContextAndService("AddBudgetCategoryTest");

            var user = "abc123";
            var categoryName = "A Test Category";
            var anotherCategoryName = "Another Test Category";

            // Add a budget category
            var results
                = _budgetingService.AddBudgetCategory(
                    user, categoryName, false, null);

            // Insure it was added to the collection
            Assert.True(results == true
                && _context.budgetCategories.Where(bc => bc.UserId == user)
                .FirstOrDefault().Name == categoryName);

            // Add another budget category
            results
                = _budgetingService.AddBudgetCategory(
                    user, anotherCategoryName, false, null);

            // Insure it was added to the collection
            Assert.True(results == true
                && _context.budgetCategories.Where(bc => bc.UserId == user)
                .OrderByDescending(bc => bc.BudgetCategoryId)
                .FirstOrDefault().Name == anotherCategoryName);

            // Try to add a budget category that is already in the collection
            results
                = _budgetingService.AddBudgetCategory(
                    user, categoryName, false, null);

            // Insure that it did not add another category
            Assert.True(results == true && _context.budgetCategories.Count() == 2);

            // Attempt to add a budget category without specifying the user Id
            results
                = _budgetingService.AddBudgetCategory(
                string.Empty, categoryName, false, null);

            // Insure the results are false and nothing was added to
            // the collection
            Assert.True(results == false && _context.budgetCategories.Count() == 2);

            // Attempt to add a budget category without specifying the user Id
            results
                = _budgetingService.AddBudgetCategory(
                null, categoryName, false, null);

            // Insure the results are false and nothing was added to
            // the collection
            Assert.True(results == false && _context.budgetCategories.Count() == 2);
        }

        [Fact]
        public async void GetBudgetCategoryTest()
        {
            InitializeContextAndService("GetBudgetPeriodTest");

            var user = "abc123";
            var user2 = "xyz789";
            var budgetCategoryName = "Test1";
            var budgetCategoryName2 = "Test2";

            _context.budgetCategories.Add(new BudgetCategory { UserId = user, Name = budgetCategoryName, IsTaxDeductible = true, ParentBudgetCategoryId = null });
            _context.budgetCategories.Add(new BudgetCategory { UserId = user2, Name = budgetCategoryName2, IsTaxDeductible = false, ParentBudgetCategoryId = null });
            _context.SaveChanges();

            var budgetCategoryId = _context.budgetCategories
                .OrderBy(bc => bc.BudgetCategoryId)
                .FirstOrDefault().BudgetCategoryId;

            // Attempt to retrieve the item just placed in the list
            var budgetCategory = await _budgetingService.GetBudgetCategory(user, budgetCategoryId);
            Assert.True(budgetCategory.UserId == user && budgetCategory.Name == budgetCategoryName
                && budgetCategory.IsTaxDeductible == true && budgetCategory.ParentBudgetCategoryId == null);

            var budgetPeriodId = _context.budgetPeriods.FirstOrDefault().BudgetPeriodId;

            var budgetPeriod = await _budgetingService.GetBudgetPeriod(budgetPeriodId);

            Assert.True(budgetPeriod.BudgetPeriodTypeId == 1 && budgetPeriod.StartDate == date);
        }

        [Fact]
        public void UpdateBudgetCategoryTest()
        {

        }

        [Fact]
        public void GetBudgetCategoriesTest()
        {

        }

        #endregion
    }
}
