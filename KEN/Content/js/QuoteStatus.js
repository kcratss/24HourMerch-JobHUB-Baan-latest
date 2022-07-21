$(document).ready(function () {
    QuoteStatusList();
})
$("#commonTabs").css('display', 'none');
const pageSize = 10;
var currentPage = 1;
var allDataList;
function QuoteStatusList() {
    $.ajax({
        url: '/Opportunity/QuoteStatusList',
        type: 'Get',
        data: {},
        async: false,
        success: function (data) {
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
                    ShowQuoteData(data);
                }
            }
            else {
                $('#item').text('');
                $('.pagination').text('');
                document.getElementById('prev').disabled = true;
                document.getElementById('next').disabled = true;

                $("#item").append('<tr class="text-center"><td colspan="8">No Quotes Found.</td></tr>');
            }
        }  
    })
}

function ShowQuoteData(data) {
    document.getElementById('item').innerHTML = '';
    if (data.length != 0) {
        for (i = 0; i < data.length; i++) {
            var subTotal = parseFloat(data[i].TotalPrice).toFixed(2);
            if (data[i].StatusName == "Pending") {
                $("#item").append('<tr><td>#' + data[i].Id + '</td><td>' + data[i].TotalItems + '</td><td>' + data[i].StatusName + '</td><td>$' + subTotal + '</td><td> <button type="button" class="btn btn-primary" onclick="ModalQuotes(' + data[i].Id + ')">Quotes Details</button></td><td><button type="button" class="btn btn-primary" onclick=ApprovedModel(' + data[i].Id + ')>Approved</button><button type="button" class="btn btn-danger" onclick=RejectedModel(' + data[i].Id + ') style = "margin-left:5px;margin-right:5px;">Rejected</button><button type="button" onclick=CalculatePrice(' + data[i].Id + ')>Re-Calculate Price</button></td><tr>');
            }
            else if (data[i].StatusName == "Approved") {
                $("#item").append('<tr><td>#' + data[i].Id + '</td><td>' + data[i].TotalItems + '</td><td>' + data[i].StatusName + '</td><td>$' + subTotal + '</td><td> <button type="button" class="btn btn-primary" onclick="ModalQuotes(' + data[i].Id + ')">Quotes Details</button></td><td></td><tr>');
            }
            else if (data[i].StatusName == "Rejected") {
                $("#item").append('<tr><td>#' + data[i].Id + '</td><td>' + data[i].TotalItems + '</td><td>' + data[i].StatusName + '</td><td>$' + subTotal + '</td><td> <button type="button" class="btn btn-primary" onclick="ModalQuotes(' + data[i].Id + ')">Quotes Details</button></td><td></td><tr>');

            }

        }
    }
    else {
        $("#item").append('<tr class="text-center"><td colspan="8">No Quotes Found.</td></tr>');
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
        document.getElementById('next').disabled = false;
    }
    if (currentPage == 1) {
        document.getElementById('prev').disabled = true;
    }
    else {
        document.getElementById('prev').disabled = false;
    }
    var data = allDataList.slice(paging, showData);
    ShowQuoteData(data);
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
    ShowQuoteData(data);
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
        document.getElementById('next').disabled = false;
    }
    if (currentPage == 1) {
        document.getElementById('prev').disabled = true;
    }
    var data = allDataList.slice(paging, showData);
    ShowQuoteData(data);
}


function QuotesList(id) {
    debugger;
    var totalprice = 0;
    
    $.ajax({
        url: '/Opportunity/QuoteList',
        type: 'Post',
        data: { id: id },
        async: false,
        success: function (data) {
            debugger;
            for (let i = 0; i < data.length; i++) {
                var length = data.length;
                $("#" + id).text(length);
                var printPrice = parseFloat(data[i].Print_Price).toFixed(2);
                var pricetotal = parseFloat(data[i].Unit_Price).toFixed(2);
                var subTotal = parseFloat(data[i].TotalPrice).toFixed(2);
                totalprice = parseFloat((+subTotal) + (+totalprice)).toFixed(2);
                $("#list1").append('<tr><td><img  onclick=PreviewModal(' + data[i].Id + ') width="139px" src =' + data[i].FrontImageSource + ' ><td>' + data[i].ProcessValue + '</td><td>' + data[i].Quantity + '</td><td>' + data[i].ColorValue + '</td><td>' + data[i].SizeValue + '</td><td>' + data[i].Size + '</td><td>$' + printPrice + '</td><td>$' + data[i].Tshirt_Price + '</td><td>$' + pricetotal + '</td><td>$' + subTotal + '</td></tr>');
                $("#price").text(totalprice);
            }
        }
    })
}


