using System;
using System.Collections.Generic;

namespace SbankenYnab.Models
{
    public class BudgetSummaryResponse
    {
        public BudgetList Data { get; set; }
    }

    public class BudgetList
    {
        public List<Budget> Budgets { get; set; }
    }

    public class Budget
    {
        public String Id { get; set; }
        public String Name { get; set; }
    }
}