using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PotigianHH.Controllers.Model
{
    public class SynchronizeOrderResponse
    {
        public SynchronizeOrderResponse()
        {
            DetailInfo = new List<SynchronizeOrderDetail>();
        }

        public string HeaderMessage { get; set; }

        public int HeaderReturnCode { get; set; }

        public IList<SynchronizeOrderDetail> DetailInfo { get; set; }
    }

    public class SynchronizeOrderDetail
    {
        public decimal ArticleCode { get; set; }

        public string DetailMessage { get; set; }

        public int DetailReturnCode { get; set; }
    }
}
