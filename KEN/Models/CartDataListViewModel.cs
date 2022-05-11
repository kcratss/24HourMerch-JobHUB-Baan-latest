using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class CartDataListViewModel
    {
        public int Id { set; get; }
        public int Quantity { set; get; }
        public int TotalPrice { set; get; }
    }
}