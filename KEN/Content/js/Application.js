var AppChangesValues = [];

$(function () {

    if ($("#PageName").val() == "ApplicationDetails" || $("#PageName").val() == "ApplicationList") {
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

    $(".CatchAppChange input,.CatchAppChange select,.CatchAppChange textarea").focus(function (event) {
        AppChangesValues.push({ id: $(this).attr('id'), value: $(this).val() });
    });

    $(".CatchAppChange input,.CatchAppChange select,.CatchAppChange textarea").blur(function (event) {
        var OldValues = AppChangesValues.find(x=>x.id == $(this).attr('id'));
        var index = AppChangesValues.findIndex(x=>x.id == $(this).attr('id'));
        if (OldValues != null && OldValues != undefined) {

            if (OldValues.value != $(this).val()) {
                $("#btnAppSave").addClass("customAlertChange");

            }
            AppChangesValues.splice(index, 1);
        }
    });

    $(".colortab_change input,.colortab_change select").focus(function (event) {
        AppChangesValues.push({ id: $(this).attr('id'), value: $(this).val() });
    });

    $(".colortab_change input,.colortab_change select").blur(function (event) {
        var OldValues = AppChangesValues.find(x=>x.id == $(this).attr('id'));
        var index = AppChangesValues.findIndex(x=>x.id == $(this).attr('id'));
        if (OldValues != null && OldValues != undefined) {

            if (OldValues.value != $(this).val()) {
                $("#btnColoursave").addClass("customAlertChange");

            }
            AppChangesValues.splice(index, 1);
        }
    });

    if ($('#PageName').val() == "ApplicationDetails") {
        if (PageType == "Custom") {
            $('#tabCustom').css('visibility', 'visible');
            $('#chkCustom').prop('checked', true);
        }
    }

    $('#chkCustom').change(function () {
        if ($('#chkCustom').prop('checked') == true) {
            $('#tabCustom').css('visibility', 'visible');
        }
        else {
            $('#tabCustom').css('visibility', 'hidden');
        }
    });

    $('#div_img').dblclick(function () {
        if ($('#Appimg').attr('src') != '#') {
            $("#div_img").toggleClass('imgEnlarge');
            //alert($('#Appimg').attr('src'));
        }

    });

    $('#drdApplicationType').change(function () {
        if ($('#drdApplicationType').val() == "Embroidery") {
            $('#drdApplicationArt').val('Brilliant');
        }
        else {
            $('#drdApplicationArt').val('Internal');
        }
        ChangeColourTab();
    });

    $('#drdApplicationType').val('Screen Print');
    $('#drdApplicationType').trigger('change');
    $('#drdApplicationProduction').val('Internal');
    //$('#drdApplicationDesigner').val('Nigel');
    $('#drdApplicationDesigner').val('Kenneth'); //Change By Baans 08Aug2020
    $('#drdApplicationArtSupplier').val('Internal Job');
    $('#drdApplicationStatus').val('Active');
    
});

function ShowSupplierModal() {
    $('#txtNewSupplier').val('');

    $.ajax({
        url: '/Application/GetSupplierId',
        data: {},
        async: false,
        success: function (response) {
            if (response != null && response != undefined && response != "") {
                if ($('#drdApplicationArtSupplier').val() == "Add New" && response.SupplierName == "Add New") {
                    $("#SupplierModal").css("display", "block");
                    $('#drdApplicationArtSupplier').val('Select');
                }

            }
        },
        type: 'post',

    });
}

function SaveArtSupplier() {
    if ($('#txtNewSupplier').val() != null) {
        var ArtSupplierName = $('#txtNewSupplier').val();
        $.ajax({
            url: '/Application/SaveArtSupplier',
            data: { ArtSupplierName: ArtSupplierName },
            async: false,
            success: function (response) {
                if (response != null && response != undefined && response != "") {
                    $("#drdApplicationArtSupplier").append($('<option></option>').attr("value", response.ID).text(response.Result)).trigger("chosen:updated");
                    var Res = { Result: "Success", Message: response.Message };
                    CustomAlert(Res);
                }
            },
            type: 'post',

        });
    }
}

function SaveApplication() {
    var isValid = true;
    var filter = /^[^-\s][a-zA-Z0-9_.\s-]+$/;

    var ApplicationId, AppName, DecorationDate, AppType, AppWidth, Art, ArtNotes, Bill, Production, ProductionNotes, ArtSupplier, Designer, DesignerNotes, AppStatus, ProofNotes, AcctMgrId, Link;

    ApplicationId = $('#lblapplicationId').text();
    
    AppName = $('#txtApplicationName').val();
    if (AppName != "" && AppName != undefined) {
        $('#txtApplicationName').removeClass('customAlertChange');
    }
    else {
        isValid = false;
        $('#txtApplicationName').addClass('customAlertChange');
    }

    AppWidth = $('#txtApplicationWidth').val();
    if (AppWidth != "" && AppWidth != undefined && filter.test($('#txtApplicationWidth').val())) {
        $('#txtApplicationWidth').removeClass('customAlertChange');
    }
    else {
        isValid = false;
        $('#txtApplicationWidth').addClass('customAlertChange');
    }

    Bill = $('#txtBill').val();
    if (Bill != "" && Bill != undefined) {
        $('#txtBill').removeClass('customAlertChange');
    }
    else {
        isValid = false;
        $('#txtBill').addClass('customAlertChange');
    }

    AppType = $('#drdApplicationType').val();
    if (AppType == "" || AppType == undefined) {
        isValid = false;
        $('#drdApplicationType').addClass('customAlertChange');
    }
    else {
        $('#drdApplicationType').removeClass('customAlertChange');
    }

    Art = $('#drdApplicationArt').val();
    if (Art == "" || Art == undefined) {
        isValid = false;
        $('#drdApplicationArt').addClass('customAlertChange');
    }
    else {
        $('#drdApplicationArt').removeClass('customAlertChange');
    }

    ArtNotes = $('#txtArtNotes').val();

    Production = $('#drdApplicationProduction').val();
    if (Production == "" || Production == undefined) {
        isValid = false;
        $('#drdApplicationProduction').addClass('customAlertChange');
    }
    else {
        $('#drdApplicationProduction').removeClass('customAlertChange');
    }

    ProductionNotes = $('#txtProductionNotes').val();

    ArtSupplier = $('#drdApplicationArtSupplier').val();
    if (ArtSupplier == "" || ArtSupplier == undefined) {
        isValid = false;
        $('#drdApplicationArtSupplier').addClass('customAlertChange');
    }
    else {
        $('#drdApplicationArtSupplier').removeClass('customAlertChange');
    }

    Designer = $('#drdApplicationDesigner').val();
    if (Designer == "" || Designer == undefined) {
        isValid = false;
        $('#drdApplicationDesigner').addClass('customAlertChange');
    }
    else {
        $('#drdApplicationDesigner').removeClass('customAlertChange');
    }

    DesignerNotes = $('#txtDesignerNotes').val();

    AppStatus = $('#drdApplicationStatus').val();
    if (AppStatus == "" || AppStatus == undefined) {
        isValid = false;
        $('#drdApplicationStatus').addClass('customAlertChange');
    }
    else {
        $('#drdApplicationStatus').removeClass('customAlertChange');
    }

    ProofNotes = $('#txtProofNotes').val();
    Link = $('#txtApplicationlink').val();

    if (isValid == true) {
        $.ajax({
            url: "/Application/SaveApplication",
            data: { ApplicationId: ApplicationId, AppName: AppName, AppType: AppType, AppWidth: AppWidth, Art: Art, ArtNotes: ArtNotes, Bill: Bill, Production: Production, ProductionNotes: ProductionNotes, ArtSupplier: ArtSupplier, Designer: Designer, DesignerNotes: DesignerNotes, AppStatus: AppStatus, ProofNotes: ProofNotes, Link: Link },
            async: false,
            success: function (response) {
                data = response;
                if (data.ID != 0 && data.ID != undefined) {
                    var ReturnAppId = "000000" + data.ID;
                    ReturnAppId = ReturnAppId.substr(ReturnAppId.length - 6, ReturnAppId.length);
                    $('#lblapplicationId').text(ReturnAppId);
                    $("#btnAppSave").removeClass("customAlertChange");
                    $('#hdnApplicationId').val(data.ID);

                    if (OptionId != null && OptionId != "" && OptionId != undefined) {
                        history.pushState('', '', '/Application/ApplicationDetails?Id=' + data.ID + '&PageType=' + PageType + '&OptionId=' + OptionId);
                    }
                    else {
                        history.pushState('', '', '/Application/ApplicationDetails?Id=' + data.ID + '&PageType=' + PageType);
                    }

                }
                CustomAlert(response);
            },
            type: "Post",
        });
    }
    else {
        CustomErrorCode("Required");
    }
}

function GetApplicationById(ApplicationId) {
    $.ajax({
        url: '/Application/GetApplicationById',
        data: { ApplicationId },
        async: false,
        type: "Post",
        success: function (response) {
            data = response;

            if (data.ApplicationId != null && data.ApplicationId != 0) {
                var sts = false;

                var ApplicationId = "000000" + data.ApplicationId;
                ApplicationId = ApplicationId.substr(ApplicationId.length - 6, ApplicationId.length);

                $('#hdnApplicationId').val(data.ApplicationId);

                $('#lblapplicationId').text(ApplicationId);
                $('#txtApplicationName').val(data.AppName);
                $('#lblApplicationDate').text(DateFormat(data.DecorationDate));
                $('#txtApplicationWidth').val(data.AppWidth);
                $('#txtArtNotes').val(data.ArtNotes);
                $('#txtBill').val(data.Bill);
                $('#drdApplicationProduction').val(data.Production);
                $('#txtProductionNotes').val(data.ProductionNotes);
                $('#txtProofNotes').val(data.ProofNotes);
                $('#txtDesignerNotes').val(data.DesignerNotes);
                if (data.AppImage != null && data.AppImage.length > 13) {
                    $('#AppImageName').text(data.AppImage == null ? "" : (data.AppImage).substr(0, 12));
                }
                else {
                    $('#AppImageName').text(data.AppImage == null ? "" : data.AppImage);
                }

                $('#ArtImageName').val(data.AppImage == null ? "" : data.AppImage);

                $('#VectorFileName').val(data.AppVector == null ? "" : data.AppVector);
                if ($('#VectorFileName').val() != "") {
                    $('#vectorDownload').css("visibility", "visible");
                }

                $('#SrcImageName').val(data.SourceImage == null ? "" : data.SourceImage);
                if ($('#SrcImageName').val() != "") {
                    $('#sourceDownload').css("visibility", "visible");
                }
                $('#txtApplicationlink').val(data.Link);

                $('#MockImageName').val(data.MockUpImage == null ? "" : data.MockUpImage);
                if ($('#MockImageName').val() != "") {
                    $('#mockDownload').css("visibility", "visible");
                }

                //Active InActive ApplicationTypes
                $("#drdApplicationType option").each(function () {
                    if ($(this).val() == data.AppType) {
                        sts = true;
                    }
                });
                if (sts == true) {
                    $('#drdApplicationType').val(data.AppType);
                } else {
                    $('#drdApplicationType').append($('<option></option>').attr("value", data.AppType).text(data.AppType));
                    $('#drdApplicationType').val(data.AppType);
                }
                sts = false;

                //Active InActive ApplicationArt
                $("#drdApplicationArt option").each(function () {
                    if ($(this).val() == data.Art) {
                        sts = true;
                    }
                });
                if (sts == true) {
                    $('#drdApplicationArt').val(data.Art);
                } else {
                    $('#drdApplicationArt').append($('<option></option>').attr("value", data.Art).text(data.Art));
                    $('#drdApplicationArt').val(data.Art);
                }
                sts = false;

                //Active InActive ApplicationArtSupplier
                $("#drdApplicationArtSupplier option").each(function () {
                    if ($(this).val() == data.ArtSupplier) {
                        sts = true;
                    }
                });
                if (sts == true) {
                    $('#drdApplicationArtSupplier').val(data.ArtSupplier);
                } else {
                    $('#drdApplicationArtSupplier').append($('<option></option>').attr("value", data.ArtSupplier).text(data.ArtSupplier));
                    $('#drdApplicationArtSupplier').val(data.ArtSupplier);
                }
                sts = false;

                //Active InActive ApplicationDesigner
                $("#drdApplicationDesigner option").each(function () {
                    if ($(this).val() == data.Designer) {
                        sts = true;
                    }
                });
                if (sts == true) {
                    $('#drdApplicationDesigner').val(data.Designer);
                } else {
                    $('#drdApplicationDesigner').append($('<option></option>').attr("value", data.Designer).text(data.Designer));
                    $('#drdApplicationDesigner').val(data.Designer);
                }
                sts = false;

                //Active InActive ApplicationStatus
                $("#drdApplicationStatus option").each(function () {
                    if ($(this).val() == data.AppStatus) {
                        sts = true;
                    }
                });
                if (sts == true) {
                    $('#drdApplicationStatus').val(data.AppStatus);
                } else {
                    $('#drdApplicationStatus').append($('<option></option>').attr("value", data.AppStatus).text(data.AppStatus));
                    $('#drdApplicationStatus').val(data.AppStatus);
                }
                sts = false;

                //Show Application Image
                var hostname = window.location.origin;
                $("#Appimg").attr("src", hostname + "/Content/uploads/Application/" + data.AppImage);

                $('#drdApplicationType').trigger("change");
                GetApplicationColoursGrid(ApplicationId);
            }
        }
    });
}

function ChangeColourTab() {
    var ApplicationType = $('#drdApplicationType').val();

    if (ApplicationType == "Embroidery") {
        $('#lblInkColor').css('display', 'none');
        $('#lblThreadColor').css('display', 'block');
        $('#lblTransferColor').css('display', 'none');

        $('#txtInkColor').css('display', 'none');
        $('#txtThreadColor').css('display', 'block');
        $('#txtTransferColor').css('display', 'none');

        $('#div_Embroidery').css('display', 'block');
        $('#div_ScreenPrint').css('display', 'none');
        $('#div_Transfer').css('display', 'none');

        $('#btnvectordigitize').text('DIGITIZE');
    } 

    else if (ApplicationType == "Screen Print") {
        $('#lblInkColor').css('display', 'block');
        $('#lblThreadColor').css('display', 'none');
        $('#lblTransferColor').css('display', 'none');

        $('#txtInkColor').css('display', 'block');
        $('#txtThreadColor').css('display', 'none');
        $('#txtTransferColor').css('display', 'none');

        $('#div_Embroidery').css('display', 'none');
        $('#div_ScreenPrint').css('display', 'block');
        $('#div_Transfer').css('display', 'none');

        $('#btnvectordigitize').text('VECTOR');
    }

    else if (ApplicationType == "Digital Print") {
        $('#lblInkColor').css('display', 'block');
        $('#lblThreadColor').css('display', 'none');
        $('#lblTransferColor').css('display', 'none');

        $('#txtInkColor').css('display', 'block');
        $('#txtThreadColor').css('display', 'none');
        $('#txtTransferColor').css('display', 'none');

        $('#div_Embroidery').css('display', 'none');
        $('#div_ScreenPrint').css('display', 'none');
        $('#div_Transfer').css('display', 'none');

        $('#btnvectordigitize').text('VECTOR');
    }

    else if (ApplicationType == "Supa Colour Transfer" || ApplicationType == "Ultra Colour Transfer" || ApplicationType == "Vinyl Cut Transfer" || ApplicationType == "Sports Numbers") {
        $('#lblInkColor').css('display', 'none');
        $('#lblThreadColor').css('display', 'none');
        $('#lblTransferColor').css('display', 'block');

        $('#txtInkColor').css('display', 'none');
        $('#txtThreadColor').css('display', 'none');
        $('#txtTransferColor').css('display', 'block');

        $('#div_Embroidery').css('display', 'none');
        $('#div_ScreenPrint').css('display', 'none');
        $('#div_Transfer').css('display', 'block');

        $('#btnvectordigitize').text('VECTOR');
    }

    GetApplicationColoursGrid($('#lblapplicationId').text())
}

function ChangeTabView(TabName) {
    if (TabName == "Colours") {
        $('#tab_Colours').css('display', 'block');
        $('#tab_Custom').css('display', 'none');

        $('#ApplicationColourGrid').css('display', 'block');
        $('#ApplicationJobGrid').css('display', 'none');
        $('#ApplicationCustomGrid').css('display', 'none');
    }
    if (TabName == "Jobs") {
        $('#tab_Colours').css('display', 'none');
        $('#tab_Custom').css('display', 'none');

        $('#ApplicationColourGrid').css('display', 'none');
        $('#ApplicationJobGrid').css('display', 'block');
        $('#ApplicationCustomGrid').css('display', 'none');
        ApplicationJobGrid();
    }
    if (TabName == "Custom") {
        $('#tab_Colours').css('display', 'none');
        $('#tab_Custom').css('display', 'block');

        $('#ApplicationColourGrid').css('display', 'none');
        $('#ApplicationJobGrid').css('display', 'none');
        $('#ApplicationCustomGrid').css('display', 'block');
        GetApplicationCustomGrid();
    }
}

function SaveAppColor() {
    var isValid = true;
    var filter = /^[^-\s][a-zA-Z0-9_.\s-]+$/;

    if ($('#lblapplicationId').text() != 0 && $('#lblapplicationId').text() != "" && $('#lblapplicationId').text() != undefined) {

        if ($('#txtColourWayNo').val() != "" && $('#txtColourWayNo').val() != undefined /*&& filter.test($('#txtColourWayNo').val())*/) {
            $('#txtColourWayNo').removeClass('customAlertChange');
        }
        else {
            isValid = false;
            $('#txtColourWayNo').addClass('customAlertChange');
        }

        if ($('#txtPantone').val() != "" && $('#txtPantone').val() != undefined && filter.test($('#txtPantone').val())) {
            $('#txtPantone').removeClass('customAlertChange');
        }
        else {
            isValid = false;
            $('#txtPantone').addClass('customAlertChange');
        }

        if (($('#txtHexValue').val() != "" && $('#txtHexValue').val() != undefined )) {
            $('#txtHexValue').removeClass('customAlertChange');
        }
        else {
            isValid = false;
            $('#txtHexValue').addClass('customAlertChange');
        }

        //On behalf of ApplicationType
        if ($('#drdApplicationType').val() == "Digital Print" || $('#drdApplicationType').val() == "Screen Print") {
            if ($('#txtInkColor').val() != "" && $('#txtInkColor').val() != undefined && filter.test($('#txtInkColor').val())) {
                $('#txtInkColor').removeClass('customAlertChange');
            }
            else {
                isValid = false;
                $('#txtInkColor').addClass('customAlertChange');
            }
        }
        else if ($('#drdApplicationType').val() == "Embroidery") {
            if ($('#txtThreadColor').val() != "" && $('#txtThreadColor').val() != undefined && filter.test($('#txtThreadColor').val())) {
                $('#txtThreadColor').removeClass('customAlertChange');
            }
            else {
                isValid = false;
                $('#txtThreadColor').addClass('customAlertChange');
            }
        }
        else {
            if ($('#txtTransferColor').val() != "" && $('#txtTransferColor').val() != undefined && filter.test($('#txtTransferColor').val())) {
                $('#txtTransferColor').removeClass('customAlertChange');
            }
            else {
                isValid = false;
                $('#txtTransferColor').addClass('customAlertChange');
            }

            if ($('#txtSubstrate').val() != "" && $('#txtSubstrate').val() != undefined && filter.test($('#txtSubstrate').val())) {
                $('#txtSubstrate').removeClass('customAlertChange');
            }
            else {
                isValid = false;
                $('#txtSubstrate').addClass('customAlertChange');
            }
        }

        if (isValid == true) {
            var ApplicationColourId = $('#hdnApplicationColourId').val();
            SaveApplicationColours(ApplicationColourId);
        }
        else {
            CustomErrorCode("Required");
        }
    }
    else {
        CustomWarning("Create application first!");
    }
}

function SaveApplicationColours(ApplicationColourId) {
    var ApplictionId, ColourWayNo, GarmentColour, InkId, InkColour, ThreadId, ThreadColour, Pantone, PrintOrder, Mesh, Flash, TransferColour, Substrate, ColourNotes, BucketId, Hexvalue, PantoneId;

    ApplicationId = $('#lblapplicationId').text();

    ApplicationColourId = ApplicationColourId;
    ColourWayNo = $('#txtColourWayNo').val();
    GarmentColour = $('#txtGarmentColour').val();
    InkColour = $('#txtInkColor').val();
    ThreadColour = $('#txtThreadColor').val();
    Pantone = $('#txtPantone').val();
    ThreadId = $('#txtThreadId').val();
    InkId = $('#txtInkId').val();
    PrintOrder = $('#txtPrintOrder').val();
    Mesh = $('#txtMesh').val();
    Flash = $('#chkFlash').prop('checked');
    TransferColour = $('#txtTransferColor').val();
    Substrate = $('#txtSubstrate').val();
    ColourNotes = $('#txtColourNotes').val();
    //BucketId = $('#txtBucketId').val();
    Hexvalue = $('#txtHexValue').val();
    PantoneId = $('#hdnPantoneId').val();

    $.ajax({
        url: '/Application/SaveApplicationColours',
        data: { Model: { ApplicationColourId: ApplicationColourId, ColourWayNo: ColourWayNo, GarmentColour: GarmentColour, InkColour: InkColour, ThreadColour: ThreadColour, ThreadId: ThreadId, InkId: InkId, PrintOrder: PrintOrder, Mesh: Mesh, Flash: Flash, TransferColour: TransferColour, Substrate: Substrate, ColourNotes: ColourNotes }, ApplicationId: ApplicationId, PantoneModel: { Pantone: Pantone, Hexvalue: Hexvalue, Id: PantoneId, /*BucketId: BucketId*/ } },
        async: false,
        type: "Post",
        success: function (response) {
            var saveresponse = response.response;
            var exceptionmsg = response.exceptionMessage;
            if (exceptionmsg != "Could not find any recognizable digits.") {
                CustomAlert(response);
                if (saveresponse.Result == "Success") {
                    $("#btnColoursave").removeClass("customAlertChange");
                    ColourFormReset();
                    GetApplicationColoursGrid(ApplicationId);
                    //ApplicationJobGrid();
                }
            }
            CustomWarning("Please provide valid hex value for colour.")
        }
    });
}

function CopyApplicationColours() {
    if ($('#hdnApplicationColourId').val() != "") {

        $('#hdnApplicationColourId').val(0);
        SaveAppColor();
    }
    else {
        CustomWarning("Please select colour first!");
    }
}

function SaveApplicationCustom() {
    var isValid = true;
    var filter = /^[^-\s][a-zA-Z0-9_.\s-]+$/;

    if ($('#lblapplicationId').text() != 0 && $('#lblapplicationId').text() != "" && $('#lblapplicationId').text() != undefined) {
        if ($('#txtCustomFirstName').val() != "" && $('#txtCustomFirstName').val() != undefined && filter.test($('#txtCustomFirstName').val())) {
            $('#txtCustomFirstName').removeClass('customAlertChange');
        }
        else {
            isValid = false;
            $('#txtCustomFirstName').addClass('customAlertChange');
        }

        if ($('#txtCustomLastName').val() != "" && $('#txtCustomLastName').val() != undefined && filter.test($('#txtCustomLastName').val())) {
            $('#txtCustomLastName').removeClass('customAlertChange');
        }
        else {
            isValid = false;
            $('#txtCustomLastName').addClass('customAlertChange');
        }

        //if ($('#txtCustomNicName').val() != "" && $('#txtCustomNicName').val() != undefined && filter.test($('#txtCustomNicName').val())) {
        //    $('#txtCustomNicName').removeClass('customAlertChange');
        //}
        //else {
        //    isValid = false;
        //    $('#txtCustomNicName').addClass('customAlertChange');
        //}

        if (isValid == true) {
            var ApplicationCustomId = $('#hdnApplicationCustom').val();
            SaveApplicationCustomInfo(ApplicationCustomId);
        }
        else {
            CustomErrorCode("Required");
        }
    }
    else {
        CustomWarning("Create application first!");
    }
}

function SaveApplicationCustomInfo(ApplicationCustomId){
    var ApplicationId, CustomInfoId, FirstName, LastName, NickName, JersyNumber, CustomNotes, Garment, Garmentcolour, GarmentSize;

    ApplicationId = $('#lblapplicationId').text();

    CustomInfoId = ApplicationCustomId;
    FirstName = $('#txtCustomFirstName').val();
    LastName = $('#txtCustomLastName').val();
    NickName = $('#txtCustomNicName').val();
    JersyNumber = $('#txtCustomNumber').val();
    CustomNotes = $('#txtCustomNotes').val();
    Garment = $('#txtCustomGarmentColour').val();
    Garmentcolour = $('#txtCustomGarment').val();
    GarmentSize = $('#txtCustomGarmentSize').val();

    $.ajax({
        url: '/Application/SaveCustomInfo',
        data: { Model: { ApplicationId: ApplicationId, CustomInfoId: CustomInfoId, FirstName: FirstName, LastName: LastName, NickName: NickName, JersyNumber: JersyNumber, CustomNotes: CustomNotes, Garment: Garment, Garmentcolour: Garmentcolour, GarmentSize: GarmentSize }, ApplicationId: ApplicationId },
        async: false,
        success: function (response) {
            CustomAlert(response);
            if (response.Result == "Success") {
                CustomFormReset();
                GetApplicationCustomGrid(ApplicationId);
                //ApplicationJobGrid();
            }
        },
        type: "Post",
    });
}

function CopyApplicationCustom() {
    if ($('#hdnApplicationCustom').val() != "") {

        $('#hdnApplicationCustom').val(0);
        SaveApplicationCustom();
    }
    else {
        CustomWarning("Please select custom decoration  first!");
    }
}

function GetApplicationColoursGrid(ApplicationId) {

    var ApplicationType = $('#drdApplicationType').val();
    var data;
    ApplicationId = Number(ApplicationId);
    $.ajax({
        url: '/Application/GetApplicationColoursGrid',
        data: { ApplicationId },
        async: false,
        type: "Post",
        success: function (response) {
            data = response;
        }
    });

    var obj = {
        selectionModel: { type: 'row', fireSelectChange: true },
        numberCell: { show: false },
        resizable: true,
        editable: false,
        wrap: false,
        hwrap: false,
        pageModel: { type: "local", rPP: 10 },
        strNoRows: 'No records found for the selected Criteria',
        width: "99%",
        height: 300,
        columnTemplate: { width: 150, halign: "left" },
        scrollModel: { pace: 'consistent', horizontal: false },
    };

    obj.colModel = [
        { title: "ApplicationColourId", dataIndx: "ApplicationColourId", width: "2%", dataType: "int", hidden: true },
        { title: "CWay No", dataIndx: "ColourWayNo", width: "6%", dataType: "int" },
        { title: "Garment Colour(s)", dataIndx: "GarmentColour", width: "10%", dataType: "string" },
        {
            title: "Ink Colour", dataIndx: "InkColour", width: "10%", dataType: "string",
            hidden: ApplicationType == "Digital Print" ? false : ApplicationType == "Screen Print" ? false : true
        },
        {
            title: "Thread Colour", dataIndx: "ThreadColour", width: "10%", dataType: "string",
            hidden: ApplicationType == "Embroidery" ? false : true
        },
        {
            title: "Transfer Colour", dataIndx: "TransferColour", width: "10%", dataType: "string",
            hidden: ApplicationType == "Digital Print" ? true : ApplicationType == "Screen Print" ? true : ApplicationType == "Embroidery" ? true : false
        },
        { title: "Pantone", dataIndx: "PantoneName", width: "8%", dataType: "string", },
        {
            title: "Colour Chip", dataIndx: "HexvalueColour", width: "6%", dataType: "string",
            render: function (ui) {
                var data = ui.cellData
                return "<center><input type='text' style='width:70%;background-color: #" + data + ";border: 1px solid #" + data + ";'/></center>";
            }
        },
        {
            title: "Ink Id", dataIndx: "InkId", width: "6%", dataType: "int",
            hidden: ApplicationType == "Screen Print" ? false : true
        },
        {
            title: "Thread Id", dataIndx: "ThreadId", width: "6%", dataType: "int",
            hidden: ApplicationType == "Embroidery" ? false : true
        },
        {
            title: "Print Order", dataIndx: "PrintOrder", width: "7%", dataType: "string",
            hidden: ApplicationType == "Screen Print" ? false : true
        },
        {
            title: "Mesh", dataIndx: "Mesh", width: "5%", dataType: "string",
            hidden: ApplicationType == "Screen Print" ? false : true
        },
        {
            title: "Flash", dataIndx: "Flash", width: "5%", dataType: "string",
            hidden: ApplicationType == "Screen Print" ? false : true,
            render: function (ui) {
                var celldata = ui.cellData;
                if (celldata == true) {
                    return "Yes";
                } else {
                    return "No";
                }
            },
        },
        {
            title: "Substrate", dataIndx: "Substrate", width: "6%", dataType: "string",
            hidden: ApplicationType == "Digital Print" ? true : ApplicationType == "Screen Print" ? true : ApplicationType == "Embroidery" ? true : false
        },
        {
            title: "Colour Note", dataIndx: "ColourNotes", dataType: "string",
            width: ApplicationType == "Digital Print" ? "55%" : ApplicationType == "Embroidery" ? "50%" : ApplicationType == "Screen Print" ? "34%" : "50%"
        },
        {
            title: "Delete", dataIndx: "", width: "4%", dataType: "string",
            render: function (ui) {
                var dataindx = ui.rowIndx;
                var hostname = window.location.origin;
                return "<a onclick='DeleteApplicationColours("+ dataindx +")'><img src='" + hostname + "/Content/images/DeleteContact.png' class='internalLnk' style='width:12px'/></a>";
            }
        },

    ];

    obj.rowDblClick = function (evt, ui) {
        var rowIndx = getRowIndx(this);
        if (rowIndx != null) {
            var griddata = this.pdata[rowIndx];
            if (griddata != undefined && griddata != null) {
                $('#hdnApplicationColourId').val(griddata.ApplicationColourId);
                $('#txtColourWayNo').val(griddata.ColourWayNo);
                $('#txtInkColor').val(griddata.InkColour);
                $('#txtThreadColor').val(griddata.ThreadColour);
                $('#txtPantone').val(griddata.PantoneName);
                $('#txtThreadId').val(griddata.ThreadId);
                $('#txtInkId').val(griddata.InkId);
                $('#txtPrintOrder').val(griddata.PrintOrder);
                $('#txtMesh').val(griddata.Mesh);
                if (griddata.Flash == true) {
                    $('#chkFlash').prop('checked', true);
                } else {
                    $('#chkFlash').prop('checked', false);
                }                
                $('#txtTransferColor').val(griddata.TransferColour);
                $('#txtSubstrate').val(griddata.Substrate);
                $('#txtColourNotes').val(griddata.ColourNotes);

                $('#hdnPantoneId').val(griddata.Pantone);
                $('#txtBucketId').val(griddata.Bucket);
                $('#txtHexValue').val(griddata.HexvalueColour);
                if (griddata.HexvalueColour != null) {
                    $('#txtColourChip').css('background-color', '#' + griddata.HexvalueColour);
                    $('#txtColourChip').css('border', '1px solid #' + griddata.HexvalueColour);
                }
                else {
                    $('#txtColourChip').css('background-color', 'white');
                    $('#txtColourChip').css('border', '1px solid white');
                }
 
            }
        }
    }

    obj.dataModel = { data: data };

    pq.grid("#ApplicationColourGrid", obj);
    pq.grid("#ApplicationColourGrid", "refreshDataAndView");
}

function DeleteApplicationColours(rowIndx) {
    bootbox.confirm("Do you want to delete this colour?", function (result) {
        if (result) {
            var rowdata = $('#ApplicationColourGrid').pqGrid("getRowData", { rowIndx: rowIndx });
            var ApplicationColourId = rowdata.ApplicationColourId;

            $.ajax({
                url: "/Application/DeleteApplicationColours",
                data: { Model: { ApplicationColourId: ApplicationColourId }, ApplicationId: $('#lblapplicationId').text() },
                async: false,
                type: "Post",
                success: function (response) {
                    //CustomAlert(response);
                    GetApplicationColoursGrid($('#lblapplicationId').text());
                }
            });
        }
    });
}

function ColourFormReset() {
    $('#hdnApplicationColourId').val('');
    $('#txtColourWayNo').val('');
    $('#txtInkColor').val('');
    $('#txtThreadColor').val('');
    $('#txtPantone').val('');
    $('#txtThreadId').val('');
    $('#txtInkId').val('');
    $('#txtPrintOrder').val('');
    $('#txtMesh').val('');
    $('#chkFlash').prop('checked',false);
    $('#txtTransferColor').val('');
    $('#txtSubstrate').val('');
    $('#txtColourNotes').val('');

    $('#txtBucketId').val('');
    $('#txtHexValue').val('');
    $('#txtColourChip').css('background-color', 'white');
    $('#txtColourChip').css('border', '1px solid white');
}

function CustomFormReset() {

    $('#txtCustomFirstName').val('');
    $('#txtCustomLastName').val('');
    $('#txtCustomNicName').val('');
    $('#txtCustomNumber').val('');
    $('#txtCustomNotes').val('');
}

function ApplicationJobGrid() {

    $.ajax({
        url: '/Application/ApplicationJobsList',
        data: { ApplicationId: $('#lblapplicationId').text() },
        async: false,
        type: 'Post',
        success: function (response) {
            data = response;
        },
    });

    var obj = {
        selectionModel: { type: 'row', fireSelectChange: true },
        numberCell: { show: false },
        resizable: true,
        editable: false,
        wrap: false,
        hwrap: false,
        pageModel: { type: "local", rPP: 10 },
        strNoRows: 'No records found for the selected Criteria',
        width: "99%",
        height: 300,
        columnTemplate: { width: 150, halign: "left" },
        scrollModel: { pace: 'consistent', horizontal: false,vertical: false },
    };

    obj.colModel = [
        {
            title: "Job No", dataIndx: "OpportunityId", width: "7%", dataType: "int",
            render: function (ui) {
                var id = ui.cellData;
                var Stage = ui.rowData.Stage;

                if (Stage == "Quote") {
                    return "<a href='/opportunity/QuoteDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
                }
                else if (Stage == "Order") {
                    return "<a href='/opportunity/OrderDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
                }
                else if (Stage == "Job" || Stage == "Order Confirmed") {
                    return "<a href='/opportunity/JobDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
                }
                else if (Stage == "Stock Decorated") {
                    return "<a href='/opportunity/PackingDetails/" + parseInt(id) + "'><label  class='internalLnk'>" + id + "</label></a>"
                }
                else if (Stage == "Order Shipped" || Stage == "Paid") {
                    return "<a href='/opportunity/ShippingDetails/" + parseInt(id) + "'><label  class='internalLnk'>" + id + "</label></a>"
                }
                else if (Stage == "Complete") {
                    return "<a href='/opportunity/CompleteDetails/" + parseInt(id) + "'><label  class='internalLnk'>" + id + "</label></a>"
                }
                else if (Stage == "Order Invoiced" || Stage == "Order Packed") {
                    return "<a href='/opportunity/InvoicingDetails/" + parseInt(id) + "'><label  class='internalLnk'>" + id + "</label></a>"
                }
                else {
                    return "<a href='/opportunity/OpportunityDetails/" + parseInt(id) + "'><label class='internalLnk'>" + id + "</label></a>"
                }
            }
        },
        {
            title: "Confirmed Date", dataIndx: "ConfirmedDate", width: "10%", dataType: "string",
            render: function (ui) {
                var date = ui.cellData;
                if (date != null && date != undefined) {
                    return ListDateFormat(date);
                }
            },
        },
        { title: "Job Name", dataIndx: "OppName", width: "25%", dataType: "string" },
        { title: "Job Qty", dataIndx: "Quantity", width: "7%", dataType: "int" },
        { title: "Contact First Name", dataIndx: "first_name", width: "20%", dataType: "string" },
        { title: "Contact Last Name", dataIndx: "last_name", width: "32%", dataType: "string" },
    ];

    obj.rowDblClick = function (evt, ui) {
        var rowIndx = getRowIndx(this);
        if (rowIndx != null) {
            var data = this.pdata[rowIndx];
            if (data != undefined && data != null) {
                $('#lblApplicationManager').text(data.JobManagerInitial);
                //$('#tabCustom').css('visibility', 'visible');
            }
        }
    }

    obj.dataModel = { data: data };

    pq.grid("#ApplicationJobGrid", obj);
    pq.grid("#ApplicationJobGrid", "refreshDataAndView");
}

function GetApplicationCustomGrid() {

    var ApplicationId = $('#lblapplicationId').text();

    $.ajax({
        url: '/Application/GetApplicationCustomGrid',
        data: { ApplicationId },
        async: false,
        type: "Post",
        success: function (response) {
            data = response;
        }
    });

    var obj = {
        selectionModel: { type: 'row', fireSelectChange: true },
        numberCell: { show: false },
        resizable: true,
        editable: false,
        wrap: false,
        hwrap: false,
        pageModel: { type: "local", rPP: 10 },
        strNoRows: 'No records found for the selected Criteria',
        width: "99%",
        height: 300,
        columnTemplate: { width: 150, halign: "left" },
        scrollModel: { pace: 'consistent', horizontal: false },
    };

    obj.colModel = [
        { title: "CustomInfoId", dataIndx: "CustomInfoId", width: "2%", dataType: "int", hidden: true },
        { title: "First Name", dataIndx: "FirstName", width: "10%", dataType: "string" },
        { title: "LastName", dataIndx: "LastName", width: "10%", dataType: "string" },
        { title: "NickName", dataIndx: "NickName", width: "10%", dataType: "string" },
        { title: "Garment", dataIndx: "Garment", width: "10%", dataType: "string" },
        { title: "Garment Colour", dataIndx: "Garmentcolour", width: "10%", dataType: "string" },
        { title: "GarmentSize", dataIndx: "GarmentSize", width: "10%", dataType: "string", },
        { title: "Number", dataIndx: "JersyNumber", width: "6%", dataType: "string" },
        { title: "CustomNotes", dataIndx: "CustomNotes", width: "30%", dataType: "string" },
        {
            title: "Delete", dataIndx: "", width: "5%", dataType: "string",
            render: function (ui) {
                var dataindx = ui.rowIndx;
                var hostname = window.location.origin;
                return "<a onclick='DeleteApplicationCustomInfo(" + dataindx + ")'><img src='" + hostname + "/Content/images/DeleteContact.png' class='internalLnk' style='width:12px'/></a>";
            }
        },
    ];

    obj.rowDblClick = function (evt, ui) {
        var rowIndx = getRowIndx(this);
        if (rowIndx != null) {
            var griddata = this.pdata[rowIndx];
            if (griddata != undefined && griddata != null) {
                $('#hdnApplicationCustom').val(griddata.CustomInfoId);
                $('#txtCustomFirstName').val(griddata.FirstName);
                $('#txtCustomLastName').val(griddata.LastName);
                $('#txtCustomNicName').val(griddata.NickName);
                $('#txtCustomNumber').val(griddata.JersyNumber);
                $('#txtCustomNotes').val(griddata.CustomNotes);
            }
        }
    }

    obj.dataModel = { data: data };

    pq.grid("#ApplicationCustomGrid", obj);
    pq.grid("#ApplicationCustomGrid", "refreshDataAndView");
}

function DeleteApplicationCustomInfo(rowIndx) {
    bootbox.confirm("Do you want to delete this custom decoration?", function (result) {
        if (result) {
            var rowdata = $('#ApplicationCustomGrid').pqGrid("getRowData", { rowIndx: rowIndx });
            var CustomInfoId = rowdata.CustomInfoId;

            $.ajax({
                url: "/Application/DeleteApplicationCustomInfo",
                data: { Model: { CustomInfoId: CustomInfoId }, ApplicationId: $('#lblapplicationId').text() },
                async: false,
                type: "Post",
                success: function (response) {
                    //CustomAlert(response);
                    GetApplicationCustomGrid();
                }
            });
        }
    });
}

function ApplicationListGrid(ApplicationType) {
    var data;
    var url = "/Application/GetApplicationGridData";
    var model = { ApplicationType: ApplicationType };

    if (ApplicationType == "Custom") {
        var Searchdata = $('#searchtextbox').val();
        if (Searchdata != "") {
            url = "/Application/GetApplicationCustomData";
            model = { CustomText: Searchdata };
        }
        else {
            CustomWarning("Please put the search criteria in the search box and click on search button");
        }
    }

    $.ajax({
        url: url,
        data: model,
        async: false,
        success: function (response) {
            data = response;
        },
        type: "Post",
    });  

    var colModel = [
        {
            title: "Applic No", dataIndx: "ApplicationId", width: "6%", align: "center", datatype: "int",
            render: function (ui) {
                var id = ui.cellData;
                return "<a href='/Application/ApplicationDetails?Id=" + parseInt(id) + "&PageType=Default'><label class='internalLnk'>" + ("000000"+id).substr(-6) + "</label></a>";
            },
        },
        {
            title: "Date", dataIndx: "DecorationDate", width: "6%", align: "right", datatype: "date",
            render: function (ui) {
                var date = ui.cellData;
                if (date != null && date != undefined) {
                    return ListDateFormat(date);
                }
            },
        },
        {
            title: "Name", dataIndx: "AppName", width: "17%", align: "left", datatype: "string",
        },
        //{
        //    title: "Location", dataIndx: "", width: "10%", align: "left", datatype: "string",
        //},
        {
            title: "Type", dataIndx: "AppType", width: "8%", align: "left", datatype: "string",
        },
        {
            title: "Status", dataIndx: "AppStatus", width: "5%", align: "left", datatype: "string",
        },
        {
            title: "Art Notes", dataIndx: "ArtNotes", width: "26%", align: "left", datatype: "string",
        },
        {
            title: "Production Notes", dataIndx: "ProductionNotes", width: "26%", align: "left", datatype: "string",
        },
        {
            title: "Bill", dataIndx: "Bill", width: "7%", align: "left", datatype: "string",
        },
        //{
        //    title: "Acct Manager", dataIndx: "", width: "10%", align: "left", datatype: "string",
        //},
    ];

    var obj = {
        colModel: colModel,
        dataModel: { data: data },
        selectionModel: { type: 'row' },
        scrollModel: { pace: 'consistent', horizontal: false },
        editable: false,
        wrap: false,
        hwrap: false,
        pageModel: { type: "local", rPP: 50 },
        strNoRows: 'No records found for the selected Criteria',
        width: "96.5%",
        height: 782,
        columnTemplate: { width: 120, halign: "left" },
        numberCell: { width: 0, title: "", minWidth: 0 },
        sortable: true,
        //sortModel: { cancel: false, type: 'local', sorter: [{ dataIndx: 1, dir: ['up', 'down', 'up'] }] }
    };

    obj.selectChange = function (evt, ui) {
        var rowIndx = getRowIndx(this);
        if (rowIndx != null) {
            var data = this.pdata[rowIndx];
            var dataImage = data.AppImage;
            var hostname = window.location.origin;

            if (dataImage != null) {
                $("#imgApplicationThumbnail").attr("src", hostname + "/Content/uploads/Application/" + dataImage);
            }
            else {
                $("#imgApplicationThumbnail").attr("src", hostname + "/Content/uploads/Opportunity/NoImage.png");
            }
        }
    }

    function getRowIndx(grid) {
        var arr1 = grid.getChanges();
        var arr = grid.selection({ type: 'row', method: 'getSelection' });
        if (arr && arr.length > 0) {
            return arr[0].rowIndx;
        }
        else {
            //alert("Select a row.");
            return null;
        }
    }

    pq.grid("#ApplicationListGrid", obj);
    pq.grid("#ApplicationListGrid", "refreshDataAndView");
}

function readImage(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#Appimg')
                .attr('src', e.target.result);
                //.width(444)
                //.height(438);
        };

        reader.readAsDataURL(input.files[0]);
    }
}

