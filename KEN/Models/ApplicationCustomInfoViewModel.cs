using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class ApplicationCustomInfoViewModel
    {
        public int CustomInfoId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public Nullable<int> JersyNumber { get; set; }
        public string CustomNotes { get; set; }
        public string Garment { get; set; }
        public string Garmentcolour { get; set; }
        public string GarmentSize { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    }
}