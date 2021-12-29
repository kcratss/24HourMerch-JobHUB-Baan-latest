var OrganisationChangesValues = [];
var AddressChangesValues = [];
$(function () {
    if ($("#PageName").val() == "OrganisationDetail")
    {
        $("#btnRefreshOrganisation,#btnOpenOrganisation").css("display", "none");
        
    }
    // baans change 15th November for user by title
    if ($("#PageName").val() == "OrganisationDetail" || $("#PageName").val() == "OrganisationList") {
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
    $('#txtMainPhone').inputmask({ "mask": "(9999) 999-999" });

    $(".OrganisationContainer :input").focus(function (event) {
        OrganisationChangesValues.push({ id: $(this).attr('id'), value: $(this).val() });
    });
    $(".OrganisationContainer :input").blur(function (event) {
        var OldValues = OrganisationChangesValues.find(x=>x.id == $(this).attr('id'));
        var index = OrganisationChangesValues.findIndex(x=>x.id == $(this).attr('id'));
        if (OldValues != null && OldValues != undefined) {

            if (OldValues.value != $(this).val()) {
                $("#btnsaveOrganisation").addClass("customAlertChange");

            }
            OrganisationChangesValues.splice(index, 1);
        }
    });

    $(".AddressContainer :input").focus(function (event) {
        AddressChangesValues.push({ id: $(this).attr('id'), value: $(this).val() });
    });
    $(".AddressContainer :input").blur(function (event) {
        var OldValues = AddressChangesValues.find(x=>x.id == $(this).attr('id'));
        var index = AddressChangesValues.findIndex(x=>x.id == $(this).attr('id'));
        if (OldValues != null && OldValues != undefined) {

            if (OldValues.value != $(this).val()) {
                $("#btnsaveAddress").addClass("customAlertChange");

            }
            AddressChangesValues.splice(index, 1);
        }
    });

    $("#ddlStateList").change(function () {
        $("#btnsaveAddress").addClass("customAlertChange");
    });

});
function OrganisationList(type) {
    var data;
    var model = { type: type };
    var url = '/Organisation/GetOrganisationList';
    if (type == "Custom") {
        // baans change 04th December for Custom
        //url = '/Organisation/GetCustomOrganisationList';
        //model = { CustomText: $('#searchtextbox').val(), TableName: 'Vw_tblOrganisation' };
        var searchdata = $('#searchtextbox').val();
        if (searchdata != "") {
            url = '/Organisation/GetCustomOrganisationList';
            model = { CustomText: $('#searchtextbox').val(), TableName: 'Vw_tblOrganisation' };
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




    var obj =
    {
        selectionModel: { type: 'row' },
        virtualX: true, virtualY: true,
        resizable: false,
        wrap: false,
        hwrap: false,
        pageModel: { type: "local", rPP: 50 },
        strNoRows: 'No records found for the selected Criteria',
        scrollModel: { horizontal: false },
        //scrollModel: { autoFit: false },
        columnTemplate: { width: 120, halign: "left" },
        numberCell: { width: 0, title: "", minWidth: 0 },
        sortable: true,
        //sortModel: { cancel: false, type: 'local', sorter: [{ dataIndx: 1, dir: ['up', 'down', 'up'] }] }
    };
    obj.width = "97%";
    obj.height = 782;
    obj.numberCell = { width: 0, title: "", minWidth: 0 };
    obj.columnTemplate = { width: 120, halign: "left" };

    obj.colModel = [
        {
            title: "Org No", width: "6%", dataType: "int", align: "center", dataIndx: "DisplayOrganisationId", editable: false,
            render: function (ui) {
                var id = ui.cellData;
                return "<a href='/Organisation/OrganisationDetails/" +parseInt(id)+ "'><label class='internalLnk'>" + id + "</label></a>";
            }
        },
        { title: "Organisation Name", dataIndx: "OrgName", dataType: "string", width: "10%", editable: false },
        { title: "Trading Name", width: "12%", dataType: "string", dataIndx: "TradingName", editable: false },
        { title: "Brand", width: "10%", dataType: "string", dataIndx: "Brand", editable: false },
        {
            title: "Main Phone", width: "9%", dataIndx: "MainPhone", align: "left", dataType: "int", editable: false,


            render: function (ui) {
                var currentF = 0;
                currentF = ui.cellData;
                //var re = new RegExp("([0-9])([0-9]{3})([0-9]{3})([0-9]{3,6})", "g");
                if (currentF != null) {
                    var newFormat1 = currentF.toString().substr(0, 4);
                    var newFormat2 = currentF.toString().substr(4, 3);
                    var newFormat3 = currentF.toString().substr(7, 3);
                    var newFormat = newFormat1 + ' ' + newFormat2 + ' ' + newFormat3;
                    return newFormat;
                }
            }

        },
        { title: "Organisation Notes", width: "43%", dataType: "string", dataIndx: "OrgNotes", editable: false },
        { title: "Account Manager", width: "11%", dataType: "string", dataIndx: "AccountManager", editable: false },
    ];




    obj.dataModel = { data: data };
    pq.grid("#OrganisationGrid", obj);
    pq.grid("#OrganisationGrid", "refreshDataAndView");

}
function VaalidateOrg()
{
    var checkflag = true;
    if ($("#txtorgName").val() != "") {
        //$("#txtorgName").css("border-color", "rgba(204, 204, 204, 1)");
        $("#txtorgName").removeClass('customAlertChange');
    } else {
        //$("#txtorgName").css("border-color", "red");
        $("#txtorgName").addClass('customAlertChange');
      
        checkflag = false;
    }

    //if ($("#txtTradeName").val() != "") {
    //    $("#txtTradeName").css("border-color", "rgba(204, 204, 204, 1)");
    //} else {
    //    $("#txtTradeName").css("border-color", "red");
    //    checkflag = false;
    //}

    //if ($("#txtABNName").val() != "") {
    //    $("#txtABNName").css("border-color", "rgba(204, 204, 204, 1)");
    //} else {
    //    $("#txtABNName").css("border-color", "red");
    //    checkflag = false;
    //}

    //if ($("#txtMainPhone").val() != "") {
    //    $("#txtMainPhone").css("border-color", "rgba(204, 204, 204, 1)");
    //} else {
    //    $("#txtMainPhone").css("border-color", "red");
    //    checkflag = false;
    //}

    //if ($("#txtWebAddress").val() != "") {
    //    $("#txtWebAddress").css("border-color", "rgba(204, 204, 204, 1)");
    //} else {
    //    $("#txtWebAddress").css("border-color", "red");
    //    checkflag = false;
    //}

    //if ($("#txtbrand").val() != "") {
    //    $("#txtbrand").css("border-color", "rgba(204, 204, 204, 1)");
    //} else {
    //    $("#txtbrand").css("border-color", "red");
    //    checkflag = false;
    //}
    return checkflag;
}
function Saveorganisation()
{
    if ($('#txtTradeName').val() == "")
    {
        $('#txtTradeName').val($('#txtorgName').val());
    }
    if ($('#PageName').val() == "OrganisationDetail" || $('#PageName').val() == "PurchaseDetails") {        //tarun Purchase Reference
        AddOrganisation();
    }
    else {
        var checkflag = VaalidateOrg()
        if (checkflag == true) {
            if ($("#HiddenContactId").val() != "") {
                if ($('#HiddenforDefaultOrgID').val() != "") {
                    if ($("#HiddenforDefaultOrgID").val() == $('#hdnOrgId').val()) {
                        AddOrganisation();
                    }
                    else {
                        bootbox.confirm('Organisation for this contact already exist. Do you wish to change?', function (result) {
                            if (result) {
                                AddOrganisation();
                            }
                        });
                    }

                }
                else {
                    if ($('#PageName').val() != "ContactDetails") {
                        var ContactExist = ContactList.find(x=>x.id == $('#HiddenContactId').val());
                        if ((ContactExist == null || ContactExist == undefined) && $('#PageName').val() != "ContactDetails") {
                            CustomWarning('Contact does not linked with opportunity.');

                        }
                        else {
                            AddOrganisation();
                        }
                    }
                    else
                    {
                        AddOrganisation();
                    }
                }
            }
            else
            {
                CustomWarning('Save Contact first')
            }
        }
        else
        {
            //CustomWarning('Fill all required fields.');
            CustomErrorCode("Required");
        }
    }
}
function AddOrganisation(){
    // alert(id);
    var checkflag = VaalidateOrg()
    // baans change 29th Sept for checking the organisation Validation
    var IsOrgValid = true;
    var filter = /^[^-\s][a-zA-Z0-9_.\s-]+$/;
    //var filter = /^[a-z\d\-_\s]+$/;
    //var filter = /^[a - zA - Z0-9][\sa - zA - Z0-9]*/
    if (filter.test($('#txtorgName').val())) {
        $("#txtorgName").removeClass("customAlertChange");
    }
    else {
        $("#txtorgName").addClass("customAlertChange");
        IsOrgValid = false;
    }

    //baans change 11th January for checking the AcccountMgr
    //var orgAcctMgr = $('#ddlOrgAcctMgr').val();
    if ($('#PageName').val() != "PurchaseDetails") {
        if ($('#ddlOrgAcctMgr').val() == "") {
            $("#ddlOrgAcctMgr").addClass("customAlertChange");
            IsOrgValid = false;
        }
        else {
            $("#ddlOrgAcctMgr").removeClass("customAlertChange");
        }
    }
    
    if ($('#PageName').val() == "PurchaseDetails") {
        if ($('#txtTradeName').val() == "") {
            $("#txtTradeName").addClass("customAlertChange");   
            IsOrgValid = false;
        }
        else {
            $("#txtTradeName").removeClass("customAlertChange");
        }

        if ($('#txtMainPhone').val() == "") {
            $("#txtMainPhone").addClass("customAlertChange");
            IsOrgValid = false;
        }
        else {
            $("#txtMainPhone").removeClass("customAlertChange");
        }
    }

    // baans end 11th January
    if (IsOrgValid) {
        if (checkflag == true) {
            var id, txtorgName, txtTradeName, txtABNName, txtMainPhone, txtWebAddress, txtIndustry, txtOrgNotes, ContactID, txtOrgType, ddlOrgAcctMgr, PurchaseId,EmailId;

            txtorgName = $("#txtorgName").val();
            txtTradeName = $("#txtTradeName").val();
            txtABNName = $("#txtABNName").val();
            txtMainPhone = $("#txtMainPhone").val();
            txtWebAddress = $("#txtWebAddress").val();
            txtbrand = $("#txtbrand").val();
            txtOrgType = $('#txtOrgType').val();
            ddlOrgAcctMgr = $('#ddlOrgAcctMgr').val();
            //if ($("#PageName").val() == "OrganisationDetail") {
            if ($("#hdnOrgId").val() != "") {
                id = $("#hdnOrgId").val();
            }
            else {
                id = 0;
            }
            //}
            //else {
            //    id = 0;
            //}

            txtOrgNotes = $("#txtOrgNotes").val();
            ContactID = 0;
            if ($("#HiddenContactId").val() != "") {
                ContactID = $("#HiddenContactId").val();
            }
            if ($('#PageName').val() == "PurchaseDetails") {  /*04- sep-2018 (N)*/
                PurchaseId = $('#lblpurch').text();
                EmailId = $('#txtorgEmail').val();
            }

            $.ajax({
                url: '/Organisation/AddOrganisation',
                data: { OrgId: id, OrgName: txtorgName, TradingName: txtTradeName, ABN: txtABNName, MainPhone: txtMainPhone, WebAddress: txtWebAddress, Brand: txtbrand, OrgNotes: txtOrgNotes, AcctMgrId: ddlOrgAcctMgr, OrganisationType: txtOrgType, PageSource: $('#PageName').val(), ContactID: ContactID, PurchaseId: PurchaseId, EmailAddress: EmailId },
                async: false,

                success: function (response) {
                    if (response.ID != null && response.ID != undefined) {
                        $("#btnsaveOrganisation").removeClass("customAlertChange");
                        if ($('#PageName').val() == "OrganisationDetail" && $('#hdnOrgId').val() == "") {
                            history.pushState('', '', '/Organisation/OrganisationDetails/' + response.ID);
                        }
                        $('#hdnOrgId').val(response.ID);
                        if ($('#PageName').val() == "ContactDetails") {
                            MakeorgReadonly();
                            $('#HiddenforDefaultOrgID').val(response.ID);
                        }

                        if ($('#PageName').val() == "OrderDetails" || $('#PageName').val() == "QuoteDetails") {
                            MakeorgReadonly();
                            var OrgContactindex = ContactList.findIndex(x => x.id == ContactID);
                            if (OrgContactindex != -1) {
                                ContactList[OrgContactindex].OrgId = id;
                            }
                        }
                        //tarun 06/09/2018
                        // var count = CheckDeliveryAddress(response.ID);
                        if (response.ID > 0) {
                            if ($('#PageName').val() != "PurchaseDetails") {
                                var count = CheckDeliveryAddress(response.ID);
                                GetOrganisationAddress(response.ID, "");
                                $("#txtTradingName").val(txtTradeName);
                            }
                            //end
                        }
                    }

                    CustomAlert(response);
                    //tarun 22/08/2018
                    if ($('#PageName').val() == "PurchaseDetails") {
                        MakeorgReadonly();
                    }
                    //end
                },
                error: function (response) {
                    CustomAlert(response);
                },
                type: 'post',

            });
        }
        else {
            //CustomWarning('Fill all required fields.');
            CustomErrorCode("Required");
        }
    }
    else {
        //CustomWarning('Fill all required fields.');
        CustomErrorCode("Required");
    }


}

function AddOrganisationAddress() {
    var checkflag = true;
    var Validcheckflag = true;
    var txtTradingName, txtAttention, ddlDelivery, txtAddress1, txtAddress2, ddlStateList, txtPostCode, txtAddressNotes, OrgId, PurchaseId, PageSource, AddressId; //tarun 6/09/2018

    if ($("#txtTradingName").val() != "") {
        txtTradingName = $("#txtTradingName").val();
        //$("#txtTradingName").css("border-color", "rgba(204, 204, 204, 1)");
        $("#txtTradingName").removeClass('customAlertChange');
    } else {
        //$("#txtTradingName").css("border-color", "red");
        $("#txtTradingName").addClass('customAlertChange');
        checkflag = false;
    }
  

    //if ($("#txtAttention").val() != "") {
    //    txtAttention = $("#txtAttention").val();
    //    $("#txtAttention").css("border-color", "rgba(204, 204, 204, 1)");
    //} else {
    //    $("#txtAttention").css("border-color", "red");
    //    checkflag = false;
    //}

    if ($("#ddlDelivery").val() != "") {
        ddlDelivery = $("#ddlDelivery").val();
        //$("#ddlDelivery").css("border-color", "rgba(204, 204, 204, 1)");
        $("#ddlDelivery").removeClass('customAlertChange');
    } else {
        //$("#ddlDelivery").css("border-color", "red");
        $("#ddlDelivery").addClass('customAlertChange');
        checkflag = false;
    }

    //if ($("#txtAddress1").val() != "") {
    //    txtAddress1 = $("#txtAddress1").val();
    //    $("#txtAddress1").css("border-color", "rgba(204, 204, 204, 1)");
    //} else {
    //    $("#txtAddress1").css("border-color", "red");
    //    checkflag = false;
    //}

    //if ($("#txtAddress2").val() != "") {
    //    txtAddress2 = $("#txtAddress2").val();
    //    $("#txtWebAddress").css("border-color", "rgba(204, 204, 204, 1)");
    //}
    //else {
    //    txtAddress2 = "";
    //}

    //if ($("#ddlStateList").val() != "") {
    //    ddlStateList = $("#ddlStateList").val();
    //    $("#ddlStateList").css("border-color", "rgba(204, 204, 204, 1)");
    //} else {
    //    $("#ddlStateList").css("border-color", "red");
    //    checkflag = false;
    //}
    PageSource = $("#PageName").val();
    //tarun 18/09/2018
    //AddressId = $('#HiddenOrgIdforAddress').val();
    if ($('#HiddenAddressId').val() != "") {
        AddressId = $('#HiddenAddressId').val();
    }
    else {
        AddressId = 0;
    }
    //end
    txtAttention = $("#txtAttention").val();
    txtAddress1 = $("#txtAddress1").val();
    txtAddress2 = $("#txtAddress2").val();
    ddlStateList = $("#ddlStateList").val();
    OppId = $('#lblOpportunityId').text();    //tarun 18/09/2018
    //tarun 6/09/2018
    if ($('#PageName').val() == "PurchaseDetails") {
        PurchaseId = $('#lblpurch').text();
    }
    //end
    if ($("#txtPostCode").val() != "") {
        txtPostCode = $("#txtPostCode").val();
        //$("#txtPostCode").css("border-color", "rgba(204, 204, 204, 1)");
        $("#txtPostCode").removeClass('customAlertChange');
    } else {
        //$("#txtPostCode").css("border-color", "red");
        $("#txtPostCode").addClass('customAlertChange');
        checkflag = false;
    }

    //18 Sep 2018
    if ($("#HiddenAddressId").val() != "") {
        AddressId = $("#HiddenAddressId").val();
    }
    else {
        AddressId = 0;
    }
    //18 Sep 2018

    if ($('#PageName').val() == "OrganisationDetail" || $('#PageName').val() == "ContactDetails" ) {
        if ($("#hdnOrgId").val() != "") {
            OrgId = $("#hdnOrgId").val();
        } else {
            checkflag = false;
            Validcheckflag = false;
            if ($("#txtorgName").val() != "") {
                CustomWarning("Save Organisation First");
            } else {
                CustomWarning("Select Organisation First");
            }
        }
    }

    //tarun 22/08/2018
    if ($('#PageName').val() == "PurchaseDetails") {
        OrgId = $("#HiddenPurchaseOrg").val();
    }
    //end

    if ($('#PageName').val() == "JobDetails" || $('#PageName').val() == "InvoicingDetails" || $('#PageName').val() == "PackingDetails" || $('#PageName').val() == "JobDetails" || $("#PageName").val() == "CompleteDetails" || $('#PageName').val() == "ShippingDetails") {
        if ($("#HiddenforDefaultOrgID").val() != "") {
            OrgId = $("#HiddenforDefaultOrgID").val();
        } else {
            checkflag = false;
            Validcheckflag = false;
                CustomWarning("Link Organisation with contact before saving address");
        }
    }
   // $('#HiddenOrgIdforAddress').val();

    txtAddressNotes = $("#txtAddressNotes").val();

    if (checkflag == true) {

        $.ajax({
            url: '/Organisation/AddOrganisationAddress',
            data: { OrgId: OrgId, TradingName: txtTradingName, Attention: txtAttention, DeliveryAddress: ddlDelivery, Address1: txtAddress1, Address2: txtAddress2, State: ddlStateList, Postcode: txtPostCode, AddNotes: txtAddressNotes, PageSource: PageSource, PurchaseId: PurchaseId, AddressId: AddressId, OppId: OppId },  //tarun 06/9/2018
            async: false,

            success: function (response) {
                if (response.ID != null && response.ID != undefined) {
                    $("#btnsaveAddress").removeClass("customAlertChange");
                    var count = CheckDeliveryAddress(OrgId);
                    GetOrganisationAddress(OrgId, ddlDelivery);
                    $("#ddlDelivery").val(ddlDelivery);
                    $("#HiddenAddressId").val(response.ID);     //18 Sep 2018
                }
                CustomAlert(response);

            },
            type: 'post',

        });
    }
    else
    {
        if (Validcheckflag)
        //CustomWarning('Fill all required fields.');
            CustomErrorCode("Required");
    }
}


function CheckDeliveryAddress(OrgId) {
    var data = "";
    $.ajax({
        url: '/Organisation/CheckDeliveryAddress',
        data: { OrgId: OrgId },
        async: false,

        success: function (response) {
            data = response;
            var HtmlData = "<option value=''>--Select--</option>";
            if (response <= 5) {
                var count = 0;
                if (response == 5) {
                    count = parseInt(response);
                } else {
                    count = parseInt(response) + 1;
                }

                for (i = 1; i <= count; i++) {
                    HtmlData += "<option value='Delivery" + i + "'>Delivery" + i + "</option>";
                }

                $("#ddlDelivery").html(HtmlData);
            }
        },
        type: 'post',

    });
    return data;
}

function GetOrganisationAddress(OrgId, AddType) {
    $.ajax({
        url: '/Organisation/GetOrganisationAddress',
        data: { OrgId: OrgId, AddType: AddType },
        async: false,

        success: function (response) {

            //$('#HiddenOrgIdforAddress').val(response.AddressId)  //tarun 06/09/2018

            $("#txtAttention").val('');
            $("#ddlDelivery").val('');
            $("#txtAddress1").val('');
            $("#txtAddress2").val('');
            $("#ddlStateList").val('');
            $("#txtPostCode").val('');
            $("#txtAddressNotes").val('');
            $("#HiddenAddressId").val('');      //18 Sep 2018

            if (response != null && response != "undefined") {
                var data = response;
                $("#txtTradingName").val(data.TradingName);
                $("#txtAttention").val(data.Attention);
                $("#ddlDelivery").val(data.DeliveryAddress);
                $("#txtAddress1").val(data.Address1);
                $("#txtAddress2").val(data.Address2);
                $("#ddlStateList").val(data.State);
                $("#txtPostCode").val(data.Postcode);
                $("#txtAddressNotes").val(data.AddNotes);
                $("#HiddenAddressId").val(data.AddressId);      //18 Sep 2018
            }
        },
        error: function (response) {
            //tarun 12/09/2018
            if ($('#PageName').val() != "PurchaseDetails") {
                $('#txtTradingName').val($('#txtTradeName').val());
            }
            else {
                $('#txtTradingName').val('');
            }
           //end 
            $("#txtAttention").val('');
            $("#ddlDelivery").val('');
            $("#txtAddress1").val('');
            $("#txtAddress2").val('');
            $("#ddlStateList").val('');
            $("#txtPostCode").val('');
            $("#txtAddressNotes").val('');
        },
        type: 'post',

    });
}
function GetOrganisationById(OrgId) {
    if ($('#PageName').val() != "JobDetails" && $('#PageName').val() != "Opportunity") {
        $.ajax({
            url: '/Organisation/GetOrganisationById',
            data: { OrgId: OrgId },
            async: false,

            success: function (response) {
                var data = response;
                //alert(JSON.stringify(data));
                $("#txtorgName").val(data.OrgName);
                $("#txtTradeName").val(data.TradingName);
                $("#txtABNName").val(data.ABN);
                $("#txtMainPhone").val(data.MainPhone);
                $("#txtWebAddress").val(data.WebAddress);
                $("#txtbrand").val(data.Brand);
                $("#txtOrgNotes").val(data.OrgNotes);
                $("#hdnOrgId").val(data.OrgId);
                $('#txtOrgType').val(data.OrganisationType);
                SetAccountManager(data.AcctMgrId, "ddlOrgAcctMgr", "AccountManager");
               // $('#ddlOrgAcctMgr').val(data.AcctMgrId);
                $('#txtorgEmail').val(data.EmailAddress);   //21 June 2019 (N)
                MakeorgReadonly();

                if (data.OrgId != null && data.OrgId != undefined && $("#PageName").val() == "OrganisationDetail") {
                    GetContactByOrgId(data.OrgId);
                }

                // $("#txtTradingName").val(data.TradingName);

                //tarun 07/09/2018
                if ($('#PageName').val() != "PurchaseDetails" && $('#PageName').val() != "InvoicingDetails" && $('#PageName').val() != "PackingDetails" && $('#PageName').val() != "JobDetails" && $("#PageName").val() != "CompleteDetails" && $('#PageName').val() != "ShippingDetails" ) {
                    var count = CheckDeliveryAddress(data.OrgId);

                    GetOrganisationAddress(data.OrgId, "");
                }
                //end

            },
            error: function (response) {
                alert(response);
            },
            type: 'post',

        });
    }
    // Tarun 21Sept
    //else if($('#PageName').val() == "JobDetails")
    //{
    //    $('#HiddenforDefaultOrgID').val(OrgId);
    //    var count = CheckDeliveryAddress(OrgId);
    //    GetOrganisationAddress(OrgId, "");
    //}
}

function OpenOrganisation() {
    if ($('#hdnOrgId').val() != "") {
        location.href = "/Organisation/OrganisationDetails/" + $('#hdnOrgId').val();
    }
    else {
        CustomWarning("Select Organisation First");
    }
}
function ChangeTradingName() {
    $('#txtTradeName').val($('#txtorgName').val());
    $('#txtTradingName').val($('#txtorgName').val());

}

// 18 Sep 2018 (N)
function PrintAddressPdf() {
    if ($('#PageName').val() == "InvoicingDetails" || $('#PageName').val() == "PackingDetails" || $('#PageName').val() == "JobDetails" || $('#PageName').val() == "ShippingDetails" || $('#PageName').val() == "CompleteDetails") {
        var oppid = $('#lblOpportunityId').text();
        oppid = parseInt(oppid);
        var data;
        $.ajax({
            url: '/Organisation/GetAddressStatus',
            data: { OppId: oppid },
            async: false,
            type: 'POST',
            success: function (response) {
                if (response == true) {
                    if (oppid != "" && oppid != undefined) {
                        window.open('/Organisation/PrintAddressPdf/?oppid=' + oppid, '_blank');
                    }
                    else {
                        CustomWarning('Please fill or save address first');
                    }
                }
                else {
                    CustomWarning('Please Associate the Address with the Opportunity first.');
                }
            },
            error: function (response) {
                data = response;
               // alert(response);
            },
        })
    }
    else {
        CustomWarning('You cant access the Information at this Stage.');
    }
}
// 18 Sep 2018 (N)
