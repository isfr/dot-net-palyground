using System;
using System.Collections.Generic;

using BankApi.Domain.Entities;
using BankApi.Infrastructure;

namespace BankApi.IntegrationTest.Helpers
{
    internal static class Utilities
    {
        public static void InitializeDbForTests(BankDbContext db)
        {
            db.Customers.AddRange(GetCustomers());
            db.Accounts.AddRange(GetAccounts());
            db.Transactions.AddRange(GetTransactions());
            db.SaveChanges();
        }

        public static void ReinitializeDbForTests(BankDbContext db)
        {
            db.Customers.RemoveRange(db.Customers);
            db.Accounts.RemoveRange(db.Accounts);
            db.Transactions.RemoveRange(db.Transactions);
            InitializeDbForTests(db);
        }

        internal static List<Customer> GetCustomers()
        {
            return new List<Customer>()
            {
                new Customer() { Id = 1, Name = "Arisha Barron"},
                new Customer() { Id = 2, Name = "Branden Gibson"},
                new Customer() { Id = 3, Name = "Rhonda Church"},
                new Customer() { Id = 4, Name = "Georgina Hazel"},
            };
        }

        internal static List<Account> GetAccounts()
        {
            return new List<Account>()
            {
                new Account() { Id = 1, OwnerId = 1, Balance = 10 },
                new Account() { Id = 2, OwnerId = 2, Balance = 15 },
                new Account() { Id = 3, OwnerId = 3, Balance = 20 },
                new Account() { Id = 4, OwnerId = 4, Balance = 25 }
            };
        }

        internal static List<Transaction> GetTransactions()
        {
            return new List<Transaction>()
            {
                new Transaction() { Id = 1, DestinationAccountId = 2, OriginAccountId = 1, Timestamp = DateTime.Now.AddDays(-3), TransactionAmount = 10 },
                new Transaction() { Id = 2, DestinationAccountId = 3, OriginAccountId = 2, Timestamp = DateTime.Now.AddDays(-2), TransactionAmount = 10 },
                new Transaction() { Id = 3, DestinationAccountId = 1, OriginAccountId = 3, Timestamp = DateTime.Now.AddDays(-1), TransactionAmount = 10 },
                new Transaction() { Id = 4, DestinationAccountId = 3, OriginAccountId = 4, Timestamp = DateTime.Now, TransactionAmount = 10 },
            };
        }
    }
}
