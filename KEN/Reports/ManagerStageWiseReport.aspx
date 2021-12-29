<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManagerStageWiseReport.aspx.cs" Inherits="KEN.Reports.ManagerStageWiseReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken =89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../Content/css/libraries/bootstrap.min.css" rel="stylesheet" type="text/css" />

    <%--@*<link href="~/Content/css/bootstrap4.min.css" rel="stylesheet" type="text/css" />*@--%>
    <!-- font Awesome -->
    <%--@*<link href="~/AdminLTE/css/font-awesome.min.css" rel="stylesheet" type="text/css" />*@--%>
    <link rel="stylesheet" href="http://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css" />

    <link href="../Content/css/libraries/AdminLTE.css" rel="stylesheet" type="text/css" />
    <link href="../Content/css/custom.css" rel="stylesheet" type="text/css" />
    <link href="../Content/css/libraries/breadcrumb.css" rel="stylesheet" type="text/css" />
    <link href="../Content/css/libraries/SecStyle.css" rel="stylesheet" type="text/css" />
    <link href="../Content/css/libraries/w3.css" rel="stylesheet" type="text/css" />
    <link href="../Content/css/libraries/magicsuggest.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/chosen/1.5.1/chosen.min.css" />


    <link rel="stylesheet" href="~/Content/jquery-ui.css" />

    <script src="../Content/js/libraries/jquery.min.js"></script>
    <script src="../Content/js/libraries/jquery-ui.min.js"></script>
    <script src="../Content/js/libraries/jquery.mark.js"></script>
    <script src="../Content/js/libraries/jquery.mark.min.js"></script>
    <script src="../Content/js/libraries/jquery.mark.es6.js"></script>
    <%--<script src="../Content/js/libraries/jquery.mark.es6.min.js"></script>--%>


    <link href="../Content/pqGrid/bootstrap-pqgrid.css" rel="stylesheet" />
    <link href="../Content/pqGrid/pqgrid.min.css" rel="stylesheet" />
    <link href="../Content/pqGrid/pqgrid.bootstrap.min.css" rel="stylesheet" />
    <link href="../Content/pqGrid/pqgrid.ui.min.css" rel="stylesheet" />
    <script src="../Content/pqGrid/pqgrid.min.js"></script>
    <script src="../Content/js/libraries/magicsuggest.js"></script>
    <script src="../Content/js/libraries/bootstrap.min.js"></script>
    <script src="../Content/js/libraries/bootbox.min.js"></script>
    <%--<script src="../Content/js/app.js"></script>--%>
    <script src="../Content/js/libraries/inputmask.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/chosen/1.5.1/chosen.jquery.min.js"></script>

    <script>
        $(function () {
            $("#todate").datepicker({
                dateFormat: "dd/mm/yy"
            });
            $("#fromDate").datepicker({
                dateFormat: "dd/mm/yy"
            });
        }
        );
    </script>

    <style>
        .btnRefresh {
            border-color: #ddd;
            top: 0px;
            float: left;
            padding: 5px 0px 0px 13px;
            width: 74px;
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


            .btnRefresh:hover {
                background: inherit;
                background-color: rgba(204, 204, 204, 1);
                color: black;
            }
    </style>


</head>
<body>
    <form id="form1" runat="server">
        <asp:HiddenField ID="hdndate" runat="server" />
        <div class="row">
            <div class="col-lg-2" style="width: 161px;">
                <label style="margin-top: 10px; margin-left: 10px; font-size: 20px;">Date Range</label>
            </div>
              <div class="col-lg-2" style="width: 140px">
                <asp:TextBox ID="fromDate" runat="server" Style="margin-top: 10px; width: 110px"></asp:TextBox>
            </div>
            <div class="col-lg-2" style="width: 140px">
                <asp:TextBox ID="todate" runat="server" Style="margin-top: 10px; width: 110px"></asp:TextBox>
            </div>
             <div class="col-lg-2" style="width: 130px;">
                <label style="margin-top: 10px; margin-left: 10px; font-size: 20px;">Source</label>
            </div>
            <div class="col-lg-2" style="width: 150px;">
                <asp:DropDownList ID="drdSource" runat="server" style="margin-top: 10Px; width: 150px;height:28px;"></asp:DropDownList>
            </div>
            <div class="col-lg-2" style="margin-top: 9px;margin-left:25Px;">
                <asp:LinkButton ID="btnid" runat="server" CssClass="btnRefresh" OnClick="btnid_Click">Refresh</asp:LinkButton>
            </div>
        
        </div>
        <div style="margin-top: 20px;">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <rsweb:ReportViewer ID="ManagerStageWiseReportViewer" runat="server" Width="100%" Height="800px"></rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>
