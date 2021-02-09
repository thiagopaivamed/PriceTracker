using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceTracker.Models
{
    public class ProductInformation
    {
        public int ProductInformationId { get; set; }

        public int ProductId { get; set; }

        public double Value { get; set; }

        public int Day { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }
    }
}
