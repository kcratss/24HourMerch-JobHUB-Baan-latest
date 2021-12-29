using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class UserViewModels
    {
        public int id { get; set; }
        public string username { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string hashed_password { get; set; }
        public string email { get; set; }
        public string status { get; set; }
        public Nullable<bool> admin { get; set; }
        public Nullable<System.DateTime> created { get; set; }
        public string userpic { get; set; }
        public string title { get; set; }
        public string access { get; set; }
        public string last_active { get; set; }
        public string last_login { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UserRole { get; set; }
    }
}