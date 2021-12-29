using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class EmailContentViewModel
    {

        public int EmailContentId { get; set; }
        public string Purpose { get; set; }
        public string Subject { get; set; }
        public string Body1 { get; set; }
        public string Body2 { get; set; }
        public string Body3 { get; set; }
        public string ClientEmailID { get; set; }
    }
}