$(document).ready(function () {
    ViewNotification();
    var notificationtTime;
    
});
var price;
const pageSize = 10;
var currentPage = 1;
var allDataList;

function ViewNotification() {
    $.ajax({
        url: '/Design/ViewNotification',
        type: 'GET',
        data: {},
        success: function (data) {
            debugger;          
            if (data.length != 0) {
                allDataList = data;
                $('.pagination').text('');
                $('#notificationitem').text('');
                var totalRecords = data.length;
                currentPage = 1;
                if (data.length <= pageSize) {
                    document.getElementById('prev').disabled = true;
                    document.getElementById('next').disabled = true;
                }
                else {
                    document.getElementById('next').disabled = false;
                }
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
                        var paging = parseInt(pageSize * (currentPage - 1));
                    }
                    else {
                        var paging = parseInt(0);
                    }
                    var data = allDataList.slice(paging, showData);
                    dataPagination(data);
                }


            }
            else {
                $('#notificationitem').text('');
                $('.pagination').text('');
                document.getElementById('prev').disabled = true;
                document.getElementById('next').disabled = true;
                $('#notificationitem').append('<a class="border-bottom-0 notification notification-unread rounded-0 border-x-0 border-300"  href="#!"><div class="notification-body" style="margin-left:40%;"><p class="mb-1" ><strong>No Notification Found</strong></p></div></a>')

            }
            
        }
    })
}

function timeSince(date) {
    debugger;
   
    var seconds = Math.floor((new Date() - date) / 1000);

    var interval = seconds / 31536000;

    if (interval > 1) {
        notificationtTime = Math.floor(interval) + " years ago";
        return true;
    }
    interval = seconds / 2628288;
    if (interval > 1) {
        notificationtTime = Math.floor(interval) + " months ago";
        return true;
    }
    interval = seconds / 604800;
    if (interval > 1) {
        notificationtTime = Math.floor(interval) + " Week ago";
        return true;
    }
    interval = seconds / 86400;
    if (interval > 1) {
        if (interval < 2) {
            notificationtTime = Math.floor(interval) + " day ago";
        }
        else {
            notificationtTime = Math.floor(interval) + " days ago";

        }
         return true;
    }
    interval = seconds / 3600;
     if (interval > 1) {
         notificationtTime = Math.floor(interval) + " hours ago";
         return true;
    }
    interval = seconds / 60;
    if (interval > 1) {
        notificationtTime = Math.floor(interval) + " minutes ago";
        return true;
    }
}

function ReadAllNotification() {

    $.ajax({
        url: '/Design/ReadAllNotification',
        type: 'Get',
        data: {},
        async: false,
        success: function (data) {
            var notifcationId = $(".tablinks.active").attr('id');
            if (notifcationId == "All") {
                NotificationListGrid(notifcationId);
            }
            else if (notifcationId == "Read") {
                NotificationListGrid(notifcationId);
            }
            else if (notifcationId == "Unread") {
                NotificationListGrid(notifcationId);
            }
            else {
                Notification();
                ViewNotification();
            }
        }
    })

}

function NotificationStatus(id) {
    $.ajax({
        url: '/Design/NotificationStatus',
        type: 'Get',
        data: { id, id },
        async: false,
        success: function (data) {
            debugger;
            var notifcationId = $(".tablinks.active").attr('id');
            if (notifcationId == "All") {
                NotificationListGrid(notifcationId);
            }
            else if (notifcationId == "Read") {
                NotificationListGrid(notifcationId);
            }
            else if (notifcationId == "Unread") {
                NotificationListGrid(notifcationId);
            }
            else {
                Notification();
                ViewNotification();
            }
           
        }
    })

}

