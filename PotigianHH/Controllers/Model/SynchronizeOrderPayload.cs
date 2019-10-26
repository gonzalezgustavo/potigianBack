using System;
using System.Collections.Generic;

namespace PotigianHH.Controllers.Model
{
    public class SynchronizeOrderPayload
    {
        public OrderAdditionalInfo OrderAdditionalInfo { get; set; }

        public Dictionary<decimal?, int> ArticleCount { get; set; }
    }

    public class OrderAdditionalInfo
    {
        public int BillType { get; set; }

        public decimal PrefixOc { get; set; }

        public decimal SuffixOc { get; set; }

        public string Observations { get; set; }

        public DateTime Date { get; set; }

        public string User { get; set; }

        public string Terminal { get; set; }

        public decimal Branch { get; set; }
    }
}
