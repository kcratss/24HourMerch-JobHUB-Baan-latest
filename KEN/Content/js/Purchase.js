var PurchaseChangesValues = [];

$(function () {

    $(".CatchPurchaseChange input,.CatchPurchaseChange select").focus(function (event) {
        PurchaseChangesValues.push({ id: $(this).attr('id'), value: $(this).val() });
    });

    $(".CatchPurchaseChange input,.CatchPurchaseChange select").blur(function (event) {
        var OldValues = PurchaseChangesValues.find(x=>x.id == $(this).attr('id'));
        var index = PurchaseChangesValues.findIndex(x=>x.id == $(this).attr('id'));
        if (OldValues != null && OldValues != undefined) {

            if (OldValues.value != $(this).val()) {
                $("#btnsavePurchase").addClass("customAlertChange");

            }
            PurchaseChangesValues.splice(index, 1);
        }
    });

    $('#txtOrgType').val('Supplier');
    if ($('#PageName').val() == "PurchaseDetails") {
        $('#lblOrg').text('Supplier');
        //$('#txtPurchaseStatus').text('Ordered');
    }
    if ($('#PageName').val() == "PurchaseDetails") {
        //$('#lblAddress').text('Deliver to');
        $('#lblAddress').text('Ship To');
    }
    if ($('#PageName').val() == "PurchaseDetails") {
        $('#txtOrgType').attr('disabled', true).addClass('MakeReadonly');
        //$('#txtContactEmail').attr('disabled', true).addClass('MakeReadonly');
    }
    // baans change 15th November for user by title
    if ($("#PageName").val() == "PurchaseDetails" || $("#PageName").val() == "PurchaseList" || $("#PageName").val() == "PaymentReport" || $("#PageName").val() == "SalesReport" || $("#PageName").val() == "ValueConversionReport" || $("#PageName").val() == "OpportunityValueConversionReport" || $("#PageName").val() == "ManagerStageWiseReport") {
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

    }
    // baans end 15th November
    //7 Sep 2018 (N)
    $("#Purchfromdate").datepicker(
        {
            //dateFormat: 'dd/mm/yy',
            dateFormat: 'dd-mm-yy',
            onSelect: function (dateText) {
                ValidDate('Purchfromdate');
            }
        }
    );

    $("#Purchtodate").datepicker(
        {
            dateFormat: 'dd-mm-yy',
            onSelect: function (dateText) {
                ValidDate('Purchtodate');
            }
        }
    );
    //7 Sep 2018 (N)

    //tarun 11 Sep 2018

    $("#requiredbydate").datepicker(
        {
            dateFormat: 'dd/mm/yy',
            onSelect: function (dateText) {

                SetDate('requiredbydate');
            }
        }
    );
    $("#txtPurchaseDate").datepicker(
        {
            dateFormat: 'dd/mm/yy',
            onSelect: function (dateText) {

                SetDate('txtPurchaseDate');
            }
        }
    );
    $("#BillDate").datepicker(
        {
            dateFormat: 'dd/mm/yy',

        }
        //end
    );
    //tarun 11/09/2018
    $("#txtPurchaseStatus").on("change", function () {
        if ($(this).val() == "Billed") {
            $("#billVal").css("display", "block");
            $("#billdateVal").css("display", "block");
            $("#shipchargeVal").css("display", "block");
            var BillNo, BillDate, ShippingCharge;
            if ($("#BillNo").val() != "") {
                BillNo = $("#BillNo").val();
                //$("#BillNo").css("border-color", "rgba(204, 204, 204, 1)");
                var filter = /^[^-\s][a-zA-Z0-9_.\s-]+$/;
                if (filter.test($('#BillNo').val())) {
                    $("#BillNo").removeClass("customAlertChange");
                }
                else {
                    $("#BillNo").addClass("customAlertChange");
                }
                //$("#BillNo").removeClass('customAlertChange');
            } else {
                //$("#BillNo").css("border-color", "red");
                $("#BillNo").addClass('customAlertChange');
                checkflag = false;
            }
            if ($("#BillDate").val() != "") {
                BillDate = $("#BillDate").val();
                //$("#BillDate").css("border-color", "rgba(204, 204, 204, 1)");
                $("#BillDate").removeClass('customAlertChange');
            } else {
                //$("#BillDate").css("border-color", "red");
                $("#BillDate").addClass('customAlertChange');
                checkflag = false;
            }
            if ($("#ShippingCharge").val() != "") {
                ShippingCharge = $("#ShippingCharge").val();
                //$("#ShippingCharge").css("border-color", "rgba(204, 204, 204, 1)");
                $("#ShippingCharge").removeClass('customAlertChange');
            } else {
                //$("#ShippingCharge").css("border-color", "red");
                $("#ShippingCharge").addClass('customAlertChange');
                checkflag = false;
            }
            //$("#txtshippingday").val($(this).val());
            //$("#txtshippingprice").val('0');
        }
        else {
            $("#billVal").css("display", "none");
            $("#billdateVal").css("display", "none");
            $("#shipchargeVal").css("display", "none");
            //$("#BillNo").css("border-color", "rgba(204, 204, 204, 1)");
            $("#BillNo").removeClass('customAlertChange');
            //$("#BillDate").css("border-color", "rgba(204, 204, 204, 1)");
            $("#BillDate").removeClass('customAlertChange');
            //$("#ShippingCharge").css("border-color", "rgba(204, 204, 204, 1)");
            $("#ShippingCharge").removeClass('customAlertChange');
        }
        //end
        //else {
        //    $("#txtshippingday").val('');
        //    if ($("#txtshippingprice").val() == "0") {
        //        $("#txtshippingprice").val('');
        //    }
        //}
    });

    if ($('#PageName').val() == "PurchaseDetails") {

        $.ajax({
            url: '/Organisation/GetOrgByType',
            async: false,

            success: function (response) {
                $('#HiddenPurchaseOrg').val(response.OrgId);
                CheckDeliveryAddress(response.OrgId);
                GetOrganisationAddress(response.OrgId, "");
            }
        })
    }

    $(".tablinks").click(function () {
        PurchaseList($(this).attr('id'));
    });

    $("#btnFindPurchase").click(function () {
        $(".tablinks").each(function () {
            if ($(this).hasClass('active')) {
                PurchaseList($(this).attr('id'));
            }
        });
    });

    $("#txtShippingIn").on("change", function () {
        if ($(this).val() == "PickUp") {
            $("#ShippingCharge").val('0');
        }
        else {
            if ($("#ShippingCharge").val() == "0") {
                $("#ShippingCharge").val('');
            }
        }
    });

    $('#BillNo').keyup(function () {
        var billnumber = $('#BillNo').val();
        if (billnumber.length > 10) {
            $('#BillNo').trigger('blur');
            $('#BillNo').val(billnumber.substr(0,10))
            CustomWarning("Only 10 characters are allowed in bill number!")
        }
    });

});

