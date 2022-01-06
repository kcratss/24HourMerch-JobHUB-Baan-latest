$(function () {
    setTimeout(function () {
        $("#DecorationCostMasterGrid .pq-toolbar button").click(function () {
            $("#editdecorationpopup").css("display", "block");
            $('#lblAddNew').css("display", "block");
            $('#lblEdit').css("display", "none");
            $('#HiddenDecorationId').val(0);
            $('#txtDecDesc').val('');
            $("#txtQuantity").val('');
            $("#txtCost").val('');
            $("#statusid").val('');
            //if (action == 0) {
            //    $('#editdecorationpopup').css("display", "block");
            //}
            //$("#txtDecDesc").css("border-color", "rgba(204, 204, 204, 1)");
            $("#txtDecDesc").removeClass('customAlertChange');
            //$("#txtQuantity").css("border-color", "rgba(204, 204, 204, 1)");
            $("#txtQuantity").removeClass('customAlertChange');
            //$("#txtCost").css("border-color", "rgba(204, 204, 204, 1)");
            $("#txtCost").removeClass('customAlertChange');
        });
    }, 2000);
});
function CancelPopup() {
    $("#editdecorationpopup").css("display", "none");
    $('#txtDecDesc').val('');
    $("#txtQuantity").val('');
    $("#txtCost").val('');
    $("#statusid").val('');
    //$("#txtDecDesc").css("border-color", "rgba(204, 204, 204, 1)");
    $("#txtDecDesc").removeClass('customAlertChange');
    //$("#txtQuantity").css("border-color", "rgba(204, 204, 204, 1)");
    $("#txtQuantity").removeClass('customAlertChange');
    //$("#txtCost").css("border-color", "rgba(204, 204, 204, 1)");
    $("#txtCost").removeClass('customAlertChange');
}
function ShowDecorationPopup(rowIndx, action) {
//function ShowDecorationPopup(action) {
    if (action == 0) {
        $('#lblAddNew').css("display", "block");
        $('#lblEdit').css("display", "none");
        
    }
    else {
        $('#lblAddNew').css("display", "none");
        $('#lblEdit').css("display", "block");
    }
    var checkflag;
    if (action == 1) {
        var rowdata = $("#DecorationCostMasterGrid").pqGrid("getRowData", { rowIndx: rowIndx });
        $('#HiddenDecorationId').val(rowdata.DecCostId);
        $('#txtDecDesc').val(rowdata.Dec_Desc);
        $("#txtQuantity").val(rowdata.Quantity);
        $("#txtCost").val(rowdata.Cost);
        $("#statusid").val(rowdata.Status);
        //$("#txtDecDesc").css("border-color", "rgba(204, 204, 204, 1)");
        $("#txtDecDesc").removeClass('customAlertChange');
        //$("#txtQuantity").css("border-color", "rgba(204, 204, 204, 1)");
        $("#txtQuantity").removeClass('customAlertChange');
        //$("#txtCost").css("border-color", "rgba(204, 204, 204, 1)");
        $("#txtCost").removeClass('customAlertChange');
        if (($('#txtDecDesc').val() == "" && $('#txtDecDesc').val() == 0) || ($('#txtQuantity').val() == "" && $('#txtQuantity').val() == 0) || ($('#txtCost').val() == "" && $('#txtCost').val() == 0)) {

            if ($("#txtDecDesc").val() == "") {
                //txtRequiredByDate = $("#requiredbydate").val();
                //$("#txtDecDesc").css("border-color", "red");
                $("#txtDecDesc").addClass('customAlertChange');
                checkflag = false;
            } else {

                //$("#txtDecDesc").css("border-color", "rgba(204, 204, 204, 1)");
                $("#txtDecDesc").removeClass('customAlertChange');

            }
            if ($("#txtQuantity").val() == "") {
                //txtRequiredByDate = $("#requiredbydate").val();
                //$("#txtQuantity").css("border-color", "red");
                $("#txtQuantity").addClass('customAlertChange');
                checkflag = false;
            } else {

                //$("#txtQuantity").css("border-color", "rgba(204, 204, 204, 1)");
                $("#txtQuantity").removeClass('customAlertChange');

            }
            if ($("#txtCost").val() == "") {
                //txtRequiredByDate = $("#requiredbydate").val();
                //$("#txtCost").css("border-color", "red");
                $("#txtCost").addClass('customAlertChange');
                checkflag = false;
            } else {

                //$("#txtCost").css("border-color", "rgba(204, 204, 204, 1)");
                $("#txtCost").removeClass('customAlertChange');

            }

        }
        $('#editdecorationpopup').css("display", "block");
    }
    else {
        $('#txtDecDesc').val('');
        $("#txtQuantity").val('');
        $("#txtCost").val('');
        if (action == 0) {
            $('#editdecorationpopup').css("display", "block");
        }
        //$("#txtDecDesc").css("border-color", "rgba(204, 204, 204, 1)");
        $("#txtDecDesc").removeClass('customAlertChange');
        //$("#txtQuantity").css("border-color", "rgba(204, 204, 204, 1)");
        $("#txtQuantity").removeClass('customAlertChange');
        //$("#txtCost").css("border-color", "rgba(204, 204, 204, 1)");
        $("#txtCost").removeClass('customAlertChange');
    }
    // $('#PaymentModal').css("display", "none");
   
}

