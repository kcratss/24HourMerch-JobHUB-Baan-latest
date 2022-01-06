using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KEN.Models;
using KEN_DataAccess;
using KEN.AppCode;
using AutoMapper;
using Microsoft.Reporting.WebForms;

namespace KEN.Reports
{
    public partial class DailyPaymentReport : System.Web.UI.Page
    {
        KENNEWEntities dbContext = new KENNEWEntities();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string timeZoneId = "AUS Eastern Standard Time";

                DateTime now = DateTime.Now.ToUniversalTime();
                now = now.AddDays(-1);

                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                var date =  TimeZoneInfo.ConvertTimeFromUtc(now, tzi).ToString();

                txtdate.Text = Convert.ToDateTime(date).ToString("dd/MM/yyyy");

                //ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "getreport();", true);
                //var newdate = hdndate.Value;
                var NewWholeDate = date.Split(' ');
                var NewDateGroup = NewWholeDate[0].Split('/');
                //date = NewDateGroup[2]+ "-" + NewDateGroup[0]+ "-" + NewDateGroup[1] + " " + NewWholeDate[1] + " " + NewWholeDate[2];
                //Commented by Baans 11Sep2020
               // GeneratedReport(date);

               // GeneratedReport(Convert.ToDateTime(date).ToString("yyyy/mm/dd"));
                GeneratedReport(Convert.ToDateTime(date).ToString("dd/MM/yyyy"));
            }
        }

        public void GeneratedReport(string date)
        {
            string timeZoneId = "AUS Eastern Standard Time";

            DateTime now = DateTime.Now.ToUniversalTime();

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var currdate = TimeZoneInfo.ConvertTimeFromUtc(now, tzi).ToString();

            var NewWholeDate = currdate.Split(' ');
            var NewDateGroup = NewWholeDate[0].Split('/');
            var designdate = NewDateGroup[1] + "/" + NewDateGroup[0] + "/" + NewDateGroup[2];




            DailyPaymentReportReportViewer.Visible = true;
            DailyPaymentReportReportViewer.LocalReport.Refresh();
            DailyPaymentReportReportViewer.ProcessingMode = ProcessingMode.Local;


            var Payment = dbContext.Pro_PaymentReport(date).ToList();

            DailyPaymentReportReportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/DailyPaymentReport.rdlc");
            DailyPaymentReportReportViewer.LocalReport.DataSources.Clear();

            ReportDataSource rds = new ReportDataSource("DataSet1", Payment);
            var showdate = txtdate.Text; 
            ReportParameter rpt1 = new ReportParameter("ReportParameter1", showdate);
            ReportParameter rpt2 = new ReportParameter("designdate", designdate);

            this.DailyPaymentReportReportViewer.LocalReport.SetParameters(new ReportParameter[] { rpt1,rpt2 });

            DailyPaymentReportReportViewer.LocalReport.DataSources.Add(rds);
            DailyPaymentReportReportViewer.LocalReport.Refresh();
            DailyPaymentReportReportViewer.DataBind();
        } 
        public void btnid_Click(object sender, EventArgs e)
        {

            // ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "getreport();",true);

            // var date = hdndate.Value;
            //string timeZoneId = "AUS Eastern Standard Time";
            var date = "";
            var newdate = txtdate.Text;
            var NewDateGroup = newdate.Split('/');
            date = NewDateGroup[2] + "-" + NewDateGroup[1] + "-" + NewDateGroup[0];
            


            //DateTime Todata = Convert.ToDateTime(newdate.ToString("yyyy-MM-dd"));

            // TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            //DateTime date = TimeZoneInfo.ConvertTimeFromUtc(newdate, tzi);

            //txtdate.Text = Convert.ToDateTime(newdate).ToString("MM/dd/yyyy");

            GeneratedReport(date);
        }
    }
}