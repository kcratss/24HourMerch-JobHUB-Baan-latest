$(document).ready(function () {
    var total;
    Checkoutcart();
    CheckAddress();
});

function Checkoutcart() {
    $.ajax({
        url: '/Design/MyCart',
        type: "GET",
        data: {},
        success: function (data) {
            var ex = data.length;
            console.log(ex);

            var total = 0;
            debugger;
            for (let i = 0; i < data.length; i++) {
                 total = data[i].Total + total;
                $("#OrderSummary").append(' <tr class="border-bottom" ><th class="pl-0 pt-0">' + data[i].ProcessValue + '" x' + data[i].Quantity + '<div class="text-400 font-weight-normal fs--2"></div></th><th class="pr-0 text-right">$' + data[i].Total + '</th></tr>')
            }
            var totalPrice = total + 20;
            $("#subtotal").text("$" + total);
            $("#total").text("$" + totalPrice);
            $("#payabletotal").text("$" + totalPrice);
            $("#ConfirmPay").text("$" + totalPrice);
            
        }

    });

}

function CheckAddress() {
    $.ajax({
        url: '/Design/addAddress',
        type: "GET",
        data: {},
        success: function (data) {
            debugger;      
            console.log("hello");
         

            $("#address").append('<div class="row"> <div class="col-md-6 mb-3 mb-md-0"> <div class="custom-control custom-radio radio-select"> <input class="custom-control-input" id="address-1" type="radio" name="clientName" checked>  <label class="custom-control-label font-weight-bold d-block" for="address-1"><span id="name">' + data.tblAddress.TradingName + '</span><span class="radio-select-content" id="name"><span id="address1">' + data.tblAddress.Address1 + '</span>,<br><span id = "state">' + data.tblAddress.State + '</span><span class="d-block mb-0 pt-2" id="postcode">' + data.tblAddress.Postcode + '</span></span></span></label><a class="mt-2 fs--1" href="#!">Edit</a></div></div> <div class="col-md-6"> <div class="position-relative"> <div class="custom-control custom-radio radio-select"> <input class="custom-control-input" id="address-2" type="radio" name="clientName"> <label class="custom-control-label font-weight-bold d-block" for="address-2"><span id="name1">' + data.tblAddress.TradingName + '</span><span class="radio-select-content" id="address2">' + data.tblAddress.Address2 + ',<br><span id="stateadd2">' + data.tblAddress.State + '</span> <br><span class="d-block mb-0 pt-2" id="postcodeAdd2">' + data.tblAddress.Postcode + '</span></span></span></label><a class="mt-2 fs--1" href="#!">Edit</a>  </div> </div> </div>')
            

        }

    });

}


function OrderDetail()
{
    $.ajax({
        url: '/Design/OrderDetail',
        type: "Get",
        data: {},
        success: function (data) {
            
        }
       
    })
    
    OrderAddress();

}

function OrderAddress() {
    if (document.getElementById('address-1').checked) {
        var name = $("#name").text();
        var address = $("#address1").text();
        var state = $("#state").text();
        var postCode = $("#postcode").text();      
    }
    else {
        var name = $("#name1").text();
        var address = $("#address2").text();
        var state = $("#stateadd2").text();
        var postCode = $("#postcodeAdd2").text();
        
    }
    ajax({
        url: 'Design/OrderAddress',
        type: "Post",
        data: { name: name, address: address, state: state, postCode: postCode },
        success: function () {

        }
    })
}