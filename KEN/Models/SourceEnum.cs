using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public enum SourceEnum
    {
        PhoneCall = 1,
        Email = 2,
        WebForm = 3,
        ShowRoomVisit = 4,
        Referral = 5,
        Repeat = 6,
        WebOrder = 7,
        // Baans change 13th October for Old Database
        Z_OldData = 8
        // baans end 13th October
    }
    public enum Stages
    {
        Opportunity = 1,
        Quote = 2,
        Order = 3,
        Job = 4,
        //Confirmed=5,
        //Ordered=6,
        //Approved,
        Packing = 5,
        Invoicing = 6,
        Shipping = 7,
        Complete = 8
    }
    public enum Cycles
    {
        Monthly = 1,
        Quarterly = 2,
        HalfYearly = 3,
        Annually = 4,
        BiAnnually = 5,
        Other = 6
    }
    public enum ContactType
    {
        Lead = 1,
        Prospect = 2,
        Customer = 3,
        Supplier = 4,
        Influencer = 5,
        Friend = 6,
        Team = 7,
        JobHub24Hour = 8  //tarun 26/09/2018
    }
    public enum PurchaseStatus
    {
        Ordered = 1,
        Received = 2,
        Checked = 3,
        Billed = 4,
        Deleted = 5
         //Tarun
    }
    //public enum ContactRole
    //{
    //    Accounts = 1,
    //    CEO = 2,
    //    Designer = 3,
    //    Manager = 4,
    //    Primary = 5,
    //    Warehouse = 6,
    //    Other = 7
    //}

    public class ResponseMessage
    {
        public const string ErrorMessage = "Something went wrong";
        public const string SuccessMessage = "Data saved successfully";

    }
    public class ResponseCode
    {
        public const string OK = "200";
        public const string Error = "400";
        public const string NotFound = "404";
        public const string Warning = "401";
    }
    public class ResponseType

    {
        public const string Error = "Error";
        public const string Warning = "Warning";
        public const string Success = "Success";
    }
    public class TableNames

    {
        public const string tblOpportunity = "Opportunity";
        public const string tblContact = "Contact";
        public const string tblOption = "Option";
        public const string tblAddress = "Address";
        public const string tblOrganisation = "Organisation";
        public const string tblEvent = "Event";
        public const string tblOppContactMapping = "Contact Mapping";
        public const string tblInquiry = "Inquiry";
        public const string tblPurchase = "Purchase";     //Tarun
        public const string tblApplication = "Application";
        public const string tblApplictaionColours = "Application Colours";
        public const string ApplicationCustom = "Custom Info";
    }
    public enum ContactTitle
    {
        Mr = 1,
        Ms = 2,
        Mrs = 3
    }
    public enum Shipping
    {
        Auspost = 1,
        Courier = 2,
        Express = 3,
        Post = 4,
        Personal = 5,
        PickUp = 6,
        Satchel = 7,
        Standard = 8,
        Uber = 9,
        // baans change 13th October for Old database
        Z_OldData = 10
        // baans end 13th October
    }
    public enum PaymentMethod
    {
        EFT = 1,
        CHEQUE = 2,
        CASH = 3,
        VISA = 4,
        MASTERCARD = 5,
        PAYPAL = 6
        
    }
    public enum ApplicaionProduction
    {
        Internal = 1,
        External = 2    //by sandeep (24Sept)
    }


}