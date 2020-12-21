using System;
using System.Collections.Generic;

namespace SbankenYnab.Models.Sbanken
{
    public class TransactionsList
    {
        public int AvailableItems { get; set; }
        public List<Transaction> Items { get; set; }
    }

    public class Transaction{
        public DateTime AccountingDate { get; set; }
        public double Amount { get; set; }
        public String Text { get; set; }
        public bool IsReservation { get; set; }
        public TransactionDetail TransactionDetail { get; set; }
    }

    public class TransactionDetail
    {
        public String Cid { get; set; }
        public String AmountDescription { get; set; }
        public String ReceiverName { get; set; }
        public String PayerName { get; set; }
        public String RegistrationDate { get; set; }
    }
}