function GetOptionInfo(OptionId) {
    if (OptionId != "" && OptionId != undefined && OptionId != null) {
        $.ajax({
            url: '/Application/GetOptionInfo',
            data: { OptionId },
            async: false,
            type: "Post",
            success: function (response) {
                data = response;
                if (data != null) {
                    $('#txtGarmentColour,#txtCustomGarmentColour').val(data.colour);
                    $('#txtCustomGarment').val(data.ItemName);
                    $('#txtCustomGarmentSize').val(data.InitialSizes);
                }
            },
        });
    }
}

function MovetoLINK() {
    var Link = $('#txtApplicationlink').val();
    if (Link != "" && Link != null) {
        window.open(Link, '_blank');
    }
    else {
        window.open('https://24hm-my.sharepoint.com/personal/kenneth_24hourmerchandise_com_au/_layouts/15/onedrive.aspx?id=%2Fpersonal%2Fkenneth%5F24hourmerchandise%5Fcom%5Fau%2FDocuments%2F1Orders', '_blank');
    }
}

function DownloadFiles(FileType) {
    var fileName = "";

    if (FileType == "Source") {
        fileName = $('#SrcImageName').val();
    }
    else if (FileType == "Vector") {
        fileName = $('#VectorFileName').val();
    }
    else if (FileType == "Mock") {
        fileName = $('#MockImageName').val();
    }
    location.href = "/Application/DownloadApplicationFiles?fileName=" + fileName;
}

function onHover() {
    var hostname = window.location.origin;
    if ($('#MockImageName').val() != "" && $('#MockImageName').val() != null && $('#MockImageName').val() != undefined)
    {
        $("#Appimg").attr('src', hostname + "/Content/uploads/Application/" + $('#MockImageName').val());
    }
}

function offHover() {
    var hostname = window.location.origin;
    if ($('#ArtImageName').val() != "" && $('#ArtImageName').val() != null && $('#ArtImageName').val() != undefined)
    {
        $("#Appimg").attr('src', hostname + "/Content/uploads/Application/" + $('#ArtImageName').val());
    }
}