﻿using System.Collections.Generic;

namespace PotigianHH.Controllers.Model
{
    public class CloseRequestPayload
    {
        public IDictionary<string, int> ArticleCount { get; set; }

        public int Printer { get; set; }

        public int Bags { get; set; }
    }
}
