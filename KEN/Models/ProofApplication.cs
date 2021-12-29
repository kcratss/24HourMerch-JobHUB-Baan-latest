using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class ProofApplication
    {
        public int ApplicationId { get; set; }
        public string ApplicationLocation { get; set; }
        public string AppType { get; set; }
        public string AppWidth { get; set; }
        public string AppImage { get; set; }
        public string MockUpImage { get; set; }
        public List<ProofClours> Colours { get; set; }
    }
    public class ProofClours
    {
        public int ApplicationColourId { get; set; }
        public string InkColour { get; set; }
        public string ThreadColour { get; set; }
        public string TransferColour { get; set; }
        public string PantoneName { get; set; }
        public string HexvalueColour { get; set; }
    }
}