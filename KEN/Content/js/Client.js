$(document).ready(function () {
    var notificationtTime;
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
   
    Notification();
});


function timeSince(date) {
    debugger;

    var seconds = Math.floor((new Date() - date) / 1000);

    var interval = seconds / 31536000;

    if (interval > 1) {
        notificationtTime = Math.floor(interval) + " years ago";
        return true;
    }
    interval = seconds / 2628288;
    if (interval > 1) {
        notificationtTime = Math.floor(interval) + " months ago";
        return true;
    }
    interval = seconds / 604800;
    if (interval > 1) {
        notificationtTime = Math.floor(interval) + " Week ago";
        return true;
    }
    interval = seconds / 86400;
    if (interval > 1) {
        notificationtTime = Math.floor(interval) + " days ago";
        return true;
    }
    interval = seconds / 3600;
    if (interval > 1) {
        notificationtTime = Math.floor(interval) + " hours ago";
        return true;
    }
    interval = seconds / 60;
    if (interval > 1) {
        notificationtTime = Math.floor(interval) + " minutes ago";
        return true;
    }
}

function Notification() {
    debugger;
    $.ajax({
        url: '/Design/Notification',
        type: 'GET',
        data: {},
        async: false,
        success: function (data) {
            debugger;
            var a = 0;
            $('#notific').text('');
            $('#icons').text('');
            for (let i = 0; i < data.length; i++) {
                var dateString = data[i].CreatedOn;
                var num = parseInt(dateString.replace(/[^0-9]/g, ""));
                var date1 = new Date(num);
                timeSince(date1);
                if (data[i].Status == true) {
                    if (data[i].Quotes_Id == null) {
                        $('#notific').append('<div class="list-group-item" onclick=LogoDetails(' + data[i].Logo_Id + ')><a class="notification notification-flush notification-read" href="#!"><div class="notification-avatar"><div class="avatar avatar-xl me-3"><i class="fas fa-info-circle" style="color:blue;font-size:38px;"></i></div></div><div class="notification-body"><p class="mb-1"><strong>' + data[i].Message + '</strong></p><span class="notification-time"><span class="me-2" role="img" aria-label="Emoji">💬</span>' + notificationtTime + '</span></div></a></div>');
                    }
                    else {
                        $('#notific').append('<div class="list-group-item" onclick=ModalQuotes(' + data[i].Quotes_Id + '),NotificationStatus(' + data[i].Id + ');><a class="notification notification-flush notification-read" href="#!"><div class="notification-avatar"><div class="avatar avatar-xl me-3"><i class="fas fa-info-circle" style="color:blue;font-size:38px;"></i></div></div><div class="notification-body"><p class="mb-1"><strong>' + data[i].Message + '</strong></p><span class="notification-time"><span class="me-2" role="img" aria-label="Emoji">💬</span>' + notificationtTime + '</span></div></a></div>');

                    }
                }
                else {
                    $('#icons').text('');
                    a++;
                    var datalength = parseInt(a);
                    $('#icons').append(datalength);
                    if (data[i].Quotes_Id == null) {
                        $('#notific').append('<div class="list-group-item" onclick=LogoDetails(' + data[i].Logo_Id + '),NotificationStatus(' + data[i].Id + ');><a class="notification notification-flush notification-unread" href="#!"><div class="notification-avatar"><div class="avatar avatar-xl me-3"><i class="fas fa-info-circle" style="color:blue;font-size:38px;"></i></div></div><div class="notification-body"><p class="mb-1"><strong>' + data[i].Message + '</strong></p><span class="notification-time"><span class="me-2" role="img" aria-label="Emoji">💬</span>' + notificationtTime + '</span></div></a></div>');
                    }
                    else {
                        $('#notific').append('<div class="list-group-item" onclick=ModalQuotes(' + data[i].Quotes_Id + '),NotificationStatus(' + data[i].Id + ');><a class="notification notification-flush notification-unread" href="#!"><div class="notification-avatar"><div class="avatar avatar-xl me-3"><i class="fas fa-info-circle" style="color:blue;font-size:38px;"></i></div></div><div class="notification-body"><p class="mb-1"><strong>' + data[i].Message + '</strong></p><span class="notification-time"><span class="me-2" role="img" aria-label="Emoji">💬</span>' + notificationtTime + '</span></div></a></div>');

                    }
                }
            }
        }
    })
}

function NotificationStatus(id) {
    debugger;
    $.ajax({
        url: '/Design/NotificationStatus',
        type: 'Get',
        data: { id, id },
        async: false,
        success: function (data) {
            Notification();
        }
    })

}

