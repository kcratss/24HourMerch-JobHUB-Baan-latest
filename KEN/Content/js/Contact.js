var ContactList;
var CurrentContactIndex;
var ContactChangesValues = [];
var EmailFlag = true;
$(function () {


    $("#ddlDelivery").on("change", function () {
        var OrgID = "";
        if ($("#ddlDelivery").val() == "") {
            $("#txtAttention").val('');
            $("#ddlDelivery").val('');
            $("#txtAddress1").val('');
            $("#txtAddress2").val('');
            $("#ddlStateList").val('');
            $("#txtPostCode").val('');
            $("#txtTradingName").val('');
            $("#txtAddressNotes").val('');
            $("#HiddenAddressId").val('');
            $("#btnsaveAddress").removeClass("customAlertChange");  
        }
        if ($('#PageName').val() == "JobDetails" || $('#PageName').val() == "InvoicingDetails" || $('#PageName').val() == "PackingDetails" || $('#PageName').val() == "ShippingDetails" || $('#PageName').val() == "CompleteDetails") {
            OrgID = $('#HiddenforDefaultOrgID').val();
        }
        //tarun 22/08/2018
        else if ($('#PageName').val() == "PurchaseDetails") {
            OrgID = $('#HiddenPurchaseOrg').val();
        }
            //end
        else {
            OrgID = $('#hdnOrgId').val();
        }
        if ($(this).val() != "" && OrgID != "") {
            var currDeliveryVal = $(this).val();
            //var currTradingVal = $("#txtTradeName").val();
            GetOrganisationAddress(OrgID, $(this).val());
            $("#ddlDelivery").val(currDeliveryVal);
            
            //$("#txtTradingName").val(currTradingVal);
        }
        if ($('#PageName').val() != "JobDetails") {
            $("#btnsaveAddress").addClass("customAlertChange");
        }
    });
    if ($('#PageName').val() == "ContactDetails") {
        $('#DivContactType').removeClass("col-lg-6 col-md-6 col-sm-6 col-xs-6");
        $('#txtContactType').css("width", "94%");
        $('#DivIsPrimary').removeClass("col-lg-3 col-md-3 col-sm-3 col-xs-3").css("display", "none");
        $('#DivUnlink').removeClass("col-lg-3 col-md-3 col-sm-3 col-xs-3").css("display", "none");
        $('#btnOpenContact').css("display", "none");
        $('#btnContactreset').css("display", "none");


    }
    // baans change 15th November for user by title
    if ($("#PageName").val() == "ContactDetails" || $("#PageName").val() == "ContactList") {
        $.ajax({
            url: '/Opportunity/GetUserByTitle',
            data: {},
            async: false,
            success: function (response) {
                //if (response.AdminRight == true) {
                //    $("#profileOfUser").append('<option value= "All">' + "All Records" + '</option>');
                //}
                //if (response.CurrentUser == 0) {
                //    var All = "All";
                //    $('#profileOfUser').val(All);
                //}
                //else {
                //    if (response.CurrentUser != "") {
                //        $('#profileOfUser').val(response.CurrentUser);
                //    }
                //}
                if (response.AdminRight == true) {
                    $("#profileOfUser").append('<option value= "All">' + "All Records" + '</option>');
                    var All = "All";
                    $('#profileOfUser').val(All);
                }
                    //if (response.CurrentUser == 0) {
                    //    var All = "All";
                    //    $('#profileOfUser').val(All);
                    //}
                else {
                    if (response.CurrentUser != "") {
                        $('#profileOfUser').val(response.CurrentUser);
                    }
                }
            },
            type: 'post',
        });
        //$('#profileOfUser').val("Hitesh Sindhu");
    }
    // baans end 15th November
    if ($('#PageName').val() == "OrganisationDetail") {
        $('#DivIsPrimary').css("visibility", "hidden");
        $('#btnResetorganisation').css("display", "none");
        $('#btnOpenOrganisation').css("display", "none");
    }
    //|| $('#PageName').val() == "OrganisationDetail"
    $(".ContactContainer :input").focus(function (event) {
        ContactChangesValues.push({ id: $(this).attr('id'), value: $(this).val() });
    });
    $(".ContactContainer :input").blur(function (event) {
        var OldValues = ContactChangesValues.find(x=>x.id == $(this).attr('id'));
        var index = ContactChangesValues.findIndex(x=>x.id == $(this).attr('id'));
        if (OldValues != null && OldValues != undefined) {

            if (OldValues.value != $(this).val()) {
                $("#btnsaveContact").addClass("customAlertChange");

            }
            ContactChangesValues.splice(index, 1);
        }
    });

    $('#txtContactNo').inputmask({ "mask": "(9999) 999-999" });
});

