using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class PrintColorViewModel
    {
        public int ColorId { get; set; }
      
        public string Name { get; set; }
    }
    public class SizeViewModel
    {
        public int SizeId { get; set; }

        public string Name { get; set; }
    }
    public class ProcessViewModel
    {
        public int ProcessId { get; set; }

        public string Name { get; set; }
    }
}