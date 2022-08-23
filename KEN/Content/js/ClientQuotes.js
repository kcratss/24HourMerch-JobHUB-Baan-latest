$(document).ready(function () {   
        Quotes();         
});

function Quotes() {
    debugger;
    $.ajax({
        url: '/Design/Quotes',
        type: 'GET',
        data: {},
        async: false,
        success: function (data) {
            $("#quotesModal").modal('hide');
            $("#removeModal").modal('hide');
            if (data.length != 0) {
                allDataList = data;
                $('#quotedetaillist').text('');
                $('#quotedetaillist').append('<div id="tableExample2"><div class="table-responsive scrollbar"><table class="table table-bordered table-striped fs--1 mb-0" id="addresstable"><thead class="bg-200 text-900"><tr><th class="sort" data-sort="quote">QuoteId</th><th class="sort" data-sort="item">No. of Items</th><th class="sort" data-sort="status">Status</th><th class="sort" data-sort="attention">Estimated Price</th><th class="sort" data-sort="address1">Quote details</th><th class="sort" data-sort="quote">Quote_Date</th><th class="sort" data-sort="address2">Action</th></tr></thead><tbody class="list" id="list1"></tbody></table></div><div class="d-flex justify-content-center mt-3"><button class="btn btn-sm btn-falcon-default me-1" type="button" title="Previous" id ="prev" onclick="Prevbtn()" data-list-pagination="prev"><span class="fas fa-chevron-left"></span></button><ul class="pagination mb-0"></ul><button class="btn btn-sm btn-falcon-default ms-1" id ="next" onclick="Nextbtn()" type="button" title="Next" data-list-pagination="next"><span class="fas fa-chevron-right"></span></button></div></div>')
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
                $('#list1').text('');
                $('.pagination').text('');
                document.getElementById('prev').disabled = true;
                document.getElementById('next').disabled = true;

                $("#list1").append('<tr class="text-center"><td colspan="8">No Quotes Found.</td></tr>');
            }
        }
        })
}




var price;
const pageSize = 5;
var currentPage = 1;
var allDataList;
function ClientQuotesList(id) {   
    debugger;
    $.ajax({
        url: '/Design/QuoteList',
        type: 'Post',
        data: { id: id },
        async: false,
        success: function (data) {
            debugger;
            price = 0;
            $("#price").text('');
            if (data.length > 0) {
                for (let i = 0; i < data.length; i++) {
                    var length = data.length;
                    $("#code1").text(length);
                    var printPrice = parseFloat(data[i].Print_Price).toFixed(2);
                    var pricetotal = parseFloat(data[i].Unit_Price).toFixed(2);
                    var subTotal = parseFloat(data[i].TotalPrice).toFixed(2);                    
                    price = parseFloat((+subTotal) + (+price)).toFixed(2);
                    if (data[i].Status == "Draft" || data[i].Status == "Approved") {
                        $("#listdetail").append('<tr><td><img  width="139px" src =' + data[i].FrontImageSource + ' ><td>' + data[i].ProcessValue + '</td><td>' + data[i].Quantity + '</td><td>' + data[i].ColorValue + '</td><td>' + data[i].SizeValue + '</td><td>' + data[i].Size + '</td><td>$' + printPrice + '</td><td>$' + data[i].Tshirt_Price + '</td><td>$' + pricetotal + '</td><td>$' + subTotal + '</td><td> <button type="button" id="addressbtn" onclick="RemoveQuoteItem(' + data[i].Id + ')"><i class="fas fa-trash-alt" style="color:red; font-size:24px;"></i></button></td></tr>');
                        $("#price").text("$" + price);
                    }
                    else {
                        $("#listdetail").append('<tr><td><img  width="139px" src =' + data[i].FrontImageSource + ' ><td>' + data[i].ProcessValue + '</td><td>' + data[i].Quantity + '</td><td>' + data[i].ColorValue + '</td><td>' + data[i].SizeValue + '</td><td>' + data[i].Size + '</td><td>$' + printPrice + '</td><td>$' + data[i].Tshirt_Price + '</td><td>$' + pricetotal + '</td><td>$' + subTotal + '</td><td></td></tr>');
                        $("#price").text("$" + price);
                    }
                }
            }
            else {
                $('#total').text('');
                $("#listdetail").append('<tr class="text-center"><td colspan="8">No Quotes Found.</td></tr>');
            }
        }
    })
}

