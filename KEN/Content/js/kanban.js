var WeekTabs = [];
var MacTabs = [];
var DeptTabs = [];
var unassignHeight;
function UpdateKanBanOpp(GetPriority, GetMachineNo, GetProdDate, GetkanbanId) {
    if (GetkanbanId != "" && GetkanbanId != null && GetkanbanId != undefined) {

        $.ajax({
            type: "GET",
            url: "/KanBan/updateKanBan/",
            async: false,
            data: { MachineNo: GetMachineNo, KanbanId: GetkanbanId, ProductionDate: GetProdDate, Priority: GetPriority, KanbanStatus: "Inactive" },
            success: function (response) {
                if (response.Result == "Success") {
                    GetAllKanbanJobs(DeptTabs, WeekTabs, MacTabs);
                    GetSidebarKanbanOpportunity(DeptTabs);
                    CustomAlert(response);
                }
            },

        });

    }
}

function GetFormattedDate(date) {
    return date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
}

function GetKanbanFormattedDate(date) {
    return date.getDate() + '-' + (date.getMonth() + 1) + '-' + date.getFullYear();
}

function GetAllKanbanJobs(DeptTabs, WeekTabs, MacTabs) {
    var currMonday, currTuesday, currWednesday, currThursday, currFriday, currSaturday;
    var nextMonday, nextTuesday, nextWednesday, nextThursday, nextFriday, nextSaturday;
    var Week = new Array();

    // var nextWeek = new Array();
    var WeekDays = new Array();
    var ActiveTab;

    WeekDays = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

    if (WeekTabs.length > 0) {

        if (WeekTabs.includes("currentWeek")) {
            var curr = new Date();
            currMonday = formatDate(new Date(curr.setDate(curr.getDate() - curr.getDay() + 1)));
            currTuesday = formatDate(new Date(curr.setDate(curr.getDate() - curr.getDay() + 2)));
            currWednesday = formatDate(new Date(curr.setDate(curr.getDate() - curr.getDay() + 3)));
            currThursday = formatDate(new Date(curr.setDate(curr.getDate() - curr.getDay() + 4)));
            currFriday = formatDate(new Date(curr.setDate(curr.getDate() - curr.getDay() + 5)));
            currSaturday = formatDate(new Date(curr.setDate(curr.getDate() - curr.getDay() + 6)));

            Week = [currMonday, currTuesday, currWednesday, currThursday, currFriday, currSaturday];

        }

        if (WeekTabs.includes("NextWeek")) {

            var next = new Date();
            var NextWeekDate = parseInt(next.getDate()) + 7;
            next.setDate(NextWeekDate);

            nextMonday = formatDate(new Date(next.setDate(next.getDate() - next.getDay() + 1)));
            nextTuesday = formatDate(new Date(next.setDate(next.getDate() - next.getDay() + 2)));
            nextWednesday = formatDate(new Date(next.setDate(next.getDate() - next.getDay() + 3)));
            nextThursday = formatDate(new Date(next.setDate(next.getDate() - next.getDay() + 4)));
            nextFriday = formatDate(new Date(next.setDate(next.getDate() - next.getDay() + 5)));
            nextSaturday = formatDate(new Date(next.setDate(next.getDate() - next.getDay() + 6)));

            Week = [nextMonday, nextTuesday, nextWednesday, nextThursday, nextFriday, nextSaturday];

            //GetValidatedDate('10-8-2018');

        }

        if (MacTabs.includes("machine12")) {
            ActiveTab = "machine12";
        }

        if (MacTabs.includes("machine34")) {
            ActiveTab = "machine34";
        }

        // alert(JSON.stringify(model));
    }

    //var postData = { Week: Week, WeekDays: WeekDays, ActiveTabs: ActiveTab, DeptId: DeptTabs[0] };
    var postData = { DeptId: DeptTabs[0] };

    $.ajax({
        type: "GET",
        url: "/KanBan/GetAllKanbanJobs/",
        data: postData,
        success: function (response) {
            var GetAllHolidays = response.res2;
            var data = response.res1;
            var kenOppdata = "";
            var FilterData = [];

            if (ActiveTab == "machine12") {
                i = 1;
                j = 2;
            } else if (ActiveTab == "machine34") {
                i = 3;
                j = 4;
            }

            //Looping For Machines
            for (var mac = i; mac <= j; mac++) {
                kenOppdata += "<div class='row RowColumns'>";

                //Looping For Columns
                for (var cw = 0; cw < Week.length; cw++) {
                    kenOppdata += "<div class='col-lg-2 KanOppdaysColumn'>";
                    var GetNewDate = GetddmmyyyyDate(Week[cw]);
                    var GetComparableDate = GetCompareDate(Week[cw]);
                    var GetValidationDate = GetValidatedDate(GetNewDate);

                    if (cw == 0) {
                        kenOppdata += "<div class='columnHeader'><label class='currMachine'>Mach " + mac + "</label>" + WeekDays[cw] + " <label class='currMonday'>" + GetNewDate + "</label></div>";
                    }
                    else {
                        kenOppdata += "<div class='columnHeader'>" + WeekDays[cw] + " <label class='currMonday'>" + GetNewDate + "</label></div>";
                    }

                    //var FilterHoliday = GetAllHolidays.find(_=>_.GetDayofyear == Week[cw]);
                    //if (FilterHoliday != null && FilterHoliday != undefined) {
                    //    kenOppdata += "<div class='KanColumnOppList connectedSortable NonDraggable' id='mac~" + mac + "_" + Week[cw] + "'>";
                    //} else {
                    //    kenOppdata += "<div class='KanColumnOppList connectedSortable' id='mac~" + mac + "_" + Week[cw] + "'>";
                    //}

                    kenOppdata += "<div class='KanColumnOppList connectedSortable' id='mac~" + mac + "_" + Week[cw] + "'>";

                    for (var prCol = 1; prCol <= 10; prCol++) {

                        var dateObj = new Date(Week[cw]);
                        timestamp = dateObj.getTime();
                        var date = "/Date(" + timestamp + ")/";
                        //Taking Kanban Data according to Looped Data
                        FilterData = data.find(x => x.MachineNo == mac && x.Priority == prCol && x.NewProductionDate == GetComparableDate);

                        if (FilterData != undefined && FilterData != "") {
                            // for (var kan = 0; kan < FilterData.length; kan++) {
                            // baans change 18th October for Confirm date not null checking
                            if (FilterData.ConfirmedDate != null) {
                                var getDate = new Date(parseInt(FilterData.ConfirmedDate.substr(6)));
                                var ConfirmedDate = GetKanbanFormattedDate(getDate);
                                //Making Kanban Jobs slot Assigned or Unassigned
                                kenOppdata += "<div class='row InvisiblePriorityCol assigned' id='" + mac + WeekDays[cw] + "pro_" + prCol + "'>";

                                if (FilterData.KanbanStatus == "Active") {
                                    kenOppdata += "<div class='row KanOppMainRow Active' id='kan_" + FilterData.KanbanId + "' draggable='true'><div class='col-lg-8 KanOppLeftSection'><div class='row'>";
                                }
                                else {
                                    kenOppdata += "<div class='row KanOppMainRow' id='kan_" + FilterData.KanbanId + "' draggable='true'><div class='col-lg-8 KanOppLeftSection'><div class='row'>";
                                }


                                kenOppdata += "<div class='KanOppName'><label class='KanOppName'>" + FilterData.OppName + "</label></div>";
                                kenOppdata += "<div class='KanOppDetails'><label class='KanOppAcctMngr'>" + FilterData.AccountManagerName + "</label>";

                                var confirmationDate = new Date(GetyyyymmddDate(ConfirmedDate));
                                var productionDate = new Date(GetyyyymmddDate(GetValidationDate));

                                if (confirmationDate > productionDate) {

                                    kenOppdata += "<label class='KanOppDate validate'>" + ConfirmedDate + "</label>";
                                }
                                else {
                                    kenOppdata += "<label class='KanOppDate InvalidDate'>" + ConfirmedDate + "</label>";
                                }


                                kenOppdata += "</div></div>";

                                //kenOppdata += "<div class='row' style='margin-top:-3px;'><div class='oppNumber'>";
                                //kenOppdata += "<a href='/Opportunity/OpportunityDetails/" + FilterData.OppId + "'><label class='oppNumber'>" + FilterData.OppId + "</label></a>";
                                //kenOppdata += "</div></div>";

                                //baans change 4Sept starts here [P]
                                kenOppdata += "<div class='row middleCardRow'>";
                                kenOppdata += "<div class='col-lg-3' style='float: left;padding: 0 0 0 0;margin: 0 0 0 -3px;'><div class='oppNumber'>";
                                kenOppdata += "<a href='/Opportunity/JobDetails/" + FilterData.OppId + "'><label class='oppNumber'>" + FilterData.OppId + "</label></a>";
                                kenOppdata += "</div></div>";

                                kenOppdata += "<div class='col-lg-9' style='padding: 0 0 0 0px;'>";

                                kenOppdata += "<button class='decButton OppQuantity' type='button' id='OppQuantity" + FilterData.OppId + "'>" + FilterData.Quantity + "</button>";

                                if (FilterData.FrontPrint == "Y") {
                                    kenOppdata += "<button class='decButton active' type='button' id='FrontDecInfo" + FilterData.OppId + "'></button>";
                                } else {
                                    kenOppdata += "<button class='decButton inactive' type='button' id='FrontDecInfo" + FilterData.OppId + "'></button>";
                                }

                                if (FilterData.BackCount == "Y") {
                                    kenOppdata += "<button class='decButton active' type='button' id='BackDecInfo" + FilterData.OppId + "'></button>";
                                } else {
                                    kenOppdata += "<button class='decButton inactive' type='button' id='BackDecInfo" + FilterData.OppId + "'></button>";
                                }

                                if (FilterData.LeftCount == "Y") {
                                    kenOppdata += "<button class='decButton active' type='button' id='LeftDecInfo" + FilterData.OppId + "'></button>";
                                } else {
                                    kenOppdata += "<button class='decButton inactive' type='button' id='LeftDecInfo" + FilterData.OppId + "'></button>";
                                }

                                if (FilterData.RightCount == "Y") {
                                    kenOppdata += "<button class='decButton active' type='button' id='RightDecInfo" + FilterData.OppId + "'></button>";
                                } else {
                                    kenOppdata += "<button class='decButton inactive' type='button' id='RightDecInfo" + FilterData.OppId + "'></button>";
                                }

                                if (FilterData.ExtraCount == "Y") {
                                    kenOppdata += "<button class='decButton active' type='button' id='OtherDecInfo" + FilterData.OppId + "'></button>";
                                } else {
                                    kenOppdata += "<button class='decButton inactive' type='button' id='OtherDecInfo" + FilterData.OppId + "'></button>";
                                }

                                kenOppdata += "</div></div>";
                                //baans change 4Sept Ends here [P]


                                kenOppdata += "<div class='row'><div class='artRow'>";

                                kenOppdata += "<label class='KanArthead'>Art</label>";

                                //Making Art Labels Active and Inactive
                                if (FilterData.ArtOrderedDate != null) {
                                    kenOppdata += "<label class='KanArtOrderedStatus Active'>Ord</label>";
                                }
                                else {
                                    kenOppdata += "<label class='KanArtOrderedStatus'>Ord</label>";
                                }
                                if (FilterData.ApprovedDate != null) {
                                    kenOppdata += "<label class='KanArtApprovedStatus Active'>App</label>";
                                }
                                else {
                                    kenOppdata += "<label class='KanArtApprovedStatus'>App</label>";
                                }

                                if (FilterData.ArtReadyDate != null) {
                                    kenOppdata += "<label class='KanArtReadyStatus Active'>Go</label>";
                                }
                                else {
                                    kenOppdata += "<label class='KanArtReadyStatus'>Go</label>";
                                }

                                //Making Stock Labels Active and Inactive
                                kenOppdata += "</div><div class='stkRow'>";
                                kenOppdata += "<label class='KanArthead'>Stk</label>";
                                if (FilterData.StockOrderedDate != null) {
                                    kenOppdata += "<label class='KanSktOrderedStatus Active'>Ord</label>";
                                }
                                else {
                                    kenOppdata += "<label class='KanSktOrderedStatus'>Ord</label>";
                                }

                                if (FilterData.ReceivedDate != null) {
                                    kenOppdata += "<label class='KanSktApprovedStatus Active'>Rec</label>";
                                }
                                else {
                                    kenOppdata += "<label class='KanSktApprovedStatus'>Rec</label>";
                                }

                                if (FilterData.Checkeddate != null) {
                                    kenOppdata += "<label class='KanSktReadyStatus Active'>Go</label>";
                                }
                                else {
                                    kenOppdata += "<label class='KanSktReadyStatus'>Go</label>";
                                }

                                kenOppdata += "</div></div></div>";
                                kenOppdata += "<div class='col-lg-4 KanOppRightSection'>";

                                if (FilterData.OppThumbnail != null) {
                                    kenOppdata += "<img src='/Content/uploads/Opportunity/" + FilterData.OppThumbnail + "' class='img-responsive OppImage' />";
                                }
                                else {
                                    kenOppdata += "<img src='/Content/uploads/Opportunity/NoImage.png' class='img-responsive OppImage' />";
                                }
                                //kenOppdata += "<img src='/Content/images/newpic.jpg' class='img-responsive OppImage' />";

                                kenOppdata += "</div></div>";

                                kenOppdata += "</div>";
                                //}
                            }
                            else {
                                kenOppdata += "<div class='row InvisiblePriorityCol' id='" + mac + WeekDays[cw] + "pro_" + prCol + "'>";
                                kenOppdata += "</div>";
                            }
                            // baans end 18th October
                        }
                        else {
                            kenOppdata += "<div class='row InvisiblePriorityCol' id='" + mac + WeekDays[cw] + "pro_" + prCol + "'>";
                            kenOppdata += "</div>";
                        }

                    }
                    kenOppdata += "</div>";
                    kenOppdata += "</div>";
                }
                kenOppdata += "</div>";
            }

            $("#loaddata").html(kenOppdata);

            var DropFlag = true;
            $(".KanOppMainRow").draggable({
                start: function() {
                    DropFlag = false;
                },
                revert: function (obj) {
                    if (!DropFlag) { return true; }
                }
            });
            $(".InvisiblePriorityCol,#kanSidebarOppList_unassign,#kanSidebarOppList_uncomplete").droppable({
                hoverClass: "highlight",
                drop: function (event, ui) {
                    var DraggedId = ui.draggable.attr('id');
                    var DroppedId = $(this)[0].id;
                    DropFlag = true;
                    //alert(document.getElementById(DroppedId).classList.contains('InvisiblePriorityCol'));
                    var OldId = $("#" + DraggedId).parent().attr('id');
                    if (OldId != DroppedId) {
                        if ($(this)[0].id == "kanSidebarOppList_unassign") {
                            bootbox.confirm("Do you want to Unassign this job", function (result) {
                                if (result) {
                                    var GetkanbanId = DraggedId.split("_")[1];
                                    UpdateKanBanOpp(null, null, null, GetkanbanId);
                                    //var MinHeight = parseInt(unassignHeight) + 100;
                                    //$("#kanSidebarOppList_unassign").css('min-height', MinHeight + 'px');
                                }
                                else {
                                    GetSidebarKanbanOpportunity(DeptTabs);
                                    GetAllKanbanJobs(DeptTabs, WeekTabs, MacTabs);
                                }
                            });
                        } else if ($(this)[0].id == "kanSidebarOppList_uncomplete") {
                            ui.draggable.draggable({ revert: true });
                        }
                        else {
                            var ColumnDate = new Date($("#" + DroppedId).parent().attr('id').split('_')[1]);
                            var today = new Date();
                            var GetCurrentDate = new Date(GetFormattedDate(today));
                            if (ColumnDate >= GetCurrentDate) {

                                if (document.getElementById(DroppedId).classList.contains('assigned')) {
                                    GetSidebarKanbanOpportunity(DeptTabs);
                                    GetAllKanbanJobs(DeptTabs, WeekTabs, MacTabs);
                                    bootbox.alert("This slot is already assigned to another job!", function () {
                                    });
                                }
                                else {

                                    var GetColumnId = $("#" + DroppedId).parent().attr('id');
                                    var IsEditable = document.getElementById(GetColumnId).classList.contains('NonDraggable');
                                    if (IsEditable == true) {
                                        ui.draggable.draggable({ revert: true });
                                    } else {
                                        var columnId = $(this).parent().attr('id');
                                        var GetKanData = columnId.split("_");
                                        var GetPriority = DroppedId.split("_")[1];
                                        var GetMachineNo = GetKanData[0].split("~")[1];
                                        var GetProdDate = GetKanData[1];
                                        var GetkanbanId = DraggedId.split("_")[1];

                                        if (document.getElementById(DraggedId).classList.contains('Active')) {
                                            bootbox.confirm("This Job is in Progress...</br>Do you wish to continue?", function (result) {
                                                if (result) {
                                                    UpdateKanBanOpp(GetPriority, GetMachineNo, GetProdDate, GetkanbanId);
                                                }
                                                else {
                                                    GetSidebarKanbanOpportunity(DeptTabs);
                                                    GetAllKanbanJobs(DeptTabs, WeekTabs, MacTabs);
                                                }
                                            });
                                        }
                                        else {
                                            UpdateKanBanOpp(GetPriority, GetMachineNo, GetProdDate, GetkanbanId);
                                        }
                                    }
                                }

                            } else {
                                bootbox.alert("Job cannot be assigned to previous dates!!", function () {
                                    GetSidebarKanbanOpportunity(DeptTabs);
                                    GetAllKanbanJobs(DeptTabs, WeekTabs, MacTabs);
                                });
                            }
                        }
                    } else {
                        ui.draggable.draggable({ revert: true });
                    }

                }
            });
        },
        dataType: "json",
        traditional: true
    });
}

