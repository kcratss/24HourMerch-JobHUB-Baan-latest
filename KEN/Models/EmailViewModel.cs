using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class EmailViewModel
    {
        public string Email { get; set; }
        public string Subject { get; set; }

        public string MailMessage1 { get; set; }
        public string MailMessage2 { get; set; }
        public string MailMessage3 { get; set; }

        public string OptionStatus { get; set; }

        public string Type { get; set; }
        public Nullable <DateTime> ConfirmedDate { get; set; }
        public string Shipping { get; set; }

    }
}