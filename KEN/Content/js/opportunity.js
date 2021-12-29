var ms;
var OppIsDIrty = false, OppFucusInvalue = "", OppFucusoutvalue = "";
var ChangesValues = [];

$(function () {
    $(".getImage").click(function () {
        var hostname = window.location.origin;
        var currentId = $(this).attr('id').split('~');
        $("#PrintImageModal").css("display", "block");
        var dataImage = $("#" + currentId[0] + "hdnimg").val();

        $("#printimage").attr("src", hostname + "/Content/uploads/Opportunity/" + dataImage);
    });
    $(".deleteImage").click(function () {
        var currentId = $(this).attr('id').split('~');
        bootbox.confirm("Do you wish to delete?", function (result) {
            if (result) {
                var OppId = $('#lblOpportunityId').text();
                $.ajax({

                    url: '/Opportunity/DeleteOppImage',
                    data: { Type: currentId[0], OppId: OppId },
                    async: false,
                    success: function (response) {
                        data = response;

                        CustomAlert(response);
                        if (response.ID > 0) {
                            //getAllImages(response.ID);
                            if (response.Result == "Success") {
                                if (currentId[0] == "left") {
                                    $("#lefthdnimg").val('');
                                    $(".leftdetails").css("display", "none");
                                }
                                if (currentId[0] == "right") {
                                    $("#rightthdnimg").val('');
                                    $(".rightdetails").css("display", "none");
                                }
                                if (currentId[0] == "front") {
                                    $("#fronthdnimg").val('');
                                    $(".frontdetails").css("display", "none");
                                }
                                if (currentId[0] == "back") {
                                    $("#backthdnimg").val('');
                                    $(".backdetails").css("display", "none");
                                }
                            }
                            GetInquiryData(response.ID);
                            CustomAlert(response);
                        }

                    },
                    type: 'post',

                });
            }
        });

    });
    //if ($("#PageName").val() == "QuoteDetails" || $("#PageName").val() == "OrderDetails")
    //{
    //    $('#btnOptionEmail').css("display", "block");
    //    $('#btnOptionPrint').css("display", "block");
    //    $('#btnOptionOrder').css("display", "block");
    //}
    if ($("#PageName").val() == "Opportunity") {
        $('#lblOppoName').html('Opportunity Name <span class="reqField" style="color:red;">*</span>');
    }
    if ($("#PageName").val() == "OrderDetails") {
        $('#lblOppoName').html('Order Name <span class="reqField" style="color:red;">*</span>');
    }
    if ($("#PageName").val() == "JobDetails") {
        $('#lblOppoName').html('Job Name <span class="reqField" style="color:red;">*</span>');
    }
    if ($("#PageName").val() != "Opportunity") {
        $('#DivQuote').css("display", "block");
        $('#lblRepeat').css("display", "block");
    }
    if ($("#PageName").val() != "Opportunity") {
        $("#spanShippingTo,#spanShiping,#SpanReqDate,#spanShipingPrice").css("display", "block");
    }
    //20 Aug 2018 (N)
    if ($("#PageName").val() == "Event") {
        $('#lbltotlaMargin').css("display", "none");
        $('btnOpenOpportunity').css("display", "block");
        $("#spanShippingTo,#spanShiping,#SpanReqDate,#spanShipingPrice").css("display", "none");
    }
    //20 Aug 2018 (N)
    // 17 Sep 2018 (N)
    if ($("#PageName").val() == "InvoicingDetails" || $("#PageName").val() == "PackingDetails" || $("#PageName").val() == "ShippingDetails") {
        $('#lblOppoName').html('Job Name <span class="reqField" style="color:red;">*</span>');
        $('#btnOptionCopy').css("display", "none");
        $('.OptionRowSizeType').css("display", "block");
        $('.OptionRowLink').css("display", "none");
        $('#lblAddress').text("Shipping");
        //$('#btnOptionSave').attr('disabled', true).addClass('MakeReadonly');
        $('#ddlSizeType').attr('disabled', true).addClass('MakeReadonly');
        $('#txtSizes').attr('disabled', true).addClass('MakeReadonly');  
    }
    if ($("#PageName").val() == "InvoicingDetails" || $("#PageName").val() == "CompleteDetails" || $("#PageName").val() == "ShippingDetails") {
    $('#txtSizesPacked').attr('disabled', true).addClass('MakeReadonly');
    $('#txtSizes').attr('disabled', true).addClass('MakeReadonly');  
    }

    if ($("#PageName").val() == "InvoicingDetails") {
        $('#btnOptionList').text("EMAIL"); /*Tarun Change */
    }

    // baans change 1st November for hide the pay button on complete screen
    if ($("#PageName").val() == "CompleteDetails") {
        //$('#btnOptionPay').css("display", "none");
        //$('#btnOptionHist').css("margin-left", "84px"); 
        //$('#btnOptionList').css("margin-left", "155px");
        //$('#btnOptionRepeat').css("margin-left", "225px");
        //$('#btnOptionLabel').css("margin-left", "295px");
        $('#btnOptionPrint').css("display", "none");
        $('#btnOptionPayBlank').css("display", "none");
        $('#btnOptionHistBlank').css("display", "none");
        $('#btnViewProof').css("display", "none");
        $('#btnSendProof').css("display", "none");
        $('#btnOptionPay').css("display", "none");

    }
   
    if ($("#PageName").val() == "ShippingDetails") {
        //$('#btnOptionSave').css("display", "none");
        $('#btnOptionPay').css("margin-left", "8px");
        //$('#btnOptionLabel').css("margin-left", "147px");
        $('#btnOptionPrint').css("display", "none");
        $('#btnOptionPayBlank').css("display", "none");
        $('#btnOptionHistBlank').css("display", "none");
        $('#btnViewProof').css("display", "none");
        $('#btnSendProof').css("display", "none");
        $('#btnOptionPay').css("display", "none"); 
        $('#btnOptionList').css("display", "none");

    }
    if ($("#PageName").val() == "InvoicingDetails") {
        $('#btnOptionPay').css("margin-left", "8px");
       // $('#btnOptionSave').css("margin-left", "73px");
        $('#btnOptionPrint').css("margin-left", "0px");
        $('#btnOptionPayBlank').css("display", "none");
        $('#btnOptionHistBlank').css("display", "none");
        $('#btnSendProof').css("display", "none");
        $('#btnOptionLabel').css("display", "none");
    }
    if ($("#PageName").val() == "PackingDetails") {
        $('#btnOptionPay').css("margin-left", "8px");
        //$('#btnOptionSave').css("margin-left", "144px");
        $('#btnOptionPrint').css("margin-left", "0px");
        $('#btnOptionPayBlank').css("display", "none");
        $('#btnOptionHistBlank').css("display", "none");
        $('#btnSendProof').css("display", "none");
    }
    if ($("#PageName").val() == "JobDetails") {
        $('#btnOptionPay').css("margin-left", "8px");
        //$('#btnOptionCopy').css("margin-left", "144px");
        $('#btnOptionPrint').css("margin-left", "0px");
        $('#btnOptionPayBlank').css("display", "none");
        $('#btnOptionHistBlank').css("display", "none");
        $('#btnOptionCopy').css("display","none");
    }
    if ($("#PageName").val() == "OrderDetails") {
        $('#btnOptionPay').css("margin-left", "8px");
        $('#btnOptionPayBlank').css("display", "none");
        $('#btnOptionHistBlank').css("display", "none");
    }
    if ($("#PageName").val() == "QuoteDetails") {
        $('#btnOptionPay').css("display", "none");
        $('#btnOptionHist').css("display", "none");       
    }
    // baans end 1st November

    // baans change 06th October for Required on Packing and shipping
    if ($("#PageName").val() == "InvoicingDetails" || $("#PageName").val() == "PackingDetails" || $("#PageName").val() == "ShippingDetails" || $("#PageName").val() == "CompleteDetails") {
        //$('#lblpackedIn1').html('Packed In <span class="reqField speField" style="color:red;">*</span>');
        if ($("#PageName").val() != "PackingDetails") {
            $('#lblConNoteNo').html('Con Note No <span class="reqField speField" style="color:red;">*</span>');
            
        }
    }
    // baans end 06th October
    // baans change 15th November for user by title
    if ($("#PageName").val() == "OpportunityList" || $("#PageName").val() == "Opportunity" || $("#PageName").val() == "QuoteDetails" || $("#PageName").val() == "OrderDetails" || $("#PageName").val() == "JobDetails" || $("#PageName").val() == "CompleteDetails" || $("#PageName").val() == "ShippingDetails" || $("#PageName").val() == "PackingDetails" || $("#PageName").val() == "InvoicingDetails")    {
        $.ajax({
            url: '/Opportunity/GetUserByTitle',
            data: { },
            async: false,
            success: function (response) {
                //if (response.AdminRight == true) {
                //    $("#profileOfUser").append('<option value= "All">' + "All Records" + '</option>');
                //}
                //    if (response.CurrentUser == 0) {
                //        var All = "All";
                //        $('#profileOfUser').val(All);
                //    }
                //    else
                //    {
                //        if(response.CurrentUser != ""){
                //            $('#profileOfUser').val(response.CurrentUser);
                //        }
                //    }
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
       
            //var dataOfUserProfile = $("#hiddenUserProfile").val();
            //if (dataOfUserProfile == "") {
            //    $.ajax({
            //        url: '/Opportunity/GetUserByTitle',
            //        data: {},
            //        async: false,
            //        success: function (response) {
            //            if (response != "") {
            //                $('#profileOfUser').val(response);
                            
            //            }
                        
            //        },
            //        type: 'post',
            //    });
            //}
            //else {
            //    $('#profileOfUser').val(dataOfUserProfile);
            //}
        
        //$('#profileOfUser').val("Hitesh Sindhu");
    }
    // baans end 15th November
    
    setTimeout(function () {
        $(".chosen").chosen();
    }, 500);
    // Baans change 24th September for CompleteDetails
    if ($("#PageName").val() == "CompleteDetails") {
        
        $("#btnOptionCopy").css("display", "none");
        $("#btnOptionSave").css("display", "none");
        $("#btnOptionPro").css("display", "none");
        $('#lblAddress').text("Shipping");
        $('#lblOppoName').html('Job Name <span class="reqField" style="color:red;">*</span>');
        $("#btnOptionRepeat").css("display", "initial");
        
    }
    // baans end 24th September
    if ($("#PageName").val() == "Opportunity" || $("#PageName").val() == "QuoteDetails" || $("#PageName").val() == "OrderDetails") {
        var data = $("#PageName").val();
        if ($("#PageName").val() == "Opportunity") {
            
            $('#oppCampaign').val("3");
        }
        
        $('#ddlservice').val("Standard - 2 weeks*");
    }
    // baans change 5th October for default service and Campaign value

    // baans change 12th November for default margin value
    if ($("#PageName").val() == "Opportunity" || $("#PageName").val() == "QuoteDetails" || $("#PageName").val() == "OrderDetails" || $("#PageName").val() == "JobDetails") {
        $('#txtMargin').val("35");
    }
    // baans change 11th January
    if ($("#PageName").val() == "Opportunity" || $("#PageName").val() == "QuoteDetails" || $("#PageName").val() == "OrderDetails" || $("#PageName").val() == "JobDetails") {
        $("#ddlSizeType").val("Custom");
        $("#txtSizes").val("TBC");
    }
    // baans end 11th January
    // baans end 12th November

    // baans end 5th October
    // baans end 05th Sept
    //else {
    //    alert();
    //    $('.OptionRowLink').css("display", "block");
    //}
    //17 Sep 2018 (N)
    //$('#ddlOppAcctMgr').change(function () {
    //    if ($('#ddlOppAcctMgr').val() != "") {
    //        var AcManagerShortName = "";
    //        var ShortName = $("#ddlOppAcctMgr option:selected").text().split(' ');
    //        for (var i = 0; i < ShortName.length; i++) {
    //            AcManagerShortName += ShortName[i].charAt(0);
    //        }
    //        $('#lblManagerName').text(AcManagerShortName);
    //        // $('#ddlOppAcctMgr').val= AcManagerShortName;
    //    }
    //    else {
    //        $('#lblManagerName').text('');
    //    }
    //});
    $('#ddlOppAcctMgr').change(function () {
        if ($('#ddlOppAcctMgr').val() != "") {
            // baans change 22nd August for getting the name of accountManager
            //var AcManagerShortName = "";
            //var ShortName = $("#ddlOppAcctMgr option:selected").text().split(' ');
            //for (var i = 0; i < ShortName.length; i++) {
            //    AcManagerShortName += ShortName[i].charAt(0);
            //}
            //$('#lblManagerName').text(AcManagerShortName);
            var AcManagerShortName = "";
            var first = "";
            var Last = "";
            $.ajax({
                url: '/Opportunity/GetAccountManagerById',
                data: { Id: $('#ddlOppAcctMgr').val() },
                async: false,
                success: function (response) {
                    first = response.FirstName;
                    AcManagerShortName += first[0].charAt(0);
                    Last = response.LastName;
                    AcManagerShortName += Last[0].charAt(0);
                    $('#lblManagerName').text(AcManagerShortName);
                },
                type: 'post',
            });
            // baans end 22nd August
        }
        else {
            $('#lblManagerName').text('');
        }
    });

    $(".OptionContainer :input").focus(function (event) {
        ChangesValues.push({ id: $(this).attr('id'), value: $(this).val() });
    });

    $(".OptionContainer :input").blur(function (event) {
        var OldValues = ChangesValues.find(x=>x.id == $(this).attr('id'));
        var index = ChangesValues.findIndex(x=>x.id == $(this).attr('id'));
        if (OldValues != null && OldValues != undefined) {

            if (OldValues.value != $(this).val()) {
                $("#btnOptionSave").addClass("customAlertChange");

            }
            ChangesValues.splice(index, 1);
        }
    });
    $("#ddlinclude").change(function () {
        $("#btnOptionSave").addClass("customAlertChange");
    });

    $(".OpportunityContainer :input").focus(function (event) {
        ChangesValues.push({ id: $(this).attr('id'), value: $(this).val() });
    });
    $(".OpportunityContainer :input").blur(function (event) {
        var OldValues = ChangesValues.find(x=>x.id == $(this).attr('id'));
        var index = ChangesValues.findIndex(x=>x.id == $(this).attr('id'));
        if (OldValues != null && OldValues != undefined) {

            if (OldValues.value != $(this).val()) {
                $("#btnOppSubmit").addClass("customAlertChange");

            }
            ChangesValues.splice(index, 1);
        }
    });

    $("#txtConfirmedDate").datepicker(
        {
            dateFormat: 'dd/mm/yy',
            onSelect: function (dateText) {
                var OldValues = ChangesValues.find(x=>x.id == "txtConfirmedDate");
                var index = ChangesValues.findIndex(x=>x.id == "txtConfirmedDate");
                if (OldValues != null && OldValues != undefined) {

                    if (OldValues.value != $(this).val()) {
                        $("#btnOppSubmit").addClass("customAlertChange");

                    }
                    ChangesValues.splice(index, 1);
                }
            }
        }
            );
    $("#datepicker").datepicker(
          {
              dateFormat: 'dd/mm/yy',
              onSelect: function (dateText) {
                  var OldValues = ChangesValues.find(x=>x.id == "datepicker");
                  var index = ChangesValues.findIndex(x=>x.id == "datepicker");
                  if (OldValues != null && OldValues != undefined) {

                      if (OldValues.value != $(this).val()) {
                          $("#btnOppSubmit").addClass("customAlertChange");

                      }
                      ChangesValues.splice(index, 1);
                  }

                  SetDate('datepicker');
              }
          }
              );

    $("#depositreqdate").datepicker(
 {
     dateFormat: 'dd/mm/yy',
     onSelect: function (dateText) {
         var OldValues = ChangesValues.find(x=>x.id == "depositreqdate");
         var index = ChangesValues.findIndex(x=>x.id == "depositreqdate");
         if (OldValues != null && OldValues != undefined) {

             if (OldValues.value != $(this).val()) {
                 $("#btnOppSubmit").addClass("customAlertChange");

             }
             ChangesValues.splice(index, 1);
         }

         SetDate('depositreqdate');
     }
 }
      );


    //$("#fromdate").datepicker(
    //    {
    //        // baans change 26th October
    //        firstDay: 1,
    //        //changeMonth: true,
    //        //changeYear: true,
    //        // baans end 26th october
    //        //dateFormat: 'dd/mm/yy',
    //        dateFormat: 'dd-mm-yy',
    //        onSelect: function (dateText) {
    //            CheckDate('fromdate');
    //        }
    //    }
    //        );
    //$("#todate").datepicker(
    //    {
    //        // baans change 26th October
    //        firstDay: 1,
    //        //changeMonth: true,
    //        //changeYear: true,
    //        // baans end 26th october
    //        dateFormat: 'dd-mm-yy',
    //        onSelect: function (dateText) {

    //        CheckDate('todate');
    //    }
    //}
    //  );
});


 //tarun change 17th August for option Grid
$(function () {
    if ($("#PageName").val() == "PurchaseDetails") {
        $('#DetailTab').css("display", "block");
        $('#options').css("display", "block");
        //$('#ddlSizeType').css("display", "block");
        //$('#lblSizeType').css("display", "block");


    }
})

 //tarun end
$(function () {

    var newdata = [];
    $.ajax({
        url: '/Department/GetDepartmentList',
        data: {},
        async: false,
        success: function (response) {
            newdata = response;
            //alert(JSON.stringify(newdata));
        },
        type: 'get',
    });

    ms = $('#ms1').magicSuggest({
        allowFreeEntries: false,
        useTabKey: true,
        autoSelect: false,
        //maxSuggestions:2,
        // DepartmentList is created from here
        data: newdata,
    });
    $(ms).on('focus', function (c) {
        ChangesValues.push({ id: 'ms1', value: "'" + this.getValue() + "'" });
    });
    $(ms).on('selectionchange', function (e, m) {
        var OldValues = ChangesValues.find(x => x.id == "ms1");
        var index = ChangesValues.findIndex(x => x.id == "ms1");
        if (OldValues != null && OldValues != undefined) {
            if (OldValues.value != "'" + this.getValue() + "'") {
                $("#btnOppSubmit").addClass("customAlertChange");
            }
            ChangesValues.splice(index, 1);
        }
    });
    //Baans Change (Client Comment)
    $("#OppShipping").on("change", function () {
        if ($(this).val() == "PickUp") {
            $("#txtshippingday").val($(this).val());
            $("#txtshippingprice").val('0');
        }
        else {
            $("#txtshippingday").val('');
            if ($("#txtshippingprice").val() == "0") {
                $("#txtshippingprice").val('');
            }
        }
    });

    $("#txtPaymentMethod").on("change", function () {
        if ($(this).val() == "PAYPAL") {
            $("#txtFieldValue").val('PayPal Account');
        }
        else {
            $("#txtFieldValue").val('Common Account');
        }
    });

});
function SaveOpportunity() {
    // baans change 28th September for Validation for space in required Fields
    var IsValid = true;
    var filter = /^[^-\s][a-zA-Z0-9_.\s-]+$/;
    var data = $('#PageName').val();
    if ($('#PageName').val() == "Opportunity" || $('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "OrderDetails" || $('#PageName').val() == "JobDetails" || $('#PageName').val() == "PackingDetails" || $('#PageName').val() == "InvoicingDetails" || $('#PageName').val() == "ShippingDetails" || $('#PageName').val() == "CompleteDetails") {
        if (filter.test($('#oppName').val())) {
            $("#oppName").removeClass("customAlertChange");
            //$("#oppQuantity").removeClass("customchangecheck");
        }
        else {
            IsValid = false;
            $("#oppName").addClass("customAlertChange");
            //$("#oppQuantity").addClass("customchangecheck");
        }
        
    }
    if ($('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "OrderDetails" || $('#PageName').val() == "JobDetails" || $('#PageName').val() == "PackingDetails" || $('#PageName').val() == "InvoicingDetails" || $('#PageName').val() == "ShippingDetails" || $('#PageName').val() == "CompleteDetails")  
    {
        
        
        if (filter.test($('#txtshippingday').val())) {
            $("#txtshippingday").removeClass("customAlertChange");
        }
        else {
            IsValid = false;
            $("#txtshippingday").addClass("customAlertChange");
           
            
        }
    }
    if ($('#PageName').val() == "JobDetails" || $('#PageName').val() == "PackingDetails" || $('#PageName').val() == "InvoicingDetails" || $('#PageName').val() == "ShippingDetails" || $('#PageName').val() == "CompleteDetails") {
        
        if ($('#txtConfirmedDate').val() == "") {
            $("#txtConfirmedDate").addClass("customAlertChange");
            IsValid = false;

        }
        else {
            $("#txtConfirmedDate").removeClass("customAlertChange");
        }
    }
    // baans change 5th October for check valid fields
    if ($("#datepicker").val() != "") {
        $("#datepicker").removeClass('customAlertChange');
    } else {
        $("#datepicker").addClass('customAlertChange');

    }

    if ($("#oppSource").val() != "") {

        $("#oppSource").removeClass('customAlertChange');
    } else {

        $("#oppSource").addClass('customAlertChange');

    }

    if ($("#oppQuantity").val() != "" && $("#oppQuantity").val() != "0" && parseInt($("#oppQuantity").val()) > 0) {

        $("#oppQuantity").removeClass('customAlertChange');
    } else {

        $("#oppQuantity").addClass('customAlertChange');
    }


    if ($('#ms1').magicSuggest().getValue() != "") {
        $('#ms1').removeClass('customAlertChange');
    } else {
        $('#ms1').addClass('customAlertChange');
    }

    if ($("#ddlOppAcctMgr").val() != "") {
        $("#ddlOppAcctMgr").removeClass('customAlertChange');
    } else {
        $("#ddlOppAcctMgr").addClass('customAlertChange');
    }

    if ($('#PageName').val() != "Opportunity") {
        if ($("#depositreqdate").val() != "") {
            $("#depositreqdate").removeClass('customAlertChange');
        } else {
            $("#depositreqdate").addClass('customAlertChange');
        }

        if ($("#OppShipping").val() != "") {
            $("#OppShipping").removeClass('customAlertChange');
        } else {
            $("#OppShipping").addClass('customAlertChange');
        }

        if ($("#txtshippingprice").val() != "") {
            $("#txtshippingprice").removeClass('customAlertChange');
        }
        else {
            $("#txtshippingprice").addClass('customAlertChange');
        }
    }
    // baans end 5th October
    if (IsValid) {
        SaveOppo();
    }
    else {
        //CustomWarning('Fill all required fields.');
        CustomErrorCode("Required");
    }

}
function SaveOppo() {
    var checkflag = ValidateOpportunity();
    if (checkflag == true) {
       

        var oppName, oppQuantity, datepicker, oppSource, oppCampaign, oppDepart, oppNotes, oppStage, EventID, depositreqdate, OppShipping, txtshippingday, txtshippingprice, ddlDecline, txtrepeatfrom, txtlost, ddlOppAcctMgr, Cancelled, oppDepartId, ConfirmedDate;
        var OppData = new Object();
        oppName = $("#oppName").val();
        oppQuantity = $("#oppQuantity").val();
        datepicker = GetDate($("#datepicker").val());
        oppSource = $("#oppSource").val();
        oppCampaign = $("#oppCampaign").val();
        depositreqdate = GetDate($("#depositreqdate").val());
        OppShipping = $("#OppShipping").val();
        txtshippingday = $("#txtshippingday").val();
        txtshippingprice = $("#txtshippingprice").val();
        ddlDecline = $("#ddlDecline").val();
        txtrepeatfrom = $("#txtrepeatfrom").val();
        txtlost = $("#txtlost").val();
        Cancelled = $("#txtCancelled").val();
        ddlOppAcctMgr = $("#ddlOppAcctMgr").val();
        ConfirmedDate = GetDate($("#txtConfirmedDate").val());
        var DeliveryDate = $("#ddlDeliveryDate").val();

        //22 Aug 2018 (N)
        if ($('#PageName').val() == "Event") {
            EventID = $('#HiddenEventId').val();
        }
        //22 Aug 2018 (N)
        //$("#oppDepart").val($('#ms1').magicSuggest().getValue());
        //oppDepart = $("#oppDepart").val();

        //Code Change For Adding DeptId 10/07/2018
        //$("#oppDepart").val($('#ms1').magicSuggest().getValue());
        var getSelectedDepart = $('#ms1').magicSuggest().getSelection();
        var DepartName = [];
        var DepartId = [];
        if (getSelectedDepart.length > 0) {
            for (var i = 0; i < getSelectedDepart.length; i++) {
                DepartId.push(getSelectedDepart[i]["id"]);
                DepartName.push(getSelectedDepart[i]["name"]);
            }

            $("#oppDepart").val(DepartName);
            $("#oppDepartId").val(DepartId);
        }

        oppDepartId = $("#oppDepartId").val();
        oppDepart = $("#oppDepart").val();
        oppStage = $("#oppStage").val();
        oppNotes = $("#oppNotes").val();

        var OppDate, SetOppDate;
        if ($('#lblOpportunityId').text() == "000000") {
            OppDate = new Date();
            SetOppDate = ("0" + OppDate.getDate()).slice(-2) + '/' + ("0" + (OppDate.getMonth() + 1)).slice(-2) + '/' + OppDate.getFullYear() + ' ' + ("0" + OppDate.getHours()).slice(-2) + ':' + ("0" + OppDate.getMinutes()).slice(-2);
            OppDate = OppDate.getFullYear() + '-' + (OppDate.getMonth() + 1) + '-' + OppDate.getDate() + ' ' + OppDate.getHours() + ':' + OppDate.getMinutes();

        }
        else {
            OppDate = "";
        }
        var OppConfirmationId = $('#lblOpportunityId').text();
        var ConfirmedDate = GetDate($("#txtConfirmedDate").val());
        $.ajax({
            url: '/Opportunity/CheckOldConfirmationDateByOppId',
            data: { OppId: OppConfirmationId, ConfirmedDate: ConfirmedDate},
            async: false,
            success: function (response) {
                if (response == true) {
                    bootbox.confirm("You have changed the confirmation date. Are you sure you want to change the Confirmation Date?", function (result) {
                        if (result) {
                            openOrderEmailModal();
                            var Id = $('#lblOpportunityId').text();
                            $.ajax({
                                url: '/Opportunity/updateOpportunity',
                                data: { OpportunityId: $('#lblOpportunityId').text(), OppName: oppName, Quantity: oppQuantity, ReqDate: datepicker, Source: oppSource, DepartmentName: oppDepart, Notes: oppNotes, Compaign: oppCampaign, Stage: oppStage, OppDate: OppDate, DepositReqDate: depositreqdate, Shipping: OppShipping, ShippingTo: txtshippingday, Price: txtshippingprice, Declined: ddlDecline, Lost: txtlost, Cancelled: Cancelled, RepeatJobId: txtrepeatfrom, PageSource: $("#PageName").val(), AcctManagerId: ddlOppAcctMgr, job_department: oppDepartId, ConfirmedDate: ConfirmedDate, EventId: EventID, DeliveryDate: DeliveryDate },    // EventId (N) },
                                async: false,

                                success: function (response) {
                                    if ($('#lblOpportunityId').text() == "000000") {

                                        // baans change 12th Sept for Opp date

                                        //$('#lblOppDate').text(SetOppDate);
                                        //$('#lblOppDate').text(DateTimeFormat(response.OppDate));
                                        $.ajax({
                                            url: '/Opportunity/GetOppById',
                                            data: { OppId: response.ID },
                                            async: false,
                                            success: function (response) {
                                                $('#lblOppDate').text(DateTimeFormat(response.OppDate));
                                            },
                                            error: function (response) {
                                            },
                                            type: 'post'

                                        });
                                        // baans end 12th Sept
                                        setTimeout(function () {
                                            GetOptionGrid(response.ID);
                                        }, 200)


                                    }
                                    if (response.ID != 0 && response.ID != undefined) {
                                        $("#btnOppSubmit").removeClass("customAlertChange");
                                        if ($('#lblOpportunityId').text() == "000000") {
                                            history.pushState('', '', '/Opportunity/OpportunityDetails/' + response.ID);
                                            $("#HiddenFOrPrimary").val($("#HiddenContactId").val());

                                        }

                                        //setOptionTotal();
                                        getBalance(response.ID);
                                        $('#lblOpportunityId').text(('000000' + response.ID).substr(-6));
                                        SetNotes($('#oppStage').val());
                                        if ($('#PageName').val() == "Opportunity") {
                                            UpdateOppInquiry(response.ID);

                                        }


                                        $("#DetailTab").css("display", "block");
                                        $("#options").css("display", "block");
                                        // baans change 27th october for chosen dropdown
                                        //setTimeout(function () {
                                        //    $(".chosen").chosen();
                                        //}, 500);
                                        //$('#ddlItem').val('').trigger("chosen:updated");
                                        //$('#ddlItem').css("width", "182px");
                                        // baans end 27th october
                                    }
                                    SetOppoStage();
                                    //CustomAlert(response);
                                    //22 Aug 2018 (N)
                                    if ($('#PageName').val() == "Event") {
                                        //MakeOpportunityReadonly();
                                        GetOppoListByEvent('All', $('#HiddenEventId').val());
                                    }
                                    //22 Aug 2018 (N)
                                    // baans change 24th September
                                    if ($('#PageName').val() == "JobDetails") {
                                        $("#txtConfirmedDate").removeClass('customAlertChange');

                                    }
                                    // baans end 24th Sept

                                },
                                type: 'post',

                            });
                        }
                    });
                }
                else {
                    var Id = $('#lblOpportunityId').text();
                    $.ajax({
                        url: '/Opportunity/updateOpportunity',
                        data: { OpportunityId: $('#lblOpportunityId').text(), OppName: oppName, Quantity: oppQuantity, ReqDate: datepicker, Source: oppSource, DepartmentName: oppDepart, Notes: oppNotes, Compaign: oppCampaign, Stage: oppStage, OppDate: OppDate, DepositReqDate: depositreqdate, Shipping: OppShipping, ShippingTo: txtshippingday, Price: txtshippingprice, Declined: ddlDecline, Lost: txtlost, Cancelled: Cancelled, RepeatJobId: txtrepeatfrom, PageSource: $("#PageName").val(), AcctManagerId: ddlOppAcctMgr, job_department: oppDepartId, ConfirmedDate: ConfirmedDate, EventId: EventID, DeliveryDate: DeliveryDate },    // EventId (N) },
                        async: false,

                        success: function (response) {
                            if ($('#lblOpportunityId').text() == "000000") {

                                // baans change 12th Sept for Opp date

                                //$('#lblOppDate').text(SetOppDate);
                                //$('#lblOppDate').text(DateTimeFormat(response.OppDate));
                                $.ajax({
                                    url: '/Opportunity/GetOppById',
                                    data: { OppId: response.ID },
                                    async: false,
                                    success: function (response) {
                                        $('#lblOppDate').text(DateTimeFormat(response.OppDate));
                                    },
                                    error: function (response) {
                                    },
                                    type: 'post'

                                });
                                // baans end 12th Sept
                                setTimeout(function () {
                                    GetOptionGrid(response.ID);
                                }, 200)


                            }
                            if (response.ID != 0 && response.ID != undefined) {
                                $("#btnOppSubmit").removeClass("customAlertChange");
                                if ($('#lblOpportunityId').text() == "000000") {
                                    history.pushState('', '', '/Opportunity/OpportunityDetails/' + response.ID);
                                    $("#HiddenFOrPrimary").val($("#HiddenContactId").val());

                                }

                                //setOptionTotal();
                                getBalance(response.ID);
                                $('#lblOpportunityId').text(('000000' + response.ID).substr(-6));
                                SetNotes($('#oppStage').val());
                                if ($('#PageName').val() == "Opportunity") {
                                    UpdateOppInquiry(response.ID);

                                }


                                $("#DetailTab").css("display", "block");
                                $("#options").css("display", "block");
                                // baans change 27th october for chosen dropdown
                                //setTimeout(function () {
                                //    $(".chosen").chosen();
                                //}, 500);
                                //$('#ddlItem').val('').trigger("chosen:updated");
                                //$('#ddlItem').css("width", "182px");
                                // baans end 27th october
                            }
                            SetOppoStage();
                            CustomAlert(response);
                            //22 Aug 2018 (N)
                            if ($('#PageName').val() == "Event") {
                                //MakeOpportunityReadonly();
                                GetOppoListByEvent('All', $('#HiddenEventId').val());
                            }
                            //22 Aug 2018 (N)
                            // baans change 24th September
                            if ($('#PageName').val() == "JobDetails") {
                                $("#txtConfirmedDate").removeClass('customAlertChange');

                            }
                            // baans end 24th Sept

                        },
                        type: 'post',

                    });
                }
            }
        });

        
        // baans end 2nd November
    }
    else {
        //CustomWarning('Fill all required fields.');
        CustomErrorCode("Required");
    }
}

function submitSize() {

    var sizeFlag = false;
    var SizeString = "";
    // baans change 6th July for option total quantity of size
    var TotalOptSize = 0;
    $(".sizeQuantity").each(function () {
        if ($(this).val() != 0 && $(this).val() != null) {
            //if ($(this).val() != null) {
            SizeString += "" + $(this).attr('id') + "=" + $(this).val() + "  ";
            sizeFlag = true;
            TotalOptSize += parseInt($(this).val());

        }
        //else {
        //    SizeString += "" + $(this).attr('id') + "=" + 0 + " "; 
        //}
    });
    // baans change 12th Sept for optionPopUp Calculation
    var optsize = parseInt($('#txtOptionQty').val());
     // baans change 17th September for packing size
    //if (TotalOptSize > optsize) {

    if (TotalOptSize != optsize) {
       
        
            CustomWarning("Quantity Should be equal to Option Quantity");
     

    }
    // baans end 12th Sept
    else {
        if (SizeString != "" && SizeString != null && sizeFlag == true) {
            if ($("#hiddenforsizes").val() == "SizeOrdered") {
                $("#txtSizes").val(SizeString);
            }
            else {
                $("#txtSizesPacked").val(SizeString);
            }

            //$(".sizeQuantity").css("border-color", "rgba(204, 204, 204, 1)");
            $(".sizeQuantity").removeClass('customAlertChange');
            $("#SizeModel").css("display", "none");

        }
        else {
            //$(".sizeQuantity").css("border-color", "red");
            $(".sizeQuantity").addClass('customAlertChange');
        }
    }
}






//function submitSize() {

//    var sizeFlag = false;
//    var SizeString = "";
//    // baans change 6th July for option total quantity of size
//    var TotalOptSize = 0;
//    $(".sizeQuantity").each(function () {
//        if ($(this).val() != 0 && $(this).val() != null) {
//            SizeString += "" + $(this).attr('id') + "=" + $(this).val() + "  ";
//            sizeFlag = true;
//            TotalOptSize += parseInt($(this).val());

//        }
//        //else {
//        //    SizeString += "" + $(this).attr('id') + "=" + 0 + " "; 
//        //}
//    });
//    var optsize = parseInt($('#txtOptionQty').val());
//    if (TotalOptSize > optsize) {
//        CustomWarning("Quantity Could not be more than Option Quantity");
//    }
//    else {
//        if (SizeString != "" && SizeString != null && sizeFlag == true) {
//            $("#txtSizes").val(SizeString);
//            $(".sizeQuantity").css("border-color", "rgba(204, 204, 204, 1)");
//            $("#SizeModel").css("display", "none");
//        } else {
//            $(".sizeQuantity").css("border-color", "red");
//        }
//    }
//}
function ShowModal(SizesPacked, txtPackedandOrderId) {

    $('#hiddenforsizes').val(SizesPacked);
    var ddlSize = $("#ddlSizeType").val();
    if ($("#ddlSizeType").val() != "") {
        $(".sizeQuantity").each(function () {
            $(this).val('');
        });
       
        if ($("#" + txtPackedandOrderId).val() != null && $("#" + txtPackedandOrderId).val() != "") {
            var sizes = $("#" + txtPackedandOrderId).val();
            var arr = sizes.split(' ');
            $(".sizeQuantity").each(function () {
                $(this).val('');
                for (var arrSize = 0; arrSize < arr.length - 1; arrSize++) {
                    var splitarr = arr[arrSize].split('=');

                    if ($(this).attr('id') == splitarr[0]) {
                        $(this).val(splitarr[1]);
                    }
                }

            });


        }


        var optQuan = $("#txtOptionQty").val();
        $("#txttotalQuan").text(optQuan);
        if (optQuan != null && optQuan != undefined && optQuan != "" && optQuan != "0") {
            $("#txtSizes").removeClass('customAlertChange');
            $("#txtOptionQty").removeClass('customAlertChange');
            if ($("#ddlSizeType").val() != "Custom") {

                $("#SizeModel").css("display", "block");

            }
        }
        else {
            CustomWarning("Option Quantity should not be empty or 0 !!!");
            $("#txtSizes").addClass('customAlertChange');
            $("#txtOptionQty").addClass('customAlertChange');
            $("#" + txtPackedandOrderId).blur();
        }


        // baans end 6th July
        //$("#ddlSizeType").css("border-color", "rgba(204, 204, 204, 1)");
        $("#ddlSizeType").removeClass('customAlertChange');
    }
    else {
        //$("#ddlSizeType").css("border-color", "red");
        $("#ddlSizeType").addClass('customAlertChange');
    }
    // baans change 12th Sept for Calculate option popup
    var sizevalin = $('#txtSizes').val();
    if (sizevalin != null && sizevalin != undefined && sizevalin != "" && sizevalin != "0") {
        var total = 0;
        $(".sizeQuantity").each(function (i, obj) {
            if (obj.value != "" && obj.value != null && obj.value != undefined) {
                total += parseInt(obj.value);
            }
        });
        $('#txtTotalSize').val(total);
        var ExpectQuan = $('#txttotalQuan').text();
        if (total != parseInt(ExpectQuan)) {
            //$("#txtTotalSize").css("border-color", "red");
            $("#txtTotalSize").addClass('customAlertChange');
            
        }
        else {
            
            $("#txtTotalSize").removeClass('customAlertChange')
            $("#txtTotalSize").addClass('customAlertGreen');
        }
    }
        // baans end 12th Sept

}


//function ShowModal() {
//    if ($("#ddlSizeType").val() != "") {
//        if ($("#txtSizes").val() != null && $("#txtSizes").val() != "") {
//            var sizes = $("#txtSizes").val();
//            var arr = sizes.split(' ');
//            $(".sizeQuantity").each(function () {

//                for (var arrSize = 0; arrSize < arr.length - 1; arrSize++) {
//                    var splitarr = arr[arrSize].split('=');

//                    if ($(this).attr('id') == splitarr[0]) {
//                        $(this).val(splitarr[1]);
//                    }
//                }

//            });

//            //alert();
//            //GetSize("shirt");
//        }
//        // baans change 6th July
//        var optQuan = $("#txtOptionQty").val();
//        $("#txttotalQuan").text(optQuan);
//        if (optQuan != null && optQuan != undefined && optQuan != "" && optQuan != "0") {
//            if ($("#ddlSizeType").val() != "Custom") {
//                $("#SizeModel").css("display", "block");
//            }
//        }
//        else {
//            CustomWarning("Option Quantity should not be empty or 0 !!!");
//            $("#txtSizes").blur();
//        }
//        // baans end 6th July
//        $("#ddlSizeType").css("border-color", "rgba(204, 204, 204, 1)");
//    }
//    else {
//        $("#ddlSizeType").css("border-color", "red");
//    }

//}
// baans change 6th July for change in quantity
function calculatequantity() {
    var total = 0;
    var ExpectQuan = $('#txttotalQuan').text();
    $(".sizeQuantity").each(function (i, obj) {
        if (obj.value != "" && obj.value != null && obj.value != undefined) {
            total += parseInt(obj.value);
        }
    });
        // baans change 12th Sept for optionPopUp Calculation
    $('#txtTotalSize').val(total);
    if (total != parseInt(ExpectQuan)) {
        //$("#txtTotalSize").css("border-color", "red");
        $("#txtTotalSize").removeClass('customAlertGreen');
        $("#txtTotalSize").addClass('customAlertChange');

    }
    else {
        //$("#txtTotalSize").css("border-color", "rgba(204, 204, 204, 1)");
        //$("#txtTotalSize").css("border-color", "green");
        $("#txtTotalSize").removeClass('customAlertChange');
        $("#txtTotalSize").addClass('customAlertGreen');
    }
    // baans end 12th Sept

}
// baans end 6th July

function HideModal() {
    $("#SizeModel").css("display", "none");
    if ($('#HiddenOptionID').val() == "") {
        $('#txtTotalSize').val('');
        $("#txtTotalSize").removeClass('customAlertGreen');
        $("#txtTotalSize").removeClass('customAlertChange');

    }

}
$(function () {
    $('#oppStage').change(function () {
        var Stage = $('#oppStage').val();
        if (Stage == "Opportunity") {
            $('#oppNotes').val($('#HiddenOppNotes').val());
        }
        if (Stage == "Quote") {
            $('#oppNotes').val($('#HiddenQuoteNotes').val());
        }
        if (Stage == "Order") {
            $('#oppNotes').val($('#HiddenOrderNotes').val());
        }
        if (Stage == "Job") {
            $('#oppNotes').val($('#HiddenJobNotes').val());
        }
        if (Stage == "Packing") {
            $('#oppNotes').val($('#HiddenPackingNotes').val());
        }
        if (Stage == "Invoicing") {
            $('#oppNotes').val($('#HiddenInvoiceingNotes').val());
        }
        if (Stage == "Shipping") {
            $('#oppNotes').val($('#HiddenShippingNotes').val());
        }
        if (Stage == "Complete") {
            $('#oppNotes').val($('#HiddenCompleteNotes').val());
        }
    });
    $("#DecGridSearch").on("click", function () {
        if ($("#txtSearchImg").val() != "" && $("#txtSearchImg").val() != null) {
            $("#txtSearchImg").removeClass('customAlertChange');
            GetDecorationImageGrid($("#txtSearchImg").val());
        } else {
            $("#txtSearchImg").addClass('customAlertChange');
        }
    });
});

function NewOpp() {
    location.href = "/Opportunity/OpportunityDetails/0";
}
function editRow() {
    var rowIndx = getRowIndx(this);

    if (rowIndx != null) {
        var data = this.pdata[rowIndx];
        location.href = "/Opportunity/OpportunityDetails/" + data.OpportunityId;
    }
}

function getRowIndx(grid) {
    var arr1 = grid.getChanges();
    var arr = grid.selection({ type: 'row', method: 'getSelection' });
    if (arr && arr.length > 0) {
        //alert(arr[0].rowIndx);
        return arr[0].rowIndx;
    }
    else {
        $('#CustomAlert').removeClass('j-alertBox-Failed').removeClass('j-alertBox-Success').css('display', 'none');
        $('#CustomAlert').addClass('j-alertBox-Warning').css('display', 'block');
        $('#alertMsg').html('<strong>Warning !</strong> Select a row.');
        setTimeout(function () {
            $('#CustomAlert').fadeOut('slow', function () {
                $('#CustomAlert').removeClass('j-alertBox-Warning').css('display', 'none');
                $('#alertMsg').html('');

            });
        }, 5000);
        return null;
    }
}
function OptionCopy() {
   
    if ($('#HiddenOptionID').val() != "") {
            // BAANS CHANGE 11th January
            // baans change 29th September for valid option fields
            var OptionIsValid = true;
            //var filter = /^[^-\s][a-zA-Z0-9_\s-]+$/;
            if ($("#ddlSizeType").val() == "Custom" && $("#ddlSizeType").val() != "") {
                //if (filter.test($('#txtSizes').val())) {
                //    $("#txtSizes").removeClass("customAlertChange");
                //}
                if ($('#txtSizes').val() != "") {
                    $("#txtSizes").removeClass("customAlertChange");
                }
                else {
                    OptionIsValid = false;
                    $("#txtSizes").addClass("customAlertChange");
                }
                if ($('#PageName').val() == "PackingDetails" || $('#PageName').val() == "InvoicingDetails" || $('#PageName').val() == "ShippingDetails" || $('#PageName').val() == "CompleteDetails") {
                    //if (filter.test($('#txtSizesPacked').val())) {
                    //    $("#txtSizesPacked").removeClass("customAlertChange");
                    //}
                    if ($('#txtSizesPacked').val() != "") {
                        $("#txtSizesPacked").removeClass("customAlertChange");
                    }
                    // baans end 11th January
                    else {
                        OptionIsValid = false;
                        $("#txtSizesPacked").addClass("customAlertChange");
                    }
                }
            }
            if (OptionIsValid) {
                $('#hdnIsCopyPressId').val("True");
                UpdateOption(0);
            }
            else {
                //CustomWarning('Fill all required fields.');
                CustomErrorCode("Required");
            }
        }
        else {
            $("#txtOptionQty").removeClass('customAlertChange');
            $("#ddlBrand_chosen .chosen-single").removeClass('customAlertChange');
            $("#ddlItem_chosen .chosen-single").removeClass('customAlertChange');
            //$("#ddlItem").removeClass('customAlertChange');
            //$("#ddlBrand").removeClass('customAlertChange');
            $("#txtCode").removeClass('customAlertChange');
            $("#ddlSizeType").removeClass('customAlertChange');
            $("#txtSizes").removeClass('customAlertChange');
            CustomWarning('Select Option First');

        }
    

}

function OptionSave() {
    debugger;
    var OptionAvail = $('#HiddenOptionID').val();
    UpdateOption($('#HiddenOptionID').val());
    if ($('#hdnBrandStatus').val() == "InActive") {
        var Brandstatus = $('#hdnTypevalue').val();
        var BrandId = $('#hdnBrandId').val();
        $("#ddlBrand option[value='" + BrandId + "']").each(function () {
            $(this).remove();
        });
    }
    var Status = $('#hdnTypeStatus').val();
    if ($('#hdnTypeStatus').val() == "InActive") {
        var Status = $('#hdnTypeStatus').val();
        var Itemstatus = $('#hdnTypevalue').val();
        var ItemId = $('#hdnTypeId').val();
        $("#ddlItem option[value='" + ItemId + "']").each(function () {
            $(this).remove();
        });
    }
   

}
function SaveOppoPacking() {
    var Result;
    $.ajax({
        url: '/Opportunity/UpdateOppPackin',
        data: { OpportunityId: $('#lblOpportunityId').text(), PackedInSet1: $('#ddlPackedInSet1').val(), PackedInSet2: $('#ddlPackedInSet2').val(), ConsigNoteNo: $('#txtConsigNoteNo').val(), PacagingNotes: $('#txtPackingNotes').val() },
        async: false,
        success: function (response) {
            Result = response;
            CustomAlert(response);
            // baans change 27th September for Moving to Complete Screen
            if ($("#PageName").val() == "ShippingDetails") {
                $('#txtConsigNoteNo').removeClass('customAlertChange');
            }
                            // baans end 27th September
        },
        type: 'post',

    });
    return Result;
}
function UpdateOption(OptionID) {
    debugger;
    var IsValid = true;
    
    // baans change 29th September for valid option fields
    var OptionIsValid = true;
    var filter = /^[^-\s][a-zA-Z0-9_\s-]+$/;
    if ($('#PageName').val() != "ShippingDetails" && $('#PageName').val() != "InvoicingDetails" && $('#PageName').val() != "CompleteDetails" && $('#PageName').val() != "PackingDetails") {
    //if (filter.test($('#txtCode').val())) {
    //    $("#txtCode").removeClass("customAlertChange");
    //}
    //else {
    //    OptionIsValid = false;
    //    $("#txtCode").addClass("customAlertChange");
    //}
    }
    // baans change 5th October for valid fields in option
    if ($('#PageName').val() != "ShippingDetails" && $('#PageName').val() != "InvoicingDetails" && $('#PageName').val() != "CompleteDetails" && $('#PageName').val() != "PackingDetails") {
        if ($("#txtOptionQty").val() != "" && $("#txtOptionQty").val() != "0" && parseInt($("#txtOptionQty").val()) > 0) {
            $("#txtOptionQty").removeClass('customAlertChange');
        } else {
            $("#txtOptionQty").addClass('customAlertChange');
        }

        if ($("#ddlBrand").val() != "") {
            //$("#ddlBrand").removeClass('customAlertChange');
            $("#ddlBrand_chosen .chosen-single").removeClass('customAlertChange');
        } else {
            //$("#ddlBrand").addClass('customAlertChange');
            $("#ddlBrand_chosen .chosen-single").addClass('customAlertChange');
        }

        if ($("#ddlItem").val() != "") {
            //$("#ddlItem").removeClass('customAlertChange');
            $("#ddlItem_chosen .chosen-single").removeClass('customAlertChange');
        } else {
            //$("#ddlItem").addClass('customAlertChange');
            $("#ddlItem_chosen .chosen-single").addClass('customAlertChange');
        }

        if ($("#ddlSizeType").val() != "") {
            $("#ddlSizeType").removeClass('customAlertChange');
        } else {
            $("#ddlSizeType").addClass('customAlertChange');
        }

        if ($("#txtSizes").val() != "") {
            $("#txtSizes").removeClass('customAlertChange');
        } else {
            $("#txtSizes").addClass('customAlertChange');
        }
    }
    // baans end 5th october

    // baans change 6th October for option valid fields
    if ($('#PageName').val() == "ShippingDetails" || $('#PageName').val() == "InvoicingDetails" || $('#PageName').val() == "CompleteDetails" || $('#PageName').val() == "PackingDetails") {
        if ($('#HiddenOptionID').val() != "") {
            
            if ($("#txtOptionQty").val() != "" && $("#txtOptionQty").val() != "0" && parseInt($("#txtOptionQty").val()) > 0) {
                $("#txtOptionQty").removeClass('customAlertChange');
            } else {
                $("#txtOptionQty").addClass('customAlertChange');
            }

            //var filter = /^[^-\s][<>'"/`%][a-zA-Z0-9_\s-]+$/;          
            //if (filter.test($('#txtCode').val())) {
            //    $("#txtCode").removeClass("customAlertChange");
            //}
            //else {
            //    OptionIsValid = false;
            //    $("#txtCode").addClass("customAlertChange");
            //}

            if ($("#ddlBrand").val() != "") {
                //$("#ddlBrand").removeClass('customAlertChange');
                $("#ddlBrand_chosen .chosen-single").removeClass('customAlertChange');
            } else {
                //$("#ddlBrand").addClass('customAlertChange');
                $("#ddlBrand_chosen .chosen-single").addClass('customAlertChange');
            }

            if ($("#ddlItem").val() != "") {
                //$("#ddlItem").removeClass('customAlertChange');
                $("#ddlItem_chosen .chosen-single").removeClass('customAlertChange');
            } else {
                //$("#ddlItem").addClass('customAlertChange');
                $("#ddlItem_chosen .chosen-single").addClass('customAlertChange');
            }

            if ($("#ddlSizeType").val() != "") {
                $("#ddlSizeType").removeClass('customAlertChange');
            } else {
                $("#ddlSizeType").addClass('customAlertChange');
            }

            if ($("#txtSizes").val() != "") {
                $("#txtSizes").removeClass('customAlertChange');
            } else {
                $("#txtSizes").addClass('customAlertChange');
            }
        }
    }
    // baans end 6th October

    var data = $("#ddlSizeType").val();
    if ($("#ddlSizeType").val() == "Custom" && $("#ddlSizeType").val() != ""){
        if ($('#txtSizes').val() != "") {
            $("#txtSizes").removeClass("customAlertChange");
            }
            else
            {
                OptionIsValid = false;
                $("#txtSizes").addClass("customAlertChange");
        }
        if ($('#PageName').val() == "PackingDetails" || $('#PageName').val() == "InvoicingDetails" || $('#PageName').val() == "ShippingDetails" || $('#PageName').val() == "CompleteDetails") {
            if (filter.test($('#txtSizesPacked').val())) {
                $("#txtSizesPacked").removeClass("customAlertChange");
            }
            else {
                OptionIsValid = false;
                $("#txtSizesPacked").addClass("customAlertChange");
            }
        }
        }
    if (OptionIsValid) {
        // baans change 18th Sept for Option save on packing
        if ($('#PageName').val() == "PackingDetails" && $("#ddlSizeType").val() != "Custom" && $("#ddlSizeType").val() != "") {
            var OptionSizes = $('#txtSizesPacked').val();
            var OptionQuan = parseInt($('#txtOptionQty').val());
            var total = 0;

            var OptEnteredQuan;
            var ExactQuan = "";
            var bigOption = OptionSizes.split(' ');
            for (var h = 0; h < bigOption.length; h++) {
                if (bigOption[h] != "") {
                    OptEnteredQuan += "," + bigOption[h].split('=');
                }
            }
            if (OptEnteredQuan != "" && OptEnteredQuan != 0 && OptEnteredQuan != "0" && OptEnteredQuan != undefined) {
                ExactQuan = OptEnteredQuan.split(',');
            }
            if (ExactQuan.length != 0, ExactQuan.length != undefined, ExactQuan.length != "") {
                for (var t = 2; t < ExactQuan.length; t++) {
                    //if (Number.isInteger(ExactQuan[t])) {
                    total += parseInt(ExactQuan[t]);
                    t = t + 1;
                    //}
                }
            }
            if (total != OptionQuan && OptionQuan != NaN && $("#ddlSizeType").val() != "Custom") {
                //$("#txtSizes").css("border-color", "red");
                $("#txtSizes").addClass('customAlertChange');
                //$("#txtOptionQty").css("border-color", "red");
                $("#txtOptionQty").addClass('customAlertChange');
                //$("#txtSizesPacked").css("border-color", "red");
                $("#txtSizesPacked").addClass('customAlertChange');
                CustomWarning("Before Moving to Packed Screen Sizes should be equal to Option Quantity !!!");
                IsValid = false;
            }
        }
        else {
            if ($("#ddlSizeType").val() != "Custom" && $("#ddlSizeType").val() != "") {
                // baans end 18th September

                // baans change 10th Sept for option Calculation
                var OptionSizes = $('#txtSizes').val();
                var OptionQuan = parseInt($('#txtOptionQty').val());
                var total = 0;

                var OptEnteredQuan;
                var ExactQuan = "";
                var bigOption = OptionSizes.split(' ');
                for (var h = 0; h < bigOption.length; h++) {
                    if (bigOption[h] != "") {
                        OptEnteredQuan += "," + bigOption[h].split('=');
                    }
                }
                if (OptEnteredQuan != "" && OptEnteredQuan != 0 && OptEnteredQuan != "0" && OptEnteredQuan != undefined) {
                    ExactQuan = OptEnteredQuan.split(',');
                }
                if (ExactQuan.length != 0, ExactQuan.length != undefined, ExactQuan.length != "") {
                    for (var t = 2; t < ExactQuan.length; t++) {
                        //if (Number.isInteger(ExactQuan[t])) {
                        total += parseInt(ExactQuan[t]);
                        t = t + 1;
                        //}
                    }
                }
                if (total != OptionQuan && $("#ddlSizeType").val() != "Custom") {
                    //$("#txtSizes").css("border-color", "red");
                    $("#txtSizes").addClass('customAlertChange');
                    //$("#txtOptionQty").css("border-color", "red");
                    $("#txtOptionQty").addClass('customAlertChange');
                    CustomError("Sizes should be equal to Option Quantity !!!");
                    IsValid = false;
                }
            }
            // baans change 11th January for checking txtSizes value
            if ($("#ddlSizeType").val() == "Custom" && $("#txtSizes").val() == "TBC") {
                var defaultOptionQuan = parseInt($('#txtOptionQty').val());
                $("#txtSizes").val("TBC=" + defaultOptionQuan);

            }
                //baans end 11th Jan
        }
        if (IsValid) {
            var Result;
            var IsInsert = true;
            var checkflag = true;
            if ($('#PageName').val() == "ShippingDetails" || $('#PageName').val() == "InvoicingDetails" || $('#PageName').val() == "CompleteDetails" || $('#PageName').val() == "PackingDetails") {

                IsInsert = false;
                if ($("#ddlPackedInSet1").val() != "") {
                    //$("#ddlPackedInSet1").css("border-color", "rgba(204, 204, 204, 1)"); 
                    $("#ddlPackedInSet1").removeClass('customAlertChange');
                } else {
                    //$("#ddlPackedInSet1").css("border-color", "red");
                    $("#ddlPackedInSet1").addClass('customAlertChange');
                    checkflag = false;
                }
                if ($('#ddlPackedInSet1').val() != "") {
                    Result = SaveOppoPacking();
                }
                else {
                    //CustomWarning('Fill all required fields.');
                    CustomErrorCode("Required");
                }
            }


            var txtOptionQty, txtCode, txtColor, txtLink, txtCost, txtMargin, ddlBrand, ddlItem, txtotherDesc, txtunitprcExgst, ddlinclude, txtothercost, txtComment, ddlservice, ddlSizeType, txtSizes, txtSizesPacked, ddlDecline, OptionStage, ProofSent;
            if (OptionID != "" || ($('#PageName').val() != "ShippingDetails" && $('#PageName').val() != "InvoicingDetails" && $('#PageName').val() != "CompleteDetails" && $('#PageName').val() != "PackingDetails")) {
                if ($("#txtCode").val() != "") {
                    txtCode = $("#txtCode").val();
                    //$("#txtCode").css("border-color", "rgba(204, 204, 204, 1)");
                    $("#txtCode").removeClass('customAlertChange');
                } else {
                    //$("#txtCode").css("border-color", "red");
                    $("#txtCode").addClass('customAlertChange');

                    checkflag = false;
                }

                if ($("#txtOptionQty").val() != "" && $("#txtOptionQty").val() != "0" && parseInt($("#txtOptionQty").val()) > 0) {
                    txtOptionQty = $("#txtOptionQty").val();
                    //$("#txtOptionQty").css("border-color", "rgba(204, 204, 204, 1)");
                    $("#txtOptionQty").removeClass('customAlertChange');
                } else {
                    //$("#txtOptionQty").css("border-color", "red");
                    $("#txtOptionQty").addClass('customAlertChange');

                    checkflag = false;
                }
                if ($("#ddlBrand").val() != "") {
                    ddlBrand = $("#ddlBrand").val();
                    //$("#ddlBrand").css("border-color", "rgba(204, 204, 204, 1)");
                    //$("#ddlBrand").removeClass('customAlertChange');
                    $("#ddlBrand_chosen .chosen-single").removeClass('customAlertChange');
                } else {
                    //$("#ddlBrand").css("border-color", "red");
                    //$("#ddlBrand").addClass('customAlertChange');
                    $("#ddlBrand_chosen .chosen-single").addClass('customAlertChange');

                    checkflag = false;
                }
                if ($("#ddlItem").val() != "") {
                    ddlItem = $("#ddlItem").val();
                    //$("#ddlItem").css("border-color", "rgba(204, 204, 204, 1)");
                    //$("#ddlItem").removeClass('customAlertChange');
                    $("#ddlItem_chosen .chosen-single").removeClass('customAlertChange');
                } else {
                    //$("#ddlItem").css("border-color", "red");
                    //$("#ddlItem").addClass('customAlertChange');
                    $("#ddlItem_chosen .chosen-single").addClass('customAlertChange');

                    checkflag = false;
                }
                if ($("#ddlSizeType").val() != "") {
                    ddlSizeType = $("#ddlSizeType").val();
                    //$("#ddlSizeType").css("border-color", "rgba(204, 204, 204, 1)");
                    $("#ddlSizeType").removeClass('customAlertChange');
                } else {
                    //$("#ddlSizeType").css("border-color", "red");
                    $("#ddlSizeType").addClass('customAlertChange');

                    checkflag = false;
                }
                if ($("#txtSizes").val() != "") {
                    txtSizes = $("#txtSizes").val();
                    //$("#txtSizes").css("border-color", "rgba(204, 204, 204, 1)");
                    $("#txtSizes").removeClass('customAlertChange');
                } else {
                    /*$("#txtSizes").css("border-color", "red");*/
                    $("#txtSizes").addClass('customAlertChange');
                    checkflag = false;
                }
                if ($('#PageName').val() == "PackingDetails" || $('#PageName').val() == "InvoicingDetails") {
                    if ($("#txtSizesPacked").val() != "") {
                        txtSizesPacked = $("#txtSizesPacked").val();
                        //$("#txtSizesPacked").css("border-color", "rgba(204, 204, 204, 1)");
                        $("#txtSizesPacked").removeClass('customAlertChange');
                    } else {
                        //$("#txtSizesPacked").css("border-color", "red");
                        $("#txtSizesPacked").addClass('customAlertChange');
                        checkflag = false;
                    }
                }

            }

            if ($('#PageName').val() == "Opportunity" || $('#PageName').val() == "QuoteDetails")
                OptionStage = "Opp";
            else
                OptionStage = "Order";

            txtColor = $("#txtColor").val();
            txtLink = $("#txtLink").val();
            txtCost = $("#txtCost").val();
            txtMargin = $("#txtMargin").val();
            ddlSizeType = $("#ddlSizeType").val();
            txtSizes = $("#txtSizes").val();
            txtSizesPacked = $("#txtSizesPacked").val();
            ddlDecline = $("#ddlDecline").val();
            txtotherDesc = $("#txtotherdesc").val();
            txtothercost = $("#txtothercost").val();
            txtComment = $("#txtComment").val();
            ddlservice = $("#ddlservice").val();
            if ($("#ddlinclude").prop('checked')) {
                ddlinclude = "Yes";
            } else {
                ddlinclude = "No";
            }

            //17 Oct 2019 (N) ProofSent
            ProofSent = $('#chkProofSelect').prop('checked');
            //17 Oct 2019 (N) ProofSent

            // baans change 20th November
            CalculateUnitPriceAtOptionSavingStage('');
                    // baans end 20th November
            txtunitprcExgst = $("#txtunitprcExgst").val();
            if (IsInsert || OptionID != "") {
                if (checkflag == true) {
                    // baans change 31st October for decoration cost and its quantity
                    var DecorationValid = true;
                    if (($('#txtDecorationFront').val() == "" && $('#txtRangeFront').val() != "") || ($('#txtRangeFront').val() == "" && $('#txtDecorationFront').val() != "")) {
                        DecorationValid = false;
                        $('#txtDecorationFront').addClass('customAlertChange');
                        $('#txtRangeFront').addClass('customAlertChange');
                    }
                    else {
                        $('#txtDecorationFront').removeClass('customAlertChange');
                        $('#txtRangeFront').removeClass('customAlertChange');
                    }
                    var FrontDecCost;
                    if ($('#txtRangeFront').val() == "" || $('#txtRangeFront').val() == undefined) {
                        FrontDecCost = " ";
                    }
                    else {
                        FrontDecCost = $('#hiddenRangeFrontCost').val();
                    }

                    // Back
                    if (($('#txtDecorationBack').val() == "" && $('#txtRangeBack').val() != "") || ($('#txtRangeBack').val() == "" && $('#txtDecorationBack').val() != "")) {
                        DecorationValid = false;
                        $('#txtDecorationBack').addClass('customAlertChange');
                        $('#txtRangeBack').addClass('customAlertChange');
                    }
                    else {
                        $('#txtDecorationBack').removeClass('customAlertChange');
                        $('#txtRangeBack').removeClass('customAlertChange');
                    }
                    var BackDecCost;
                    if ($('#txtRangeBack').val() == "") {
                        BackDecCost = " ";
                    }
                    else {
                        BackDecCost = $('#hiddenRangeBackCost').val();
                    }

                    // Left
                    if (($('#txtDecorationLeft').val() == "" && $('#txtRangeLeft').val() != "") || ($('#txtRangeLeft').val() == "" && $('#txtDecorationLeft').val() != "")) {
                        DecorationValid = false;
                        $('#txtDecorationLeft').addClass('customAlertChange');
                        $('#txtRangeLeft').addClass('customAlertChange');
                    }
                    else {
                        $('#txtDecorationLeft').removeClass('customAlertChange');
                        $('#txtRangeLeft').removeClass('customAlertChange');
                    }
                    var LeftDecCost;
                    if ($('#txtRangeLeft').val() == "") {
                        LeftDecCost = " ";
                    }
                    else {
                        LeftDecCost = $('#hiddenRangeLeftCost').val();
                    }

                    // Right
                    if (($('#txtDecorationRight').val() == "" && $('#txtRangeRight').val() != "") || ($('#txtRangeRight').val() == "" && $('#txtDecorationRight').val() != "")) {
                        DecorationValid = false;
                        $('#txtDecorationRight').addClass('customAlertChange');
                        $('#txtRangeRight').addClass('customAlertChange');
                    }
                    else {
                        $('#txtDecorationRight').removeClass('customAlertChange');
                        $('#txtRangeRight').removeClass('customAlertChange');
                    }
                    var RightDecCost;
                    if ($('#txtRangeRight').val() == "") {
                        RightDecCost = " ";
                    }
                    else {
                        RightDecCost = $('#hiddenRangeRightCost').val();
                    }

                    //Other
                    if (($('#txtDecorationOther').val() == "" && $('#txtRangeOther').val() != "") || ($('#txtRangeOther').val() == "" && $('#txtDecorationOther').val() != "")) {
                        DecorationValid = false;
                        $('#txtDecorationOther').addClass('customAlertChange');
                        $('#txtRangeOther').addClass('customAlertChange');
                    }
                    else {
                        $('#txtDecorationOther').removeClass('customAlertChange');
                        $('#txtRangeOther').removeClass('customAlertChange');
                    }
                    var OtherDecCost;
                    if ($('#txtRangeOther').val() == "") {
                        OtherDecCost = " ";
                    }
                    else {
                        OtherDecCost = $('#hiddenRangeOtherCost').val();
                    }
                   
                    if (DecorationValid) {
                        var model = {
                            "model": { id: OptionID, quantity: txtOptionQty, code: txtCode, band_id: ddlBrand, item_id: ddlItem, colour: txtColor, comment: txtComment, SizeGrid: ddlSizeType, Link: txtLink, Cost: txtCost, Margin: txtMargin, InitialSizes: txtSizes, SizesPacked: txtSizesPacked, OtherDesc: txtotherDesc, OtherCost: txtothercost, Declined: ddlDecline, include: ddlinclude, Service: ddlservice, Front_decDesign: $('#txtDecorationFront').val(), Back_decDesign: $('#txtDecorationBack').val(), Left_decDesign: $('#txtDecorationLeft').val(), Right_decDesign: $('#txtDecorationRight').val(), Extra_decDesign: $('#txtDecorationOther').val(), Front_decQuantity: $('#txtRangeFront').val(), Back_decQuantity: $('#txtRangeBack').val(), Left_decQuantity: $('#txtRangeLeft').val(), Right_decQuantity: $('#txtRangeRight').val(), Extra_decQuantity: $('#txtRangeOther').val(), Front_decCost: FrontDecCost, Back_decCost: BackDecCost, Left_decCost: LeftDecCost, Right_decCost: RightDecCost, Extra_decCost: OtherDecCost, OpportunityId: $('#lblOpportunityId').text(), uni_price: txtunitprcExgst, front_decoration: $('#FrontDecorationID').val(), back_decoration: $('#BackDecorationID').val(), left_decoration: $('#LeftDecorationID').val(), right_decoration: $('#RightDecorationID').val(), extra_decoration: $('#OtherDecorationID').val(), OptionStage: OptionStage, ProofSent: ProofSent },

                            //P 10 Jan OptionCode
                            "OptionCodeModel": { id: $("#HiddenOptionCodeID").val(), Code: txtCode, itemId: ddlItem, BrandId: ddlBrand, Link: txtLink, cost: parseFloat(txtCost) }
                            //P 10 Jan OptionCode
                        }
                        $.ajax({
                            url: '/Opportunity/UpdateOption',
                            data: model,
                            async: false,
                            success: function (response) {
                                // baans change 17th September for disable save on packing and invoice
                                if ($("#PageName").val() == "InvoicingDetails" || $("#PageName").val() == "PackingDetails") {
                                    //$('#btnOptionSave').attr('disabled', true).addClass('MakeReadonly');
                                    $('#txtSizesPacked').val('');

                                }
                                // baans end 17th Sept
                                // baans change 27th September for Moving to Complete Screen
                                if ($("#PageName").val() == "ShippingDetails") {
                                    $('#txtConsigNoteNo').removeClass('customAlertChange');
                                }
                                // baans end 27th September
                                if (response.ID != null && response.ID != undefined) {


                                    $('#HiddenOptionID').val(response.ID);
                                    GetOptionGrid($('#lblOpportunityId').text());
                                }
                                if (response.Result == "Success") {
                                    // baans change 26th November for OptionCopy
                                    if ($('#hdnIsCopyPressId').val() == "True") {
                                        var Id = response.ID;
                                        $.ajax({
                                            url: '/Opportunity/getOptionDataByOptId',
                                            data: { OptId: Id },
                                            async: false,
                                            success: function (response) {
                                                data = response;
                                                if (data != undefined && data != null) {
                                                    $('#txtOptionQty').val(data.quantity);
                                                    $('#txtCode').val(data.code);
                                                    $('#txtColor').val(data.colour);
                                                    $('#txtLink').val(data.Link);
                                                    $('#txtCost').val(data.Cost);
                                                    $('#txtMargin').val(data.Margin);
                                                    // baans change 15th Sept
                                                    //$('#ddlBrand').val(data.band_id);
                                                    if (data.Status == "InActive") {
                                                        $('#hdnBrandStatus').val(data.Status);
                                                        $('#hdnBrandvalue').val(data.BrandName);
                                                        $('#hdnBrandId').val(data.band_id);
                                                        $("#ddlBrand").append($('<option></option>').attr("value", data.band_id).text(data.BrandName)).trigger("chosen:updated");
                                                        $('#ddlBrand').val(data.band_id).trigger("chosen:updated");
                                                    }
                                                    else {
                                                        $('#ddlBrand').val(data.band_id).trigger("chosen:updated");
                                                    }
                                                    var Id = data.item_id;
                                                    $.ajax({
                                                        url: '/Opportunity/GetItemStatus',
                                                        data: { ItemId: Id },
                                                        async: false,
                                                        success: function (response) {
                                                            if (response == "InActive") {
                                                                $('#hdnTypeStatus').val('InActive');
                                                                $('#hdnTypevalue').val(data.ItemName);
                                                                $('#hdnTypeId').val(data.item_id);
                                                                $("#ddlItem").append($('<option></option>').attr("value", data.item_id).text(data.ItemName)).trigger("chosen:updated");
                                                                $('#ddlItem').val(data.item_id).trigger("chosen:updated");
                                                            }
                                                            else {
                                                                $('#ddlItem').val(data.item_id).trigger("chosen:updated");
                                                            }
                                                        },
                                                        error: function (response) {
                                                            data = response;
                                                        },
                                                        type: 'get',
                                                    });
                                                    $('#txtotherdesc').val(data.OtherDesc);
                                                    $('#txtunitprcExgst').val(data.uni_price);
                                                    if (data.include_job == true) {
                                                        $('#ddlinclude').prop('checked', true);
                                                    } else {
                                                        $('#ddlinclude').prop('checked', false);
                                                    }

                                                    $('#txtothercost').val(data.OtherCost);
                                                    $('#txtComment').val(data.comment);
                                                    $('#ddlservice').val(data.Service);
                                                    $('#ddlSizeType').val(data.SizeGrid);
                                                    $('#txtSizes').val(data.InitialSizes);
                                                    $('#txtSizesPacked').val(data.SizesPacked);
                                                    //  $('#ddlDecline').val(data.Declined);
                                                    $('#HiddenOptionID').val(parseInt(data.id));
                                                    $('#txtDecorationFront').val(data.Front_decDesign);
                                                    $('#txtRangeFront').val(data.Front_decQuantity);
                                                    $('#hiddenRangeFrontCost').val(data.Front_decCost);
                                                    $('#txtDecorationBack').val(data.Back_decDesign);
                                                    $('#txtRangeBack').val(data.Back_decQuantity);
                                                    $('#hiddenRangeBackCost').val(data.Back_decCost);
                                                    $('#txtDecorationRight').val(data.Right_decDesign);
                                                    $('#txtRangeRight').val(data.Right_decQuantity);
                                                    $('#hiddenRangeRightCost').val(data.Right_decCost);
                                                    $('#txtDecorationLeft').val(data.Left_decDesign);
                                                    $('#txtRangeLeft').val(data.Left_decQuantity);
                                                    $('#hiddenRangeLeftCost').val(data.Left_decCost);
                                                    $('#txtDecorationOther').val(data.Extra_decDesign);
                                                    $('#txtRangeOther').val(data.Extra_decQuantity);
                                                    $('#hiddenRangeOtherCost').val(data.Extra_decCost);
                                                    if (data.front_decoration != null && data.front_decoration != undefined) {
                                                        $("#FrontShortlbl").text(data.Front_decDesignName.substr(0, 6));
                                                        $("#FrontLonglbl").text(data.Front_decDesignName);
                                                        $("#FrontDecorationID").val(data.front_decoration);
                                                    }
                                                    else {
                                                        $("#FrontShortlbl").text("...");
                                                        $("#FrontLonglbl").text("Click On Search");
                                                        $("#FrontDecorationID").val("");
                                                    }

                                                    if (data.back_decoration != null && data.back_decoration != undefined) {
                                                        $("#BackShortlbl").text(data.Back_decDesignName.substr(0, 6));
                                                        $("#BackLonglbl").text(data.Back_decDesignName);
                                                        $("#BackDecorationID").val(data.back_decoration);
                                                    }
                                                    else {
                                                        $("#BackShortlbl").text("...");
                                                        $("#BackLonglbl").text("Click On Search");
                                                        $("#BackDecorationID").val("");
                                                    }
                                                    if (data.left_decoration != null && data.left_decoration != undefined) {
                                                        $("#LeftShortlbl").text(data.Left_decDesignName.substr(0, 6));
                                                        $("#LeftLonglbl").text(data.Left_decDesignName);
                                                        $("#LeftDecorationID").val(data.left_decoration);
                                                    }
                                                    else {
                                                        $("#LeftShortlbl").text("...");
                                                        $("#LeftLonglbl").text("Click On Search");
                                                        $("#LeftDecorationID").val("");
                                                    }
                                                    if (data.right_decoration != null && data.right_decoration != undefined) {
                                                        $("#RightShortlbl").text(data.Right_decDesignName.substr(0, 6));
                                                        $("#RightLonglbl").text(data.Right_decDesignName);
                                                        $("#RightDecorationID").val(data.right_decoration);
                                                    }
                                                    else {

                                                        $("#RightShortlbl").text("...");
                                                        $("#RightLonglbl").text("Click On Search");
                                                        $("#RightDecorationID").val("");
                                                    }
                                                    if (data.extra_decoration != null && data.extra_decoration != undefined) {
                                                        $("#OtherShortlbl").text(data.Extra_decDesignName.substr(0, 6));
                                                        $("#OtherLonglbl").text(data.Extra_decDesignName);
                                                        $("#OtherDecorationID").val(data.extra_decoration);
                                                    }
                                                    else {
                                                        $("#OtherShortlbl").text("...");
                                                        $("#OtherLonglbl").text("Click On Search");
                                                        $("#OtherDecorationID").val("");
                                                    }

                                                    if ($('#txtDecorationBack').val() != "" && $('#txtDecorationBack').val() != undefined) {
                                                        GetDecorationCost("Back", $('#txtDecorationBack').val());
                                                    }
                                                    if ($('#txtDecorationFront').val() != "" && $('#txtDecorationFront').val() != undefined) {
                                                        GetDecorationCost("Front", $('#txtDecorationFront').val());
                                                    }
                                                    if ($('#txtDecorationLeft').val() != "" && $('#txtDecorationLeft').val() != undefined) {
                                                        GetDecorationCost("Left", $('#txtDecorationLeft').val());
                                                    }
                                                    if ($('#txtDecorationRight').val() != "" && $('#txtDecorationRight').val() != undefined) {
                                                        GetDecorationCost("Right", $('#txtDecorationRight').val());
                                                    }
                                                    if ($('#txtDecorationOther').val() != "" && $('#txtDecorationOther').val() != undefined) {
                                                        GetDecorationCost("Other", $('#txtDecorationOther').val());
                                                    }
                                                    if ($('#ddlSizeType').val() != "" && $('#ddlSizeType').val() != undefined) {
                                                        GetSize($('#ddlSizeType').val(), 'Edit');
                                                    }
                                                    $('#hdnIsCopyPressId').val("False");
                                                }

                                            },
                                            error: function (response) {
                                                alert();
                                            },
                                            type: 'post'

                                        });
                                    }
                                    else {
                                        OptionFormReset();
                                        $("#btnOptionSave").removeClass("customAlertChange");

                                    }
                                    // baans end 26th November
                                }
                                CustomAlert(response);
                                // baans change 12th Sept for Option Calculation
                                $('#txtTotalSize').val('');
                                //$("#txtTotalSize").css("border-color", "rgba(204, 204, 204, 1)");
                                $("#txtTotalSize").removeClass('customAlertChange');
                                // baans end 12th Sept

                            },
                            type: 'post',

                        });
                    }
                    else {
                        CustomWarning("Please make sure that you select the Decoration Type as well as the Quantity.");
                    }
                }
                else {
                    //CustomWarning('Fill all required fields.');
                    CustomErrorCode("Required");
                }
            }
            else {
                if ($('#txtSizesPacked').val() != "")
                    CustomWarning('Cannot create new record on packaging. Please select an option.');
            }
            if (Result != undefined && checkflag) {
                $("#btnOptionSave").removeClass("customAlertChange");
            }
        }
    }
    else {
        //CustomWarning('Fill all required fields.');
        CustomErrorCode("Required");
    }
    // baans end 29th September
    
}
function GetOptionGrid(OpportunityID) {
    var data, Status;
    if ($('#PageName').val() == "Opportunity" || $('#PageName').val() == "QuoteDetails")
        Status = "Opp";
    else
        Status = "Order";

    $.ajax({
        url: '/Opportunity/GetOptionGrid',
        data: { 'OpportunityID': OpportunityID, Status: Status },
        async: false,
        success: function (response) {
            data = response.data;
            $("#lbltotalbalance").text('$' + response.TotalWithShipping);
            // BAANS CHANGE 18TH August for GSTValue
            $("#lblGSTbalance").text(response.GSTValue);
            // baans end 18th August
            $("#lblpaidbalance").text('$' + (response.TotalPaid).toFixed(2));
            $("#lblbalance").text('$' + response.TotalDue);
            $('#lbltotlaPrice').text('$' + response.Total);
            $('#HiddenPaymentTotal').val(response.PaymentTotal);
            
            $('#lbltotlaMargin').text(response.TotalGp + '%');
        },
        error: function (response) {
            alert();
        },
        type: 'post'

    });
    var obj = {
        selectionModel: { type: 'row' },
        scrollModel: { horizontal: false, verticle: true },
        virtualX: true, virtualY: true,
        resizable: true,
        numberCell: { show: false },
        editable: false,
        //baans change 16th July
        wrap: false,
        hwrap: false,
        // baans end 16th July
        width: "95%",
        height: 270,
        columnTemplate: { width: 120, halign: "left" }
    };

    obj.colModel = [
        {
            title: "Option No", dataIndx: "DispalayId", align: "center", width: "4%", dataType: "string"
        },
        {
            title: "Qty", dataIndx: "quantity", width: "1%", align: "right", dataType: "int"
        },
        {
            title: "Code", dataIndx: "code", width: "1%", dataType: "string"
        },
        {
            title: "Colour", dataIndx: "colour", width: "3%", dataType: "string"
        },
        {
            title: "GP", dataIndx: "Margin", width: "1%", dataType: "float"
        },
        {
            //title: "Front Dec", dataIndx: "Front_decDesign", width: "4%", dataType: "string"
            title: "Front App", dataIndx: "Front_decDesign", width: "4%", dataType: "string"
        },
        {
            title: "Brand", dataIndx: "BrandName", width: "3%", dataType: "string"
        },
        //{
        //    title: "Code", dataIndx: "code", width: "1%", dataType: "string"
        //},
        {
            title: "Item", dataIndx: "ItemName", width: "7%", dataType: "string"
        },

        //{
        //    title: "Colour", dataIndx: "colour", width: "3%", dataType: "string"
        //},
        {
            title: "  Link", dataIndx: "Link", width: "3%", dataType: "string",
            render: function (ui) {
                var data = ui.cellData;
                var label;
                if (data != null && data != undefined) {
                    // baans change 16th Jan for Link
                    //label = "<a onclick = 'window.open('/" + data + "','_blank')' target='_blank'><label class='externalLnk'>" + data + "</label></a>";
                    label = "<a href='" + data + "' target='_blank'><label class='externalLnk'>" + data + "</label></a>";
                    // baans end 16th Jan
                }
                else {
                    label = data;
                }
                return label;
            }
        },
        {
            title: "Cost", dataIndx: "Cost", width: "1%", dataType: "float"
        },
        //{
        //    title: "GP", dataIndx: "Margin", width: "1%", dataType: "float"
        //},
        {
            title: "Sizes", dataIndx: "InitialSizes", width: "8%", dataType: "string"
        },
        //{
        //    title: "Front Dec", dataIndx: "Front_decDesign", width: "4%", dataType: "string"
        //},
        {
            //title: "Back Dec", dataIndx: "Back_decDesign", width: "4%", dataType: "string"
            title: "Back App", dataIndx: "Back_decDesign", width: "4%", dataType: "string"
        },
        {
            //title: "Lft Slv Dec", dataIndx: "Left_decDesign", width: "5%", dataType: "string"
            title: "Lft Slv App", dataIndx: "Left_decDesign", width: "5%", dataType: "string"
        },
        {
            //title: "Rht Slv Dec", dataIndx: "Right_decDesign", width: "5%", dataType: "string"
            title: "Rht Slv App", dataIndx: "Right_decDesign", width: "5%", dataType: "string"
        },
        {
            //title: "Extra Dec", dataIndx: "Extra_decDesign", width: "4%", dataType: "string"
            title: "Extra App", dataIndx: "Extra_decDesign", width: "4%", dataType: "string"
        },
        {
            title: "Other", dataIndx: "OtherDesc", width: "3%", dataType: "string"
        },
        {
            title: "Service", dataIndx: "Service", width: "4%", dataType: "string"
        },
        {
            title: "Unit", dataIndx: "uni_price", width: "4%", dataType: "float"
        },
        {
            title: "Unit+GST", dataIndx: "UnitInclGST", width: "5%", dataType: "float"
        },
        {
            title: "Ext Ex GST", dataIndx: "ExtExGST", width: "6%", dataType: "float"
        },
        {
            title: "Ext+GST", dataIndx: "ExtInclGST", width: "6%", dataType: "float"
        },
        {
            title: "Incl", dataIndx: "include", width: "1%", dataType: "string"
        },
        {
            title: "", dataIndx: "", width: "1%", dataType: "int",
            render: function (ui) {
                var dataindx = ui.rowIndx;
                var hostname = window.location.origin;
                return "<a onclick='deleteOptionRow(" + dataindx + ")'><img src='" + hostname + "/Content/images/DeleteContact.png' class='internalLnk' style='width:12px'/></a>";
            }
        }

    ];
    // baans change 26th November 
    if ($('#hdnIsCopyPressId').val() == "True") {
        var OptSelectId = parseInt($('#HiddenOptionID').val());
        var index = data.findIndex(x => x.id == OptSelectId);
        data[index].pq_rowattr = { style: "background:#cccccc;" };
        
    }
    // baans end 26th November
 // 17 Sep 2018 (N)
    obj.rowDblClick = function (evt, ui) {
        var rowIndx = getRowIndx(this);
        if (rowIndx != null) {
            // baans change 17th September for disable save on packing and invoice
            if ($("#PageName").val() == "InvoicingDetails" || $("#PageName").val() == "PackingDetails") {
               // $('#btnOptionSave').attr('disabled', false).removeClass('MakeReadonly');
            }
            // baans end 17th Sept
            var data = this.pdata[rowIndx];
            if (data != undefined && data != null) {
                $('#txtOptionQty').val(data.quantity);
                $('#txtCode').val(data.code);
                $('#txtColor').val(data.colour);
                $('#txtLink').val(data.Link);
                $('#txtCost').val(data.Cost);
                $('#txtMargin').val(data.Margin);
                // baans change 15th Sept
                //$('#ddlBrand').val(data.band_id);
                if (data.Status == "InActive") {
                    $('#hdnBrandStatus').val(data.Status);
                    $('#hdnBrandvalue').val(data.BrandName);
                    $('#hdnBrandId').val(data.band_id);
                    $("#ddlBrand").append($('<option></option>').attr("value", data.band_id).text(data.BrandName)).trigger("chosen:updated");
                   // $('#ddlBrand').val(data.band_id);
                    $('#ddlBrand').val(data.band_id).trigger("chosen:updated");
                }
                else {

                    //$('#ddlBrand').val(data.band_id);
                    $('#ddlBrand').val(data.band_id).trigger("chosen:updated");
                }
                // baans end 15th Sept
                //baans change 20th Sept
                //$('#ddlItem').val(data.item_id);

                var Id = data.item_id;
                $.ajax({
                    url: '/Opportunity/GetItemStatus',
                    data: { ItemId: Id },
                    async: false,

                    success: function (response) {
                        if (response == "InActive") {
                            $('#hdnTypeStatus').val('InActive');
                            $('#hdnTypevalue').val(data.ItemName);
                            $('#hdnTypeId').val(data.item_id);
                            $("#ddlItem").append($('<option></option>').attr("value", data.item_id).text(data.ItemName)).trigger("chosen:updated");
                            // baans change 27th October for chosen dropdown
                           // $('#ddlItem').val(data.item_id);
                            $('#ddlItem').val(data.item_id).trigger("chosen:updated");
                        }
                        else {
                            $('#ddlItem').val(data.item_id).trigger("chosen:updated");
                           // $('#ddlItem').val(data.item_id);
                        }

                        // baans end 27th October
                       
                    },
                    error: function (response) {
                        data = response;
                    },
                    type: 'get',
                });
                // baans end 20th Sept
                $('#txtotherdesc').val(data.OtherDesc);
                $('#txtunitprcExgst').val(data.uni_price);
                if (data.include == "Yes") {
                    $('#ddlinclude').prop('checked', true);
                } else {
                    $('#ddlinclude').prop('checked', false);
                }

                $('#txtothercost').val(data.OtherCost);
                $('#txtComment').val(data.comment);
                $('#ddlservice').val(data.Service);
                $('#ddlSizeType').val(data.SizeGrid);
                $('#txtSizes').val(data.InitialSizes);
                $('#txtSizesPacked').val(data.SizesPacked);
                //  $('#ddlDecline').val(data.Declined);
                $('#HiddenOptionID').val(parseInt(data.id));
                $('#txtDecorationFront').val(data.Front_decDesign);
                $('#txtRangeFront').val(data.Front_decQuantity);
                $('#hiddenRangeFrontCost').val(data.Front_decCost);
                $('#txtDecorationBack').val(data.Back_decDesign);
                $('#txtRangeBack').val(data.Back_decQuantity);
                $('#hiddenRangeBackCost').val(data.Back_decCost);
                $('#txtDecorationRight').val(data.Right_decDesign);
                $('#txtRangeRight').val(data.Right_decQuantity);
                $('#hiddenRangeRightCost').val(data.Right_decCost);
                $('#txtDecorationLeft').val(data.Left_decDesign);
                $('#txtRangeLeft').val(data.Left_decQuantity);
                $('#hiddenRangeLeftCost').val(data.Left_decCost);
                $('#txtDecorationOther').val(data.Extra_decDesign);
                $('#txtRangeOther').val(data.Extra_decQuantity);
                $('#hiddenRangeOtherCost').val(data.Extra_decCost);
                if (data.front_decoration != null && data.front_decoration != undefined) {
                    var shortName = "";
                    if (data.Front_decDesignName.length > 12) {
                        shortName = data.Front_decDesignName.substr(0, 12);
                        $("#FrontShortlbl").text(shortName);
                    }
                    else {
                        $("#FrontShortlbl").text(data.Front_decDesignName);
                    }

                    //$("#FrontShortlbl").text(data.Front_decDesignName.substr(0, 6));
                    $("#FrontLonglbl").text(data.Front_decDesignName);
                    $("#FrontDecorationID").val(data.front_decoration);
                }
                else {
                    $("#FrontShortlbl").text("......");
                    $("#FrontLonglbl").text("Click On Search");
                    $("#FrontDecorationID").val("");
                }

                if (data.back_decoration != null && data.back_decoration != undefined) {
                    var shortName = "";
                    if (data.Back_decDesignName.length > 12) {
                        shortName = data.Back_decDesignName.substr(0, 12);
                        $("#BackShortlbl").text(shortName);
                    }
                    else {
                        $("#BackShortlbl").text(data.Back_decDesignName);
                    }

                    //$("#BackShortlbl").text(data.Back_decDesignName.substr(0, 6));
                    $("#BackLonglbl").text(data.Back_decDesignName);
                    $("#BackDecorationID").val(data.back_decoration);
                }
                else {
                    $("#BackShortlbl").text("......");
                    $("#BackLonglbl").text("Click On Search");
                    $("#BackDecorationID").val("");
                }
                if (data.left_decoration != null && data.left_decoration != undefined) {
                    var shortName = "";
                    if (data.Left_decDesignName.length > 12) {
                        shortName = data.Left_decDesignName.substr(0, 12);
                        $("#LeftShortlbl").text(shortName);
                    }
                    else {
                        $("#LeftShortlbl").text(data.Left_decDesignName);
                    }

                    //$("#LeftShortlbl").text(data.Left_decDesignName.substr(0, 6));
                    $("#LeftLonglbl").text(data.Left_decDesignName);
                    $("#LeftDecorationID").val(data.left_decoration);
                }
                else {
                    $("#LeftShortlbl").text("......");
                    $("#LeftLonglbl").text("Click On Search");
                    $("#LeftDecorationID").val("");
                }
                if (data.right_decoration != null && data.right_decoration != undefined) {
                    var shortName = "";
                    if (data.Right_decDesignName.length > 12) {
                        shortName = data.Right_decDesignName.substr(0, 12);
                        $("#RightShortlbl").text(shortName);
                    }
                    else {
                        $("#RightShortlbl").text(data.Right_decDesignName);
                    }

                    //$("#RightShortlbl").text(data.Right_decDesignName.substr(0, 6));
                    $("#RightLonglbl").text(data.Right_decDesignName);
                    $("#RightDecorationID").val(data.right_decoration);
                }
                else {

                    $("#RightShortlbl").text("......");
                    $("#RightLonglbl").text("Click On Search");
                    $("#RightDecorationID").val("");
                }
                if (data.extra_decoration != null && data.extra_decoration != undefined) {
                    var shortName = "";
                    if (data.Extra_decDesignName.length > 12) {
                        shortName = data.Extra_decDesignName.substr(0, 12);
                        $("#OtherShortlbl").text(shortName);
                    }
                    else {
                        $("#OtherShortlbl").text(data.Extra_decDesignName);
                    }

                    //$("#OtherShortlbl").text(data.Extra_decDesignName.substr(0, 6));
                    $("#OtherLonglbl").text(data.Extra_decDesignName);
                    $("#OtherDecorationID").val(data.extra_decoration);
                }
                else {
                    $("#OtherShortlbl").text("......");
                    $("#OtherLonglbl").text("Click On Search");
                    $("#OtherDecorationID").val("");
                }

                if ($('#txtDecorationBack').val() != "" && $('#txtDecorationBack').val() != undefined) {
                    GetDecorationCost("Back", $('#txtDecorationBack').val());
                }
                if ($('#txtDecorationFront').val() != "" && $('#txtDecorationFront').val() != undefined) {
                    GetDecorationCost("Front", $('#txtDecorationFront').val());
                }
                if ($('#txtDecorationLeft').val() != "" && $('#txtDecorationLeft').val() != undefined) {
                    GetDecorationCost("Left", $('#txtDecorationLeft').val());
                }
                if ($('#txtDecorationRight').val() != "" && $('#txtDecorationRight').val() != undefined) {
                    GetDecorationCost("Right", $('#txtDecorationRight').val());
                }
                if ($('#txtDecorationOther').val() != "" && $('#txtDecorationOther').val() != undefined) {
                    GetDecorationCost("Other", $('#txtDecorationOther').val());
                }
                if ($('#ddlSizeType').val() != "" && $('#ddlSizeType').val() != undefined) {
                    GetSize($('#ddlSizeType').val(), 'Edit');
                }

                //17 Oct 2019 (N) ProofSent
                if (data.ProofSent == true) {
                    $('#chkProofSelect').prop('checked', true);
                }
                else {
                    $('#chkProofSelect').prop('checked', false);
                }
                if (data.ProofMailSent != null) {
                    $('#mailIcon').css('visibility', 'visible').css('color', '#11de11');
                    //$('#mailIconOpen').css('visibility', 'visible');
                }
                else {
                    $('#mailIcon').css('visibility', 'visible').css('color', 'red');
                    //$('#mailIconOpen').css('visibility', 'hidden');
                }
                //17 Oct 2019 (N) ProofSent
            }

        }


    }

    obj.dataModel = { data: data };

    pq.grid("#OptionsGrid", obj);
    pq.grid("#OptionsGrid", "refreshDataAndView");

}
function GetDecorationCost(dropdwnId, Desc) {

    var url = "/Opportunity/GetDecorationCostById/";

    $.ajax({
        url: url,
        data: { Desc: Desc },
        cache: false,
        type: "get",
        success: function (data) {
            //alert(data.length);
            //alert(JSON.stringify(data));
            var markup = "<div class='items Header row'><div class='col-lg-6 LeftCol'>Quantity</div><div class='col-lg-6 rightCol'>Cost</div></div>";
            //var markup = "<div class='row'>";
            for (var x = 0; x < data.length; x++) {
                //    markup += "<option value='" + data[x].Quantity + "'" + data[x].Quantity + "</div><div class='col-lg-6'><input id='" + data[x].Text + "' class='sizeQuantity' type='number'/></div></div>";

                markup += "<div class='items row'><div class='col-lg-6 LeftCol'><label class='quantity'>" + data[x].Quantity + "</label></div><div class='col-lg-6 rightCol'><label class='cost'>" + data[x].Cost + "</label></div></div>";
            }
            //markup += "</div>";
            $("#ListRange" + dropdwnId + "").html(markup);

            $("#txtRange" + dropdwnId + "").click(function () {
                if ($("ul.dropdown-content.active").length == 0) {

                    //$("#txtRange" + dropdwnId + "").val("");
                    //$("#hiddenRange" + dropdwnId + "Cost").val("");

                    $(this).parent().find("ul.dropdown-content").addClass("active").css("display", "block");

                    $(this).parent().find('.items').not(".Header").click(function () {

                        $("#btnOptionSave").addClass("customAlertChange");
                        var Quantity = "";
                        var Cost = "";
                        Quantity = $(this).find('.quantity').text();
                        Cost = $(this).find('.cost').text();
                        $("#txtRange" + dropdwnId + "").val(Quantity);
                        $("#hiddenRange" + dropdwnId + "Cost").val(Cost);
                        CalculateUnitPrice('');
                        $(this).parent().removeClass("active").css("display", "none");
                    });
                }

            });

        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });
}
function CalculateUnitPrice(event) {
    if (event != '') {
        var value = event.val();
        if (parseFloat(value) < 0) {
            event.val('0')
            CustomWarning('Negative values not allowed');
        }
    }
    var Price, OtherCost, Cost, FrontDecCost, BackDecCost, LeftDecCost, RightDecCost, OtherDecCost, GP;
    Price = parseFloat(0);
    if ($('#txtothercost').val() == "" || $('#txtothercost').val() == undefined) {
        OtherCost = 0;
    }
    else {
        OtherCost = parseFloat($('#txtothercost').val());
    }
    if ($('#txtCost').val() == "" || $('#txtCost').val() == undefined) {
        Cost = 0;
    }
    else {
        Cost = parseFloat($('#txtCost').val());
    }
    if ($('#hiddenRangeFrontCost').val() == "" || $('#hiddenRangeFrontCost').val() == undefined) {
        FrontDecCost = 0;
    }
    else {
        FrontDecCost = parseFloat($('#hiddenRangeFrontCost').val());
    }
    if ($('#hiddenRangeBackCost').val() == "" || $('#hiddenRangeBackCost').val() == undefined) {
        BackDecCost = 0;
    }
    else {
        BackDecCost = parseFloat($('#hiddenRangeBackCost').val());
    }
    if ($('#hiddenRangeLeftCost').val() == "" || $('#hiddenRangeLeftCost').val() == undefined) {
        LeftDecCost = 0;
    }
    else {
        LeftDecCost = parseFloat($('#hiddenRangeLeftCost').val());
    }
    if ($('#hiddenRangeRightCost').val() == "" || $('#hiddenRangeRightCost').val() == undefined) {
        RightDecCost = 0;
    }
    else {
        RightDecCost = parseFloat($('#hiddenRangeRightCost').val());
    }
    if ($('#hiddenRangeOtherCost').val() == "" || $('#hiddenRangeOtherCost').val() == undefined) {
        OtherDecCost = 0;
    }
    else {
        OtherDecCost = parseFloat($('#hiddenRangeOtherCost').val());
    }
    if ($('#txtMargin').val() == "" || $('#txtMargin').val() == undefined) {
        GP = 0;
    }
    else {
        GP = parseFloat($('#txtMargin').val());
    }


    var Total = (Price + OtherCost + Cost + FrontDecCost + BackDecCost + LeftDecCost + RightDecCost + OtherDecCost) / ((100 - GP) * 0.01)
    $('#txtunitprcExgst').val(parseFloat(Total).toFixed(2));
}
function MarginBackCalculate(event) {
    var value = event.val();
    if (value == "")
    {
        value = "0";
    }
    if (parseFloat(value) < 0) {
        event.val('0')
        CustomWarning('Negative values not allowed');
    }
    var Price, OtherCost, Cost, FrontDecCost, BackDecCost, LeftDecCost, RightDecCost, OtherDecCost, UnitPrice;
    Price = parseFloat(0);
    if ($('#txtothercost').val() == "" || $('#txtothercost').val() == undefined) {
        OtherCost = 0;
    }
    else {
        OtherCost = parseFloat($('#txtothercost').val());
    }
    if ($('#txtCost').val() == "" || $('#txtCost').val() == undefined) {
        Cost = 0;
    }
    else {
        Cost = parseFloat($('#txtCost').val());
    }
    if ($('#hiddenRangeFrontCost').val() == "" || $('#hiddenRangeFrontCost').val() == undefined) {
        FrontDecCost = 0;
    }
    else {
        FrontDecCost = parseFloat($('#hiddenRangeFrontCost').val());
    }
    if ($('#hiddenRangeBackCost').val() == "" || $('#hiddenRangeBackCost').val() == undefined) {
        BackDecCost = 0;
    }
    else {
        BackDecCost = parseFloat($('#hiddenRangeBackCost').val());
    }
    if ($('#hiddenRangeLeftCost').val() == "" || $('#hiddenRangeLeftCost').val() == undefined) {
        LeftDecCost = 0;
    }
    else {
        LeftDecCost = parseFloat($('#hiddenRangeLeftCost').val());
    }
    if ($('#hiddenRangeRightCost').val() == "" || $('#hiddenRangeRightCost').val() == undefined) {
        RightDecCost = 0;
    }
    else {
        RightDecCost = parseFloat($('#hiddenRangeRightCost').val());
    }
    if ($('#hiddenRangeOtherCost').val() == "" || $('#hiddenRangeOtherCost').val() == undefined) {
        OtherDecCost = 0;
    }
    else {
        OtherDecCost = parseFloat($('#hiddenRangeOtherCost').val());
    }
    if ($('#txtunitprcExgst').val() == "" || $('#txtunitprcExgst').val() == undefined) {
        UnitPrice = 0;
    }
    else {
        UnitPrice = parseFloat($('#txtunitprcExgst').val());
    }
    var TotalMargin=0;
    if (UnitPrice != 0) {
        TotalMargin = 100 - ((Price + OtherCost + Cost + FrontDecCost + BackDecCost + LeftDecCost + RightDecCost + OtherDecCost) / (UnitPrice * 0.01));
    }
    // ((100 - GP) * 0.01)
    $('#txtMargin').val(parseFloat(TotalMargin).toFixed(2));
}
// baans change 20th November for calculate Unit price before saving the option
function CalculateUnitPriceAtOptionSavingStage(event) {
    if (event != '') {
        var value = event.val();
        if (parseFloat(value) < 0) {
            event.val('0')
            CustomWarning('Negative values not allowed');
        }
    }
    var Price, OtherCost, Cost, FrontDecCost, BackDecCost, LeftDecCost, RightDecCost, OtherDecCost, GP;
    Price = parseFloat(0);
    if ($('#txtothercost').val() == "" || $('#txtothercost').val() == undefined) {
        OtherCost = 0;
    }
    else {
        OtherCost = parseFloat($('#txtothercost').val());
    }
    if ($('#txtCost').val() == "" || $('#txtCost').val() == undefined) {
        Cost = 0;
    }
    else {
        Cost = parseFloat($('#txtCost').val());
    }
    var txtRangeFrontQuan = $("#txtRangeFront").val();
    if (($('#hiddenRangeFrontCost').val() == "" || $('#hiddenRangeFrontCost').val() == undefined) || (txtRangeFrontQuan == "" || txtRangeFrontQuan == undefined)) {
        FrontDecCost = 0;
    }
    else {
        FrontDecCost = parseFloat($('#hiddenRangeFrontCost').val());
    }
    var txtRangeBackQuan = $("#txtRangeBack").val();
    if (($('#hiddenRangeBackCost').val() == "" || $('#hiddenRangeBackCost').val() == undefined) || (txtRangeBackQuan == "" || txtRangeBackQuan == undefined)) {
        BackDecCost = 0;
    }
    else {
        BackDecCost = parseFloat($('#hiddenRangeBackCost').val());
    }
    var txtRangeLeftQuan = $("#txtRangeLeft").val();
    if (($('#hiddenRangeLeftCost').val() == "" || $('#hiddenRangeLeftCost').val() == undefined) || (txtRangeLeftQuan == "" || txtRangeLeftQuan == undefined)) {
        LeftDecCost = 0;
    }
    else {
        LeftDecCost = parseFloat($('#hiddenRangeLeftCost').val());
    }
    var txtRangeRightQuan = $("#txtRangeRight").val();
    if (($('#hiddenRangeRightCost').val() == "" || $('#hiddenRangeRightCost').val() == undefined) || (txtRangeRightQuan == "" || txtRangeRightQuan == undefined)) {
        RightDecCost = 0;
    }
    else {
        RightDecCost = parseFloat($('#hiddenRangeRightCost').val());
    }
    var txtRangeOtherQuan = $("#txtRangeOther").val();
    if (($('#hiddenRangeOtherCost').val() == "" || $('#hiddenRangeOtherCost').val() == undefined) || (txtRangeOtherQuan == "" || txtRangeOtherQuan == undefined)) {
        OtherDecCost = 0;
    }
    else {
        OtherDecCost = parseFloat($('#hiddenRangeOtherCost').val());
    }
    if ($('#txtMargin').val() == "" || $('#txtMargin').val() == undefined) {
        GP = 0;
    }
    else {
        GP = parseFloat($('#txtMargin').val());
    }




    var Total = (Price + OtherCost + Cost + FrontDecCost + BackDecCost + LeftDecCost + RightDecCost + OtherDecCost) / ((100 - GP) * 0.01)
    $('#txtunitprcExgst').val(parseFloat(Total).toFixed(2));
}
// baans end 20th November
function txtrangeonBlur() {
    setTimeout(function () {

        $("ul.dropdown-content.active").removeClass("active").css("display", "none");
    }, 200);
}


// baans change 15th Sept for Check brand check
//function CheckBrand(value) {
//    var Data = value;
//    var filter = /^[A-Za-z]+$/;
//    if (filter.test(Data)) { }
//    else {
//        $('#txtNewBrand').val('');
//    }
    
//}

// baans end 15th Sept

// baans change 23rd October for Deleting Option
function deleteOptionRow(rowIndx) {
    
    if ($('#PageName').val() == "Opportunity" || $('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "OrderDetails")  {
        bootbox.confirm("Do you want to delete the Option?", function (result) {
            if (result) {
                var rowdata = $('#OptionsGrid').pqGrid("getRowData", { rowIndx: rowIndx });
                var id = rowdata.id;
                $.ajax({
                    url: '/Opportunity/DeleteOption',
                    data: { Id: id },
                    async: false,
                    success: function (response) {
                        var Res = { Result: "Success", Message: response.Message };
                        CustomAlert(Res);
                    },
                    type: "Post"
                });
                var OppId = $('#lblOpportunityId').text();
                GetOptionGrid(OppId);
            }
            else {
            }
        })
    }
    else
    {
      CustomWarning("Option cannot be deleted at this stage !!!")
    }
}
// baans end 23rd October


function OpenQuote() {
    if ($('#lblOpportunityId').text() != "000000") {
        location.href = "/Opportunity/QuoteDetails/" + $('#lblOpportunityId').text();
    }
    else {

    }
}
function GetOppById(OppId) {
    if (OppId != undefined && OppId != 0) {

        $.ajax({
            url: '/Opportunity/GetOppById',
            data: { OppId: OppId },
            async: false,
            success: function (response) {
                //tarun 18/09/2018
                if ($('#PageName').val() == "ShippingDetails" || $('#PageName').val() == "InvoicingDetails" || $('#PageName').val() == "PackingDetails" || $('#PageName').val() == "JobDetails" || $("#PageName").val() == "CompleteDetails") {
                    
                    if (response.AddressId != null && response.AddressId != 0) {
                        var count = CheckDeliveryAddress(response.OrgID);
                        GetOrganisationAddress(response.AddressId, "Purchase");
                    }
                    else {
                        var count = CheckDeliveryAddress(response.OrgID);
                        GetOrganisationAddress(response.OrgID, "");
                    }
                }
                //end
                // 17 Aug 2018 (N)
                $('#lblOpportunityId').text(response.DisplayOpportunityId);
                // 17 Aug 2018 (N)
                // baans change 26th September for  Plandate in right side bar
                //if ($("#PageName").val() == "OpportunityDetails" || $("#PageName").val() == "QuoteDetails" || $("#PageName").val() == "OrderDetails" || $("#PageName").val() == "JobDetails" || $("#PageName").val() == "PackingDetails" || $("#PageName").val() == "InvoicingDetails" || $("#PageName").val() == "ShippingDetails" || $("#PageName").val() == "CompleteDetails") {
                    var Id = $('#lblOpportunityId').text();
                    if (OppId != "") {
                        var OppId = parseInt(Id);
                        $.ajax({
                            url: '/Opportunity/GetOppProductionDate',
                            data: { OppId: OppId },
                            async: false,
                            success: function (response) {
                                if (response.length > 0) {
                                    if (response.length < 2)
                                    {
                                        if (response[0].ProductionDate != null)
                                        {
                                            $('#lblPlanDate1').text(DateFormat(response[0].ProductionDate));
                                        var DepId = parseInt(response[0].DeptId);
                                        $.ajax({
                                            url: '/Opportunity/GetOppProductionDepartment',
                                            data: { DepId: DepId },
                                            async: false,
                                            success: function (response) {
                                                $('#lblProdLine').text(response[0].department);
                                            },
                                            error: function (response) {
                                                alert("error");
                                            },
                                            type: 'post'
                                        });
                                       }
                                   }
                                    else if (response.length > 1) {
                                        $('#lblPlanDate1').text(DateFormat(response[0].ProductionDate));
                                        $('#lblPlanDate2').text(DateFormat(response[1].ProductionDate));
                                        var DepId = parseInt(response[0].DeptId);
                                        var DepId1 = parseInt(response[1].DeptId);

                                        $.ajax({
                                            url: '/Opportunity/GetOppProductionDepartment',
                                            data: { DepId: DepId },
                                            async: false,
                                            success: function (response) {
                                                
                                                $('#lblProdLine').text(response[0].department);
                                                $.ajax({
                                                    url: '/Opportunity/GetOppProductionDepartment',
                                                    data: { DepId: DepId1 },
                                                    async: false,
                                                    success: function (response) {
                                                       
                                                        $('#lblProdLine1').text(response[0].department);
                                                        //alert("hii Line 2");
                                                    },
                                                    error: function (response) {
                                                        alert("error");
                                                    },
                                                    type: 'post'
                                                });
                                            },
                                            error: function (response) {
                                                alert("error");
                                            },
                                            type: 'post'
                                        });
                                    }
                                }  
                            },
                            error: function (response) {
                                alert("error");
                            },
                            type: 'post'
                        });
                    }
                //}
    // baans end 26th September
                $('#hdnOppName').val(response.OppName);
                $('#hdnforoppOrgID').val(response.OrgID);
                $('#lblOppDate').text(DateTimeFormat(response.OppDate));
                $('#lblQuoteDate').text(DateTimeFormat(response.QuoteDate));
                $('#lblOrderDate').text(DateTimeFormat(response.Orderdate));
                $('#lblJobDate').text(DateTimeFormat(response.JobDate));
                $('#lblPlanedDate').text(DateTimeFormat(response.PlannedDate));
                $('#lblOrderConfirmDate').text(DateTimeFormat(response.OrderConfirmedDate));
                $('#lblJobAcceptedDate').text(DateTimeFormat(response.JobAcceptedDate));
                $('#lblArtOrderdDate').text(DateTimeFormat(response.ArtOrderedDate));
                $('#lblProofCreatedDate').text(DateTimeFormat(response.ProofCreatedDate));
                $('#lblProofSentDate').text(DateTimeFormat(response.ProofSentdate));
                $('#lblProofApprovedDate').text(DateTimeFormat(response.ApprovedDate));
                $('#lblDigiReadyDate').text(DateTimeFormat(response.ArtReadyDate));
                $('#lblStockOrderedDate').text(DateTimeFormat(response.StockOrderedDate));
                $('#lblStockInDate').text(DateTimeFormat(response.ReceivedDate));
                $('#lblStockCheckedDate').text(DateTimeFormat(response.Checkeddate));
                $('#lblStockDecoratedDate').text(DateTimeFormat(response.DecoratedDate));
                $('#lblOrderPackedDate').text(DateTimeFormat(response.PackingDate));
                $('#lblInvoiceDate').text(DateTimeFormat(response.InvoicingDate));
                $('#lblPaidDate').text(DateTimeFormat(response.PaidDate));
                $('#lblShippedDate').text(DateTimeFormat(response.ShippedDate));
                $('#lblCompleteDate').text(DateTimeFormat(response.CompleteDate));
                $('#lblManagerName').text(response.AccountManagerName);
                if (response.OppThumbnail != null && response.OppThumbnail != undefined && response.OppThumbnail != "") {
                    $('#imageOppThumbnail').attr('src', '/Content/uploads/Opportunity/' + response.OppThumbnail)
                    //$('#imageUserExist').attr('src', '/Content/uploads/Opportunity/' + response.OppThumbnail)
                    
                }
                else {
                    $('#imageOppThumbnail').attr('src', '/Content/uploads/Opportunity/NoImage.png')
                }
                if (response.ProdLine != null && response.ProdLine != undefined) {
                   // $('#lblProdLine').text(response.ProdLine);
                }
                else {
                    //$('#lblProdLine').text("");
                }
                $('#oppName').val(response.OppName);
                SetAccountManager(response.AcctManagerId, "ddlOppAcctMgr", "OppAccountManager");
                //  $('#ddlOppAcctMgr').val(response.AcctManagerId);
                $('#oppQuantity').val(response.Quantity);
                $('#datepicker').val(DateFormat(response.ReqDate));
                $('#oppSource').val(response.Source);
                $('#oppCampaign').val(response.Compaign);
                $('#txtrepeatfrom').val(response.RepeatJobId);
                $('#txtlost').val(response.Lost);
                $("#OppShipping").val(response.Shipping);
                
                $('#ddlDecline').val(response.Declined);
                $('#txtCancelled').val(response.Cancelled);
                $("#txtConfirmedDate").val(DateFormat(response.ConfirmedDate));
                $('#txtshippingprice').val(response.Price);
                $('#txtshippingday').val(response.ShippingTo);
                $('#depositreqdate').val(DateFormat(response.DepositReqDate));
                $('#ddlPackedInSet1').val(response.PackedInSet1);
                $('#ddlPackedInSet2').val(response.PackedInSet2);
                $('#txtConsigNoteNo').val(response.ConsigNoteNo);
                $('#txtPackingNotes').val(response.PacagingNotes);
                // $('#HiddenForOppStage').val(response.StageID);
                // SetStage(response.StageID, '');
                $("#ddlDeliveryDate").val(response.DeliveryDate);
                SetOppoStage();

                $('#HiddenOppNotes').val(response.OppNotes);
                $('#HiddenQuoteNotes').val(response.QuoteNotes);
                $('#HiddenOrderNotes').val(response.OrderNotes);
                $('#HiddenJobNotes').val(response.JobNotes);
                $('#HiddenPackingNotes').val(response.PackingNotes);
                $('#HiddenInvoiceingNotes').val(response.InvoicingNotes);
                $('#HiddenShippingNotes').val(response.ShippingNotes);
                $('#HiddenCompleteNotes').val(response.CompleteNotes);
                if (response.EventId != null && response.EventId != undefined) {
                    $("#HiddenEventId").val(response.EventId);
                    $("#HiddenOppEventId").val(response.EventId);
                }
                if ($("#PageName").val() == "Opportunity") {
                    $('#oppStage').val("Opportunity");
                    $('#oppNotes').val(response.OppNotes);
                    if (response.EventId != null && response.EventId != undefined) {
                        GetEventById(response.EventId);
                    }
                }
                else if ($("#PageName").val() == "QuoteDetails") {
                    $('#txtMailMessage2').val(response.QuoteMail);
                    $('#oppNotes').val(response.QuoteNotes);
                    $('#oppStage').val("Quote");
                }
                else if ($("#PageName").val() == "OrderDetails") {
                    $('#txtMailMessage2').val(response.QuoteMail);
                    $('#oppNotes').val(response.OrderNotes);
                    $('#oppStage').val("Order");
                }
                else if ($("#PageName").val() == "JobDetails") {
                    $('#txtMailMessage2').val(response.QuoteMail);
                    $('#oppNotes').val(response.JobNotes);
                    $('#oppStage').val("Job");
                }
                else if ($("#PageName").val() == "InvoicingDetails") {
                    $('#oppNotes').val(response.InvoicingNotes);
                    $('#oppStage').val("Invoicing");
                }
                else if ($("#PageName").val() == "PackingDetails") {
                    $('#oppNotes').val(response.PackingNotes);
                    $('#oppStage').val("Packing");
                }
                else if ($("#PageName").val() == "ShippingDetails") {
                    $('#oppNotes').val(response.ShippingNotes);
                    $('#oppStage').val("Shipping"); 
                }
                else if ($("#PageName").val() == "CompleteDetails") {
                    $('#oppNotes').val(response.CompleteNotes);
                    $('#oppStage').val("Complete");
                }
                //if (response.DepartmentName != "" && response.DepartmentName != undefined && response.DepartmentName != null) {
                //    ms.setValue(response.DepartmentName.split(','));
                //}

                if (response.job_department != "" && response.job_department != undefined && response.job_department != null) {
                  
                        ms.setValue(response.job_department.split(','));
                  
                    
                }
                //22 Aug 2018 (N)
                if ($('#PageName').val() == "Event") {
                    MakeOpportunityReadonly();
                }
                //22 Aug 2018 (N)
            },
            error: function (response) {
            },
            type: 'post'

        });
    }
}
function SetNotes(Stage) {
    if (Stage == "Opportunity")
        $('#HiddenOppNotes').val($('#oppNotes').val());
    if (Stage == "Quote")
        $('#HiddenQuoteNotes').val($('#oppNotes').val());
    if (Stage == "Order")
        $('#HiddenOrderNotes').val($('#oppNotes').val());
    if (Stage == "Job")
        $('#HiddenJobNotes').val($('#oppNotes').val());
    if (Stage == "Packing")
        $('#HiddenPackingNotes').val($('#oppNotes').val());
    if (Stage == "Invoicing")
        $('#HiddenInvoiceingNotes').val($('#oppNotes').val());
    if (Stage == "Shipping")
        $('#HiddenShippingNotes').val($('#oppNotes').val());
    if (Stage == "Complete")
        $('#HiddenCompleteNotes').val($('#oppNotes').val());

}

function readURL(input) {
    var checkFlag = true;
    if ($('#lblOpportunityId').text() == "000000") {
        CustomWarning("Please Save Opportunity First");
        checkFlag = false;
    }
    var currentId = input.id.split('~');
    var hdnimg = currentId[0];
    var base64data;

    if (checkFlag == true) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                // $('#PetImg').attr('src', e.target.result);
                var image = e.target.result;
                //$('#HiddenImg').val(image);

                var OppId = $('#lblOpportunityId').text();
                var filename = hdnimg + "_" + OppId + "." + input.files[0].name.split('.')[1];
                if ($("#" + hdnimg + "hdnimg").val() != null && $("#" + hdnimg + "hdnimg").val() != "") {
                    filename = $("#" + hdnimg + "hdnimg").val();
                }

                $.ajax({

                    url: '/Opportunity/UploadOppImage',
                    data: { imageData: image, filename: filename, OppId: OppId },
                    async: false,
                    success: function (response) {
                        data = response;

                        CustomAlert(response);
                        if (response.ID > 0) {
                            //getAllImages(response.ID);
                            GetInquiryData(response.ID);
                        }

                    },
                    type: 'post',

                });
            }

            reader.readAsDataURL(input.files[0]);
        }
    }

}

function GetInquiryData(Oppid) {
    //var hostname = window.location.origin;
    if (Oppid > 0) {
        $.ajax({
            url: '/Opportunity/GetInquiryData',
            data: { Oppid: Oppid },
            async: false,
            success: function (response) {
                data = response;
                //alert(JSON.stringify(data));
                if (data.FrontPrintArt != null && data.FrontPrintArt != "") {
                    $("#fronthdnimg").val(data.FrontPrintArt);
                    $(".frontdetails").css("display", "block");
                }

                if (data.BackPrintArt != null && data.BackPrintArt != "") {
                    $("#backhdnimg").val(data.BackPrintArt);
                    $(".backdetails").css("display", "block");
                }

                if (data.LeftPrintArt != null && data.LeftPrintArt != "") {
                    $("#lefthdnimg").val(data.LeftPrintArt);
                    $(".leftdetails").css("display", "block");
                }

                if (data.RighPrintArt != null && data.RighPrintArt != "") {
                    $("#righthdnimg").val(data.RighPrintArt);
                    $(".rightdetails").css("display", "block");
                }

                if (data.ItemColours != null && data.ItemColours != "") {
                    $("#txtitemcolor").val(data.ItemColours);
                }

                if (data.PrefBrandsAndStyle != null && data.PrefBrandsAndStyle != "") {
                    $("#txtbrandstyle").val(data.PrefBrandsAndStyle);
                }

                if (data.GeneralNotes != null && data.GeneralNotes != "") {
                    $("#txtgeneraloppnotes").val(data.GeneralNotes);
                }

                if (data.FrontPrintNotes != null && data.FrontPrintNotes != "") {
                    $("#txtfrontprintnotes").val(data.FrontPrintNotes);
                }

                if (data.BackPrintNotes != null && data.BackPrintNotes != "") {
                    $("#txtbackprintnotes").val(data.BackPrintNotes);
                }

                if (data.LEftPrintNotes != null && data.LEftPrintNotes != "") {
                    $("#txtgeneraloppnotes").val(data.LEftPrintNotes);
                }

                if (data.RightPrintNotes != null && data.RightPrintNotes != "") {
                    $("#txtrightprintnotes").val(data.RightPrintNotes);
                }


            },
            type: 'post',

        });
    }
}

function UpdateOppInquiry(OppId) {
    var checkFlag = false;
    var txtitemcolor, txtbrandstyle, txtgeneraloppnotes, fronthdnimg, txtfrontprintnotes, backhdnimg, txtbackprintnotes, lefthdnimg, txtleftprintnotes, righthdnimg, txtrightprintnotes;

    if ($("#txtitemcolor").val() != "") {
        txtitemcolor = $("#txtitemcolor").val();
        checkFlag = true;
    }

    if ($("#txtbrandstyle").val() != "") {
        txtbrandstyle = $("#txtbrandstyle").val();
        checkFlag = true;
    }

    if ($("#txtgeneraloppnotes").val() != "") {
        txtgeneraloppnotes = $("#txtgeneraloppnotes").val();
        checkFlag = true;
    }

    if ($("#fronthdnimg").val() != "") {
        fronthdnimg = $("#fronthdnimg").val();
        checkFlag = true;
    }

    if ($("#txtfrontprintnotes").val() != "") {
        txtfrontprintnotes = $("#txtfrontprintnotes").val();
        checkFlag = true;
    }

    if ($("#backhdnimg").val() != "") {
        backhdnimg = $("#backhdnimg").val();
        checkFlag = true;
    }

    if ($("#txtbackprintnotes").val() != "") {
        txtbackprintnotes = $("#txtbackprintnotes").val();
        checkFlag = true;
    }

    if ($("#lefthdnimg").val() != "") {
        lefthdnimg = $("#lefthdnimg").val();
        checkFlag = true;
    }

    if ($("#txtleftprintnotes").val() != "") {
        txtleftprintnotes = $("#txtleftprintnotes").val();
        checkFlag = true;
    }

    if ($("#righthdnimg").val() != "") {
        righthdnimg = $("#righthdnimg").val();
        checkFlag = true;
    }

    if ($("#txtrightprintnotes").val() != "") {
        txtrightprintnotes = $("#txtrightprintnotes").val();
        checkFlag = true;
    }

    if (checkFlag == true) {
        $.ajax({
            url: "/Opportunity/UpdateOppInquiry",
            data: { OpportunityId: OppId, ItemColours: txtitemcolor, PrefBrandsAndStyle: txtbrandstyle, GeneralNotes: txtgeneraloppnotes, FrontPrintArt: fronthdnimg, FrontPrintNotes: txtfrontprintnotes, BackPrintArt: backhdnimg, BackPrintNotes: txtbackprintnotes, LeftPrintArt: lefthdnimg, LEftPrintNotes: txtleftprintnotes, RighPrintArt: righthdnimg, RightPrintNotes: txtrightprintnotes },
            async: false,
            type: 'get',
            success: function (response) {
                CustomAlert(response);
                if (response.ID > 0) {
                    GetInquiryData(response.ID);
                }
            },
            error: function (response) {
                CustomAlert(response);
            }
        });
    }

}
function br2nl(str) {
    if (str != null && str != "") {
        return str.replace(/<br\s*\/?>/mg, "\n");
    } else {
        return "";
    }
}
// baans change 13th Sept for open brandModel
function ShowBrandModal() {
    $('#txtNewBrand').val('');
    var Data = $('#ddlBrand').val().value;
    // baans 19th Sept
    var NewBrand = "Add New";
    $.ajax({
        url: '/Opportunity/GetBrandId',
        data: { BrandName: NewBrand },
        async: false,
        success: function (response) {
            if (response != null && response != undefined && response != "") {
                if ($('#ddlBrand').val() == response.id && response.name == "Add New") {
                    $("#BrandModel").css("display", "block");
                    $('#ddlBrand').val('Select');
                }

            }
        },
        type: 'post',

    });
    // baans 19th Sept


    //if ($('#ddlBrand').val() == "65") {
    //    $("#BrandModel").css("display", "block");
    //    $('#ddlBrand').val('Select');
    //}
}
function SaveBrand() {
    if ($('#txtNewBrand').val() != null) {
        var BrandData = $('#txtNewBrand').val();
        $.ajax({
            url: '/Opportunity/SaveNewBrand',
            data: { OptionBrand: BrandData },
            async: false,
            success: function (response) {
                if (response != null && response != undefined && response != "") {
                    //$('#ddlBrand').val(response.source);
                    //location.reload();
                    $("#ddlBrand").append($('<option></option>').attr("value", response.ID).text(response.Result)).trigger("chosen:updated");
                    var Res = { Result: "Success", Message: response.Message };
                    CustomAlert(Res);

                }
            },
            type: 'post',

        });
    }
}
// baans end 13th Sept

//baans change 10th Jan for MakeRepeatOrderAtCompleteStage
function MakeRepeatOrder() {
    var OppId = $('#lblOpportunityId').text();
    $.ajax({
        url: '/Opportunity/StatusChkByOppIdForMakeRepeat',
        data: { OppId: OppId },
        async: false,
        success: function (response) {
            if (response == true) {
                $.ajax({
                    url: '/Opportunity/MakeRepeatOrder',
                    data: { OppId: OppId },
                    async: false,
                    success: function (response) {
                        CustomAlert(response);
                        window.location = '/opportunity/QuoteDetails/' + parseInt(response.ID);
                    },
                    error: function (response) {
                        alert(response);
                    },
                    type: 'post'
                });
            }
            else {
                CustomError("Please Make Sure that the Opportunity has reached the Complete Stage!!!");
            }
        },
        error: function (response) {
            alert(response);
        },
        type: 'post'

    });
}
//baans end 10th Jan

function openEmailModal(MailType) {
    /*tarun 12/10/2018*/
    var IsMailValid = true;
    if (MailType == 'Invoice') {
        if ($('#PageName').val() != "InvoicingDetails") {
            IsMailValid = false;
        }
    }
    //end
    if (IsMailValid) {     /*tarun 12/10/2018 condition change here*/
        $('#HiddenforConfirm').val('No');
        var data = QuoteValidCheck();
        var OptionStatus = "Order";
        if ($('#PageName').val() == "QuoteDetails") {
            OptionStatus = "Opp";
        }
        if (MailType == "Invoice") {
            $('#HiddenforConfirm').val('Invoice');
            OptionStatus = "Invoice";
        }
        if (MailType == "Proof") {
            $('#HiddenforConfirm').val('Proof');
            OptionStatus = "Proof";
        }
        if (data.resultOrg) {
            if (data.OptionCount != "0") {
                $.ajax({
                    url: '/MasterPdf/GetMailMessage',
                    data: { OpportunityId: $('#lblOpportunityId').text(), OptionStatus: OptionStatus },
                    async: false,
                    success: function (response) {
                        if (response != null && response != undefined && response != "") {
                            //$('#txtMailMessage1').html(br2nl(response.Body1));
                            //$('#txtMailMessage3').val(br2nl(response.Body2));
                            var EmailContent = "";
                            if (response.Body2 != null && response.Body2 != "") {
                                EmailContent = br2nl(response.Body1) + "\n \n" + br2nl(response.Body2);
                            }
                            else {
                                EmailContent = br2nl(response.Body1);
                            }
                            $('#txtMailMessage2').val(EmailContent);
                            //$('#txtMailSubject').val(br2nl(response.Subject + $('#oppName').val()));
                            $('#txtMailSubject').val(br2nl(response.Subject));      //+$('#oppName').val() 20 Sep(N)
                            $('#txtToMail').val(response.ClientEmailID);


                        }
                    },
                    type: 'post',

                });

                $("#EmailModel").css("display", "block");
            }
            else {
                CustomWarning('Option data does not exist');
            }
        }
        else if (data.result == true && data.resultOrg == false) {
            CustomWarning('Please link Organisation with the Primary Contact.')
        }
        else {
            CustomWarning('Primary Contact should be link with Opportunity');

        }
    }    /*tarun 12/10/2018*/
}
function Print() {

    var data = QuoteValidCheck();
    var OptionStatus = "Order";
    if ($('#PageName').val() == "QuoteDetails") {
        OptionStatus = "Opp";
    }
    //tarun 21/09/2018
    else if ($('#PageName').val() == "InvoicingDetails") {
        OptionStatus = "Invoice";
    }
    else if ($('#PageName').val() == "PackingDetails") {
        OptionStatus = "Packing";
    }
    else if ($('#PageName').val() == "JobDetails") {
        OptionStatus = "Confirm";
    }
    //end
    if (data.resultOrg) {
        if (data.OptionCount != "0") {
            if (OptionStatus == "Opp") {
                window.open('/MasterPdf/QuotesPdf/?id=' + $('#lblOpportunityId').text() + '&path=""&OptionStatus=' + OptionStatus + '&QuoteType=Print', '_blank');
            }
            else if (OptionStatus == "Order") {
                window.open('/MasterPdf/OrderPdf/?id=' + $('#lblOpportunityId').text() + '&path=""&OptionStatus=' + OptionStatus + '&QuoteType=Print', '_blank');
            }
            //tarun 21/09/2018
            else if (OptionStatus == "Invoice") {
                window.open('/MasterPdf/InvoicePdf/?id=' + $('#lblOpportunityId').text() + '&path=""&OptionStatus=' + OptionStatus + '&QuoteType=Print', '_blank');
            }
            //end
            //21 Sep 2018 (N)
            else if (OptionStatus == "Packing") {
                window.open('/MasterPdf/PackagingPdf/?id=' + $('#lblOpportunityId').text() + '&path=""&OptionStatus=' + OptionStatus + '&QuoteType=Print', '_blank');
            }
            //21 Sep 2018 (N)
            else if (OptionStatus == "Confirm") {
                window.open('/MasterPdf/ConfirmationPdf/?id=' + $('#lblOpportunityId').text() + '&path=""&OptionStatus=' + OptionStatus + '&QuoteType=Print', '_blank');
            }
            //23 Nov 2018 (N)

        }
        else {
            CustomWarning('Option data does not exist');
        }
    }
    else if (data.result == true && data.resultOrg == false) {
        CustomWarning('Please link Organisation with the Primary Contact.')
    }
    else {
        CustomWarning('Primary Contact should be link with Opportunity');

    }
}

function OpenProofPdf() {

    var OptionId = $('#HiddenOptionID').val();
    if (OptionId != "" && OptionId != null && OptionId != undefined) {
        var PdfType = "";

        if ($('#PageName').val() == "Opportunity" || $('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "OrderDetails") {
            PdfType = "Proof";
        }
        else if ($('#PageName').val() == "JobDetails") {
            PdfType = "JobSheet";
        }
        else {
            PdfType = "PackingList"
        }

        window.open('/MasterPdf/Proof_24HourMerchandise?OptionId=' + $('#HiddenOptionID').val() + '&PathPdf=""&QuoteType=Print&PdfType=' + PdfType + '', '_blank');
    }
    else {
        CustomWarning('Please select option first.');
    }
}

function SendEmail() {

    var checkflag = true;
    if ($("#txtToMail").val() != "") {
        //$("#txtToMail").css("border-color", "rgba(204, 204, 204, 1)");
        $("#txtToMail").removeClass('customAlertChange');
    } else {
        //$("#txtToMail").css("border-color", "red");
        $("#txtToMail").addClass('customAlertChange');
        checkflag = false;
    }
    if (checkflag) {
        var url = '/MasterPdf/SendEmail';

        var OptionStatus = "Order";
        if ($('#PageName').val() == "QuoteDetails") {
            OptionStatus = "Opp";
        }

        if ($('#HiddenforConfirm').val() == "Yes") {
            OptionStatus = "Confirm";
        }
        if ($('#HiddenforConfirm').val() == "Invoice") {
            OptionStatus = "Invoice";
        }
        if ($('#HiddenforConfirm').val() == "Proof") {
            OptionStatus = "Proof";
            url = '/MasterPdf/SendProofEmail';
        }
        //var dataMsg2 = $('#txtMailMessage2').val();
        //if (dataMsg2 != null) {
        //    dataMsg2.replace("\n", "<br/>");
        //}
        // baans change 16th November
        //var data = document.getElementById('txtMailMessage2').value.replace(/\n/g, "<br />");
        // baans end 16th November
        var model = {
            "model": { Email: $('#txtToMail').val(), Subject: $('#txtMailSubject').val(), MailMessage1: "", MailMessage2: $('#txtMailMessage2').val(), MailMessage3: "", OptionStatus: OptionStatus, Type: 'Email', ConfirmedDate: GetDate($("#txtConfirmedDate").val()), Shipping:$("#OppShipping").val() },
            "OpportunityId": $('#lblOpportunityId').text()
        };
        if ($('#HiddenforConfirm').val() == "Proof") {
            model = {
                "model": { Email: $('#txtToMail').val(), Subject: $('#txtMailSubject').val(), MailMessage1: "", MailMessage2: $('#txtMailMessage2').val(), MailMessage3: "", OptionStatus: OptionStatus, Type: 'Email', ConfirmedDate: GetDate($("#txtConfirmedDate").val()) },
                "OpportunityId": $('#lblOpportunityId').text(), "PageName": $('#PageName').val()
            };
        }

        //$('#EmailModel').css("display", "none");
        //$('#ajaxLoader').show();
        $.ajax({
            url: url,
            data: model,
            success: function (response) {
                if (response.Result == "Success") {
                    $("#EmailModel").css("display", "none");
                    // baans change 03rd November for changing the confirmed date and not changing the stage
                    if ($('#HiddenforConfirm').val() == "Yes") {
                        var OppConfirmedId = $('#lblOpportunityId').text();
                        $.ajax({
                            url: '/Opportunity/CheckStageByOppoIdForReconfirmation',
                            data: { OppId: OppConfirmedId },
                            async: false,
                            success: function (response) {
                                if (response == true) {
                                    $.ajax({
                                        url: '/Opportunity/ChangeStageByOppoID',
                                        data: { OppId: $('#lblOpportunityId').text(), Stage: 'Order Confirmed' },
                                        async: false,
                                        success: function (response) {
                                            var data = response.data;
                                            if (data.response.Result == "Success") {
                                                $('#lblOrderConfirmDate').text(DateTimeFormat(data.ChangeDate));
                                                SetOppoStage();
                                            }
                                            CustomAlert(data.response);
                                        }
                                    });

                                }
                                else {
                                    $.ajax({
                                        url: '/Opportunity/ChangeConfirmedDateOppoID',
                                        data: {
                                            OppId: $('#lblOpportunityId').text(), ConfirmedDate: GetDate($('#txtConfirmedDate').val())
                                        },
                                        async: false,
                                        success: function (response) {
                                            if (response.Result == "Success") {
                                                CustomAlert(response);
                                            }
                                        }
                                    });
                                }
                            }
                        });
                    }

                    // baans end 03rd November
                }
                GetOptionGrid($('#lblOpportunityId').text());
                CustomAlert(response);
            },
            error: function (response) {
                alert(response);
            },
            type: 'post',
            beforeSend: function (response) {

            },

        });
    }
    else {
        CustomWarning('Enter email address');
    }


}
function ValidateOpportunity() {
    var checkflag = true;
    if ($("#oppName").val() != "") {
        //$("#oppName").css("border-color", "rgba(204, 204, 204, 1)");
        $("#oppName").removeClass('customAlertChange');
    } else {
        //$("#oppName").css("border-color", "red"); 
        $("#oppName").addClass('customAlertChange');
        checkflag = false;
    }

    if ($("#oppQuantity").val() != "" && $("#oppQuantity").val() != "0" && parseInt($("#oppQuantity").val()) > 0) {
        //$("#oppQuantity").css("border-color", "rgba(204, 204, 204, 1)");
        $("#oppQuantity").removeClass('customAlertChange');
    } else {
        /*$("#oppQuantity").css("border-color", "red");*/ 
        $("#oppQuantity").addClass('customAlertChange');
        checkflag = false;
    }




    if ($("#datepicker").val() != "") {
        //$("#datepicker").css("border-color", "rgba(204, 204, 204, 1)");
        $("#datepicker").removeClass('customAlertChange');
    } else {
        //$("#datepicker").css("border-color", "red");
        $("#datepicker").addClass('customAlertChange');
        checkflag = false;
    }

    if ($("#oppSource").val() != "") {
        //$("#oppSource").css("border-color", "rgba(204, 204, 204, 1)");
        $("#oppSource").removeClass('customAlertChange');
    } else {
       /* $("#oppSource").css("border-color", "red"); */
       $("#oppSource").addClass('customAlertChange');
        checkflag = false;
    }
    if ($('#PageName').val() != "Opportunity") {
        if ($("#depositreqdate").val() != "") {
            //$("#depositreqdate").css("border-color", "rgba(204, 204, 204, 1)");
            $("#depositreqdate").removeClass('customAlertChange');
        } else {
            //$("#depositreqdate").css("border-color", "red");
            $("#depositreqdate").addClass('customAlertChange');
            checkflag = false;
        }
        if ($("#OppShipping").val() != "") {
            //$("#OppShipping").css("border-color", "rgba(204, 204, 204, 1)");
            $("#OppShipping").removeClass('customAlertChange');
        } else {
            /*$("#OppShipping").css("border-color", "red");*/ 
            $("#OppShipping").addClass('customAlertChange');
            checkflag = false;
        }
        if ($("#txtshippingday").val() != "") {
            //$("#txtshippingday").css("border-color", "rgba(204, 204, 204, 1)");
            $("#txtshippingday").removeClass('customAlertChange');
        } else {
            /*$("#txtshippingday").css("border-color", "red");*/ 
            $("#txtshippingday").addClass('customAlertChange');
            checkflag = false;
        }
        if ($("#txtshippingprice").val() != "") {
            //$("#txtshippingprice").css("border-color", "rgba(204, 204, 204, 1)");
            $("#txtshippingprice").removeClass('customAlertChange');
        }
        else {
            //$("#txtshippingprice").css("border-color", "red"); 
            $("#txtshippingprice").addClass('customAlertChange');
            checkflag = false;
        }
    }
    if ($("#ddlOppAcctMgr").val() != "") {
        //$("#ddlOppAcctMgr").css("border-color", "rgba(204, 204, 204, 1)");
        $("#ddlOppAcctMgr").removeClass('customAlertChange');
    } else {
        //$("#ddlOppAcctMgr").css("border-color", "red"); 
        $("#ddlOppAcctMgr").addClass('customAlertChange');
        checkflag = false;
    }

    if ($('#ms1').magicSuggest().getValue() != "") {
        //$('#ms1').css("border-color", "rgba(204, 204, 204, 1)");
        $('#ms1').removeClass('customAlertChange');
    } else {
        /*$('#ms1').css("border-color", "red");*/ 
        $('#ms1').addClass('customAlertChange');
        checkflag = false;
    }
    return checkflag;
}

function SetDate(InputID) {
    var RequiredbyDate = GetDate($('#datepicker').val());
    var DepReqDate = GetDate($('#depositreqdate').val());
    if (RequiredbyDate != "" && RequiredbyDate != undefined && RequiredbyDate != null && DepReqDate != "" && DepReqDate != undefined && DepReqDate != null) {
        var RequiredbyDatenew = new Date(RequiredbyDate);
        var DepReqDatenew = new Date(DepReqDate);
        if (RequiredbyDatenew <= DepReqDatenew) {
            $('#' + InputID).val('');
            if (InputID == "datepicker") {
                CustomWarning('Required By Date should be greater than Deposit Required Date');
            }
            else {
                CustomWarning('Deposit Required Date should be less than Required By Date');
            }
        }

    }
}
function OptionFormReset() {
    $('#txtOptionQty').val('');
    $('#txtCode').val('');
    $('#txtColor').val('');
    $('#txtLink').val('');
    $('#txtCost').val('');

    //$('#txtMargin').val('');
    $('#txtMargin').val("35");
    $('#ddlBrand').val('').trigger("chosen:updated");

    $('#ddlItem').val('').trigger("chosen:updated");
    $('#txtotherdesc').val('');
    $('#txtunitprcExgst').val('');
    $('#ddlinclude').val('');
    if ($('#PageName').val() == "PurchaseDetails") {
        $('#txtShipping').val('');
        $('#txtPrice').val('');
        $('#ItemNotes').val('');
    }
    $('#txtothercost').val('');
    $('#txtComment').val('');
    //$('#ddlservice').val('');
    $('#ddlservice').val("Standard - 2 weeks*");
    //baans change 11th January
    $('#ddlSizeType').val("Custom");
    //$("#ddlSizeType").val() = "Custom";
    //$('#txtSizes').val('');
    $("#txtSizes").val("TBC");
    //baans end 11th January
    $('#HiddenOptionID').val('');
    $('#txtDecorationFront').val('');
    $('#txtRangeFront').val('');
    $('#hiddenRangeFrontCost').val('');
    $('#txtDecorationBack').val('');
    $('#txtRangeBack').val('');
    $('#hiddenRangeBackCost').val('');
    $('#txtDecorationRight').val('');
    $('#txtRangeRight').val('');
    $('#hiddenRangeRightCost').val('');
    $('#txtDecorationLeft').val('');
    $('#txtRangeLeft').val('');
    $('#hiddenRangeLeftCost').val('');
    $('#txtDecorationOther').val('');
    $('#txtRangeOther').val('');
    $('#hiddenRangeOtherCost').val('');
    $("#FrontShortlbl").text("...");
    $("#FrontLonglbl").text("Click On Search");
    $("#FrontDecorationID").val("");
    $("#BackShortlbl").text("...");
    $("#BackLonglbl").text("Click On Search");
    $("#BackDecorationID").val("");
    $("#LeftShortlbl").text("...");
    $("#LeftLonglbl").text("Click On Search");
    $("#LeftDecorationID").val("");
    $("#RightShortlbl").text("...");
    $("#RightLonglbl").text("Click On Search");
    $("#RightDecorationID").val("");
    $("#OtherShortlbl").text("...");
    $("#OtherLonglbl").text("Click On Search");
    $("#OtherDecorationID").val("");
    $('#chkProofSelect').prop('checked', false);
    $('#mailIcon').css('visibility', 'hidden');
}

function PhotoreadURL(input) {
    var checkFlag = true;
    if ($('#lblOpportunityId').text() == "000000") {
        CustomWarning("Please Save Opportunity First");
        checkFlag = false;
    }
    var hdnimg = input.id;
    var base64data;

    if (checkFlag == true) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                var image = e.target.result;
                var OppId = $('#lblOpportunityId').text();
                var filename = hdnimg + "_" + OppId + "." + input.files[0].name.split('.')[1];
                //if ($("#" + hdnimg + "hdnimg").val() != null && $("#" + hdnimg + "hdnimg").val() != "") {
                //    filename = $("#" + hdnimg + "hdnimg").val();
                //}
                $.ajax({

                    url: '/Opportunity/UploadOppThumbnail',
                    data: { imageData: image, filename: filename, OppId: OppId },
                    async: false,
                    success: function (response) {
                        CustomAlert(response);
                        if (response.Result == "Success") {
                            $('#imageOppThumbnail').attr('src', '/Content/uploads/Opportunity/' + response.FileName)
                        }

                    },
                    type: 'post',

                });
            }

            reader.readAsDataURL(input.files[0]);
        }
    }

}

function SaveOppThumbnail() {
    $('#OppThumbnail').click();
}

function GetyyyymmddDate(date) {
    //return date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
    var GetDate = date.split('-');

    return GetDate[2] + "-" + GetDate[1] + "-" + GetDate[0];
}

function GetFormattedDate(date) {
    //Get Current date Without Time
    return date.getFullYear() + '-' + ('00' + (date.getMonth() + 1)).substr(-2) + '-' + ('00' + date.getDate()).substr(-2);    //tarun 22/09/2018
}

function GetddmmyyyyDate(date) {
    //For Setting Date In Datepicker(Entered date Will be yy-mm-dd)
    var GetDate = date.split('-');
    return GetDate[2] + "-" + GetDate[1] + "-" + GetDate[0];
}

function CheckDate(InputID) {
    var DepReqDate = GetyyyymmddDate($('#fromdate').val());     //10 Sep 2018 (N)
    var RequiredbyDate = GetyyyymmddDate($('#todate').val());       //10 Sep 2018 (N)
    if (RequiredbyDate != "" && RequiredbyDate != undefined && RequiredbyDate != null && DepReqDate != "" && DepReqDate != undefined && DepReqDate != null) {
        var RequiredbyDatenew = new Date(RequiredbyDate);
        var DepReqDatenew = new Date(DepReqDate);
        // BAANS CHANGE 3rd December
        var ReqDateHours = RequiredbyDatenew.getHours();
        var DepDateHours = DepReqDatenew.getHours();
        if (ReqDateHours != 0) {
            var tzOffset = RequiredbyDatenew.getTimezoneOffset();
            RequiredbyDatenew.setMinutes(RequiredbyDatenew.getMinutes() + tzOffset);
        }
        if (DepDateHours != 0) {
            var tzOffset = DepReqDatenew.getTimezoneOffset();
            DepReqDatenew.setMinutes(DepReqDatenew.getMinutes() + tzOffset);
        }

        // baans end 
        // baans change 21st November for checking the current date data
        if (RequiredbyDatenew < DepReqDatenew) {
            $('#' + InputID).val('');
            if (InputID == "todate") {
                CustomWarning('End Date should be greater than Start Date');
            }
            else {
                CustomWarning('Start Date should be less than End Date');
            }
        }
        // baans end 21st November

    }
}

//$(function () {
//    $(".tablinks").click(function () {
//        GetAllOpportunityList($(this).attr('id'));
//    });
//    $("#btnFindOpp").click(function () {
//        $(".tablinks").each(function () {
//            if ($(this).hasClass('active')) {
//                GetAllOpportunityList($(this).attr('id'));
//            }
//        });
//    });
//});

function setOptionTotal() {
    var totalvalue = $('#HiddenPaymentTotal').val();
    var totaldue = $("#lblbalance").text().split('$');
    
    var Paidvalue = $("#lblpaidbalance").text().split('$');
    if ($('#txtshippingprice').val() != "") {
        // Nikhil change 21st November
        var ShippingGST = parseFloat($('#txtshippingprice').val()) * parseFloat($('#lblGSTbalance').text());
        var Totalwithshipping = parseFloat(ShippingGST) + parseFloat(totalvalue);
        // Nikhil End
        $("#lbltotalbalance").text('$' + Totalwithshipping.toFixed(2));
        var Paidvalue = Totalwithshipping - parseFloat(totaldue[totaldue.length - 1]);
        $("#lblpaidbalance").text('$' + Paidvalue.toFixed(2));
        
    }
    else {
        var Totalwithshipping = 0 + parseFloat(totalvalue);
        $("#lbltotalbalance").text('$' + Totalwithshipping);
        var Paidvalue = Totalwithshipping - parseFloat(totaldue[totaldue.length - 1]);
        $("#lblpaidbalance").text('$' + Paidvalue.toFixed(2));
    }

}

//8-Aug-2018 (N)
function ShowHistory(OppID) {
    $('#historypopup').css("display", "block");
   // $('#PaymentModal').css("display", "none");
    var id = OppID;
    GetPaymentHistory(id);
}


function hidepopup() {
    $('#historypopup').css("display", "none");
}

function GetPaymentHistory(id) {
    $.ajax({
        url: '/Opportunity/GetPaymentHistory/',
        data: { id: id },
        async: false,

        success: function (response) {
            data = response;

            var name;
            if (data.length > 0) {
                var id = data[0].OpportunityId;
                for (i = 0; i < data.length; i++) {
                    if (data[i].OppName != "") {
                        name = data[i].OppName;
                    }
                }
            }

            $("#jobid").text(id);
            //$("#jobname").text($('#oppName').val());
            $("#jobname").text(name);



        },
        //error:function(response){
        //    alert(response);
        //},
        type: 'get',
    });

    //27 Aug 2018 (N)
    var totalData;
    function calculatesummary() {
        var ChargeTotal = 0;
        var PaymentsTotal = 0;
        for (var i = 0; i < data.length; i++) {
            var row = data[i];
            ChargeTotal += parseFloat(row["price"]);
            PaymentsTotal += parseFloat(row["payments"]);

        }
        //ChargeTotal = $.paramquery.formatCurrency(ChargeTotal);
        //PaymentsTotal = $.paramquery.formatCurrency(PaymentsTotal);

        //totalData = ["<b>Total</b>", "", "", "", ChargeTotal, PaymentsTotal];
        totalData = { Src: "<b>Total</b>", oppnewdate: null, DepositActNo: "", memodesc: "", price: ChargeTotal, payments: PaymentsTotal, pq_rowcls: 'green' };
        return [totalData];

        //averageData = { rank: "<b>Average</b>", company: "", revenues: revenueAverage, profits: profitAverage, pq_rowcls: 'yellow' };
    }
    //27 Aug 2018 (N)


    var obj = {
        scrollModel: { horizontal: false },
        //selectionModel: { type: 'row' },
        hwrap: false,
        minWidth: 400,
        resizable: true,
        summaryData: calculatesummary(),                // 27 Aug 2018(N)
    };

    obj.width = "100%";
    obj.height = 280;
    obj.numberCell = { resizable: true, width: 0, title: "", minWidth: 0 };
    obj.columnTemplate = { width: 150 };
    obj.colModel = [

            //{ title: "ID", dataIndx: "OpportunityId", width: "10%", dataType: "integer", editable: false },

        { title: "Src", dataIndx: "Src", width: "14%", dataType: "string", editable: false },

            {
                title: "Date", dataIndx: "oppnewdate", width: "13%", dataType: "string", editable: false,
                render: function (ui) {
                    var date = ui.cellData;
                    if (date != null && date != undefined) {
                        //var nowDateopp = new Date(parseInt(date.substr(6)));
                        //return ('00' + nowDateopp.getDate()).substr(-2) + '/' + (nowDateopp.getMonth() + 1) + '/' + nowDateopp.getFullYear();
                        // baans change 22nd November
                        return DateFormat(date);
                       // baans end 22nd November
                    }
                }
            },

            { title: "Acct", dataIndx: "DepositActNo", width: "12%", dataType: "string", editable: false, align: "right" },

            { title: "Memo", dataIndx: "memodesc", width: "34%", dataType: "string", editable: false },
        {
            title: "Charges", dataIndx: "price", width: "13%", dataType: "string", editable: false, format: "$#,###.00", align: "right",
            //27 Aug 2018 (N)
            render: function (ui) {
                var amount = ui.cellData;
                if (amount == 0) {
                    return ('');
                }
            }
            //27 Aug 2018 (N)
        },

        {
            title: "Payments", dataIndx: "payments", width: "13%", dataType: "string", editable: false, format: "$#,###.00", align: "right",
            //27 Aug 2018 (N)
            render: function (ui) {
                var amount = ui.cellData;
                if (amount == 0) {
                    return ('');
                }
            }
            //27 Aug 2018 (N)
        },
            //{ title: "Job", dataIndx: "OppName", width: "15%", dataType: "string", editable: false },
    ];

    obj.dataModel = { data: data };
    pq.grid("#paymenthistorygrid", obj);
    pq.grid("#paymenthistorygrid", "refreshDataAndView");
}
//8-Aug-2018 (N)

// baans change 07 august for email on order page
function openOrderEmailModal() {
    $('#HiddenforConfirm').val('Yes');
    var data = QuoteValidCheck();
    if (data.resultOrg) {
        if (data.OptionCount != "0") {
            $.ajax({
                url: '/MasterPdf/GetMailMessage',
                data: { OpportunityId: $('#lblOpportunityId').text(), OptionStatus: 'Confirm' },
                async: false,
                success: function (response) {
                    if (response != null && response != undefined && response != "") {
                        //$('#txtMailMessage1').html(br2nl(response.Body1));
                        //$('#txtMailMessage3').val(br2nl(response.Body2));
                        $('#txtMailMessage2').val(br2nl(response.Body1) + "\n \n" + br2nl(response.Body2));
                        $('#txtMailSubject').val(br2nl(response.Subject)); // + $('#oppName').val() 25 Oct 2018(N)
                        $('#txtToMail').val(response.ClientEmailID);
                    }
                },
                type: 'post',

            });

            $("#EmailModel").css("display", "block");
        }
        else {
            CustomWarning('Option data does not exist');
        }
    }
    else if (data.result == true && data.resultOrg == false) {
        CustomWarning('Please link Organisation with the Primary Contact.')
    }
    else {
        CustomWarning('Primary Contact should be link with Opportunity');

    }
    //}
    //else {
    //    CustomWarning('Confirm Date should not be empty !!!');
    //}
}
// baans end 07 august

//baans change by prashant 14 aug start
function getBalance(oppId) {
    var Optionstage = "Order";
    if ($('#PageName').val() == "Opportunity" || $('#PageName').val() == "QuoteDetails")
    {
        Optionstage = "Opp";
    }

    $.ajax({
        url: '/Opportunity/getBalance',
        data: { OppId: oppId, Optionstage },
        async: false,
        success: function (response) {
            var balance = "$" + response;
            $('#lblPayBalance').text(balance);
            $('#lblbalance').text(balance);
           setOptionTotal();
            $('#hdnAmountCheckValidation').val(response);
        },
        error: function (response) {

        },
        type: 'post',
    });
}

function OpenPayModal() {
    

            $('#txtAmountReceived').val('');
            $('#txtPaymentMethod').val('');
            var OrgId = $('#hdnforoppOrgID').val();
            if (OrgId != "") {

                $.ajax({
                    url: "/Opportunity/OrgData",
                    data: { OrgId: OrgId },
                    async: false,
                    success: function (response) {
                        data = response;
                        if (data == true) {
                            // Baans change 7th December checking the Opp OptionData
                            var data = QuoteValidCheck();
                            if (data.resultOrg) {
                                if (data.OptionCount != 0) {
                                    $('#PaymentModal').css("display", "block");
                                    var OpportunityId = $('#lblOpportunityId').text();
                                    $('#txtID').val(OpportunityId);
                                    getBalance(OpportunityId);
                                    GetOrganisationId();
                                    GetCommonDataOnPaymentDDL();
                                    var todayDate = new Date();
                                    var newdate = ("0" + todayDate.getDate()).slice(-2) + '/' + ("0" + (todayDate.getMonth() + 1)).slice(-2) + '/' + todayDate.getFullYear()
                                    $('#CurrentDate').val(newdate);
                                    //var OrgId = $('#hdnforoppOrgID').val();
                                    GetPaymentGrid(OrgId);
                                }
                                else {
                                    CustomWarning("Option Data does not exist so you cannot make payment against this job.");
                                }
                            }
                            // Baans end 7th December
                        }
                        else {
                            CustomWarning("Please assign the valid organisation before proceeding to the payment.");
                        }
                    }
                });

            }
            else {
                CustomWarning("Organisation has not been linked to the Opportunity . Please go QuoteDetails tab and specify Organisation before making Payment");
            }
      
    
}

function HidePaymentModal() {
    $("#txtFieldValue").val('');
    $('#PaymentModal').css("display", "none");
}
function GetOrganisationId() {

    var OrgId = $('#hdnforoppOrgID').val();
    data = { OrgId: OrgId };
    $.ajax({
        url: '/Organisation/GetOrganisationById',
        data: data,
        async: false,
        success: function (response) {
            var OppName = $('#hdnOppName').val();
            $('#txtCustomerName').val(response.OrgName);

            var OrgNamewithPayment = "Payment :" + OppName + "_" + response.OrgName;
            $('#txtMemo').val(OrgNamewithPayment);
        }
    });
}

function GetCommonDataOnPaymentDDL() {
    $.ajax({
        url: '/Opportunity/GetCommandDataForPaymentDescription',
        data: {},
        async: false,
        success: function (response) {
            $('#lblDeposite').text(response.FieldName);
            //$('#txtFieldValue').val(response.FieldValue);
            $('#lblFieldDescription').text(response.FieldDescription);
        }

    });
}

function ValidationPayment() {
    var CheckFlag = true;
   
    return CheckFlag;
}

function submitPayment() {

    var CheckFlag = true;
    var IsMax = false;
    if ($("#txtAmountReceived").val() != "") {
        //$("#txtAmountReceived").css("border-color", "rgba(204,204,204,1)");
        $("#txtAmountReceived").removeClass('customAlertChange');
    }
    else {
        //$("#txtAmountReceived").css("border-color", "red"); 
        $("#txtAmountReceived").addClass('customAlertChange'); 

        CheckFlag = false;
    }
    if ($("#CurrentDate").val() != "") {
        //$("#CurrentDate").css("border-color", "rgba(204,204,204,1)");
        $("#CurrentDate").removeClass('customAlertChange');
    }
    else {
        //$("#CurrentDate").css("border-color", "red"); 
        $("#CurrentDate").addClass('customAlertChange');
        CheckFlag = false;
    }
    if ($("#txtPaymentMethod").val() != "") {
        //$("#txtPaymentMethod").css("border-color", "rgba(204,204,204,1)");
        $("#txtPaymentMethod").removeClass('customAlertChange');
    }
    else {
        //$("#txtPaymentMethod").css("border-color", "red"); 
        $("#txtPaymentMethod").addClass('customAlertChange');
        CheckFlag = false;
    }


    if ($("#txtMemo").val() != "") {
        //$("#txtMemo").css("border-color", "rgba(204,204,204,1)");
        $("#txtMemo").removeClass('customAlertChange');
    }
    else {
        //$("#txtMemo").css("border-color", "red"); 
        $("#txtMemo").addClass('customAlertChange');
        CheckFlag = false;
    }

    var CurrentFillAmount = parseInt($('#txtAmountReceived').val());
    
    if ($("#txtAmountReceived").val() != "") {
        var CurrentFillAmount = parseInt($('#txtAmountReceived').val());
        var TotalAmtCheck = parseInt($('#hdnAmountCheckValidation').val());
        if (CurrentFillAmount > TotalAmtCheck) {
            IsMax = true;
            CheckFlag = false;
            CustomWarning("Enter valid amount");
            //$("#txtAmountReceived").css("border-color", "red");
            $("#txtAmountReceived").addClass('customAlertChange');
        }
        else {

            $("#txtAmountReceived").css("border-color", "gray");
        }
    }
    else {
        //$("#txtAmountReceived").css("border-color", "red");
        $("#txtAmountReceived").addClass('customAlertChange');
        CheckFlag = false;
    }
    
    if (CheckFlag == true) {
        var OrgId = $('#hdnforoppOrgID').val();
        var DepositAccountNo = $('#txtFieldValue').val()
        var CustomerName = $('#txtCustomerName').val();
        var OpportunityId = $('#txtID').val();
        var AmtReceived = $('#txtAmountReceived').val();
       var CurrentDate = GetDate($('#CurrentDate').val()); 
        var PaymentMethod = $('#txtPaymentMethod').val();
        var Memo = $('#txtMemo').val();
        var pmtId = 0;
        var data = { OrgId: OrgId, OrgName: CustomerName, OpportunityId: OpportunityId, AmtReceived: AmtReceived, PmtDate: CurrentDate, PmtMethod: PaymentMethod, MemoDesc: Memo, PmtId: pmtId, DepositAccountNo: DepositAccountNo };
        $.ajax({
            url: '/Opportunity/PaymentDetails',
            data: data,
            async: true,
            success: function (response) {
                data = response;
                $('#txtAmountReceived').val('');
                $('#txtPaymentMethod').val('');
                var OrgId = $('#hdnforoppOrgID').val();
                GetPaymentGrid(OrgId);
                var OpportunityId = $('#lblOpportunityId').text();
                getBalance(OpportunityId);
                CustomAlert(response);
                // Baans change 12th November
                $("#txtFieldValue").val('');
                $('#PaymentModal').css("display", "none");
                // Baans end 12th November
            },
            error: function (response) {
                data = response;
            },
            type: 'post',
        });

    }
    else {
        if (!IsMax) {
            //CustomWarning("Fill all required fields ");
            CustomErrorCode("Required");
        }
            $('#PaymentModal').css("display", "block");
    }

}

function GetPaymentGrid(OrgId) {

    var NewOrgId = parseInt(OrgId);

    $.ajax({
        url: "/Opportunity/GetPaymentList",
        data: { OrgId: NewOrgId },
        async: false,
        success: function (response) {
            data = response;
        },
        error: function (response) {
            data = response;
            CustomWarning('Negative values not allowed');
        },
        type: 'post',
    });


    var colModel = [

        {
            title: "Invoice #", dataIndx: "DisplayOpportunityId", align: "center", width: "15%", dataType: "string",

        },
           {
               title: "Status", dataIndx: "Stage", align: "center", width: "18%", dataType: "string",
               //render: function(ui)
               //{
               //    var stage = ui.cellData;
               //    if(stage!=null && stage!= undefined)
               //    {
               //        return "=>" + stage;
               //    }
               //}

           },
        {
            title: "Date", dataIndx: "PmtDate", width: "13%", align: "right", datatype: "date",
            render: function (ui) {
                var date = ui.cellData;
                if (date != null && date != undefined) {
                    //var nowDateopp = new Date(parseInt(date.substr(6)));
                    //return ('00' + nowDateopp.getDate()).substr(-2) + '/' + (nowDateopp.getMonth() + 1) + '/' + nowDateopp.getFullYear();
                    // baans change 22nd November
                    return DateFormat(date);
                   // baans end 22nd November
                }
            },
        },

        { title: "Amount", dataIndx: "OrderTotal", width: "14%", dataType: "integer", format: "$#,###.00", align: "right" },
        { title: "Discount", dataIndx: "Quantity", width: "5%", dataType: "integer", hidden: true },
         { title: "Total Due", dataIndx: "Balance", width: "14%", dataType: "integer", format: "$#,###.00", align: "right" },
          { title: "Amount Applied", dataIndx: "AmtReceived", width: "14%", dataType: "integer", format: "$#,###.00", align: "right" },
          {
              title: "", dataIndx: "", width: "12%", editable: false, align: "center",
              render: function (ui) {
                  var invoiceId = ui.rowData.OpportunityId;
                  return "<a   onclick= 'ShowHistory(" + invoiceId + ")'><span style='color:blue;border-bottom:1px solid blue;cursor:pointer'>History</span></a>";
              }
          },

    ];
    //23 Aug 2018 (N)
    var index = data.findIndex(x => x.DisplayOpportunityId == $('#txtID').val());
    data[index].pq_rowattr = { style: "background:#cccccc;" };
    //23 Aug 2018 (N)
    var obj = {
        dataModel: { data: data },
        colModel: colModel,
        selectionModel: { type: 'row' },
        scrollModel: { pace: 'consistent', horizontal: false },
       // pageModel: { type: "local", rPP: 50 },
        strNoRows: 'No records found for the selected Criteria',
        editable: false,
        wrap: false,
        hwrap: false,
        width: "98%",
        height: 297,
        columnTemplate: { width: 150, halign: "left" },
        numberCell: { width: 0, title: "", minWidth: 0 },
        sortable: true,
        numberCell: { width: 0, title: "", minWidth: 0 },
        //sortModel: { cancel: false, type: 'local', sorter: [{ dataIndx: 1, dir: ['up', 'down', 'up'] }] }
    };



    pq.grid("#PaymentGrid", obj);
    pq.grid("#PaymentGrid", "refreshDataAndView");


}
// baans change by prashant 14  aug end

//17 Aug 2018 (N)
function OpenOpportunity() {
    if ($('#lblOpportunityId').text() != "" && $('#lblOpportunityId').text() != "000000") {
        location.href = "/Opportunity/OpportunityDetails/" + $('#lblOpportunityId').text();
    }
}

function OppFormReset() {
    $('#lblOpportunityId').text('000000');
    $('#oppName').val('');
    $('#oppQuantity').val('');
    $('#datepicker').val('');
    $('#depositreqdate').val('');
    $('#OppShipping').val('');
    $('#txtshippingday').val('');
    $('#txtshippingprice').val('');
    $('#oppSource').val('');
    $('#oppCampaign').val('');
    $('#txtConfirmedDate').val('');
    $('#oppStage').val('');
    $('#ddlOppAcctMgr').val('');
    $('#ddlDecline').val('');
    $('#oppNotes').val('');
    $('#ddlDecline').val('');
    $('#txtCancelled').val('');
    $('#txtlost').val('');
    $('#txtrepeatfrom').val('');

    ms.clear();
    ms.enable();

    $('#oppName').removeAttr('readonly').removeClass('MakeReadonly');
    $('#oppQuantity').removeAttr('readonly').removeClass('MakeReadonly');
    $('#datepicker').removeAttr('disabled').removeClass('MakeReadonly');
    $('#depositreqdate').removeAttr('disabled').removeClass('MakeReadonly');
    $('#OppShipping').removeAttr('disabled').removeClass('MakeReadonly');
    $('#txtshippingday').removeAttr('readonly').removeClass('MakeReadonly');
    $('#txtshippingprice').removeAttr('readonly').removeClass('MakeReadonly');
    $('#oppSource').removeAttr('disabled').removeClass('MakeReadonly');
    $('#oppCampaign').removeAttr('disabled').removeClass('MakeReadonly');
    //$('#txtConfirmedDate').removeAttr('disabled').removeClass('MakeReadonly');
    $('#oppStage').removeAttr('disabled').removeClass('MakeReadonly');
    $('#ddlOppAcctMgr').removeAttr('disabled').removeClass('MakeReadonly');
    $('#oppNotes').removeAttr('readonly').removeClass('MakeReadonly');
    $('#ms1').removeAttr('readonly').removeClass('MakeReadonly');
    $('#ddlDecline').removeAttr('disabled').removeClass('MakeReadonly');
    $('#txtCancelled').removeAttr('disabled').removeClass('MakeReadonly');
    $('#txtlost').removeAttr('disabled').removeClass('MakeReadonly');
    $('#txtrepeatfrom').removeAttr('readonly').removeClass('MakeReadonly');

    if ($('#PageName').val() == "Event") {
        $('#StatCampaign').css("display", "block");
        $('#DivQuote').css("display", "none");
        $('#lblRepeat').css("display", "none");
        $('#lblCampaign').css("display", "block");
    }
    ResetForm();

    //11 Sep 2018 (N)
    $('#btnPre').css("display", "none");
    $('#btnNext').css("display", "none");
    //11 Sep 2018 (N)
}
//17 Aug 2018 (N)

//31 July 2019 (N) Application Popup/Modal
function openDecorationModal(currId) {
    $('#hdnOptionid').val($('#HiddenOptionID').val());
    $('#hdnLocationName').val(currId);
    $('#AppJobName').text($('#oppName').val());
    $('#AppLocation').text(currId + ' Dec');

    $("#DecorationModel").css("display", "block");
    GetDecorationImageGrid("All");
}

function GetDecorationImageGrid(keyword) {
    //console.log(keyword);
    var LocationName = $('#hdnLocationName').val();

    $.ajax({
        url: '/Opportunity/GetDecorationImagesList',
        data: { keyword: keyword },
        async: false,

        success: function (response) {
            data = response;
            // alert(JSON.stringify(data));
        },
        error: function (response) {
            data = response;
        },
        type: 'get',
    });
    var obj = {
        selectionModel: { type: 'row', fireSelectChange: true },
        numberCell: { show: false },
        resizable: true,
        editable: false,
        wrap: false,
        hwrap: false,
        pageModel: { type: "local", rPP: 17 },
        strNoRows: 'No records found for the selected Criteria',
        width: "95%",
        height: 480,
        columnTemplate: { width: 150, halign: "left" }
    };

    obj.colModel = [
        { title: "", width: "0", dataIndx: "AppImage", hidden: true },
        { title: "Number", dataIndx: "ApplicationId", width: "15%", dataType: "int" },
        {
            title: "Date", dataIndx: "DecorationDate", width: "20%", dataType: "date",
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
        { title: "Name", dataIndx: "AppName", width: "37%", dataType: "string" },
        { title: "Type", dataIndx: "AppType", width: "28%", dataType: "string" },
         //{ title: "  Acct Manager", dataIndx: "", width: "24%", dataType: "string" },
    ];

    obj.dataModel = { data: data };

    obj.selectChange = function (evt, ui) {
        var rowIndx = getRowIndx(this);
        if (rowIndx != null) {
            //console.log(rowIndx);
            var data = this.pdata[rowIndx];
            var dataImage = data.AppImage;
            var dataName = data.AppName;
            var hostname = window.location.origin;

            $("#decImage").attr("src", hostname + "/Content/uploads/Application/" + dataImage);
            $("#btnSelectApp").click(function () {
                //console.log(rowIndx);
                //console.log(dataName);
                var shortName = "";
                if (dataName.length > 12) {
                    shortName = dataName.substr(0, 12);
                    $("#" + LocationName + "Shortlbl").text(shortName);
                }
                else {
                    $("#" + LocationName + "Shortlbl").text(dataName);
                }

                $("#" + LocationName + "Longlbl").text(dataName);
                $("#" + LocationName + "DecorationID").val(data.ApplicationId);
                $("#DecorationModel").css("display", "none");
            });
        }
    }
    // $("#gridContainer").pqGrid("setSelection", null);
    pq.grid("#DecImageGrid", obj);
    pq.grid("#DecImageGrid", "refreshDataAndView");
}

function MoveToApplication(Location) {
    if ($('#HiddenOptionID').val() != '' && $('#HiddenOptionID').val() != undefined && $('#HiddenOptionID').val() != 0) {

        if ($('#' + Location + 'DecorationID').val() != "" && $('#' + Location + 'DecorationID').val() != undefined) {

            window.open('/Application/ApplicationDetails?Id=' + $('#' + Location + 'DecorationID').val() + '&PageType=Default&OptionId=' + $('#HiddenOptionID').val(), '_blank');
        }
    }
    else {
        CustomWarning('Please save or select the option first');
    }
}

function MoveToNewApplication(Type) {
    if(Type == "Standard"){
        window.open('/Application/ApplicationDetails?Id=0&PageType=Default&OptionId=' + $('#hdnOptionid').val(), '_blank');
    }
    else {
        window.open('/Application/ApplicationDetails?Id=0&PageType=Custom&OptionId=' + $('#hdnOptionid').val(), '_blank');
    }
}
//31 July 2019 (N) Application Popup/Modal
function ClickOnCrossBtn(Location) {
    //$("#" + Location + "Shortlbl").text('......');
    var OptionId=parseInt($('#HiddenOptionID').val());
    if(isNaN(OptionId)){
        OptionId=0;
    }

    if (OptionId != 0) {

        $.ajax({
            url: '/Opportunity/UpdateDecoration',
            async: false,
            data: { id: OptionId, Location, Location },
            success: function (response) {
                if (response == "Success") {
                    $("#" + Location + "Shortlbl").text('......');
                }
            }
        });
    }
    else {
        CustomWarning('Please save or select the option first');
    }
}