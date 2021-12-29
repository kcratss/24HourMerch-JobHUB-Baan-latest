var EventChangesValues = [];
$(function () {

    $(".EventContainer :input").focus(function (event) {
        EventChangesValues.push({ id: $(this).attr('id'), value: $(this).val() });
    });
    $(".EventContainer :input").blur(function (event) {
        var OldValues = EventChangesValues.find(x=>x.id == $(this).attr('id'));
        var index = EventChangesValues.findIndex(x=>x.id == $(this).attr('id'));
        if (OldValues != null && OldValues != undefined) {

            if (OldValues.value != $(this).val()) {
                $("#btnsaveevent").addClass("customAlertChange");
            }
            EventChangesValues.splice(index, 1);
        }
    });
    // baans change 15th November for user by title
    if ($("#PageName").val() == "EventList" || $("#PageName").val() == "Event") {
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
    //16 Aug 2018 (N)
    if ($('#PageName').val() == "Event") {
        $('#DivQuote').css("display", "none");
        $('#lblRepeat').css("display", "none");
        $('#btnOpenOpportunity').css("display", "block")
        //$('#btnContactreset').css("display", "none");
        $('#lblEventId').text('0000');
        $('#lblOppoName').html('Opportunity Name <span class="reqField" style="color:red;">*</span>');
        $('#lbltype').css("display", "none");
        $('#lblorgname').css("display", "block");
        $('#txtContactType').css("display", "none");
        $('#txtOrgName').css("display", "block");
        $('#lblEventId').css("display", "Block");
        $('#lblEventHeading').css("display", "none");
        $('#btnsaveevent').css("display", "block");
        $('#openevent').css("display", "none");

    }
    //16 Aug 2018 (N)
    $("#txtnextDate").datepicker(
         {
             dateFormat: 'dd/mm/yy',
             onSelect: function (dateText) {
                 var OldValues = EventChangesValues.find(x=>x.id == "txtnextDate");
                 var index = EventChangesValues.findIndex(x=>x.id == "txtnextDate");
                 if (OldValues != null && OldValues != undefined) {

                     if (OldValues.value != $(this).val()) {
                         $("#btnsaveevent").addClass("customAlertChange");

                     }
                     EventChangesValues.splice(index, 1);
                 }
                 SetEventDate('txtnextDate');
             }
         }
             );
    $("#txteventDate").datepicker(
 {
     dateFormat: 'dd/mm/yy',
     onSelect: function (dateText) {
         var OldValues = EventChangesValues.find(x=>x.id == "txteventDate");
         var index = EventChangesValues.findIndex(x=>x.id == "txteventDate");
         if (OldValues != null && OldValues != undefined) {

             if (OldValues.value != $(this).val()) {
                 $("#btnsaveevent").addClass("customAlertChange");

             }
             EventChangesValues.splice(index, 1);
         }
         SetEventDate('txteventDate');
     }
 }
      );
    


});
function saveEvent()
{
    
        var checkflag = true;
    var txtEventName, txteventDate, txteventCycle, txtnextDate, txtEventLocation, txtEventWebsite, txtEventNotes;
    var OppData = new Object();
    if ($("#txtEventName").val() != "") {
        txtEventName = $("#txtEventName").val();
        //$("#txtEventName").css("border-color", "rgba(204, 204, 204, 1)");
        $("#txtEventName").removeClass('customAlertChange');
    } else {
        //$("#txtEventName").css("border-color", "red");
        $("#txtEventName").addClass('customAlertChange');
        checkflag = false;
    }

    txteventDate = GetDate($("#txteventDate").val());
    txteventCycle = $("#txteventCycle").val();
    txtnextDate = GetDate($("#txtnextDate").val());
    txtEventLocation = $("#txtEventLocation").val();
    txtEventWebsite = $("#txtEventWebsite").val();
    txtEventNotes = $("#txtEventNotes").val();

    if ($('#HiddenEventId').val() != "") {
        eventid = $('#HiddenEventId').val();
    }
    else {
        eventid = 0;
    }
    //16 Aug 2018 (N)
    if (checkflag == true) {
        $.ajax({
            url: '/Event/AddEvent',
            data: { EventId: $('#HiddenEventId').val(), EventName: txtEventName, EventDate: txteventDate, EventCycle: txteventCycle, EventLocation: txtEventLocation, EventWebsite: txtEventWebsite, EventNotes: txtEventNotes, NextDate: txtnextDate, OpportunityId: $('#lblOpportunityId').text(), PageSource: $("#PageName").val() },
            async: false,

            success: function (response) {
                if (response.ID != null && response.ID != undefined) {
                    if ($('#lblEventId').text() == "0000") {
                        history.pushState('', '', '/Event/EventDetails/' + response.ID);
                    }
                    $("#btnsaveevent").removeClass("customAlertChange");
                    $('#HiddenEventId').val(response.ID);
                    if ($("#PageName").val() == "Opportunity") {
                        $("#HiddenOppEventId").val(response.ID);
                        MakeEventReadonly();
                    }
                    // 22 Aug 2018 (N)
                    if ($("#PageName").val() == "Event") {
                        var id = "0000" + response.ID;
                        var newid = id.substring(id.length - 4);
                        $("#lblEventId").text(newid);                 //lbl id
                    }
                    // 22 Aug 2018 (N)
                    // baans change 16th November 
                    $("#txtnextDate").removeClass("customAlertChange");
                    $("#txteventDate").removeClass("customAlertChange");
                    // baans end 16th November
                }
                CustomAlert(response)
            },
            type: 'post',

        });
    }
    else
    {
        //CustomWarning('Fill all required fields.');
        CustomErrorCode("Required");
    }

}
function AddEvent() {
    // baans change 29th Sept for checking the event Validation
    var IsEventValid = true;
    var filter = /^[^-\s][a-zA-Z0-9_.\s-]+$/; 
    if (filter.test($('#txtEventName').val())) {
        $("#txtEventName").removeClass("customAlertChange");
    }
    else {
        IsEventValid = false;
        $("#txtEventName").addClass("customAlertChange");
    }
    if (IsEventValid) {
        if ($('#lblOpportunityId').text() != "000000") {
            if ($("#HiddenOppEventId").val() != "") {
                if ($("#HiddenOppEventId").val() == $('#HiddenEventId').val()) {
                    saveEvent();
                }
                else {
                    bootbox.confirm('Event for this Opportunity already exist. Do you wish to change?', function (result) {
                        if (result) {
                            saveEvent();
                        }
                    });
                }

            }
            else {
                saveEvent();
            }

        }
        else {
            CustomWarning('Save opportunity first.');
        }
    }
    else {
        //CustomWarning('Fill all required fields.');
        CustomErrorCode("Required");
    }

}


function GetEventById(EventId) {
    if (EventId != undefined && EventId != 0) {
        $.ajax({
            url: '/Event/GetEventById',
            data: { EventId: EventId },
            async: false,
            success: function (response) {
                $("#txtEventName").val(response.EventName);
                $("#txteventDate").val(DateFormat(response.EventDate));
                $("#txtEventLocation").val(response.EventLocation);
                $("#txtEventWebsite").val(response.EventWebsite);
                $("#txtEventNotes").val(response.EventNotes);
                $("#txtnextDate").val(DateFormat(response.NextDate));
                $("#txteventCycle").val(response.EventCycle);
                $("#HiddenEventId").val(response.EventId);
                $("#lblEventId").text(response.DispalayId);
                MakeEventReadonly();
            },
            error: function (response) {
                alert(response)
            },
            type: 'post'

        });
    }
}
function EventsListGrid(activetab) {
    var data;
    var model;
    var url = '/Event/GetEventList';
    if (activetab == 'Custom') {
        // baans change 4th December for Custom
        //model = { CustomText: $('#searchtextbox').val(), TableName: 'tblEvent' };
        //url = '/Event/GetCustomEventList';
        var searchdata = $('#searchtextbox').val();
        if (searchdata != "") {
            model = { CustomText: $('#searchtextbox').val(), TableName: 'tblEvent' };
            url = '/Event/GetCustomEventList';
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
            data = response;
        },
        type: 'post'
    });
    var obj = {
        selectionModel: { type: 'row' },
        scrollModel: { horizontal: false },
        pageModel: { type: "local", rPP: 50 },
        strNoRows: 'No records found for the selected Criteria',
        editable: false,
        width: "97%",
        wrap: false,
        hwrap: false,
        height: 782,
        columnTemplate: { width: 120, halign: "left" },
        numberCell: { width: 0, title: "", minWidth: 0 },
        sortable: true,
       // sortModel: { cancel: false, type: 'local', sorter: [{ dataIndx: 1, dir: ['up', 'down', 'up'] }] }
    };
    obj.colModel = [
        {
            title: "Event No", dataIndx: "DispalayId", align: "center", width: "6%", datatype: "integer",
            render: function (ui) {
                var id = ui.cellData;
                //return "<a href='/Event/EventDetails/" + id + "' class='nav-link' style='text-decoration:underline;color:#00adef !important;'><label style='cursor:pointer;width:100%;color:#00adef !important'>" + id + "</label></a>";
                return "<a href='/Event/EventDetails/" +parseInt(id)+ "'><label class='internalLnk'>" + id + "</label></a>";
            }
        },
        {
            title: "Next Date", dataIndx: "NextDate", width: "6%", align: "left", datatype: "date",    //tarun 22/09/2018
            render: function (ui) {
                var date = ui.cellData;
                if (date != null && date != undefined && date != "") {
                    //var nowDateopp = new Date(parseInt(date.substr(6)));
                    //return ("0" + nowDateopp.getDate()).slice(-2) + '/' + ("0" + (nowDateopp.getMonth() + 1)).slice(-2) + '/' + nowDateopp.getFullYear();
                    // baans change 22nd November
                    return ListDateFormat(date);
                    // baans end 22nd November
                }
            },
        },
        {
            title: "Event Name", dataIndx: "EventName", width: "15%", datatype: "string"
        },
        {
            title: "Event Location", dataIndx: "EventLocation", width: "12%", datatype: "string"
        },
         {
             title: "Event Notes", dataIndx: "EventNotes", width: "63%", datatype: "string"
         },
    ];
    obj.dataModel = { data: data };

    pq.grid("#EventGrid", obj);
    pq.grid("#EventGrid", "refreshDataAndView");
}