$(function () {
    $("#lblUncomplete").click(function () {
        $("#kanSidebarOppList_uncomplete").slideToggle();
    });

    $("#lblUnassign").click(function () {
        $("#kanSidebarOppList_unassign").slideToggle();
    });

    WeekTabs = ["currentWeek"];
    MacTabs = ["machine12"];
    DeptTabs = ["" + $(".kanbantab.active").attr('id') + ""];
    GetSidebarKanbanOpportunity(DeptTabs);
    GetAllKanbanJobs(DeptTabs, WeekTabs, MacTabs);

    //var MinHeight = parseInt(unassignHeight) + 100;
    //$("#kanSidebarOppList_unassign").css('min-height', MinHeight + 'px');


    var weekFlag = false;
    var macFlag = false;
    var DeptFlag = false;

    $(".kanbantab").click(function () {
        if ($(this).hasClass("active")) {
            DeptFlag = true;
        } else {
            DeptTabs = [];
            $(".kanbantab").removeClass("active");
            $(this).addClass("active");
            DeptTabs.push($(this).attr('id'));
        }

        if (DeptFlag == false) {
            GetSidebarKanbanOpportunity(DeptTabs);
            GetAllKanbanJobs(DeptTabs, WeekTabs, MacTabs);
        }

    });

    $(".kanbanWeekbtns").click(function () {

        if ($(this).hasClass("active")) {
            weekFlag = true;
        } else {
            WeekTabs = [];
            $(".kanbanWeekbtns").removeClass("active");
            $(this).addClass("active");
            WeekTabs.push($(this).attr('id'));
            weekFlag = false;
        }

        if (weekFlag == false) {
            GetAllKanbanJobs(DeptTabs, WeekTabs, MacTabs);
        }

    });

    $(".kanbanMacbtns").click(function () {
        if ($(this).hasClass("active")) {

            macFlag = true;

        } else {
            MacTabs = [];
            $(".kanbanMacbtns").removeClass("active");
            $(this).addClass("active");
            MacTabs.push($(this).attr('id'));
            macFlag = false;
        }

        if (macFlag == false) {
            GetAllKanbanJobs(DeptTabs, WeekTabs, MacTabs);
        }
    });
    // baans change 15th November for user by title
    if ($("#PageName").val() == "kanban") {
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
// baans end 16th November

});
// baans change 16th November

function GetSidebarKanbanOpportunity(DeptTabs) {
    var heightCount = 0;
    $.ajax({
        type: "GET",
        url: "/KanBan/GetSidebarKanbanOpportunity/",
        data: { DeptId: DeptTabs[0] },
        async: false,
        success: function (response) {
            console.log(response);
            var newData = response.Data.param1;
            var uncompltedata = response.Data.param2;
            //console.log(response.Data.param1);
            var sidebarData = "";
            var sidebarUncompletedata = "";
            $("#kanSidebarOppList_unassign").empty();
            $("#kanSidebarOppList_uncomplete").empty();
            if (newData.length > 0) {
                for (var kan = 0; kan < newData.length; kan++) {
                    // baans change 18th October for Confirm date not null checking
                    if (newData[kan].ConfirmedDate != null) {
                        var getDate = new Date(parseInt(newData[kan].ConfirmedDate.substr(6)));
                        var ConfirmedDate = GetKanbanFormattedDate(getDate);
                        sidebarData += "<div class='row KanOppMainRow' id='kan_" + newData[kan].KanbanId + "' draggable='true'><div class='col-lg-8 KanOppLeftSection'><div class='row'>";
                        sidebarData += "<div class='KanOppName'><label class='KanOppName'>" + newData[kan].OppName + "</label></div>";
                        sidebarData += "<div class='KanOppDetails'><label class='KanOppAcctMngr'>" + newData[kan].AccountManagerName + "</label>";
                        sidebarData += "<label class='KanOppDate'>" + ConfirmedDate + "</label>";
                        sidebarData += "</div></div>";

                        //sidebarData += "<div class='row' style='margin-top:-3px;'><div class='oppNumber'>";
                        //sidebarData += "<a href='/Opportunity/OpportunityDetails/" + newData[kan].OppId + "'><label class='oppNumber'>" + newData[kan].OppId + "</label></a>";
                        //sidebarData += "</div></div>";

                        //baans change 4Sept starts here [P]
                        sidebarData += "<div class='row middleCardRow'>";
                        sidebarData += "<div class='col-lg-3' style='float: left;padding: 0 0 0 0;margin: 0 0 0 -3px;'><div class='oppNumber'>";
                        sidebarData += "<a href='/Opportunity/JobDetails/" + newData[kan].OppId + "'><label class='oppNumber'>" + newData[kan].OppId + "</label></a>";
                        sidebarData += "</div></div>";

                        sidebarData += "<div class='col-lg-9' style='padding: 0 0 0 0px;'>";
                        sidebarData += "<button class='decButton OppQuantity' type='button' id='OppQuantity" + newData[kan].OppId + "'>" + newData[kan].Quantity + "</button>";

                        if (newData[kan].FrontPrint == "Y") {
                            sidebarData += "<button class='decButton active' type='button' id='FrontDecInfo" + newData[kan].OppId + "'></button>";
                        } else {
                            sidebarData += "<button class='decButton inactive' type='button' id='FrontDecInfo" + newData[kan].OppId + "'></button>";
                        }

                        if (newData[kan].BackCount == "Y") {
                            sidebarData += "<button class='decButton active' type='button' id='BackDecInfo" + newData[kan].OppId + "'></button>";
                        } else {
                            sidebarData += "<button class='decButton inactive' type='button' id='BackDecInfo" + newData[kan].OppId + "'></button>";
                        }

                        if (newData[kan].LeftCount == "Y") {
                            sidebarData += "<button class='decButton active' type='button' id='LeftDecInfo" + newData[kan].OppId + "'></button>";
                        } else {
                            sidebarData += "<button class='decButton inactive' type='button' id='LeftDecInfo" + newData[kan].OppId + "'></button>";
                        }

                        if (newData[kan].RightCount == "Y") {
                            sidebarData += "<button class='decButton active' type='button' id='RightDecInfo" + newData[kan].OppId + "'></button>";
                        } else {
                            sidebarData += "<button class='decButton inactive' type='button' id='RightDecInfo" + newData[kan].OppId + "'></button>";
                        }

                        if (newData[kan].ExtraCount == "Y") {
                            sidebarData += "<button class='decButton active' type='button' id='OtherDecInfo" + newData[kan].OppId + "'></button>";
                        } else {
                            sidebarData += "<button class='decButton inactive' type='button' id='OtherDecInfo" + newData[kan].OppId + "'></button>";
                        }
                        sidebarData += "</div></div>";
                        //baans change 4Sept Ends here [P]



                        sidebarData += "<div class='row'><div class='artRow'>";

                        sidebarData += "<label class='KanArthead'>Art</label>";
                        //Making Art Labels Active and Inactive
                        if (newData[kan].ArtOrderedDate != null) {
                            sidebarData += "<label class='KanArtOrderedStatus Active'>Ord</label>";
                        }
                        else {
                            sidebarData += "<label class='KanArtOrderedStatus'>Ord</label>";
                        }
                        if (newData[kan].ApprovedDate != null) {
                            sidebarData += "<label class='KanArtApprovedStatus Active'>App</label>";
                        }
                        else {
                            sidebarData += "<label class='KanArtApprovedStatus'>App</label>";
                        }

                        if (newData[kan].ArtReadyDate != null) {
                            sidebarData += "<label class='KanArtReadyStatus Active'>Go</label>";
                        }
                        else {
                            sidebarData += "<label class='KanArtReadyStatus'>Go</label>";
                        }

                        //Making Stock Labels Active and Inactive
                        sidebarData += "</div><div class='stkRow'>";

                        sidebarData += "<label class='KanArthead'>Stk</label>";
                        if (newData[kan].StockOrderedDate != null) {
                            sidebarData += "<label class='KanSktOrderedStatus Active'>Ord</label>";
                        }
                        else {
                            sidebarData += "<label class='KanSktOrderedStatus'>Ord</label>";
                        }

                        if (newData[kan].ReceivedDate != null) {
                            sidebarData += "<label class='KanSktApprovedStatus Active'>Rec</label>";
                        }
                        else {
                            sidebarData += "<label class='KanSktApprovedStatus'>Rec</label>";
                        }

                        if (newData[kan].Checkeddate != null) {
                            sidebarData += "<label class='KanSktReadyStatus Active'>Go</label>";
                        }
                        else {
                            sidebarData += "<label class='KanSktReadyStatus'>Go</label>";
                        }

                        sidebarData += "</div></div></div>";
                        sidebarData += "<div class='col-lg-4 KanOppRightSection'>";

                        if (newData[kan].OppThumbnail != null) {
                            sidebarData += "<img src='/Content/uploads/Opportunity/" + newData[kan].OppThumbnail + "' class='img-responsive OppImage' />";
                        }
                        else {
                            sidebarData += "<img src='/Content/uploads/Opportunity/NoImage.png' class='img-responsive OppImage' />";
                        }

                        sidebarData += "</div></div>";
                    }
                    // baans end 18th October
                }
                $("#kanSidebarOppList_unassign").html(sidebarData);

                $("#kanSidebarOppList_unassign").css('min-height', 'auto').css('height', 'auto');

                unassignHeight = document.getElementById('kanSidebarOppList_unassign').offsetHeight;

                var MinHeight = parseInt(unassignHeight) + 80;
                $("#kanSidebarOppList_unassign").css('min-height', MinHeight + 'px');

                //if (heightCount == 0)
                //{
                //    var MinHeight = parseInt(unassignHeight) + 100;
                //    $("#kanSidebarOppList_unassign").css('min-height', MinHeight + 'px');
                //    heightCount++;
                //}
            } else {
                $("#kanSidebarOppList_unassign").css('min-height', 'auto').css('height', 'auto');

                unassignHeight = document.getElementById('kanSidebarOppList_unassign').offsetHeight;

                var MinHeight = parseInt(unassignHeight) + 80;
                $("#kanSidebarOppList_unassign").css('min-height', MinHeight + 'px');
            }

            if (uncompltedata.length > 0) {
                for (var kan = 0; kan < uncompltedata.length; kan++) {
                    // baans change 18th October for ConfirmDate Not Null Checking
                    if (uncompltedata[kan].ConfirmedDate != null) {
                        var getDate = new Date(parseInt(uncompltedata[kan].ConfirmedDate.substr(6)));
                        var ConfirmedDate = GetKanbanFormattedDate(getDate);
                        sidebarUncompletedata += "<div class='row KanOppMainRow' id='kan_" + uncompltedata[kan].KanbanId + "' draggable='true'><div class='col-lg-8 KanOppLeftSection'><div class='row'>";
                        sidebarUncompletedata += "<div class='KanOppName'><label class='KanOppName'>" + uncompltedata[kan].OppName + "</label></div>";
                        sidebarUncompletedata += "<div class='KanOppDetails'><label class='KanOppAcctMngr'>" + uncompltedata[kan].AccountManagerName + "</label>";
                        sidebarUncompletedata += "<label class='KanOppDate'>" + ConfirmedDate + "</label>";
                        sidebarUncompletedata += "</div></div>";

                        //baans change 4Sept starts here [P]
                        sidebarUncompletedata += "<div class='row middleCardRow'>";
                        sidebarUncompletedata += "<div class='col-lg-3' style='float: left;padding: 0 0 0 0;margin: 0 0 0 -3px;'><div class='oppNumber'>";
                        sidebarUncompletedata += "<a href='/Opportunity/JobDetails/" + uncompltedata[kan].OppId + "'><label class='oppNumber'>" + uncompltedata[kan].OppId + "</label></a>";
                        sidebarUncompletedata += "</div></div>";

                        sidebarUncompletedata += "<div class='col-lg-9' style='padding: 0 0 0 0px;'>";
                        sidebarUncompletedata += "<button class='decButton OppQuantity' type='button' id='OppQuantity" + uncompltedata[kan].OppId + "'>" + uncompltedata[kan].Quantity + "</button>";

                        if (uncompltedata[kan].FrontPrint == "Y") {
                            sidebarUncompletedata += "<button class='decButton active' type='button' id='FrontDecInfo" + uncompltedata[kan].OppId + "'></button>";
                        } else {
                            sidebarUncompletedata += "<button class='decButton inactive' type='button' id='FrontDecInfo" + uncompltedata[kan].OppId + "'></button>";
                        }

                        if (uncompltedata[kan].BackCount == "Y") {
                            sidebarUncompletedata += "<button class='decButton active' type='button' id='BackDecInfo" + uncompltedata[kan].OppId + "'></button>";
                        } else {
                            sidebarUncompletedata += "<button class='decButton inactive' type='button' id='BackDecInfo" + uncompltedata[kan].OppId + "'></button>";
                        }

                        if (uncompltedata[kan].LeftCount == "Y") {
                            sidebarUncompletedata += "<button class='decButton active' type='button' id='LeftDecInfo" + uncompltedata[kan].OppId + "'></button>";
                        } else {
                            sidebarUncompletedata += "<button class='decButton inactive' type='button' id='LeftDecInfo" + uncompltedata[kan].OppId + "'></button>";
                        }

                        if (uncompltedata[kan].RightCount == "Y") {
                            sidebarUncompletedata += "<button class='decButton active' type='button' id='RightDecInfo" + uncompltedata[kan].OppId + "'></button>";
                        } else {
                            sidebarUncompletedata += "<button class='decButton inactive' type='button' id='RightDecInfo" + uncompltedata[kan].OppId + "'></button>";
                        }

                        if (uncompltedata[kan].ExtraCount == "Y") {
                            sidebarUncompletedata += "<button class='decButton active' type='button' id='OtherDecInfo" + uncompltedata[kan].OppId + "'></button>";
                        } else {
                            sidebarUncompletedata += "<button class='decButton inactive' type='button' id='OtherDecInfo" + uncompltedata[kan].OppId + "'></button>";
                        }
                        sidebarUncompletedata += "</div></div>";
                        //baans change 4Sept Ends here [P]



                        sidebarUncompletedata += "<div class='row'><div class='artRow'>";

                        sidebarUncompletedata += "<label class='KanArthead'>Art</label>";
                        //Making Art Labels Active and Inactive
                        if (uncompltedata[kan].ArtOrderedDate != null) {
                            sidebarUncompletedata += "<label class='KanArtOrderedStatus Active'>Ord</label>";
                        }
                        else {
                            sidebarUncompletedata += "<label class='KanArtOrderedStatus'>Ord</label>";
                        }
                        if (uncompltedata[kan].ApprovedDate != null) {
                            sidebarUncompletedata += "<label class='KanArtApprovedStatus Active'>App</label>";
                        }
                        else {
                            sidebarUncompletedata += "<label class='KanArtApprovedStatus'>App</label>";
                        }

                        if (uncompltedata[kan].ArtReadyDate != null) {
                            sidebarUncompletedata += "<label class='KanArtReadyStatus Active'>Go</label>";
                        }
                        else {
                            sidebarUncompletedata += "<label class='KanArtReadyStatus'>Go</label>";
                        }

                        //Making Stock Labels Active and Inactive

                        sidebarUncompletedata += "</div><div class='stkRow'>";

                        sidebarUncompletedata += "<label class='KanArthead'>Stk</label>";
                        if (uncompltedata[kan].StockOrderedDate != null) {
                            sidebarUncompletedata += "<label class='KanSktOrderedStatus Active'>Ord</label>";
                        }
                        else {
                            sidebarUncompletedata += "<label class='KanSktOrderedStatus'>Ord</label>";
                        }

                        if (uncompltedata[kan].ReceivedDate != null) {
                            sidebarUncompletedata += "<label class='KanSktApprovedStatus Active'>Rec</label>";
                        }
                        else {
                            sidebarUncompletedata += "<label class='KanSktApprovedStatus'>Rec</label>";
                        }

                        if (uncompltedata[kan].Checkeddate != null) {
                            sidebarUncompletedata += "<label class='KanSktReadyStatus Active'>Go</label>";
                        }
                        else {
                            sidebarUncompletedata += "<label class='KanSktReadyStatus'>Go</label>";
                        }

                        sidebarUncompletedata += "</div></div></div>";
                        sidebarUncompletedata += "<div class='col-lg-4 KanOppRightSection'>";

                        if (uncompltedata[kan].OppThumbnail != null) {
                        }
                        else {
                            sidebarUncompletedata += "<img src='/Content/uploads/Opportunity/NoImage.png' class='img-responsive OppImage' />";
                        }

                        sidebarUncompletedata += "</div></div>";
                    }
                }
                $("#kanSidebarOppList_uncomplete").html(sidebarUncompletedata);

                $("#kanSidebarOppList_uncomplete").css('min-height', 'auto').css('height', 'auto');

                unassignHeight = document.getElementById('kanSidebarOppList_uncomplete').offsetHeight;

                var MinHeight = parseInt(unassignHeight) + 80;
                $("#kanSidebarOppList_uncomplete").css('min-height', MinHeight + 'px');
            }
        },
    });
}


function GetValidatedDate(date) {
    var Getdate = GetmmddyyyyDate(date);
    var ValidDate = new Date(Getdate);
    var GetValidateDate = parseInt(ValidDate.getDate()) + 2;
    ValidDate.setDate(GetValidateDate);
    var finaldate = formatDate(ValidDate);
    finaldate = GetddmmyyyyDate(finaldate);
    return finaldate;
}

function GetddmmyyyyDate(date) {
    //For Setting Date In Datepicker(Entered date Will be yy-mm-dd)
    var GetDate = date.split('-');
    return GetDate[2] + "-" + GetDate[1] + "-" + GetDate[0];
}

function GetCompareDate(date) {
    //For Setting Date In Datepicker(Entered date Will be yy-mm-dd)
    var GetDate = date.split('-');
    return GetDate[2] + "/" + GetDate[1] + "/" + GetDate[0];
}

function GetyyyymmddDate(date) {
    //For Setting Date In Datepicker(Entered date Will be dd-mm-yy)
    var GetDate = date.split('-');
    return GetDate[2] + "-" + GetDate[1] + "-" + GetDate[0];
}

function GetmmddyyyyDate(date) {
    //For Setting Date In Datepicker(Entered date Will be dd-mm-yy)
    var GetDate = date.split('-');
    return GetDate[1] + "-" + GetDate[0] + "-" + GetDate[2];
}


function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [year, month, day].join('-');
}
