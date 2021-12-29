using KEN.Models;
using KEN_DataAccess;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KEN.AppCode;


namespace KEN.Reports
{
   
    public partial class ValueConversionReport : System.Web.UI.Page
    {
        KENNEWEntities dbContext = new KENNEWEntities();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string timeZoneId = "AUS Eastern Standard Time";

                DateTime now = DateTime.Now.ToUniversalTime();
                //now = now.AddDays(-1);

                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                var tdate = TimeZoneInfo.ConvertTimeFromUtc(now, tzi).ToString();

                todate.Text = Convert.ToDateTime(tdate).ToString("dd/MM/yyyy");

                var NewWholeToDate = tdate.Split(' ');
                var NewDateToGroup = NewWholeToDate[0].Split('/');
                tdate = NewDateToGroup[2] + "-" + NewDateToGroup[0] + "-" + NewDateToGroup[1];// + " " + NewWholeDate[1] + " " + NewWholeDate[2];

                DateTime FDate = DateTime.Now.ToUniversalTime();
                FDate = FDate.AddDays(-90);
                var fdate = TimeZoneInfo.ConvertTimeFromUtc(FDate, tzi).ToString();

                fromDate.Text = Convert.ToDateTime(fdate).ToString("dd/MM/yyyy");

                var NewWholeFromDate = fdate.Split(' ');
                var NewDateFromGroup = NewWholeFromDate[0].Split('/');
                fdate = NewDateFromGroup[2] + "-" + NewDateFromGroup[0] + "-" + NewDateFromGroup[1];// + " " + NewDateFromGroup[1] + " " + NewDateFromGroup[2];
                drdSource.DataSource = GetSource();
                drdSource.DataBind();
                drdSource.DataTextField = "Name";
                drdSource.DataValueField = "Name";
                drdSource.Items.Insert(0, new ListItem("ALL", "ALL"));
                drdSource.Items.Insert(1, new ListItem("New Business", "New Business"));

                var source = drdSource.SelectedValue;

                GeneratedReport(tdate, fdate, source);
            }
        }
        public void GeneratedReport(string tdate, string fdate, string source)
        {
            string timeZoneId = "AUS Eastern Standard Time";

            DateTime now = DateTime.Now.ToUniversalTime();

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var currdate = TimeZoneInfo.ConvertTimeFromUtc(now, tzi).ToString();

            var NewWholeDate = currdate.Split(' ');
            var NewDateGroup = NewWholeDate[0].Split('/');
            var designdate = NewDateGroup[1] + "/" + NewDateGroup[0] + "/" + NewDateGroup[2];


            ValueConversionReportViewer.Visible = true;
            ValueConversionReportViewer.LocalReport.Refresh();
            ValueConversionReportViewer.ProcessingMode = ProcessingMode.Local;

            var currentuser = DataBaseCon.ActiveUser();
            var ReportData = dbContext.Pro_ValueConversionReport(fdate, tdate, source).ToList();
          
            ValueConversionReportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/ValueConversionReport.rdlc");
            ValueConversionReportViewer.LocalReport.DataSources.Clear();

            ReportDataSource rds = new ReportDataSource("DataSet1", ReportData);

            var fromdate = fromDate.Text;
            var todte = todate.Text;
            var getSource = drdSource.SelectedValue;

            DateTime datetime = DateTime.Now.ToUniversalTime();
            TimeZoneInfo tzinfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var currentdate = TimeZoneInfo.ConvertTimeFromUtc(datetime, tzinfo).ToString();

            var NewWholeToDate = currentdate.Split(' ');
            var NewDateToGroup = NewWholeToDate[0].Split('/');
            currentdate = NewDateToGroup[1] + "/" + NewDateToGroup[0] + "/" + NewDateToGroup[2];

            ReportParameter sourceParameter = new ReportParameter("sourceParameter", getSource);
            ReportParameter currentdateParameter = new ReportParameter("currentdateParameter", currentdate);
            ReportParameter fromdateParameter = new ReportParameter("fromdate", fromdate);
            ReportParameter todateparameter = new ReportParameter("todate", todte);

            this.ValueConversionReportViewer.LocalReport.SetParameters(new ReportParameter[] { sourceParameter, currentdateParameter, fromdateParameter, todateparameter });


            ValueConversionReportViewer.LocalReport.DataSources.Add(rds);
            ValueConversionReportViewer.LocalReport.Refresh();
            ValueConversionReportViewer.DataBind();
        }
        protected void btnid_Click(object sender, EventArgs e)
        {
            var tdate = "";
            var newdate = todate.Text;
            var NewDateGroup = newdate.Split('/');
            tdate = NewDateGroup[2] + "-" + NewDateGroup[1] + "-" + NewDateGroup[0];

            var fDate = "";
            var newformDate = fromDate.Text;
            var NewFromDateGroup = newformDate.Split('/');
            fDate = NewFromDateGroup[2] + "-" + NewFromDateGroup[1] + "-" + NewFromDateGroup[0];

            var source = drdSource.SelectedValue;

            GeneratedReport(tdate, fDate, source);
        }
        public List<string> GetSource()
        {
            List<string> newdata = new List<string>();

            var getData = (from SourceEnum e in Enum.GetValues(typeof(SourceEnum))
                           select new { Name = e.ToString() }).ToList();

            for (int i = 0; i < getData.Count; i++)
            {
                newdata.Add(getData[i].Name);
            }
            return newdata;
        }

      
    }
}