function GetQuotes() {
    debugger;
    $.ajax({
        url: '/Opportunity/GetQuotes',
        type: 'Get',
        data: {},
        success: function (data) {
            debugger;
            for (i = 0; i < data.length; i++) {
                $("#itemlist").append('<div class="card col-6 mb-3 offset-3"><div class="card-header"><span class="col-2">Quotes :#</span><span>' + data[i].Id + '</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="col-2 total">No Of Items : </span><span id="items"></span><span class="date">Status : </span><span>' + data[i].StatusName + '</span> <a href="#" aria-expanded="false" aria-controls=' + data[i].Id + 'multiCollapseExample2 class="invoice" data-target=.' + data[i].Id + 'multi-collapse data-toggle="collapse">Quotes Details</a></div><div id =' + data[i].Id + 'item></div></div>');
                QuotesList(data[i].Id);
            }
        }
    })
}

function ModalQuotes(id) {
    document.getElementById('list1').innerHTML = '';
    $("#quotesModal").modal('show');
    QuotesList(id);
}


function closeModal() {

    $("#quotesModal").modal('hide');
    $("#approvedModal").modal('hide');
    $("#rejectedModal").modal('hide');
    $("#PreviewModal").modal('hide');
    $("#priceModal").modal('hide');
}

function closePreviewModal() {

    $("#PreviewModal").modal('hide');
}

function ApprovedQuotes() {
    debugger;
    let id = modelId;
    var status = "Approved";
    $.ajax({
        url: '/Opportunity/UpdateQuotes',
        type: 'Post',
        data: { id: id, status: status },
        success: function (responses) {          
            ShowMessage(responses)
            setTimeout(function () {
                RedirectPage();
            }, 2000)
        }
    })
}

function RejectedQuotes() {
    debugger;
    let id = modelId;
    var status = "Rejected";
    $.ajax({
        url: '/Opportunity/UpdateQuotes',
        type: 'Post',
        data: { id: id, status: status },
        success: function (responses) {
            ShowMessage(responses)
            setTimeout(function () {
                RedirectPage();
            }, 2000)
        }
    })
}

function QuotesGrid(evt) {
    var i, tablinks;
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    evt.className += " active";
}

function QuotesListGrid(element)
{
    debugger;
    if (element == "All") {
        location.href = "/Opportunity/QuoteStatus";
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

function Quotesfilter(element)
{
    debugger
    var status = element;
    $.ajax({
        url: '/Opportunity/QuotesFilter',
        type: 'Post',
        data: { status:status },
        success: function (data) {
            document.getElementById('item').innerHTML = '';
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
                    ShowQuoteData(data);
                }
            }
            else {
                $('#item').text('');
                $('.pagination').text('');
                document.getElementById('prev').disabled = true;
                document.getElementById('next').disabled = true;
                $("#item").append('<tr class="text-center"><td colspan="8">No Quotes Found.</td></tr>');
            }
        }
        
    })
}


var QuotesId;

function CalculatePrice(id)
{
    debugger;
    document.getElementById('list2').innerHTML = '';
    QuotesId = id;
    $("#priceModal").modal('show');
    QuotesLi(id);
}


