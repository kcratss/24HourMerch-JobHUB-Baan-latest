using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class ResponseViewModel
    {
        public string Message { get; set; }
        public int ID { get; set; }

        public string Result { get; set; }
        public int ErrorCode { get; set; }
        public string tblName { get; set; }
    }
    public class StageChangeResponseViewModel
    {
        public ResponseViewModel response { get; set; }
        public DateTime ChangeDate { get; set; }
    }

    public class ImageResponseViewModel
    {
        public string Message { get; set; }
        public int ID { get; set; }

        public string FileName { get; set; }
        public string Result { get; set; }
    }
}