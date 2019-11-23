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
        public async void AddBudgetPeriodTest()
        {
            InitializeContextAndService("AddBudgetPeriodTest");

            var user = "abc123";
            var date1 = new DateTime(2019, 11, 1);
            var date2 = new DateTime(2019, 11, 2);

            // Add a budget period
            var results = await _budgetingService.AddBudgetPeriod(user, date1, 1);

            // Get the budget period just added
            var budgetPeriod = _context.budgetPeriods.Where(bp => bp.UserId == user).FirstOrDefault();

            // Insure the budget period was added
            Assert.True(results ==  true && budgetPeriod.BudgetPeriodTypeId == 1 
                && budgetPeriod.StartDate == date1);

            // Add another budget period
            results = await _budgetingService.AddBudgetPeriod(user, date2, 1);

            // Get the budget period just added
            budgetPeriod = _context.budgetPeriods
                .Where(bp => bp.UserId == user)
                .OrderByDescending(bp => bp.BudgetPeriodId)
                .FirstOrDefault();

            // Insure the budget period was added
            Assert.True(results == true && budgetPeriod.BudgetPeriodTypeId == 1
                && budgetPeriod.StartDate == date2);

            // Attempt to add the same buget period again
            results = await _budgetingService.AddBudgetPeriod(user, date2, 1);

            // Insure another budget period was not added to the collection
            Assert.True(results == true && _context.budgetPeriods.Count() == 2);

            // Attempt to add a budget period and NOT specify a user Id
            results = await _budgetingService.AddBudgetPeriod(string.Empty, date1, 1);

            Assert.True(results == false && _context.budgetPeriods.Count() == 2);

            // Attempt to add a budget period and specify null for the user Id
            results = await _budgetingService.AddBudgetPeriod(null, date1, 1);

            Assert.True(results == false && _context.budgetPeriods.Count() == 2);
        }

        [Fact]
        public async void GetBudgetPeriodTest()
        {
            InitializeContextAndService("GetBudgetPeriodTest");

            var user = "abc123";
            var user2 = "xyz789";
            var date = new DateTime(2019, 11, 1);

            _context.budgetPeriodTypes.Add(new BudgetPeriodType { BudgetPeriodTypeId = 1, Name = "1", Description = "1" });
            _context.budgetPeriodTypes.Add(new BudgetPeriodType { BudgetPeriodTypeId = 2, Name = "2", Description = "2" });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user, BudgetPeriodTypeId = 1, StartDate = date });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user2, BudgetPeriodTypeId = 2, StartDate = date });
            _context.SaveChanges();
            var budgetPeriodIdUser = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user).BudgetPeriodId;
            var budgetPeriodIdUser2 = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user2).BudgetPeriodId;

            var budgetPeriodUser = await _budgetingService.GetBudgetPeriod(user, budgetPeriodIdUser);

            Assert.True(budgetPeriodUser.BudgetPeriodTypeId == 1 && budgetPeriodUser.StartDate == date);

            var budgetPeriodUser2 = await _budgetingService.GetBudgetPeriod(user, budgetPeriodIdUser2);

            Assert.True(budgetPeriodUser2 == null);
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
            _context.budgetPeriodTypes.Add(new BudgetPeriodType { BudgetPeriodTypeId = 1, Name = "1", Description = "1" });
            _context.budgetPeriodTypes.Add(new BudgetPeriodType { BudgetPeriodTypeId = 2, Name = "2", Description = "2" });
            _context.budgetPeriodTypes.Add(new BudgetPeriodType { BudgetPeriodTypeId = 3, Name = "3", Description = "3" });
            _context.budgetPeriodTypes.Add(new BudgetPeriodType { BudgetPeriodTypeId = 4, Name = "3", Description = "4" });
            _context.budgetPeriodTypes.Add(new BudgetPeriodType { BudgetPeriodTypeId = 5, Name = "3", Description = "4" });
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
        public async void UpdateBudgetPeriodTest()
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
            var results = await _budgetingService.UpdateBudgetPeriod(user1, budgetPeriod);

            // Insure the BudgetPeriod was updated
            Assert.True(results == true && _context.budgetPeriods.FirstOrDefault().BudgetPeriodTypeId == 2);

            // Budget period does not exist for the user, no changes should be made
            budgetPeriod.BudgetPeriodTypeId = 3;
            results = await _budgetingService.UpdateBudgetPeriod(userDoesNotExist, budgetPeriod);
            Assert.True(results == false && _context.budgetPeriods.FirstOrDefault().BudgetPeriodTypeId == 2);

            // No user specified, no changes should be made
            results = await _budgetingService.UpdateBudgetPeriod(string.Empty, budgetPeriod);
            Assert.True(results == false && _context.budgetPeriods.FirstOrDefault().BudgetPeriodTypeId == 2);

            // No user specified, no changes should be made
            results = await _budgetingService.UpdateBudgetPeriod(null, budgetPeriod);
            Assert.True(results == false && _context.budgetPeriods.FirstOrDefault().BudgetPeriodTypeId == 2);
        }

        [Fact]
        public async void DeleteBudgetPeriodTest()
        {
            InitializeContextAndService("DeleteBudgetPeriodTest");

            var user1 = "abc123";
            var user2 = "xyz789";
            var userDoesNotExist = "999999";
            var date = new DateTime(2019, 11, 1);

            // Create a BudgetPeriod
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user1, BudgetPeriodTypeId = 1, StartDate = date });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user1, BudgetPeriodTypeId = 2, StartDate = date.AddMonths(1) });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user2, BudgetPeriodTypeId = 3, StartDate = date.AddMonths(2) });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = userDoesNotExist, BudgetPeriodTypeId = 2, StartDate = date });
            _context.SaveChanges();

            _context.budgetCategories.Add(new BudgetCategory { UserId = user1, Name = "Test", IsTaxDeductible = false, 
                ParentBudgetCategoryId = null });
            _context.SaveChanges();

            var budgetPeriod = _context.budgetPeriods
                .FirstOrDefault(
                bp => bp.UserId == user1 && bp.BudgetPeriodTypeId == 1);
            var budgetPeriod2 = _context.budgetPeriods
                .FirstOrDefault(
                bp => bp.UserId == user1 && bp.BudgetPeriodTypeId == 2);
            var budgetPeriod3 = _context.budgetPeriods
                .FirstOrDefault(
                bp => bp.UserId == userDoesNotExist);
            var budgetPeriod4 = _context.budgetPeriods
                .FirstOrDefault(
                bp => bp.UserId == user2);
            var budgetCategory = _context.budgetCategories
                .FirstOrDefault(
                bc => bc.UserId == user1);

            // Create a budget item for a specific budget period
            _context.budgetItems.Add(
                new BudgetItem
                {
                    UserId = user1,
                    BudgetPeriodId = budgetPeriod.BudgetPeriodId,
                    BudgetCategoryId = budgetCategory.BudgetCategoryId,
                    Date = date,
                    Amount = 20.00M,
                    TransactionType = Enumerations.TransactionType.Credit,
                    IsReoccurring = false
                });

            // Create a actual item for a specific budget period
            _context.actualItems.Add(
                new ActualItem
                {
                    UserId = user2,
                    BudgetPeriodId = budgetPeriod4.BudgetPeriodId,
                    BudgetCategoryId = budgetCategory.BudgetCategoryId,
                    Date = date,
                    Amount = 20.00M,
                    TransactionType = Enumerations.TransactionType.Credit
                });

            _context.SaveChanges();

            // delete budget period that does not have an associted 
            // budget item or actual item.  Additionally this is a 
            // budget period owned by the user.  This delete should 
            // complete successfully.
            var result = await _budgetingService.DeleteBudgetPeriod(
                user1, budgetPeriod2.BudgetPeriodId);
            Assert.True(result == true
                && _context.budgetPeriods.Count() == 3);

            // delete budget period that does not have an associted 
            // budget item or actual item.  Additionally this is a 
            // budget period should NOT be owned by the user.  This 
            // delete should NOT complete successfully, the item should 
            // remain in the collection.
            result = await _budgetingService.DeleteBudgetPeriod(
                user1, budgetPeriod3.BudgetPeriodId);
            Assert.True(result == false
                && _context.budgetPeriods.Count() == 3);

            // Attempt to delete a budget period that does have an 
            // associated budget item.  The delete should NOT complete
            // successfully, this item should remain in the collection.
            result = await _budgetingService.DeleteBudgetPeriod(
                user1, budgetPeriod.BudgetPeriodId);
            Assert.True(result == false
                && _context.budgetPeriods.Count() == 3);

            // Attemp to delete a budget period that does have an 
            // associated actual item.  The delete should NOT complete
            // successfully, this item should remain in the collection.
            result = await _budgetingService.DeleteBudgetPeriod(
                user2, budgetPeriod4.BudgetPeriodId);
            Assert.True(result == false
                && _context.budgetPeriods.Count() == 3);
        }

        #endregion

        #region BudgetCategory

        [Fact]
        public async void AddBudgetCategoryTest()
        {
            InitializeContextAndService("AddBudgetCategoryTest");

            var user = "abc123";
            var categoryName = "A Test Category";
            var anotherCategoryName = "Another Test Category";

            // Add a budget category
            var results
                = await _budgetingService.AddBudgetCategory(
                    user, categoryName, false, null);

            // Insure it was added to the collection
            Assert.True(results == true
                && _context.budgetCategories.Where(bc => bc.UserId == user)
                .FirstOrDefault().Name == categoryName);

            // Add another budget category
            results
                = await _budgetingService.AddBudgetCategory(
                    user, anotherCategoryName, false, null);

            // Insure it was added to the collection
            Assert.True(results == true
                && _context.budgetCategories.Where(bc => bc.UserId == user)
                .OrderByDescending(bc => bc.BudgetCategoryId)
                .FirstOrDefault().Name == anotherCategoryName);

            // Try to add a budget category that is already in the collection
            results
                = await _budgetingService.AddBudgetCategory(
                    user, categoryName, false, null);

            // Insure that it did not add another category
            Assert.True(results == true && _context.budgetCategories.Count() == 2);

            // Attempt to add a budget category without specifying the user Id
            results
                = await _budgetingService.AddBudgetCategory(
                string.Empty, categoryName, false, null);

            // Insure the results are false and nothing was added to
            // the collection
            Assert.True(results == false && _context.budgetCategories.Count() == 2);

            // Attempt to add a budget category without specifying the user Id
            results
                = await _budgetingService.AddBudgetCategory(
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
            var budgetCategoryName3 = "Test3";

            // Add items to the budget category collection
            _context.budgetCategories.Add(new BudgetCategory { UserId = user, Name = budgetCategoryName, IsTaxDeductible = true, ParentBudgetCategoryId = null });
            _context.budgetCategories.Add(new BudgetCategory { UserId = user2, Name = budgetCategoryName2, IsTaxDeductible = false, ParentBudgetCategoryId = null });
            _context.budgetCategories.Add(new BudgetCategory { UserId = null, Name = budgetCategoryName3, IsTaxDeductible = true, ParentBudgetCategoryId = null });
            _context.SaveChanges();

            // Get the budget category Id of the first item added to the collection
            var budgetCategoryId = _context.budgetCategories
                .FirstOrDefault(bc => bc.UserId == user).BudgetCategoryId;

            // Attempt to retrieve the item the first item placed in the list.  Since the 'user' owns
            // the budget category, it returns the item
            var budgetCategory = await _budgetingService.GetBudgetCategory(user, budgetCategoryId);
            Assert.True(budgetCategory.UserId == user && budgetCategory.Name == budgetCategoryName
                && budgetCategory.IsTaxDeductible == true && budgetCategory.ParentBudgetCategoryId == null);

            // Get the budget category Id of the item with no user specified
            budgetCategoryId = _context.budgetCategories
                .FirstOrDefault(bc => bc.UserId == null).BudgetCategoryId;

            // Attempt to get the category with no user specified
            budgetCategory = await _budgetingService.GetBudgetCategory(null, budgetCategoryId);
            Assert.True(budgetCategory.Name == budgetCategoryName3);

            // Attempt to access a budgetCategory you do not have access to (i.e., userIds dont match)
            budgetCategoryId = _context.budgetCategories
                .FirstOrDefault(bc => bc.UserId == user).BudgetCategoryId;

            budgetCategory = await _budgetingService.GetBudgetCategory(user2, budgetCategoryId);
            Assert.True(budgetCategory == null);

            budgetCategory = await _budgetingService.GetBudgetCategory(null, budgetCategoryId);
            Assert.True(budgetCategory == null);
        }

        [Fact]
        public async void UpdateBudgetCategoryTest()
        {
            InitializeContextAndService("UpdateBudgetCategoryTest");

            var user = "abc123";
            var user2 = "xyz789";
            var budgetCategoryName = "Test1";
            var budgetCategoryName2 = "Test2";
            var budgetCategoryName3 = "Test3";
            var budgetCategoryUpdate = "Updated";
            var budgetCategoryUpdate2 = "Updated2";

            // Add items to the budget category collection
            _context.budgetCategories.Add(new BudgetCategory { UserId = user, Name = budgetCategoryName, IsTaxDeductible = true, ParentBudgetCategoryId = null });
            _context.budgetCategories.Add(new BudgetCategory { UserId = user2, Name = budgetCategoryName2, IsTaxDeductible = false, ParentBudgetCategoryId = null });
            _context.budgetCategories.Add(new BudgetCategory { UserId = null, Name = budgetCategoryName3, IsTaxDeductible = true, ParentBudgetCategoryId = null });
            _context.SaveChanges();

            var budgetCategory = _context
                .budgetCategories
                .FirstOrDefault(bc => bc.UserId == user && bc.Name == budgetCategoryName);

            budgetCategory.Name = budgetCategoryUpdate;

            // A successful update of a budget category
            var results = await _budgetingService.UpdateBudgetCategory(user, budgetCategory);
            Assert.True(results == true && _context
                .budgetCategories
                .FirstOrDefault(bc => bc.BudgetCategoryId == budgetCategory.BudgetCategoryId)
                .Name == budgetCategoryUpdate);

            budgetCategory = _context
                .budgetCategories
                .AsNoTracking()
                .FirstOrDefault(bc => bc.UserId == user2 && bc.Name == budgetCategoryName2);

            budgetCategory.Name = budgetCategoryUpdate2;

            results = await _budgetingService.UpdateBudgetCategory(user, budgetCategory);

            // You cant update a budget category that isn't yours
            Assert.True(results == false && _context
                .budgetCategories
                .FirstOrDefault(bc => bc.BudgetCategoryId == budgetCategory.BudgetCategoryId)
                .Name == budgetCategoryName2);

            budgetCategory = _context
                .budgetCategories
                .AsNoTracking()
                .FirstOrDefault(bc => bc.UserId == null);

            budgetCategory.Name = budgetCategoryUpdate;

            results = await _budgetingService.UpdateBudgetCategory(user, budgetCategory);
            
            // You can't update one of the default budget categories
            Assert.True(results == false
                && _context
                .budgetCategories
                .FirstOrDefault(bc => bc.BudgetCategoryId == budgetCategory.BudgetCategoryId)
                .Name == budgetCategoryName3);

            budgetCategory = _context
                .budgetCategories
                .AsNoTracking()
                .FirstOrDefault(bc => bc.UserId == null);

            budgetCategory.Name = budgetCategoryUpdate;

            results = await _budgetingService.UpdateBudgetCategory(null, budgetCategory);

            // Can't specify the userId as null
            Assert.True(results == false
                && _context
                .budgetCategories
                .AsNoTracking()
                .FirstOrDefault(bc => bc.BudgetCategoryId == budgetCategory.BudgetCategoryId)
                .Name == budgetCategoryName3);

            budgetCategory = _context
                .budgetCategories
                .AsNoTracking()
                .FirstOrDefault(bc => bc.UserId == null);

            budgetCategory.Name = budgetCategoryUpdate;

            results = await _budgetingService.UpdateBudgetCategory(string.Empty, budgetCategory);

            // Can't specify the userId as an empty string
            Assert.True(results == false
                && _context
                .budgetCategories
                .FirstOrDefault(bc => bc.BudgetCategoryId == budgetCategory.BudgetCategoryId)
                .Name == budgetCategoryName3);
        }

        [Fact]
        public async void GetBudgetCategoriesTest()
        {
            InitializeContextAndService("GetBudgetCategoriesTest");

            var user = "abc123";
            var user2 = "xyz789";
            var budgetCategoryName = "Test1";
            var budgetCategoryName2 = "Test2";
            var budgetCategoryName3 = "Test3";

            // Add items to the budget category collection
            _context.budgetCategories.Add(new BudgetCategory { UserId = user, Name = budgetCategoryName, IsTaxDeductible = true, ParentBudgetCategoryId = null });
            _context.budgetCategories.Add(new BudgetCategory { UserId = user2, Name = budgetCategoryName2, IsTaxDeductible = false, ParentBudgetCategoryId = null });
            _context.budgetCategories.Add(new BudgetCategory { UserId = null, Name = budgetCategoryName3, IsTaxDeductible = true, ParentBudgetCategoryId = null });
            _context.SaveChanges();

            var budgetCategories = await _budgetingService.GetBudgetCategories(user);

            Assert.True(budgetCategories.Count() == 2);

            foreach (var bc in budgetCategories)
            {
                Assert.True(bc.Name == budgetCategoryName || bc.Name == budgetCategoryName3);
            }
        }

        [Fact]
        public async void DeleteBudgetCategotyTest()
        {
            InitializeContextAndService("DeleteBudgetCategotyTest");

            var user1 = "abc123";
            var userDoesNotExist = "999999";
            var date = new DateTime(2019, 11, 1);

            // Create a BudgetPeriod
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user1, BudgetPeriodTypeId = 1, StartDate = date });
            _context.SaveChanges();

            _context.budgetCategories.Add(new BudgetCategory
            {
                UserId = user1,
                Name = "Category1",
                IsTaxDeductible = false,
                ParentBudgetCategoryId = null
            });
            _context.budgetCategories.Add(new BudgetCategory
            {
                UserId = user1,
                Name = "Category2",
                IsTaxDeductible = false,
                ParentBudgetCategoryId = null
            });
            _context.budgetCategories.Add(new BudgetCategory
            {
                UserId = userDoesNotExist,
                Name = "Category3",
                IsTaxDeductible = false,
                ParentBudgetCategoryId = null
            });
            _context.budgetCategories.Add(new BudgetCategory
            {
                UserId = user1,
                Name = "Category4",
                IsTaxDeductible = false,
                ParentBudgetCategoryId = null
            });
            _context.SaveChanges();

            var budgetPeriod = _context.budgetPeriods
                .FirstOrDefault(
                bp => bp.UserId == user1);
            var budgetCategory1 = _context.budgetCategories
                .FirstOrDefault(
                bc => bc.UserId == user1 && bc.Name == "Category1");
            var budgetCategory2 = _context.budgetCategories
                .FirstOrDefault(
                bc => bc.UserId == user1 && bc.Name == "Category2");
            var budgetCategory3 = _context.budgetCategories
                .FirstOrDefault(
                bc => bc.UserId == userDoesNotExist && bc.Name == "Category3");
            var budgetCategory4 = _context.budgetCategories
                .FirstOrDefault(
                bc => bc.UserId == user1 && bc.Name == "Category4");

            // Create a budget item for a specific budget category
            _context.budgetItems.Add(
                new BudgetItem
                {
                    UserId = user1,
                    BudgetPeriodId = budgetPeriod.BudgetPeriodId,
                    BudgetCategoryId = budgetCategory1.BudgetCategoryId,
                    Date = date,
                    Amount = 20.00M,
                    TransactionType = Enumerations.TransactionType.Credit,
                    IsReoccurring = false
                });

            // Create a actual item for a specific budget category
            _context.actualItems.Add(
                new ActualItem
                {
                    UserId = user1,
                    BudgetPeriodId = budgetPeriod.BudgetPeriodId,
                    BudgetCategoryId = budgetCategory2.BudgetCategoryId,
                    Date = date,
                    Amount = 20.00M,
                    TransactionType = Enumerations.TransactionType.Credit
                });

            _context.SaveChanges();

            // delete budget category that does not have an associted 
            // budget item or actual item.  Additionally this is a budget 
            // category owned by the user.  This delete should complete successfully.
            var result = await _budgetingService.DeleteBudgetCategory(
                user1, budgetCategory4.BudgetCategoryId);
            Assert.True(result == true
                && _context.budgetCategories.Count() == 3);

            // delete budget category that does not have an associted 
            // budget item or actual item.  Additionally this is a budget
            // category should NOT be owned by the user.  This delete 
            // should NOT complete successfully, the item should remain 
            // in the collection.
            result = await _budgetingService.DeleteBudgetCategory(
                user1, budgetCategory3.BudgetCategoryId);
            Assert.True(result == false
                && _context.budgetCategories.Count() == 3);

            // Attempt to delete a budget category that does have an 
            // associated budget item.  The delete should NOT complete
            // successfully, this item should remain in the collection. 
            result = await _budgetingService.DeleteBudgetCategory(
                user1, budgetCategory1.BudgetCategoryId);
            Assert.True(result == false
                && _context.budgetCategories.Count() == 3);

            // Attemp to delete a budget category that does have an associated actual item.  The delete should NOT complete
            // successfully, this item should remain in the collection.
            result = await _budgetingService.DeleteBudgetCategory(
                user1, budgetCategory2.BudgetCategoryId);
            Assert.True(result == false
                && _context.budgetCategories.Count() == 3);
        }

        #endregion

        #region BudgetItems

        [Fact]
        public async void AddBudgetItemTest()
        {
            InitializeContextAndService("AddBudgetItemTest");

            var user = "abc123";
            var user2 = "xyz789";
            var date = new DateTime(2019, 11, 1);

            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user, BudgetPeriodTypeId = 1, StartDate = date });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user2, BudgetPeriodTypeId = 1, StartDate = date });

            _context.budgetCategories.Add(new BudgetCategory
            {
                UserId = user,
                IsTaxDeductible = true,
                Name = "I own it",
                ParentBudgetCategoryId = null
            });
            _context.budgetCategories.Add(new BudgetCategory
            {
                UserId = user2,
                IsTaxDeductible = true,
                Name = "I don't own it",
                ParentBudgetCategoryId = null
            });

            _context.SaveChanges();

            var budgetPeriodOwned = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user);
            var budgetPeriodNotOwned = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user2);
            var budgetCategoryOwned = _context.budgetCategories.FirstOrDefault(bc => bc.UserId == user);
            var budgetCategoryNotOwned = _context.budgetCategories.FirstOrDefault(bc => bc.UserId == user2);

            // Add a budget item to the collection.  The budget category is one of the budget category owned by the user
            // or one of the global categories.  Check the collection to be sure the item was added
            var result = await _budgetingService.AddBudgetItem(user, budgetCategoryOwned.BudgetCategoryId,
                budgetPeriodOwned.BudgetPeriodId, date, 10.00M, Enumerations.TransactionType.Debit, false);

            Assert.True(result == true && _context.budgetItems.Where(bi => bi.UserId == user).Count() == 1);

            // Attempt to add a budget item for which the user doesn't own the category.  The budget item should not be added
            // to the collection.
            result = await _budgetingService.AddBudgetItem(user, budgetCategoryNotOwned.BudgetCategoryId,
                budgetPeriodOwned.BudgetPeriodId, date, 10.00M, Enumerations.TransactionType.Debit, false);

            Assert.True(result == false && _context.budgetItems.Where(bi => bi.UserId == user).Count() == 1);

            // Attempt to add a budget item for which the user doesn't own the budget period.  The budget item should not be added
            // to the collection.
            result = await _budgetingService.AddBudgetItem(user, budgetCategoryOwned.BudgetCategoryId,
                budgetPeriodNotOwned.BudgetPeriodId, date, 10.00M, Enumerations.TransactionType.Debit, false);

            Assert.True(result == false && _context.budgetItems.Where(bi => bi.UserId == user).Count() == 1);

            // Attempt to add a budget item and specify empty string for userId.  The budget item should not be added to the
            // collection.
            result = await _budgetingService.AddBudgetItem(string.Empty, budgetCategoryOwned.BudgetCategoryId,
                budgetPeriodOwned.BudgetPeriodId, date, 10.00M, Enumerations.TransactionType.Debit, false);

            Assert.True(result == false && _context.budgetItems.Where(bi => bi.UserId == user).Count() == 1);

            // Attempt to add a budget item and specify null for the userId.  The budget item should not be added to the
            // colelction.
            result = await _budgetingService.AddBudgetItem(null, budgetCategoryOwned.BudgetCategoryId,
                budgetPeriodOwned.BudgetPeriodId, date, 10.00M, Enumerations.TransactionType.Debit, false);

            Assert.True(result == false && _context.budgetItems.Where(bi => bi.UserId == user).Count() == 1);
        }

        [Fact]
        public async void UpdateBudgetItemTest()
        {
            InitializeContextAndService("UpdateBudgetItemTest");

            var user = "abc123";
            var user2 = "xyz789";
            var date = new DateTime(2019, 11, 1);

            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user, BudgetPeriodTypeId = 1, StartDate = date });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user2, BudgetPeriodTypeId = 1, StartDate = date });

            _context.budgetCategories.Add(new BudgetCategory
            {
                UserId = user,
                IsTaxDeductible = true,
                Name = "I own it",
                ParentBudgetCategoryId = null
            });
            _context.budgetCategories.Add(new BudgetCategory
            {
                UserId = user2,
                IsTaxDeductible = true,
                Name = "I don't own it",
                ParentBudgetCategoryId = null
            });

            _context.SaveChanges();

            var budgetPeriodOwned = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user);
            var budgetPeriodNotOwned = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user2);
            var budgetCategoryOwned = _context.budgetCategories.FirstOrDefault(bc => bc.UserId == user);
            var budgetCategoryNotOwned = _context.budgetCategories.FirstOrDefault(bc => bc.UserId == user2);

            _context.budgetItems.Add(new BudgetItem
            {
                UserId = user,
                BudgetCategoryId = budgetCategoryOwned.BudgetCategoryId,
                BudgetPeriodId = budgetPeriodOwned.BudgetPeriodId,
                Date = date,
                Amount = 10.00M,
                TransactionType = Enumerations.TransactionType.Debit,
                IsReoccurring = false,
                FrequencyTypeId = null,
                FrequencyQuantity = null
            });

            _context.SaveChanges();

            var budgetItemToUpdate
                = _context.budgetItems.AsNoTracking()
                .FirstOrDefault(bi => bi.UserId == user);

            // Update a budget item which the user owns.  The update should work successfully.
            budgetItemToUpdate.Amount = 20.00M;
            var result = await _budgetingService.UpdateBudgetItem(user, budgetItemToUpdate);
            Assert.True(result == true 
                && _context.budgetItems.FirstOrDefault(bi => 
                bi.BudgetItemId == budgetItemToUpdate.BudgetItemId).Amount == 20.00M);

            // Attempt to update a budget item where the user does not own the updated category.  This update
            // should not be successful.
            budgetItemToUpdate
                = _context.budgetItems.AsNoTracking()
                .FirstOrDefault(bi => bi.UserId == user);
            budgetItemToUpdate.BudgetCategoryId = budgetCategoryNotOwned.BudgetCategoryId;
            result = await _budgetingService.UpdateBudgetItem(user, budgetItemToUpdate);
            Assert.True(result == false
                && _context.budgetItems.FirstOrDefault(bi =>
                bi.BudgetItemId == budgetItemToUpdate.BudgetItemId).BudgetCategoryId
                == budgetCategoryOwned.BudgetCategoryId);

            // Attempt to update a budget item where the user does not own the updated budget period.  This update
            // should not be successful.
            budgetItemToUpdate
                = _context.budgetItems.AsNoTracking()
                .FirstOrDefault(bi => bi.UserId == user);
            budgetItemToUpdate.BudgetPeriodId = budgetPeriodNotOwned.BudgetPeriodId;
            result = await _budgetingService.UpdateBudgetItem(user, budgetItemToUpdate);
            Assert.True(result == false
                && _context.budgetItems.FirstOrDefault(bi =>
                bi.BudgetItemId == budgetItemToUpdate.BudgetItemId).BudgetPeriodId
                == budgetPeriodOwned.BudgetPeriodId);

            // Attempt to update a budget item where the userID specified is an empty string.  This upate should
            // not be successful.
            budgetItemToUpdate
                = _context.budgetItems.AsNoTracking()
                .FirstOrDefault(bi => bi.UserId == user);
            budgetItemToUpdate.Amount = 30.00M;
            result = await _budgetingService.UpdateBudgetItem(string.Empty, budgetItemToUpdate);
            Assert.True(result == false
                && _context.budgetItems.FirstOrDefault(bi =>
                bi.BudgetItemId == budgetItemToUpdate.BudgetItemId).Amount == 20.00M);

            // Attempt to update a budget item where the userId specified is a null string.  This update should 
            // not be successful.
            budgetItemToUpdate
                = _context.budgetItems.AsNoTracking()
                .FirstOrDefault(bi => bi.UserId == user);
            budgetItemToUpdate.Amount = 30.00M;
            result = await _budgetingService.UpdateBudgetItem(null, budgetItemToUpdate);
            Assert.True(result == false
                && _context.budgetItems.FirstOrDefault(bi =>
                bi.BudgetItemId == budgetItemToUpdate.BudgetItemId).Amount == 20.00M);
        }

        [Fact]
        public async void GetBudgetItemsForUserTest()
        {
            InitializeContextAndService("GetBudgetItemsForUserTest");

            var user = "abc123";
            var user2 = "xyz789";
            var user3 = "lmn456";
            var date = new DateTime(2019, 11, 1);

            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user, BudgetPeriodTypeId = 1, StartDate = date });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user2, BudgetPeriodTypeId = 1, StartDate = date });

            _context.budgetCategories.Add(new BudgetCategory
            {
                UserId = user,
                IsTaxDeductible = true,
                Name = "I own it",
                ParentBudgetCategoryId = null
            });
            _context.budgetCategories.Add(new BudgetCategory
            {
                UserId = user2,
                IsTaxDeductible = true,
                Name = "I don't own it",
                ParentBudgetCategoryId = null
            });

            _context.SaveChanges();

            var budgetPeriodOwned = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user);
            var budgetPeriodNotOwned = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user2);
            var budgetCategoryOwned = _context.budgetCategories.FirstOrDefault(bc => bc.UserId == user);
            var budgetCategoryNotOwned = _context.budgetCategories.FirstOrDefault(bc => bc.UserId == user2);

            _context.budgetItems.Add(new BudgetItem
            {
                UserId = user,
                BudgetCategoryId = budgetCategoryOwned.BudgetCategoryId,
                BudgetPeriodId = budgetPeriodOwned.BudgetPeriodId,
                Date = date,
                Amount = 10.00M,
                TransactionType = Enumerations.TransactionType.Debit,
                IsReoccurring = false,
                FrequencyTypeId = null,
                FrequencyQuantity = null
            });
            _context.budgetItems.Add(new BudgetItem
            {
                UserId = user,
                BudgetCategoryId = budgetCategoryOwned.BudgetCategoryId,
                BudgetPeriodId = budgetPeriodOwned.BudgetPeriodId,
                Date = date,
                Amount = 20.00M,
                TransactionType = Enumerations.TransactionType.Debit,
                IsReoccurring = false,
                FrequencyTypeId = null,
                FrequencyQuantity = null
            });
            _context.budgetItems.Add(new BudgetItem
            {
                UserId = user2,
                BudgetCategoryId = budgetCategoryNotOwned.BudgetCategoryId,
                BudgetPeriodId = budgetPeriodNotOwned.BudgetPeriodId,
                Date = date,
                Amount = 10.00M,
                TransactionType = Enumerations.TransactionType.Debit,
                IsReoccurring = false,
                FrequencyTypeId = null,
                FrequencyQuantity = null
            });
            _context.budgetItems.Add(new BudgetItem
            {
                UserId = user,
                BudgetCategoryId = budgetCategoryOwned.BudgetCategoryId,
                BudgetPeriodId = budgetPeriodOwned.BudgetPeriodId,
                Date = date,
                Amount = 30.00M,
                TransactionType = Enumerations.TransactionType.Debit,
                IsReoccurring = false,
                FrequencyTypeId = null,
                FrequencyQuantity = null
            });

            _context.SaveChanges();

            // Get user's items, should be 3
            var result = await _budgetingService.GetBudgetItemsForUser(user);
            Assert.True(result.Count == 3);

            // Get user2's items, there should be 1
            result = await _budgetingService.GetBudgetItemsForUser(user2);
            Assert.True(result.Count == 1);

            // Not specifying a user should results in no items
            result = await _budgetingService.GetBudgetItemsForUser(string.Empty);
            Assert.True(result.Count == 0);

            // Not specifying a user should results in no items
            result = await _budgetingService.GetBudgetItemsForUser(null);
            Assert.True(result.Count == 0);

            // Specifying a user that does not exists return no items
            result = await _budgetingService.GetBudgetItemsForUser(user3);
            Assert.True(result.Count == 0);
        }

        [Fact]
        public async void GetBudgetItemsForUserAndBudgetPeriodTest()
        {
            InitializeContextAndService("GetBudgetItemsForUserAndBudgetPeriodTest");

            var user = "abc123";
            var user2 = "xyz789";
            var user3 = "lmn456";
            var date = new DateTime(2019, 11, 1);

            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user, BudgetPeriodTypeId = 1, StartDate = date });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user, BudgetPeriodTypeId = 1, StartDate = date.AddMonths(1) });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user2, BudgetPeriodTypeId = 1, StartDate = date });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user3, BudgetPeriodTypeId = 1, StartDate = date });

            _context.budgetCategories.Add(new BudgetCategory
            {
                UserId = user,
                IsTaxDeductible = true,
                Name = "I own it",
                ParentBudgetCategoryId = null
            });
            _context.budgetCategories.Add(new BudgetCategory
            {
                UserId = user2,
                IsTaxDeductible = true,
                Name = "I don't own it",
                ParentBudgetCategoryId = null
            });

            _context.SaveChanges();

            var budgetPeriodOwned = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user && bp.StartDate == date);
            var budgetPeriodNextMonthOwned = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user && bp.StartDate == date.AddMonths(1));
            var budgetPeriodNotOwned = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user2);
            var budgetPeriodForUser3 = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user3);
            var budgetCategoryOwned = _context.budgetCategories.FirstOrDefault(bc => bc.UserId == user);
            var budgetCategoryNotOwned = _context.budgetCategories.FirstOrDefault(bc => bc.UserId == user2);

            _context.budgetItems.Add(new BudgetItem
            {
                UserId = user,
                BudgetCategoryId = budgetCategoryOwned.BudgetCategoryId,
                BudgetPeriodId = budgetPeriodOwned.BudgetPeriodId,
                Date = date,
                Amount = 10.00M,
                TransactionType = Enumerations.TransactionType.Debit,
                IsReoccurring = false,
                FrequencyTypeId = null,
                FrequencyQuantity = null
            });
            _context.budgetItems.Add(new BudgetItem
            {
                UserId = user,
                BudgetCategoryId = budgetCategoryOwned.BudgetCategoryId,
                BudgetPeriodId = budgetPeriodNextMonthOwned.BudgetPeriodId,
                Date = date.AddMonths(1),
                Amount = 20.00M,
                TransactionType = Enumerations.TransactionType.Debit,
                IsReoccurring = false,
                FrequencyTypeId = null,
                FrequencyQuantity = null
            });
            _context.budgetItems.Add(new BudgetItem
            {
                UserId = user2,
                BudgetCategoryId = budgetCategoryNotOwned.BudgetCategoryId,
                BudgetPeriodId = budgetPeriodNotOwned.BudgetPeriodId,
                Date = date,
                Amount = 10.00M,
                TransactionType = Enumerations.TransactionType.Debit,
                IsReoccurring = false,
                FrequencyTypeId = null,
                FrequencyQuantity = null
            });
            _context.budgetItems.Add(new BudgetItem
            {
                UserId = user,
                BudgetCategoryId = budgetCategoryOwned.BudgetCategoryId,
                BudgetPeriodId = budgetPeriodOwned.BudgetPeriodId,
                Date = date,
                Amount = 30.00M,
                TransactionType = Enumerations.TransactionType.Debit,
                IsReoccurring = false,
                FrequencyTypeId = null,
                FrequencyQuantity = null
            });

            _context.SaveChanges();

            // Get user's items, should be 3
            var result = await _budgetingService.GetBudgetItemsForUserAndBudgetPeriod(user, budgetPeriodOwned.BudgetPeriodId);
            Assert.True(result.Count == 2);

            // Get user2's items, there should be 1
            result = await _budgetingService.GetBudgetItemsForUserAndBudgetPeriod(user2, budgetPeriodNotOwned.BudgetPeriodId);
            Assert.True(result.Count == 1);

            // Get user's items, should be 3
            result = await _budgetingService.GetBudgetItemsForUserAndBudgetPeriod(user, budgetPeriodNextMonthOwned.BudgetPeriodId);
            Assert.True(result.Count == 1);

            // Not specifying a user should results in no items
            result = await _budgetingService.GetBudgetItemsForUserAndBudgetPeriod(string.Empty, budgetPeriodOwned.BudgetPeriodId);
            Assert.True(result.Count == 0);

            // Not specifying a user should results in no items
            result = await _budgetingService.GetBudgetItemsForUserAndBudgetPeriod(null, budgetPeriodOwned.BudgetPeriodId);
            Assert.True(result.Count == 0);

            // Specifying a user that does not exists return no items
            result = await _budgetingService.GetBudgetItemsForUserAndBudgetPeriod(user3, budgetPeriodForUser3.BudgetPeriodId);
            Assert.True(result.Count == 0);

            // Specifying a user that does not exists return no items
            result = await _budgetingService.GetBudgetItemsForUserAndBudgetPeriod(user3, budgetPeriodOwned.BudgetPeriodId);
            Assert.True(result.Count == 0);
        }

        [Fact]
        public async void DeleteBudgetItemTest()
        {
            InitializeContextAndService("DeleteBudgetItemTest");
        }

        #endregion

        #region ActualItems

        [Fact]
        public async void AddActualItemTest()
        {
            InitializeContextAndService("AddActualItemTest");

            var user = "abc123";
            var user2 = "xyz789";
            var date = new DateTime(2019, 11, 1);

            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user, BudgetPeriodTypeId = 1, StartDate = date });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user2, BudgetPeriodTypeId = 1, StartDate = date });

            _context.budgetCategories.Add(new BudgetCategory
            {
                UserId = user,
                IsTaxDeductible = true,
                Name = "I own it",
                ParentBudgetCategoryId = null
            });
            _context.budgetCategories.Add(new BudgetCategory
            {
                UserId = user2,
                IsTaxDeductible = true,
                Name = "I don't own it",
                ParentBudgetCategoryId = null
            });

            _context.SaveChanges();

            var budgetPeriodOwned = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user);
            var budgetPeriodNotOwned = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user2);
            var budgetCategoryOwned = _context.budgetCategories.FirstOrDefault(bc => bc.UserId == user);
            var budgetCategoryNotOwned = _context.budgetCategories.FirstOrDefault(bc => bc.UserId == user2);

            // Add a budget item to the collection.  The budget category is one of the budget category owned by the user
            // or one of the global categories.  Check the collection to be sure the item was added
            var result = await _budgetingService.AddActualItem(user, budgetCategoryOwned.BudgetCategoryId,
                budgetPeriodOwned.BudgetPeriodId, date, 10.00M, Enumerations.TransactionType.Debit);

            Assert.True(result == true && _context.actualItems.Where(ai => ai.UserId == user).Count() == 1);

            // Attempt to add a budget item for which the user doesn't own the category.  The budget item should not be added
            // to the collection.
            result = await _budgetingService.AddActualItem(user, budgetCategoryNotOwned.BudgetCategoryId,
                budgetPeriodOwned.BudgetPeriodId, date, 10.00M, Enumerations.TransactionType.Debit);

            Assert.True(result == false && _context.actualItems.Where(ai => ai.UserId == user).Count() == 1);

            // Attempt to add a budget item for which the user doesn't own the budget period.  The budget item should not be added
            // to the collection.
            result = await _budgetingService.AddActualItem(user, budgetCategoryOwned.BudgetCategoryId,
                budgetPeriodNotOwned.BudgetPeriodId, date, 10.00M, Enumerations.TransactionType.Debit);

            Assert.True(result == false && _context.actualItems.Where(ai => ai.UserId == user).Count() == 1);

            // Attempt to add a budget item and specify empty string for userId.  The budget item should not be added to the
            // collection.
            result = await _budgetingService.AddActualItem(string.Empty, budgetCategoryOwned.BudgetCategoryId,
                budgetPeriodOwned.BudgetPeriodId, date, 10.00M, Enumerations.TransactionType.Debit);

            Assert.True(result == false && _context.actualItems.Where(ai => ai.UserId == user).Count() == 1);

            // Attempt to add a budget item and specify null for the userId.  The budget item should not be added to the
            // colelction.
            result = await _budgetingService.AddActualItem(null, budgetCategoryOwned.BudgetCategoryId,
                budgetPeriodOwned.BudgetPeriodId, date, 10.00M, Enumerations.TransactionType.Debit);

            Assert.True(result == false && _context.actualItems.Where(ai => ai.UserId == user).Count() == 1);
        }

        [Fact]
        public async void UpdateActualItemTest()
        {
            InitializeContextAndService("UpdateActualItemTest");

            var user = "abc123";
            var user2 = "xyz789";
            var date = new DateTime(2019, 11, 1);

            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user, BudgetPeriodTypeId = 1, StartDate = date });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user2, BudgetPeriodTypeId = 1, StartDate = date });

            _context.budgetCategories.Add(new BudgetCategory
            {
                UserId = user,
                IsTaxDeductible = true,
                Name = "I own it",
                ParentBudgetCategoryId = null
            });
            _context.budgetCategories.Add(new BudgetCategory
            {
                UserId = user2,
                IsTaxDeductible = true,
                Name = "I don't own it",
                ParentBudgetCategoryId = null
            });

            _context.SaveChanges();

            var budgetPeriodOwned = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user);
            var budgetPeriodNotOwned = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user2);
            var budgetCategoryOwned = _context.budgetCategories.FirstOrDefault(bc => bc.UserId == user);
            var budgetCategoryNotOwned = _context.budgetCategories.FirstOrDefault(bc => bc.UserId == user2);

            _context.actualItems.Add(new ActualItem
            {
                UserId = user,
                BudgetCategoryId = budgetCategoryOwned.BudgetCategoryId,
                BudgetPeriodId = budgetPeriodOwned.BudgetPeriodId,
                Date = date,
                Amount = 10.00M,
                TransactionType = Enumerations.TransactionType.Debit
            });

            _context.SaveChanges();

            var actualItemToUpdate
                = _context.actualItems.AsNoTracking()
                .FirstOrDefault(ai => ai.UserId == user);

            // Update a budget item which the user owns.  The update should work successfully.
            actualItemToUpdate.Amount = 20.00M;
            var result = await _budgetingService.UpdateActualItem(user, actualItemToUpdate);
            Assert.True(result == true
                && _context.actualItems.FirstOrDefault(ai =>
                ai.ActualItemId == actualItemToUpdate.ActualItemId).Amount == 20.00M);

            // Attempt to update a budget item where the user does not own the updated category.  This update
            // should not be successful.
            actualItemToUpdate
                = _context.actualItems.AsNoTracking()
                .FirstOrDefault(ai => ai.UserId == user);
            actualItemToUpdate.BudgetCategoryId = budgetCategoryNotOwned.BudgetCategoryId;
            result = await _budgetingService.UpdateActualItem(user, actualItemToUpdate);
            Assert.True(result == false
                && _context.actualItems.FirstOrDefault(ai =>
                ai.ActualItemId == actualItemToUpdate.ActualItemId).BudgetCategoryId
                == budgetCategoryOwned.BudgetCategoryId);

            // Attempt to update a budget item where the user does not own the updated budget period.  This update
            // should not be successful.
            actualItemToUpdate
                = _context.actualItems.AsNoTracking()
                .FirstOrDefault(ai => ai.UserId == user);
            actualItemToUpdate.BudgetPeriodId = budgetPeriodNotOwned.BudgetPeriodId;
            result = await _budgetingService.UpdateActualItem(user, actualItemToUpdate);
            Assert.True(result == false
                && _context.actualItems.FirstOrDefault(ai =>
                ai.ActualItemId == actualItemToUpdate.ActualItemId).BudgetPeriodId
                == budgetPeriodOwned.BudgetPeriodId);

            // Attempt to update a budget item where the userID specified is an empty string.  This upate should
            // not be successful.
            actualItemToUpdate
                = _context.actualItems.AsNoTracking()
                .FirstOrDefault(bi => bi.UserId == user);
            actualItemToUpdate.Amount = 30.00M;
            result = await _budgetingService.UpdateActualItem(string.Empty, actualItemToUpdate);
            Assert.True(result == false
                && _context.actualItems.FirstOrDefault(ai =>
                ai.ActualItemId == actualItemToUpdate.ActualItemId).Amount == 20.00M);

            // Attempt to update a budget item where the userId specified is a null string.  This update should 
            // not be successful.
            actualItemToUpdate
                = _context.actualItems.AsNoTracking()
                .FirstOrDefault(ai => ai.UserId == user);
            actualItemToUpdate.Amount = 30.00M;
            result = await _budgetingService.UpdateActualItem(null, actualItemToUpdate);
            Assert.True(result == false
                && _context.actualItems.FirstOrDefault(ai =>
                ai.ActualItemId == actualItemToUpdate.ActualItemId).Amount == 20.00M);
        }

        [Fact]
        public async void GetActualItemsForUserTest()
        {
            InitializeContextAndService("GetActualItemsForUserTest");

            var user = "abc123";
            var user2 = "xyz789";
            var user3 = "lmn456";
            var date = new DateTime(2019, 11, 1);

            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user, BudgetPeriodTypeId = 1, StartDate = date });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user2, BudgetPeriodTypeId = 1, StartDate = date });

            _context.budgetCategories.Add(new BudgetCategory
            {
                UserId = user,
                IsTaxDeductible = true,
                Name = "I own it",
                ParentBudgetCategoryId = null
            });
            _context.budgetCategories.Add(new BudgetCategory
            {
                UserId = user2,
                IsTaxDeductible = true,
                Name = "I don't own it",
                ParentBudgetCategoryId = null
            });

            _context.SaveChanges();

            var budgetPeriodOwned = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user);
            var budgetPeriodNotOwned = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user2);
            var budgetCategoryOwned = _context.budgetCategories.FirstOrDefault(bc => bc.UserId == user);
            var budgetCategoryNotOwned = _context.budgetCategories.FirstOrDefault(bc => bc.UserId == user2);

            _context.actualItems.Add(new ActualItem
            {
                UserId = user,
                BudgetCategoryId = budgetCategoryOwned.BudgetCategoryId,
                BudgetPeriodId = budgetPeriodOwned.BudgetPeriodId,
                Date = date,
                Amount = 10.00M,
                TransactionType = Enumerations.TransactionType.Debit
            });
            _context.actualItems.Add(new ActualItem
            {
                UserId = user,
                BudgetCategoryId = budgetCategoryOwned.BudgetCategoryId,
                BudgetPeriodId = budgetPeriodOwned.BudgetPeriodId,
                Date = date,
                Amount = 20.00M,
                TransactionType = Enumerations.TransactionType.Debit
            });
            _context.actualItems.Add(new ActualItem
            {
                UserId = user2,
                BudgetCategoryId = budgetCategoryNotOwned.BudgetCategoryId,
                BudgetPeriodId = budgetPeriodNotOwned.BudgetPeriodId,
                Date = date,
                Amount = 10.00M,
                TransactionType = Enumerations.TransactionType.Debit
            });
            _context.actualItems.Add(new ActualItem
            {
                UserId = user,
                BudgetCategoryId = budgetCategoryOwned.BudgetCategoryId,
                BudgetPeriodId = budgetPeriodOwned.BudgetPeriodId,
                Date = date,
                Amount = 30.00M,
                TransactionType = Enumerations.TransactionType.Debit
            });

            _context.SaveChanges();

            // Get user's items, should be 3
            var result = await _budgetingService.GetActualItemsForUser(user);
            Assert.True(result.Count == 3);

            // Get user2's items, there should be 1
            result = await _budgetingService.GetActualItemsForUser(user2);
            Assert.True(result.Count == 1);

            // Not specifying a user should results in no items
            result = await _budgetingService.GetActualItemsForUser(string.Empty);
            Assert.True(result.Count == 0);

            // Not specifying a user should results in no items
            result = await _budgetingService.GetActualItemsForUser(null);
            Assert.True(result.Count == 0);

            // Specifying a user that does not exists return no items
            result = await _budgetingService.GetActualItemsForUser(user3);
            Assert.True(result.Count == 0);
        }

        [Fact]
        public async void GetActualItemsForUserAndBudgetPeriodTest()
        {
            InitializeContextAndService("GetActualItemsForUserAndBudgetPeriodTest");

            var user = "abc123";
            var user2 = "xyz789";
            var user3 = "lmn456";
            var date = new DateTime(2019, 11, 1);

            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user, BudgetPeriodTypeId = 1, StartDate = date });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user, BudgetPeriodTypeId = 1, StartDate = date.AddMonths(1) });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user2, BudgetPeriodTypeId = 1, StartDate = date });
            _context.budgetPeriods.Add(new BudgetPeriod { UserId = user3, BudgetPeriodTypeId = 1, StartDate = date });

            _context.budgetCategories.Add(new BudgetCategory
            {
                UserId = user,
                IsTaxDeductible = true,
                Name = "I own it",
                ParentBudgetCategoryId = null
            });
            _context.budgetCategories.Add(new BudgetCategory
            {
                UserId = user2,
                IsTaxDeductible = true,
                Name = "I don't own it",
                ParentBudgetCategoryId = null
            });

            _context.SaveChanges();

            var budgetPeriodOwned = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user && bp.StartDate == date);
            var budgetPeriodNextMonthOwned = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user && bp.StartDate == date.AddMonths(1));
            var budgetPeriodNotOwned = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user2);
            var budgetPeriodForUser3 = _context.budgetPeriods.FirstOrDefault(bp => bp.UserId == user3);
            var budgetCategoryOwned = _context.budgetCategories.FirstOrDefault(bc => bc.UserId == user);
            var budgetCategoryNotOwned = _context.budgetCategories.FirstOrDefault(bc => bc.UserId == user2);

            _context.actualItems.Add(new ActualItem
            {
                UserId = user,
                BudgetCategoryId = budgetCategoryOwned.BudgetCategoryId,
                BudgetPeriodId = budgetPeriodOwned.BudgetPeriodId,
                Date = date,
                Amount = 10.00M,
                TransactionType = Enumerations.TransactionType.Debit
            });
            _context.actualItems.Add(new ActualItem
            {
                UserId = user,
                BudgetCategoryId = budgetCategoryOwned.BudgetCategoryId,
                BudgetPeriodId = budgetPeriodNextMonthOwned.BudgetPeriodId,
                Date = date.AddMonths(1),
                Amount = 20.00M,
                TransactionType = Enumerations.TransactionType.Debit
            });
            _context.actualItems.Add(new ActualItem
            {
                UserId = user2,
                BudgetCategoryId = budgetCategoryNotOwned.BudgetCategoryId,
                BudgetPeriodId = budgetPeriodNotOwned.BudgetPeriodId,
                Date = date,
                Amount = 10.00M,
                TransactionType = Enumerations.TransactionType.Debit
            });
            _context.actualItems.Add(new ActualItem
            {
                UserId = user,
                BudgetCategoryId = budgetCategoryOwned.BudgetCategoryId,
                BudgetPeriodId = budgetPeriodOwned.BudgetPeriodId,
                Date = date,
                Amount = 30.00M,
                TransactionType = Enumerations.TransactionType.Debit
            });

            _context.SaveChanges();

            // Get user's items, should be 3
            var result = await _budgetingService.GetActualItemsForUserAndBudgetPeriod(user, budgetPeriodOwned.BudgetPeriodId);
            Assert.True(result.Count == 2);

            // Get user2's items, there should be 1
            result = await _budgetingService.GetActualItemsForUserAndBudgetPeriod(user2, budgetPeriodNotOwned.BudgetPeriodId);
            Assert.True(result.Count == 1);

            // Get user's items, should be 3
            result = await _budgetingService.GetActualItemsForUserAndBudgetPeriod(user, budgetPeriodNextMonthOwned.BudgetPeriodId);
            Assert.True(result.Count == 1);

            // Not specifying a user should results in no items
            result = await _budgetingService.GetActualItemsForUserAndBudgetPeriod(string.Empty, budgetPeriodOwned.BudgetPeriodId);
            Assert.True(result.Count == 0);

            // Not specifying a user should results in no items
            result = await _budgetingService.GetActualItemsForUserAndBudgetPeriod(null, budgetPeriodOwned.BudgetPeriodId);
            Assert.True(result.Count == 0);

            // Specifying a user that does not exists return no items
            result = await _budgetingService.GetActualItemsForUserAndBudgetPeriod(user3, budgetPeriodForUser3.BudgetPeriodId);
            Assert.True(result.Count == 0);

            // Specifying a user that does not exists return no items
            result = await _budgetingService.GetActualItemsForUserAndBudgetPeriod(user3, budgetPeriodOwned.BudgetPeriodId);
            Assert.True(result.Count == 0);
        }

        [Fact]
        public async void DeleteActualItemTest()
        {
            InitializeContextAndService("DeleteActualItemTest");
        }

        #endregion
    }
}
