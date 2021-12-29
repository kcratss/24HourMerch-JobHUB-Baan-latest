using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class KanBanViewModel
    {
        public int KanbanId { get; set; }
        public Nullable<System.DateTime> ProductionDate { get; set; }
        public Nullable<int> OppId { get; set; }
        public Nullable<int> DeptId { get; set; }
        public Nullable<int> MachineNo { get; set; }
        public Nullable<int> Priority { get; set; }
        public string KanbanStatus
        {
            get
            {
                DateTime CurrentDate = DateTime.Now;
                string GetCurrentDate = Convert.ToString(CurrentDate).Split(' ')[0];
                string GetProductionDate = Convert.ToString(ProductionDate).Split(' ')[0];
                string status;
                if (GetProductionDate == GetCurrentDate)
                {
                    status = "Active";
                }
                else {
                    status = "InActive";
                }
                return status;
            }
        }

        public string NewProductionDate
        {
            get
            {
                string date = "";
                if (ProductionDate != null)
                {
                    date =Convert.ToDateTime( ProductionDate).ToString("dd/MM/yyyy");// Convert.ToString(ProductionDate).Split(' ')[0];
                }
                return date;
            }
        }
        public string OppName { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> ReqDate { get; set; }
        public string AccountManagerName { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string DeptName { get; set; }
        public Nullable<System.DateTime> ArtOrderedDate { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public Nullable<System.DateTime> ArtReadyDate { get; set; }
        public Nullable<System.DateTime> StockOrderedDate { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public Nullable<System.DateTime> Checkeddate { get; set; }
        public string OppThumbnail { get; set; }
        public Nullable<System.DateTime> ConfirmedDate { get; set; }

        public Nullable<int> Quantity { get; set; }
        public string FrontPrint { get; set; }
        public string BackCount { get; set; }
        public string LeftCount { get; set; }
        public string RightCount { get; set; }
        public string ExtraCount { get; set; }
        public Nullable<System.DateTime> DecoratedDate { get; set; }
    }

}