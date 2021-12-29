<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoicedReport.aspx.cs" Inherits="KEN.Reports.InvoicedReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken =89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <link href="../Content/css/libraries/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="http://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css" />
    <link href="../Content/css/libraries/AdminLTE.css" rel="stylesheet" type="text/css" />
    <link href="../Content/css/libraries/w3.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="~/Content/jquery-ui.css" />
    
    <script src="../Content/js/libraries/jquery.min.js"></script>
    <script src="../Content/js/libraries/jquery-ui.min.js"></script>

    <script src="../Content/js/libraries/bootstrap.min.js"></script>
    <script src="../Content/js/libraries/bootbox.min.js"></script>
    <script src="../Content/js/libraries/inputmask.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/chosen/1.5.1/chosen.jquery.min.js"></script>


    <script>
        $(function () {
            $("#txtfromdate").datepicker({
                dateFormat: "dd/mm/yy"
            });

            $("#txttodate").datepicker({
                dateFormat: "dd/mm/yy"
            });

            $(".DateType input[type='checkbox']").on("change", function () {
                $(".DateType input[type='checkbox']").not(this).attr('checked', false);

                if (this.id == "today" && $(this).val() == "on") {
                    var curr = new Date;
                    var firstday = curr;
                    var lastday = curr;
                }

                if (this.id == "Week" && $(this).val() == "on") {
                    var curr = new Date; // get current date
                    var first = curr.getDate() - curr.getDay(); // First day is the day of the month - the day of the week
                    var last = first + 6; // last day is the first day + 6

                    var firstday = new Date(curr.setDate(first));
                    var lastday = new Date(curr.setDate(last));
                
                    //var start = GetFormattedDate(firstday);
                    //var end = GetFormattedDate(lastday);

                    //$("#txtfromdate").val(start);
                    //$("#txttodate").val(end);
                }

                if (this.id == "Month" && $(this).val() == "on") {
                    var curr = new Date();
                    var firstday = new Date(curr.getFullYear(), curr.getMonth(), 1);
                    var lastday = new Date(curr.getFullYear(), curr.getMonth() + 1, 0);

                    //var start = GetFormattedDate(firstday);
                    //var end = GetFormattedDate(lastday);

                    //$("#txtfromdate").val(start);
                    //$("#txttodate").val(end);
                }

                if (this.id == "year"&& $(this).val() == "on") {

                    var firstday = new Date(new Date().getFullYear(), 0, 1);
                    var lastday = new Date(new Date().getFullYear(), 12, 0);
                }

                if (this.id == "quarter" && $(this).val() == "on") {
                    var curr = new Date;
                    var month = curr.getMonth();
                    
                    if (month == 1 || month == 2 || month == 3) {
                        var firstday = new Date(new Date().getFullYear(), 0, 1);
                        var lastday = new Date(new Date().getFullYear(), 3, 0);
                    }
                    if (month == 4 || month == 5 || month == 6) {
                        var firstday = new Date(new Date().getFullYear(), 3, 1);
                        var lastday = new Date(new Date().getFullYear(), 6, 0);
                    }
                    if (month == 7 || month == 8 || month == 9) {
                        var firstday = new Date(new Date().getFullYear(), 6, 1);
                        var lastday = new Date(new Date().getFullYear(), 9, 0);
                    }
                    if (month == 10 || month == 11 || month == 12) {
                        var firstday = new Date(new Date().getFullYear(), 9, 1);
                        var lastday = new Date(new Date().getFullYear(), 12, 0);
                    }
                }

                var start = GetFormattedDate(firstday);
                var end = GetFormattedDate(lastday);

                $("#txtfromdate").val(start);
                $("#txttodate").val(end);

                
                
            });

            function GetFormattedDate(date) {
                return date.getDate() + '/' + ('00' + (date.getMonth() + 1)).substr(-2) + '/' + date.getFullYear();    //tarun 22/09/2018
            }

        });
    </script>

    <style>
        .btnRefresh{
            border-color: #ddd;
    top: 0px;
    float: left;
    padding: 5px 0px 0px 13px;
    width: 74px;
    margin-top: 5px;
    height: 31px;
    background: inherit;
    background-color: rgba(255, 255, 255, 1) !important;
    box-sizing: border-box;
    border-width: 1px;
    border-style: solid;
    border-color: rgba(204, 204, 204, 1);
    border-radius: 5px;
    -moz-box-shadow: none;
    -webkit-box-shadow: none;
    box-shadow: none;
    color: #666666;
        }
        .btnRefresh:hover {
                                background: #9e978f !important;
                                color: #fff;
                          }
        

        .btnRefresh:hover{
            background: inherit;
            background-color: rgba(204, 204, 204, 1);
            color:black;
        }
        .lblFromstyle{
            margin-top:15px;
            font-size: 20px;
        }
        .lblTostyle{
            margin-top:15px;
            font-size: 20px;
        }
        .txtFromstyle{
            margin-top:15px;
            width:110px;
            margin-left: -16px;
        }

        .txtTostyle{
            margin-top:15px;
            width:110px;
            margin-left: -49px;
        }
        .lblheader{
            margin-top:10px;
            font-size: 20px;
        }
    </style>

