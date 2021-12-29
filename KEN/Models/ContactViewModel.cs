using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class ContactViewModel
    {
        public int id { get; set; }
        public string DisplayContactId
        {
            get
            {
                string newId = "000000" + id;
                return newId.Substring(newId.Length - 6, 6);
            }
        }
        public Nullable<int> acct_manager_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public Nullable<short> send_email { get; set; }
        public string notes { get; set; }
        public string title { get; set; }
        public string company { get; set; }
        public string ContactType { get; set; }
        public string ContactRole { get; set; }
        public bool IsPrimary { get; set; }
        public string MainPhone { get; set; }
        public string AccountManagerFirstName { get; set; }

        public string AccountManagerLastName { get; set; }
        public string AccountManager { get; set; }
        public Nullable<int> OrgId { get; set; }

        public string OrgName { get; set; }
        public string PageSource { get; set; }

    }
    public class OppContactMappingViewModel
    {
        public int ContactID { get; set; }
        public int MappingID { get; set; }
        public bool isPrimary { get; set; }

        public bool IsLinked { get; set; }
    }
    public class DropDownViewModel
    {
        public int id { get; set; }
        public Nullable<int> acct_manager_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public Nullable<short> send_email { get; set; }
        public string notes { get; set; }
        public string title { get; set; }
        public string Company { get; set; }
        public string ContactType { get; set; }
        public string ContactRole { get; set; }
        public Nullable<int> OrgId { get; set; }
        //22 Aug 2018 (N)
        public string OrgName { get; set; }
        //22 Aug 2018 (N)

    }
    public class AccountManagerDropdownViewModel
    {
        public int id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string AccountManagerFullName
        {
            get
            {
                return firstname + " " + lastname;
            }
        }
    }
}