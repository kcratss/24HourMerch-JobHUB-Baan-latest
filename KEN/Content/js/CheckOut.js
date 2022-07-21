$(document).ready(function () {
    var total;
    Checkoutcart();
});
function Checkoutcart() {
    $.ajax({
        url: '/Design/MyCart',
        type: "GET",
        data: {},
        success: function (data) {
            var ex = data.length;
            CartCount(ex);
            var total = 0;
            debugger;
            for (let i = 0; i < data.length; i++) {
                var subTotal = parseFloat(data[i].Total_Price).toFixed(2);
                total = parseFloat((+subTotal) + (+total)).toFixed(2);
                debugger;
                if (data[i].Quantity > 0) {
                    $("#OrderSummary").append(' <tr class="border-bottom" ><th class="pl-0 pt-0">' + data[i].ProcessValue + '" x' + data[i].Quantity + '<div class="text-400 font-weight-normal fs--2"></div></th><th class="pr-0 text-right">$' + subTotal + '</th></tr>')
                }
                }
            var totalPrice =parseFloat((+total) + (+ 20.00)).toFixed(2);
            $("#subtotal").text("$" + total);
            $("#total").text("$" + totalPrice);
            $("#payabletotal").text("$" + totalPrice);
            $("#ConfirmPay").text("$" + totalPrice);
            CheckAddress();
        }
    });
}

function CheckAddress() {
    $.ajax({
        url: '/Design/AddAddress',
        type: "GET",
        data: {},
        success: function (data) {
            debugger;
            for (var i = 0; i < data.length; i++)
            {
                $("#address").append('<div class="col-md-6 mb-4 mb-md-4"> <div class="position-relative"><div class="custom-control custom-radio radio-select"><input class="custom-control-input" id="address-1" type="radio" onChange=OrderAddressSelect(' + data[i].tblAddress.AddressId + ') name="clientName">  <label class="custom-control-label font-weight-bold d-block" for="address-1"><span id="' + data[i].tblAddress.AddressId + 'name">' + data[i].tblAddress.TradingName + '</span><span class="radio-select-content" id="name"><span id="' + data[i].tblAddress.AddressId + 'address1">' + data[i].tblAddress.Address1 + '</span>,<br><span id = "' + data[i].tblAddress.AddressId + 'state">' + data[i].tblAddress.State + '</span><span class="d-block mb-0 pt-2" id="' + data[i].tblAddress.AddressId + 'postcode">' + data[i].tblAddress.Postcode + '</span></span></span></label></div></div></div>')
            }

        }

    });

}


function OrderDetail()
{
    debugger;
    var shipping = $('#ShippingCharge').text();  
    var shippingCharge = parseInt(shipping.substring(1));
    var total = $("#payabletotal").text();
    var TotalPrice = parseFloat(total.substring(1));
    $.ajax({
        url: '/Design/OrderDetail',
        type: "Post",
        data: { shippingCharge: shippingCharge, TotalPrice: TotalPrice },
        success: function (response) {
            debugger;
            ShowMessage(response);
            $("#order").text('');
            $("#icon").text('0');
           
        }
       
    })
    
   

}

function CartCount(ex) {
    $("#icon").text(ex);
    
}


var addressobj = {};
function OrderAddressSelect(id) {
    debugger;
    
        addressobj.name = $("#" + id + "name").text();
        addressobj.address = $("#" + id + "address1").text();
        addressobj.state = $("#" + id + "state").text();
        addressobj.postCode = $("#" + id + "postcode").text();
    
}


function OrderAddress()
{
    var response = {};
    debugger;
    if ( $.isEmptyObject(addressobj) == false) {
        $.ajax({
            url: '/Design/OrderAddress',
            type: "Post",
            data: { addressobj: addressobj },
            success: function () {
                OrderDetail();
            }
        })
    }
    else {
        response.IsSuccess = false;
        response.Message = "Address is required";
        ShowMessage(response);
    }
    
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

$(document).ready(function () {
    formValidation();

});
function formValidation() {
    $("#addressForm").validate({
        rules: {
            txtname: {
                required: true
            },
            txtattention: {
                required: true
            },
            txtaddress1: {
                required: true,

            },
            txtaddress2: {
                required: true,

            },
            states: {
                required: true,

            },
            txtpostcode: {
                required: true,
                digits: true,
            },
            addressnote: {
                required: true
            }

        },
        messages: {
            txtname: {
                required: "Please Enter Name",
            },
            txtattention: {
                required: "Please Enter Attention"
            },
            txtaddress1: {
                required: "Please Enter Address",

            },
            txtaddress2: {
                required: "Please Enter Address",

            },
            states: {
                required: "Please Enter State",

            },
            txtpostcode: {
                required: "Please Enter PostCode",
                digits: "Enter only number",
            },
            addressnote: {
                required: "Please Enter Address note"
            }
        },
        submitHandler: function () {
            debugger;
            AddUpdateAddress();
        }
    });
}
function AddAddress() {
    debugger;
    $('#addAddressModal').modal('show');
}

function AddUpdateAddress() {
    debugger;


    var addressData = {
        addressId: $("#txtaddressId").val(),
        name: $("#txtname").val(),
        attention: $("#txtattention").val(),
        addressLine1: $("#txtaddress1").val(),
        addressLine2: $("#txtaddress2").val(),
        addressNote: $("#addressnote").val(),
        postCode: $("#txtpostcode").val(),
        state: $('#txtstate').find(":selected").text()
    }

    $.ajax({
        url: '/Address/AddUpdateAddress',
        data: JSON.stringify(addressData),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        async: false,
        type: 'post',
        success: function (response) {
            ShowMessage(response);
            setTimeout(function () {
                
            }, 500)
            RedirectPage();
        },
        error: function (response) {

        }
    });

}

function RedirectPage() {
    window.location.href = '/Design/CheckOut';
}

function FormClear() {
    $('#addAddressModal form')[0].reset();
    $('#addAddressModal').modal('hide');
   
    
}