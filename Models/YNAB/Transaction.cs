using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SbankenYnab.Models.YNAB
{
    /// <summary>
    /// This class is in snake case since that's what the YNAB API expects
    /// </summary>
    public class Transaction
    {
        public String account_id { get; set; }
        public String date { get; set; }
        public int amount { get; set; }
        public String payee_name { get; set; }
        public String memo { get; set; }
        public String cleared { get; set; }
        public bool approved { get; set; }
        public String import_id { get; set; }
    }
}