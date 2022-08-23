var AutoCompleteFlag = "";
//var inactiveManager = [{ id: 17, AccountManagerFullName :"dheeraj"}]
var inactiveManager = [];
$(document).ajaxStart(function () {
    $("#ajaxLoader").css("display", "block");
}).ajaxStop(function () {
    $("#ajaxLoader").css("display", "none");
});
$(function () {
    //baans comment
    var PageName = "";
    PageName = $("#PageName").val();
    if (PageName != "" && PageName != undefined) {
        if (PageName == "kanban") {
            $("#commonTabs").css("visibility", "hidden");
            $("#kanbanTabs").css("visibility", "visible");
        } else {
            $("#commonTabs").css("visibility", "visible");
            $("#kanbanTabs").css("visibility", "hidden");
        }

        $("." + PageName).css("display", "block");
        if (PageName.indexOf('List') > -1) {
            //  $('#lblSystemsearch').css("display", "block");
            $('#searchtextbox').css("display", "block");
            $('#imgsearchicon').css("display", "block");
        }
        if (PageName == "Opportunity") {//added by baans 04Sep2020
            $("#BlankInnerDiv").css("display", "block");
        }
    }
    // Nikhil Change 22nd November 

    // baans change 11th December 
    if ($("#PageName").val() == "PaymentReport" || $("#PageName").val() == "SalesReport" || $("#PageName").val() == "ValueConversionReport" || $("#PageName").val() == "OpportunityValueConversionReport" || $("#PageName").val() == "ManagerStageWiseReport"||$("#PageName").val() == "InvoicedReport") {
        $("#commonTabs").css("visibility", "hidden")
    }
    // baans end 11th December

    $("#fromdate").datepicker(
        {
            firstDay: 1,
            dateFormat: 'dd-mm-yy',
            onSelect: function (dateText) {
                CheckDate('fromdate');
            }
        }
    );
    $("#todate").datepicker(
        {
            firstDay: 1,
            dateFormat: 'dd-mm-yy',
            onSelect: function (dateText) {

                CheckDate('todate');
            }
        }
    );

    // Nikhil end 22nd November


    //tarun 04/09/2018
    //if ($('#PageName').val() != "PurchaseDetails") {
    //    //$('#ddlOrgAcctMgr').parent().removeClass("col-lg-5");
    //    //$('#ddlOrgAcctMgr').parent().addClass("col-lg-12");

    //}
    if ($('#PageName').val() == "PurchaseDetails") {
        //$('#ddlOrgAcctMgr').addClass("txtOpprtDetails oppInnerleftCol");
        $("#ddlOrgAcctMgr").css("display", "none");     //21 June 2019 (N)
        $("#lblOrgAcctmanager").css("display", "none");
    }
    //end

    $(".QtyFormat").keydown(function (e) {
        // Allow: backspace, delete, tab, escape and enter
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13]) !== -1 ||
            // Allow: Ctrl+A
            (e.keyCode == 65 && e.ctrlKey === true) ||
            // Allow: home, end, left, right
            (e.keyCode >= 35 && e.keyCode <= 39)) {
            // let it happen, don't do anything
            return;
        }

        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }

    });

    $(".CurrencyFormat").blur(function (e) {
        var newtext = $(this).val();
        if (newtext != "" && newtext != undefined && newtext!=null)
            $(this).val(parseFloat(newtext).toFixed(2));
    });
    $(".CurrencyFormat").keydown(function (e) {
        // Allow: backspace, delete, tab, escape and enter
        var newtext = $(this).val();
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
            // Allow: Ctrl+A
            (e.keyCode == 65 && e.ctrlKey === true) ||
            // Allow: home, end, left, right
            (e.keyCode >= 35 && e.keyCode <= 39)) {
            // let it happen, don't do anything
            var count = 0;
            if (e.keyCode == 110 || e.keyCode == 190) {
                for (var i = 0; i < newtext.length; i++) {
                    if (newtext[i] === '.') {
                        count++;
                    }
                }
                if (count > 0) {
                    e.preventDefault();
                }
            }
            return;
        }
        var len = newtext.length;
        var index = newtext.indexOf('.');
        if (index >= 0) {
            var CharAfterdot = (len + 1) - index;
            if (CharAfterdot > 4) {
                e.preventDefault();
            }
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }


    });
    $.ajax({
        url: '/Opportunity/GetDeletedManager',
        data: {},
        async: false,
        success: function (response) {
            inactiveManager = [];
            inactiveManager = response;
        },
        type: 'get',
    });
    if (PageName != "Login") {
        setInterval(function () {
            if ($('#searchtextbox').val() != "") {
                PageSearch();
            }
        }, 300);
    }
    // baans change 5th October
    //var elem = document.getElementById('searchtextbox');
    //elem.onkeyup = function(e) {
    //    if (e.keyCode == 13) {
    //        MasterSearch();
    //    }
    //}
    // baans end 5th October
});

