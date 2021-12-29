using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class ApplicationColourViewModel
    {
        public int ApplicationColourId { get; set; }
        public Nullable<int> ColourWayNo { get; set; }
        public Nullable<int> InkId { get; set; }
        public string InkColour { get; set; }
        public Nullable<int> ThreadId { get; set; }
        public string ThreadColour { get; set; }
        public Nullable<int> Pantone { get; set; }
        public Nullable<int> PrintOrder { get; set; }
        public string Mesh { get; set; }
        public Nullable<bool> Flash { get; set; }
        public string TransferColour { get; set; }
        public string Substrate { get; set; }
        public string ColourNotes { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string GarmentColour { get; set; }
        public string PantoneName { get; set; }
        public string Bucket { get; set; }
        public string HexvalueColour { get; set; }
    }
}