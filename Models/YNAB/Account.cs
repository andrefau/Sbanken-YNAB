using System;
using System.Collections.Generic;

namespace SbankenYnab.Models.YNAB
{
    public class AccountResponse
    {
        public AccountList Data { get; set; }
    }

    public class AccountList
    {
        public List<Account> Accounts { get; set; }
    }

    public class Account
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public int Balance { get; set; }
    }
}