$(function () {
    $.ui.autocomplete.prototype._renderMenu = function (ul, items) {
        //P 10 Jan OptionCode
        if (AutoCompleteFlag == "OptionCode") {
            var self = this;
            console.log(items);
            $.each(items, function (index, item) {
                self._renderItem(ul, item);
                if (index == 0) ul.prepend('<li style="font-weight:bold" class="header-auto" ><table><thead><tr><th style="width:150px">Code</th><th style="width:150px">Cost</th><th style="width:150px">Item Name</th><th style="width:150px">Brand Name</th><th style="width:150px">Link</th></tr></thead><tbody></tbody></table></li>');
            });
        }
        //P 10 Jan OptionCode

        if (AutoCompleteFlag == "Contact") {
            var self = this;
            $.each(items, function (index, item) {
                self._renderItem(ul, item);
                if (index == 0) ul.prepend('<li style="font-weight:bold" class="header-auto" ><table><thead><tr><th style="width:150px">First Name</th><th style="width:150px">Last Name</th><th style="width:300px">Email </th><th style="width:250px">Company </th></tr></thead><tbody></tbody></table></li>');
            });
        }
        if (AutoCompleteFlag == "Event") {
            var self = this;
            $.each(items, function (index, item) {
                self._renderItem(ul, item);
                if (index == 0) ul.prepend('<li style="font-weight:bold" class="header-auto" ><table><thead><tr><th style="width:150px">Event Name</th><th style="width:150px">Event Date</th><th style="width:300px">Event Location </th></tr></thead><tbody></tbody></table></li>');
            });
        }
        if (AutoCompleteFlag == "DecorationCost") {
            var self = this;
            $.each(items, function (index, item) {
                self._renderItem(ul, item);
                if (index == 0) ul.prepend('<li style="font-weight:bold" class="header-auto" ><table><thead><tr><th style="width:100px">Quantity</th><th style="width:100px">Cost</th></tr></thead><tbody></tbody></table></li>');
            });
        }
        if (AutoCompleteFlag == "Decoration") {
            var self = this;
            $.each(items, function (index, item) {
                self._renderItem(ul, item);
                if (index == 0) ul.prepend('<li style="font-weight:bold; display:none" class="header-auto" ><table><thead><tr><th style="width:150px">Decoration</th></tr></thead><tbody></tbody></table></li>');
            });
        }
        if (AutoCompleteFlag == "Organisation") {
            var self = this;
            $.each(items, function (index, item) {
                self._renderItem(ul, item);
                if (index == 0) ul.prepend('<li style="font-weight:bold" class="header-auto" ><table><thead><tr><th style="width: 280px;">Organisation Name</th><th style="width: 230px;">Trading Name</th><th style="width: 210px;">Branch</th></tr></thead><tbody></tbody></table></li>');
            });
        }
        //18 Aug 2018 (N)
        if (AutoCompleteFlag == "Opportunity") {
            var self = this;
            $.each(items, function (index, item) {
                self._renderItem(ul, item);
                if (index == 0) ul.prepend('<li style="font-weight:bold" class="header-auto"><table><thead><tr><th style="width:200px;">Opportunity Name</th><th style="width:200px;">Required by date</th><th style="width:200px;">Stage</th><th style="width:150px;">Status</th><th style="width:200px;">JobManager</th></tr></thead><tbody></tbody></table></li>');
            });
        }
        //18 Aug 2018 (N)

        //31 July 2019 (N) Pantone Number
        if (AutoCompleteFlag == "Pantone") {
            var self = this;
            $.each(items, function (index, item) {
                self._renderItem(ul, item);
                if(index == 0) ul.prepend('<li style="font-weight:bold" class="header-auto"><table><tr><th style="width:150px;">Pantone</th><th style="width:150px;">Hex Value</th></tr></table></li>')
            });
        }
        //31 July 2019 (N) Pantone Number
    };
    $.ui.autocomplete.prototype._renderItem = function (ul, item) {

        //P 10 Jan OptionCode
        if (AutoCompleteFlag == "OptionCode") {
            return $("<li></li>")
                .data("item.autocomplete", item)
                .append('<a class="myclass"> <table><tr><td style="width:150px">' + item.Code + '</td><td style="width:150px">' + (item.cost == null ? parseFloat(0).toFixed(2) : parseFloat(item.cost).toFixed(2)) + '</td><td style="width:150px">' + item.ItemName + '</td><td style="width:150px">' + item.BrandName + '</td><td style="width:150px">' + item.Link + '</td></tr></table></a>')
                .appendTo(ul);
        }
        //P 10 Jan OptionCode

        if (AutoCompleteFlag == "Contact") {
            return $("<li></li>")
                .data("item.autocomplete", item)
                .append('<a class="myclass"> <table><tr><td style="width:150px">' + item.first_name + '</td><td style="width:150px">' + item.last_name + '</td><td style="width:300px">' + item.email + '</td><td style="width:250px">' + item.Company + '</td></tr></table></a>')
                .appendTo(ul);
        }
        if (AutoCompleteFlag == "Event") {
            return $("<li></li>")
                .data("item.autocomplete", item)
                .append('<a class="myclass"> <table><tr><td style="width:150px">' + item.EventName + '</td><td style="width:150px">' + item.EventDate1 + '</td><td style="width:300px">' + item.EventLocation + '</td></tr></table></a>')
                .appendTo(ul);
        }
        if (AutoCompleteFlag == "DecorationCost") {
            return $("<li></li>")
                .data("item.autocomplete", item)
                .append('<a class="myclass"> <table><tr><td style="width:100px">' + item.Quantity + '</td><td style="width:100px">' + item.Cost + '</td></tr></table></a>')
                .appendTo(ul);
        }
        if (AutoCompleteFlag == "Decoration") {
            return $("<li></li>")
                .data("item.autocomplete", item)
                .append('<a class="myclass"> <table><tr><td style="width:150px">' + item.Dec_Desc + '</td></tr></table></a>')
                .appendTo(ul);
        }
        if (AutoCompleteFlag == "Organisation") {
            return $("<li></li>")
                .data("item.autocomplete", item)
                .append('<a class="myclass"> <table><tr><td style="width: 280px;">' + item.OrgName + '</td><td style="width: 230px;">' + item.TradingName + '</td><td style="width: 210px;">' + item.Brand + '</td></tr></table></a>')
                .appendTo(ul);
        }
        //18 Aug 2018 (N)
        if (AutoCompleteFlag == "Opportunity") {
            return $("<li></li>")
                .data("item.autocomplete", item)
                .append('<a class="myclass"><table><tr><td style="width:200px;">' + item.OppName + '</td><td style="width:200px;">' + item.ReqDate + '</td><td style="width:200px;">' + item.Stage + '</td><td style="width:200px;">' + item.Status + '</td><td style="width:200px;">' + item.AccountManagerFullName + '</td></tr></table></a>')
                .appendTo(ul);
        }
        //18 Aug 2018 (N)

        //31 July 2019 (N) Pantone Number
        if (AutoCompleteFlag == "Pantone") {
            return $("<li></li>")
                .data("item.autocomplete", item)
                .append('<a class="myclass"><table><tr><td style="width:150px;">' + item.Pantone + '</td><td style="width:150px;">' + item.Hexvalue + '</td></tr></table></a>')
                .appendTo(ul);
        }
        //31 July 2019 (N) Pantone Number
    };

    //18 Aug 2018 (N)
    if ($('#PageName').val() == "Event") {

        $("#oppName").autocomplete({

            source: function (request, response) {
                AutoCompleteFlag = "Opportunity";
                $.ajax({
                    url: '/Opportunity/GetOpportunityByOppName',
                    data: "{ 'prefix': '" + request.term + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        response($.map(data, function (item) {
                            return item;
                        }))
                    },
                    error: function (response) {

                    },
                    failure: function (response) {

                    }
                });
            },
            select: function (e, i) {
                $("#btnsaveContact").addClass("customAlertChange");
                i.item.value = i.item.OppName;
                $('#lblOpportunityId').text('');
                $('#oppQuantity').val('');
                $('#datepicker').val('');
                $('#depositreqdate').val('');
                $('#OppShipping').val('');
                $('#txtshippingday').val('');
                $('#txtshippingprice').val('');
                $('#oppSource').val('');
                $('#oppCampaign').val('');
                //$('#txtConfirmedDate').val('');
                $('#oppStage').val('');
                $('#ddlOppAcctMgr').val('');
                //$('#ddlDecline').val('');
                $('#oppNotes').val('');
                $('#lblOpportunityId').text(i.item.OpportunityId);
                $('#oppQuantity').val(i.item.Quantity);
                $('#datepicker').val(i.item.ReqDate);
                $('#depositreqdate').val(i.item.DepositReqDate);
                $('#OppShipping').val(i.item.Shipping);
                $('#txtshippingday').val(i.item.ShippingTo);
                $('#txtshippingprice').val(i.item.Price);
                //$('#oppSource').val(i.item.Source);
                if ($('#StatCampaign').is(":visible")) {
                    $('#oppSource1').val(i.item.Source);

                }
                if ($('#DivQuote').is(":visible")) {
                    $('#oppSource2').val(i.item.Source);

                }
                $('#oppCampaign').val(i.item.Compaign);
                //$('#txtConfirmedDate').val('');
                $('#oppStage').val(i.item.Stage);

                if ($('#oppStage').val() == "Opportunity") {
                    //alert();
                    $('#StatCampaign').css("display", "block");
                    //$('#txtlost').css("display", "none");
                    $('#DivQuote').css("display", "none");
                    //$('#txtCancelled').css("display", "none");
                    //$('#lblrepeatform').css("display", "block");
                }
                if ($('#oppStage').val() == "Quote") {
                    //$('#lblrepeatform').css("display", "none");
                    $('#DivQuote').css("display", "block");
                    $('#StatCampaign').css("display", "none");
                    //$('#txtrepeatfrom').css("display", "none");
                    //$('#ddlDecline').css("display", "none");
                    $('#lblRepeat').css("display", "block");
                    $('#quotestatus').css("display", "block");
                    $('#orderstatus').css("display", "none");
                    $('#lblCampaign').css("display", "none");
                }
                if ($('#oppStage').val() != "Opportunity" && $('#oppStage').val() != "Quote") {
                    //$('#lblrepeatform').css("display", "none");
                    $('#DivQuote').css("display", "block");
                    $('#StatCampaign').css("display", "none");
                    //$('#txtrepeatfrom').css("display", "none");
                    //$('#ddlDecline').css("display", "none");
                    $('#quotestatus').css("display", "none");
                    $('#lblRepeat').css("display", "block");
                    $('#lblCampaign').css("display", "none");
                    $('#orderstatus').css("display", "block");
                }

                SetAccountManager(i.item.AcctManagerId, "ddlOppAcctMgr", "OppAccountManager");

                if (i.item.job_department != "" && i.item.job_department != undefined && i.item.job_department != null) {
                    ms.setValue(i.item.job_department.split(','));
                }
                //$('#ddlOppAcctMgr').val(i.item.AcctManagerId);
                //$('#ddlDecline').val('');
                $('#oppNotes').val(i.item.OppNotes);
                MakeOpportunityReadonly();
                GetContactByOppId($('#lblOpportunityId').text());
            },
            // baans change 11th Sept for autocomplete
            //minLength: 1
            minLength: 2
            // baans end 11th Sept
        })
    }
    //18 Aug 2018 (N)
    if ($('#PageName').val() != "ContactDetails") {
        $("#txtFirstName").autocomplete({

            source: function (request, response) {
                AutoCompleteFlag = "Contact";
                // baans change 27th September for Autocomplete by Type
                var ContactType = $('#txtContactType').val();
                $.ajax({
                    url: '/Contact/GetContactByFirstName',
                    data: "{ 'prefix': '" + request.term + "', 'ContType': '" + ContactType + "'}",
                    // baans end 27th September 
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        ContactType = "";
                        // baans end 27th September
                        response($.map(data, function (item) {
                            return item;
                        }))
                    },
                    error: function (response) {

                    },
                    failure: function (response) {

                    }
                });
            },
            select: function (e, i) {
                $("#btnsaveContact").addClass("customAlertChange");
                i.item.value = i.item.first_name;
                $("#txtLastName").val('');
                $("#txtEmail").val('');
                $("#txtContactNo").val('');
                $("#txtNotes").val('');
                $("#txtLastName").val(i.item.last_name);
                $("#txtEmail").val(i.item.email);
                $("#txtContactNo").val(i.item.mobile);
                $("#txtNotes").val(i.item.notes);
                $("#txtContactRole").val(i.item.ContactRole);
                $("#ddlTitle").val(i.item.title);
                $("#txtContactType").val(i.item.ContactType);
                $("#HiddenContactId").val(i.item.id);
                SetAccountManager(i.item.acct_manager_id, "ddlAcctMgr", "AccountManager");
                // $("#ddlAcctMgr").val(i.item.acct_manager_id);

                if ($("#HiddenFOrPrimary").val() != "") {
                    if ($("#HiddenContactId").val() == $("#HiddenFOrPrimary").val()) {
                        $('#ContactisPrimary').prop('checked', true);
                    }
                    else {
                        $('#ContactisPrimary').prop('checked', false);
                    }
                }
                if ($('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "OrderDetails") {
                    //if ($('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "ContactDetails") {
                    if (i.item.OrgId != null && i.item.OrgId != "") {
                        GetOrganisationById(i.item.OrgId);
                        $('#HiddenforDefaultOrgID').val(i.item.OrgId);
                    }
                    else {
                        ResetOrgForm();
                        $('#HiddenforDefaultOrgID').val('');
                    }
                }
                if ($('#PageName').val() == "Event") {
                    $('#txtOrgName').val('');
                    $('#txtOrgName').val(i.item.OrgName);
                }
                //  $('#ContactisPrimary').prop('checked', false);
                MakeReadonly();
            },
            // baans change 11th Sept for autocomplete
            //minLength: 1
            minLength: 2
            // baans end 11th Sept
        })

        $("#txtLastName").autocomplete({

            source: function (request, response) {
                AutoCompleteFlag = "Contact";
                // baans change 27th September for Autocomplete by Type
                var ContactType = $('#txtContactType').val();
                $.ajax({
                    url: '/Contact/GetContactByLastName',
                    data: "{ 'prefix': '" + request.term + "', 'ContType': '" + ContactType + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        ContactType = "";
                        // baans end 27th Sept
                        response($.map(data, function (item) {
                            return item;
                        }))
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
            },
            select: function (e, i) {
                $("#btnsaveContact").addClass("customAlertChange");
                i.item.value = i.item.last_name;
                $("#txtFirstName").val('');
                $("#txtEmail").val('');
                $("#txtContactNo").val('');
                $("#txtNotes").val('');
                $("#txtFirstName").val(i.item.first_name);
                $("#txtEmail").val(i.item.email);
                $("#txtContactNo").val(i.item.mobile);
                $("#txtNotes").val(i.item.notes);
                $("#txtContactRole").val(i.item.ContactRole);
                $("#ddlTitle").val(i.item.title);

                $("#txtContactType").val(i.item.ContactType);
                $("#HiddenContactId").val(i.item.id);
                SetAccountManager(i.item.acct_manager_id, "ddlAcctMgr", "AccountManager");
                // $("#ddlAcctMgr").val(i.item.acct_manager_id);
                if ($("#HiddenFOrPrimary").val() != "") {
                    if ($("#HiddenContactId").val() == $("#HiddenFOrPrimary").val()) {
                        $('#ContactisPrimary').prop('checked', true);
                    }
                    else {
                        $('#ContactisPrimary').prop('checked', false);
                    }
                }
                if ($('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "OrderDetails") {
                    //if ($('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "ContactDetails") {
                    if (i.item.OrgId != null && i.item.OrgId != "") {
                        GetOrganisationById(i.item.OrgId);
                        $('#HiddenforDefaultOrgID').val(i.item.OrgId);
                    }
                    else {
                        ResetOrgForm();
                        $('#HiddenforDefaultOrgID').val('');
                    }

                }
                if ($('#PageName').val() == "Event") {
                    $('#txtOrgName').val('');
                    $('#txtOrgName').val(i.item.OrgName);
                }
                MakeReadonly();
            },
            // baans change 11th Sept
            //minLength: 1
            minLength: 2
            // baans end 11th Sept
        });
        $("#txtEmail").autocomplete({
            source: function (request, response) {
                AutoCompleteFlag = "Contact";
                // baans change 27th September for Autocomplete by Type
                var ContactType = $('#txtContactType').val();
                $.ajax({
                    url: '/Contact/GetContactByEmail',
                    data: "{ 'prefix': '" + request.term + "', 'ContType': '" + ContactType + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        ContactType = "";
                        // baans end 27th Sept
                        response($.map(data, function (item) {
                            return item;
                        }))
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
            },
            select: function (e, i) {
                $("#btnsaveContact").addClass("customAlertChange");
                i.item.value = i.item.email;
                $("#txtFirstName").val('');
                $("#txtLastName").val('');
                $("#txtContactNo").val('');
                $("#txtNotes").val('');
                $("#txtFirstName").val(i.item.first_name);
                $("#txtLastName").val(i.item.last_name);
                $("#txtContactNo").val(i.item.mobile);
                $("#txtNotes").val(i.item.notes);
                $("#txtContactRole").val(i.item.ContactRole);
                $("#ddlTitle").val(i.item.title);

                $("#txtContactType").val(i.item.ContactType);
                $("#HiddenContactId").val(i.item.id);
                SetAccountManager(i.item.acct_manager_id, "ddlAcctMgr", "AccountManager");
                // $("#ddlAcctMgr").val(i.item.acct_manager_id);
                if ($("#HiddenFOrPrimary").val() != "") {
                    if ($("#HiddenContactId").val() == $("#HiddenFOrPrimary").val()) {
                        $('#ContactisPrimary').prop('checked', true);
                    }
                    else {
                        $('#ContactisPrimary').prop('checked', false);
                    }
                }
                if ($('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "OrderDetails") {
                    //if ($('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "ContactDetails") {
                    if (i.item.OrgId != null && i.item.OrgId != "") {
                        GetOrganisationById(i.item.OrgId);
                        $('#HiddenforDefaultOrgID').val(i.item.OrgId);
                    }
                    else {
                        ResetOrgForm();
                        $('#HiddenforDefaultOrgID').val('');
                    }
                }
                if ($('#PageName').val() == "Event") {
                    $('#txtOrgName').val('');
                    $('#txtOrgName').val(i.item.OrgName);
                }
                MakeReadonly();
            },
            // baans change 11th Sept for autocomplete
            //minLength: 1
            minLength: 2
            // baans end 11th Sept
        });
    }

    //P 10 Jan OptionCode
    $("#txtCode").autocomplete({
        source: function (request, response) {
            AutoCompleteFlag = "OptionCode";
            $("#HiddenOptionCodeID").val(0);
            $.ajax({
                url: '/Opportunity/OptionCodeByPrefix',
                data: "{ 'prefix': '" + request.term + "'}",
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }))
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
            //$("#btnsaveevent").addClass("customAlertChange");
            i.item.value = i.item.Code;
            $("#txtLink").val(i.item.Link);
            $('#ddlItem').val(i.item.itemId).trigger("chosen:updated");
            $('#ddlBrand').val(i.item.BrandId).trigger("chosen:updated");
            $("#HiddenOptionCodeID").val(i.item.id);
            if (i.item.cost != null && i.item.cost != 0) {
                $("#txtCost").val(parseFloat(i.item.cost).toFixed(2));
                CalculateUnitPrice($(this));
            }
        },
        minLength: 2
    });
    //}
    //P 10 Jan OptionCode

    //31 July 2019 (N) Pantone Number
    $('#txtPantone').autocomplete({
        source: function(request, response){
            AutoCompleteFlag = "Pantone";
            $.ajax({
                url:'/Application/GetPantone',
                data: "{ 'prefix': '" + request.term + "'}",
                dataType:"json",
                type:"Post",
                contentType:"application/json; charset=utf-8",
                success: function(data){
                    response($.map(data, function(item){
                        return item;
                    }))
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function(e, i){
            i.item.value = i.item.Pantone;
            $('#hdnPantoneId').val('');
            $('#txtBucketId').val('');
            $('#txtHexValue').val('');
            $('#hdnPantoneId').val(i.item.Id);
            //$('#txtBucketId').val(i.item.BucketId);
            $('#txtHexValue').val(i.item.Hexvalue);

            $('#txtColourChip').css('background-color', '#' + i.item.Hexvalue);
            $('#txtColourChip').css('border', '1px solid #' + i.item.Hexvalue);
        },
        minLength: 2
    });
    //31 July 2019 (N) Pantone Number

    $("#txtEventName").autocomplete({

        source: function (request, response) {
            AutoCompleteFlag = "Event";
            $.ajax({
                url: '/Event/GetEventByName',
                data: "{ 'prefix': '" + request.term + "'}",
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }))
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
            $("#btnsaveevent").addClass("customAlertChange");
            i.item.value = i.item.EventName;
            $("#txteventDate").val('');
            $("#txtEventLocation").val('');
            $("#txteventCycle").val('');
            $("#txtnextDate").val('');
            $("#txtEventWebsite").val('');
            $("#txtEventNotes").val('');
            $("#txteventDate").val(DateFormat(i.item.EventDate));
            $("#txtEventLocation").val(i.item.EventLocation);
            $("#txtEventWebsite").val(i.item.EventWebsite);
            $("#txtEventNotes").val(i.item.EventNotes);
            $("#txtnextDate").val(DateFormat(i.item.NextDate));
            $("#txteventCycle").val(i.item.EventCycle);

            $("#HiddenEventId").val(i.item.EventId);
            MakeEventReadonly();

        },
        // baans change 11th Sept for autocomplete
        //minLength: 1
        minLength: 2
        // baans end 11th Sept
    });
    $("#txtRangeFront,#txtRangeBack,#txtRangeLeft,#txtRangeRight,#txtRangeOther").autocomplete({

        source: function (request, response) {
            AutoCompleteFlag = "DecorationCost";
            var target = $(this.element);
            var txtDecoration = "";
            if ((target.attr('id') == "txtRangeFront")) {
                txtDecoration = $('#txtDecorationFront').val();
            }
            if ((target.attr('id') == "txtRangeBack")) {
                txtDecoration = $('#txtDecorationBack').val();
            }
            if ((target.attr('id') == "txtRangeLeft")) {
                txtDecoration = $('#txtDecorationLeft').val();
            }
            if ((target.attr('id') == "txtRangeRight")) {
                txtDecoration = $('#txtDecorationRight').val();
            }
            if ((target.attr('id') == "txtRangeOther")) {
                txtDecoration = $('#txtDecorationOther').val();
            }
            $.ajax({
                url: '/Opportunity/GetDecorationCostByQty',
                data: "{ 'prefix': '" + request.term + "','Decoration': '" + txtDecoration + "'}",
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }))
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
            i.item.value = i.item.Quantity;
            var target = $(e.target);

            if ((target.attr('id') == "txtRangeFront")) {
                $('#hiddenRangeFrontCost').val(i.item.Cost);
            }
            if ((target.attr('id') == "txtRangeBack")) {
                $('#hiddenRangeBackCost').val(i.item.Cost);
            }
            if ((target.attr('id') == "txtRangeLeft")) {
                $('#hiddenRangeLeftCost').val(i.item.Cost);
            }
            if ((target.attr('id') == "txtRangeRight")) {
                $('#hiddenRangeRightCost').val(i.item.Cost);
            }
            if ((target.attr('id') == "txtRangeOther")) {
                $('#hiddenRangeOtherCost').val(i.item.Cost);
            }


        },
        // baans change 11th Sept for autocomplete
        //minLength: 1
        minLength: 2
        // baans end 11th Sept
    })
    $("#txtDecorationFront,#txtDecorationBack,#txtDecorationLeft,#txtDecorationRight,#txtDecorationOther").autocomplete({

        source: function (request, response) {
            AutoCompleteFlag = "Decoration";
            $.ajax({
                url: '/Opportunity/GetDecorationByDesc',
                data: "{ 'prefix': '" + request.term + "'}",
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }))
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
            i.item.value = i.item.Dec_Desc;
            var target = $(e.target);
            if (target.attr('id') == "txtDecorationFront") {
                //GetDecorationCost('txtRange1', i.item.value);
                $("#txtRangeFront").val('');
                // baans change 31st October for calculation without quantity and cost
                $("#btnOptionSave").addClass("customAlertChange");
                var Quantity = "";
                var Cost = "";
                Quantity = "0";
                Cost = "0";
                $("#txtRange" + "Front" + "").val(Quantity);
                $("#hiddenRange" + "Front" + "Cost").val(Cost);
                CalculateUnitPrice('');
                $("#txtRangeFront").val('');
                $("#hiddenRangeFrontCost").val('');     //19 Nov 2018 (N)
                // baans end 31st October
                GetDecorationCost('Front', i.item.value);
            }
            if (target.attr('id') == "txtDecorationBack") {
                $("#txtRangeBack").val('');
                // baans change 31st October for calculation without quantity and cost
                $("#btnOptionSave").addClass("customAlertChange");
                var Quantity = "";
                var Cost = "";
                Quantity = "0";
                Cost = "0";
                $("#txtRange" + "Back" + "").val(Quantity);
                $("#hiddenRange" + "Back" + "Cost").val(Cost);
                CalculateUnitPrice('');
                $("#txtRangeBack").val('');
                $("#hiddenRangeBackCost").val('');      //19 Nov 2018 (N)
                // baans end 31st October

                GetDecorationCost('Back', i.item.value);
            }
            if (target.attr('id') == "txtDecorationLeft") {
                $("#txtRangeLeft").val('');
                // baans change 31st October for calculation without quantity and cost
                $("#btnOptionSave").addClass("customAlertChange");
                var Quantity = "";
                var Cost = "";
                Quantity = "0";
                Cost = "0";
                $("#txtRange" + "Left" + "").val(Quantity);
                $("#hiddenRange" + "Left" + "Cost").val(Cost);
                CalculateUnitPrice('');
                $("#txtRangeLeft").val('');
                $("#hiddenRangeLeftCost").val('');      //19 Nov 2018 (N)
                // baans end 31st October
                GetDecorationCost('Left', i.item.value);
            }
            if (target.attr('id') == "txtDecorationRight") {
                $("#txtRangeRight").val('');
                // baans change 31st October for calculation without quantity and cost
                $("#btnOptionSave").addClass("customAlertChange");
                var Quantity = "";
                var Cost = "";
                Quantity = "0";
                Cost = "0";
                $("#txtRange" + "Right" + "").val(Quantity);
                $("#hiddenRange" + "Right" + "Cost").val(Cost);
                CalculateUnitPrice('');
                $("#txtRangeRight").val('');
                $("#hiddenRangeRightCost").val('');     //19 Nov 2018 (N)
                // baans end 31st October
                GetDecorationCost('Right', i.item.value);
            }
            if (target.attr('id') == "txtDecorationOther") {
                $("#txtRangeOther").val('');
                // baans change 31st October for calculation without quantity and cost
                $("#btnOptionSave").addClass("customAlertChange");
                var Quantity = "";
                var Cost = "";
                Quantity = "0";
                Cost = "0";
                $("#txtRange" + "Other" + "").val(Quantity);
                $("#hiddenRange" + "Other" + "Cost").val(Cost);
                CalculateUnitPrice('');
                $("#txtRangeOther").val('');
                $("#hiddenRangeOtherCost").val('');     //19 Nov 2018 (N)
                // baans end 31st October
                GetDecorationCost('Other', i.item.value);
            }
        },
        // baans change 11th Sept for autocomplete
        //minLength: 1
        minLength: 2
        // baans end 11th September
    })
    if ($('#PageName').val() != "OrganisationDetail") {
        $("#txtorgName").autocomplete({

            source: function (request, response) {
                AutoCompleteFlag = "Organisation";
                // baans change 27th September for Autocomplete  by type
                var OrgType = $('#txtOrgType').val();
                $.ajax({
                    url: '/Organisation/GetOrganisationByFirstName',
                    //data: "{ 'prefix': '" + request.term + "','PageSource':'" + $('#PageName').val() + "', 'OrgType': '" + OrgType + "'}",
                    data: "{ 'prefix': '" + request.term + "', 'OrgType': '" + OrgType + "'}",
                    // baans end 27th Sept
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        OrgType = "";
                        response($.map(data, function (item) {
                            return item;
                        }))
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
            },
            select: function (e, i) {
                $("#btnsaveOrganisation").addClass("customAlertChange");
                //alert(i.item.OrgId);
                i.item.value = i.item.OrgName;
                $("#txtorgName").val('');
                $("#txtTradeName").val('');
                $("#txtABNName").val('');
                $("#txtMainPhone").val('');
                $("#txtWebAddress").val('');
                $("#txtbrand").val('');
                $("#txtOrgNotes").val('');
                $('#txtOrgType').val('');
                $('#ddlOrgAcctMgr').val('');
                $("#txtorgName").val(i.item.OrgName);
                $("#txtTradeName").val(i.item.TradingName);
                $("#txtABNName").val(i.item.ABN);
                $("#txtMainPhone").val(i.item.MainPhone);
                $("#txtWebAddress").val(i.item.WebAddress);
                $("#txtbrand").val(i.item.Brand);
                $("#txtOrgNotes").val(i.item.OrgNotes);
                $("#hdnOrgId").val(i.item.OrgId);
                $('#txtOrgType').val(i.item.OrganisationType);
                //$('#txtContactEmail').val(i.item.email);
                SetAccountManager(i.item.AcctMgrId, "ddlOrgAcctMgr", "AccountManager");
                // $('#ddlOrgAcctMgr').val(i.item.AcctMgrId);
                $('#txtorgEmail').val(i.item.EmailAddress);


                MakeorgReadonly();
                //tarun 06/09/2018
                if ($('#PageName').val() != "PurchaseDetails") {
                    $("#txtTradingName").val(i.item.TradingName);
                    var count = CheckDeliveryAddress(i.item.OrgId);
                    GetOrganisationAddress(i.item.OrgId, "");
                }
                //end

            },
            // baans change 11th Sept for autocomplete
            //minLength: 1
            minLength: 2
            // baans end 11th Sept
        })
    }
}

)
function changeImage(type, department) {
    var hostname = window.location.origin;
    if (type == "Job" && department != "") {
        $("#ImageChange").attr("src", hostname + "/Content/images/JOB.png");
        //document.getElementById('btnShipping').style.display = 'block';
        //document.getElementById('btnInvoiced').style.display = 'block';
        //document.getElementById('btnInvoicing').style.display = 'block';
        $(".tablinks").each(function () {
            $(this).removeClass("active");
        });
        $("#" + type).addClass("active");
    }
    else {
        //$("#ImageChange").attr("src", hostname + "/Content/images/ORDER.png");
        //document.getElementById('btnShipping').style.display = 'none';
        //document.getElementById('btnInvoicing').style.display = 'none';
        //document.getElementById('btnInvoiced').style.display = 'block';
        // baans change 17th November
        switch (type) {
            case "All":
                alert("all");
                //document.getElementById("tab_Img").src = "~/Content/images/OPPORTUNITY.png";
                $("#ImageChange").attr("src", hostname + "/Content/images/OPPORTUNITY.png");
                break;

            default:
                alert("Individual");
                break;
        }
        // baans end 17th November


    }
}

function changeImageNew(type, department) {
    var hostname = window.location.origin;
    if (type == "Job1" && department != "") {
        $("#ImageChangeNew").attr("src", hostname + "/Content/images/JOB.png");
        //document.getElementById('btnShipping1').style.display = 'block';
        //document.getElementById('btnInvoiced1').style.display = 'block';
        $(".UpperTabs").each(function () {
            $(this).removeClass("active");
        });
        $("#" + type).addClass("active");
    }
    else {
        //document.getElementById('btnShipping1').style.display = 'none';
        //document.getElementById('btnInvoiced1').style.display = 'block';
        // baans change 17th November
        switch (type) {
            case "All":
                $("#ImageChangeNew").attr("src", hostname + "/Content/images/OPPORTUNITY.png");
                break;

            case "Opportunity":
                $("#ImageChangeNew").attr("src", hostname + "/Content/images/OPPORTUNITY.png");
                break;

            case "Declined":
                $("#ImageChangeNew").attr("src", hostname + "/Content/images/OPPORTUNITY.png");
                break;

            case "Quote":
                $("#ImageChangeNew").attr("src", hostname + "/Content/images/QUOTE.png");
                break;

            case "Lost":
                $("#ImageChangeNew").attr("src", hostname + "/Content/images/QUOTE.png");
                break;

            case "Order":
                $("#ImageChangeNew").attr("src", hostname + "/Content/images/ORDER.png");
                break;

            case "Cancelled":
                $("#ImageChangeNew").attr("src", hostname + "/Content/images/ORDER.png");
                break;

            case "Packing":
                $("#ImageChangeNew").attr("src", hostname + "/Content/images/PACKING.png");
                break;

            case "Invoiced":
                $("#ImageChangeNew").attr("src", hostname + "/Content/images/INVOICING.png");
                break;

            case "Shipping":
                $("#ImageChangeNew").attr("src", hostname + "/Content/images/SHIPPING.png");
                break;

            case "Complete":
                $("#ImageChangeNew").attr("src", hostname + "/Content/images/COMPLETE.png");
                break;

            default:
                $("#ImageChangeNew").attr("src", hostname + "/Content/images/OPPORTUNITY.png");
                break;
        }
        // baans end 17th November
    }
}

//function OrderGridList(type, department, tabType) {

//    if (tabType == "UpperTab") {
//        changeImageNew(type, department);
//        if (type == "Job1") {
//            type = "Job";
//        }
//        if (type == "btnInvoiced1") {
//            type == "btnInvoiced";
//        }
//        if (type == "btnShipping1") {
//            type == "btnShipping";
//        }
//    } else {
//        changeImage(type, department);
//    }
//    var UserProfile = $('#profileOfUser').val();
//    var url = '/Order/GetOrderList';
//    var model = { Type: type, Department: department, UserProfile: UserProfile };

//    if (type == "Custom") {
//        url = '/Opportunity/GetCustomOppList';
//        model = { CustomText: $('#searchtextbox').val(), TableName: 'Vw_tblOpportunity' };
//    }
//    var data;
//    $.ajax({
//        url: url,
//        data: model,
//        async: false,
//        success: function (response) {
//            data = response;
//        },
//        error: function (response) {
//            alert();
//        },
//        type: 'post'

//    });

//    var obj =
//    {
//        selectionModel: { type: 'row' },
//        virtualX: true, virtualY: true,
//        resizable: false,
//        pageModel: { type: "local", rPP: 50 },
//        strNoRows: 'No records found for the selected Criteria',
//        scrollModel: { horizontal: false },
//        editable: false,
//        wrap: false,
//        hwrap: false,
//        width: "97%",
//        height: 750,
//        numberCell: { width: 0, title: "", minWidth: 0 },
//        columnTemplate: { width: 120,halign:"left" },
//    };


//    obj.colModel = [
//        {
//            title: "Job No", width: "4%", dataType: "int", align: "center", dataIndx: "DisplayOpportunityId",
//            render: function (ui) {
//                var id = ui.cellData;
//                var Stage = ui.rowData.Stage;

//                if (Stage == "Quote") {
//                    return "<a href='/opportunity/QuoteDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
//                }
//                else if (Stage == "Order") {
//                    return "<a href='/opportunity/OrderDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
//                }
//                else if (Stage == "Job") {
//                    return "<a href='/opportunity/JobDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
//                }
//                else if (Stage == "Order Packed" || Stage == "Stock Decorated") {
//                    return "<a href='/opportunity/PackingDetails/" + parseInt(id) + "'><label  class='internalLnk'>" + id + "</label></a>"
//                }
//                else if (Stage == "Order Shipped") {
//                    return "<a href='/opportunity/ShippingDetails/" + parseInt(id) + "'><label  class='internalLnk'>" + id + "</label></a>"
//                }
//                else if (Stage == "Complete") {
//                    return "<a href='/opportunity/CompleteDetails/" + parseInt(id) + "'><label  class='internalLnk'>" + id + "</label></a>"
//                }
//                else if (Stage == "Order Invoiced") {
//                    return "<a href='/opportunity/InvoicingDetails/" + parseInt(id) + "'><label  class='internalLnk'>" + id + "</label></a>"
//                }
//                else {
//                    return "<a href='/opportunity/OpportunityDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
//                }
//                //if (Stage == "Quote") {
//                //    return "<a href='/opportunity/QuoteDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
//                //}
//                //else if (Stage == "Order") {
//                //    return "<a href='/opportunity/OrderDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
//                //}
//                //else {
//                //    return "<a href='/opportunity/OpportunityDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
//                //}




//            }
//        },
//        {
//            title: "Date", dataIndx: "Orderdate", dataType: "string", align: "right", width: "6%",
//            render: function (ui) {
//                var date = ui.cellData;
//                if (date != null && date != undefined && date != "") {
//                    var nowDateopp = new Date(parseInt(date.substr(6)));
//                    // baans change 17th July
//                    //return nowDateopp.getDate() + '/' + (nowDateopp.getMonth() + 1) + '/' + nowDateopp.getFullYear();
//                    return ("0" + nowDateopp.getDate()).slice(-2) + '/' + ("0" + (nowDateopp.getMonth() + 1)).slice(-2) + '/' + nowDateopp.getFullYear();
//                    // baans end 17th July
//                }
//            },
//        },
//        {
//            title: "JobHUB Name", width: "7%", dataType: "string", dataIndx: "OppName"

//        },
//        { title: "Qty", width: "3%", dataType: "string", align: "right", dataIndx: "Quantity" },
//        { title: "Department", width: "8%", dataIndx: "DepartmentName", dataType: "int" },
//        {
//            title: "Req By", width: "6%", dataType: "string", align: "right", dataIndx: "ReqDate",
//            render: function (ui) {
//                var date = ui.cellData;
//                if (date != null && date != undefined && date != "") {
//                    var nowDateopp = new Date(parseInt(date.substr(6)));
//                    // baans change 17th July
//                    //return nowDateopp.getDate() + '/' + (nowDateopp.getMonth() + 1) + '/' + nowDateopp.getFullYear();
//                    return ("0" + nowDateopp.getDate()).slice(-2) + '/' + ("0" + (nowDateopp.getMonth() + 1)).slice(-2) + '/' + nowDateopp.getFullYear();
//                    // baans end 17th July
//                }
//            },
//        },
//        {
//            title: "Dep Req By", width: "6%", dataType: "string",align: "right", dataIndx: "DepositReqDate",
//            render: function (ui) {
//                var date = ui.cellData;
//                if (date != null && date != undefined && date != "") {
//                    var nowDateopp = new Date(parseInt(date.substr(6)));
//                    // baans change 17th July
//                    //return nowDateopp.getDate() + '/' + (nowDateopp.getMonth() + 1) + '/' + nowDateopp.getFullYear();
//                    return ("0" + nowDateopp.getDate()).slice(-2) + '/' + ("0" + (nowDateopp.getMonth() + 1)).slice(-2) + '/' + nowDateopp.getFullYear();
//                    // baans end 17th July
//                }
//            },
//        },
//        {
//            title: "Contact", width: "10%", dataType: "string", dataIndx: "mobile",
//            render: function (ui) {
//                var currentF = 0;
//                currentF = ui.cellData;
//                if (currentF != null && currentF != undefined) {
//                    var newFormat1 = currentF.toString().substr(0, 4);
//                    var newFormat2 = currentF.toString().substr(4, 3);
//                    var newFormat3 = currentF.toString().substr(7, 3);
//                    var newFormat = newFormat1 + ' ' + newFormat2 + ' ' + newFormat3;
//                    return newFormat;
//                }
//            }
//        },
//        { title: "Order Notes", width: "30%", dataType: "string", dataIndx: "OrderNotes" },
//        { title: "Value", width: "6%", dataType: "string", dataIndx: "Value" },
//        { title: "Margin", width: "6%", dataType: "string", dataIndx: "Margin" },
//        { title: "Acct Manager", width: "9%", dataType: "string", dataIndx: "AcctManager" }
//    ];

//    obj.dataModel = { data: data };
//    if (tabType == "UpperTab") {
//        pq.grid("#OrderGridNew", obj);
//        pq.grid("#OrderGridNew", "refreshDataAndView");
//    } else {
//        pq.grid("#OrderGrid", obj);
//        pq.grid("#OrderGrid", "refreshDataAndView");
//    }

//}


function JobsGridList(type, department, tabType) {
    var dpmt = $(".headerBtn.active").attr('id');
    //alert($(".headerBtn.active").attr('id'));
    if (dpmt == "Current" || dpmt == "List") {
        department = "All";
    }
    else if (dpmt == "ScreenPrint") {
        department = "Screen Print";
    }
    else {
        department = $(".headerBtn.active").attr('id');
    }

    //29 April Stage Change List
    if (type == "Job1") {
        var data_start = null;
        var stageDataCalling = "OpportunityList";
        $('#HiddenCallingSource').val(stageDataCalling);
        $('#OppListSidebar').css("display", "block");
        $('#SectionRightSidebar').css("display", "block");
        $('#imageOppThumbnail').css("display", "block");
        //$('#leftsideStageheading').css("display", "block");
        $('.RightSideStage').css("display", "block");
        $('#OppThumbnail').css("display", "none");
        $('.rightheading').css("display", "none");
        $('.AboveStages').css("display", "none");
        $("#lblListJobStageData").text('');

        $('#btnOppoStage').css("display", "none");
        $('#lblOppDate').css("display", "none");
        $('#spanOppDate').css("display", "none");

        $('#spanQuoteDate').css("display", "none");
        $('#lblQuoteDate').css("display", "none");
        $('#btnQuoteStage').css("display", "none");

        $('#spanOrderDate').css("display", "none");
        $('#lblOrderDate').css("display", "none");
        $('#btnOrderStage').css("display", "none");

        $('#spanJobDate').css("display", "none");
        $('#lblJobDate').css("display", "none");
        $('#btnJobStage').css("display", "none");

        $('#spanConfirmDate').css("display", "none");
        $('#lblOrderConfirmDate').css("display", "none");
        $('#btnOrderConfirmedStage').css("display", "none");

        //Commented by baans 19Sep2020 Start
        //$('#spanJobAcceptedDate').css("display", "none");
        //$('#lblJobAcceptedDate').css("display", "none");
        //$('#btnJobAccepted').css("display", "none");

        //$('#spanProofCreatedDate').css("display", "none");
        //$('#lblProofCreatedDate').css("display", "none");
        //$('#btnProofCreated').css("display", "none");

        //$('#spanProofSentDate').css("display", "none");
        //$('#lblProofSentDate').css("display", "none");
        //$('#btnProofSent').css("display", "none");
        //Commented by baans 19Sep2020 End

        // Decorated
        $('#spanStockDecoratedDate').css("display", "none");
        $('#lblStockDecoratedDate').css("display", "none");
        $('#btnStockDecoratedStage').css("display", "none");

        $('#spanOrderPackedDate').css("display", "none");
        $('#lblOrderPackedDate').css("display", "none");
        $('#btnOrderPackedStage').css("display", "none");

        $('#spanOrderInvoicedDate').css("display", "none");
        $('#lblInvoiceDate').css("display", "none");
        $('#btnOrderInvoicedStage').css("display", "none");

        $('#spanPaidDate').css("display", "none");
        $('#lblPaidDate').css("display", "none");
        $('#btnPaidStage').css("display", "none");

        $('#spanOrderShippedDate').css("display", "none");
        $('#lblShippedDate').css("display", "none");
        $('#btnOrderShippedStage').css("display", "none");

        $('#spanCompletteDate').css("display", "none");
        $('#lblCompleteDate').css("display", "none");
        $('#btnCompleteStage').css("display", "none");

        // $('.sidebar-menu').css("margin-top", "-109px");
        if ($('#HiddenStageDateExist').val() == "DateExist") {
            $('#HiddenStageDateExist').val("");
        }
        else {
            ResetRightSideBar(data_start);
        }

    }
    else {
        $('#OppListSidebar').css("display", "none");
    }
    //29 April Stage Change List


    var stagetype = type;
    if (tabType == "UpperTab") {
        changeImageNew(type, department);
        if (type == "Job1") {
            type = "Job";
        }
        if (type == "btnInvoiced1") {
            type == "btnInvoiced";
        }
        if (type == "btnShipping1") {
            type == "btnShipping";
        }
    } else {
        changeImage(type, department);
    }

    var tfcondition = true;
    if (stagetype == "Quote" || stagetype == "Lost" || stagetype == "Order" || stagetype == "Cancelled" || stagetype == "Job1") {
        tfcondition = false;
    }

    var jwidth = "22%";
    if (stagetype == "Job1") {
        jwidth = "16%"
    }

    var JMwidth = "10%";
    if (stagetype == "Opportunity" || stagetype == "Declined" || stagetype == "All" || stagetype == "Complete" || stagetype == "Custom") {
        JMwidth = "11%";
    }

    var UserProfile = $('#profileOfUser').val();
    var data, fromdate, todate;
    var curr = new Date();
    var count = 0;
    var InputId;
    var dateFlag = false;

    if ($("#fromdate").val() != "") {
        //fromdate = $("#fromdate").val();
        fromdate = GetyyyymmddDate($("#fromdate").val());
        InputId = 'fromdate';
        count++;
    }
    if ($("#todate").val() != "") {
        todate = GetyyyymmddDate($("#todate").val());
        InputId = 'todate';
        count++;
    }

    switch (count) {
        case 0:
            var next = new Date();
            // baans change 15th November for date filter
            //var NextWeekDate = parseInt(next.getDate()) - 60;
            var NextWeekDate = parseInt(next.getDate()) - 90;
            next.setDate(NextWeekDate);
            fromdate = GetFormattedDate(next);
            todate = GetFormattedDate(curr);
            dateFlag = true;
            break;

        case 1:
            var ErrField;
            if (InputId == "fromdate") {
                ErrField = "EndDate";
            } else {
                ErrField = "StartDate";
            }

            bootbox.confirm(ErrField + " is not filled!</br>Do you want to see Last 3 months Record?", function (result) {
                if (result) {
                    var next = new Date();
                    //var NextWeekDate = parseInt(next.getDate()) - 60;
                    var NextWeekDate = parseInt(next.getDate()) - 90;
                    // baans end 15th November
                    next.setDate(NextWeekDate);
                    fromdate = GetFormattedDate(next);
                    todate = GetFormattedDate(curr);
                    dateFlag = true;
                } else {
                    dateFlag = false;
                }
            });

            break;

        case 2:
            dateFlag = true;
            break;
    }

    var url = '/Order/GetOrderList';
    var model = { Type: type, Department: department, StartDate: fromdate, EndDate: todate, UserProfile: UserProfile };

    if (type == "Custom") {
        // baans change 26th October for checking the custom data based on search
        var searchdata = $('#searchtextbox').val();
        if (searchdata != "") {
            url = '/Opportunity/GetCustomOppList';
            model = { CustomText: $('#searchtextbox').val(), TableName: 'Vw_tblOpportunity' };
        }
        else {
            CustomWarning("Please put the search criteria in the search box and click on search button");
        }
        // baans end 26th October
    }

    if (dateFlag == true) {
        setfromdate = GetddmmyyyyDate(fromdate);
        settodate = GetddmmyyyyDate(todate);
        $("#fromdate").val(setfromdate);
        $("#todate").val(settodate);

        var data;
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

    }
    function calculateSumsummary() {
        var sumTotal = 0;

        for (var h = 0; h < data.length; h++) {
            if (data[h].Total != null) {
                sumTotal += data[h].Total;
            }
        }

        totalData = { DisplayOpportunityId: "", StageWiseDate: "", OppName: "", Quantity: "", DepartmentName: "", ArtOrderedDate: "Other", ApprovedDate: "Other", ArtReadyDate: "Other", StockOrderedDate: "Other", ReceivedDate: "Other", Checkeddate: "Other", ReqDate: "", Contactfullname: "", EventName: "", StageWiseNotes: "Total Sum", Total: sumTotal, AccountManagerFullName: "" };
        if (stagetype == "Quote" || stagetype == "Lost" || stagetype == "Order" || stagetype == "Cancelled" || stagetype == "Job1") {
            return [totalData];
        }
        else {
            return "";
        }
    }
    var obj =
    {
        selectionModel: { type: 'row' },
        virtualX: true, virtualY: true,
        resizable: false,
        pageModel: { type: "local", rPP: 50 },
        strNoRows: 'No records found for the selected Criteria',
        scrollModel: { autoFit: false },
        //editable: false,
        wrap: false,
        hwrap: false,
        width: "97%",
        height: 750,
        numberCell: { width: 0, title: "", minWidth: 0 },
        columnTemplate: { width: 120, halign: "left" },
        summaryData: calculateSumsummary(),
        editModel: { clicksToEdit: 1, onTab: 'nextEdit', pressToEdit: true, onBlur: 'validate' },
    };

    // 29 April NotesEditing List
    obj.editorEnd = function (event, ui) {
        if (ui.colIndx == 24) {
            var rowIndx = ui.rowIndx;
            var id = ui.rowData.DisplayOpportunityId;
            var stage = ui.rowData.Stage;
            var notes = ui.rowData.StageWiseNotes;

            $.ajax({
                url: '/Opportunity/UpdateStatusNotes/',
                data: { id: id, stage: stage, notes: notes },
                async: true,
                success: function (response) {
                    if (response.Message != null) {
                        //CustomAlert(response);
                        //ui.rowData.StageWiseNotes = notes;
                    }

                },
                type:'Post',
            });
        }
    }
    // 29 April NotesEditing List

    obj.colModel = [
       {
            title: "Job No",  editable: false, width: "5%", dataType: "string", align: "center", dataIndx: "DisplayOpportunityId",
            render: function (ui) {
                var id = ui.cellData;
                var Stage = ui.rowData.Stage;
                // Nikhil change 17th November
                if (Stage == "Quote") {
                    return "<a href='/opportunity/QuoteDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
                }
                else if (Stage == "Order") {
                    return "<a href='/opportunity/OrderDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
                }
                else if (Stage == "Job" || Stage == "Order Confirmed" || Stage == "Job Accepted" || Stage == "Art Ordered" || Stage == "Proof Created" || Stage == "Proof Sent" || Stage == "Proof Approved" || Stage == "Film/Digi Ready" || Stage == "Stock Order" || Stage == "Stock In" || Stage == "Stock Checked") {        //15 Nov 2018 (N)
                    return "<a href='/opportunity/JobDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
                }
                else if (Stage == "Stock Decorated") {         //15 Nov 2018 (N)
                    return "<a href='/opportunity/PackingDetails/" + parseInt(id) + "'><label  class='internalLnk'>" + id + "</label></a>"
                }
                else if (Stage == "Order Shipped" || Stage == "Paid") {     //15 Nov 2018 (N)
                    return "<a href='/opportunity/ShippingDetails/" + parseInt(id) + "'><label  class='internalLnk'>" + id + "</label></a>"
                }
                else if (Stage == "Complete") {
                    return "<a href='/opportunity/CompleteDetails/" + parseInt(id) + "'><label  class='internalLnk'>" + id + "</label></a>"
                }
                else if (Stage == "Order Invoiced" || Stage == "Order Packed") {        //15 Nov 2018 (N)
                    return "<a href='/opportunity/InvoicingDetails/" + parseInt(id) + "'><label  class='internalLnk'>" + id + "</label></a>"
                }
                else {
                    return "<a href='/opportunity/OpportunityDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
                }
                // Nikhil end
                //if (Stage == "Quote") {
                //    return "<a href='/opportunity/QuoteDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
                //}
                //else if (Stage == "Order") {
                //    return "<a href='/opportunity/OrderDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
                //}
                //else if (Stage == "Job") {
                //    return "<a href='/opportunity/JobDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
                //}
                //else if (Stage == "Order Packed" || Stage == "Stock Decorated") {
                //    return "<a href='/opportunity/PackingDetails/" + parseInt(id) + "'><label  class='internalLnk'>" + id + "</label></a>"
                //}
                //else if (Stage == "Order Shipped") {
                //    return "<a href='/opportunity/ShippingDetails/" + parseInt(id) + "'><label  class='internalLnk'>" + id + "</label></a>"
                //}
                //else if (Stage == "Complete") {
                //    return "<a href='/opportunity/CompleteDetails/" + parseInt(id) + "'><label  class='internalLnk'>" + id + "</label></a>"
                //}
                //else if (Stage == "Order Invoiced") {
                //    return "<a href='/opportunity/InvoicingDetails/" + parseInt(id) + "'><label  class='internalLnk'>" + id + "</label></a>"
                //}
                //else {
                //    return "<a href='/opportunity/OpportunityDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
                //}

            }
        },

        {
            title: "Date", editable: false, dataIndx: "StageWiseDate", dataType: "string", align: "right",
            width: "5%"/*stagetype == "All" ? "8%" : stagetype == "Opportunity" ? "8%" : stagetype == "Declined" ? "8%" : stagetype == "Quote" ? "7%" : stagetype == "Lost" ? "7%" : stagetype == "Order" ? "7%" : stagetype == "Cancelled" ? "7%" : stagetype == "Job1" ? "6%" : stagetype == "Complete" ? "8%" : stagetype == "Custom" ? "8%" : "6%"*/,
            hidden: stagetype == "Packing" ? true : stagetype == "Invoiced" ? true : stagetype == "Shipping" ? true : false,
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

        { title: "JobHUB Name", editable: false, width: stagetype == "Job1" ? "8%" : "12%", dataType: "string", dataIndx: "OppName" },

        {
            title: "Qty", editable: false, width: "2%", dataType: "string", align: "right", dataIndx: "Quantity"
        },

        { title: "Department", editable: false, width: stagetype == "Opportunity" ? "15%" : stagetype == "All" ? "15%" : stagetype == "Declined" ? "15%" : stagetype == "Custom" ? "15%" : stagetype == "Packing" ? "15%" : stagetype == "Invoiced" ? "15%" : stagetype == "Shipping" ? "15%" : stagetype == "Complete" ? "15%" : "11%", dataIndx: "DepartmentName", dataType: "string" },
        //17 Nov 2018 (N)

        {
            title: "Req By", editable: false,
            width: "5%"/* stagetype == "Opportunity" ? "8%" : stagetype == "All" ? "8%" : stagetype == "Declined" ? "8%" : stagetype == "Complete" ? "8%" : stagetype == "Custom" ? "8%" : "7%"*/,
            dataType: "string", align: "right", dataIndx: "ReqDate",
            hidden: stagetype == "Job1" ? true : stagetype == "Packing" ? true : stagetype == "Invoiced" ? true : stagetype == "Shipping" ? true : false,
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
            title: "Dep Req By", editable: false,
            width: "7%", dataType: "string", align: "right", dataIndx: "DepositReqDate",
            hidden: stagetype == "Job1" ? true : stagetype == "Opportunity" ? true : stagetype == "Declined" ? true : stagetype == "All" ? true : stagetype == "Custom" ? true : stagetype == "Packing" ? true : stagetype == "Invoiced" ? true : stagetype == "Shipping" ? true : stagetype == "Complete" ? true : false,
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
            title: "Deposit Date", editable: false, width: "7%", dataType: "string", align: "right", dataIndx: "DepositDate",
            hidden: stagetype == "Packing" ? false : stagetype == "Invoiced" ? false : stagetype == "Shipping" ? false : true,
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

        //{
        //    title: "Dispatch Date", width: "7%", dataType: "string", align: "right", dataIndx: "DispDate", hidden: type == "Job" ? true : false,
        //    render: function (ui) {
        //        var date = ui.cellData;
        //        if (date != null && date != undefined && date != "") {
        //            var nowDateopp = new Date(parseInt(date.substr(6)));
        //            // baans change 17th July
        //            //return nowDateopp.getDate() + '/' + (nowDateopp.getMonth() + 1) + '/' + nowDateopp.getFullYear();
        //            return ("0" + nowDateopp.getDate()).slice(-2) + '/' + ("0" + (nowDateopp.getMonth() + 1)).slice(-2) + '/' + nowDateopp.getFullYear();
        //            // baans end 17th July
        //        }
        //    },
        //},
        //{
        //    title: "Planned Date", width: "7%", dataType: "string", align: "right", dataIndx: "PlannedDate", hidden: type == "Job" ? true : false,
        //    render: function (ui) {
        //        var date = ui.cellData;
        //        if (date != null && date != undefined && date != "") {
        //            var nowDateopp = new Date(parseInt(date.substr(6)));
        //            // baans change 17th July
        //            //return nowDateopp.getDate() + '/' + (nowDateopp.getMonth() + 1) + '/' + nowDateopp.getFullYear();
        //            return ("0" + nowDateopp.getDate()).slice(-2) + '/' + ("0" + (nowDateopp.getMonth() + 1)).slice(-2) + '/' + nowDateopp.getFullYear();
        //            // baans end 17th July
        //        }
        //    },
        //},
        {
            title: "Confirmed", editable: false, width: "6%", dataType: "string", align: "right", dataIndx: "ConfirmedDate",
            hidden: stagetype == "Job1" ? false : stagetype == "Packing" ? false : stagetype == "Invoiced" ? false : stagetype == "Shipping" ? false : true,
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
            title: "Proof Sent", editable: false, dataIndx: "ArtOrderedDate", width: "4%", dataType: "string", hidden: stagetype == "Job1" ? false : true, align: "center",
            render: function (ui) {
                var data = ui.cellData;
                if (data != null && data != undefined && data != "Other") {
                    return "<label class='lblstyle' style='background:green;color:white !important;padding: 2px 7px 1px 7px;'>Yes</label>"
                }
                else if (data == "Other") {
                    return " "
                }
                else {
                    return "<label class='lblstyle' style='background:Red;color:white !important;padding: 1px 8px 1px 8px;'>No</label>"
                }
            }
        },
        {
            title: "Proof App", editable: false, dataIndx: "ApprovedDate", width: "5%", dataType: "string", hidden: stagetype == "Job1" ? false : true, align: "center",
            render: function (ui) {
                var data = ui.cellData;
                if (data != null && data != undefined && data != "Other") {
                    return "<label class='lblstyle' style='background:green;color:white !important;padding: 2px 7px 1px 7px;'>Yes</label>"
                }
                else if (data == "Other") {
                    return " "
                }
                else {
                    return "<label class='lblstyle' style='background:Red;color:white !important;padding: 1px 8px 1px 8px;'>No</label>"
                }
            }
        },
        {
            title: "Film/Digi", editable: false, dataIndx: "ArtReadyDate", width: "5%", dataType: "string", hidden: stagetype == "Job1" ? false : true, align: "center",
            render: function (ui) {
                var data = ui.cellData;
                if (data != null && data != undefined && data != "Other") {
                    return "<label class='lblstyle' style='background:green;color:white !important;padding: 2px 7px 1px 7px;'>Yes</label>"
                }
                else if (data == "Other") {
                    return " "
                }
                else {
                    return "<label class='lblstyle' style='background:Red;color:white !important;padding: 1px 8px 1px 8px;'>No</label>"
                }
            }
        },
        {
            title: "Stk Ord", editable: false, dataIndx: "StockOrderedDate", width: "5%", dataType: "string", hidden: stagetype == "Job1" ? false : true, align: "center",
            render: function (ui) {
                var data = ui.cellData;
                if (data != null && data != undefined && data != "Other") {
                    return "<label class='lblstyle' style='background:green;color:white !important;padding: 2px 7px 1px 7px;'>Yes</label>"
                }
                else if (data == "Other") {
                    return " "
                }
                else {
                    return "<label class='lblstyle' style='background:Red;color:white !important;padding: 1px 8px 1px 8px;'>No</label>"
                }
            }
        },
        {
            title: "Stk In", editable: false, dataIndx: "ReceivedDate", width: "4%", dataType: "string", hidden: stagetype == "Job1" ? false : true, align: "center",
            render: function (ui) {
                var data = ui.cellData;
                if (data != null && data != undefined && data != "Other") {
                    return "<label class='lblstyle' style='background:green;color:white !important;padding: 2px 7px 1px 7px;'>Yes</label>"
                }
                else if (data == "Other") {
                    return " "
                }
                else {
                    return "<label class='lblstyle' style='background:Red;color:white !important;padding: 1px 8px 1px 8px;'>No</label>"
                }
            }
        },
        {
            title: "Stk OK", editable: false, dataIndx: "Checkeddate", width: "4%", dataType: "string", hidden: stagetype == "Job1" ? false : true, align: "center",
            render: function (ui) {
                var data = ui.cellData;
                if (data != null && data != undefined && data != "Other") {
                    return "<label class='lblstyle' style='background:green;color:white !important;padding: 2px 7px 1px 7px;'>Yes</label>"
                }
                else if (data == "Other") {
                    return " "
                }
                else {
                    return "<label class='lblstyle' style='background:Red;color:white !important;padding: 1px 8px 1px 8px;'>No</label>"
                }
            }
        },

        {
            title: "Shipping", editable: false, dataIndx: "Shipping", width: "11%", dataType: "string", hidden: stagetype == "Packing" ? false : true,
        },

        {
            title: "Shipping To", editable: false, dataIndx: "ShippingTo", width: "11%", dataType: "string", hidden: stagetype == "Packing" ? false : true,
        },

        {
            title: "Deposit", editable: false, dataIndx: "Totalpayment", width: "11%", dataType: "string", hidden: stagetype == "Invoiced" ? false : true,
        },

        {
            title: "Balance", editable: false, dataIndx: "TotalBalance", width: "11%", dataType: "string", hidden: stagetype == "Invoiced" ? false : true,
        },

        {
            title: "Packed In", editable: false, dataIndx: "PackedInSet1", width: "11%", dataType: "string", hidden: stagetype == "Shipping" ? false : true,
        },

        {
            title: "Packed In", editable: false, dataIndx: "PackedInSet2", width: "11%", dataType: "string", hidden: stagetype == "Shipping" ? false : true,
        },

        {
            title: "Contact", editable: false, dataIndx: "Contactfullname", width: "10%", dataType: "string",
            hidden: stagetype == "Opportunity" ? false : stagetype == "All" ? false : stagetype == "Declined" ? false : stagetype == "Custom" ? false : stagetype == "Quote" ? false : stagetype == "Order" ? false : stagetype == "Lost" ? false : stagetype == "Cancelled" ? false : stagetype == "Complete" ? false : true,
        },

        { title: "Event", editable: false, dataIndx: "EventName", width: "12%", dataType: "string", hidden: stagetype == "Opportunity" ? false : stagetype == "Declined" ? false : stagetype == "Complete" ? false : stagetype == "Custom" ? false : true, },

        {
            title: "Status", editable: false, dataIndx: "Stage", width: "12%", dataType: "string", hidden: stagetype == "All" ? false : true,
        },

        {
            title: "Notes", editable: true, width: jwidth, dataType: "string", dataIndx: "StageWiseNotes"
        },

        {
            title: "Value", editable: false, width: "6%", dataType: "string", dataIndx: "Total", align: "right",
            hidden: tfcondition,
            format: "#,###.00",
        },

        {
            title: "Margin", editable: false, width: "5%", dataType: "string", dataIndx: "GP", align: "right",
            hidden: tfcondition,
            format: "###.00%"
        },

        {
            title: "Job Manager", editable: false, width: JMwidth, dataType: "string", dataIndx: "AccountManagerFullName"
        }
        //17 Nov 2018 (N)
    ];

    //29 April Stage Change List
    obj.rowDblClick = function (evt, ui) {
        var rowIndx = getRowIndx(this);
        if (rowIndx != null) {
            var dataOpp = this.pdata[rowIndx];
            dataOpp = data[rowIndx];
            //alert(dataOpp.OpportunityId);
            var OppId = dataOpp.OpportunityId;
            $("#HiddenOppIdInList").val(OppId);
            if (OppId != undefined && OppId != 0) {

                $("#lblListJobStageData").text("");
                ResetRightSideBar(null);
                $.ajax({
                    url: '/Opportunity/GetOppById',
                    data: { OppId: OppId },
                    async: false,
                    success: function (response) {
                        $("#lblListJobStageData").text(response.Stage);
                        $("#lblOrderConfirmDate").text(DateTimeFormat(response.OrderConfirmedDate));
                        $('#lblArtOrderdDate').text(DateTimeFormat(response.ArtOrderedDate));
                        $('#lblProofApprovedDate').text(DateTimeFormat(response.ApprovedDate));
                        $('#lblDigiReadyDate').text(DateTimeFormat(response.ArtReadyDate));
                        $('#lblStockOrderedDate').text(DateTimeFormat(response.StockOrderedDate));
                        $('#lblStockInDate').text(DateTimeFormat(response.ReceivedDate));
                        $('#lblStockCheckedDate').text(DateTimeFormat(response.Checkeddate));
                        if (response.OppThumbnail != null && response.OppThumbnail != undefined && response.OppThumbnail != "") {
                            $('#imageOppThumbnail').attr('src', '/Content/uploads/Opportunity/' + response.OppThumbnail)
                        }
                        else {
                            $('#imageOppThumbnail').attr('src', '/Content/uploads/Opportunity/NoImage.png')
                        }
                        SetOppoStage();

                    },

                    error: function (response) {
                    },
                    type: 'post'
                });
            }
        }
    }
    //29 April Stage Change List

    var LstData;
    obj.dataModel = { data: data };
    if (tabType == "UpperTab") {
        pq.grid("#OrderGridNew", obj);
        pq.grid("#OrderGridNew", "refreshDataAndView");
    } else {
        pq.grid("#OrderGrid", obj);
        pq.grid("#OrderGrid", "refreshDataAndView");
    }
}

function CustomAlert(response) {
    if (response.Result == "Success") {
        if (response.tblName != "" && response.tblName!=null)
            response.Message = response.tblName + " saved successfully";

        $('#CustomAlert').removeClass('j-alertBox-Failed').removeClass('j-alertBox-Warning').css('display', 'none');
        $('#CustomAlert').addClass('j-alertBox-Success').css('display', 'block');
        $('#alertMsg').html('<strong>Success ! </strong>' + response.Message);
        setTimeout(function () {
            $('#CustomAlert').fadeOut('slow', function () {
                $('#CustomAlert').removeClass('j-alertBox-Success').css('display', 'none');
                $('#alertMsg').html('');

            });
        }, 5000);
    }
    else if (response.Result == "Warning") {
        $('#CustomAlert').removeClass('j-alertBox-Failed').removeClass('j-alertBox-Success').css('display', 'none');
        $('#CustomAlert').addClass('j-alertBox-Warning').css('display', 'block');
        $('#alertMsg').html('<strong>Warning ! </strong>' + response.Message);
        setTimeout(function () {
            $('#CustomAlert').fadeOut('slow', function () {
                $('#CustomAlert').removeClass('j-alertBox-Warning').css('display', 'none');
                $('#alertMsg').html('');

            });
        }, 5000);
    }
    else {
        if (response.tblName != "" && response.ErrorCode == -2147467259)
        {
            response.Message =  response.tblName + " with this name already exists";
        }

        $('#CustomAlert').removeClass('j-alertBox-Warning').removeClass('j-alertBox-Success').css('display', 'none');
        $('#CustomAlert').addClass('j-alertBox-Failed').css('display', 'block');
        $('#alertMsg').html('<strong>Error ! </strong>' + response.Message);
        setTimeout(function () {
            $('#CustomAlert').fadeOut('slow', function () {
                $('#CustomAlert').removeClass('j-alertBox-Failed').css('display', 'none');
                $('#alertMsg').html('');

            });
        }, 5000);
    }
}
function CustomWarning(Message) {

    $('#CustomAlert').removeClass('j-alertBox-Failed').removeClass('j-alertBox-Success').css('display', 'none');
    $('#CustomAlert').addClass('j-alertBox-Warning').css('display', 'block');
    $('#alertMsg').html('<strong>Warning ! </strong>' + Message);
    setTimeout(function () {
        $('#CustomAlert').fadeOut('slow', function () {
            $('#CustomAlert').removeClass('j-alertBox-Warning').css('display', 'none');
            $('#alertMsg').html('');

        });
    }, 5000);
}
function CustomError(Message) {
    $('#CustomAlert').removeClass('j-alertBox-Warning').removeClass('j-alertBox-Success').css('display', 'none');
    $('#CustomAlert').addClass('j-alertBox-Failed').css('display', 'block');
    $('#alertMsg').html('<strong>Error ! </strong>' + Message);
    setTimeout(function () {
        $('#CustomAlert').fadeOut('slow', function () {
            $('#CustomAlert').removeClass('j-alertBox-Failed').css('display', 'none');
            $('#alertMsg').html('');

        });
    }, 5000);
}

// baans change 10th October for CustomWariningCode
function CustomErrorCode(Warning) {
    var Message = "";
    //if (ErrorCode == 100) {
    //    Message = "Fill All Required Fields";
    //}
    switch (Warning) {
        case "Required":
            Message = "Fill All Required Fields.";
            break;

        default:
            Message = "";
            break;
    }
    $('#CustomAlert').removeClass('j-alertBox-Failed').removeClass('j-alertBox-Success').css('display', 'none');
    $('#CustomAlert').addClass('j-alertBox-Warning').css('display', 'block');
    $('#alertMsg').html('<strong>Warning ! </strong>' + Message);
    setTimeout(function () {
        $('#CustomAlert').fadeOut('slow', function () {
            $('#CustomAlert').removeClass('j-alertBox-Warning').css('display', 'none');
            $('#alertMsg').html('');

        });
    }, 5000);
}
// baans end 10th October
function SetOppoStage() {
    var color = "#ed1c24";
    if ($('#lblOppDate').text() != "") {
        $("#btnOppoStage").css("border-color", color);
    }
    if ($('#lblQuoteDate').text() != "") {
        $("#btnQuoteStage").css("border-color", color);
    }   
    if ($('#lblOrderDate').text() != "") {
        if ($('#PageName').val() == "Opportunity" || $('#PageName').val() == "QuoteDetails")
        {
            $('#btnOptionSave,#btnOptionCopy').css("display", "none");
            $('#btnOptionSaveBlank,#btnOptionCopyBlank').css("display", "initial");//Added by baans 07Sep2020

        }
        $("#btnOrderStage").css("border-color", color);
    }
    var dd = $('#lblJobDate').text();
    if ($('#lblJobDate').text() != "") {
        $("#btnJobStage").css("border-color", color);
    }

    if ($('#lblOrderConfirmDate').text() != "") {
        $("#btnOrderConfirmedStage").css("border-color", color);
    }
    if ($('#lblArtOrderdDate').text() != "") {
        $("#btnArtOrderedStage").css("border-color", color);
    }
    if ($('#lblProofApprovedDate').text() != "") {
        $("#btnProofApprovedStage").css("border-color", color);
    }
    if ($('#lblDigiReadyDate').text() != "") {
        $("#btnDigiReadyStage").css("border-color", color);
    }

    if ($('#lblStockOrderedDate').text() != "") {
        $("#btnStockOrderStage").css("border-color", color);
    }
    if ($('#lblStockInDate').text() != "") {
        $("#btnStockInStage").css("border-color", color);
    }
    if ($('#lblStockCheckedDate').text() != "") {
        $("#btnStockCheckedStage").css("border-color", color);
    }
    if ($('#lblStockDecoratedDate').text() != "") {
        $("#btnStockDecoratedStage").css("border-color", color);
    }
    if ($('#lblOrderPackedDate').text() != "") {
        $("#btnOrderPackedStage").css("border-color", color);
    }
    if ($('#lblInvoiceDate').text() != "") {
        $("#btnOrderInvoicedStage").css("border-color", color);
    }
    if ($('#lblPaidDate').text() != "") {
        $("#btnPaidStage").css("border-color", color);
    }
    if ($('#lblShippedDate').text() != "") {
        $("#btnOrderShippedStage").css("border-color", color);
    }
    if ($('#lblCompleteDate').text() != "") {
        $("#btnCompleteStage").css("border-color", color);
    }
    if ($('#lblJobAcceptedDate').text() != "") {
        $("#btnJobAccepted").css("border-color", color);
    }
    if ($('#lblProofCreatedDate').text() != "") {
        $("#btnProofCreated").css("border-color", color);
    }
    if ($('#lblProofSentDate').text() != "") {
        $("#btnProofSent").css("border-color", color);
    }
}

function ReSetOppoStage(Stage) {   
    var color = "white"; 
    switch (Stage) {
        case "Quote": {
            if ($('#lblQuoteDate').text() != "") {
                $('#lblQuoteDate').html('');
                $("#btnQuoteStage").css("border-color", color);
            }
            break;
        }
        case "Order": {
            if ($('#lblOrderDate').text() != "") {
                $('#lblOrderDate').html('');
                $("#btnOrderStage").css("border-color", color);
            }
            break;
        }
        case "Job": {
            if ($('#lblJobDate').text() != "") {
                $('#lblJobDate').html('');
                $("#btnJobStage").css("border-color", color);
            }
            break;
        }
        case "Order Confirmed":
            {
                if ($('#lblOrderConfirmDate').text() != "") {
                    $('#lblOrderConfirmDate').html('');
                    $("#btnOrderConfirmedStage").css("border-color", color);
                }
                break;
            }
        
        case "Job Accepted":
            {
                if ($('#lblJobAcceptedDate').text() != "") {
                    $('#lblJobAcceptedDate').html('');
                    $("#btnJobAccepted").css("border-color", color);
                }
                break;
            }
        case "Art Ordered":
            {
                if ($('#lblArtOrderdDate').text() != "") {
                    $('#lblArtOrderdDate').html('');
                    $("#btnArtOrderedStage").css("border-color", color);
                }
                break;
            }
        case "Proof Created":
            {
                if ($('#lblProofCreatedDate').text() != "") {
                    $('#lblProofCreatedDate').html('');
                    $("#btnProofCreated").css("border-color", color);
                }
                break;
            }
        case "Proof Sent":
            {
                if ($('#lblProofSentDate').text() != "") {
                    $('#lblProofSentDate').html('');
                    $("#btnProofSent").css("border-color", color);
                }
                break;
            }
        case "Proof Approved":
            {
                if ($('#lblProofApprovedDate').text() != "") {
                    $('#lblProofApprovedDate').html('');
                    $("#btnProofApprovedStage").css("border-color", color);
                }
                break;
            }
        case "Film/Digi Ready":
            {
                if ($('#lblDigiReadyDate').text() != "") {
                    $('#lblDigiReadyDate').html('');
                    $("#btnDigiReadyStage").css("border-color", color);
                }
                break;
            }
        case "Stock Ordered":
            {
                if ($('#lblStockOrderedDate').text() != "") {
                    $('#lblStockOrderedDate').html('');
                    $("#btnStockOrderStage").css("border-color", color);
                }
                break;
            }
        case "Stock In":
            {
                if ($('#lblStockInDate').text() != "") {
                    $('#lblStockInDate').html('');
                    $("#btnStockInStage").css("border-color", color);
                }
                break;
            }
        case "Stock Checked":
            {
                if ($('#lblStockCheckedDate').text() != "") {
                    $('#lblStockCheckedDate').html('');
                    $("#btnStockCheckedStage").css("border-color", color);
                }
                break;
            }

        case "Stock Decorated":
            {
                if ($('#lblStockDecoratedDate').text() != "") {
                    $('#lblStockDecoratedDate').html('');
                    $("#btnStockDecoratedStage").css("border-color", color);
                }
                break;
            }
        case "Order Packed":
            {
                if ($('#lblOrderPackedDate').text() != "") {
                    $('#lblOrderPackedDate').html('');
                    $("#btnOrderPackedStage").css("border-color", color);
                }
                break;
            }
        case "Order Invoiced":
            {
                if ($('#lblInvoiceDate').text() != "") {
                    $('#lblInvoiceDate').html('');
                    $("#btnOrderInvoicedStage").css("border-color", color);
                }
                break;
            }
        case "Paid":
            {
                if ($('#lblPaidDate').text() != "") {
                    $('#lblPaidDate').html('');
                    $("#btnPaidStage").css("border-color", color);
                }
                break;
            }
        case "Order Shipped":
            {
                if ($('#lblShippedDate').text() != "") {
                    $('#lblShippedDate').html('');
                    $("#btnOrderShippedStage").css("border-color", color);
                }
                break;
            }
        case "Complete":
            {
                if ($('#lblCompleteDate').text() != "") {
                    $('#lblCompleteDate').html('');
                    $("#btnCompleteStage").css("border-color", color);
                }
                break;
            }
    }
    
}

function ResetOppStagePopUp(stage, updateURL) {
    bootbox.confirm("Are you sure you want to Revert this Stage?", function (result) {
        if (result) {

            $.ajax({
                url: '/Opportunity/ResetStageByOppoID',
                data: { oppId: $('#lblOpportunityId').text(), stage: stage },
                async: false,
                success: function (response) {
                    var data = response.data;
                    if (data.response.Result == "Success") {
                        ReSetOppoStage(stage);
                        CustomAlert(data.response);
                        if (updateURL != "") {
                            setTimeout(function () {
                                location.href = "/Opportunity/" + updateURL + "/" + parseInt($('#lblOpportunityId').text());

                            }, 1000);
                        }                       

                    }
                    else {
                        CustomAlert(data.response);
                    }
                }
            });

        }
    });
}

function ChangeStage(UpdateURL, Stage) {
    if (UpdateURL == '') {
        $.ajax({
            url: '/Opportunity/GetOppById',
            data: { OppId: $('#lblOpportunityId').text() },
            async: false,
            success: function (response) {
                CmfrmDate = DateFormat(response.ConfirmedDate);
            },
            error: function (response) {
            },
            type: 'post'

        });
    }
    else {
        $.ajax({
            url: '/Opportunity/ChangeStageByOppoID',
            data: { OppId: $('#lblOpportunityId').text(), Stage: Stage },
            async: false,
            success: function (response) {
                var data = response.data;
                if (data.response.Result == "Success") {
                    location.href = "/Opportunity/" + UpdateURL + "/" + parseInt($('#lblOpportunityId').text());
                }
                else {
                    CustomAlert(data.response);
                }
            }
        });
    }
}


function ChangeStageByOppoID(Stage, UpdateURL, StageID) {
    var revertmessage = "You can not Revert this Stage First you have to Revert Last Stage";
    
    if ($('#lblOpportunityId').val() != "000000") {
        if (Stage == "Opportunity") {
            if ($("#lblQuoteDate").text() == "") {
                location.href = "/Opportunity/" + UpdateURL + "/" + parseInt($('#lblOpportunityId').text()); 
            }
            CustomWarning(revertmessage);
        }
        if (Stage == "Quote") {
            if ($('#lblQuoteDate').text() == "") {
                var ConfirmMessage = "";
                var Isvalid = QuoteValidCheck();
                if (Isvalid.resultOrg) {
                    ConfirmMessage = 'Are you sure you want to Continue?';
                }
                //else  {
                //    ConfirmMessage=  "Organisation is not associated with contact. Are you sure want to Continue?"
                //}
                else if (Isvalid.result == true && Isvalid.resultOrg == false) {
                    ConfirmMessage = "Organisation is not associated with Primary Contact. Are you sure you want to Continue?"
                }
                else {

                    ConfirmMessage = "Opportunity dont have an Primary Contact linked with it. Are you sure you want to Continue?"
                }

                bootbox.confirm(ConfirmMessage, function (result) {
                    if (result) {
                        $.ajax({
                            url: '/Opportunity/ChangeStageByOppoID',
                            data: { OppId: $('#lblOpportunityId').text(), Stage: Stage },
                            async: false,
                            success: function (response) {
                                var data = response.data;
                                if (data.response.Result == "Success") {
                                    location.href = "/Opportunity/" + UpdateURL + "/" + parseInt($('#lblOpportunityId').text());
                                }
                                else {
                                    CustomAlert(data.response);
                                }
                            }
                        });

                    }
                });




            }
            else {
                if ($('#lblOppDate').text() != "" && $('#lblOrderDate').text() == "") {
               
                    ResetOppStagePopUp(Stage, "OpportunityDetails");

                }
                else {
                    CustomWarning(revertmessage);
                }
            }
        }
        if (Stage == "Order") {

            var Oppid = $('#lblOpportunityId').text();
            $.ajax({
                url: '/Opportunity/GetOptionStatus',
                data: { Oppid: Oppid },
                async: false,

                success: function (response) {
                    if (response == true) {
                        if ($('#lblQuoteDate').text() != "") {

                            if ($('#lblOrderDate').text() == "") {
                                var Isvalid = QuoteValidCheck();
                                // baans change 23rd August for Change in Confirmation Message
                                if (Isvalid.RequiredData) {
                                    //if (Isvalid.result) {
                                    if (Isvalid.resultOrg) {
                                        // baans end 23rd August
                                        bootbox.confirm("Are you sure you want to Continue?", function (result) {
                                            if (result) {
                                                $.ajax({
                                                    url: '/Opportunity/ChangeStageByOppoID',
                                                    data: { OppId: $('#lblOpportunityId').text(), Stage: Stage },
                                                    async: false,
                                                    success: function (response) {
                                                        var data = response.data;
                                                        if (data.response.Result == "Success") {
                                                            location.href = "/Opportunity/" + UpdateURL + "/" + parseInt($('#lblOpportunityId').text());
                                                        }
                                                        else {
                                                            CustomAlert(data.response);
                                                        }
                                                    }
                                                });

                                            }
                                        });
                                    }
                                    else if (Isvalid.result == false) {
                                        CustomWarning('Primary Contact should be link with Opportunity');
                                    }
                                    else {
                                        CustomWarning('Organisation should be link with Primary Contact of the Opportunity');
                                    }
                                }
                                else {
                                    //24 Oct 2018 (N)
                                    if ($('#depositreqdate').val() == "") {
                                        $('#depositreqdate').addClass("customAlertChange");
                                    }
                                    else {
                                        $('#depositreqdate').removeClass("customAlertChange");
                                    }

                                    if ($('#OppShipping').val() == "") {
                                        $('#OppShipping').addClass("customAlertChange");
                                    }
                                    else {
                                        $('#OppShipping').removeClass("customAlertChange");
                                    }

                                    if ($('#txtshippingday').val() == "") {
                                        $('#txtshippingday').addClass("customAlertChange");
                                    }
                                    else {
                                        $('#txtshippingday').removeClass("customAlertChange");
                                    }

                                    if ($('#txtshippingprice').val() == "") {
                                        $('#txtshippingprice').addClass("customAlertChange");
                                    }
                                    else {
                                        $('#txtshippingprice').removeClass("customAlertChange");
                                    }
                                    //24 Oct 2018 (N)
                                    CustomError('Please Fill All Required Fields Marked with *');
                                }
                            }

                            else {
                                if ($('#lblOrderDate').text() != "" && $('#lblJobDate').text() == "") {
                                
                                    ResetOppStagePopUp(Stage, "QuoteDetails");
                                }
                                else {
                                    CustomWarning(revertmessage);
                                }
                            }
                        }
                        else {
                            CustomWarning('You can not make order before quote');
                        }
                    }
                    else {
                        CustomError('You can not move to order without any existing option.');
                    }
                }
            });

        }
        if (Stage == "Job") {
            if ($('#lblOrderDate').text() != "") {
                if ($('#lblJobDate').text() == "") {
                    // baans change 30th October for TBC
                    var OppIdtbc = $('#lblOpportunityId').text();
                    $.ajax({
                        url: '/Opportunity/CheckTbcSize',
                        data: { OppId: OppIdtbc },
                        async: false,
                        success: function (response) {
                            if (response == true) {
                                // baans change 24th October for checking the partial payment
                                var CurrentOppId = $('#lblOpportunityId').text();
                                $.ajax({
                                    url: '/Opportunity/CheckPaymentForOpportunity',
                                    data: { OppId: CurrentOppId },
                                    async: false,
                                    success: function (response) {
                                        if (response == true) {
                                            bootbox.confirm("Are you sure you want to Continue?", function (result) {
                                                if (result) {
                                                    $.ajax({
                                                        url: '/Opportunity/ChangeStageByOppoID',
                                                        data: { OppId: $('#lblOpportunityId').text(), Stage: Stage },
                                                        async: false,
                                                        success: function (response) {
                                                            var data = response.data;
                                                            if (data.response.Result == "Success") {
                                                                location.href = "/Opportunity/" + UpdateURL + "/" + parseInt($('#lblOpportunityId').text());
                                                            }
                                                            else {
                                                                CustomAlert(data.response);
                                                            }
                                                        }
                                                    });

                                                }
                                            });
                                            // baans change 2nd November for checking the Delivery Address
                                            //var OpporId = $('#lblOpportunityId').text();
                                            //$.ajax({
                                            //    url: '/Opportunity/CheckDeliveryAddressByOppId',
                                            //    data: { OppId: OpporId },
                                            //    async: false,
                                            //    success: function (response) {
                                            //        if (response == true) {
                                            //            bootbox.confirm("Are you sure you want to Continue?", function (result) {
                                            //                if (result) {
                                            //                    $.ajax({
                                            //                        url: '/Opportunity/ChangeStageByOppoID',
                                            //                        data: { OppId: $('#lblOpportunityId').text(), Stage: Stage },
                                            //                        async: false,
                                            //                        success: function (response) {
                                            //                            var data = response.data;
                                            //                            if (data.response.Result == "Success") {
                                            //                                location.href = "/Opportunity/" + UpdateURL + "/" + parseInt($('#lblOpportunityId').text());
                                            //                            }
                                            //                            else {
                                            //                                CustomAlert(data.response);
                                            //                            }
                                            //                        }
                                            //                    });

                                            //                }
                                            //            });
                                            //        }
                                            //        //else {
                                            //        //    bootbox.confirm("There is still no delivery address recorded for this Job. Are you sure you want to Continue?", function (result) {
                                            //        //        if (result) {
                                            //        //            $.ajax({
                                            //        //                url: '/Opportunity/ChangeStageByOppoID',
                                            //        //                data: { OppId: $('#lblOpportunityId').text(), Stage: Stage },
                                            //        //                async: false,
                                            //        //                success: function (response) {
                                            //        //                    var data = response.data;
                                            //        //                    if (data.response.Result == "Success") {
                                            //        //                        location.href = "/Opportunity/" + UpdateURL + "/" + parseInt($('#lblOpportunityId').text());
                                            //        //                    }
                                            //        //                    else {
                                            //        //                        CustomAlert(data.response);
                                            //        //                    }
                                            //        //                }
                                            //        //            });

                                            //        //        }
                                            //        //    });
                                            //        //}
                                            //    }
                                            //});
                                            //bootbox.confirm("Are you sure you want to Continue?", function (result) {
                                            //    if (result) {
                                            //        $.ajax({
                                            //            url: '/Opportunity/ChangeStageByOppoID',
                                            //            data: { OppId: $('#lblOpportunityId').text(), Stage: Stage },
                                            //            async: false,
                                            //            success: function (response) {
                                            //                var data = response;
                                            //                if (data.response.Result == "Success") {
                                            //                    location.href = "/Opportunity/" + UpdateURL + "/" + parseInt($('#lblOpportunityId').text());
                                            //                }
                                            //                else {
                                            //                    CustomAlert(data.response);
                                            //                }
                                            //            }
                                            //        });

                                            //    }
                                            //});
                                        }
                                        else {
                                            CustomWarning("No Payment has been made, so cannot convert to a Job!!!");
                                        }
                                    }
                                });
                            }
                            else {
                                CustomWarning("The Current order has Sizes set as TBC. Please go to Sizes Option and fill in the Appropriate size.");
                            }
                        }
                    });




                    //bootbox.confirm("Are you sure you want to Continue?", function (result) {
                    //    if (result) {
                    //        $.ajax({
                    //            url: '/Opportunity/ChangeStageByOppoID',
                    //            data: { OppId: $('#lblOpportunityId').text(), Stage: Stage },
                    //            async: false,
                    //            success: function (response) {
                    //                var data = response;
                    //                if (data.response.Result == "Success") {
                    //                    location.href = "/Opportunity/" + UpdateURL + "/" + parseInt($('#lblOpportunityId').text());
                    //                }
                    //                else {
                    //                    CustomAlert(data.response);
                    //                }
                    //            }
                    //        });

                    //    }
                    //});
                    // baans end 24th October
                }
                else {
                    if ($('#lblJobDate').text() != "" && $('#lblOrderConfirmDate').text() == "") {

                        ResetOppStagePopUp(Stage, "OrderDetails");
                    }
                    else {
                        CustomWarning(revertmessage);
                    }


                }
            }
            else {
                CustomWarning('You can not make job before order');
            }

        }
        if (Stage == "Order Confirmed") {

            if ($('#lblOrderConfirmDate').text() == "") {
                if ($('#lblJobDate').text() != "") {
                    var CmfrmDate = "";
                    var Isconfirmed = true;
                    $.ajax({
                        url: '/Opportunity/GetOppById',
                        data: { OppId: $('#lblOpportunityId').text() },
                        async: false,
                        success: function (response) {
                            CmfrmDate = DateFormat(response.ConfirmedDate);
                        },
                        error: function (response) {
                        },
                        type: 'post'

                    });
                    if ($('#txtConfirmedDate').val() != CmfrmDate)
                        Isconfirmed = false;

                    if ($('#txtConfirmedDate').val() != "" && Isconfirmed) {
                        $("#txtConfirmedDate").removeClass('customAlertChange');
                        // baans change 2nd November for checking the Delivery Address
                        var OpporId = $('#lblOpportunityId').text();
                        $.ajax({
                            url: '/Opportunity/CheckDeliveryAddressByOppId',
                            data: { OppId: OpporId },
                            async: false,
                            success: function (response) {
                                if (response == true) {
                                    bootbox.confirm("Are you sure you want to confirm?", function (result) {
                                        if (result) {
                                            openOrderEmailModal();

                                        }
                                    });
                                }
                                else {
                                    bootbox.confirm("There is still no delivery address recorded for this Job. Are you sure you want to Continue?", function (result) {
                                        if (result) {
                                            openOrderEmailModal();
                                        }
                                    });
                                }
                            }
                        });


                    }
                    else {
                        CustomWarning('Confirm date should be saved on job details before comfirming order.');
                        $("#txtConfirmedDate").addClass('customAlertChange');
                    }
                }

                else {
                    CustomWarning('You cannot confirm order before job');
                }
            }
            else {
                if ($('#lblJobDate').text() != "" && $('#lblJobAcceptedDate').text() == "") {
                
                    ResetOppStagePopUp(Stage, "");

                }
                else {
                    CustomWarning(revertmessage);
                }



            }
        }

        if (Stage == "Art Ordered") {
            // 29 April Stage Change List (if start)
            if ($("#HiddenCallingSource").val() == "OpportunityList") {
                var OppId = $("#HiddenOppIdInList").val();
                $.ajax({
                    url: '/Opportunity/GetJobStatusByOppId',
                    data: { OppId: OppId, lblDate: "OrderConfirmedDate" },
                    async: false,
                    success: function (response) {
                        if (response == true) {
                            SaveStageByOppoID(Stage, 'lblArtOrderdDate');
                        }
                        else {
                            CustomWarning('You can not order art before order confirm');
                        }
                    }
                });
            }
            else {
                // 29 April Stage Change List (else for if)
                if ($('#lblOrderConfirmDate').text() != "") {
                    if ($('#lblArtOrderdDate').text() == "") {
                        SaveStageByOppoID(Stage, 'lblArtOrderdDate');
                    }
                    else {
                        if ($('#lblArtOrderdDate').text() != "" && $('#lblProofCreatedDate').text() == "") {

                            ResetOppStagePopUp(Stage, "");

                        }
                        else {
                            CustomWarning(revertmessage);
                        }
                    }
                }
                else {
                    CustomWarning('You can not order art before order confirm');
                }
            }// 29 April Stage Change List (else end)
        }
        if (Stage == "Proof Approved") {
            if ($('#lblArtOrderdDate').text() != "") {
                if ($('#lblProofApprovedDate').text() == "") {
                    SaveStageByOppoID(Stage, 'lblProofApprovedDate');
                }
                else {
                    if ($('#lblProofSentDate').text() != "" && $('#lblDigiReadyDate').text() == "") {

                        ResetOppStagePopUp(Stage, "");

                    }
                    else {
                        CustomWarning(revertmessage);
                    }
                }
            }
            else {
                CustomWarning('You can not apporove art before art order');
            }
        }
        if (Stage == "Film/Digi Ready") {
            if ($('#lblProofApprovedDate').text() != "") {
                if ($('#lblDigiReadyDate').text() == "") {
                    SaveStageByOppoID(Stage, 'lblDigiReadyDate');
                }
                else {
                    if ($('#lblProofApprovedDate').text() != "" && $('#lblStockOrderedDate').text() == "") {

                        ResetOppStagePopUp(Stage, "");

                    }
                    else {
                        CustomWarning(revertmessage);
                    }
                }
            }
            else {
                CustomWarning('You can not art ready before art approve');
            }
        }
        if (Stage == "Stock Ordered") {
            // 29 April Stage Change List (if start)
            //if ($("#HiddenCallingSource").val() == "OpportunityList") {
            //    var OppId = $("#HiddenOppIdInList").val();
            //    $.ajax({
            //        url: '/Opportunity/GetJobStatusByOppId',
            //        data: { OppId: OppId, lblDate: "JobDate" },
            //        async: false,
            //        success: function (response) {
            //            if (response == true) {
            //                SaveStageByOppoID(Stage, 'lblStockOrderedDate');
            //            }
            //            else {
            //                CustomWarning('You can not order stock before order confirm or job');
            //            }
            //        }
            //    });
            //}
            //else {
            //    // 29 April Stage Change List (else for if)
            //    if ($('#lblOrderConfirmDate').text() != "" || $('#lblJobDate').text() != "") {
            //        if ($('#lblStockOrderedDate').text() == "") {
            //            SaveStageByOppoID(Stage, 'lblStockOrderedDate');
            //        }
            //    }
            //    else {
            //        CustomWarning('You can not order stock before Job');
            //    }
            //}// 29 April Stage Change List (else end)

            // Commented and changed by Baans 16Sep2020
            // if ($('#lblJobDate').text() != "") {

            if ($('#lblStockOrderedDate').text() == "") {
                SaveStageByOppoID(Stage, 'lblStockOrderedDate');
            }
            else {
               if ($('#lblDigiReadyDate').text() != "" && $('#lblStockInDate').text() == "") {

                   ResetOppStagePopUp(Stage, "");
                }
                else {
                    CustomWarning(revertmessage);
                }
            }
        }
        //else {
        //    CustomWarning('You can not order stock before Job');
        //}
        //}

        if (Stage == "Stock In") {
            if ($('#lblOrderConfirmDate').text() != "") {
                if ($('#lblStockOrderedDate').text() != "") {
                    if ($('#lblStockInDate').text() == "") {
                        SaveStageByOppoID(Stage, 'lblStockInDate');
                    }
                    else {
                        if ($('#lblStockOrderedDate').text() != "" && $('#lblStockCheckedDate').text() == "") {

                            ResetOppStagePopUp(Stage, "");
                        }
                        else {
                            CustomWarning(revertmessage);
                        }
                    }
                }
                else {
                    CustomWarning('You can not stock In before stock order');
                }
            }
            else {
                CustomWarning('You can not stock In until the order is confirmed and stock is ordered');
            }
        }

        if (Stage == "Stock Checked") {
            if ($('#lblStockInDate').text() != "") {
                if ($('#lblStockCheckedDate').text() == "") {
                    SaveStageByOppoID(Stage, 'lblStockCheckedDate');
                }
                else {
                    if ($('#lblStockInDate').text() != "" && $('#lblStockDecoratedDate').text() == "") {

                        ResetOppStagePopUp(Stage, "");
                    }
                    else {
                        CustomWarning(revertmessage);
                    }
                }
            }
            else {
                CustomWarning('You can not stock check before stock In');
            }
        }
        if (Stage == "Stock Decorated") {

            if ($('#lblStockCheckedDate').text() != "" && $('#lblDigiReadyDate').text() != "") {

                if ($('#lblStockDecoratedDate').text() == "") {
                    bootbox.confirm("Are you sure you want to Continue?", function (result) {
                        if (result) {
                            $.ajax({
                                url: '/Opportunity/ChangeStageByOppoID',
                                data: { OppId: $('#lblOpportunityId').text(), Stage: Stage },
                                async: false,
                                success: function (response) {
                                    var data = response.data;
                                    if (data.response.Result == "Success") {
                                        location.href = "/Opportunity/" + UpdateURL + "/" + parseInt($('#lblOpportunityId').text());
                                    }
                                    else {
                                        CustomAlert(data.response);
                                    }
                                }
                            });

                        }
                    });
                }
                else {
                    if ($('#lblStockCheckedDate').text() != "" && $('#lblOrderPackedDate').text() == "") {
                   
                        ResetOppStagePopUp(Stage, "JobDetails");
                    }
                    else {
                        CustomWarning(revertmessage);
                    }
                    
                }
            }
            else {
                CustomWarning('You can not decorate before Art ready and Stock checked');//msg disscuss
            }

        }
        if (Stage == "Order Packed") {
            if ($('#lblStockDecoratedDate').text() != "") {
                var isPackValid = false;
                $.ajax({
                    url: '/Opportunity/GetPackinDetails',
                    data: { OppId: $('#lblOpportunityId').text() },
                    async: false,
                    success: function (response) {
                        if(response!=undefined)
                        {
                            isPackValid = response;
                        }
                    },
                    error: function (response) {
                    },
                    type: 'post'

                });
                if (isPackValid) {
                    //$("#ddlPackedInSet1").css("border-color", "rgba(204, 204, 204, 1)");
                    $("#ddlPackedInSet1").removeClass('customAlertChange');
                    if ($('#lblOrderPackedDate').text() == "") {
                        bootbox.confirm("Are you sure you want to Continue?", function (result) {
                            if (result) {
                                $.ajax({
                                    url: '/Opportunity/ChangeStageByOppoID',
                                    data: { OppId: $('#lblOpportunityId').text(), Stage: Stage },
                                    async: false,
                                    success: function (response) {
                                        var data = response.data;
                                        if (data.response.Result == "Success") {
                                            location.href = "/Opportunity/" + UpdateURL + "/" + parseInt($('#lblOpportunityId').text());
                                        }
                                        else {
                                            CustomAlert(data.response);
                                        }
                                    }
                                });

                            }
                        });
                    }
                    else {
                        if ($('#lblStockDecoratedDate').text() != "" && $('#lblInvoiceDate').text() == "") {
                        
                            ResetOppStagePopUp(Stage, "PackingDetails");
                        }
                        else {
                            CustomWarning(revertmessage);
                        }
                        
                    }
                }
                else
                {
                    //$("#ddlPackedInSet1").css("border-color", "red");
                    $("#ddlPackedInSet1").addClass('customAlertChange');
                    CustomWarning('Pack In details should be filled before packing');
                }
            }
            else {
                CustomWarning('You can not pack order before decorate');//msg disscuss
            }
        }
        if (Stage == "Order Invoiced") {
            if ($('#lblOrderPackedDate').text() != "") {
                // baans change 25th September for Moving to Invoice
                var OppId = parseInt($('#lblOpportunityId').text());
                $.ajax({
                    url: '/Opportunity/GetOptionPackStatus',
                    data: { OppId: OppId },
                    async: false,
                    success: function (response) {
                        if (response == true) {
                            $('#txtSizesPacked').removeClass('customAlertChange');
                            if ($('#lblInvoiceDate').text() == "") {

                                bootbox.confirm("Are you sure you want to Continue?", function (result) {
                                    if (result) {
                                        $.ajax({
                                            url: '/Opportunity/ChangeStageByOppoID',
                                            data: { OppId: $('#lblOpportunityId').text(), Stage: Stage },
                                            async: false,
                                            success: function (response) {
                                                var data = response.data;
                                                if (data.response.Result == "Success") {
                                                    location.href = "/Opportunity/" + UpdateURL + "/" + parseInt($('#lblOpportunityId').text());
                                                }
                                                else {
                                                    CustomAlert(data.response);
                                                }
                                            }
                                        });

                                    }
                                });
                            }
                            else {
                                if ($('#lblOrderPackedDate').text() != "" && $('#lblPaidDate').text() == "") {
                                
                                    ResetOppStagePopUp(Stage, "InvoicingDetails");
                                }
                                else {
                                    CustomWarning(revertmessage);
                                }
                            }
                        }
                        else {
                            CustomError("Size Packed Should be complete before Moving to Invoice Screen.");
                            $('#txtSizesPacked').addClass('customAlertChange');
                        }


                    },
                    error: function (response) {

                    },
                    type: 'post'
                });

                // baans end 25th September
            }
            else {
                CustomWarning('You can not make invoice before order packed');//msg disscuss
            }
        }
        if (Stage == "Paid") {
            if ($('#lblInvoiceDate').text() != "") {
                var balancePaid = $('#lblbalance').text();
                if ($('#lblbalance').text() == "$0") {
                    if ($('#lblPaidDate').text() == "") {
                        // baans change 13th November for change stage when paid
                        //SaveStageByOppoID(Stage, 'lblPaidDate');
                        bootbox.confirm("Are you sure you want to confirm?", function (result) {
                            if (result) {
                                $.ajax({
                                    url: '/Opportunity/ChangeStageByOppoID',
                                    data: { OppId: $('#lblOpportunityId').text(), Stage: Stage },
                                    async: false,
                                    success: function (response) {
                                        var data = response.data;
                                        if (data.response.Result == "Success") {
                                            $('#lblPaidDate').text(DateTimeFormat(data.ChangeDate));
                                            SetOppoStage();
                                            location.href = "/Opportunity/" + UpdateURL + "/" + parseInt($('#lblOpportunityId').text());
                                        }
                                        CustomAlert(data.response);
                                    }
                                });
                            }
                        });
                        // baans end 13th November
                    }
                    else {
                        if ($('#lblInvoiceDate').text() != "" && $('#lblShippedDate').text() == "") {
                        
                            ResetOppStagePopUp(Stage, "InvoicingDetails");
                        }
                        else {
                            CustomWarning(revertmessage);
                        }
                    }
                }
                else {
                    CustomError('Please Clear the paying amount before moving to Paid Section.');
                }
            }
            else {
                CustomWarning('You can not mark paid before invocing');
            }
        }
        if (Stage == "Order Shipped") {
            if ($('#lblPaidDate').text() != "") {
                var isShipValid = false;
                $.ajax({
                    url: '/Opportunity/GetShipDetails',
                    data: { OppId: $('#lblOpportunityId').text() },
                    async: false,
                    success: function (response) {
                        if (response != undefined) {
                            isShipValid = response;
                        }
                    },
                    error: function (response) {
                    },
                    type: 'post'

                });
                if (isShipValid) {
                    if ($('#lblShippedDate').text() == "") {

                        bootbox.confirm("Are you sure you want to Continue?", function (result) {
                            if (result) {
                                $.ajax({
                                    url: '/Opportunity/ChangeStageByOppoID',
                                    data: { OppId: $('#lblOpportunityId').text(), Stage: Stage },
                                    async: false,
                                    success: function (response) {
                                        var data = response.data;
                                        if (data.response.Result == "Success") {
                                            location.href = "/Opportunity/" + UpdateURL + "/" + parseInt($('#lblOpportunityId').text());
                                        }
                                        else {
                                            CustomAlert(data.response);
                                        }
                                    }
                                });

                            }
                        });
                    }
                    else {
                        if ($('#lblPaidDate').text() != "" && $('#lblCompleteDate').text() == "") {
                        
                            ResetOppStagePopUp(Stage, "ShippingDetails");
                        }
                        else {
                            CustomWarning(revertmessage);
                        }
                    }
                }
                else{
                    CustomWarning('Consignment No. should be saved before shipping');//msg disscuss
                }
            }
            else {
                CustomWarning('You can not ship order before paid');//msg disscuss
            }
        }
        if (Stage == "Complete") {
            if ($('#lblShippedDate').text() != "") {
                // baans change 25th Sept for Moving to Complete Screen
                var OppId = parseInt($('#lblOpportunityId').text());
                $.ajax({
                    url: '/Opportunity/GetOptionCompleteStatus',
                    data: { OppId: OppId },
                    async: false,
                    success: function (response) {
                        if (response == true) {
                            $('#txtConsigNoteNo').removeClass('customAlertChange');
                            if ($('#lblCompleteDate').text() == "") {

                                bootbox.confirm("Are you sure you want to Continue?", function (result) {
                                    if (result) {
                                        $.ajax({
                                            url: '/Opportunity/ChangeStageByOppoID',
                                            data: { OppId: $('#lblOpportunityId').text(), Stage: Stage },
                                            async: false,
                                            success: function (response) {
                                                var data = response.data;
                                                if (data.response.Result == "Success") {
                                                    location.href = "/Opportunity/" + UpdateURL + "/" + parseInt($('#lblOpportunityId').text());
                                                }
                                                else {
                                                    CustomAlert(data.response);
                                                }
                                            }
                                        });

                                    }
                                });
                            }
                            else {
                                if ($('#lblShippedDate').text() != "" && $('#lblCompleteDate').text() != "") {
                                
                                    ResetOppStagePopUp(Stage, "ShippingDetails");
                                }
                                else {
                                    CustomWarning(revertmessage);
                                }
                              
                            }
                        }
                        else {
                            CustomError("consignment Note No. Should be saved before Moving to Complete Screen.");
                            $('#txtConsigNoteNo').addClass('customAlertChange');
                        }


                    },
                    error: function (response) {

                    },
                    type: 'post'
                });
                // baans end 25th Sept
            }
            else {
                CustomWarning('You can not complete order before shipped');//msg disscuss
            }
        }
        if (Stage == "Job Accepted") {
            if ($('#lblOrderConfirmDate').text() != "") {
                if ($('#lblJobAcceptedDate').text() == "") {
                    SaveStageByOppoID(Stage, 'lblJobAcceptedDate');
                }
                else {

                    if ($('#lblOrderConfirmDate').text() != "" && $('#lblArtOrderdDate').text() == "") {

                        ResetOppStagePopUp(Stage, "");
                    }
                    else {
                        CustomWarning(revertmessage);
                    }
                }
            }
            else {
                CustomWarning('You can not accept before Confirmed');
            }
        }
        if (Stage == "Proof Created") {
            if ($('#lblArtOrderdDate').text() != "") {
                if ($('#lblProofCreatedDate').text() == "") {
                    SaveStageByOppoID(Stage, 'lblProofCreatedDate');
                }
                else {

                    if ($('#lblArtOrderdDate').text() != "" && $('#lblProofSentDate').text() == "") {

                        ResetOppStagePopUp(Stage, "");

                    }
                    else {
                        CustomWarning(revertmessage);
                    }
                }
            }
            else {
                CustomWarning('You can not create before Ordered');
            }
        }
        if (Stage == "Proof Sent") {
            if ($('#lblProofCreatedDate').text() != "") {
                if ($('#lblProofSentDate').text() == "") {
                    SaveStageByOppoID(Stage, 'lblProofSentDate');
                }
                else {
                     if ($('#lblArtOrderdDate').text() != "" && $('#lblProofApprovedDate').text() == "") {

                         ResetOppStagePopUp(Stage,"");
                    }
                    else {
                        CustomWarning(revertmessage);
                    }
                }
            }
            else {
                CustomWarning('You can not sent before created');
            }
        }

    }
    else {
        CustomWarning('Opportunity not saved');
    }


}

// baans change 15th November for ShowFilterDataByProfile
function ShowFilterDataByProfile() {
    debugger;
    var UserProfile = $('#profileOfUser').val();
    //window.location = '/opportunity/OpportunityList/';
    //$("#hiddenUserProfile").val(UserProfile);
    if (UserProfile == "All") {
        UserProfile = 0;
        $.ajax({
            url: '/Opportunity/SetNewUser',
            data: { UserProfile: UserProfile },
            async: false,
            success: function (response) {
                var All = "All";
                $('#profileOfUser').val(All);
                //GetAllOpportunityList('Opportunity');
                window.location = '/opportunity/OpportunityList/';
            },
            type: 'post',
        });
    } else {
        if (UserProfile != "") {
            $.ajax({
                url: '/Opportunity/SetNewUser',
                data: { UserProfile: UserProfile },
                async: false,
                success: function (response) {

                    $('#profileOfUser').val(response);
                    //GetAllOpportunityList('Opportunity');
                    window.location = '/opportunity/OpportunityList/';
                },
                type: 'post',
            });
        }
    }
}
// baans end 15th November
function SaveStageByOppoID(Stage, lbldate) {
    bootbox.confirm("Are you sure you want to confirm?", function (result) {
        if (result) {
            // 29 April Stage Change List (right side stage change)
            var HiddenSrc = $("#HiddenCallingSource").val();
            if ($("#HiddenCallingSource").val() == "OpportunityList") {
                var OppId = $("#HiddenOppIdInList").val();
                $.ajax({
                    url: '/Opportunity/ChangeStageByOppoID',
                    data: { OppId: OppId, Stage: Stage },
                    async: false,
                    success: function (response) {
                        var data = response.data;
                        if (data.response.Result == "Success") {
                            $('#' + lbldate).text(DateTimeFormat(data.ChangeDate));
                            $('#HiddenStageDateExist').val("DateExist");
                            SetOppoStage();
                            JobsGridList("Job1", "All", "UpperTab");
                            if (response.Stage == "Job" || response.Stage == "Order Confirmed") {
                                $("#lblListJobStageData").text(response.Stage);
                            }
                        }
                        CustomAlert(data.response);
                    }
                });
            }
            else {
                // 29 April Stage Change List (else for if)
                $.ajax({
                    url: '/Opportunity/ChangeStageByOppoID',
                    data: { OppId: $('#lblOpportunityId').text(), Stage: Stage },
                    async: false,
                    success: function (response) {
                        var data = response.data;
                        if (data.response.Result == "Success") {
                            $('#' + lbldate).text(DateTimeFormat(data.ChangeDate));
                            SetOppoStage();
                        }
                        CustomAlert(data.response);
                    }
                });
            }// 29 April Stage Change List (else end)
        }
    });
}
function DateTimeFormat(date) {
    if (date != null && date != undefined && date != "") {
        var dateString = date.substr(6);
        //var oldUTC = new Date(parseInt(dateString));
        //var toutc = oldUTC.toUTCString() ;
        //var currentTime = new Date(toutc + " UTC");

        var currentTime = new Date(parseInt(dateString + " UTC"));
        var tzOffset = currentTime.getTimezoneOffset();
        currentTime.setMinutes(currentTime.getMinutes() + tzOffset);
        //currentTime.setHours(currentTime.getHours() - 11);
        var month = currentTime.getMonth() + 1;
        var day = currentTime.getDate();
        if (month < 10) {
            month = "0" + month;
        }
        if (day < 10) {
            day = "0" + day;
        }
        var year = currentTime.getFullYear();
        var timeHour = currentTime.getHours();
        if (timeHour < 10) {
            timeHour = "0" + timeHour;
        }
        //if (timeHour > 12) {
        //    timeHour = timeHour - 11;
        //}
        //else {
        //    day = day - 1;
        //    timeHour = timeHour - 11;
        //}
        var timeMinute = currentTime.getMinutes();
        if (timeMinute < 10) {
            timeMinute = "0" + timeMinute;
        }
        var date = day + "/" + month + "/" + year;
        return date + " " + timeHour + ":" + timeMinute;
        //var nowDate = new Date(parseInt(date.substr(6)));
        //return ("0" + nowDate.getDate()).slice(-2) + '/' + ("0" + (nowDate.getMonth() + 1)).slice(-2) + '/' + nowDate.getFullYear() + ' ' + ("0" + nowDate.getHours()).slice(-2) + ':' + ("0" + nowDate.getMinutes()).slice(-2);
    }
    else {
        return "";
    }
}

function MakeReadonly() {
    if ($('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "Opportunity" || $('#PageName').val() == "OrderDetails" || $('#PageName').val() == "JobDetails" || $('#PageName').val() == "OrganisationDetail" || $('#PageName').val() == "Event" || $('#PageName').val() == "CompleteDetails" || $('#PageName').val() == "InvoicingDetails" || $('#PageName').val() == "PackingDetails" || $('#PageName').val() == "ShippingDetails") {
        $('#txtContactType').attr('disabled', true).addClass('MakeReadonly');
        $('#txtFirstName').prop('readonly', true).addClass('MakeReadonly');
        $('#ddlTitle').attr('disabled', true).addClass('MakeReadonly');
        $('#txtLastName').prop('readonly', true).addClass('MakeReadonly');
        $('#txtEmail').prop('readonly', true).addClass('MakeReadonly');
        $('#txtContactNo').prop('readonly', true).addClass('MakeReadonly');
        $('#txtContactRole').attr('disabled', true).addClass('MakeReadonly');
        $('#ddlAcctMgr').attr('disabled', true).addClass('MakeReadonly');
        $('#txtNotes').prop('readonly', true).addClass('MakeReadonly');
        $('#txtOrgName').attr('readonly', true).addClass('MakeReadonly');       //11 Sep 2018(N)
    }
}
function MakeorgReadonly() {
    //if ($('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "ContactDetails") {
    if ($('#PageName').val() == "QuoteDetails" || $('#PageName').val() == "OrderDetails" || $('#PageName').val() == "ContactDetails" || $('#PageName').val() == "PurchaseDetails") {

        $('#txtorgName').attr('readonly', true).addClass('MakeReadonly');
        $('#txtOrgType').attr('disabled', true).addClass('MakeReadonly');
        $('#ddlOrgAcctMgr').attr('disabled', true).addClass('MakeReadonly');
        $('#txtTradeName').attr('readonly', true).addClass('MakeReadonly');
        $('#txtABNName').attr('readonly', true).addClass('MakeReadonly');
        $('#txtMainPhone').attr('readonly', true).addClass('MakeReadonly');
        $('#txtWebAddress').attr('readonly', true).addClass('MakeReadonly');
        $('#txtbrand').attr('readonly', true).addClass('MakeReadonly');
        $('#txtOrgNotes').attr('readonly', true).addClass('MakeReadonly');
        //$('#txtorgEmail').attr('readonly', true).addClass('MakeReadonly');      //21 June 2019 (N)
    }
}
function GetDate(date) {
    if (date != "" && date != null && date != undefined && date != "") {
        var newdate = date.split('/');
        return (newdate[2] + '-' + newdate[1] + '-' + newdate[0]);
    }
    else {
        return "";
    }

}

function QuoteValidCheck() {
    var data = false;
    // Baans change 30th November for checking the valid OptionData
    var Stage = "";
    if ($('#PageName').val() == "QuoteDetails") {
        Stage = "Opp";
    }
    else {
        Stage = "Order";
    }
    $.ajax({
        url: '/Contact/CheckOrgByOppoID',
        data: { 'OpportunityID': $('#lblOpportunityId').text(), Stage: Stage },
        async: false,
        success: function (response) {
            data = response;

        },
        error: function (response) {
            alert();
        },
        type: 'post'

    });
    return data;
    // baans end 30th November
}
function DateFormat(date) {
    //if (date != null && date != undefined && date != "") {
    //    var nowDate = new Date(parseInt(date.substr(6)));
    //    return ("0" + nowDate.getDate()).slice(-2) + '/' + ("0" + (nowDate.getMonth() + 1)).slice(-2) + '/' + nowDate.getFullYear();
    //}
    //else {
    //    return "";
    //}
    if (date != null && date != undefined && date != "") {
        var dateString = date.substr(6);
        var currentTime = new Date(parseInt(dateString + " UTC"));
        var tzOffset = currentTime.getTimezoneOffset();
        currentTime.setMinutes(currentTime.getMinutes() + tzOffset);
        var month = currentTime.getMonth() + 1;
        var day = currentTime.getDate();
        if (month < 10) {
            month = "0" + month;
        }
        if (day < 10) {
            day = "0" + day;
        }
        var year = currentTime.getFullYear();
        var date = day + "/" + month + "/" + year;
        return date;
    }
    else {
        return "";
    }
}

// baans change 26th November ListDateFormat
function ListDateFormat(date) {
    //if (date != null && date != undefined && date != "") {
    //    var nowDate = new Date(parseInt(date.substr(6)));
    //    return ("0" + nowDate.getDate()).slice(-2) + '/' + ("0" + (nowDate.getMonth() + 1)).slice(-2) + '/' + nowDate.getFullYear();
    //}
    //else {
    //    return "";
    //}
    if (date != null && date != undefined && date != "") {
        var dateString = date.substr(6);
        var currentTime = new Date(parseInt(dateString + " UTC"));
        var tzOffset = currentTime.getTimezoneOffset();
        currentTime.setMinutes(currentTime.getMinutes() + tzOffset);
        var month = currentTime.getMonth() + 1;
        var day = currentTime.getDate();
        if (month < 10) {
            month = "0" + month;
        }
        if (day < 10) {
            day = "0" + day;
        }
        var year = currentTime.getFullYear().toString().substr(-2);
        var date = day + "/" + month + "/" + year;
        return date;
    }
    else {
        return "";
    }
}
// baans end 26th November
function validateEmailAddress(sEmail) {
    // baans change 11th Jan
    //var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    //if (filter.test(sEmail)) {
    if (sEmail.includes('.') && sEmail.includes('@')){
        return true;
    }
    else {
        return false;
    }
    // baans end 11th January
}
function MasterSearch() {

    if ($("#PageName").val() == "ContactList") {
        $('#btnCustomSearch').click();
    }

    if ($("#PageName").val() == "OpportunityList") {
        $('#Custom').click();
    }
    //tarun 08/09/2018
    if ($("#PageName").val() == "OrganisationList") {
        $('#btnOrgCustomSearch').click();
    }
    //end
    if ($('#hiddenisCustom').val() != "" && $('#searchtextbox').val()) {
        $('#JoblistCuston').click();
    }
    //8 Sep 2018 (N)
    if ($('#PageName').val() == "PurchaseList") {
        $('#PurchaseCustom').click();
    }
    //8 Sep 2018 (N)
    //15 Oct 2019 (N)
    if ($('#PageName').val() == "ApplicationList") {
        $('#btnApplicationCustom').click();
    }
    //15 Oct 2019 (N)
}
function PageSearch() {
    var options = {
        "acrossElements": true,
    };
    $(".SearchText").unmark();
    var SearchedText = $('#searchtextbox').val();
    $(".SearchText").mark(SearchedText);
}
// baans change 5th October for filter on enter search
function FilterOnEnter() {
    var elem = document.getElementById('searchtextbox');
    elem.onkeyup = function (e) {
        if (e.keyCode == 13) {
            MasterSearch();
        }
    }
}
// baans end 5th october

function ResetOrgForm()
{
    $("#btnsaveOrganisation").removeClass("customAlertChange");
    $("#txtorgName").val('');
    $("#txtTradeName").val('');
    $("#txtABNName").val('');
    $("#txtMainPhone").val('');
    $("#txtWebAddress").val('');
    $("#txtbrand").val('');
    $("#txtOrgNotes").val('');
    $("#hdnOrgId").val('');
    $('#txtOrgType').val('');
    $('#ddlOrgAcctMgr').val('');
    $('#txtorgEmail').val('');      //21 June 2019 (N)
    $('#txtOrgType').removeAttr('disabled').removeClass('MakeReadonly');
    $('#ddlOrgAcctMgr').removeAttr('disabled').removeClass('MakeReadonly');
    $('#txtorgName').removeAttr('readonly').removeClass('MakeReadonly');
    $('#txtTradeName').removeAttr('readonly').removeClass('MakeReadonly');
    $('#txtABNName').removeAttr('readonly').removeClass('MakeReadonly');
    $('#txtMainPhone').removeAttr('readonly').removeClass('MakeReadonly');
    $('#txtWebAddress').removeAttr('readonly').removeClass('MakeReadonly');
    $('#txtbrand').removeAttr('readonly').removeClass('MakeReadonly');
    $('#txtOrgNotes').removeAttr('readonly').removeClass('MakeReadonly');
    //$('#txtorgEmail').removeAttr('readonly').removeClass('MakeReadonly');       //21 June 2019 (N)
    if($('#PageName').val()=="ContactDetails")
    {
        GetOrganisationAddress(0, "");
        CheckDeliveryAddress(0);
    }
    //tarun 06/09/2018
    if ($('#PageName').val() == "PurchaseDetails") {
        $('#txtOrgType').val('Supplier');
        $('#txtOrgType').attr('disabled', true).addClass('MakeReadonly');
    }
    //end
}
function SetAccountManager(Id, Control, Type) {
    if (Type == "OppAccountManager") {
        var Manager = inactiveManager.find(x=>x.id == Id);
        if (Manager != null && Manager != undefined) {
            $("#" + Control + " option[value='" + Id + "']").remove();
            $("#" + Control).append('<option value="' + Manager.id + '">' + Manager.firstname + '</option>');
            $('#' + Control).val(Id);
        }
        else {
            $('#' + Control).val(Id);
        }
    }
    if (Type == "AccountManager") {
        var Manager = inactiveManager.find(x=>x.id == Id);
        if (Manager != null && Manager != undefined) {
            $("#" + Control + " option[value='" + Id + "']").remove();
            $("#" + Control).append('<option value="' + Manager.id + '">' + Manager.AccountManagerFullName + '</option>');
            $('#' + Control).val(Id);
        }
        else {
            $('#' + Control).val(Id);
        }
    }

}
//21 Aug 2018 (N)
function MakeOpportunityReadonly() {
    if ($('#PageName').val() == "Event") {

        $('#oppName').attr('readonly', true).addClass('MakeReadonly');
        $('#oppQuantity').attr('readonly', true).addClass('MakeReadonly');
        $('#datepicker').attr('disabled', true).addClass('MakeReadonly');
        $('#depositreqdate').attr('disabled', true).addClass('MakeReadonly');
        $('#OppShipping').attr('disabled', true).addClass('MakeReadonly');
        $('#txtshippingday').attr('readonly', true).addClass('MakeReadonly');
        $('#txtshippingprice').attr('readonly', true).addClass('MakeReadonly');
        $('#oppSource').attr('disabled', true).addClass('MakeReadonly');
        $('#oppCampaign').attr('disabled', true).addClass('MakeReadonly');
        $('#txtConfirmedDate').attr('readonly', true).addClass('MakeReadonly');
        $('#oppStage').attr('disabled', true).addClass('MakeReadonly');
        $('#ddlOppAcctMgr').attr('disabled', true).addClass('MakeReadonly');
        $('#ddlDecline').attr('disabled', true).addClass('MakeReadonly');
        $('#txtCancelled').attr('disabled', true).addClass('MakeReadonly');
        $('#txtlost').attr('disabled', true).addClass('MakeReadonly');
        $('#txtrepeatfrom').attr('readonly', true).addClass('MakeReadonly');
        $('#oppNotes').attr('readonly', true).addClass('MakeReadonly');
        //$('#txtrepeatfrom').attr('readonly', true).addClass('MakeReadonly');
        $('#ms1').attr('disabled', true).addClass('MakeReadonly');
        //$('#ms1').magicSuggest({disabled: true});
        var ms = $('#ms1').magicSuggest({});
        ms.disable();
    }
}
//21 Aug 2018 (N)

//22 Nov 2018 (N)
function GetyyyymmddDate(date) {
    //return date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
    var GetDate = date.split('-');

    return GetDate[2] + "-" + GetDate[1] + "-" + GetDate[0];
}

function GetFormattedDate(date) {
    //Get Current date Without Time
    var DateValue = date.getDate();
    //    alert(DateValue);
    return date.getFullYear() + '-' + ('00' + (date.getMonth() + 1)).substr(-2) + '-' + date.getDate();    //tarun 22/09/2018
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
        if (RequiredbyDatenew < DepReqDatenew) {
            $('#' + InputID).val('');
            if (InputID == "todate") {
                CustomWarning('End Date should be greater than Start Date');
            }
            else {
                CustomWarning('Start Date should be less than End Date');
            }
        }

    }
}

$(function () {
    $("#btnFindOpp").click(function () {
        $(".UpperTabs").each(function () {
            if ($(this).hasClass('active')) {
                JobsGridList($(this).attr('id'), "", "UpperTab");
            }
        });
    });
});

//22 Nov 2018 (N)

//29 April Stage Change List
function ResetRightSideBar(data_start) {
    $('#imageOppThumbnail').attr('src', '/Content/uploads/Opportunity/NoImage.png');
    $("#lblOrderConfirmDate").text(DateTimeFormat(data_start));
    $('#lblArtOrderdDate').text(DateTimeFormat(data_start));
    $('#lblProofApprovedDate').text(DateTimeFormat(data_start));
    $('#lblDigiReadyDate').text(DateTimeFormat(data_start));
    $('#lblStockOrderedDate').text(DateTimeFormat(data_start));
    $('#lblStockInDate').text(DateTimeFormat(data_start));
    $('#lblStockCheckedDate').text(DateTimeFormat(data_start));

    $("#lblOrderConfirmDate").css("border-color", "#FFFFFF");
    $("#btnArtOrderedStage").css("border-color", "#FFFFFF");
    $("#btnProofApprovedStage").css("border-color", "#FFFFFF");
    $("#btnDigiReadyStage").css("border-color", "#FFFFFF");
    $("#btnStockOrderStage").css("border-color", "#FFFFFF");
    $("#btnStockInStage").css("border-color", "#FFFFFF");
    $("#btnStockCheckedStage").css("border-color", "#FFFFFF");
}
//29 April Stage Change List
