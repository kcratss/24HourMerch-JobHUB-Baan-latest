$(function () {
    $("#btnforgotpass").click(function () {
        var loginform = document.getElementById("loginblock");
        var fpform = document.getElementById("ForgotPasswordForm");
        loginform.style.display = "none";
        fpform.style.display = "block";
    });

    $("#btnfpsubmit").click(function () {
        $.ajax({
            url: '/User/ForgotPasswordSendMail/',
            data: { EmailId: $('#txtregemail').val() },
            async: false,
            success: function (response) {
                CustomAlert(response);
                setTimeout(redirectToLogin, 4000);
            },
            type: 'get'
        })
    });

    $('#btnLogin').click(function () {

    });
    $("#btnforgotpassback").click(function () {
        //var loginform = document.getElementById("loginblock");
        //var fpform = document.getElementById("ForgotPasswordForm");
        $("#loginblock").css("display", "block");
        $("#ForgotPasswordForm").css("display", "none");

        //loginform.style.display = "block";
        //fpform.style.display = "none";
    });
   
    
    
    
});
function ConfirmPassword()
{
    if ($('#txtnewsignuppassword').val() != $('#txtConfirmsignuppassword').val()) {
        
        $('#CustomAlert').removeClass('j-alertBox-Success').css('display', 'none');
        $('#CustomAlert').addClass('j-alertBox-Failed').css('display', 'block');
        $('#alertMsg').html('<strong>Error !</strong> Confirm password did not match');
        setTimeout(function () {
            $('#CustomAlert').fadeOut('slow', function () {
                $('#CustomAlert').removeClass('j-alertBox-Failed').css('display', 'none');
                $('#alertMsg').html('');

            });
        }, 5000);
        return false;
    }
    else
    {
        return true;
    }
}

function btnforgetpassword() {
    if ($('#txtregemail').val() != null || $('#txtregemail').val() != "" || $('#txtregemail').val() != 'undefined') {
        $.ajax({
            url: '/User/forgotpasswordmail',
            data: { EmailId: $('#txtregemail').val() },
            async: false,
            success: function (response) {
                location.href = "/User/Login";
            },
            type:'post'
        })
    }
}
// baans change 28th June for Forgot Password
function senddefaultpassword() {
    $.ajax({
        url: 'User/ForgotPasswordSendMail',
        data: { EmailId: $('#txtregemail').val() },
        async: false,
        success: function (response) {
            CustomAlert(response);
            setTimeout(redirectToLogin, 4000);
        },
        type: 'get'
    })
}
function redirectToLogin() {
    location.href = "/User/Login";
}

function moveToLogin() {
    location.href = "/User/Login";
}

// baans end 28th June

// baans change 4th July for Remember Me
function CheckCookies() {

    var CheckValue = $('#chkRem').is(":checked");
    if ($('#UserEmail').val() != null && $('#UserEmail').val() != "" && $('#UserEmail').val() != undefined) {
        $.ajax({
            url: '/User/SetValueFromCookies',
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
// baans end 4th July