function QuotesLi(id) {
    debugger;
    var totalprice = 0;
    $.ajax({
        url: '/Opportunity/ProcessList',
        type: 'Get',
        async: false,
        success: function (data1) {
            debugger;
                $.ajax({
                    url: '/Opportunity/QuoteList',
                    type: 'Post',
                    data: { id: id },
                    async: false,
                    success: function (data) {
                        debugger;                       
                        for (let i = 0; i < data.length; i++) {
                            var length = data.length;
                            $("#" + id).text(length);
                            var color = '<select id=' + data[i].Id +'colorlist onchange=calculatePriceList(' + data[i].Id + ')>';
                            var Size = '<select id=' + data[i].Id +'sizelist onchange=calculatePriceList(' + data[i].Id + ')>';
                            var process = '<select id='+ data[i].Id +'process onchange=process(' + data[i].Id + ')>';
                            for (var j = 0; j < data1.length; j++) {
                                if (data[i].Process_Id == data1[j].ProcessId) {
                                    process += '<option  selected value=' + data1[j].ProcessId + '>' + data1[j].Name + '</option>'
                                   var processid = data1[j].ProcessId;
                                    $.ajax({
                                        url: '/Opportunity/ColorList',
                                        type: 'Get',
                                        data: { processid: processid },
                                        async: false,
                                        success: function (Color) {
                                            debugger;                                            
                                            for (var k = 0; k < Color.length; k++) {
                                                if (data[i].Colour_Id == Color[k].ColorId) {
                                                    color += '<option  selected value=' + Color[k].ColorId + '>' + Color[k].Name + '</option>'
                                                }
                                                else {
                                                    color += '<option  value=' + Color[k].ColorId + '>' + Color[k].Name + '</option>'
                                                }                                                
                                            }
                                            color += "</select>";
                                        }
                                    })
                                    $.ajax({
                                        url: '/Opportunity/SizeList',
                                        type: 'Get',
                                        data: { processid: processid },
                                        async: false,
                                        success: function (size) {
                                            debugger;
                                            for (var l = 0; l < size.length; l++) {
                                                if (data[i].Size_Id == size[l].SizeId) {
                                                    Size += '<option  selected value=' + size[l].SizeId + '>' + size[l].Name + '</option>'
                                                }
                                                else {
                                                    Size += '<option  value=' + size[l].SizeId + '>' + size[l].Name + '</option>'
                                                }
                                            }
                                            Size += "</select>";
                                        }
                                    })                                
                                } else
                                    process += '<option  value=' + data1[j].ProcessId + '>' + data1[j].Name + '</option>'
                            }
                            process += "</select>";
                            var pricetotal = parseFloat(data[i].Unit_Price).toFixed(2);
                            var printPrice = parseFloat(data[i].Print_Price).toFixed(2);
                            var subTotal = parseFloat(data[i].TotalPrice).toFixed(2);
                            totalprice = parseFloat((+subTotal) + (+totalprice)).toFixed(2);
                            $("#list2").append('<tr><td><img  width="139px" src =' + data[i].FrontImageSource + ' ><td id=' + data[i].Id + 'processs>' + process + '</td><td id=' + data[i].Id + 'quantity>' + data[i].Quantity + '</td><td id=' + data[i].Id + 'color>' + color + '</td><td id=' + data[i].Id + 'size>' + Size + '</td><td>' + data[i].Size + '</td><td id=' + data[i].Id + 'printprice>$' + printPrice + '</td><td id=' + data[i].Id + 'tshirtprice>$' + data[i].Tshirt_Price + '</td><td id=' + data[i].Id + 'unitprice>$' + pricetotal + '</td><td id=' + data[i].Id + 'price>$'+subTotal+'</td></tr>');
                            
                            $("#prices").text(totalprice);
                        }
                    }
                })            
        }
    })
}

function process(id)
{
    debugger;
    document.getElementById(id+'color').innerHTML = ''
    document.getElementById(id+'size').innerHTML = ''
    var processid = $("#"+id+"process").val();
    $.ajax({
        url: '/Opportunity/ColorList',
        type: 'Get',
        data: { processid: processid },
        async: false,
        success: function (Color) {
            debugger;
            var color = '<select id=' + id + 'colorlist onchange=calculatePriceList('+id+')>'
            for (var k = 0; k < Color.length; k++) {               
                    color += '<option  value=' + Color[k].ColorId + '>' + Color[k].Name + '</option>'
                
            }
            color += "</select>";
            $('#' + id + 'color').append(color);
            Size(processid,id)
        }
    })
}
function Size(processid,id) {
    debugger;
    $.ajax({
        url: '/Opportunity/SizeList',
        type: 'Get',
        data: { processid: processid },
        async: false,
        success: function (Size) {
            debugger;
            var size = '<select id=' + id + 'sizelist onchange=calculatePriceList(' +id+ ')>'
            for (var k = 0; k < Size.length; k++) {
                size += '<option  value=' + Size[k].SizeId + '>' + Size[k].Name + '</option>'

            }
            size += "</select>";
            $('#' + id + 'size').append(size);
            calculatePriceList(id)
        }
    })
}