function ReadAllNotification() {
    debugger;
    $.ajax({
        url: '/Design/ReadAllNotification',
        type: 'Get',
        data: { },
        async: false,
        success: function (data) {
            Notification();
        }
    })

}
function LogoDetails(id) {
    $('#logoProces').text('');
    $('#logosize').text('');
    $('#logocolor').text('');
    $('#imglogo').text('');
    $('#logoModal').modal('show');
   /* document.getElementById('setdefault').disabled = false;*/
    $.ajax({
        url: '/Design/LogoDetails',
        type: 'Get',
        data: { id: id },
        async: false,
        success: function (dataProcess) {
            $('#setDefault').text('');
            for (var b = 0; b < dataProcess.length; b++) {
                if (dataProcess[b].Status == true) {
                    $('#setDefault').append('<option  id=' + dataProcess[b].LogoId + ' selected value=' + dataProcess[b].Id + '>' + dataProcess[b].Name + '</option>');
                }
                else {
                    $('#setDefault').append('<option  id=' + dataProcess[b].LogoId + ' value=' + dataProcess[b].Id + '>' + dataProcess[b].Name + '</option>');
                }
            }
            $.ajax({
                url: '/Design/ProcessList',
                type: 'Post',
                data: { Process_Id: dataProcess.Process_Id },
                async: false,
                success: function (data) {
                    debugger;
                    for (let i = 0; i < data.length; i++) {
                        for (var j = 0; j < dataProcess.length; j++) {
                            if (data[i].ProcessId == dataProcess[j].Process_Id && dataProcess[j].Status == true) {
                                var processid = dataProcess[j].Process_Id;
                                $('#logoProces').append('<option  selected value=' + data[i].ProcessId + '>' + data[i].Name + '</option>');

                                $.ajax({
                                    url: '/Design/ColorList',
                                    type: 'Get',
                                    data: { processid: processid },
                                    async: false,
                                    success: function (Color) {
                                        debugger;
                                        for (var k = 0; k < Color.length; k++) {
                                            if (dataProcess[j].Color_Id == Color[k].ColorId) {
                                                $('#logocolor').append('<option  selected value=' + Color[k].ColorId + '>' + Color[k].Name + '</option>');
                                            }
                                            else {
                                                $('#logocolor').append('<option  value=' + Color[k].ColorId + '>' + Color[k].Name + '</option>');
                                            }
                                        }
                                    }
                                })
                                $.ajax({
                                    url: '/Design/SizeList',
                                    type: 'Get',
                                    data: { processid: processid },
                                    async: false,
                                    success: function (size) {
                                        debugger;
                                        for (var l = 0; l < size.length; l++) {
                                            if (dataProcess[j].Size_Id == size[l].SizeId) {
                                                $('#logosize').append('<option  selected value=' + size[l].SizeId + '>' + size[l].Name + '</option>');
                                            }
                                            else {
                                                $('#logosize').append('<option  value=' + size[l].SizeId + '>' + size[l].Name + '</option>');
                                            }
                                        }
                                        $('#imglogo').append('<img  id ="logoimg" src=' + dataProcess[j].LogoUrl + ' />');
                                    }
                                })
                                $('#name').val(dataProcess[j].Name);
                                $('#createdby').val(dataProcess[j].UserName);
                                $('#length').val(dataProcess[j].Height);
                                $('#width').val(dataProcess[j].Width);
                                document.getElementById('createdby').disabled = true;
                                document.getElementById('length').disabled = true;
                                document.getElementById('width').disabled = true;
                                document.getElementById('logosize').disabled = true;
                                document.getElementById('logocolor').disabled = true;
                                document.getElementById('save').disabled = true;
                                document.getElementById('logoProces').disabled = true;
                                $('#logosize').css('background-color', '#efefef');
                                $('#logocolor').css('background-color', '#efefef');
                            }
                            else {
                                $('#logoProces').append('<option  value=' + data[i].ProcessId + '>' + data[i].Name + '</option>');

                            }
                        }
                    }
                }
            })
        }
    })
}

function ClientQuotesList(id) {
    debugger;
    $.ajax({
        url: '/Design/QuoteList',
        type: 'Post',
        data: { id: id },
        async: false,
        success: function (data) {
            debugger;           
            $('#exampleModal').text('Quote Detail?' + id + '');
            price = 0;
            if (data.length > 0) {
                for (let i = 0; i < data.length; i++) {
                    var length = data.length;
                    $("#code1").text(length);
                    var printPrice = parseFloat(data[i].Print_Price).toFixed(2);
                    var pricetotal = parseFloat(data[i].Unit_Price).toFixed(2);
                    var subTotal = parseFloat(data[i].TotalPrice).toFixed(2);

                    price = parseFloat((+subTotal) + (+price)).toFixed(2);
                    $("#listdetail").append('<tr><td><img  width="139px" src =' + data[i].FrontImageSource + ' ><td>' + data[i].ProcessValue + '</td><td>' + data[i].Quantity + '</td><td>' + data[i].ColorValue + '</td><td>' + data[i].SizeValue + '</td><td>' + data[i].Size + '</td><td>$' + printPrice + '</td><td>$' + data[i].Tshirt_Price + '</td><td>$' + pricetotal + '</td><td>$' + subTotal + '</td></tr>');
                    $("#price").text("$" + price);
                }
            }
            else {
                $("#list1").append('<tr class="text-center"><td colspan="8">No Quotes Found.</td></tr>');
            }
        }
    })
}

function ModalQuotes(id) {
    debugger;
    document.getElementById('listdetail').innerHTML = '';
    $("#quotesModal").modal('show');
    ClientQuotesList(id);
}


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

function ViewAllNotification() {
    $.ajax({
        url: '/Design/ViewAllNotification',
        type: 'Get',
        data: {},
        async: false,
        success: function (data) {
        }
    })
}

function closeModal() {
    $("#quotesModal").modal('hide');
    $('#logoModal').modal('hide');
    $('#updateAddressModal').modal('hide');
}