function confirmdelete(rowIndex) {
    bootbox.confirm("Do you want to InActive Decoration?", function (result) {
        if (result) {
            var rowdata = $("#DecorationCostMasterGrid").pqGrid("getRowData", { rowIndx: rowIndex });
            var id = rowdata.DecCostId;
            $('#HiddenDecorationId').val(id);
            var Dec_Desc = rowdata.Dec_Desc;
            var Quantity = rowdata.Quantity;
            var Cost = rowdata.Cost;
            var Status = "InActive";
            var model = { DecCostId: id, Dec_Desc: Dec_Desc, Quantity: Quantity, Cost: Cost, Status: Status };

            $.ajax({
                url: '/DecorationCostMaster/DeleteDecoration',
                async: false,
                data: { model },
                success: function (response) {
                    //var Res = { Result: "Success", Message: "Data Deleted Successfully" };
                    //CustomAlert(Res);
                    GetDecorationCostList("All");
                },

                type: 'Post'
            });
        }

    });
    
}


function GetDecorationCostList() {
    var Status = $('#StatusDropDownId').val();
    var Type = $('#TypeDropDownId').val();

    //if (Status == "All") {
    //    var url = '/DecorationCostMaster/GetDecorationCostList';
    //}

    var url = '/DecorationCostMaster/GetDecorationList';
            
        
    
    $.ajax({
        url: url,
        data: { Status: Status, Type: Type },
        async: false,
        
        success: function (response) {
            data = response;
        },

        type: 'Post'
    });

    var obj = {
        selectionModel: { type: 'row' },
        scrollModel: { pace: 'consistent', horizontal: false },
        filterModel: { on: true, mode: "AND", header: true },
        wrap: false,
        hwrap: false,
        numberCell:{width:50},
    };
    obj.width = "94%";
    obj.height = 800;
    obj.columnTemplate = { width: 120 };

    obj.toolbar = {
        items: [
            //{ type: 'button', label: 'Edit', listeners: [{ click: editrow }], icon: 'ui-icon-pencil' },
            //{ type: 'button', label: 'Add New Decoration', listeners: [{ click: ShowDecorationPopup(0) }], icon: 'ui-icon-plus test' },
            { type: 'button', label: 'Add New Decoration', icon: 'ui-icon-plus test' },
            //{ type: 'button', label: 'Delete', listeners: [{ click: deleteRow }], icon: 'ui-icon-minus', align: 'right' }
        ]
    };
    obj.colModel = [
    {
            title: "", width: "2%", editable: false, sortable: false,
            render: function (ui) {
                var dataindx = ui.rowIndx;
                var hostname = window.location.origin;
                return "<a onclick='confirmdelete(" + dataindx + ")')><img src='" + hostname + "/Content/images/delete2.png' style='width:16px'/></a>";
            }
        },
        {
            title: "Decoration Description", dataIndx: "Dec_Desc", width: "18%", dataType: "string", editable: false,
            render: function (ui) {
                var dataIndx = ui.rowIndx;
                var celldata = ui.cellData;
                //var hostname = window.location.origin;
                return "<a onclick='ShowDecorationPopup(" + dataIndx + ",1)'><label class='internalLnk'>"+celldata+"</label></a>";
            }
        },
        {
            title: "Quantity", dataIndx: "Quantity", width: "7%", align: "center", dataType: "string", editable: false
        },
        {
            title: "Cost", dataIndx: "Cost", width: "6%", align: "right", dataType: "float", format: "$#,###.00", editable: false
        },
        {
            title: "Status", dataIndx: "Status", width: "8%", align: "center", dataType: "string", editable: false,
            render: function (ui) {
                var celldata = ui.cellData;
                if (celldata == "Active") {
                   // return "<button class='button' ><label class='lblstatusbutton'>" + celldata + "</label></button>";

                    return "<label class='lblstatusbutton'>" + celldata + "</label>";{
                        
                        //can also return attr (for attributes), cls (for css classes) and text (for plain or html string) properties.
                        //style: 'font-size:15px;color:green'
                    };
                }
                else {
                   // return "<button class='button' style='background-color: darkorange;width: 67px;padding: 2px;'><label class='lblstatusbutton'>" + celldata + "</label></button>";
                    return "<label class='lblstatusbutton' style='background-color: darkorange;width: 67px;padding: 2px;'>" + celldata + "</label>";
                    {
                        //can also return attr (for attributes), cls (for css classes) and text (for plain or html string) properties.
                        //style: 'font-size:15px;;color:orange'
                    };
                }
            }   
        },
        {
            title: "Created By", dataIndx: "CreatedBy", width: "17%", dataType: "string", editable: false
        },
        {
            title: "Created On", dataIndx: "CreatedOn", width: "12%", dataType: "date", editable: false,
            render: function (ui) {
                var date = ui.cellData;
                if (date != null && date != undefined) {
                    //var nowDateopp = new Date(parseInt(date.substr(6)));
                    //return ('00' + nowDateopp.getDate()).substr(-2) + '/' + ('00' + (nowDateopp.getMonth() + 1)).substr(-2) + '/' + nowDateopp.getFullYear();
                    // baans change 22nd November
                    return DateFormat(date);
                    // baans end 22nd November

                }
            }
        },
        {
            title: "Updated By", dataIndx: "UpdatedBy", width: "17%", dataType: "string", editable: false,
        },
        {
            title: "Updated On", dataIndx: "UpdatedOn", width: "12%", dataType: "date", editable: false,
            render: function (ui) {
                var date = ui.cellData;
                if (date != null && date != undefined) {
                    //var nowDateopp = new Date(parseInt(date.substr(6)));
                    //return ('00' + nowDateopp.getDate()).substr(-2) + '/' + ('00' + (nowDateopp.getMonth() + 1)).substr(-2) + '/' + nowDateopp.getFullYear();
                    // baans change 22nd November
                    return DateFormat(date);
                   // baans end 22nd November
                }
            }
        },
    ];
    obj.dataModel = { data: data };

    pq.grid("#DecorationCostMasterGrid", obj);
    pq.grid("#DecorationCostMasterGrid", "refreshDataAndView");
};

