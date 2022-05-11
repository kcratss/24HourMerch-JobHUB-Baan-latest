function deleteRow(rowIndx) {
    bootbox.confirm("Do you want to change the status?", function (result) {
        if (result) {
            var rowdata = $('#MastersGrids').pqGrid("getRowData", { rowIndx: rowIndx });
            var id = rowdata.id;
            var status = "InActive";
            var name;
            var table = $('#ddlMasterList').val();
            if (table == "tbldepartment") {
                name = rowdata.department;
            }
            else {
                name = rowdata.name;
            }

            $.ajax({
                url: '/CommonMasters/UpdateData',
                data: { id: id, name: name, status: status, table: table },
                async: false,

                success: function (response) {
                    var Res = { Result: "Success", Message: response.Message };
                    CustomAlert(Res);
                },
                type: "Post"
            });
            GetMasterLists();
        }
    })
}

function addRow() {

    $('#commonhiddenid').val('0');
    $("#txtname").val('');
    $("#ddlstatus").val('');
    $('#lbladd').css("display", "block");
    document.getElementById("editModal").style.display = "block";
}

function ShowPopup(rowIndx) {
    $('#lbledit').css("display", "block");
    var rowdata = $("#MastersGrids").pqGrid("getRowData", { rowIndx: rowIndx });

    $('#commonhiddenid').val(rowdata.id);

    var tblname = $('#ddlMasterList').val();

    if (tblname == "tbldepartment") {
        $('#txtname').val(rowdata.department);
        //$('#txtdepartmentname').css("display", "block");
    }
    else {
        $('#txtname').val(rowdata.name);
        //$('#txtbrandname').css("display", "block");
    }
    $('#ddlstatus').val(rowdata.Status);
    $('#editModal').css("display", "block");
}

function InsertChanges() {
    var id, name, status, table;
    var flag = true;

    if ($('#txtname').val() == "") {
        //$('#txtname').css("border-color", "red");
        $('#txtname').addClass('customAlertChange');
        flag = false;
    }
    else {
        name = $('#txtname').val();
        //$('#txtname').css("border-color", "rgba(204, 204, 204, 1)");
        $('#txtname').removeClass('customAlertChange');
    }

    if ($('#ddlstatus').val() == "") {
        //$('#ddlstatus').css("border-color", "red");
        $('#ddlstatus').addClass('customAlertChange');
        flag = false;
    }
    else {
        status = $('#ddlstatus').val();
        //$('#ddlstatus').css("border-color", "rgba(204, 204, 204, 1)");
        $('#ddlstatus').removeClass('customAlertChange');
    }
    // name = $('#txtname').val();
    var id = $('#commonhiddenid').val();
    //var status = $('#ddlstatus').val();
    var table = $('#ddlMasterList').val();

    //var data = { id: id, name: name, status: status, table: table };
    if (flag == true) {
        $.ajax({
            url: '/CommonMasters/UpdateData',
            data: { id: id, name: name, status: status, table: table },
            async: false,

            success: function (response) {
                if (response.Result == "Success") {
                    var Res = { Result: "Success", Message: response.Message };
                    CustomAlert(Res);
                }
                else {
                    var Res = { Result: "Warning", Message: response.Message };
                    CustomAlert(Res);
                }

                $("#editModal").css("display", "none");
                $('#lbladd').css("display", "none");
                $('#lbledit').css("display", "none");
                GetMasterLists();
            },
            type: 'Post'
        });
    }
}