function AddContact() {
    if ($('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "Opportunity" || $('#PageName').val() == "OrderDetails" || $('#PageName').val() == "JobDetails" || $('#PageName').val() == "Event" || $('#PageName').val() == "PackingDetails" || $('#PageName').val() == "InvoicingDetails" || $('#PageName').val() == "ShippingDetails" || $('#PageName').val() == "CompleteDetails") {
        if ($('#lblOpportunityId').text() != "000000") {
            if ($('#btnunlinkContact').prop('checked')) {
                saveContact();
            }
            else {
                bootbox.confirm("Do you wish to delete?", function (result) {
                    if (result) {
                        saveContact();
                    }
                    else {
                        $('#btnunlinkContact').prop('checked', true);
                    }
                });
            }
        }
        else {
            CustomWarning('Save opportunity first.');
        }
    }
    else if($('#PageName').val() == "OrganisationDetail")
    {
        if ($('#hdnOrgId').val() != "") {
            if ($('#btnunlinkContact').prop('checked')) {
                saveContact();
            }
            else {
                bootbox.confirm("Do you wish to delete?", function (result) {
                    if (result) {
                        saveContact();
                    }
                    else {
                        $('#btnunlinkContact').prop('checked', true);
                    }
                });
            }
        }
        else {
            CustomWarning('Save Organisation first.');
        }
    }
    else if($('#PageName').val() == "ContactDetails")
    {
        saveContact();
    }


}
function saveContact() {
    

    var checkflag = ValidateContact();
    // baans change 29th Sept for checking the contact Validation
    var IsContactValid = true; 
    var filter = /^[^-\s][a-zA-Z0-9_\s-]+$/;
    if (filter.test($('#txtFirstName').val())) {
        $("#txtFirstName").removeClass("customAlertChange");
    }
    else {
        IsContactValid = false;
        $("#txtFirstName").addClass("customAlertChange");
    }
    if (filter.test($('#txtLastName').val())) {
        $("#txtLastName").removeClass("customAlertChange");
    }
    else {
        IsContactValid = false;
        $("#txtLastName").addClass("customAlertChange");
    }
    if (IsContactValid) {
        if (checkflag == true) {
            if (EmailFlag) {
                var id, ddlTitle, txtFirstName, txtLastName, txtContactRole, txtEmail, txtContactNo, txtContactType, txtNotes, ddlAcctMgr, OrgIdMappingID;
                ddlTitle = $("#ddlTitle").val();
                txtFirstName = $("#txtFirstName").val();
                txtLastName = $("#txtLastName").val();
                txtContactRole = $("#txtContactRole").val();
                txtEmail = $("#txtEmail").val();
                txtContactNo = $("#txtContactNo").val();
                txtContactType = $("#txtContactType").val();
                txtNotes = $("#txtNotes").val();
                ddlAcctMgr = $("#ddlAcctMgr").val();
                if ($("#HiddenContactId").val() != "") {
                    id = $("#HiddenContactId").val();
                }
                else {
                    id = 0;
                }
                if ($('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "Opportunity" || $('#PageName').val() == "OrderDetails" || $('#PageName').val() == "JobDetails" || $('#PageName').val() == "Event" || $('#PageName').val() == "PackingDetails" || $('#PageName').val() == "InvoicingDetails" || $('#PageName').val() == "ShippingDetails" || $('#PageName').val() == "CompleteDetails") {
                    MappingID = $('#lblOpportunityId').text();
                }
                else if ($('#PageName').val() == "OrganisationDetail") {
                    MappingID = $('#hdnOrgId').val();
                }
                else {
                    MappingID = 0;
                }

                var model = {
                    "model": { id: id, ContactType: txtContactType, first_name: txtFirstName, last_name: txtLastName, email: txtEmail, mobile: txtContactNo, title: ddlTitle, ContactRole: txtContactRole, notes: txtNotes, acct_manager_id: ddlAcctMgr, PageSource: $("#PageName").val() },
                    "MappingModel": { ContactID: $("#HiddenContactId").val(), MappingID: MappingID, isPrimary: $('#ContactisPrimary').prop("checked"), IsLinked: $('#btnunlinkContact').prop('checked') }
                }
                $.ajax({
                    url: '/Contact/AddContact',
                    data: model,
                    async: false,

                    success: function (response) {
                        if (response.ID != null && response.ID != undefined && response.ID != 0) {
                            $("#btnsaveContact").removeClass("customAlertChange");
                            if ($('#PageName').val() == "ContactDetails" && $('#HiddenContactId').val() == "") {
                                history.pushState('', '', '/Contact/ContactDetails/' + response.ID);
                            }
                            $('#HiddenContactId').val(response.ID);

                            MakeReadonly();
                            if ($('#PageName').val() == "OrganisationDetail") {
                                RefreshCOntactListByOrgId($("#hdnOrgId").val());
                            }
                            if ($('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "Opportunity" || $('#PageName').val() == "OrderDetails" || $('#PageName').val() == "JobDetails" || $('#PageName').val() == "Event" || $('#PageName').val() == "PackingDetails" || $('#PageName').val() == "InvoicingDetails" || $('#PageName').val() == "ShippingDetails" || $('#PageName').val() == "CompleteDetails") {
                                RefreshCOntactList($('#lblOpportunityId').text());
                            }

                            if ($('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "OrderDetails") {
                                $('#HiddenforDefaultOrgID').val($("#hdnOrgId").val());
                            }
                            if ($('#PageName').val() == "JobDetails") {

                                $.ajax({
                                    url: '/Contact/GetContactById',
                                    data: { ContactId: response.ID },
                                    async: false,

                                    success: function (response) {
                                        var data = response;
                                        if (data.OrgId != null && data.OrgId != "") {
                                            CheckDeliveryAddress(data.OrgId);
                                            GetOrganisationAddress(data.OrgId, "");

                                            $('#HiddenforDefaultOrgID').val(data.OrgId);
                                        }
                                        else {
                                            // ResetOrgForm()
                                            GetOrganisationAddress(0, "");
                                            CheckDeliveryAddress(0);
                                            $('#HiddenforDefaultOrgID').val('');
                                        }
                                        //22 Aug 2018 (N)
                                        if ($('#PageName').val() == "Event") {
                                            GetOppoListByEvent('All', $('#HiddenEventId').val());
                                        }
                                        //22 Aug 2018 (N)
                                    },
                                    type: 'get',

                                });


                            }

                        }
                        CustomAlert(response);
                        //tarun 11/09/2018
                        if (response.Message == "Contact with this email already exists. click on open to change the contact details") {

                            var contemail = $("#HiddenEmailId").val();
                            $("#txtEmail").val(contemail);
                        }
                        //end
                    },
                    type: 'post',

                });
            }
            else {
                CustomWarning('Enter valid email address.');
            }
        }
        else {
            CustomWarning('Fill all required fields.');
            //CustomErrorCode("Required");
        }
    }
    else {
        CustomWarning('Fill all required fields.');
        //CustomErrorCode("Required");
    }
    // baans end 29th Sept
    }
   
function ValidateContact() {
    var checkflag = true;
    if ($("#ddlTitle").val() != "") {
        //$("#ddlTitle").css("border-color", "rgba(204, 204, 204, 1)");
        $("#ddlTitle").removeClass('customAlertChange');
    } else {
        //$("#ddlTitle").css("border-color", "red");
        $("#ddlTitle").addClass('customAlertChange');
        checkflag = false;
    }
    if ($("#txtFirstName").val() != "") {
        //$("#txtFirstName").css("border-color", "rgba(204, 204, 204, 1)");
        $("#txtFirstName").removeClass('customAlertChange');
    } else {
        //$("#txtFirstName").css("border-color", "red");
        $("#txtFirstName").addClass('customAlertChange');
        checkflag = false;
    }
    if ($("#txtLastName").val() != "") {
        //$("#txtLastName").css("border-color", "rgba(204, 204, 204, 1)");
        $("#txtLastName").removeClass('customAlertChange');
    } else {
        //$("#txtLastName").css("border-color", "red");
        $("#txtLastName").addClass('customAlertChange');
        checkflag = false;
    }

    if ($("#txtEmail").val() != "") {
            //$("#txtEmail").css("border-color", "rgba(204, 204, 204, 1)");
        $("#txtEmail").removeClass('customAlertChange');
    } else {
        //$("#txtEmail").css("border-color", "red");
        $("#txtEmail").addClass('customAlertChange');
        checkflag = false;
    }
    if ($("#txtEmail").val() != "") {
        if (validateEmailAddress($("#txtEmail").val())) {
            //$("#txtEmail").css("border-color", "rgba(204, 204, 204, 1)");
            $("#txtEmail").removeClass('customAlertChange');
            EmailFlag = true;
        }
        else {
            //$("#txtEmail").css("border-color", "red");
            $("#txtEmail").addClass('customAlertChange');
            EmailFlag = false;
        }
    }
    if ($("#ddlAcctMgr").val() != "") {
        //$("#ddlAcctMgr").css("border-color", "rgba(204, 204, 204, 1)");
        $("#ddlAcctMgr").removeClass('customAlertChange');
    } else {
        //$("#ddlAcctMgr").css("border-color", "red");
        $("#ddlAcctMgr").addClass('customAlertChange');
        checkflag = false;
    }
    return checkflag;
}
function OpenContact() {
    if ($('#HiddenContactId').val() != "") {
        //location.href = "/Contact/ContactDetails/" + $('#HiddenContactId').val();
        window.open('/Contact/ContactDetails/' + $('#HiddenContactId').val());
    }
    else {
        $('#CustomAlert').removeClass('j-alertBox-Failed').removeClass('j-alertBox-Success').css('display', 'none');
        $('#CustomAlert').addClass('j-alertBox-Warning').css('display', 'block');
        // baans change 11th September for Contact Alert Message changeg
        //$('#alertMsg').html('<strong>Warning !</strong> Select Contact First');
        $('#alertMsg').html('<strong>Warning !</strong> Please Select the Contact'+','+ ' then click on Open to see the Detail.');
        // baans end 11th September
        setTimeout(function () {
            $('#CustomAlert').fadeOut('slow', function () {
                $('#CustomAlert').removeClass('j-alertBox-Warning').css('display', 'none');
                $('#alertMsg').html('');

            });
        }, 5000);
    }
}
function ContactDetailsListDown(OppID, Status) {
    var data;
    $.ajax({
        url: '/Contact/GetContactDownList',
        data: { id: OppID, Status: Status },
        async: false,
        success: function (response) {
            data = response;
        },
        error: function (response) {
            alert();
        },
        type: 'post'

    });
    var obj = {
        selectionModel: { type: 'row' },
        virtualX: true, virtualY: true,
        resizable: false,
        pageModel: { type: "local", rPP: 10 },
        strNoRows: 'No records found for the selected Criteria',
        scrollModel: { horizontal: false },
        width: "96.5%",
        wrap: false,
        hwrap: false,
        height: 200,
        columnTemplate: { width: 120, halign: "left" },
        numberCell: { width: 0, title: "", minWidth: 0 },
        editable: false
    };
    obj.colModel = [
        {
            title: "Option No", dataIndx: "DispalayId", width: "6%", align: "center", dataType: "int"
        },
        {
            title: "Qty", dataIndx: "quantity", width: "2%", align: "right", dataType: "date"
        },
        {
            title: "Brand", dataIndx: "BrandName", width: "5%", dataType: "string"
        },
        {
            title: "Code", dataIndx: "code", width: "4%", dataType: "int"
        },
        {
            title: "Item", dataIndx: "ItemName", width: "7%", dataType: "date"
        },

        {
            title: "Color", dataIndx: "colour", width: "5%", dataType: "string"
        },
        {
            title: "Front Dec", dataIndx: "Front_decDesign", width: "8%", dataType: "string"
        },
        {
            title: "Back Dec", dataIndx: "Back_decDesign", width: "8%", dataType: "string"
        },
          {
              title: "Lft Slv Dec", dataIndx: "Left_decDesign", width: "9%", dataType: "string"
          },
          {
              title: "Rht Slv Dec", dataIndx: "Right_decDesign", width: "9%", dataType: "string"
          },
          {
              title: "Extra Dec", dataIndx: "Extra_decDesign", width: "8%", dataType: "string"
          },
          {
              title: "Unit", dataIndx: "uni_price", align: "right", width: "5%", dataType: "string"
          },
          {
              title: "Unit+GST", dataIndx: "UnitInclGST", align: "right", width: "6%", dataType: "string"
          },
          {
              title: "Ext Ex GST", dataIndx: "ExtExGST", align: "right", width: "7%", dataType: "string"
          },
          {
              title: "Ext+GST", dataIndx: "ExtInclGST", align: "right", width: "6%", dataType: "string"
          },
          {
              title: "Incl", dataIndx: "include", width: "5%", dataType: "string"
          },
    ];

    obj.dataModel = { data: data };

    obj.rowDblClick = function (evt, ui) {
        var rowIndx = getRowIndx(this);
        if (rowIndx != null) {
            var data = this.pdata[rowIndx];
            var dataform = data.id;
            location.href = "/Contact/ContactDetails/" + dataform;
        }
    }

    function getRowIndx(grid) {
        var arr1 = grid.getChanges();
        var arr = grid.selection({ type: 'row', method: 'getSelection' });
        if (arr && arr.length > 0) {
            return arr[0].rowIndx;
        }
        else {
            alert("Select a row.");
            return null;
        }
    }
    pq.grid("#ContactDetailsGST", obj);
    pq.grid("#ContactDetailsGST", "refreshDataAndView");

}



function HighlightTabs(evt, cityName) {
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabBody");
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    evt.currentTarget.className += " active";
}






function GetOppoListByContactOrOrganisation(Stage, ID) {
    //alert(Stage +" "+ContactId);
    var data;
    $.ajax({
        url: '/Contact/GetOppoListByContactOrOrganisation',
        data: { Stage: Stage, ID: ID, PageSource: $("#PageName").val() },
        async: false,
        success: function (response) {
            data = response;
        },
        error: function (response) {
            alert();
        },
        type: 'post'

    });
    var obj = {
        selectionModel: { type: 'row' },
        virtualX: true, virtualY: true,
        resizable: false,
        wrap: false,
        hwrap: false,
        pageModel: { type: "local", rPP: 10 },
        strNoRows: 'No records found for the selected Criteria',
        scrollModel: { horizontal: false },
    };

    obj.width = "96.5%";
    obj.height = 200;
    obj.columnTemplate = { width: 120, halign: "left" };
    obj.numberCell = { width: 0, title: "", minWidth: 0 };


    obj.colModel = [
        {
            title: "Job No", dataIndx: "DisplayOpportunityId", align: "center", width: "6%", dataType: "string", editable: false
, render: function (ui) {
    var id = ui.cellData;
    var Stage = ui.rowData.Stage;
    if (Stage == "Quote") {
        return "<a href='/opportunity/QuoteDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
    }
    else if (Stage == "Order") {
        return "<a href='/opportunity/OrderDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
    }
    else if (Stage == "Job") {
        return "<a href='/opportunity/JobDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
    }
    else if (Stage == "Order Packed" || Stage == "Stock Decorated") {
        return "<a href='/opportunity/PackingDetails/" + parseInt(id) + "'><label  class='internalLnk'>" + id + "</label></a>"
    }
    else if (Stage == "Order Shipped") {
        return "<a href='/opportunity/ShippingDetails/" + parseInt(id) + "'><label  class='internalLnk'>" + id + "</label></a>"
    }
    else if (Stage == "Complete") {
        return "<a href='/opportunity/CompleteDetails/" + parseInt(id) + "'><label  class='internalLnk'>" + id + "</label></a>"
    }
    else if (Stage == "Order Invoiced") {
        return "<a href='/opportunity/InvoicingDetails/" + parseInt(id) + "'><label  class='internalLnk'>" + id + "</label></a>"
    }
    else {
        return "<a href='/opportunity/OpportunityDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
    }
}
        },
        {
            title: "Date", dataIndx: "OppDate", align: "left", width: "7%", dataType: "date", editable: false,   // tarun 22/09/2018
            render: function (ui) {
                var date = ui.cellData;
                if (date != null && date != undefined && date != "") {
                    //var nowDateopp = new Date(parseInt(date.substr(6)));
                    //return ("0" + nowDateopp.getDate()).slice(-2) + '/' + ("0" + (nowDateopp.getMonth() + 1)).slice(-2) + '/' + nowDateopp.getFullYear();
                    // baans change 22nd November
                    return DateFormat(date);
                    // baans end 22nd November
                }
            },
        },
        {
            title: "Name", dataIndx: "OppName", width: "10%", dataType: "string", editable: false
        },
        {
            title: "Qty", dataIndx: "Quantity", align: "right", width: "4%", dataType: "int", editable: false
        },
        {
            title: "Required By", dataIndx: "ReqDate", align: "left", width: "8%", dataType: "date", editable: false,
            render: function (ui) {
                var date = ui.cellData;
                if (date != null && date != undefined && date != "") {
                    //var nowDateopp = new Date(parseInt(date.substr(6)));
                    //return ("0" + nowDateopp.getDate()).slice(-2) + '/' + ("0" + (nowDateopp.getMonth() + 1)).slice(-2) + '/' + nowDateopp.getFullYear();
                    // baans change 22nd November
                    return DateFormat(date);
                    // baans end 22nd November
                }
            },

        },

        {
            title: "Department", dataIndx: "DepartmentName", width: "22%", dataType: "string", editable: false,
        },
        {
            title: "Status", dataIndx: "Stage", width: "7%", dataType: "string", editable: false,
        },
        {
            title: "Contact", dataIndx: "Contactfullname", width: "13%", dataType: "string", editable: false,
        },
          {
              title: "Event", dataIndx: "EventName", width: "13%", dataType: "string", editable: false,
          },
          {
              title: "Total Balance", dataIndx: "TotalBalance", width: "12%", dataType: "decimal", editable: false,
          },
    ];

    obj.dataModel = { data: data };

    obj.selectChange = function (evt, ui) {
        //alert("select change");
        var row = getRowIndx(this);
        if (row != null) {
            var data = this.pdata[row];
            var dataid = data.OpportunityId;
            if (data.Stage == "Opportunity" || data.Stage == "Quote") {
                ContactDetailsListDown(dataid, "Opp");
            }
            else {
                ContactDetailsListDown(dataid, "Order");
            }

        }
    }


    obj.rowDblClick = function (evt, ui) {


        var row = getRowIndx(this);
        if (row != null) {
            var data = this.pdata[row];
            var dataid = data.OpportunityId;
            ContactDetailsListDown(dataid);
        }
    }


    function getRowIndx(grid) {
        var arr1 = grid.getChanges();
        var arr = grid.selection({ type: 'row', method: 'getSelection' });
        if (arr && arr.length > 0) {
            return arr[0].rowIndx;
        }
        else {
            alert("Select a row.");
            return null;
        }
    }
    pq.grid("#ContactDetailsGrid", obj);
    pq.grid("#ContactDetailsGrid", "refreshDataAndView");

}

// baans change 4th October for deleting the Contact
function deleteContactRow(rowIndx) {
    var rowdata = $('#ContactGrid').pqGrid("getRowData", { rowIndx: rowIndx });
    var id = rowdata.id;
    $.ajax({
        url: '/Contact/GetContactLinkStatus',
        data: { Id: id },
        async: false,
        success: function (response) {
            if (response == false) {
                bootbox.confirm("Do you want to delete the contact?", function (result) {
                    if (result) {
                        var rowdata = $('#ContactGrid').pqGrid("getRowData", { rowIndx: rowIndx });
                        var id = rowdata.id;


                        $.ajax({
                            url: '/Contact/DeleteContact',
                            data: { Id: id },
                            async: false,

                            success: function (response) {
                                //var Message = "Data Deleted Successfully";
                                var Res = { Result: "Success", Message: response.Message };
                                CustomAlert(Res);
                            },
                            type: "Post"
                        });
                         ContactListGrid('Customer');
                    }
                    else {

                    }
                })
            }
            else {
                if (response == true) {
                    CustomWarning("Contact is linked with one or more opportunity. So it cannot be deleted!!!")
                }
            }
            //alert(response);
            //newdata = response;
            //alert(JSON.stringify(newdata));
        },
        type: 'get',
    });


}
// baans end 4th October
function ContactListGrid(Leads) {
    var data;
    var model = { Leads: Leads };
    var url = '/Contact/GetContactList';
    if (Leads == "Custom") {
        // baans change 04th December for Custom
        //url = '/Contact/GetCustomContactList';
        //model = { CustomText: $('#searchtextbox').val(), TableName: 'Vw_tblContact' };
        var searchdata = $('#searchtextbox').val();
        if (searchdata != "") {
        url = '/Contact/GetCustomContactList';
        model = { CustomText: $('#searchtextbox').val(), TableName: 'Vw_tblContact' };
        }
        else {
            CustomWarning("Please put the search criteria in the search box and click on search button");
        }
        // baans end 4th December
    }
    $.ajax({
        url: url,
        data: model,
        async: false,
        success: function (response) {
            data = response;
        },
        error: function (response) {
            alert();
        },
        type: 'post'

    });

    //var obj =
    // {
    //     selectionModel: { type: 'row' },
    //     virtualX: true, virtualY: true,
    //     resizable: false,
    //     pageModel: { type: "local", rPP: 10 },
    //     strNoRows: 'No records found for the selected Criteria',
    //     scrollModel: { horizontal: false },
    //     //scrollModel: { autoFit: false },
    //     columnTemplate: { width: 120, halign: "left" },
    //     numberCell: { width: 0, title: "", minWidth: 0 },
    //     sortable: false,
    //     sortModel: { cancel: false, type: 'local', sorter: [{ dataIndx: 1, dir: ['up', 'down', 'up'] }] }
    // };
    //obj.width = "97%";
    //obj.height = 600;
    //obj.numberCell = { width: 0, title: "", minWidth: 0 };
    //obj.columnTemplate = { width: 120, halign: "left" };


    var colModel = [
        {
             title: "Contact No", dataIndx: "DisplayContactId", width: "7%", align: "center", dataType: "int", editable: false,
             render: function (ui) {
                 var id = ui.cellData;
                 return "<a href='/Contact/ContactDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>";
             }
         },
         {
             title: "First Name", dataIndx: "first_name", width: "10%", dataType: "string", editable: false
         },
         {
             title: "Last Name", dataIndx: "last_name", width: "10%", dataType: "string", editable: false
         },
         {
             title: "Mobile", dataIndx: "mobile", width: "9%", align: "left", dataType: "integer", editable: false,
             render: function (ui) {
                 var currentF = 0;
                 currentF = ui.cellData;
                 if (currentF != null && currentF != undefined) {
                     //var re = new RegExp("([0-9])([0-9]{3})([0-9]{3})([0-9]{3,6})", "g");
                     var newFormat1 = currentF.toString().substr(0, 4);
                     var newFormat2 = currentF.toString().substr(4, 3);
                     var newFormat3 = currentF.toString().substr(7, 3);
                     var newFormat = newFormat1 + ' ' + newFormat2 + ' ' + newFormat3;
                     return newFormat;
                 }
             }
         },
         {
             title: "Type", dataIndx: "ContactType", width: "9%", dataType: "int", editable: false
         },

         {
             title: "Organisation", dataIndx: "OrgName", width: "10%", dataType: "string", editable: false
         },
         {
             title: "Main Phone", dataIndx: "MainPhone", width: "9%", align: "left", dataType: "integer", editable: false,
             render: function (ui) {
                 var currentF = 0;
                 currentF = ui.cellData;
                 if (currentF != null && currentF != undefined) {
                     //var re = new RegExp("([0-9])([0-9]{3})([0-9]{3})([0-9]{3,6})", "g");
                     var newFormat1 = currentF.toString().substr(0, 4);
                     var newFormat2 = currentF.toString().substr(4, 3);
                     var newFormat3 = currentF.toString().substr(7, 3);
                     var newFormat = newFormat1 + ' ' + newFormat2 + ' ' + newFormat3;
                     return newFormat;
                 }
             }
         },
         {
             title: "Contact Notes", dataIndx: "notes", width: "24%", dataType: "string", editable: false
         },
           {
               title: "Account Manager", dataIndx: "AccountManager", width: "12%", dataType: "string", editable: false
         },
           // Baans change 4th October for Delete the contact
           //{
           //    title: "", dataIndx: "", width: "2%", dataType: "int",
           //    render: function (ui) {
           //        var dataindx = ui.rowIndx;
           //        var hostname = window.location.origin;
           //        return "<a onclick='deleteContactRow(" + dataindx + ")'><img src='" + hostname + "/Content/images/DeleteContact.png' style='width:18px'/></a>";
           //    }
           //}
    // baans end 4th October
    ];
    var obj = {
        colModel: colModel,
        dataModel: { data: data },
        selectionModel: { type: 'row' },
        scrollModel: { pace: 'consistent', horizontal: false },
        //virtualX: true, virtualY: true,
        scrollModel: { horizontal: false },
        //resizable: false,
        editable: false,
        wrap: false,
        hwrap: false,
        pageModel: { type: "local", rPP: 50 },
        strNoRows: 'No records found for the selected Criteria',
        width: "96.5%",
        height: 782,
        columnTemplate: { width: 120, halign: "left" },
        numberCell: { width: 0, title: "", minWidth: 0 },
        columnTemplate: { width: 120, halign: "left" },
        sortable: true,
        //sortModel: { cancel: false, type: 'local', sorter: [{ dataIndx: 1, dir: ['up', 'down', 'up'] }] }

    };

    pq.grid("#ContactGrid", obj);
    pq.grid("#ContactGrid", "refreshDataAndView");

}


function Contactlist() {
    location.href = "/Contact/ContactList";
}

function GetContactByOppId(OppId) {
    if (OppId != undefined && OppId != 0) {

        $.ajax({
            url: '/Contact/GetContactByOppId',
            data: { OppId: OppId },
            async: false,
            success: function (response) {
                ContactList = response;
                if (ContactList.length > 0) {
                    if (ContactList.length == 1) {
                        $('#btnNext').css("display", "none")
                        $('#btnPre').css("display", "none")
                    }
                    else {
                        $('#btnNext').css("display", "block")
                    }
                    var primaryContact = ContactList.find(x => x.IsPrimary === true);
                    if (primaryContact != null && primaryContact != undefined) {
                        CurrentContactIndex = ContactList.findIndex(x => x.id == primaryContact.id);
                        if (CurrentContactIndex > 0) {
                            $('#btnPre').css("display", "block")
                        }
                        $('#txtOrgName').val(primaryContact.OrgName);               //  OrgName (N)
                        $("#txtFirstName").val(primaryContact.first_name);
                        $("#txtLastName").val(primaryContact.last_name);
                        $("#txtEmail").val(primaryContact.email);
                        $("#txtContactNo").val(primaryContact.mobile);
                        $("#txtNotes").val(primaryContact.notes);
                        $("#txtContactRole").val(primaryContact.ContactRole);
                        $("#ddlTitle").val(primaryContact.title);

                        $("#txtContactType").val(primaryContact.ContactType);
                        $("#HiddenContactId").val(primaryContact.id);
                        SetAccountManager(primaryContact.acct_manager_id, "ddlAcctMgr", "AccountManager");
                      //  $("#ddlAcctMgr").val(primaryContact.acct_manager_id);
                        $('#ContactisPrimary').prop('checked', true);
                        $("#HiddenFOrPrimary").val(primaryContact.id);
                        $("#HiddenforDefaultOrgID").val(primaryContact.OrgId);
                        if ($('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "OrderDetails" || $('#PageName').val() == "JobDetails" || $('#PageName').val() == "InvoicingDetails" || $('#PageName').val() == "PackingDetails") {
                            if (primaryContact.OrgId != null && primaryContact.OrgId != 0) {
                                GetOrganisationById(primaryContact.OrgId);
                                $('#HiddenforDefaultOrgID').val(primaryContact.OrgId);
                            }
                            else {
                                ResetOrgForm();
                                $('#HiddenforDefaultOrgID').val('');
                            }
                        }
                    }
                    else {
                        CustomError('Primary contact for this opportunity does not exist. Add a primary contact');
                        var SecondaryContact = ContactList[0];
                        CurrentContactIndex = 0;
                        if (CurrentContactIndex > 0) {
                            $('#btnPre').css("display", "block")
                        }
                        $("#txtFirstName").val(SecondaryContact.first_name);
                        $("#txtLastName").val(SecondaryContact.last_name);
                        $("#txtEmail").val(SecondaryContact.email);
                        $("#txtContactNo").val(SecondaryContact.mobile);
                        $("#txtNotes").val(SecondaryContact.notes);
                        $("#txtContactRole").val(SecondaryContact.ContactRole);
                        $("#ddlTitle").val(SecondaryContact.title);

                        $("#txtContactType").val(SecondaryContact.ContactType);
                        $("#HiddenContactId").val(SecondaryContact.id);
                        SetAccountManager(SecondaryContact.acct_manager_id, "ddlAcctMgr", "AccountManager");
                     //   $("#ddlAcctMgr").val(SecondaryContact.acct_manager_id);
                        // $('#ContactisPrimary').prop('checked', true);
                        //  $("#HiddenFOrPrimary").val(primaryContact.id);
                        $("#HiddenforDefaultOrgID").val('');
                        if ($('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "OrderDetails" || $('#PageName').val() == "JobDetails" || $('#PageName').val() == "InvoicingDetails" || $('#PageName').val() == "PackingDetails") {
                            if (SecondaryContact.OrgId != null && SecondaryContact.OrgId != 0) {
                                GetOrganisationById(SecondaryContact.OrgId);
                                $('#HiddenforDefaultOrgID').val(SecondaryContact.OrgId);
                            }
                            else {
                                ResetOrgForm();
                                $('#HiddenforDefaultOrgID').val('');
                            }
                        }

                    }
                    MakeReadonly();
                }
                else
                {
                    CustomError('Primary contact for this opportunity does not exist. Add a primary contact');
                }
            },
            error: function (response) {
                //alert();
            },
            type: 'post'

        });
    }
}

function NextContact(Next) {
    if (ContactList.length > 0) {

        var contact;
        if (CurrentContactIndex != undefined) {
            contact = ContactList[CurrentContactIndex + 1];
            CurrentContactIndex++;
            $('#btnPre').css("display", "block")
            $("#txtFirstName").val(contact.first_name);
            $("#txtLastName").val(contact.last_name);
            $("#txtEmail").val(contact.email);
            $("#txtContactNo").val(contact.mobile);
            $("#txtNotes").val(contact.notes);
            $("#txtContactRole").val(contact.ContactRole);
            $("#ddlTitle").val(contact.title);

            $("#txtContactType").val(contact.ContactType);
            $("#HiddenContactId").val(contact.id);
            SetAccountManager(contact.acct_manager_id, "ddlAcctMgr", "AccountManager");
           // $("#ddlAcctMgr").val(contact.acct_manager_id);
            if (contact.IsPrimary == true) {
                $('#ContactisPrimary').prop('checked', true);
            }
            else {
                $('#ContactisPrimary').prop('checked', false);
            }
            if ($('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "OrderDetails") {
                if (contact.OrgId != null && contact.OrgId != "") {
                    GetOrganisationById(contact.OrgId);
                    $('#HiddenforDefaultOrgID').val(contact.OrgId);
                }
                else {
                    ResetOrgForm()
                    $('#HiddenforDefaultOrgID').val('');
                }
            }
            if ($('#PageName').val() == "JobDetails") {
                if (contact.OrgId != null && contact.OrgId != "") {
                    CheckDeliveryAddress(contact.OrgId);
                    GetOrganisationAddress(contact.OrgId, "");
                    
                    $('#HiddenforDefaultOrgID').val(contact.OrgId);
                }
                else {
                    // ResetOrgForm()
                    GetOrganisationAddress(0, "");
                    CheckDeliveryAddress(0);
                    $('#HiddenforDefaultOrgID').val('');
                }
                
            }
            MakeReadonly();

        }
        else {
            contact = ContactList[0];
            CurrentContactIndex = 0;

        }
        if (CurrentContactIndex == ContactList.length - 1) {

            $('#btnNext').css("display", "none")
            $('#btnPre').css("display", "block")

        }
        if (ContactList.length == 1) {
            $('#btnNext').css("display", "none")
            $('#btnPre').css("display", "none")
        }
        if (CurrentContactIndex == 0) {
            $('#btnPre').css("display", "none")
        }

    }
}
function PreContact(Previous) {
    if (ContactList.length > 0) {
        var contact;
        if (CurrentContactIndex != undefined) {
            contact = ContactList[CurrentContactIndex - 1];
            CurrentContactIndex--;
            $('#btnNext').css("display", "block")
            $("#txtFirstName").val(contact.first_name);
            $("#txtLastName").val(contact.last_name);
            $("#txtEmail").val(contact.email);
            $("#txtContactNo").val(contact.mobile);
            $("#txtNotes").val(contact.notes);
            $("#txtContactRole").val(contact.ContactRole);
            $("#ddlTitle").val(contact.title);

            $("#txtContactType").val(contact.ContactType);
            $("#HiddenContactId").val(contact.id);
            SetAccountManager(contact.acct_manager_id, "ddlAcctMgr", "AccountManager");
          //  $("#ddlAcctMgr").val(contact.acct_manager_id);
            if (contact.IsPrimary == true) {
                $('#ContactisPrimary').prop('checked', true);
            }
            else {
                $('#ContactisPrimary').prop('checked', false);
            }
            if ($('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "OrderDetails") {
                if (contact.OrgId != null && contact.OrgId != "") {
                    GetOrganisationById(contact.OrgId);
                    $('#HiddenforDefaultOrgID').val(contact.OrgId);
                }
                else {
                    ResetOrgForm();
                    $('#HiddenforDefaultOrgID').val('');
                }
            }
            if ($('#PageName').val() == "JobDetails") {
                if (contact.OrgId != null && contact.OrgId != "") {
                    CheckDeliveryAddress(contact.OrgId);
                    GetOrganisationAddress(contact.OrgId, "");
                   
                    $('#HiddenforDefaultOrgID').val(contact.OrgId);
                }
                else {
                    // ResetOrgForm()
                    CheckDeliveryAddress(0);
                    GetOrganisationAddress(0, "");
                    $('#HiddenforDefaultOrgID').val('');
                }

            }
            MakeReadonly();
        }
        else {
            contact = ContactList[0];
            CurrentContactIndex = 0;

        }
        if (CurrentContactIndex == 0) {

            $('#btnNext').css("display", "block")
            $('#btnPre').css("display", "none")
        }
    }
}
function RefreshCOntactList(OppId) {
    if (OppId != undefined && OppId != 0) {

        $.ajax({
            url: '/Contact/GetContactByOppId',
            data: { OppId: OppId },
            async: false,
            success: function (response) {
                ContactList = response;
                if (ContactList.length > 0) {

                    var ContactID = parseInt($("#HiddenContactId").val());

                    CurrentContactIndex = ContactList.findIndex(x => x.id == ContactID);


                    $('#btnNext').css("display", "none")
                    $('#btnPre').css("display", "none")
                    if (CurrentContactIndex != -1) {
                        if (CurrentContactIndex > 0) {
                            $('#btnPre').css("display", "block")
                        }
                        if (CurrentContactIndex < ContactList.length - 1) {
                            $('#btnNext').css("display", "block")
                        }
                    }
                    else {
                        $('#btnunlinkContact').prop('checked', true);
                        CurrentContactIndex = 0;
                        if (ContactList.length > 1) {
                            $('#btnNext').css("display", "block")
                        }
                        if (ContactList.length > 0) {
                            $("#txtContactType").val(ContactList[0].ContactType);
                            $("#ddlTitle").val(ContactList[0].title);
                            $("#txtFirstName").val(ContactList[0].first_name);
                            $("#txtLastName").val(ContactList[0].last_name);
                            $("#txtEmail").val(ContactList[0].email);
                            $("#txtContactNo").val(ContactList[0].mobile);
                            $("#txtContactRole").val(ContactList[0].ContactRole);
                            SetAccountManager(ContactList[0].acct_manager_id, "ddlAcctMgr", "AccountManager");
                           // $("#ddlAcctMgr").val(ContactList[0].acct_manager_id);
                            $("#txtNotes").val(ContactList[0].notes);
                            $("#HiddenContactId").val(ContactList[0].id);
                            $('#ContactisPrimary').prop('checked', ContactList[0].IsPrimary);
                            if (ContactList[0].OrgId != null && ContactList[0].OrgId != "" && $('#PageName').val() != "Opportunity" && $('#PageName').val() != "JobDetails" && $('#PageName').val() != "OrganisationDetail" && $('#PageName').val() != "Event") {
                                GetOrganisationById(ContactList[0].OrgId);
                                $('#HiddenforDefaultOrgID').val(ContactList[0].OrgId);
                            }
                            else
                            {
                                ResetOrgForm();
                                $('#HiddenforDefaultOrgID').val('');
                            }
                        }
                        else {
                            ResetForm();
                        }
                    }
                }
                else {
                    $('#btnunlinkContact').prop('checked', true);
                    ResetForm();
                }

            },
            error: function (response) {
                //alert();
            },
            type: 'post'

        });
    }
}


function GetContactById(ContactId) {
    $.ajax({
        url: '/Contact/GetContactById',
        data: { ContactId: ContactId },
        async: false,

        success: function (response) {
            var data = response;
            //alert(data.first_name);

            $("#txtContactType").val(data.ContactType);
            $("#ddlTitle").val(data.title);
            $("#txtFirstName").val(data.first_name);
            $("#txtLastName").val(data.last_name);
            $("#txtEmail").val(data.email);
            //tarun 11/09/2018
            $("#HiddenEmailId").val(data.email);
            //end
            $("#txtContactNo").val(data.mobile);
            $("#txtContactRole").val(data.ContactRole);
            SetAccountManager(data.acct_manager_id, "ddlAcctMgr", "AccountManager");
           // $("#ddlAcctMgr").val(data.acct_manager_id);
            $("#txtNotes").val(data.notes);
            $("#HiddenContactId").val(data.id);
            $('#HiddenforDefaultOrgID').val(data.OrgId);
            if (data.OrgId != null && data.OrgId != "") {
                GetOrganisationById(data.OrgId);
            }

        },
        type: 'get',

    });
}
function GetContactByOrgId(OrgId) {
    if (OrgId != undefined && OrgId != 0) {

        $.ajax({
            url: '/Contact/GetContactByOrgId',
            data: { OrgId: OrgId },
            async: false,
            success: function (response) {
                ContactList = response;
                if (ContactList.length > 0) {
                    if (ContactList.length == 1) {
                        $('#btnNext').css("display", "none")
                        $('#btnPre').css("display", "none")
                    }
                    else {
                        $('#btnNext').css("display", "block")
                    }
                    var ContactData = ContactList[0];
                    if (ContactData != null && ContactData != undefined) {
                        CurrentContactIndex = ContactList.findIndex(x => x.id == ContactData.id);
                        if (CurrentContactIndex > 0) {
                            $('#btnPre').css("display", "block")
                        }
                        $("#txtFirstName").val(ContactData.first_name);
                        $("#txtLastName").val(ContactData.last_name);
                        $("#txtEmail").val(ContactData.email);
                        $("#txtContactNo").val(ContactData.mobile);
                        $("#txtNotes").val(ContactData.notes);
                        $("#txtContactRole").val(ContactData.ContactRole);
                        $("#ddlTitle").val(ContactData.title);

                        $("#txtContactType").val(ContactData.ContactType);
                        $("#HiddenContactId").val(ContactData.id);
                        SetAccountManager(ContactData.acct_manager_id, "ddlAcctMgr", "AccountManager");
                       // $("#ddlAcctMgr").val(ContactData.acct_manager_id);
                        MakeReadonly();

                    }
                }
            },
            error: function (response) {
                //alert();
            },
            type: 'post'

        });
    }
}
function RefreshCOntactListByOrgId(OrgId) {
    if (OrgId != undefined && OrgId != 0) {

        $.ajax({
            url: '/Contact/GetContactByOrgId',
            data: { OrgId: OrgId },
            async: false,
            success: function (response) {
                //ContactList = response;
                //if (ContactList.length > 0) {

                //    var ContactID = parseInt($("#HiddenContactId").val());

                //    CurrentContactIndex = ContactList.findIndex(x => x.id == ContactID);


                //    $('#btnNext').css("display", "none")
                //    $('#btnPre').css("display", "none")

                //    if (CurrentContactIndex > 0) {
                //        $('#btnPre').css("display", "block")
                //    }
                //    if (CurrentContactIndex < ContactList.length - 1) {
                //        $('#btnNext').css("display", "block")
                //    }
                //}
                ContactList = response;
                if (ContactList.length > 0) {

                    var ContactID = parseInt($("#HiddenContactId").val());

                    CurrentContactIndex = ContactList.findIndex(x => x.id == ContactID);


                    $('#btnNext').css("display", "none")
                    $('#btnPre').css("display", "none")
                    if (CurrentContactIndex != -1) {
                        if (CurrentContactIndex > 0) {
                            $('#btnPre').css("display", "block")
                        }
                        if (CurrentContactIndex < ContactList.length - 1) {
                            $('#btnNext').css("display", "block")
                        }
                    }
                    else {
                        $('#btnunlinkContact').prop('checked', true);
                        CurrentContactIndex = 0;
                        if (ContactList.length > 1) {
                            $('#btnNext').css("display", "block")
                        }
                       if (ContactList.length > 0) {
                            $("#txtContactType").val(ContactList[0].ContactType);
                            $("#ddlTitle").val(ContactList[0].title);
                            $("#txtFirstName").val(ContactList[0].first_name);
                            $("#txtLastName").val(ContactList[0].last_name);
                            $("#txtEmail").val(ContactList[0].email);
                            $("#txtContactNo").val(ContactList[0].mobile);
                            $("#txtContactRole").val(ContactList[0].ContactRole);
                            SetAccountManager(ContactList[0].acct_manager_id, "ddlAcctMgr", "AccountManager");
                           // $("#ddlAcctMgr").val(ContactList[0].acct_manager_id);
                            $("#txtNotes").val(ContactList[0].notes);
                            $("#HiddenContactId").val(ContactList[0].id);
                        }
                        else {
                            ResetForm();
                        }
                    }
                }
                else {
                    $('#btnunlinkContact').prop('checked', true);
                    ResetForm();
                }

            },
            error: function (response) {
                //alert();
            },
            type: 'post'

        });
    }
}

function ResetForm() {
    $("#btnsaveContact").removeClass("customAlertChange");
    $('#txtContactType').val('Customer');
    $('#txtFirstName').val('');
    $('#ddlTitle').val('Mr.');
    $('#txtLastName').val('');
    $('#txtEmail').val('');
    $('#txtContactNo').val('');
    $('#txtContactRole').val('');
    $('#ddlAcctMgr').val('');
    $('#txtNotes').val('');
    $('#HiddenContactId').val('');
    $('#HiddenforDefaultOrgID').val('');
    if ($('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "OrderDetails")
    {
        ResetOrgForm();
    }
    if ($('#PageName').val() == "JobDetails") {

        GetOrganisationAddress(0, "");
        CheckDeliveryAddress(0);
        $('#HiddenforDefaultOrgID').val('');
    }
    $('#txtOrgName').val('');
    $('#ContactisPrimary').prop('checked', false);
    $('#txtContactType').removeAttr('disabled').removeClass('MakeReadonly');
    $('#txtFirstName').removeAttr('readonly').removeClass('MakeReadonly');
    $('#ddlTitle').removeAttr('disabled').removeClass('MakeReadonly');
    $('#txtLastName').removeAttr('readonly').removeClass('MakeReadonly');
    $('#txtEmail').removeAttr('readonly').removeClass('MakeReadonly');
    $('#txtContactNo').removeAttr('readonly').removeClass('MakeReadonly');
    $('#txtContactRole').removeAttr('disabled').removeClass('MakeReadonly');
    $('#ddlAcctMgr').removeAttr('disabled').removeClass('MakeReadonly');
    $('#txtNotes').removeAttr('readonly').removeClass('MakeReadonly');
    CurrentContactIndex = -1;
    $('#btnNext').css("display", "none")
    $('#btnPre').css("display", "none")
    if (ContactList.length >0) {
        $('#btnNext').css("display", "block")
    }
    


}
function DeletelinkwithOpportunity() {
    if ($('#HiddenContactId').val() == "") {
        CustomWarning('Save contact first.');
        $('#btnunlinkContact').prop('checked', true);
    }
    else {
        var ContactExist = ContactList.find(x=>x.id == $('#HiddenContactId').val());
        if (ContactExist == null || ContactExist == undefined) {
            CustomWarning('Contact does not linked with opportunity.');
            $('#btnunlinkContact').prop('checked', true);
        }
    }
}