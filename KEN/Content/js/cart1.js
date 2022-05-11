$(document).ready(function () {
    var total;
    MyCart();

});

function MyCart() {
    debugger;
    $.ajax({
        url: '/Design/MyCart',
        type: "GET",
        data: {},
        success: function (data) {
            var ex = data.length;
            $(".length").text(ex + "(items)");
            $(".item").text("Shopping Cart ("+ ex + "items)");
            CartCount(ex);
            document.getElementById('allOrders').innerHTML = "";

            total = 0;

            for (let i = 0; i < data.length; i++) {
                total = parseInt(data[i].Total + total);
/*                $('#allOrders').append('<div style="background-color: white;" id="' + data[i].Id + '" class="row col-8 mb-4 d-flex justify-content-start align-items-center align-content-start row1"><div class="col-3"><img src="' + data[i].tblUserItem.FrontImageSource + '"class="img-fluid rounded-3" alt="T-shirt" style = "height: 150px;"></div><div class="col-1" id="price" ><h6 id="' + data[i].Id + "price" + '" class="' + data[i].Price + '"> Price :$' + data[i].Price + '</h6></div><div class="col-2" id="process"><h6 id="' + data[i].Id + "process" + '">Process : ' + data[i].ProcessValue + '</h6></div><div class="col-2 d-flex" ><button class="btn btn-link px-2"onclick="DecreaseQuantity(' + data[i].Id + ')"><i class="fas fa-minus"></i></button><input id="' + data[i].Id + "quantity" + '" min="0" name="quantity" value="' + data[i].Quantity + '" type="number"class="form-control form-control-md form1 " readonly /><button class="btn btn-link px-2 increasebtn"onclick=IncreaseQuantity(' + data[i].Id + ') ><i class="fas fa-plus"></i></button></div><div class="col-1 " id = "label"><label for=' + data[i].Id + '>Total : $</label><h6 id="' + data[i].Id + "total" + '" class="' + data[i].Total + '">' + data[i].Total + '</h6></div><div class="col-1" id ="deleteIcon"><a href="#!" class="text-muted" onclick=RemoveCartItem(' + data[i].Id + ')><i class="fa fa-trash" aria-hidden="true" ></i></a></div></div>');
*/

                $('#allOrders').append('<div  id="' + data[i].Id + '" class="row no-gutters align-items-center px-1 border-bottom border-200"><div class="col-8 py-3 px-2 px-md-3"  > <div class="media align-items-center"><img class="rounded mr-3 d-none d-md-block" src="' + data[i].tblUserItem.FrontImageSource + '" alt="" width="92" height="70" /> <div class="media-body"><h5 class="fs-0" id="process">Process : ' + data[i].ProcessValue + '</h5><div id="price"><h5 class="fs-0 price" id="' + data[i].Id + "price" + '">Price :$' + data[i].Price + '</h5></div> <div class="fs--2 fs-md--1"><a class="text-danger" href="#!" id="remove" onclick=RemoveCartItem(' + data[i].Id + ')><i class="fa fa-trash" aria-hidden="true" ></i></a></div></div></div></div><div class="col-4 p-3"><div class="row"> <div class="col-md-8 d-flex justify-content-end justify-content-md-center px-2 px-md-3 order-1 order-md-0" id="' + data[i].Id + '"><div> <div class="input-group input-group-sm"> <div class="input-group-prepend"> <button class="btn btn-sm btn-outline-secondary border-300 px-2" data-field="input-quantity" data-type="minus" onclick="DecreaseQuantity(' + data[i].Id + ')"><i class="fas fa-minus"></i></button></div> <input id="' + data[i].Id + "quantity" + '" class="form-control text-center px-2 input-quantity input-spin-none" type="number" min="0" value="' + data[i].Quantity + '"  style="max-width: 40px" /> <div class="input-group-append"> <button class="btn btn-sm btn-outline-secondary border-300 px-2" data-field="input-quantity" data-type="plus" onclick=IncreaseQuantity(' + data[i].Id + ')><i class="fas fa-plus"></i></button></div></div></div></div><div class="col-md-4 text-right pl-0 pr-2 pr-md-3 order-0 order-md-1 mb-2 mb-md-0 text-600" id="' + data[i].Id + "total" + '">$' + data[i].Total + '</div></div></div></div>')
            }
            $("#allItemTotalPrice").text("$"+total);
        }

    });

}


function IncreaseQuantity(Id) {

    var subprice = $("#" + Id + "price").text();
    var priceSubString = parseInt(subprice.substring(8));
    var total = $("#" + Id + "total").text();
    var totalSubString = parseInt(total.substring(1));
    var quantity = parseInt($("#" + Id + "quantity").val()) + 1;
    var subtotalPrice = "$" + (totalSubString + priceSubString);
    var allItemTotalPrice = $("#allItemTotalPrice").text();
    var allItemTotalPriceSubstring = parseInt(allItemTotalPrice.substring(1));
    var totalPrice = (priceSubString + allItemTotalPriceSubstring);
    $("#allItemTotalPrice").text("$" + totalPrice);
    $("#" + Id + "quantity").val(quantity);
    $("#" + Id + "total").text(subtotalPrice);
    quantity = 0;

}

function DecreaseQuantity(Id) {

    var subprice = $("#" + Id + "price").text();
    var priceSubString = parseInt(subprice.substring(8));
    var total = $("#" + Id + "total").text();
    var totalSubString = parseInt(total.substring(1));
    var quantity = parseInt($("#" + Id + "quantity").val()) - 1;
    if (quantity == 0) {
        RemoveCartItem(Id);
    }
    else {
        var subtotalPrice = "$" + (totalSubString - priceSubString);
        var allItemTotalPrice = $("#allItemTotalPrice").text();
        var allItemTotalPriceSubstring = parseInt(allItemTotalPrice.substring(1));
        var totalPrice = (allItemTotalPriceSubstring - priceSubString);
        $("#allItemTotalPrice").text("$" + totalPrice);
        $("#" + Id + "quantity").val(quantity);
        $("#" + Id + "total").text(subtotalPrice);
        quantity = 0;
    }

}

function RemoveCartItem(Id) {
    debugger;
   
    var total = $("#" + Id + "total").text();
    var totalSubString = parseInt(total.substring(1));
    var allItemTotalPrice = $("#allItemTotalPrice").text();
    var allItemTotalPriceSubstring = parseInt(allItemTotalPrice.substring(1));
    var totalPrice = (allItemTotalPriceSubstring - totalSubString);
    $("#allItemTotalPrice").text("$" + totalPrice);
    var length = $(".length").text();
    var lengthsubstring = parseInt(length.substring(0, 1)) - 1;
    $(".length").text(lengthsubstring + "(items)");
    $(".item").text("Shopping Cart (" + lengthsubstring + "items)");
    $("#icon").text(lengthsubstring);
    $("#" + Id).remove();
}
function CartCount(ex) {

    $("#icon").text(ex);

}

function orderdetail() {
    debugger;
    $.ajax({
        url: '/Design/MyCart',
        type: "GET",
        data: {},
        success: function (data) {
        }
    })
}