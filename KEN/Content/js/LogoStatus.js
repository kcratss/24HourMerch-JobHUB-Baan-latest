$(document).ready(function () {
    LogoStatusList();
})

const pageSize = 15;
var currentPage = 1;
var allDataList;
function LogoStatusList() {
    $.ajax({
        url: '/Opportunity/LogoStatusList',
        type: 'Get',
        data: {},
        success: function (data) {
            debugger;
            if (data.length != 0) {
                if (data.length == 1) {
                    document.getElementById('prev').disabled = true;
                    document.getElementById('next').disabled = true;

                }
                allDataList = data;
                var totalRecords = data.length;
                currentPage = 1;
                let pageQuantity = Math.ceil(totalRecords / pageSize);
                for (let i = 1; i <= pageQuantity; i++) {
                    if (i == 1) {
                        $('.pagination').append('<li><button class="page active" onclick ="pagbutton(this)" type="button">' + i + '</button></li>')
                    }
                    else {
                        $('.pagination').append('<li><button class="page" onclick ="pagbutton(this)" type="button">' + i + '</button></li>')

                    }
                    document.getElementById('prev').disabled = true;
                    var showData = parseInt(pageSize * currentPage);
                    if (currentPage > 1) {
                        var paging = parseInt(showData - currentPage);
                    }
                    else {
                        var paging = parseInt(0);
                    }
                    var data = allDataList.slice(paging, showData);
                    ShowLogoData(data);
                }
            }
            else {
                $('#list').text('');
                $('.pagination').text('');
                document.getElementById('prev').disabled = true;
                document.getElementById('next').disabled = true;
                $("#list").append('<tr class="text-center"><td colspan="12">No Quotes Found.</td></tr>');
            }       
        }
    })
}

function pagbutton(evt) {
    debugger;
    var i, tablinks;
    tablinks = document.getElementsByClassName("page");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    evt.className += " active";
    var b = document.getElementsByClassName("page");
    currentPage = parseInt($('button.page.active').text());
    var showData = parseInt(pageSize * currentPage);
    if (currentPage > 1) {
        var paging = parseInt(pageSize * (currentPage - 1));
    }
    else {
        var paging = parseInt(0);
    }
    let totalPage = Math.ceil((allDataList.length) / pageSize);
    if (totalPage <= currentPage) {
        document.getElementById('next').disabled = true;
    }
    else {
        document.getElementById('next').disabled = false;
    }
    if (currentPage == 1) {
        document.getElementById('prev').disabled = true;
    }
    else {
        document.getElementById('prev').disabled = false;
    }
    var data = allDataList.slice(paging, showData);
    ShowLogoData(data);
}

function Nextbtn() {
    debugger;
    currentPage++;
    tablinks = document.getElementsByClassName("page");
    for (i = 0; i < tablinks.length; i++) {
        if (tablinks[i].className == 'page active') {
            var Pages = parseInt($('.page.active').text());
            tablinks[i].className = tablinks[i].className.replace(" active", "");
        }
        else {
            if ((Pages + 1) == currentPage) {
                tablinks[i].className += " active";
                Pages = 0;
            }
        }
    }
    document.getElementById('prev').disabled = false;

    var showData = parseInt(pageSize * currentPage);
    if (currentPage > 1) {
        var paging = parseInt(pageSize * (currentPage - 1));
    }
    else {
        var paging = parseInt(0);
    }
    let totalPage = Math.ceil((allDataList.length) / pageSize);
    if (totalPage <= currentPage) {
        document.getElementById('next').disabled = true;
    }
    else {
        document.getElementById('next').disabled = false;
    }
    var data = allDataList.slice(paging, showData);
    ShowLogoData(data);
}

function Prevbtn() {
    debugger;
    currentPage--;
    var pages1
    tablinks = document.getElementsByClassName("page");
    for (i = 0; i < tablinks.length; i++) {
        if (tablinks[i].className == 'page active') {
            tablinks[i].className = tablinks[i].className.replace(" active", "");
            tablinks[i - 1].className += " active";
        }
    }
    var showData = parseInt(pageSize * currentPage);
    if (currentPage > 1) {
        var paging = parseInt(pageSize * (currentPage - 1));
        document.getElementById('next').disabled = false;      
    }
    else {
        var paging = parseInt(0);       
    }
    if (currentPage == 1) {
        document.getElementById('prev').disabled = true;
    }
    var data = allDataList.slice(paging, showData);
    ShowLogoData(data);
}

