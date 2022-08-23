$(document).ready(function () {

    formValidation();
    MyCart();
});

function formValidation() {
    debugger
    
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
            
            AddUpdateAddress();
        }
    });
}


function closeModal() {
    debugger;
   
    /*$('.modal-backdrop').remove();
    $('.modal-body').removeClass('modal-open');*/
    $('#updateAddressModal').modal('hide');
    $("#quotesModal").modal('hide');

}

function UpdateAddress(id) {
    debugger;
    alert(id);
    if ($("#txtAddressId").val() !== "" && $("#txtaddress").val() !== "" && $("#txtstate").val() !== "" && $("#txtpostcode").val() !== "") {
        var addressData = {
            AddressId: $("#txtAddressId").val(),
            Address: $("#txtaddress").val(),
            PostCode: $("#txtpostcode").val(),
            State: $("#txtstate").val()

        }

        $.ajax({
            url: '/Client/ClientUpdateAddress',
            data: { model: addressData },
            async: false,
            type: 'post',
            success: function (response) {
                debugger
                ShowMessage(response);


            },
            error: function (response) {
                ShowMessage(response);

            },


        });

    }

}



function AddUpdateAddress() {
    debugger;
    if ($("#addressForm").valid()) {
       
            var addressData = {
                addressId: $("#txtaddressId").val(),
                name: $("#txtname").val(),
               /* attention: $("#txtattention").val(),*/
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
                        window.location.href = '/Address/AddressList';
                    }, 1000)                    
                },
                error: function (response) {
                   
                }
            });

        }

}

function RedirectPage() {
    window.location.href = '/Address/AddressList';
}

function GetAddressById(addressId) {
    debugger;
    $.ajax({
        url: '/Address/GetAddressById?addressId=' + addressId,
       
        async: false,
        type: 'get',
        success: function (response) {
            
            $("#updateAddressModal .modal-body").html(response);
            $('.modal-backdrop').show();
            $("#updateAddressModal").modal("show");
            formValidation();

        },
        error: function (response) {
            ShowMessage(response);
        }
    });
}

function DeleteAddress(addressId) {

    confirmDialog("Do you really want to delete this Address", function () {
        $.ajax({
            url: '/Address/DeleteAddress',
            data: { addressId: addressId },

            async: false,
            type: 'post',
            success: function (response) {
                ShowMessage(response);
                setTimeout(function () {
                    RedirectPage();
                }, 500)

            },
            error: function (response) {
                ShowMessage(response);
            }
        });
    });  
}

function confirmDialog(message, onConfirm) {
    var fClose = function () {
        modal.modal("hide");
    };
    var modal = $("#confirmModal");
    modal.modal("show");
    $("#confirmMessage").empty().append(message);
    $("#confirmOk").unbind().one('click', onConfirm).one('click', fClose);
    $("#confirmCancel").unbind().one("click", fClose);
    
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
        "timeOut": "7000",
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

function clearForm() {
    debugger
    $('#addAddressModal form')[0].reset();
    var validator = $("#addAddressModal").validate();
    $("#txtstate").css("color", "black");
    validator.resetForm();
}

function CartCount(ex) {
    $("#icon").text(ex);
}

function MyCart() {

    $.ajax({
        url: '/Design/MyCart',
        type: "GET",
        data: {},
        success: function (data) {
            debugger;
            var ex = data.length;
            CartCount(ex);
        }
    });
}