function SaveDecoration() {
    
    

    var checkflag = true;
    //$('#requireslabel').css("display", "block");

    var id, Dec_Desc, Quantity, Cost,Status;
    if ($('#HiddenDecorationId').val() != "") {
        id = $('#HiddenDecorationId').val();
    }
    else {
        id = 0;
    }
    Dec_Desc = $('#txtDecDesc').val();
    Quantity = $('#txtQuantity').val();
    Cost = $('#txtCost').val();
    Status = $('#statusid').val();

    //if (id == "" && Dec_Desc == "" && Quantity == "" && Cost == "") {
    //    $("#billVal").css("display", "block");
    //}
    
    if ($("#txtDecDesc").val() == "") {
        //txtRequiredByDate = $("#requiredbydate").val();
        //$("#txtDecDesc").css("border-color", "red");
        $("#txtDecDesc").addClass('customAlertChange');
        checkflag = false;
    } else {
        
        //$("#txtDecDesc").css("border-color", "rgba(204, 204, 204, 1)");
        $("#txtDecDesc").removeClass('customAlertChange');
        
    }

    if ($("#txtQuantity").val() == "") {
        //txtRequiredByDate = $("#requiredbydate").val();
        //$("#txtQuantity").css("border-color", "red");
        $("#txtQuantity").addClass('customAlertChange');
        checkflag = false;
    } else {

        //$("#txtQuantity").css("border-color", "rgba(204, 204, 204, 1)");
        $("#txtQuantity").removeClass('customAlertChange');    

    }

    if ($("#txtCost").val() == "") {
        //txtRequiredByDate = $("#requiredbydate").val();
        //$("#txtCost").css("border-color", "red");
        $("#txtCost").addClass('customAlertChange');
        checkflag = false;
    } else {

        //$("#txtCost").css("border-color", "rgba(204, 204, 204, 1)");
        $("#txtCost").removeClass('customAlertChange');

    }
    if (checkflag == true) {
        var model = { DecCostId: id, Dec_Desc: Dec_Desc, Quantity: Quantity, Cost: Cost, Status: Status };


        $.ajax({
            url: '/DecorationCostMaster/SaveDecoration',
            async: false,
            data: { model },
            success: function (response) {
                if (response.Result == "Success") {
                    var Res = { Result: "Success", Message: response.Message };
                    CustomAlert(Res);
                    GetDecorationCostList("All");
                    $("#editdecorationpopup").css("display", "none");
                }
                else {
                    CustomError("Cannot insert duplicate data, please insert unique values!!!");
                }
            },
           

            type: 'Post'
        });
    }
    else {
        CustomWarning("Fill All Required Fields")
    }

}