function OpenEvent() {
    if ($('#HiddenEventId').val() != "") {
        location.href = "/Event/EventDetails/" + $('#HiddenEventId').val();
    }
    else {
        $('#CustomAlert').removeClass('j-alertBox-Failed').removeClass('j-alertBox-Success').css('display', 'none');
        $('#CustomAlert').addClass('j-alertBox-Warning').css('display', 'block');
        $('#alertMsg').html('<strong>Warning !</strong> Select Event First');
        setTimeout(function () {
            $('#CustomAlert').fadeOut('slow', function () {
                $('#CustomAlert').removeClass('j-alertBox-Warning').css('display', 'none');
                $('#alertMsg').html('');

            });
        }, 5000);
    }
}
function ResetEventForm() {
    $("#btnsaveevent").removeClass("customAlertChange");
    $("#txtEventName").val('');
    $("#txteventDate").val('');
    $("#txteventCycle").val('');
    $("#txtnextDate").val('');
    $("#txtEventLocation").val('');
    $("#txtEventWebsite").val('');
    $("#txtEventNotes").val('');
    $("#HiddenEventId").val('');
    $('#txtEventName').removeAttr('readonly', true).removeClass('MakeReadonly');
    $('#txteventDate').removeAttr('readonly', true).removeClass('MakeReadonly');
    $('#txteventCycle').removeAttr('readonly', true).removeClass('MakeReadonly');
    $('#txtnextDate').removeAttr('readonly', true).removeClass('MakeReadonly');
    $('#txtEventLocation').removeAttr('readonly', true).removeClass('MakeReadonly');
    $('#txtEventWebsite').removeAttr('readonly', true).removeClass('MakeReadonly');
    $('#txtEventNotes').removeAttr('readonly', true).removeClass('MakeReadonly');
}
function MakeEventReadonly() {
    //if ($('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "ContactDetails") {
    if ($('#PageName').val() == "Opportunity") {

        $('#txtEventName').attr('readonly', true).addClass('MakeReadonly');
        $('#txteventDate').attr('disabled', true).addClass('MakeReadonly');
        $('#txteventCycle').attr('disabled', true).addClass('MakeReadonly');
        $('#txtnextDate').attr('disabled', true).addClass('MakeReadonly');
        $('#txtEventLocation').attr('readonly', true).addClass('MakeReadonly');
        $('#txtEventWebsite').attr('readonly', true).addClass('MakeReadonly');
        $('#txtEventNotes').attr('readonly', true).addClass('MakeReadonly');
    }
}
function SetEventDate(InputID) {
    var txtnextDate = GetDate($('#txtnextDate').val());
    var txteventDate = GetDate($('#txteventDate').val());
    if (txtnextDate != "" && txtnextDate != undefined && txtnextDate != null && txteventDate != "" && txteventDate != undefined && txteventDate != null) {
        var txtnextDatenew = new Date(txtnextDate);
        var txteventDatenew = new Date(txteventDate);
        if (txtnextDatenew <= txteventDatenew) {
            $('#' + InputID).val('');
            if (InputID == "txtnextDate") {
                $("#txtnextDate").addClass("customAlertChange");
                $("#txteventDate").removeClass("customAlertChange");
                CustomWarning('Next date should be greater than Event Date');
            }
            else {
                $("#txtnextDate").removeClass("customAlertChange");
                $("#txteventDate").addClass("customAlertChange");
                CustomWarning('Event date should be less than Next Date');
            }
        }

    }
}