function GetPurchById(Id, OppId) {
    if (Id != undefined) {                     /*&& Id != 0*/
        $.ajax({
            url: '/Purchase/GetPurchById',
            async: false,
            data: { Id: Id, OppId: OppId },
            success: function (response) {

                var CurrentPurchaseDate = new Date();
                var CurrentRequiredDate = new Date();
                var nextrequireddate = parseInt(CurrentRequiredDate.getDate()) + 2;
                CurrentRequiredDate.setDate(nextrequireddate)
                $('#txtPurchaseDate').val(GetddmmyyFormattedDate(CurrentPurchaseDate));
                $('#requiredbydate').val(GetddmmyyFormattedDate(CurrentRequiredDate));

                if (response.data != null) {
                    //tarun 04/09/2018
                  
                    $('#lblpurch').text(("00000" + response.data.PurchaseId).substr(-6));
                    $('#txtPurchaseDate').val(DateFormat(response.data.Purchasedate));
                    $('#requiredbydate').val(DateFormat(response.data.RequiredByDate));
                    $('#ShippingCharge').val(response.data.ShippingCharge);
                    $('#BillNo').val(response.data.BillNo);
                    $('#BillDate').val(DateFormat(response.data.BillDate));
                    //tarun 11/09/2018
                    $('#txtPurchaseStatus').val(response.data.PurchStatus);
                    if (response.data.PurchStatus == "Billed") {
                        $('#txtPurchaseStatus').attr('disabled', true);
                        $('#btnBill').attr('disabled', true);
                    }
                    $('#txtShippingIn').val(response.data.ShippingIn);
                    //end
                    //$('#HiddenPurchaseid').val(response.data.PurchaseId);

                    //tarun 07/09/2018
                    GetPurchaseDetailOptionGrid(response.data.PurchaseId);
                    if (response.data.OrgId != null) {
                        GetOrganisationById(response.data.OrgId);
                    }
                    if (response.data.DeliveryToId != null) {
                        //var address=CheckDeliveryAddress(response.data.OrgId);
                        GetOrganisationAddress(response.data.DeliveryToId, "Purchase");
                    }
                    //end
                }
                //GetContactByOppId($('#HiddenOppid').val(response.OpportunityId))
                //GetOrganisationById(response.OrgID);
                if (response.model != null) {
                    //$('#txtContactEmail').val(response.model.email);      //21 June 2019 (N)
                    $('#HiddenOppid').val(response.model.OpportunityId);
                    $('#purchjobname').val(response.model.OppName);
                    $('#qtyreq').val(response.model.Quantity);
                    $('#txtDept').val(response.model.DepartmentName);

                    if (response.data != null) {
                        $('#purchjobname').val(response.model.OppName);
                        $('#qtyreq').val(response.data.QuantityRequired);
                        $('#txtDept').val(response.data.Depts);
                    }
                  
                    //GetPurchaseDetailOptionGrid(response.model.OpportunityId);
                }
                //$('#HiddenPurchaseid').val(response.data.PurchaseId);
                //GetPurchaseDetailOptionGrid(response.data.PurchaseId);
            }
           
        })
    }

}