function Notification() {
    debugger;
    $.ajax({
        url: '/Design/Notification',
        type: 'GET',
        data: {},
        async: false,
        success: function (data) {
            debugger;
            var a = 0;
            $('#notific').text('');
            $('#icons').text('');
            for (let i = 0; i < data.length; i++) {
                var dateString = data[i].CreatedOn;
                var num = parseInt(dateString.replace(/[^0-9]/g, ""));
                var date1 = new Date(num);
                timeSince(date1);
                if (data[i].Status == true) {
                    if (data[i].Quotes_Id == null) {
                        $('#notific').append('<div class="list-group-item" onclick=LogoDetails(' + data[i].Logo_Id + ')><a class="notification notification-flush notification-read" href="#!"><div class="notification-avatar"><div class="avatar avatar-xl me-3"><i class="fas fa-info-circle" style="color:blue;font-size:38px;"></i></div></div><div class="notification-body"><p class="mb-1"><strong>' + data[i].Message + '</strong></p><span class="notification-time"><span class="me-2" role="img" aria-label="Emoji">💬</span>' + notificationtTime +'</span></div></a></div>');
                    }
                    else {
                        $('#notific').append('<div class="list-group-item" onclick=ModalQuotes(' + data[i].Quotes_Id + '),NotificationStatus(' + data[i].Id + ');><a class="notification notification-flush notification-read" href="#!"><div class="notification-avatar"><div class="avatar avatar-xl me-3"><i class="fas fa-info-circle" style="color:blue;font-size:38px;"></i></div></div><div class="notification-body"><p class="mb-1"><strong>' + data[i].Message + '</strong></p><span class="notification-time"><span class="me-2" role="img" aria-label="Emoji">💬</span>' + notificationtTime +'</span></div></a></div>');

                    }
                }
                else {
                    $('#icons').text('');
                    a++;
                    var datalength = parseInt(a);
                    $('#icons').append(datalength);
                    if (data[i].Quotes_Id == null) {
                        $('#notific').append('<div class="list-group-item" onclick=LogoDetails(' + data[i].Logo_Id + ')><a class="notification notification-flush notification-unread" href="#!"><div class="notification-avatar"><div class="avatar avatar-xl me-3"><i class="fas fa-info-circle" style="color:blue;font-size:38px;"></i></div></div><div class="notification-body"><p class="mb-1"><strong>' + data[i].Message + '</strong></p><span class="notification-time"><span class="me-2" role="img" aria-label="Emoji">💬</span>' + notificationtTime +'</span></div></a></div>');
                    }
                    else {
                        $('#notific').append('<div class="list-group-item" onclick=ModalQuotes(' + data[i].Quotes_Id + '),NotificationStatus(' + data[i].Id + ');><a class="notification notification-flush notification-unread" href="#!"><div class="notification-avatar"><div class="avatar avatar-xl me-3"><i class="fas fa-info-circle" style="color:blue;font-size:38px;"></i></div></div><div class="notification-body"><p class="mb-1"><strong>' + data[i].Message + '</strong></p><span class="notification-time"><span class="me-2" role="img" aria-label="Emoji">💬</span>' + notificationtTime +'</span></div></a></div>');

                    }
                }
            }
        }
    })
}

// Pagination View All Notification 

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
    dataPagination(data);
}


