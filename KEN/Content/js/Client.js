$(document).ready(function () {

    $.validator.addMethod("regx", function (value, element, regexpr) {
        return this.optional(element) || /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/.test(value);

    }, "Correct a valid email.");



    $("#registerform").validate({
        rules: {
            txtfirstnm: {
                required: true,
                minlength: 2
            },
            txtlastnm: {
                required: true,
                minlength: 2
            },
            txtusername: {
                required: true,
                minlength: 2
            },
            txtemail: {
                required: true,
                regx: true

            },

            txtpassword: {
                required: true,
                minlength: 6

            },
            txtconfirmpassword: {
                required: true,
                minlength: 6,
                equalTo: "#txtpassword"
            },
            termsregister: "required"
        },
        messages: {
            txtfirstnm: {
                required: "Please Enter First Name",
                minlength: "Name must 2 Character"
            },
            txtlastnm: {
                required: "Please Enter Last Name",
                minlength: "Name must 2 Character"
            },
            txtusername: {
                required: "Please Enter User Name",
                minlength: "Username must 2 character"
            },
            txtemail: {
                required: "Please Enter email"

            },
            txtpassword: {
                required: "Please Enter Password",
                minlength: "Password must 6 character"

            },
            txtconfirmpassword: {
                required: "Please re-enter Password",
                minlength: "Password must 6 character"

            },
            termsregister: "Please select terms & condition"

        },
        submitHandler: function (form) {
            Register();

        }

    });

    $("#loginForm").validate({
        rules: {

            Email: {
                required: true,
                regx: true

            },

            Password: {
                required: true
            }

        },
        messages: {

            Email: {
                required: "Please Enter email"

            },
            Password: {
                required: "Please Enter Password",


            }

        },
        submitHandler: function (form) {
            form.submit();
        }

    });

    $("#resetForm").validate({
        rules: {

            Password: {
                required: true,
                minlength: 5

            },
            ConfirmPassword: {
                required: true,
                minlength: 6,
                equalTo: "#txtPassword"
            }

        },
        messages: {

            Password: {
                required: "Please Enter Password",
                minlength: "Password must 6 character"

            },
            ConfirmPassword: {
                required: "Please re-enter Password",
                minlength: "Password must 6 character"

            }
        },
        submitHandler: function (form) {
            form.submit();
        }

    });

    $("#forgotForm").validate({
        rules: {
            Email: {
                required: true,
                regx: true
            }
        },
        messages: {

            Email: {
                required: "Please Enter email"
            }
        },
        submitHandler: function (form) {
            form.submit();
        }
    });

    $("#contactForm").validate({
        rules: {
            FirstName: {
                required: true,
                minlength: 2
            },
            LastName: {
                required: true,
                minlength: 2
            },
            Email: {
                required: true,
                regx: true
            },
            Contact: {
                required: true,
                digits: true,
                minlength: 10


            }
        },
        messages: {
            FirstName: {
                required: "Please Enter First Name",
                minlength: "Name must 2 Character"
            },
            LastName: {
                required: "Please Enter Last Name",
                minlength: "Name must 2 Character"
            },
            Email: {
                required: "Please Enter email"
            },
            Contact: {
                required: "Please Enter Contact",
                digits: "Enter only number",
                minlength: "Contact contains only 10 number"

            }
        },
        submitHandler: function () {
            //form.submit();

            UpdateProfile();
        }
    });
  
   

});

function UpdateProfile() {
    debugger
    if ($("#txtfirstnm").val() !== "" && $("#txtlastnm").val() !== "" && $("#txtusername").val() !== "" && $("#txtemail").val() !== "" && $("#txtcontact").val() !== "") {
        var contactData = {
            Firstname: $("#txtfirstnm").val(),
            Lastname: $("#txtlastnm").val(),

            Email: $("#txtemail").val(),
            Contact: $("#txtcontact").val()

        }
        $.ajax({
            url: '/Contact/EditProfile',
            data: contactData,
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

function ShowMessage(response) {    
    toastr.options =
    {
        "closeButton": true,
        "debug": false,
        "positionClass": "toaster-top-width",
        "progressBar": true,
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


function Register() {

    debugger;
    if ($("#txtfirstnm").val() !== "" && $("#txtlastnm").val() !== "" && $("#txtusername").val() !== "" && $("#txtemail").val() !== "" && $("#txtpassword").val() !== "" && $("#txtconfirmpassword").val() !== "" && $("#termsregister").val() !== "") {
        var registerData = {
            Firstname: $("#txtfirstnm").val(),
            Lastname: $("#txtlastnm").val(),
            UserName: $("#txtusername").val(),
            Email: $("#txtemail").val(),
            Password: $("#txtpassword").val(),
            ConfirmPassword: $("#txtconfirmpassword").val(),
            TermsAndPolicy: $('input#termsregister').prop('checked')
        }
        $.ajax({
            url: '/Client/Register',
            data: registerData,
            async: false,
            type: 'post',
            success: function (response) {
                debugger

                $("#registerform")[0].reset();
                ShowMessage(response);


            },
            error: function (response) {
                ShowMessage(response);

            },


        });

    }


}

function CheckCookies() {

    var CheckValue = $('#chkRem').is(":checked");
    if ($('#UserEmail').val() != null && $('#UserEmail').val() != "" && $('#UserEmail').val() != undefined) {
        $.ajax({
            url: '/Client/SetValueFromCookies',
            data: { EmailId: $('#UserEmail').val(), Password: $('#txtpswrd').val(), RememberMe: CheckValue },
            async: false,
            success: function (response) {
                if (response != null && response != undefined && response != "") {
                    $('#txtpswrd').val(response);
                    $('#chkRem').prop('checked', true);
                }
            },
            type: 'get'
        })
    }
}







