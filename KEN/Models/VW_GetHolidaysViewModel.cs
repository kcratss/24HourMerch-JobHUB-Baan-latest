using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class VW_GetHolidaysViewModel
    {
        public Nullable<System.DateTime> dayofyear { get; set; }
        public int id { get; set; }
        public Nullable<bool> Holiday { get; set; }
        public string Holiday_Desc { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }


        public string GetDayofyear
        {
            get
            {
                string GetDate = "";
                if (dayofyear != null)
                {
                    GetDate = Convert.ToDateTime(dayofyear).ToString("yyyy-MM-dd");
                }

                return GetDate;
            }
        }
    }
}