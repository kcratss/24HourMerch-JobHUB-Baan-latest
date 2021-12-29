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
    public partial class SalesReport : System.Web.UI.Page
    {
        KENNEWEntities dbContext = new KENNEWEntities();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string timeZoneId = "AUS Eastern Standard Time";

                DateTime now = DateTime.Now.ToUniversalTime();

                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                var date = TimeZoneInfo.ConvertTimeFromUtc(now, tzi).ToString();

                txtfromdate.Text = Convert.ToDateTime(date).ToString("dd/MM/yyyy");
                txttodate.Text = Convert.ToDateTime(date).ToString("dd/MM/yyyy");

                //ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "getreport();", true);
                //var newdate = hdndate.Value;
                var NewWholeDate = date.Split(' ');
                var NewDateGroup = NewWholeDate[0].Split('/');
                // date = NewDateGroup[2] + "-" + NewDateGroup[0] + "-" + NewDateGroup[1] + " " + NewWholeDate[0] + " " + NewWholeDate[1];
                // GeneratedReport(date,date);

                GeneratedReport(Convert.ToDateTime(date).ToString("yyyy/MM/dd"), Convert.ToDateTime(date).ToString("yyyy/MM/dd"));


            }
        }

        public void GeneratedReport(string fromdate,string todate)
        {

            string timeZoneId = "AUS Eastern Standard Time";

            DateTime now = DateTime.Now.ToUniversalTime();

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var date = TimeZoneInfo.ConvertTimeFromUtc(now, tzi).ToString();
            
            var NewWholeDate = date.Split(' ');
            var NewDateGroup = NewWholeDate[0].Split('/');
            var designdate = NewDateGroup[1] + "/" + NewDateGroup[0] + "/" + NewDateGroup[2];
            


            SalesReportReportViewer.Visible = true;
            SalesReportReportViewer.LocalReport.Refresh();
            SalesReportReportViewer.ProcessingMode = ProcessingMode.Local;

            //date = date.Date;

            //string acf = Convert.ToDateTime(date);
            var Sales = dbContext.Pro_SalesReport(fromdate,todate).ToList();

            SalesReportReportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/SalesReport.rdlc");
            SalesReportReportViewer.LocalReport.DataSources.Clear();

            ReportDataSource rds = new ReportDataSource("SalesReport", Sales);
            var boxfromdate = txtfromdate.Text;
            var boxtodate = txttodate.Text;
            ReportParameter rpt1 = new ReportParameter("ParameterFromDate", boxfromdate);
            ReportParameter rpt2 = new ReportParameter("ParameterToDate", boxtodate);
            ReportParameter rpt3 = new ReportParameter("DesignDate", designdate);

            this.SalesReportReportViewer.LocalReport.SetParameters(new ReportParameter[] { rpt1,rpt2,rpt3 });

            SalesReportReportViewer.LocalReport.DataSources.Add(rds);
            SalesReportReportViewer.LocalReport.Refresh();
            SalesReportReportViewer.DataBind();
        }

        protected void btnsubmit_Click(object sender, EventArgs e)
        {
            var fromdate = txtfromdate.Text;
            var newfromdate = getdate(fromdate);

            var todate = txttodate.Text;
            var newtodate = getdate(todate);

            GeneratedReport(newfromdate, newtodate);
        }

        public static string getdate(string date)
        {
            var NewDateGroup = date.Split('/');
            var FormattedDate = NewDateGroup[2] + "-" + NewDateGroup[1] + "-" + NewDateGroup[0];
            return FormattedDate;
        }
    }
}