function RemoveQuoteItem(id) {
    $.ajax({
        url: '/Design/RemoveQuoteItem',
        type: 'Post',
        data: { id: id },
        async: false,
        success: function (response) {
            ShowMessage(response);
            setTimeout(function () {             
            }, 2000)
            $("#quotesModal").modal('hide');
            RedirectQuoteItem();
           
        }
    })
}
var removeQuoteModelId;

/*unction RemoveQuotesModel(id) {
    $("#removeModal").modal('show');
    removeQuoteModelId = id;
}

function RemoveQuote() {
    var id = removeQuoteModelId;
    $.ajax({
        url: '/Design/RemoveQuote',
        type: 'Post',
        data: { id: id },
        async: false,
        success: function (response) {
            ShowMessage(response);
            setTimeout(function () {               
            }, 2000)
            RedirectQuoteItem();
        }
    })
}*/

function ModalQuotes(id) {
    debugger;
    document.getElementById('listdetail').innerHTML = '';
    $("#quotesModal").modal('show');
    ClientQuotesList(id);
}


function closeModal() {
    $("#quotesModal").modal('hide');
    $("#requestModal").modal('hide');
    $('#logoModal').modal('hide');
    $('#removeModal').modal('hide');
    $('#placeOrderModal').modal('hide');

        
}
var getRequestId;
function UpdateQuotes()
{
    var id = getRequestId;
    $.ajax({
        url: '/Design/UpdateQuotes',
        type: 'Get',
        data: {id:id},
        success: function (response) {
            $("#requestModal").modal('hide');
            ShowMessage(response);
            setTimeout(function () {               
            }, 2000)
            RedirectQuoteItem();
        }
    })
}

function GetRequestModal(id) {
    getRequestId = id;
    $("#requestModal").modal('show');
}