function dataPagination(data) {
    debugger;
    if (data.length != 0) {
        var a = 0;
        $('#notificationitem').text('');
        $('#icons').text('');
        for (let i = 0; i < data.length; i++) {
            var dateString = data[i].CreatedOn;
            var num = parseInt(dateString.replace(/[^0-9]/g, ""));
            var date1 = new Date(num);
            timeSince(date1);
            if (data[i].Status == true) {
                if (data[i].Quotes_Id == null) {
                    $('#notificationitem').append('<a class="border-bottom-0 notification notification-read rounded-0 border-x-0 border-300" onclick=LogoDetails(' + data[i].Logo_Id + '),NotificationStatus(' + data[i].Id + '); href="#!"><div class="notification-avatar"><div class="avatar avatar-xl me-3"><i class="fas fa-info-circle" style="color:blue;font-size:38px;"></i></div></div><div class="notification-body"><p class="mb-1"><strong>' + data[i].Message + '</strong></p></p><span class="notification-time"><span class="me-2" role="img" aria-label="Emoji">💬</span>' + notificationtTime + '</span></div></a>')
                }
                else {
                    $('#notificationitem').append('<a class="border-bottom-0 notification notification-read rounded-0 border-x-0 border-300" onclick=ModalQuotes(' + data[i].Quotes_Id + '),NotificationStatus(' + data[i].Id + '); href="#!"><div class="notification-avatar"><div class="avatar avatar-xl me-3"><i class="fas fa-info-circle" style="color:blue;font-size:38px;"></i></div></div><div class="notification-body"><p class="mb-1"><strong>' + data[i].Message + '</strong></p></p><span class="notification-time"><span class="me-2" role="img" aria-label="Emoji">💬</span>' + notificationtTime + '</span></div></a>')
                }
            }
            else {
                $('#icons').text('');
                a++;
                var datalength = parseInt(a);
                $('#icons').append(datalength);
                if (data[i].Quotes_Id == null) {
                    $('#notificationitem').append('<a class="border-bottom-0 notification notification-unread rounded-0 border-x-0 border-300" onclick=LogoDetails(' + data[i].Logo_Id + '),NotificationStatus(' + data[i].Id + '); href="#!"><div class="notification-avatar"><div class="avatar avatar-xl me-3"><i class="fas fa-info-circle" style="color:blue;font-size:38px;"></i></div></div><div class="notification-body"><p class="mb-1"><strong>' + data[i].Message + '</strong></p></p><span class="notification-time"><span class="me-2" role="img" aria-label="Emoji">💬</span>' + notificationtTime + '</span></div></a>')
                }
                else {
                    $('#notificationitem').append('<a class="border-bottom-0 notification notification-unread rounded-0 border-x-0 border-300" onclick=ModalQuotes(' + data[i].Quotes_Id + '),NotificationStatus(' + data[i].Id + '); href="#!"><div class="notification-avatar"><div class="avatar avatar-xl me-3"><i class="fas fa-info-circle" style="color:blue;font-size:38px;"></i></div></div><div class="notification-body"><p class="mb-1"><strong>' + data[i].Message + '</strong></p></p><span class="notification-time"><span class="me-2" role="img" aria-label="Emoji">💬</span>' + notificationtTime + '</span></div></a>')

                }
            }
        }
    }
    else {
        $('#notificationitem').append('<a class="border-bottom-0 notification notification-unread rounded-0 border-x-0 border-300"  href="#!"><div class="notification-body" style="margin-left:40%;"><p class="mb-1" ><strong>No Notification Found</strong></p></div></a>')
    }
}


function NotificationGrid(evt) {
    debugger;
    var i, tablinks;
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    evt.className += " active";

}

function NotificationListGrid(element) {
    debugger;
    if (element == "All") {
        ViewNotification();
    }
    else if (element == "Read") {
        NotificationFilter(element);
    }
    else if (element == "Unread") {
        NotificationFilter(element);
    }
   
}

function NotificationFilter(element) {
    debugger
    var status = element;
    $.ajax({
        url: '/Design/NotificationFilter',
        type: 'Post',
        data: { status: status },
        success: function (data) {
            debugger;
            if (data.length != 0) {
                allDataList = data;
                $('.pagination').text('');
                $('#notificationitem').text('');
                var totalRecords = data.length;
                currentPage = 1;
                if (data.length <= pageSize) {
                    document.getElementById('prev').disabled = true;
                    document.getElementById('next').disabled = true;
                }
                else {
                    document.getElementById('next').disabled = false;
                }
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
                        var paging = parseInt(pageSize * (currentPage - 1));
                    }
                    else {
                        var paging = parseInt(0);
                    }
                    var data = allDataList.slice(paging, showData);
                    dataPagination(data);
                }
            }
            else {
                $('#notificationitem').text('');
                $('.pagination').text('');
                document.getElementById('prev').disabled = true;
                document.getElementById('next').disabled = true;
                $('#notificationitem').append('<a class="border-bottom-0 notification notification-unread rounded-0 border-x-0 border-300"  href="#!"><div class="notification-body" style="margin-left:40%;"><p class="mb-1" ><strong>No Notification Found</strong></p></div></a>')


            }
        }
    })
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
    var data = allDataList.slice(paging, showData);
    dataPagination(data);
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
        document.getElementById('prev').disabled = true
        document.getElementById('next').disabled = false;
    }
    var data = allDataList.slice(paging, showData);
    dataPagination(data);
}