function calculatePriceList(id)
{
    debugger;
    var processId = $("#" + id + "process").val();
    var colorid = $("#" + id + "colorlist").val();
    var sizeId = $("#" + id + "sizelist").val();
    var quantity = $("#" + id + "quantity").text();
    $.ajax({
        url: '/Opportunity/CalculatePriceQuotesItem',
        type: 'Post',
        data: { id: id, processId: processId, colorid: colorid, sizeId: sizeId, quantity: quantity },
        async: false,
        success: function (data) {
            debugger;
            if (data.IsSuccess != false) {
                var total = parseFloat($("#prices").text());
                var price = parseFloat($("#" + id + "price").text().substring(1));
                var subtotal = parseFloat(total - price);
                document.getElementById('prices').innerHTML = '';
                document.getElementById(id + 'unitprice').innerHTML = '';
                document.getElementById(id + 'price').innerHTML = '';              
                var printprice = parseFloat(data.Print_Price).toFixed(2);
                var tshirt = $("#" + id + "tshirtprice").text().substring(1);
                var unitPrice = parseFloat((+printprice) + (+tshirt)).toFixed(2);
                $("#" + id + "unitprice").text("$"+unitPrice);
                $("#" + id + "printprice").text("$" +printprice);
                var totalprice = parseFloat(unitPrice * data.quantity);
                var totallistprice = parseFloat(totalprice + subtotal);
                var subtotalPrice = parseFloat(totalprice).toFixed(2);
                var subtotallistPrice = parseFloat(totallistprice).toFixed(2);
                $("#" + id + "price").text("$" +subtotalPrice);
                $("#prices").text(subtotallistPrice);
            }
            else {
              
                alert(data.Message);
            }                      
        }
    })
}
var cartDataList = [];
function updateQuotesItem()
{
    debugger;
    id = QuotesId;
    $.ajax({
        url: '/Opportunity/QuoteList',
        type: 'Post',
        data: { id: id},
        async: false,
        success: function (data) {
            debugger;
            for (let i = 0; i < data.length; i++)
            {
                var cartobj = {};
                cartobj.Id = data[i].Id;
                cartobj.Quotes_Id = id;
                cartobj.Process_Id = $("#" + data[i].Id + "process").val();
                cartobj.Colour_Id = $("#" + data[i].Id+"colorlist").val();
                cartobj.Size_Id = $("#" + data[i].Id+"sizelist").val();
                cartobj.TotalPrice = ($("#" + data[i].Id + "price").text()).substring(1);
                cartobj.Quantity = $("#" + data[i].Id + "quantity").text();
                cartobj.Print_Price = ($("#" + data[i].Id + "printprice").text()).substring(1);
                cartobj.Tshirt_Price = ($("#" + data[i].Id + "tshirtprice").text()).substring(1);
                cartobj.Unit_Price = ($("#" + data[i].Id + "unitprice").text()).substring(1);
                cartDataList.push(cartobj);
               
            }
            updateQuotes(cartDataList);
        }
    })
}

function updateQuotes(cartDataList)
{
    $.ajax({
        method: 'POST',
        url: '/Opportunity/UpdateQuotesitem',
        data: { cartDataList: cartDataList },
        async: false,
        success: function (responses) {
            debugger;
            ShowMessage(responses)
            setTimeout(function () {
                RedirectPage();
            }, 2000)
            cartDataList = [];
            
        }       
    });
}

function RedirectPage() {
    window.location.href = '/Opportunity/QuoteStatus';
}


function ShowMessage(responses) {
   
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
        "zIndex": 99999,
        "hideMethod": "fadeOut",

    }

    if (responses.IsSuccess) {
        toastr["success"](responses.Message);
    }
    else {
        toastr["error"](responses.Message);
    }
}

var modelId;
function ApprovedModel(id)
{
    modelId = id;
    debugger;
    $("#approvedModal").modal('show');

}
function RejectedModel(id) {
    modelId = id;
    debugger;
    $("#rejectedModal").modal('show');

}

function PreviewModal(id)
{
    debugger;
    document.getElementById('preview').innerHTML = '';
    document.getElementById('pre').innerHTML = '';
    $("#PreviewModal").modal('show');
    $.ajax({
        url: '/Opportunity/PreviewImage',
        type: 'GET',
        data: { id: id },
        async: false,
        success: function (data) {
            debugger;
            $("#preview").append('<img  class="light-zoom" width="340" src=' + data.FrontImageSource + '>');
            $("#pre").append('<img  width="340" src=' + data.BackImageSource + '>');

        }
    })
}