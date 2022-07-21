using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class QuotesViewModel
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public int TotalItems { get; set; }
        public Decimal TotalPrice { get; set; }
    }
    public class CalculateViewModel
    {        
        public int quantity { get; set; }
        public Decimal Print_Price { get; set; }   
    }
}