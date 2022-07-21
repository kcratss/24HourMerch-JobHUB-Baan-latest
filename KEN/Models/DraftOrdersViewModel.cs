using KEN_DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class DraftOrdersItemViewModel
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int Process { get; set; }
        public string ProcessValue { get; set; }
        public double LogoPrice { get; set; }
        public double Unit_Price { get; set; }
        public int OrderId { get; set; }
        public double Gst { get; set; }
        public double Total_Price { get; set; }
        public double Tshirt_Price { get; set; }
        public int UserId { get; set; }
        public int UserItemId { get; set; }
        public int QuotesItem_Id { get; set; }
        public int Color { get; set; }
        public int Stitches { get; set; }
        public string ColorValue { get; set; }
        public string StitchesValue { get; set; }
        public string FrontImage { get; set; }
      
    }
}