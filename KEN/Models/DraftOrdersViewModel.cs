using KEN_DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class DraftOrdersViewModel
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int Process { get; set; }
        public string ProcessValue { get; set; }
        public double Price { get; set; }
        public bool Status { get; set; }
        public DateTime OrderDate { get; set; }
        public double Gst { get; set; }
        public double Total { get; set; }
        public int UserId { get; set; }
        public int UserItemId { get; set; }

        public UserItemsViewModel tblUserItem { get; set; }
    }
}