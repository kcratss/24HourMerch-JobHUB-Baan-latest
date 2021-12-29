using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Web.UI;
using System.Configuration;
using System.Web;
using Intuit.Ipp.OAuth2PlatformClient;
using System.Threading.Tasks;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.LinqExtender;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;
using Intuit.Ipp.Exception;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Web.Mvc;
using KEN_DataAccess;
using AutoMapper;
using KEN.Filters;
using KEN.Interfaces.Iservices;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;

namespace KEN.Controllers
{
    [UserAuthenticationFilter]
    public class QuickBooksController : Controller
    {
        public static String REQUEST_TOKEN_URL = ConfigurationManager.AppSettings["GET_REQUEST_TOKEN"];
        public static String ACCESS_TOKEN_URL = ConfigurationManager.AppSettings["GET_ACCESS_TOKEN"];
        public static String AUTHORIZE_URL = ConfigurationManager.AppSettings["AuthorizeUrl"];
        public static String OAUTH_URL = ConfigurationManager.AppSettings["OauthLink"];
        public String consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
        public String consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
        public static string strrequestToken = string.Empty;
        public static string tokenSecret = string.Empty;
        public string oauth_callback_url = ConfigurationManager.AppSettings["oauth_callback_url"];
        public string GrantUrl = ConfigurationManager.AppSettings["GrantUrl"];
        public static Dictionary<string, string> dictionary = new Dictionary<string, string>();
        KENNEWEntities DbContext = new KENNEWEntities();
        private readonly IQuickBooksService _baseService;
        public QuickBooksController(IQuickBooksService baseService)
        {
            _baseService = baseService;
        }
        // GET: QuickBooks
        public  ActionResult QuickBooksDetails()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            dictionary.Clear();