function GetOpportunityByEventId(Stage, EventId) {
    if (EventId != undefined && EventId != 0) {
        $.ajax({
            url: '/Event/GetOpportunityByEventId',
            data: { Stage: Stage, EventId: EventId },
            async: false,
            success: function (response) {
                if (response != undefined) {
                    if (response.length > 0) {
                        GetOppById(response[0].OpportunityId);
                        GetContactByOppId(response[0].OpportunityId);
                        MakeOpportunityReadonly();

                    }
                }
                //GetOppById(response[0].OppName)
                //$('#oppName').val(response[0].OppName);
                //$('#oppQuantity').val(response.Quantity);
                //$('#datepicker').val(DateFormat(response.ReqDate));
                //$('#depositreqdate').val(DateFormat(response.DepositReqDate));
                //$('#OppShipping').val(response.Shipping);
                //$('#txtshippingday').val(response.ShippingTo);
                //$('#txtshippingprice').val(response.Price);
                //$('#oppSource').val(response.Source);
                //$('#oppCampaign').val(response.Compaign);
                //$('#txtrepeatfrom').val(response.RepeatJobId);
                //$('#txtConfirmedDate').val(DateFormat(response.ConfirmedDate))
                ////$('#oppStage').val()
                //SetAccountManager(response.AcctManagerId, "ddlOppAcctMgr", "OppAccountManager");

            },
            type: 'post'
        });
    }
}

