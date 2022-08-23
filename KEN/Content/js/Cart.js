$(document).ready(function () {
    var total;  
    MyCart();
    /*Notification();*/
});

/*function Notification() {
    debugger;
    $.ajax({
        url: '/Design/Notification',
        type: 'GET',
        data: {},
        success: function (data) {
            debugger;
            $('#notific').text('');
            $('#icons').text('');
            $('#icons').append(data.length);
            for (let i = 0; i < data.length; i++) {
                if (data[i].Quotes_Id == null) {
                    $('#notific').append('<div class="list-group-item"><a class="notification notification-flush notification-unread" href="#!"><div class="notification-avatar"><div class="avatar avatar-2xl me-3"></div></div><div class="notification-body"><p class="mb-1"><strong>' + data[i].Message + '</strong> " ✌️ "</p><span class="notification-time"><span class="me-2" role="img" aria-label="Emoji">💬</span>Just now</span></div></a></div>');
                }
                else {
                    $('#notific').append('<div class="list-group-item" onclick=QuoteNotification(' + data[i].Quotes_Id + ')><a class="notification notification-flush notification-unread" href="/Design/Quote?' + data[i].Quotes_Id + '"><div class="notification-avatar"><div class="avatar avatar-2xl me-3"></div></div><div class="notification-body"><p class="mb-1"><strong>' + data[i].Message + '</strong> " ✌️ "</p><span class="notification-time"><span class="me-2" role="img" aria-label="Emoji">💬</span>Just now</span></div></a></div>');

                }
            }
        }
    })
}*/

function closeModal() {
   
    $("#quotesModal").modal('hide');
    $('#logoModal').modal('hide');
    $('#PreviewModal').modal('hide');
    
}

