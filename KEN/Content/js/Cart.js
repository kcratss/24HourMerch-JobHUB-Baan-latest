$(document).ready(function () {
    var total;  
    MyCart();

});

function MyCart() {
    document.getElementById("allOrders").innerHTML = ' ';
    document.getElementById("TotalPrice").innerHTML = ' ';
  
    $('#allOrders').append('<h4 class="text-center mt-4">Cart is Empty</h4>');
    document.getElementById("checkoutButton").disabled = true;
    document.getElementById("checkout").disabled = true;
    debugger;
    $.ajax({
        url: '/Design/MyCart',
        type: "GET",
        data: {},
        success: function (data) {
            debugger;
            
            document.getElementById("TotalPrice").innerHTML = 'Total ';
            $('#TotalPrice').text();
            var ex = data.length;
            ButtonDisable(ex);
            $(".length").text(ex + "(items)");
            $(".item").text("Shopping Cart (" + ex + "items)");
            CartCount(ex);
            document.getElementById("allOrders").innerHTML = ' ';
            total = 0;
            for (let i = 0; i < data.length; i++) {
                var pricetotal = parseFloat(data[i].Unit_Price).toFixed(2);
                var subTotal = parseFloat(data[i].Total_Price).toFixed(2);
               
                total = parseFloat((+subTotal) + (+total)).toFixed(2);
                $("#allItemTotalPrice").text("$" + total);
                if (data[i].Quantity > 0) {
                    $('#allOrders').append('<div  id="' + data[i].Id + '" class="row no-gutters align-items-center px-1 border-bottom border-200"><div class="col-4 py-3 px-2 px-md-3"  > <div class="media align-items-center"><img class="rounded mr-3 d-none d-md-block" src="' + data[i].FrontImage + '" alt="" width="111" height="99" /> <div class="media-body" id="process"><h5 class="fs-0" id="' + data[i].Id + "process" + '">Process : ' + data[i].ProcessValue + '</h5><div id="color"><h5 class="fs-0 price" id="' + data[i].Id + "color" + '">Color :' + data[i].ColorValue + '</h5></div><div id="Stitches"><h5 class="fs-0 price" id="' + data[i].Id + "Stitches" + '">Stitches :' + data[i].StitchesValue + '</h5></div><div class="fs--2 fs-md--1"><a class="text-danger" href="#!" id="remove" onclick=RemoveCartItem(' + data[i].Id + ')>Remove</a></div></div></div></div><div class="col-4 p-3"><div class="row"> <div class="col-md-4 d-flex justify-content-end justify-content-md-center px-2 px-md-3 order-1 order-md-0">$' + parseFloat(data[i].LogoPrice).toFixed(2)+ '</div> <div class="col-md-4 text-right pl-0 pr-2 pr-md-3 order-0 order-md-1 mb-2 mb-md-0 text-600">$' + data[i].Tshirt_Price + '</div> <div class="col-md-4 text-right pl-0 pr-2 pr-md-3 order-0 order-md-1 mb-2 mb-md-0 text-600">$' + pricetotal+'</div></div></div><div class="col-4 p-3"><div class="row"> <div class="col-md-8 d-flex justify-content-end justify-content-md-center px-2 px-md-3 order-1 order-md-0" id="' + data[i].Id + '"><div> <div class="input-group input-group-sm"><input id="' + data[i].Id + "quantity" + '" class="form-control text-center px-2 input-quantity input-spin-none" type="number" min="0" value="' + data[i].Quantity + '"  style="max-width: 65px" readonly/></div></div></div><div class="col-md-4 text-right pl-0 pr-2 pr-md-3 order-0 order-md-1 mb-2 mb-md-0 text-600" id="' + data[i].Id + "total" + '">$' + subTotal + '</div></div></div></div>')
                }
                }            
        }
    });
}


function IncreaseQuantity(Id) {
    debugger;
    var subprice = $("#" + Id + "price").text();
    var priceSubString = parseFloat(subprice.substring(8));
    var total = $("#" + Id + "total").text();
    var totalSubString = parseFloat(total.substring(1));
    var quantity = parseInt($("#" + Id + "quantity").val()) + 1;
    var subtotal = totalSubString + priceSubString;
    var subtotalPrice = "$"+ parseFloat(subtotal).toFixed(2);
    var allItemTotalPrice = $("#allItemTotalPrice").text();
    var allItemTotalPriceSubstring = parseFloat(allItemTotalPrice.substring(1));
    var totalAllPrice = (priceSubString + allItemTotalPriceSubstring);
    var totalPrice = parseFloat(totalAllPrice).toFixed(2);
    $("#allItemTotalPrice").text("$" + totalPrice);
    $("#" + Id + "quantity").val(quantity);
    $("#" + Id + "total").text(subtotalPrice);
    $.ajax({
        method: 'POST',
        url: '/Design/UpdateCartItem',
        data: { Id: Id, quantity: quantity },
        success: function (data) {
            debugger;           
        }
    });
}

