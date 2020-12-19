using System;
using System.Collections.Generic;

namespace SbankenYnab.Models
{
    public class AccountsList
    {
        public int AvailableItems { get; set; }
        public List<Account> Items { get; set; }
    }
    
    public class Account
    {
        public String AccountId { get; set; }
        public String AccountNumber { get; set; }
        public String OwnerCustomerId { get; set; }
        public String Name { get; set; }
        public String AccountType { get; set; }
        public double Available { get; set; }
        public double Balance { get; set; }
        public double CreditLimit { get; set; }
    }
}