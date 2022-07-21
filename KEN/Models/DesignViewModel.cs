using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class DesignViewModel
    {
        public int Id { get; set; }
        public string LogoUrl { get; set; }
        public String Name { get; set; }
        public int LogoId { get; set; }
        public int UserId { get; set; }
        public string StatusValue { get; set; }
        public string ProcessValue { get; set; }
        public string ColorValue { get; set; }
        public string SizeValue { get; set; }
        public Boolean Status { get; set; }
        public int Process_Id { get; set; }
        public int Color_Id { get; set; }
        public int Size_Id { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ApprovedLogo_UserId { get; set; }
        public string ApprovedLogo_UserName { get; set; }
        public string LogoCreateDateString
        {
            get
            {
                return CreatedOn.ToString("dd-MM-yyyy");
            }
            set { CreatedOn.ToString("dd-MM-yyyy"); }
        }
        public DateTime? ApprovedLogo_Date { get; set; }
        public string ApprovedLogoDateString
        {
            get
            {
                return ApprovedLogo_Date.HasValue ? ApprovedLogo_Date.Value.ToString("dd-MM-yyyy") : string.Empty;
            }
            set {}
        }
        public DateTime? RejectedLogo_Date { get; set; }
        public string RejectedLogoDateString
        {
            get
            {
                return RejectedLogo_Date.HasValue ? RejectedLogo_Date.Value.ToString("dd-MM-yyyy") : string.Empty;
            }
            set { }
        }

        public int RejectedLogo_UserId { get; set; }
        public string RejectedLogo_UserName { get; set; }
        public DateTime UploadLogoDate { get; set; }
        public string UserEmail { get; set; }
    }
}