function hidepopupADD() {
    $('#lbladd').css("display", "none");
    $('#lbledit').css("display", "none");
    $('#txtdepartmentname').val('');
    $('#txtbrandname').val('');
    $('#txtdepartmentname').css("display", "none");
    $('#txtbrandname').css("display", "none");
    //$('#txtname').css("border-color", "rgba(204, 204, 204, 1)");
    $('#txtname').removeClass('customAlertChange');
    document.getElementById("editModal").style.display = "none";
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

function GetMasterLists() {
    var MasterList = $("#ddlMasterList").val();
    var StatusList = $('#ddlStatusList').val();
    if (MasterList == "BlankValue") {

    }
    else {
        var flag = false;
        $.ajax({
            url: '/CommonMasters/ViewMaster',
            data: { ddlvalue: MasterList, ddlstatusvalue: StatusList },
            async: false,
            success: function (response) {
                data = response;
            },

            type: 'get',

        });
        var obj = {
            selectionModel: { type: 'row' },
            scrollModel: { pace: 'consistent', horizontal: false },
            hwrap: false,
            wrap: false,
            resizable: false,
            //filterModel: { on: true, mode: "AND", header: true },
        };
        obj.width = "95%";
        obj.height = 788;
        obj.columnTemplate = { width: 120 };

        obj.toolbar = {
            items: [
                { type: 'button', label: 'Add New', listeners: [{ click: addRow }], hidden: MasterList == "tbldepartment" ? false : true, align: 'right' },

            ]
        };

        obj.colModel = [
            { title: "Id", dataIndx: "id", hidden: true, width: 100, dataType: "int" },

            {
                title: "", dataIndx: "", width: "2%", dataType: "int",
                render: function (ui) {
                    var dataindx = ui.rowIndx;
                    var hostname = window.location.origin;
                    return "<a onclick='deleteRow(" + dataindx + ")'><img src='" + hostname + "/Content/images/delete2.png' style='width:18px'/></a>";
                }
            },

            {
                title: "Brand Name", dataIndx: "name", width: "18%", editable: false, dataType: "string", hidden: MasterList == "tblband" ? false : true,
                render: function (ui) {
                    var dataIndx = ui.rowIndx;
                    var celldata = ui.cellData;
                    return "<a onclick='ShowPopup(" + dataIndx + ")'><label class='internalLnk'>" + celldata + "</label></a>"
                },
            },
                  {
                      title: "Department", dataIndx: "department", width: "18%", editable: false, dataType: "string", hidden: MasterList == "tbldepartment" ? false : true,
                      render: function (ui) {
                          var dataIndx = ui.rowIndx;
                          var celldata = ui.cellData;
                          return "<a onclick='ShowPopup(" + dataIndx + ")'><label class='internalLnk'>" + celldata + "</label></a>"
                      },
                  },
                  {
                      title: "Item Name", dataIndx: "name", width: "18%", editable: false, dataType: "string", hidden: MasterList == "tblitem" ? false : true,
                      render: function (ui) {
                          var dataIndx = ui.rowIndx;
                          var celldata = ui.cellData;
                          return "<a onclick='ShowPopup(" + dataIndx + ")'><label class='internalLnk'>" + celldata + "</label></a>"
                      },
                  },
                  {
                      title: "Status", dataIndx: "Status", width: "12%", editable: false, dataType: "string", align: "center", /*hidden: MasterList == "tblband" ? false : true,*/
                      render: function (ui) {
                          var data = ui.cellData;
                          if (data == "Active") {
                              return "<label class='lblstyle' style='background:limegreen;color:white !important;padding: 5px 12px 5px 12px;'>" + data + "</label>"
                          }
                          else {
                              return "<label class='lblstyle' style='background:orange;color:white !important;padding: 5px 6px 5px 6px;'>" + data + "</label>"
                          }
                      }
                  },
                  {
                      title: "Created By", dataIndx: "CreatedBy", width: "19%", editable: false, dataType: "string", /*hidden: MasterList == "tblband" ? false : true,*/
                  },
                  {
                      title: "Created On", dataIndx: "CreatedOn", width: "15%", editable: false, dataType: "string", /*hidden: MasterList == "tblband" ? false : true,*/
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
                      title: "Updated By", dataIndx: "UpdatedBy", width: "19%", editable: false, dataType: "string", /*hidden: MasterList == "tblband" ? false : true,*/
                  },
                  {
                      title: "Updated On", dataIndx: "UpdatedOn", width: "15%", editable: false, dataType: "string", /*hidden: MasterList == "tblband" ? false : true,*/
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

        //for (var i = 0; i < data.length; i++) {
        //    var i = data.findIndex(x=>x.Status == "Active");
        //    data[i].pq_rowattr = { style: "background:#cccccc;" };
        //}

        obj.dataModel = { data: data };
        pq.grid("#MastersGrids", obj);
        pq.grid("#MastersGrids", "refreshDataAndView");

    }
}

$(function () {
    $('#ddlMasterList').change(function () {
        //alert($('option:selected', this).text());
        GetMasterLists();
    })
    $('#ddlStatusList').change(function () {
        GetMasterLists();
    })
})

function FilterStatus () {
    filter = $(".filterCondition").val();
    $.ajax({
        url: '/CommonMasters/GetOptionCodeList',
        data: { status:filter },
        success: function (response) {          
            data = response;
            InitializeGrid(data);
        },
        type: 'get',
    });
}

function GetOptionCodeList(filter) {
    $.ajax({
        url: '/CommonMasters/GetOptionCodeList',
        data: { filter },
        success: function (response) {           
            data = response;
            InitializeGrid(data);
        },
        type: 'get',
    });
}

function InitializeGrid(data) {
    var obj = {
        selectionModel: { type: 'row' },
        scrollModel: { pace: 'consistent', horizontal: false },
        hwrap: false,
        wrap: false,
        resizable: false,
    };
    obj.width = "95%";
    obj.height = 788;
    obj.columnTemplate = { width: 120 };

    obj.toolbar = {
        items: [
            {
                type: 'select',
                cls: "filterCondition",
                options: [{ All: "All", Active: "Active", InActive: "InActive" }],
                listeners: [{ change: FilterStatus }],
            },

            { type: 'button', label: 'Add New', listeners: [{ click: AddOptionCodeData }], /*align: 'right'*/ },
        ]
    };

    obj.colModel = [
        {
            title: "Id", dataIndx: "id", hidden: true, width: 100, dataType: "int"
        },

        {
            title: "Code", dataIndx: "Code", width: "15%", editable: false, dataType: "string",
            render: function (ui) {
                var dataIndx = ui.rowIndx;
                var celldata = ui.cellData;
                return "<a onclick='EditOptionCodeData(" + dataIndx + ")'><label class='internalLnk'>" + celldata + "</label></a>"
            },
        },
        {
            title: "Item Id", dataIndx: "itemId", hidden: true, width: 100, dataType: "int"
        },
        {
            title: "Item Name", dataIndx: "ItemName", width: "15%", editable: false, dataType: "string",
        },
        { title: "Brand Id", dataIndx: "BrandId", hidden: true, width: 100, dataType: "int" },
        {
            title: "Brand Name", dataIndx: "BrandName", width: "15%", editable: false, dataType: "int",
        },
        {
            title: "Link", dataIndx: "Link", width: "35%", editable: false, dataType: "string",
        },

        {
            title: "Cost", dataIndx: "cost", width: "10%", editable: false, dataType: "string", align: "center",
        },
        {
            title: "Status", dataIndx: "Status", width: "10%", editable: false, dataType: "string", align: "center",
        },

    ];
    obj.dataModel = { data: data };
    pq.grid("#OptionCodeGrids", obj);
    pq.grid("#OptionCodeGrids", "refreshDataAndView");

}
function AddOptionCodeData() {
    $('#id').val('0');
    $("#txtCode").val('');
    $("#ddlItemName").val('');
    $("#ddlBrandName").val('');
    $("#txtLink").val('');
    $("#txtCost").val('');
    $("#ddlStatus").val('Active');
    $('#lbladd').css("display", "block");
    document.getElementById("AddEditModal").style.display = "block";
}
function EditOptionCodeData(rowIndx) {
    $('#lbledit').css("display", "block");
    var rowdata = $("#OptionCodeGrids").pqGrid("getRowData", { rowIndx: rowIndx });
    $('#id').val(rowdata.id);
    $("#txtCode").val(rowdata.Code);
    $("#ddlItemName").val(rowdata.itemId);
    $("#ddlBrandName").val(rowdata.BrandId);
    $("#txtLink").val(rowdata.Link);
    $("#txtCost").val(rowdata.cost);
    $("#ddlStatus").val(rowdata.Status);
    $('#AddEditModal').css("display", "block");
}
//function DeleteOptionCodeData(rowIndx) { //Commented on 28Aug2020 no need 
//    bootbox.confirm("Do you want to change the status?", function (result) {
//        if (result) {
//            var rowdata = $('#OptionCodeGrids').pqGrid("getRowData", { rowIndx: rowIndx });
//            var id = rowdata.id;
//            var Code = rowdata.Code;
//            var itemId = rowdata.itemId;
//            var BrandId = rowdata.BrandId;
//            var Link = rowdata.Link;
//            var Cost = rowdata.cost;
//            var Status = "InActive";
//            $.ajax({
//                url: '/CommonMasters/SaveOptionCodeData',
//                data: { id: id, Code: Code, itemId: itemId, BrandId: BrandId, Link: Link, cost: Cost, Status: Status },
//                async: false,
//                success: function (response) {
//                    var Res = { Result: "Success", Message: response.Message };
//                    CustomAlert(Res);
//                },
//                type: "Post"
//            });
//            GetOptionCodeList();
//        }
//    })
//}
function hideAddEditpopup() {
    $('#lbladd').css("display", "none");
    $('#lbledit').css("display", "none");
    $("#txtCode").val('');
    $("#ddlItemName").val('');
    $("#ddlBrandName").val('');
    $("#txtLink").val('');
    $("#txtCost").val('');
    document.getElementById("AddEditModal").style.display = "none";
}
function SaveChanges() {
    var id = parseInt($("#id").val());
    if (isNaN(id)) {
        id = 0;
    }
    var Code = $("#txtCode").val();
    var itemId = $("#ddlItemName").val();
    var BrandId = $("#ddlBrandName").val();
    var Link = $("#txtLink").val();
    var Cost = $("#txtCost").val();
    var Status = $("#ddlStatus").val();
    if (Code != "" && Cost != "" && itemId != "" && BrandId != "" && Link != "") {
        $.ajax({
            url: '/CommonMasters/SaveOptionCodeData',
            data: { id: id, Code: Code, itemId: itemId, BrandId: BrandId, Link: Link, cost: Cost, Status: Status },
            async: false,
            success: function (response) {
                if (response.Result == "Success") {
                    var Res = { Result: "Success", Message: response.Message };
                    CustomAlert(Res);
                }
                else {
                    var Res = { Result: "Warning", Message: response.Message };
                    CustomAlert(Res);
                }
                $("#AddEditModal").css("display", "none");
                $('#lbladd').css("display", "none");
                $('#lbledit').css("display", "none");
                $("#txtCode,#ddlItemName,#ddlBrandName,#txtLink,#txtCost").css("border-color", '');
                GetOptionCodeList();
            },
            type: 'Post'
        });
    }
    else {
        $("#txtCode,#ddlItemName,#ddlBrandName,#txtLink,#txtCost").css("border-color", "Red");
        var Res = { Result: "Warning", Message: "Please fill all required fields." };
        CustomAlert(Res);
    }
}