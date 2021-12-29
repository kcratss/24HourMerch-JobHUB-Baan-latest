using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class OrganisationViewModel
    {
        public int OrgId { get; set; }

        public string DisplayOrganisationId
        {
            get
            {
                string newId = "000000" + OrgId;
                return newId.Substring(newId.Length - 6, 6);
            }
        }
        public string OrgName { get; set; }

        public string TradingName { get; set; }

        public string Brand { get; set; }

        public string ABN { get; set; }
        public string MainPhone { get; set; }
        public string WebAddress { get; set; }

        public string OrgNotes { get; set; }

        public string AccountManagerFirstName { get; set; }

        public string ContactType { get; set; }

        public string AccountManagerLastName { get; set; }

        public string AccountManager
        {
            get
            {
                return AccountManagerFirstName + " " + AccountManagerLastName;
            }
        }
        public string PageSource { get; set; }
        public int ContactID { get; set; }
        public Nullable<int> AcctMgrId { get; set; }
        public string OrganisationType { get; set; }

        public int PurchaseId { get; set; }         //1 Sep 2018 (N)

        public Nullable<int> IntuitID { get; set; }         //27 Dec 2018 (N)
        public string EmailAddress { get; set; }

    }
    public class AddressViewModel
    {
        public int AddressId { get; set; }
        public string TradingName { get; set; }
        public string Attention { get; set; }
        public string DeliveryAddress { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Postcode { get; set; }
        public string AddNotes { get; set; }
        public Nullable<int> OrgId { get; set; }

        public string PageSource { get; set; }
        public int PurchaseId { get; set; }
        public int OppId { get; set; }

    }

    public class StateList
    {
        public string stateName { get; set; }
    }
}