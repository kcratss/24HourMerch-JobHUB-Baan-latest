using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class OrderAddressViewModel
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Address { set; get; }
        public int PostCode { set; get; }
        public string State { set; get; }
    }
}