            Session.Clear();
            Session.Abandon();
            Request.GetOwinContext().Authentication.SignOut("Cookies");
            try
            {
                if (!dictionary.ContainsKey("Access_Token") || !dictionary.ContainsKey("Access_TokenSecret"))
                {
                    var AuthData = _baseService.AuthdataListByDesc();
                    foreach (var item in AuthData)
                    {
                        if(item.FieldName== "Access_Token")
                        {
                            if(!dictionary.ContainsKey("Access_Token"))
                            dictionary.Add("Access_Token", item.FieldValue);
                        }
                        if (item.FieldName == "Access_TokenSecret")
                        {
                            if (!dictionary.ContainsKey("Access_TokenSecret"))
                                dictionary.Add("Access_TokenSecret", item.FieldValue);
                        }
                        if (item.FieldName == "realmID")
                        {
                            if (!dictionary.ContainsKey("realmID"))
                                dictionary.Add("realmID", item.FieldValue);
                        }

                    }
                    getAuthenticate();
                }
                else
                {
                    getAuthenticate();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Unauthorized-401")
                {

                    IOAuthSession session = CreateSession();
                    IToken requestToken = session.GetRequestToken();
                    strrequestToken = requestToken.Token;
                    tokenSecret = requestToken.TokenSecret;
                    tokenSecret = requestToken.TokenSecret;
                    var authUrl = string.Format("{0}?oauth_token={1}&oauth_callback={2}", AUTHORIZE_URL, requestToken.Token, UriUtility.UrlEncode(oauth_callback_url));
                    OAUTH_URL = authUrl;
                    return Redirect(authUrl);
                }
                else
                {

                }
            }

            return View();
        }
        protected IOAuthSession CreateSession()
        {
            var consumerContext = new OAuthConsumerContext
            {
                ConsumerKey =consumerKey ,
                ConsumerSecret = consumerSecret,
                SignatureMethod = SignatureMethod.HmacSha1
            };
            return new OAuthSession(consumerContext,
                                    REQUEST_TOKEN_URL,
                                    OAUTH_URL,
                                    ACCESS_TOKEN_URL);
        }
        public ActionResult Callback(string state, string code, string realmId)
        {
            if (Request.QueryString.Count > 0)
            {
                
                IOAuthSession clientSession = CreateSession();
                IToken requestToken = clientSession.GetRequestToken();
                requestToken.Token = strrequestToken;
                requestToken.TokenSecret = tokenSecret;
                IToken accessToken = clientSession.ExchangeRequestTokenForAccessToken(requestToken, Request.QueryString["oauth_verifier"].ToString());
                UpdateTOken(accessToken);
                _baseService.UpdateTOken(realmId, "realmID");
                OAuthRequestValidator oauthValidator = new OAuthRequestValidator(accessToken.Token, accessToken.TokenSecret, consumerKey, consumerSecret);
                ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            }
            return RedirectToAction("QuickBooksDetails", "QuickBooks");
        }
        public  ActionResult UpdateTOken(IToken tokenResp)
        {
            
            dictionary["Access_TokenSecret"] = tokenResp.TokenSecret;
            dictionary["Access_Token"] = tokenResp.Token;
            _baseService.UpdateTOken(tokenResp.Token, "Access_Token");
            _baseService.UpdateTOken(tokenResp.TokenSecret, "Access_TokenSecret");
            return RedirectToAction("QuickBooksDetails", "QuickBooks");

        }
        public void addInvoice(ServiceContext serviceContext,DataService dataService)
        {
            QueryService<Customer> customerQueryService = new QueryService<Customer>(serviceContext);
            var customer = customerQueryService.ExecuteIdsQuery("Select * From Customer where DisplayName = 'Oraganization 1'").FirstOrDefault();
            //QueryService customerQueryService = new QueryService(context);
            //Customer customer = customerQueryService.ExecuteIdsQuery("Select * From Customer StartPosition 1 MaxResults 1").FirstOrDefault();

            //Find Tax Code for Invoice - Searching for a tax code named 'StateSalesTax' in this example
            QueryService<TaxCode> stateTaxCodeQueryService = new QueryService<TaxCode>(serviceContext);
            TaxCode stateTaxCode = stateTaxCodeQueryService.ExecuteIdsQuery("Select * From TaxCode Where Name='StateSalesTax' StartPosition 1 MaxResults 1").FirstOrDefault();
            //QueryService stateTaxCodeQueryService = new QueryService(context);
            //TaxCode stateTaxCode = stateTaxCodeQueryService.ExecuteIdsQuery("Select * From TaxCode Where Name='StateSalesTax' StartPosition 1 MaxResults 1").FirstOrDefault();

            //Find Account - Accounts Receivable account required
            QueryService<Account> accountQueryService = new QueryService<Account>(serviceContext);
            Account account = accountQueryService.ExecuteIdsQuery("Select * From Account Where AccountType='Accounts Receivable' StartPosition 1 MaxResults 1").FirstOrDefault();

            //Find Item
            QueryService<Item> itemQueryService = new QueryService<Item>(serviceContext);
            Item item = itemQueryService.ExecuteIdsQuery("Select * From Item StartPosition 1 MaxResults 1").FirstOrDefault();

            //Find Term
            QueryService<Term> termQueryService = new QueryService<Term>(serviceContext);
            Term term = termQueryService.ExecuteIdsQuery("Select * From Term StartPosition 1 MaxResults 1").FirstOrDefault();


            Invoice invoice = new Invoice();

            //DocNumber - QBO Only, otherwise use DocNumber
            invoice.AutoDocNumber = true;
            invoice.AutoDocNumberSpecified = true;

            //TxnDate
            invoice.TxnDate = DateTime.Now.Date;
            invoice.TxnDateSpecified = true;

            //PrivateNote
            invoice.PrivateNote = "This is a private note";

            //Line
            Line invoiceLine = new Line();
            //Line Description
            invoiceLine.Description = "Invoice line description.";
            //Line Amount
            invoiceLine.Amount = 330m;
            invoiceLine.AmountSpecified = true;
            //Line Detail Type
            invoiceLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
            invoiceLine.DetailTypeSpecified = true;
            //Line Sales Item Line Detail
            SalesItemLineDetail lineSalesItemLineDetail = new SalesItemLineDetail();
            //Line Sales Item Line Detail - ItemRef
            lineSalesItemLineDetail.ItemRef = new ReferenceType()
            {
                name = item.Name,
                Value = item.Id
            };
            //Line Sales Item Line Detail - UnitPrice
            lineSalesItemLineDetail.AnyIntuitObject = 33m;
            lineSalesItemLineDetail.ItemElementName = ItemChoiceType.UnitPrice;
            //Line Sales Item Line Detail - Qty
            lineSalesItemLineDetail.Qty = 10;
            lineSalesItemLineDetail.QtySpecified = true;
            //Line Sales Item Line Detail - TaxCodeRef
            //For US companies, this can be 'TAX' or 'NON'
            lineSalesItemLineDetail.TaxCodeRef = new ReferenceType()
            {
                Value = "TAX"
            };
            //Line Sales Item Line Detail - ServiceDate 
            lineSalesItemLineDetail.ServiceDate = DateTime.Now.Date;
            lineSalesItemLineDetail.ServiceDateSpecified = true;
            //Assign Sales Item Line Detail to Line Item
            invoiceLine.AnyIntuitObject = lineSalesItemLineDetail;
            //Assign Line Item to Invoice
            invoice.Line = new Line[] { invoiceLine };

            //TxnTaxDetail
            TxnTaxDetail txnTaxDetail = new TxnTaxDetail();
            txnTaxDetail.TxnTaxCodeRef = new ReferenceType()
            {
                name = stateTaxCode.Name,
                Value = stateTaxCode.Id
            };
            Line taxLine = new Line();
            taxLine.DetailType = LineDetailTypeEnum.TaxLineDetail;
            TaxLineDetail taxLineDetail = new TaxLineDetail();
            //Assigning the fist Tax Rate in this Tax Code
            taxLineDetail.TaxRateRef = stateTaxCode.SalesTaxRateList.TaxRateDetail[0].TaxRateRef;
            taxLine.AnyIntuitObject = taxLineDetail;
            txnTaxDetail.TaxLine = new Line[] { taxLine };
            invoice.TxnTaxDetail = txnTaxDetail;

            //Customer (Client)
            invoice.CustomerRef = new ReferenceType()
            {
                name = customer.DisplayName,
                Value = customer.Id
            };

            //Billing Address
            PhysicalAddress billAddr = new PhysicalAddress();
            billAddr.Line1 = "123 Main St.";
            billAddr.Line2 = "Unit 506";
            billAddr.City = "Brockton";
            billAddr.CountrySubDivisionCode = "MA";
            billAddr.Country = "United States";
            billAddr.PostalCode = "02301";
            billAddr.Note = "Billing Address Note";
            invoice.BillAddr = billAddr;

            //Shipping Address
            PhysicalAddress shipAddr = new PhysicalAddress();
            shipAddr.Line1 = "100 Fifth Ave.";
            shipAddr.City = "Waltham";
            shipAddr.CountrySubDivisionCode = "MA";
            shipAddr.Country = "United States";
            shipAddr.PostalCode = "02452";
            shipAddr.Note = "Shipping Address Note";
            invoice.ShipAddr = shipAddr;

            //SalesTermRef
            invoice.SalesTermRef = new ReferenceType()
            {
                name = term.Name,
                Value = term.Id
            };

            //DueDate
            invoice.DueDate = DateTime.Now.AddDays(30).Date;
            invoice.DueDateSpecified = true;

            //ARAccountRef
            invoice.ARAccountRef = new ReferenceType()
            {
                name = account.Name,
                Value = account.Id
            };

            Invoice invoiceAdded = dataService.Add(invoice);
        }
        public void getAuthenticate()
        {
            
            string realmId = dictionary["realmID"].ToString();
            string accessToken = dictionary["Access_Token"].ToString();
            string accessTokenSecret = dictionary["Access_TokenSecret"].ToString();

            OAuthRequestValidator oauthValidator = new OAuthRequestValidator(accessToken, accessTokenSecret, consumerKey, consumerSecret);

            ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            //serviceContext.IppConfiguration.BaseUrl.Qbo = "https://sandbox-quickbooks.api.intuit.com/";
            serviceContext.IppConfiguration.BaseUrl.Qbo = "https://quickbooks.api.intuit.com/";

            serviceContext.IppConfiguration.Message.Request.SerializationFormat = Intuit.Ipp.Core.Configuration.SerializationFormat.Xml;
            serviceContext.IppConfiguration.Message.Response.SerializationFormat = Intuit.Ipp.Core.Configuration.SerializationFormat.Xml;
            serviceContext.IppConfiguration.MinorVersion.Qbo = "11";
            DataService commonServiceQBO = new DataService(serviceContext);

            // addInvoice(serviceContext, commonServiceQBO);
            QueryService<Invoice> invoiceQueryService = new QueryService<Invoice>(serviceContext);
            var invoice1 = invoiceQueryService.ExecuteIdsQuery("Select * From Invoice").FirstOrDefault();
            invoice1.Balance = 200;
            invoice1 = commonServiceQBO.Update(invoice1);
            QueryService<Customer> customerQueryService = new QueryService<Customer>(serviceContext);
            var customer = customerQueryService.ExecuteIdsQuery("Select * From Customer").FirstOrDefault();
            //QueryService customerQueryService = new QueryService(context);
            //Customer customer = customerQueryService.ExecuteIdsQuery("Select * From Customer StartPosition 1 MaxResults 1").FirstOrDefault();
            //customer.Active =true;
            //customer= commonServiceQBO.Update(customer);
            //Find Tax Code for Invoice - Searching for a tax code named 'StateSalesTax' in this example
            QueryService<TaxCode> stateTaxCodeQueryService = new QueryService<TaxCode>(serviceContext);
            TaxCode stateTaxCode = stateTaxCodeQueryService.ExecuteIdsQuery("Select * From TaxCode").FirstOrDefault();
            //QueryService stateTaxCodeQueryService = new QueryService(context);
            //TaxCode stateTaxCode = stateTaxCodeQueryService.ExecuteIdsQuery("Select * From TaxCode Where Name='StateSalesTax' StartPosition 1 MaxResults 1").FirstOrDefault();

            //Find Account - Accounts Receivable account required
            QueryService<Account> accountQueryService = new QueryService<Account>(serviceContext);
            Account account = accountQueryService.ExecuteIdsQuery("Select * From Account Where AccountType='Accounts Receivable' StartPosition 1 MaxResults 1").FirstOrDefault();

            //Find Item
            QueryService<Item> itemQueryService = new QueryService<Item>(serviceContext);
            Item item = itemQueryService.ExecuteIdsQuery("Select * From Item StartPosition 1 MaxResults 1").FirstOrDefault();

            //Find Term
            QueryService<Term> termQueryService = new QueryService<Term>(serviceContext);
            Term term = termQueryService.ExecuteIdsQuery("Select * From Term StartPosition 1 MaxResults 1").FirstOrDefault();




            Invoice invoice = new Invoice();

            //DocNumber - QBO Only, otherwise use DocNumber
            invoice.AutoDocNumber = true;
            invoice.AutoDocNumberSpecified = true;

            //TxnDate
            invoice.TxnDate = DateTime.Now.Date;
            invoice.TxnDateSpecified = true;

            //PrivateNote
            invoice.PrivateNote = "This is a private note";

            //Line
            Line invoiceLine = new Line();
            //Line Description
            invoiceLine.Description = "Invoice line description.";
            //Line Amount
            invoiceLine.Amount = 330m;
            invoiceLine.AmountSpecified = true;
            //Line Detail Type
            invoiceLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
            invoiceLine.DetailTypeSpecified = true;
            //Line Sales Item Line Detail
            SalesItemLineDetail lineSalesItemLineDetail = new SalesItemLineDetail();
            //Line Sales Item Line Detail - ItemRef
            lineSalesItemLineDetail.ItemRef = new ReferenceType()
            {
                name = item.Name,
                Value = item.Id
            };
            //Line Sales Item Line Detail - UnitPrice
            lineSalesItemLineDetail.AnyIntuitObject = 33m;
            lineSalesItemLineDetail.ItemElementName = ItemChoiceType.UnitPrice;
            //Line Sales Item Line Detail - Qty
            lineSalesItemLineDetail.Qty = 10;
            lineSalesItemLineDetail.QtySpecified = true;
            //Line Sales Item Line Detail - TaxCodeRef
            //For US companies, this can be 'TAX' or 'NON'
            lineSalesItemLineDetail.TaxCodeRef = new ReferenceType()
            {
                Value = "TAX"
            };
            //Line Sales Item Line Detail - ServiceDate 
            lineSalesItemLineDetail.ServiceDate = DateTime.Now.Date;
            lineSalesItemLineDetail.ServiceDateSpecified = true;
            //Assign Sales Item Line Detail to Line Item
            invoiceLine.AnyIntuitObject = lineSalesItemLineDetail;
            //Assign Line Item to Invoice
            invoice.Line = new Line[] { invoiceLine };

            //TxnTaxDetail
            TxnTaxDetail txnTaxDetail = new TxnTaxDetail();
            txnTaxDetail.TxnTaxCodeRef = new ReferenceType()
            {
                name = stateTaxCode.Name,
                Value = stateTaxCode.Id
            };
            Line taxLine = new Line();
            taxLine.DetailType = LineDetailTypeEnum.TaxLineDetail;
            TaxLineDetail taxLineDetail = new TaxLineDetail();
            //Assigning the fist Tax Rate in this Tax Code
            taxLineDetail.TaxRateRef = stateTaxCode.SalesTaxRateList.TaxRateDetail[0].TaxRateRef;
            taxLine.AnyIntuitObject = taxLineDetail;
            txnTaxDetail.TaxLine = new Line[] { taxLine };
            invoice.TxnTaxDetail = txnTaxDetail;

            //Customer (Client)
            invoice.CustomerRef = new ReferenceType()
            {
                name = customer.DisplayName,
                Value = customer.Id
            };

            //Billing Address
            PhysicalAddress billAddr = new PhysicalAddress();
            billAddr.Line1 = "123 Main St.";
            billAddr.Line2 = "Unit 506";
            billAddr.City = "Brockton";
            billAddr.CountrySubDivisionCode = "MA";
            billAddr.Country = "United States";
            billAddr.PostalCode = "02301";
            billAddr.Note = "Billing Address Note";
            invoice.BillAddr = billAddr;

            //Shipping Address
            PhysicalAddress shipAddr = new PhysicalAddress();
            shipAddr.Line1 = "100 Fifth Ave.";
            shipAddr.City = "Waltham";
            shipAddr.CountrySubDivisionCode = "MA";
            shipAddr.Country = "United States";
            shipAddr.PostalCode = "02452";
            shipAddr.Note = "Shipping Address Note";
            invoice.ShipAddr = shipAddr;

            //SalesTermRef
            invoice.SalesTermRef = new ReferenceType()
            {
                name = term.Name,
                Value = term.Id
            };

            //DueDate
            invoice.DueDate = DateTime.Now.AddDays(30).Date;
            invoice.DueDateSpecified = true;

            //ARAccountRef
            invoice.ARAccountRef = new ReferenceType()
            {
                name = account.Name,
                Value = account.Id
            };

            Invoice invoiceAdded = commonServiceQBO.Add(invoice);

















        }
    }
}