function GetOppoListByEvent(Stage, EventId) {
    //alert(Stage +" "+ContactId);
    var data;
    $.ajax({
        url: '/Event/GetOpportunityByEventId',
        data: { Stage: Stage, EventId: EventId },
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
        //pageModel: { type: "local", rPP: 10 },
        strNoRows: 'No records found for the selected Criteria',
        scrollModel: { horizontal: false },
    };

    obj.width = "96.5%";
    obj.height = 270;
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
            title: "Date", dataIndx: "OppDate", align: "right", width: "7%", dataType: "date", editable: false,
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
            title: "Name", dataIndx: "OppName", width: "10%", dataType: "string"
        },
        {
            title: "Qty", dataIndx: "Quantity", align: "right", width: "4%", dataType: "int", editable: false
        },
        {
            title: "Required By", dataIndx: "ReqDate", align: "right", width: "8%", dataType: "date", editable: false,
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
              title: "Event", dataIndx: "EventName", width: "25%", dataType: "string", editable: false,
          }

    ];



    //obj.selectChange = function (evt, ui) {
    //    //alert("select change");
    //    var row = getRowIndx(this);
    //    if (row != null) {
    //        var data = this.pdata[row];
    //        var dataid = data.OpportunityId;
    //        if (data.Stage == "Opportunity" || data.Stage == "Quote") {
    //            ContactDetailsListDown(dataid, "Opp");
    //        }
    //        else {
    //            ContactDetailsListDown(dataid, "Order");
    //        }

    //    }
    //}


    obj.rowDblClick = function (evt, ui) {


        var rowIndx = getRowIndx(this);
        if (rowIndx != null) {
            var data = this.pdata[rowIndx];
            if (data != undefined && data != null) {
                var id = data.DisplayOpportunityId;
                $('#btnNext').css("display", "none")
                $('#btnPre').css("display", "none")
                GetOppById(id);
                GetContactByOppId(id);
            }

            //var dataid = data.OpportunityId;
            //ContactDetailsListDown(dataid);
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

    obj.dataModel = { data: data };
    pq.grid("#EventGrid", obj);
    pq.grid("#EventGrid", "refreshDataAndView");

}
//17 Aug 2018 (N)