function DecreaseQuantity(Id) {
    debugger;
    var subprice = $("#" + Id + "price").text();
    var priceSubString = parseFloat(subprice.substring(8));
    var total = $("#" + Id + "total").text();
    var totalSubString = parseFloat(total.substring(1));
    var quantity = parseInt($("#" + Id + "quantity").val()) - 1;
    if (quantity == 0) {
        RemoveCartItem(Id);
    }
    else {        
        var subtotal = totalSubString - priceSubString;
        var subtotalPrice = "$" + parseFloat(subtotal).toFixed(2);
        var allItemTotalPrice = $("#allItemTotalPrice").text();
        var allItemTotalPriceSubstring = parseFloat(allItemTotalPrice.substring(1));       
        var totalAllPrice = (allItemTotalPriceSubstring - priceSubString);
        var totalPrice = parseFloat(totalAllPrice).toFixed(2);
        $("#allItemTotalPrice").text("$" + totalPrice);
        $("#" + Id + "quantity").val(quantity);
        $("#" + Id + "total").text(subtotalPrice);
        $.ajax({
            method: 'POST',
            url: '/Design/UpdateCartItem',
            data: { Id: Id, quantity: quantity },
            success: function (data) {
                
            }
        });
      
    }
}

function RemoveCartItem(Id) {  
    var quantity = 0;
    var total = $("#" + Id + "total").text();
    var totalSubString = parseInt(total.substring(1));
    var allItemTotalPrice = $("#allItemTotalPrice").text();
    var allItemTotalPriceSubstring = parseInt(allItemTotalPrice.substring(1));
    var totalAllPrice = (allItemTotalPriceSubstring - totalSubString);
    var totalPrice = parseFloat(totalAllPrice).toFixed(2);
    $("#allItemTotalPrice").text("$" + totalPrice);
    var length = $(".length").text();
    var lengthsubstring = parseInt(length.substring(0, 1)) - 1;
    ButtonDisable(lengthsubstring);
    $(".length").text(lengthsubstring + "(items)");
    $(".item").text("Shopping Cart (" + lengthsubstring + "items)");
    $("#icon").text(lengthsubstring);
    $.ajax({
        method: 'POST',
        url: '/Design/UpdateCartItem',
        data: { Id:Id,quantity:quantity },
        success: function (data) {
            $("#" + Id).remove();
            location.href = "/Design/Cart";
        }
    });

}

function CartCount(ex) {
    debugger;
    var a = $("#icon").text();
    $("#icon").text(ex);
}



var cartDataList = [];
function orderdetail() {
    $.ajax({
        url: '/Design/MyCart',
        type: "GET",
        data: {},
        success: function (data) {
            debugger;
            var ex = data.length;
            console.log(ex);
            for (let i = 0; i < data.length; i++) {
                var cartData = {};
                cartData.id = data[i].Id;
                cartData.quantity = $("#" + data[i].Id + "quantity").val();
               var total = $("#" + data[i].Id + "total").text();
                cartData.totalPrice = parseInt(total.substring(1));
                cartDataList.push(cartData);                
            }
            checkOut(cartDataList);
        }
    })
  
}

function checkOut(cartDataList) {
    $.ajax({
        method: 'POST',
        url: '/Design/UpdateCart',
        data: { cartDataList: cartDataList},
        success: function (data) {
            cartDataList = [];
            location.href = "/Design/CheckOut";
        }          
        });
}

function ButtonDisable(ex) {
    if (ex == 0) {
        document.getElementById("checkoutButton").disabled = true;
        document.getElementById("checkout").disabled = true;
        $('#allOrders').append('<h4 style="margin:26px;">Cart is Empty</h4>');
        document.getElementById("TotalPrice").innerHTML = ' ';
    }
    else {
        document.getElementById("checkoutButton").disabled = false;
        document.getElementById("checkout").disabled = false;
    }
}