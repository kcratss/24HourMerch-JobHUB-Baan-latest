using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class OptionViewModel
    {
        public int id { get; set; }

        public string DispalayId
        {
            get
            {
                string newId = "000000" + id;
                return newId.Substring(newId.Length - 6, 6);
            }
        }
        public int quantity { get; set; }
        public string code { get; set; }
        public int band_id { get; set; }
        // baans change 15th September
        public string Status { get; set; }
            // baans change 15th September
        public string BrandName { get; set; }
        public int item_id { get; set; }
        public string ItemName { get; set; }
        public string colour { get; set; }
        public string comment { get; set; }
        public string private_comment { get; set; }
        public Nullable<int> front_decoration { get; set; }
        public Nullable<int> back_decoration { get; set; }
        public Nullable<int> left_decoration { get; set; }
        public Nullable<int> right_decoration { get; set; }
        public Nullable<int> extra_decoration { get; set; }
        public Nullable<double> uni_price { get; set; }
        public Nullable<double> total { get; set; }
        public Nullable<bool> include_job { get; set; }
        public int job_id { get; set; }
        public Nullable<int> OpportunityId { get; set; }
        public string SizeGrid { get; set; }
        public string Link { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public Nullable<decimal> Margin { get; set; }
        public string InitialSizes { get; set; }
        public string SizesPacked { get; set; }
        public string OtherDesc { get; set; }
        public Nullable<decimal> OtherCost { get; set; }
        public string Front_decDesign { get; set; }
        public string Back_decDesign { get; set; }
        public string Left_decDesign { get; set; }
        public string Right_decDesign { get; set; }
        public string Extra_decDesign { get; set; }
        public string Front_decQuantity { get; set; }
        public string Back_decQuantity { get; set; }
        public string Left_decQuantity { get; set; }
        public string Right_decQuantity { get; set; }
        public string Extra_decQuantity { get; set; }
        public string Declined { get; set; }

        public string Front_decDesignName { get; set; }
        public string Back_decDesignName { get; set; }
        public string Left_decDesignName { get; set; }
        public string Right_decDesignName { get; set; }
        public string Extra_decDesignName { get; set; }
        public string include { get; set; }
     
        public string Service { get; set; }
        public Nullable<decimal> Front_decCost { get; set; }
        public Nullable<decimal> Back_decCost { get; set; }
        public Nullable<decimal> Left_decCost { get; set; }
        public Nullable<decimal> Right_decCost { get; set; }
        public Nullable<decimal> Extra_decCost { get; set; }
        public Nullable<decimal> UnitInclGST { get; set; }
        public Nullable<decimal> ExtExGST { get; set; }
        public Nullable<decimal> ExtInclGST { get; set; }
        public string OptionStage { get; set; }
        public Nullable<bool> ProofSent { get; set; }
        public Nullable<System.DateTime> ProofMailSent { get; set; }
        public Nullable<int> ProofVerion { get; set; }

    }
}