</head>
<body>
    <form id="form1" runat="server">
        <div class="row">
            <label class="lblheader" style="margin-left: 24px;">Invoiced Report</label>
        </div>

        <div class="row" style="margin-left:-4px;margin-top:15px;">
            <div class="col-lg-2" style="width:120px;">
                <asp:CheckBox ID="today" runat="server" Checked="true" CssClass="DateType"   style=""/>
                <asp:Label AssociatedControlID="today" ID="lbltoday" runat="server" style="font-size:20px">&nbsp;Today</asp:Label>
            </div>
            <div class="col-lg-2" style="width:200px;">
                <asp:CheckBox ID="Week" runat="server" CssClass="DateType"   style=""/>
                <asp:Label AssociatedControlID="Week" ID="lblweek" runat="server" style="font-size:20px">&nbsp;Current Week</asp:Label>
            </div>
            <div class="col-lg-2" style="width:200px;">
                <asp:CheckBox ID="Month" runat="server" CssClass="DateType"   style=""/>
                <asp:Label AssociatedControlID="Month" ID="lblmonth" runat="server" style="font-size:20px">&nbsp;Current Month</asp:Label>
            </div>
            <div class="col-lg-2" style="width:200px;">
                <asp:CheckBox ID="quarter" runat="server" CssClass="DateType"   style=""/>
                <asp:Label AssociatedControlID="quarter" ID="lbl6month" runat="server" style="font-size:20px">&nbsp;Current Quarter</asp:Label>
            </div>
            <div class="col-lg-2" style="width:200px;">
                <asp:CheckBox ID="year" runat="server" CssClass="DateType"   style=""/>
                <asp:Label AssociatedControlID="year" ID="lblyear" runat="server" style="font-size:20px">&nbsp;Current Year</asp:Label>
            </div>
        </div>

    <div class="row">
        <div class="col-lg-1" style="width:130px;">
            <label class="lblFromstyle" style="margin-left: 11px;">From Date</label>
        </div>
        <div class="col-lg-1">
            <asp:TextBox ID="txtfromdate" CssClass="txtFromstyle" runat="server"></asp:TextBox>
        </div>
        <div class="col-lg-1" style="width:130px;">
            <label class="lblTostyle">To Date</label>
        </div>
        <div class="col-lg-1">
            <asp:TextBox ID="txttodate" CssClass="txtTostyle" runat="server"></asp:TextBox>
        </div>
        <div class ="col-lg-2" style="margin-top:9px;">
            <asp:LinkButton Id="btnsubmit" runat="server" CssClass="btnRefresh" onClick="btnsubmit_Click">Refresh</asp:LinkButton>
        </div>
        <div class="col-lg-6"></div>
    </div>
    
    <div style="margin-top:20px;">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="InvoicedReportReportViewer" runat="server" Width="100%" Height="800px"></rsweb:ReportViewer>    
    </div>
    </form>
</body>
</html>