function ShowMessage(response) {
    toastr.options =
    {
        "closeButton": true,
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

    if (response.IsSuccess) {
        toastr["success"](response.Message);
    }
    else {
        toastr["error"](response.Message);
    }
}

function RedirectPage() {
    window.location.href = '/Design/Quote';
}

function QuotesGrid(evt) {
    debugger;
    var i, tablinks;
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    evt.className += " active";
    
}

function QuotesListGrid(element) {
    debugger;
    if (element == "All") {
        Quotes();
    }
    else if (element == "Draft") {
        Quotesfilter(element);
    }
    else if (element == "Pending") {
        Quotesfilter(element);
    }
    else if (element == "Approved") {
        Quotesfilter(element);
    }
    else {
        Quotesfilter(element);
    }
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
        document.getElementById('next').disabled =false;
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
    $('#list1').text('');
    $("#removeModal").modal('hide');
    if (data.length != 0) {
        for (i = 0; i < data.length; i++) {
            debugger
            var dateString = data[i].CreatedOn;
            var num = parseInt(dateString.replace(/[^0-9]/g, ""));
            var date1 = new Date(num);
            var date = new Date(date1.getTime() - (date1.getTimezoneOffset() * 60000)).toISOString().split("T")[0];
            var subTotal = parseFloat(data[i].TotalPrice).toFixed(2);
            if (data[i].StatusName == "Draft") {
                $("#list1").append('<tr><td>#' + data[i].Id + '</td><td>' + data[i].TotalItems + '</td><td>' + data[i].StatusName + '</td><td>$' + subTotal + '</td><td> <button type="button" class="btn btn-secondary" onclick="ModalQuotes(' + data[i].Id + ')">Quote Details</button></td><td>' + date + '</td><td><button type="button" class="btn btn-primary" onclick=GetRequestModal(' + data[i].Id + ')>Request Quote</button></td></tr>');
            }
            else if (data[i].StatusName == "Pending") {
                $("#list1").append('<tr><td>#' + data[i].Id + '</td><td>' + data[i].TotalItems + '</td><td>' + data[i].StatusName + '</td><td>$' + subTotal + '</td><td> <button type="button" class="btn btn-secondary" onclick="ModalQuotes(' + data[i].Id + ')">Quote Details</button></td><td>' + date +'</td></tr>');
            }
            else if (data[i].StatusName == "Approved") {
                $("#list1").append('<tr><td>#' + data[i].Id + '</td><td>' + data[i].TotalItems + '</td><td>' + data[i].StatusName + '</td><td>$' + subTotal + '</td><td> <button type="button" class="btn btn-secondary" onclick="ModalQuotes(' + data[i].Id + ')">Quote Details</button></td><td>' + date + '</td><td><button type="button" class="btn btn-primary" onclick =PlaceOrderModal(' + data[i].Id + ')>Place Order</button></td></tr>');
            }
            else if (data[i].StatusName == "Rejected") {
                $("#list1").append('<tr><td>#' + data[i].Id + '</td><td>' + data[i].TotalItems + '</td><td>' + data[i].StatusName + '</td><td>$' + subTotal + '</td><td> <button type="button" class="btn btn-secondary" onclick="ModalQuotes(' + data[i].Id + ')">Quote Details</button></td><td>' + date +'</td><td></td></tr>');
            }
        }
    }
    else {
        $("#list1").append('<tr class="text-center"><td colspan="8">No Quotes Found.</td></tr>');
    }
}



function Quotesfilter(element) {
    debugger
    var status = element;
    $.ajax({
        url: '/Design/QuotesFilter',
        type: 'Post',
        data: { status: status },
        success: function (data) {
            debugger;
            if (data.length != 0) {               
                allDataList = data;
                $('#quotedetaillist').text('');
                $('#quotedetaillist').append('<div id="tableExample2"><div class="table-responsive scrollbar"><table class="table table-bordered table-striped fs--1 mb-0" id="addresstable"><thead class="bg-200 text-900"><tr><th class="sort" data-sort="quote">QuoteId</th><th class="sort" data-sort="item">No. of Items</th><th class="sort" data-sort="status">Status</th><th class="sort" data-sort="attention">Estimated Price</th><th class="sort" data-sort="address1">Quote details</th><th class="sort" data-sort="quote">Quote_Date</th><th class="sort" data-sort="address2">Action</th></tr></thead><tbody class="list" id="list1"></tbody></table></div><div class="d-flex justify-content-center mt-3"><button class="btn btn-sm btn-falcon-default me-1" type="button" title="Previous" id ="prev" onclick="Prevbtn()" data-list-pagination="prev"><span class="fas fa-chevron-left"></span></button><ul class="pagination mb-0"></ul><button class="btn btn-sm btn-falcon-default ms-1" id ="next" onclick="Nextbtn()" type="button" title="Next" data-list-pagination="next"><span class="fas fa-chevron-right"></span></button></div></div>')
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
                $('#list1').text('');
                $('.pagination').text('');
                document.getElementById('prev').disabled = true;
                document.getElementById('next').disabled = true;

                $("#list1").append('<tr class="text-center"><td colspan="8">No Quotes Found.</td></tr>');
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

var placeOrderId;
function PlaceOrderModal(id) {
    $('#placeOrderModal').modal('show');
    placeOrderId = id;
}

function AddItemToCart() {
    var id = placeOrderId;
    debugger;
    $.ajax({
        url: '/Design/AddToCart',
        type: 'Get',
        data: { id, id },
        async: false,
        success: function (response) {           
            ShowMessage(response);
            setTimeout(function () {             
            }, 2000)
            $('#placeOrderModal').modal('hide');
            RedirectQuoteItem();
        }
    })
}

function RedirectQuoteItem() {
    var quoteListId = $(".tablinks.active").attr('id');
    if (quoteListId == "All") {
        QuotesListGrid(quoteListId);
    }
    else if (quoteListId == "Draft") {
        QuotesListGrid(quoteListId);
    }
    else if (quoteListId == "Pending") {
        QuotesListGrid(quoteListId);
    }
    else if (quoteListId == "Approved") {
        QuotesListGrid(quoteListId);
    }
    else if (quoteListId == "Rejected") {
        QuotesListGrid(quoteListId);
    }    
}