function GetPurchaseDetailOptionGrid(id) {
    var data;
    //var PurchaseId = $("#lblpurch").val();
    var PurchaseId = id;
    $.ajax({
        url: '/Purchase/GetPurchaseDetailOptionGrid',
        data: { PurchaseId: PurchaseId },
        async: false,
        success: function (response) {
            data = response.data;

            $('#lblSubTotal').text('$' + response.SubTotal);
            $('#lblShipping').text('$' + response.Shipping);
            $('#lblTax').text('$' + response.Tax);
            $('#lblTotal').text('$' + response.Total);
        },
        Error: function (response) {
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
        columnTemplate: { width: 150, halign: "left" }
    };
    obj.colModel = [
        {
            title: "Option No", dataIndx: "DispalayId", align: "center", width: "6%", dataType: "string"
        },
        {
            title: "Qty", dataIndx: "quantity", width: "3%", align: "right", dataType: "int"
        },
        {
            title: "Brand", dataIndx: "BrandName", width: "5%", dataType: "string"
        },
        {
            title: "Code", dataIndx: "code", width: "3%", dataType: "string"
        },
        {
            title: "Item", dataIndx: "ItemName", width: "7%", dataType: "string"
        },

        {
            title: "Colour", dataIndx: "colour", width: "4%", dataType: "string"
        },
        {
            title: "  Link", dataIndx: "Link", width: "6%", dataType: "string",
            render: function (ui) {
                var data = ui.cellData;
                var label;
                if (data != null && data != undefined) {
                    // baans change 11th Sept for Link
                    //label = "<a onclick = 'window.open('/" + data + "','_blank')' target='_blank'><label class='externalLnk'>" + data + "</label></a>";
                    label = "<a href='http://" + data + "' target='_blank'><label class='externalLnk'>" + data + "</label></a>";
                    // baans end 11th Sept
                }
                else {
                    label = data;
                }
                return label;
            }
        },
        {
            title: "Cost", dataIndx: "Cost", width: "3%", dataType: "float"
        },
        {
            title: "Sizes", dataIndx: "InitialSizes", width: "12%", datatype: "string"
        },
        {
            title: "Item Notes", dataIndx: "ItemNotes", align: "left", width: "24%", dataType: "float"
        },
        {
            title: "Unit Ex GST", dataIndx: "uni_price", width: "6%", dataType: "float"
        },
        {
            title: "Unit+GST", dataIndx: "UnitInclGST", width: "6%", dataType: "float"
        },
        {
            title: "Ext Ex GST", dataIndx: "ExtExGST", width: "6%", dataType: "float"
        },
        {
            title: "Ext+GST", dataIndx: "ExtInclGST", width: "6%", dataType: "float"
        },
        {
            title: "", dataIndx: "", width: "1%",
            render: function (ui) {
                var dataindx = ui.rowIndx;
                var hostname = window.location.origin;
                return "<a onclick='PurchaseDelete(" + dataindx + ")'><img src='" + hostname + "/Content/images/DeleteContact.png' class='internalLnk' style='width:12px'/></a>";
            }
        }
    ];

    obj.rowDblClick = function (evt, ui) {
        var rowIndx = getRowIndx(this);
        if (rowIndx != null) {
            var data = this.pdata[rowIndx];
            if (data != undefined && data != null) {

                $('#txtOptionQty').val(data.quantity);
                $('#txtCode').val(data.code);
                $('#txtColor').val(data.colour);
                $('#txtLink').val(data.Link);
                $('#txtCost').val(data.Cost);
                $('#ddlBrand').val(data.band_id).trigger("chosen:updated");
                $('#ddlItem').val(data.item_id).trigger("chosen:updated");
                $('#txtunitprcExgst').val(data.uni_price);
                $('#ItemNotes').val(data.ItemNotes);
                $('#txtComment').val(data.comment);
                $('#ddlSizeType').val(data.SizeGrid);
                $('#txtSizes').val(data.InitialSizes);
                $('#HiddenPurchaseDetailId').val(parseInt(data.PurchaseDetailId));
                //$('#txtDecorationFront').val(data.Front_decDesign);
                //$('#txtRangeFront').val(data.Front_decQuantity);
                //$('#hiddenRangeFrontCost').val(data.Front_decCost);
                //$('#txtDecorationBack').val(data.Back_decDesign);
                //$('#txtRangeBack').val(data.Back_decQuantity);
                //$('#hiddenRangeBackCost').val(data.Back_decCost);
                //$('#txtDecorationRight').val(data.Right_decDesign);
                //$('#txtRangeRight').val(data.Right_decQuantity);
                //$('#hiddenRangeRightCost').val(data.Right_decCost);
                //$('#txtDecorationLeft').val(data.Left_decDesign);
                //$('#txtRangeLeft').val(data.Left_decQuantity);
                //$('#hiddenRangeLeftCost').val(data.Left_decCost);
                //$('#txtDecorationOther').val(data.Extra_decDesign);
                //$('#txtRangeOther').val(data.Extra_decQuantity);
                //$('#hiddenRangeOtherCost').val(data.Extra_decCost);
                if ($('#ddlSizeType').val() != "" && $('#ddlSizeType').val() != undefined) {
                    GetSize($('#ddlSizeType').val(), 'Edit');
                }
            }

        }


    }

    obj.dataModel = { data: data };

    pq.grid("#PurchaseDetailsGrid", obj);
    pq.grid("#PurchaseDetailsGrid", "refreshDataAndView");
};

function PurchaseOptionCopy() {
    // baans change 29th September for valid option fields
    var OptionIsValid = true;
    var filter = /^[^-\s][a-zA-Z0-9_.\s-]+$/;
    //if (filter.test($('#txtCode').val())) {
    //    $("#txtCode").removeClass("customAlertChange");
    //}
    //else {
    //    OptionIsValid = false;
    //    $("#txtCode").addClass("customAlertChange");
    //}
    //if ($("#ddlSizeType").val() == "Custom" && $("#ddlSizeType").val() != "") {
    //    if (filter.test($('#txtSizes').val())) {
    //        $("#txtSizes").removeClass("customAlertChange");
    //    }
    //    else {
    //        OptionIsValid = false;
    //        $("#txtSizes").addClass("customAlertChange");
    //    }
    //}
    if (OptionIsValid) {
        var datapurchId = $('#HiddenPurchaseDetailId').val();
        if ($('#HiddenPurchaseDetailId').val() != "") {

        UpdatePurchaseOption(0);
    }
    else {
        CustomWarning('Select Option First');
        }
    }
    else {
        CustomWarning('Fill all required fields.')
       // CustomErrorCode("Required");
    }
}

function PurchaseOptionSave() {
    // baans change 29th September for valid option fields
    var OptionIsValid = true;
    var filter = /^[^-\s][a-zA-Z0-9_.\s-]+$/;
    //if (filter.test($('#txtCode').val())) {
    //    $("#txtCode").removeClass("customAlertChange");
    //}
    //else {
    //    OptionIsValid = false;
    //    $("#txtCode").addClass("customAlertChange");
    //}
    //if ($("#ddlSizeType").val() == "Custom" && $("#ddlSizeType").val() != "") {
    //    if (filter.test($('#txtSizes').val())) {
    //        $("#txtSizes").removeClass("customAlertChange");
    //    }
    //    else {
    //        OptionIsValid = false;
    //        $("#txtSizes").addClass("customAlertChange");
    //    }
    //}
    if (OptionIsValid) {
        UpdatePurchaseOption($('#HiddenPurchaseDetailId').val())
    }
    else {
        CustomWarning('Fill all required fields.');
    }
}
function UpdatePurchaseOption(PurchaseDetailId) {
    // baans change 11th Sept for option Calculation
    var OptionSizes = $('#txtSizes').val();
    var OptionQuan = parseInt($('#txtOptionQty').val());
    var OptionQuanCheckNaN = isNaN(OptionQuan);
    var total = 0;
    var IsValid = true;
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
    if (total != OptionQuan && $("#ddlSizeType").val() != "Custom" && OptionQuanCheckNaN == false) {
        $("#txtSizes").addClass('customAlertChange');
        $("#txtOptionQty").addClass('customAlertChange');
        CustomError("Sizes should be equal to Option Quantity !!!");
        IsValid = false;
    }
    if (IsValid) {

    var IsInsert = true;
    var checkflag = true;

    var txtOptionQty, txtCode, txtColor, txtLink, txtCost, txtMargin, ddlBrand, ddlItem, txtShipping, txtunitprcExgst, ddlinclude, txtItemNotes, txtPrice, txtComment, /*ddlservice,OptionStage,*/ ddlSizeType, txtSizes;

    //if (PurchaseDetailId != "") {     31 May 2019(N)

        //|| ($('#PageName').val() != "ShippingDetails" && $('#PageName').val() != "InvoicingDetails" && $('#PageName').val() != "CompleteDetails" && $('#PageName').val() != "PackingDetails")

        if ($("#txtCode").val() != "") {
            txtCode = $("#txtCode").val();
            $("#txtCode").removeClass('customAlertChange');
        } else {
            $("#txtCode").addClass('customAlertChange');
            checkflag = false;
        }

        if ($("#txtOptionQty").val() != "" && $("#txtOptionQty").val() != "0" && parseInt($("#txtOptionQty").val()) > 0) {
            txtOptionQty = $("#txtOptionQty").val();
            $("#txtOptionQty").removeClass('customAlertChange');
        } else {
            $("#txtOptionQty").addClass('customAlertChange');
            checkflag = false;
        }
        if ($("#ddlBrand").val() != "") {
            ddlBrand = $("#ddlBrand").val();
            $("#ddlBrand_chosen .chosen-single").removeClass('customAlertChange');
        } else {
            $("#ddlBrand_chosen .chosen-single").addClass('customAlertChange');
            checkflag = false;
        }
        if ($("#ddlItem").val() != "") {
            ddlItem = $("#ddlItem").val();
            $("#ddlItem_chosen .chosen-single").removeClass('customAlertChange');
        } else {
            $("#ddlItem_chosen .chosen-single").addClass('customAlertChange');
            checkflag = false;
        }
    //}         31 May 2019(N)
    txtColor = $("#txtColor").val();
    txtLink = $("#txtLink").val();
    txtCost = $("#txtCost").val();
    ddlSizeType = $("#ddlSizeType").val();
    txtSizes = $("#txtSizes").val();
    txtItemNotes = $("#ItemNotes").val();
    txtComment = $("#txtComment").val();
    txtunitprcExgst = $("#txtunitprcExgst").val();

    if (IsInsert || PurchaseDetailId != "") {
        if (checkflag == true) {

            var PuchaseId = $('#lblpurch').text();

            var model = {
                "model": { PurchaseDetailId: PurchaseDetailId, PurchaseId: PuchaseId, OpportunityId:$('#HiddenOppid').val(), quantity: txtOptionQty, code: txtCode, band_id: ddlBrand, item_id: ddlItem, colour: txtColor, comment: txtComment, SizeGrid: ddlSizeType, Link: txtLink, Cost: txtCost, /*Margin: txtMargin,*/ InitialSizes: txtSizes, Shipping: txtShipping, Price: txtPrice, ItemNotes: txtItemNotes, include: ddlinclude, uni_price: txtunitprcExgst /*Service: ddlservice, Front_decDesign: $('#txtDecorationFront').val(), Back_decDesign: $('#txtDecorationBack').val(), Left_decDesign: $('#txtDecorationLeft').val(), Right_decDesign: $('#txtDecorationRight').val(), Extra_decDesign: $('#txtDecorationOther').val(), Front_decQuantity: $('#txtRangeFront').val(), Back_decQuantity: $('#txtRangeBack').val(), Left_decQuantity: $('#txtRangeLeft').val(), Right_decQuantity: $('#txtRangeRight').val(), Extra_decQuantity: $('#txtRangeOther').val(), Front_decCost: $('#hiddenRangeFrontCost').val(), Back_decCost: $('#hiddenRangeBackCost').val(), Left_decCost: $('#hiddenRangeLeftCost').val(), Right_decCost: $('#hiddenRangeRightCost').val(), Extra_decCost: $('#hiddenRangeOtherCost').val(), OpportunityId: $('#HiddenOppid').val(), front_decoration: $('#FrontDecorationID').val(), back_decoration: $('#BackDecorationID').val(), left_decoration: $('#LeftDecorationID').val(), right_decoration: $('#RightDecorationID').val(), extra_decoration: $('#OtherDecorationID').val(), OptionStage: OptionStage*/ },

            }
            $.ajax({
                url: '/Purchase/UpdatePurchaseOption',
                data: model,
                async: false,
                success: function (response) {
                    if (response.ID != null && response.ID != undefined) {

                        //$('#HiddenPurchaseDetailId').val(response.ID); 31 May 2019(N)
                        GetPurchaseDetailOptionGrid($('#lblpurch').text());

                    }
                    if (response.Result == "Success") {
                        OptionFormReset();
                        $("#btnOptionSave").removeClass("customAlertChange");
                    }
                    CustomAlert(response);
                    // baans change 12th Sept for Option Calculation
                    $('#txtTotalSize').val('');
                    $("#txtTotalSize").removeClass('customAlertChange');
                    $("#txtSizes").removeClass('customAlertChange');
                    // baans end 12th Sept
                },
                type: 'post',

            });
        }
        else {
            CustomWarning('Fill all required fields.');
        }
    }
}
// baans end 11th September
}

function PurchaseDelete(rowIndx) {

    bootbox.confirm("Do you want to delete the Purchase Option?", function (result) {
            if (result) {
                //DeletePurchaseOption(DeleteId);

                var rowdata = $('#PurchaseDetailsGrid').pqGrid("getRowData", { rowIndx: rowIndx });
                var Id = rowdata.PurchaseDetailId;

                $.ajax({
                    url: '/Purchase/PurchaseDelete',
                    async: false,
                    data: { PurchaseDetailId: Id },
                    success: function (response) {
                        GetPurchaseDetailOptionGrid($('#lblpurch').text());
                    },
                    type:"Post",
                });
            }

        });
}

function SavePurchaseDetails() {
    // baans change 29th September for valid option fields
    var PurchaseIsValid = true;
    var filter = /^[^-\s][a-zA-Z0-9_.,\s-]+$/; 
    if (filter.test($('#txtDept').val())) {
        $("#txtDept").removeClass("customAlertChange");
    }
    else {
        PurchaseIsValid = false;
        $("#txtDept").addClass("customAlertChange");
    }
    if ($('#txtPurchaseStatus').val() == "Billed") {
        if (filter.test($('#BillNo').val())) {
            $("#BillNo").removeClass("customAlertChange");
        }
        else {
            PurchaseIsValid = false;
            $("#BillNo").addClass("customAlertChange");
        }
    }

    if (PurchaseIsValid) {
        SavePurDetail($('#lblpurch').text());
    }
    else {
       CustomWarning('Fill all required fields.')
        //CustomErrorCode("Required");
    }
}
function SavePurDetail(id) {
    //tarun 11/09/2018
    var txtRequiredByDate, txtPurchaseDate, BillNo, BillDate, ShippingCharge;
    var checkflag = true;

    if ($("#requiredbydate").val() != "") {
        txtRequiredByDate = $("#requiredbydate").val();
        //$("#requiredbydate").css("border-color", "rgba(204, 204, 204, 1)");
        $("#requiredbydate").removeClass('customAlertChange');
    } else {
        //$("#requiredbydate").css("border-color", "red");
        $("#requiredbydate").addClass('customAlertChange');
        checkflag = false;
    }
    if ($("#txtPurchaseDate").val() != "") {
        txtPurchaseDate = $("#txtPurchaseDate").val();
        //$("#txtPurchaseDate").css("border-color", "rgba(204, 204, 204, 1)");
        $("#txtPurchaseDate").removeClass('customAlertChange');
    } else {
        //$("#txtPurchaseDate").css("border-color", "red");
        $("#txtPurchaseDate").addClass('customAlertChange');
        checkflag = false;
    }
    // end
    if (id == 0) {
        $('#lblpurch').text("00000");
    }

    var PurchaseDate, RequiredByDate, ShippingIn, ShippingCharge, BillNo, BillDate, JobName, DepartmentName, QuantityRequired, OppId, PurchStatus, PurchaseNotes, WebOrderNo, DeliveryToId;

    PurchaseDate = GetDate($('#txtPurchaseDate').val());
    RequiredByDate = GetDate($('#requiredbydate').val());
    PurchStatus = $('#txtPurchaseStatus').val();
    //tarun 11/09/2018
    if (PurchStatus == "Billed") {
        $("#billVal").css("display", "block");
        $("#billdateVal").css("display", "block");
        $("#shipchargeVal").css("display", "block");
        if ($("#BillNo").val() != "") {
            BillNo = $("#BillNo").val();
            //$("#BillNo").css("border-color", "rgba(204, 204, 204, 1)");
            $("#BillNo").removeClass('customAlertChange');
        } else {
            //$("#BillNo").css("border-color", "red");
            $("#BillNo").addClass('customAlertChange');
            checkflag = false;
        }
        if ($("#BillDate").val() != "") {
            BillDate = $("#BillDate").val();
            //$("#BillDate").css("border-color", "rgba(204, 204, 204, 1)");
            $("#BillDate").removeClass('customAlertChange');
        } else {
            //$("#BillDate").css("border-color", "red");
            $("#BillDate").addClass('customAlertChange');
            checkflag = false;
        }
        if ($("#ShippingCharge").val() != "") {
            ShippingCharge = $("#ShippingCharge").val();
            //$("#ShippingCharge").css("border-color", "rgba(204, 204, 204, 1)");
            $("#ShippingCharge").removeClass('customAlertChange');
        } else {
            //$("#ShippingCharge").css("border-color", "red");
            $("#ShippingCharge").addClass('customAlertChange');
            checkflag = false;
        }
    }
    else {
        $("#billVal").css("display", "none");
        $("#billdateVal").css("display", "none");
        $("#shipchargeVal").css("display", "none");
        //$("#BillNo").css("border-color", "rgba(204, 204, 204, 1)");
        $("#BillNo").removeClass('customAlertChange');
        //$("#BillDate").css("border-color", "rgba(204, 204, 204, 1)");
        $("#BillDate").removeClass('customAlertChange');
        //$("#ShippingCharge").css("border-color", "rgba(204, 204, 204, 1)");
        $("#ShippingCharge").removeClass('customAlertChange');
    }
    //end
    ShippingIn = $('#txtShippingIn').val();
    ShippingCharge = $('#ShippingCharge').val();
    BillNo = $('#BillNo').val();
    BillDate = GetDate($('#BillDate').val());
    JobName = $('#purchjobname').val();
    DepartmentName = $('#txtDept').val();
    QuantityRequired = $('#qtyreq').val();
    OppId = $('#HiddenOppid').val();
    PurchaseNotes = $('#PurchaseNotes').val();
    WebOrderNo = $('#txtweborder').val();

    DeliveryToId = $('#HiddenAddressId').val();

    if(checkflag == true) {

        $.ajax({
            url: '/Purchase/SavePurDetail',
            async: false,
            data: { PurchaseId: id, PurchaseDate: PurchaseDate, RequiredByDate: RequiredByDate, ShippingIn: ShippingIn, ShippingCharge: ShippingCharge, BillNo: BillNo, BillDate: BillDate, JobName: JobName, Depts: DepartmentName, QuantityRequired: QuantityRequired, OpportunityId: OppId, PurchStatus: PurchStatus, PurchaseNotes: PurchaseNotes, WebOrderNo: WebOrderNo, DeliveryToId: DeliveryToId },
            success: function (response) {

                if (response.ID != 0 && response.ID != undefined) {
                    $("#btnsavePurchase").removeClass("customAlertChange");
                    if ($('#lblpurch').text() == "00000") {
                        //history.pushState('', '', '/Purchase/PurchaseDetails/?Id=' + response.ID);
                        history.pushState('', '', '/Purchase/PurchaseDetails/?Id=' + response.ID + "&OppId=" + OppId);
                        //$("#HiddenFOrPrimary").val($("#HiddenContactId").val());
                        $('#lblpurch').text(("00000" + response.ID).substr(-6));
                    }
                    // GetOptionToPurchaseDetail($('#lblpurch').text(), OppId);
                    GetPurchaseDetailOptionGrid(response.ID);
                    $("#btnsavePurchase").removeClass("customAlertChange");
                    //tarun 11/09/2018
                    if (PurchStatus == "Billed") {
                        $('#txtPurchaseStatus').attr('disabled', true);
                    }
                    //end
                }


                CustomAlert(response);
                //$('#lblpurch').val(response.PuchaseId);

            },
            type: 'Post'
        });
    }
    //tarun 11/09/2018
    else {
        CustomWarning('Fill all required fields.');
        //CustomErrorCode("Required");
    }
    //end
    
}
//4 Sept 2018(N)
function PerformaInvoice() {
    //location.href = '/Opportunity/PerformaInvoicePdf';
    if ($('#hdnOrgId').val() != "") {
        window.open('/Purchase/PerformaInvoicePdf/?Id=' + $('#lblpurch').text() + '&PdfType=Print&path=""','_blank');
    }
    else {
        CustomWarning('Save the Organisation First!!!');
    }
}
//4 Sept 2018(N)

//6 Sep 2018 (N)
function PurchaseList(PurchaseTabs) {
    var griddata, fromdate, todate, Supplier;
    var supplier = $('#ddlSupplier').val();
    var curr = new Date();
    var count = 0;
    var InputId;
    var dateFlag = false;

    var date1 = $("#Purchfromdate").val();
    var date2 = $("#Purchtodate").val();

    if ($("#Purchfromdate").val() != "") {
        fromdate = GetyyyymmddDate($("#Purchfromdate").val());
        InputId = 'fromdate';
        count++;
    }
    if ($("#Purchtodate").val() != "") {
        todate = GetyyyymmddDate($("#Purchtodate").val());
        InputId = 'todate';
        count++;
    }

    switch (count) {
        case 0:
            var next = new Date();
            var NextWeekDate = parseInt(next.getDate()) - 60;
            next.setDate(NextWeekDate);
            fromdate = GetFormattedDate(next);
            todate = GetFormattedDate(curr);
            dateFlag = true;
            break;

        case 1:
            var ErrField;
            if (InputId == "fromdate") {
                ErrField = "To";
            } else {
                ErrField = "Purchase Date";
            }

            bootbox.confirm(ErrField + " is not filled!</br>Do you want to see Last 2 months Record?", function (result) {
                if (result) {
                    var next = new Date();
                    var NextWeekDate = parseInt(next.getDate()) - 60;
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

    var url = '/Purchase/GetPurchaseList';
    var model = { PurchaseTabs: PurchaseTabs, DateFrom: fromdate, DateTo: todate, Supplier: supplier };

    if (PurchaseTabs == "PurchaseCustom") {
        // baans change 04th December for Custom
        //url = '/Purchase/GetCustomPurchaseList';
        //model = { CustomText: $('#searchtextbox').val(), TableName: 'Vw_tblPurchase' }
        var searchdata = $('#searchtextbox').val();
        if (searchdata != "") {
            model = { CustomText: $('#searchtextbox').val(), TableName: 'Vw_tblPurchase' }
            url = '/Purchase/GetCustomPurchaseList';
        }
        else {
            CustomWarning("Please put the search criteria in the search box and click on search button");
        }
        // baans end 4th December
    }

    //if (oppt == "Custom") {
    //    url = '/Opportunity/GetCustomOppList';
    //    model = { CustomText: $('#searchtextbox').val(), TableName: 'Vw_tblOpportunity' };
    //}

    //  $("#ajaxLoader").css("display", "block");
    if (dateFlag == true) {
        //$("#fromdate").datepicker("setDate", fromdate);
        setfromdate = GetddmmyyyyDate(fromdate);
        settodate = GetddmmyyyyDate(todate);
        $("#Purchfromdate").val(setfromdate);
        $("#Purchtodate").val(settodate);

        $.ajax({
            url: url,
            data: model,
            async: false,

            success: function (response) {
                griddata = response;
                //  $("#ajaxLoader").css("display", "none");
            },
            error: function (response) {
                //   $("#ajaxLoader").css("display", "none");
                griddata = response;
            },
            type: 'post',
        });
    }

    var obj = {
        selectionModel: { type: 'row' },
        scrollModel: { pace: 'consistent', horizontal: false },
        pageModel: { type: "local", rPP: 50 },
        strNoRows: 'No records found for the selected Criteria',
        editable: false,
        wrap: false,
        hwrap: false,
        width: "97%",
        height: 750,
        columnTemplate: { width: 150, halign: "left" },
        numberCell: { width: 0, title: "", minWidth: 0 },
        sortable: true,
        numberCell: { width: 0, title: "", minWidth: 0 },
        //sortModel: { cancel: false, type: 'local', sorter: [{ dataIndx: 1, dir: ['up', 'down', 'up'] }] }
    };

    obj.colModel = [

        {
            title: "Purchase No", dataIndx: "DisplayPurchaseId", align: "center", width: "7%", dataType: "string",  
            render: function (ui) {
                var id = ui.rowData.DisplayPurchaseId;
                var OppId = ui.rowData.OpportunityId;
                return "<a href='/Purchase/PurchaseDetails/?id=" + parseInt(id) + "&OppId=" + parseInt(OppId) + "'><label class='internalLnk'>" + id + "</label></a>";
            },

        },
        {
            title: "Date", dataIndx: "Purchasedate", width: "6%", align: "left", datatype: "date",  //tarun 22/09/2018
            render: function (ui) {
                var date = ui.cellData;
                if (date != null && date != undefined) {
                    //var nowDateopp = new Date(parseInt(date.substr(6)));
                    //return ('00' + nowDateopp.getDate()).substr(-2) + '/' + ('00' + (nowDateopp.getMonth() + 1)).substr(-2) + '/' + nowDateopp.getFullYear();  //tarun 22/09/2018
                    // baans change 22nd November
                    return ListDateFormat(date);
                    // baans end 22nd November
                }
            },
        },

        { title: "JobHUB Name", dataIndx: "OppName", width: "10%", dataType: "string", align: "left" },
        { title: "Supplier", dataIndx: "OrgName", width: "10%", dataType: "integer", align: "left" },
        { title: "Invoice No", dataIndx: "OpportunityId", width: "8%", dataType: "string" },
        { title: "Status", dataIndx: "PurchStatus", width: "8%", align: "right", datatype: "string", align: "left" },
        { title: "Purchase Notes", dataIndx: "PurchaseNotes", width: "34%", dataType: "string" },
        { title: "Job Manager", dataIndx: "AccountManagerFullName", width: "10%", dataType: "string", align: "left" },
        { title: "Amount", dataIndx: "AmountTotal", width: "8%", dataType: "int" },
    ];
    //data[0].pq_rowattr = { style: 'background:yellow;' };

    obj.dataModel = { data: griddata };

    pq.grid("#PurchaseList", obj);
    pq.grid("#PurchaseList", "refreshDataAndView");



}

function GetyyyymmddDate(date) {
    //return date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
    var GetDate = date.split('-');

    if (GetDate[1] < 10)
    {
        GetDate[1] = "0" + GetDate[1];
    }
    if (GetDate[0] < 10)  
    {
        GetDate[0] = "0" + GetDate[0];
    }

    return GetDate[2] + "-" + GetDate[1].substr(-2) + "-" + GetDate[0].substr(-2);
}

function GetFormattedDate(date) {
    //Get Current date Without Time
    return date.getFullYear() + '-' + ('00' + (date.getMonth() + 1)).substr(-2) + '-' + date.getDate();     //tarun 22/09/2018
}

function GetddmmyyFormattedDate(date) {
    return date.getDate() + '/' + ('00' + (date.getMonth() + 1)).substr(-2) + '/' + date.getFullYear();
}

function GetddmmyyyyDate(date) {
    //For Setting Date In Datepicker(Entered date Will be yy-mm-dd)
    var GetDate = date.split('-');
    return GetDate[2] + "-" + GetDate[1] + "-" + GetDate[0];
}
function ValidDate(InputID) {
    var DepReqDate = GetyyyymmddDate($('#Purchfromdate').val());     //10 Sep 2018 (N)
    var RequiredbyDate = GetyyyymmddDate($('#Purchtodate').val());       //10 Sep 2018 (N)
    if (RequiredbyDate != "" && RequiredbyDate != undefined && RequiredbyDate != null && DepReqDate != "" && DepReqDate != undefined && DepReqDate != null) {
        var RequiredbyDatenew = new Date(RequiredbyDate);
        var DepReqDatenew = new Date(DepReqDate);
        if (RequiredbyDatenew <= DepReqDatenew) {
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
//6 Sep 2018 (N)

//tarun 11/09/2018
function SetDate(InputID) {
    var RequiredbyDate = GetDate($('#requiredbydate').val());
    var PurchaseDate = GetDate($('#txtPurchaseDate').val());
    if (RequiredbyDate != "" && RequiredbyDate != undefined && RequiredbyDate != null && PurchaseDate != "" && PurchaseDate != undefined && PurchaseDate != null) {
        var RequiredbyDatenew = new Date(RequiredbyDate);
        var PurchaseDatenew = new Date(PurchaseDate);
        if (RequiredbyDatenew <= PurchaseDatenew) {
            $('#' + InputID).val('');
            if (InputID == "requiredbydate") {
                CustomWarning('Required By Date should be greater than Purchase Date');
            }
            else {
                CustomWarning('Purchase Date should be less than Required By Date');
            }
        }

    }
}
//end

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
// baans change 13th Sept for open brandModel
function ShowBrandModalPurchase() {
    $('#txtNewBrand').val('');
    var Data = $('#ddlBrand').val().value;
    // baans 19th Sept
    var NewBrand = "Add New";
    $.ajax({
        url: '/Purchase/GetBrandId',
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
                    $("#ddlBrand").append($('<option></option>').attr("value", response.ID).text(response.Result));
                    var Res = { Result: "Success", Message: response.Message };
                    CustomAlert(Res);

                }
            },
            type: 'post',

        });
    }
}
// baans end 13th Sept

function OpenPurchaseEmail() {
    $.ajax({
        url: "/Purchase/GetPurchaseEmailContent",
        data: { OpportunityId: $('#HiddenOppid').val(), PurchaseId: $('#lblpurch').text() },
        async: false,
        success: function (response) {
            $('#txtMailMessage2').val(response.Body1 + "\n \n" + response.Body2);//(br2nl(response.Body1) + "\n \n" + br2nl(response.Body2));
            $('#txtMailSubject').val(response.Subject);
            $('#txtToMail').val(response.ClientEmailID);

            $('#EmailModel').css('display', 'block');
        },
        type: "Post",
    });
    $('#EmailModel').css('display', 'block');
}

function PurchaseMail() {
    $('#ajaxLoader').show();
    setTimeout(function () {
        SendPurchaseEmail();
    }, 1);
}

function SendPurchaseEmail() {
    $('#ajaxLoader').css("display","block");
    $.ajax({
        url: "/Purchase/SendPurchaseEmail",
        data: { "model": { Email: $('#txtToMail').val(), Subject: $('#txtMailSubject').val(), MailMessage2: $('#txtMailMessage2').val(), Type: 'Email', }, "OpportunityId": $('#HiddenOppid').val(), "PurchaseId": $('#lblpurch').text() },
        async: false,
        success: function (response) {
            if (response.Result == "Success") {
                $('#ajaxLoader').css('display', 'none');
                $('#EmailModel').css('display', 'none');
                CustomAlert(response)
            }            
        },

        error: function (response) {
            alert(response);
        },
        type:"Post",
    });
}

function PurchaseToQuickBooks() {
    $('#ajaxLoader').show();
    setTimeout(function () {
       // SubmitPurchaseBillToQuickBooks();
    }, 1);
}

function SubmitPurchaseBillToQuickBooks() {
    var flag = true;
    $('#ajaxLoader').css('display', 'block');

    if ($('#BillNo').val() != undefined && $('#BillNo').val() != "") {
        $('#BillNo').removeClass('customAlertChange');
        flag = true;
    }
    else{
        $('#BillNo').addClass('customAlertChange');
        flag = false;
    }

    if($('#BillDate').val() != undefined && $('#BillDate').val() != ""){
        $('#BillDate').removeClass('customAlertChange');
        flag = true;
    }
    else{
        $('#BillDate').addClass('customAlertChange');
        flag = false;
    }

    if(flag == true) {
        var AuthResult;
        
        $.ajax({
            url: '/Opportunity/QuickBooksAuthentication',
            data: { id: $('#lblOpportunityId').text(), PageSource: $('#PageName').val() },
            async: false,
            success: function (response) {
                AuthResult = response;
                if (!response.result) {
                    location.href = response.URI;
                }
            },
            error: function (response) {
                alert();
            },
            type: 'post'

        });
        if (AuthResult.result) {
        var OrganisationId = $('#hdnOrgId').val();

        if (OrganisationId != "" && OrganisationId != undefined) {
            $.ajax({
                url: "/Purchase/CheckOrgAddress",
                data: { OrganisationId: OrganisationId },
                async: false,
                success: function (response) {

                    if (response == true) {
                        $('#ajaxLoader').css('display', 'block');
                        $.ajax({
                            url: "/Purchase/SubmitPurchaseBillToQuickBooks",
                            data: { OpportunityId: $('#HiddenOppid').val(), PurchaseId: $('#lblpurch').text() },
                            async: false,
                            success: function (response) {
                                $('#ajaxLoader').css('display', 'none');
                                
                                if (response.Result == "Success") {
                                    SetPurchaseStatus($('#lblpurch').text());
                                }
                                CustomAlert(response);
                            },
                            type: "Post",
                        });
                    }
                    else {
                        $('#ajaxLoader').css('display', 'none');
                        CustomWarning("Address doesnot exist for the Supplier.")
                    }
                },
                type: "Post",
            });
        }
        else {
            $('#ajaxLoader').css('display', 'none');
            CustomWarning("Supplier doesnot exist for this purchase bill.");
        }
       

        }
    }
    else {
        $('#ajaxLoader').css('display', 'none');
        CustomWarning("Bill No. or Bill Date is not filled.");
    }


}

function BackToJob() {
    location.href = "/Opportunity/JobDetails/" + $('#HiddenOppid').val();
}

function SetPurchaseStatus(PurchaseId) {
    var Status = "Billed"
    $('#txtPurchaseStatus').val(Status);
    $('#txtPurchaseStatus').attr('disabled', true);
    $('#btnBill').attr('disabled', true);

    $.ajax({
        url: '/Purchase/SetPurchaseStatus',
        data: { PurchaseId: PurchaseId },
        async: false,
        type: "Post",
        success: function (response) {
            if (response.Result == "Success") {

            }
        }
    });
}