using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class sizeType
    {
        public string typeName { get; set; }
        public string value { get; set; }
    }

    public class Size
    {
        public string size { get; set; }
        public int quantity { get; set; }
        public string sizeType { get; set; }
    }
}