//Common Data View 


function GetCommonDataList() {

    $.ajax({
        url: '/DecorationCostMaster/GetCommonDataList',
        //data: { Status: Status },
        async: false,

        success: function (response) {
            data = response;
        },

        type: 'Post'
    });

    var obj = {
        selectionModel: { type: 'row' },
        scrollModel: { pace: 'consistent', horizontal: false },
        filterModel: { on: true, mode: "AND", header: true },
        wrap: false,
        hwrap: false
    };
    obj.width = "94%";
    obj.height = 700;
    obj.columnTemplate = { width: 120 };

    obj.colModel = [

        {
            title: "Field Name", dataIndx: "FieldName", width: "15%", dataType: "string", align: "left", editable: false

        },
        {
            title: "Field Value", dataIndx: "FieldValue", width: "15%", align: "left", dataType: "string", editable: false
        },
        {
            title: "Field Description", dataIndx: "FieldDescription", width: "20%", align: "left", dataType: "string", editable: false
        },
        {
            title: "Sort Order", dataIndx: "SortOrder", width: "7%", align: "center", dataType: "int", editable: false

        },
        {
            title: "Created By", dataIndx: "CreatedBy", width: "14%", dataType: "string", editable: false
        },
        {
            title: "Created On", dataIndx: "CreatedOn", width: "8%", dataType: "date", editable: false,
            render: function (ui) {
                var date = ui.cellData;
                if (date != null && date != undefined) {
                    //var nowDateopp = new Date(parseInt(date.substr(6)));
                    //return ('00' + nowDateopp.getDate()).substr(-2) + '/' + ('00' + (nowDateopp.getMonth() + 1)).substr(-2) + '/' + nowDateopp.getFullYear();
                    // baans change 22nd November
                    return DateFormat(date);
                   // baans end 22nd November
                }
            }
        },
        {
            title: "Updated By", dataIndx: "UpdatedBy", width: "14%", dataType: "string", editable: false
        },
        {
            title: "Updated On", dataIndx: "UpdatedOn", width: "8%", dataType: "date", editable: false,
            render: function (ui) {
                var date = ui.cellData;
                if (date != null && date != undefined) {
                    //var nowDateopp = new Date(parseInt(date.substr(6)));
                    //return ('00' + nowDateopp.getDate()).substr(-2) + '/' + ('00' + (nowDateopp.getMonth() + 1)).substr(-2) + '/' + nowDateopp.getFullYear();
                    // baans change 22nd November
                    return DateFormat(date);
                   // baans end 22nd November
                }
            }
        },
    ];
    obj.dataModel = { data: data };

    pq.grid("#CommonDataMasterGrid", obj);
    pq.grid("#CommonDataMasterGrid", "refreshDataAndView");
};

//3 Oct 2018(N)
$(function () {
    $('#TypeDropDownId').change(function () {
        //alert($('option:selected', this).text());
        GetDecorationCostList();
    })
    $('#StatusDropDownId').change(function () {
        GetDecorationCostList();
    })
})

function ExportToExcel() {
    location.href = '/DecorationCostMaster/ExportToExcel';
}
//3 Oct 2018(N)