function ShowLogoData(data) {
    document.getElementById('list').innerHTML = '';
    if (data.length != 0) {
        for (i = 0; i < data.length; i++) {
            if (data[i].StatusValue == "Pending") {
                $("#list").append('<tr><td><img onclick=LogoPreview(' + data[i].Id + ') src=' + data[i].LogoUrl + ' style="height:121px; width:108px; " /></td><td>' + data[i].UserName + '</td><td>' + data[i].UserEmail + '</td><td>' + data[i].StatusValue + '</td><td>' + data[i].Height + '</td><td>' + data[i].Width + '</td><td><button type="button" class="btn btn-primary" onclick="LogoProcessDetails(' + data[i].Id + ')">Logo Detail</button></td><td>' + data[i].LogoCreateDateString + '</td><td>' + data[i].ApprovedLogoDateString + '</td><td>' + data[i].ApprovedLogo_UserName + '</td><td>' + data[i].RejectedLogoDateString + '</td><td>' + data[i].RejectedLogo_UserName + '</td><td><button type="button" class="btn btn-primary" onclick= ApproveModel(' + data[i].Id + ')>Approve</button>' + " " + '<button type="button" class="btn btn-danger" onclick= RejectModel(' + data[i].Id + ')>Reject</button></td></tr>');
            }
            else if (data[i].StatusValue == "Approved") {
                $("#list").append('<tr><td><img  onclick=LogoPreview(' + data[i].Id + ') src=' + data[i].LogoUrl + ' style="height:121px; width:108px; "/></td><td>' + data[i].UserName + '</td><td>' + data[i].UserEmail + '</td><td>' + data[i].StatusValue + '</td><td>' + data[i].Height + '</td><td>' + data[i].Width + '</td><td><button type="button" class="btn btn-primary" onclick="LogoProcessDetails(' + data[i].Id + ')">Logo Detail</button></td><td>' + data[i].LogoCreateDateString + '</td><td>' + data[i].ApprovedLogoDateString + '</td><td>' + data[i].ApprovedLogo_UserName + '</td><td></td><td></td><td></td></tr>');
            }
            else if (data[i].StatusValue == "Rejected") {
                $("#list").append('<tr><td><img onclick=LogoPreview(' + data[i].Id + ')  src=' + data[i].LogoUrl + ' style="height:121px; width:108px; " /></td><td>' + data[i].UserName + '</td><td>' + data[i].UserEmail + '</td><td>' + data[i].StatusValue + '</td><td>' + data[i].Height + '</td><td>' + data[i].Width + '</td><td><button type="button" class="btn btn-primary" onclick="LogoProcessDetails(' + data[i].Id + ')">Logo Detail</button></td><td>' + data[i].LogoCreateDateString + '</td><td></td><td></td><td>' + data[i].RejectedLogoDateString + '</td><td>' + data[i].RejectedLogo_UserName + '</td><td></tr>');
            }

        }
    }
    else {
        $("#list").append('<tr class="text-center"><td colspan="12">No Logo Found.</td></tr>');
    }
}

function LogoGrid(evt) {
    var i, tablinks;
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    evt.className += " active";
}

function LogoListGrid(element) {
    debugger;
    if (element == "All") {
        location.href = "/Opportunity/LogoStatus";
    }
    else if (element == "Pending") {
        Logofilter(element);
    }
    else if (element == "Approved") {
        Logofilter(element);
    }
    else {
        Logofilter(element);
    }

}