function MyCart() {
    document.getElementById("allOrders").innerHTML = ' ';
    document.getElementById("TotalPrice").innerHTML = ' ';
    document.getElementById("checkoutButton").disabled = true;
    document.getElementById("checkout").disabled = true;
    var a = parseInt($("#icon").text());
    if (a == 0) {
        $('#allOrders').append('<h4 class="text-center mt-4">Cart is Empty</h4>');
    }
    debugger;
    $.ajax({
        url: '/Design/MyCart',
        type: "GET",
        data: {},
        async:false,
        success: function (data) {
            debugger;            
                document.getElementById("TotalPrice").innerHTML = 'Total ';
                $('#TotalPrice').text();
                var ex = data.length;
                ButtonDisable(ex);
                $(".length").text(ex + "(items)");
                $(".item").text("Shopping Cart (" + ex + " items)");
                CartCount(ex);
                document.getElementById("allOrders").innerHTML = ' ';
                total = 0;
                for (let i = 0; i < data.length; i++) {
                    var pricetotal = parseFloat(data[i].Unit_Price).toFixed(2);
                    var subTotal = parseFloat(data[i].Total_Price).toFixed(2);

                    total = parseFloat((+subTotal) + (+total)).toFixed(2);
                    $("#allItemTotalPrice").text("$" + total);
                    if (data[i].Quantity > 0) {
                        $('#allOrders').append('<div  id="' + data[i].Id + '" class="row no-gutters align-items-center px-1 border-bottom border-200"><div class="col-4 py-3 px-2 px-md-3"  > <div class="media align-items-center"><img class="rounded mr-3 d-none d-md-block" onclick=PreviewModal(' + data[i].QuotesItem_Id+') src="' + data[i].FrontImage + '" alt="" width="111" height="99" /> <div class="media-body" id="process"><h5 class="fs-0" id="' + data[i].Id + "process" + '"><strong style="font-weight:bold">Process : </strong>' + data[i].ProcessValue + '</h5><div id="color"><h5 class="fs-0 price" id="' + data[i].Id + "color" + '"><strong style="font-weight:bold">Color :</strong>' + data[i].ColorValue + '</h5></div><div id="Stitches"><h5 class="fs-0 price" id="' + data[i].Id + "Stitches" + '"><strong style="font-weight:bold">Stitches :</strong>' + data[i].StitchesValue + '</h5></div><div class="fs--2 fs-md--1"><a class="text-danger" href="#!" id="remove" onclick=RemoveCartItem(' + data[i].Id + ')>Remove</a></div></div></div></div><div class="col-4 p-3"><div class="row"> <div class="col-md-4 d-flex justify-content-end justify-content-md-center px-2 px-md-3 order-1 order-md-0">$' + parseFloat(data[i].LogoPrice).toFixed(2) + '</div> <div class="col-md-4 text-right pl-0 pr-2 pr-md-3 order-0 order-md-1 mb-2 mb-md-0 text-600" style="margin-left: -41px;">$' + data[i].Tshirt_Price + '</div> <div class="col-md-4 text-right pl-0 pr-2 pr-md-3 order-0 order-md-1 mb-2 mb-md-0 text-600">$' + pricetotal + '</div></div></div><div class="col-4 p-3"><div class="row"> <div class="col-md-8 d-flex justify-content-end justify-content-md-center px-2 px-md-3 order-1 order-md-0" id="' + data[i].Id + '"><div> <div class="input-group input-group-sm"><input id="' + data[i].Id + "quantity" + '" class="form-control text-center px-2 input-quantity input-spin-none" type="number" min="0" value="' + data[i].Quantity + '"  style="max-width: 65px" readonly/></div></div></div><div class="col-md-4 text-right pl-0 pr-2 pr-md-3 order-0 order-md-1 mb-2 mb-md-0 text-600" id="' + data[i].Id + "total" + '">$' + subTotal + '</div></div></div></div>')
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
    $(".item").text("Shopping Cart (" + lengthsubstring + " items)");
    $("#icon").text(lengthsubstring);
    $.ajax({
        method: 'POST',
        url: '/Design/UpdateCartItem',
        data: { Id:Id,quantity:quantity },
        success: function (data) {
            $("#" + Id).remove();
            MyCart();
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


function PreviewModal(id) {
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
/*            $("#pre").append('<img  width="340" src=' + data.BackImageSource + '>');
*/            $("#previewbtn").append('<button type = "button" class="btn btn-primary" onclick = "FrontPreview(' + id + ')" >Front</button >  <button type="button" onclick="BackPreview(' + id +')" class="btn btn-primary">Back</button>');
        }
    })
}

function PreviewModal(id) {
    debugger;
    document.getElementById('preview').innerHTML = '';
    document.getElementById('previewbtn').innerHTML = '';
    $("#PreviewModal").modal('show');
    $.ajax({
        url: '/Design/PreviewImage',
        type: 'GET',
        data: { id: id },
        async: false,
        success: function (data) {
            debugger;
            $("#preview").append('<img  width="450" height="350" src=' + data.FrontImageSource + '>');
            $("#previewbtn").append('<button type ="button" id="frontpreview" class="btn btn-primary" onclick = "FrontPreview(' + id + ')" disabled>Front</button >  <button type="button" id="backpreview" onclick="BackPreview(' + id + ')" class="btn btn-primary">Back</button>');
        }
    })
}

function BackPreview(id) {
    debugger;
    document.getElementById('preview').innerHTML = '';
    document.getElementById('backpreview').disabled = true;
    document.getElementById('frontpreview').disabled = false;
    $.ajax({
        url: '/Design/PreviewImage',
        type: 'GET',
        data: { id: id },
        async: false,
        success: function (data) {
            debugger;
            $("preview").text('');
            $("#preview").append('<img  width="450" height="350" src=' + data.BackImageSource + '>');
        }
    })
}

function FrontPreview(id) {
    debugger;
    document.getElementById('preview').innerHTML = '';
    document.getElementById('backpreview').disabled = false;
    document.getElementById('frontpreview').disabled = true;
    $.ajax({
        url: '/Design/PreviewImage',
        type: 'GET',
        data: { id: id },
        async: false,
        success: function (data) {
            debugger;
            $("preview").text('');
            $("#preview").append('<img  width="450" height="350" src=' + data.FrontImageSource + '>');
       }
    })
}