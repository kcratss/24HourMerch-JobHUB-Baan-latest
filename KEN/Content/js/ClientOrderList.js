$(document).ready(function () {
    ClientOrder();
});

function orderList(id)
{
    var price = 0; 
    debugger;
    $.ajax({
        url: '/Order/ClientOrderItemList',
        type: 'Post',
        data: { id: id },
        async: false,
        success: function (data) {
            debugger;
                for (let i = 0; i < data.length; i++) {                    
                    var totalprice = parseFloat(data[i].Total_Price).toFixed(2);                  
                    $("#" + id + "item").append('<div class="collapse show ' + data[i].OrderId + 'multi-collapse"><div class="card-body d-flex"> <input type="hidden" id=' + data[i].Id + 'itemid name="custId" value=' + data[i].UserItemId + '><input type="hidden" id=' + data[i].Id + 'process name="custId" value=' + data[i].Process + '><div class="img"><img  width="139px" height="122px" src =' + data[i].FrontImage + ' ></div><div class="process"><span class="card-title1">Process:</span><p>' + data[i].ProcessValue + '</p><span class="card-title1">Color:</span><p>' + data[i].ColorValue + '</p><span class="card-title1">Stitches:</span><p>' + data[i].StitchesValue + '</p></div><div class="price"><span class="card-title1">Quantity:</span><p id=' + data[i].Id + 'quantity>' + data[i].Quantity + '</p><span class="card-total">UnitPrice: </span><p id=' + data[i].Id + 'price>$' + data[i].Unit_Price + '</p><span class="card-total">Total:</span><p class="card-price">$' + totalprice + '</p></div><div><a href="#" class="a-button-text" role="button" id="a-autoid-2-announce" onclick= AddCart(' + data[i].Id + ')><i class="fa fa-share fa-lg" aria-hidden="true"></i>  ReOrder</a></div></div></div></div>');

                    }
                }
    })
}


function ClientOrder() {
    debugger;
    $.ajax({
        url: '/Order/ClientOrder',
        type: 'Get',
        data: {},
        success: function (data) {
            debugger;
            if (data.length > 0) {
                data.sort((a, b) => parseInt(b.Id) - parseInt(a.Id));
                for (let j = 0; j < data.length; j++) {
                    var totalPrice = parseFloat(data[j].TotalPrice).toFixed(2);
                    var dateString = data[j].OrderDate;
                    var num = parseInt(dateString.replace(/[^0-9]/g, ""));
                    var date1 = new Date(num);
                    var date = new Date(date1.getTime() - (date1.getTimezoneOffset() * 60000)).toISOString().split("T")[0];
                    $("#itemlist").append('<div class="card col-6 mb-3 offset-3"><div class="card-header"><span class="col-2">ORDER :#</span><span>' + data[j].Id + '</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="col-2 total">TOTAL : $</span><span>' + totalPrice + '</span><span class="date">ORDERDATE : </span><span>' + date + '</span> <a href="#" aria-expanded="false" aria-controls=' + data[j].Id + 'multiCollapseExample2 class="invoice" data-target=.' + data[j].Id + 'multi-collapse data-toggle="collapse">Invoice Details</a></div><div id =' + data[j].Id + 'item></div></div>');
                    orderList(data[j].Id);
                }
            }
            else {
                $("#itemlist").append('<div class="card col-6 mb-3 offset-3" id="cart"><h5 style="margin-top: 61px;">You have not placed any orders<h5></div>');

            }
        }
    })
}


function closeModal()
{
    debugger;
    $("#exampleModalLabel").modal('hide');
    $("#itemlist").text('');
}

function RedirectPage() {
    debugger;
    window.location.href = '/Design/DesignerTool';
}

function AddCart(id) {
    debugger;
    var itemId = $("#" + id + "itemid").val();
    var process = $("#" + id + "process").val();
    var unitPrice = $("#" + id + "price").text();
    var price = unitPrice.substring(1);
    var quantity = $("#" + id + "quantity").text();
    $.ajax({
        url: '/Design/ReorderCartItem',
        type: 'POST',
        data: {id:id},
        success: function (response) {
            location.href = "../Design/Cart";
        }
    });
}

function orderListFilter()
{
    debugger;
    var order = $("#orderfilter").val();
    if (order == 'all') {
        window.location.href ='/Order/ClientOrderList';
    }
    else if (order == 'oneweek') {
        ClientFilterOrder(order)
    }
    else if (order == '2021') {
        ClientFilterOrder(order)
    }
    else {
        ClientFilterOrder(order)
    }

}

function ClientFilterOrder(order) {
    $.ajax({
        url: '/Order/ClientOrderFilter',
        type: 'Get',
        data: {order:order},
        success: function (data) {
            debugger;
            document.getElementById('itemlist').innerHTML = '';
            if (data.length > 0) {

                data.sort((a, b) => parseInt(b.Id) - parseInt(a.Id));
                for (let j = 0; j < data.length; j++) {
                    var totalPrice = parseFloat(data[j].TotalPrice).toFixed(2);
                    var dateString = data[j].OrderDate;
                    var num = parseInt(dateString.replace(/[^0-9]/g, ""));
                    var date1 = new Date(num);
                    var date = new Date(date1.getTime() - (date1.getTimezoneOffset() * 60000)).toISOString().split("T")[0];
                    $("#itemlist").append('<div class="card col-6 mb-3 offset-3"><div class="card-header"><span class="col-2">ORDER :#</span><span>' + data[j].Id + '</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="col-2 total">TOTAL : $</span><span>' + totalPrice + '</span><span class="date">ORDERDATE : </span><span>' + date + '</span> <a href="#" aria-expanded="false" aria-controls=' + data[j].Id + 'multiCollapseExample2 class="invoice" data-target=.' + data[j].Id + 'multi-collapse data-toggle="collapse">Invoice Details</a></div><div id =' + data[j].Id + 'item></div></div>');
                    orderList(data[j].Id);
                }
            }
            else {
                $("#itemlist").append('<div class="card col-6 mb-3 offset-3" id="cart"><h5 style="margin-top: 61px;">You have not placed any orders<h5></div>');
            }
        }
    })
}
