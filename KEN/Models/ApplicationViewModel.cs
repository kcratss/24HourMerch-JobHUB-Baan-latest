using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class ApplicationViewModel
    {
        public int ApplicationId { get; set; }
        public string AppName { get; set; }
        public Nullable<System.DateTime> DecorationDate { get; set; }
        public string AppType { get; set; }
        public string AppWidth { get; set; }
        public string Art { get; set; }
        public string ArtNotes { get; set; }
        public string Bill { get; set; }
        public string Production { get; set; }
        public string ProductionNotes { get; set; }
        public string ArtSupplier { get; set; }
        public string Designer { get; set; }
        public string DesignerNotes { get; set; }
        public string AppStatus { get; set; }
        public string ProofNotes { get; set; }
        public Nullable<int> AcctMgrId { get; set; }
        public string AppImage { get; set; }
        public string AppVector { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string SourceImage { get; set; }
        public string Link { get; set; }
        public string MockUpImage { get; set; }
    }
}