function Logofilter(element) {
    debugger
    var status = element;
    $.ajax({
        url: '/Opportunity/LogoFilter',
        type: 'Post',
        data: { status: status },
        success: function (data) {
            debugger;
            document.getElementById('list').innerHTML = '';
            $('.pagination').text('');
            if (data.length != 0) {
                if (data.length == 1) {
                    document.getElementById('prev').disabled = true;
                    document.getElementById('next').disabled = true;

                }
                allDataList = data;
                var totalRecords = data.length;
                currentPage = 1;
                let pageQuantity = Math.ceil(totalRecords / pageSize);
                for (let i = 1; i <= pageQuantity; i++) {
                    if (i == 1) {
                        $('.pagination').append('<li><button class="page active" onclick ="pagbutton(this)" type="button">' + i + '</button></li>')
                    }
                    else {
                        $('.pagination').append('<li><button class="page" onclick ="pagbutton(this)" type="button">' + i + '</button></li>')

                    }
                    document.getElementById('prev').disabled = true;
                    var showData = parseInt(pageSize * currentPage);
                    if (currentPage > 1) {
                        var paging = parseInt(showData - currentPage);
                    }
                    else {
                        var paging = parseInt(0);
                    }
                    var data = allDataList.slice(paging, showData);
                    ShowLogoData(data);
                }
            }
            else {
                $('#list').text('');
                $('.pagination').text('');
                document.getElementById('prev').disabled = true;
                document.getElementById('next').disabled = true;
                $("#list").append('<tr class="text-center"><td colspan="12">No Quotes Found.</td></tr>');
            }
        }
    })
}


var modelId;
function ApproveModel(id) {
    modelId = id;
    debugger;
    $("#approveModal").modal('show');
}


function ApproveLogo() {
    let id = modelId;
    var status = "Approved";
    $.ajax({
        url: '/Opportunity/UpdateLogo',
        type: 'Post',
        data: { id: id, status: status },
        success: function (responses) {
            $("#approveModal").modal('hide');
            ShowMessage(responses);
            setTimeout(function () {
                RedirectPage();
            }, 2000)
        }
    })
}

function RejectLogo() {
    let id = modelId;
    var status = "Rejected";
    $.ajax({
        url: '/Opportunity/UpdateLogo',
        type: 'Post',
        data: { id: id, status: status },
        success: function (responses) {
            debugger;
            $("#rejectModal").modal('hide');
            ShowMessage(responses);
            setTimeout(function () {
                RedirectPage();
            }, 2000)
        }
    })
}

function RejectModel(id) {
    modelId = id;
    debugger;
    $("#rejectModal").modal('show');

}

function RedirectPage() {
    window.location.href = '/Opportunity/LogoStatus';
}


function ShowMessage(responses) {

    toastr.options =
    {
        "closeButton": true,
        "z-index": 999999,
        "debug": false,
        "positionClass": "toaster-top-width",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "400",
        "hideDuration": "1000",
        "fadeIn": "300",
        "fadeOut": "100",
        "timeOut": "3000",
        "extendedTimeOut": "1000",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut",

    }

    if (responses.IsSuccess) {
        toastr["success"](responses.Message);
    }
    else {
        toastr["error"](responses.Message);
    }
}

function LogoPreview(id) {
    document.getElementById('logoPreview').innerHTML = '';
    $("#LogoPreviewModal").modal('show');
    $.ajax({
        url: '/Opportunity/LogoPreview',
        type: 'GET',
        data: { id: id },
        async: false,
        success: function (data) {
            debugger;
            $("#logoPreview").append('<img  class="light-zoom" width="340" style="margin-left:84px;" src=' + data.LogoUrl + '>');
        }
    })
}

function closeModal() {
    $("#LogoPreviewModal").modal('hide');
    $("#approveModal").modal('hide');
    $("#rejectModal").modal('hide');
    $("#logoDetailModal").modal('hide');
}

function LogoProcessDetails(id)
{
    debugger;
    $("#logolist").text('');
    $("#logoDetailModal").modal('show');    
    $.ajax({
        url: '/Opportunity/LogoProcessDetails',
        type: 'GET',
        data: { id: id },
        async: false,
        success: function (data) {
            debugger;
            for (var i = 0; i < data.length; i++) {
                $('#logolist').append('<tr><td>' + data[i].ProcessValue + '</td><td>' + data[i].ColorValue + '</td><td>' + data[i].SizeValue + '</td></tr>');
            }
        }
    })
}