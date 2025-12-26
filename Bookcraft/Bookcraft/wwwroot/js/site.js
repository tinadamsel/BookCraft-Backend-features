
function Register() {
    debugger
    var defaultBtnValue = $('#submit_btn').html();
    $('#submit_btn').html("Please wait...");
    $('#submit_btn').attr("disabled", true);

    var data = {};
    data.FullName = $('#fullname').val();
    data.Email = $('#email').val();
    data.Password = $('#pwd').val();
    data.ConfirmPassword = $('#confirmpwd').val();

    debugger
    if (data.FullName == "" || data.FullName == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill in your full name");
        return;
    }
    if (data.Email == "" || data.Email == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill in your email");
        return;
    }
    if (data.Password == "" || data.Password == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill in your password");
        return;
    }
    if (data.ConfirmPassword == "" || data.ConfirmPassword == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please re-enter your password");
        return;
    }

    let userDetails = JSON.stringify(data);
    $.ajax({
        type: 'Post',
        url: '/Account/UserRegistration',
        dataType: 'json',
        data:
        {
            userDetails: userDetails,
        },
        success: function (result) {
            if (!result.isError) {
                debugger
                var url = '/Account/Login';
                successAlertWithRedirect(result.msg, url);
                $('#submit_btn').html(defaultBtnValue);
            }
            else {
                $('#submit_btn').html(defaultBtnValue);
                $('#submit_btn').attr("disabled", false);
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            $('#submit_btn').html(defaultBtnValue);
            $('#submit_btn').attr("disabled", false);
            errorAlert("Please check and try again. Contact Admin if issue persists..");
        },
    })

}

function login() {
    debugger
    var defaultBtnValue = $('#submit_btn').html();
    $('#submit_btn').html("Please wait...");
    $('#submit_btn').attr("disabled", true);

    var email = $('#email').val();
    var password = $('#pwd').val();
    $.ajax({
        type: 'Post',
        url: '/Account/Login',
        dataType: 'json',
        data:
        {
            email: email,
            password: password
        },
        success: function (result) {
            if (!result.isError) {
                debugger
                var n = 1;
                localStorage.removeItem("on_load_counter");
                localStorage.setItem("on_load_counter", n);
                location.replace(result.dashboard);
                return;
            }
            else {
                $('#submit_btn').html(defaultBtnValue);
                $('#submit_btn').attr("disabled", false);
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            $('#submit_btn').html(defaultBtnValue);
            $('#submit_btn').attr("disabled", false);
            errorAlert("An error occured, please try again.");
        }
    });
}

function PasswordReset() {
    debugger
    var defaultBtnValue = $('#submit_btn').html();
    $('#submit_btn').html("Please wait...");
    $('#submit_btn').attr("disabled", true);

    var email = $('#email').val();
    if (email == "" || email == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill in your email");
        return;
    }
   
    $.ajax({
        type: 'Post',
        url: '/Account/PasswordForgot',
        dataType: 'json',
        data:
        {
            email: email,
        }, 
        success: function (result) {
            if (!result.isError) {
                debugger
                var url = '/Account/ForgotPassword';
                successAlertWithRedirect(result.msg, url);
                $('#submit_btn').html(defaultBtnValue);
            }
            else {
                $('#submit_btn').html(defaultBtnValue);
                $('#submit_btn').attr("disabled", false);
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            $('#submit_btn').html(defaultBtnValue);
            $('#submit_btn').attr("disabled", false);
            errorAlert("Please check and try again. Contact Admin if issue persists..");
        },
    })

}

function Reset() {
    debugger
    var defaultBtnValue = $('#submit_btn').html();
    $('#submit_btn').html("Please wait...");
    $('#submit_btn').attr("disabled", true);

    var userId = $('#userId').val();
    var password = $('#newpwd').val();
    var confirmPassword = $('#confirmnewpwd').val(); 

    if (password == "" || password == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill in your new password");
        return;
    }

    if (confirmPassword == "" || confirmPassword == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please re-enter your new password");
        return;
    }

    $.ajax({
        type: 'Post',
        url: '/Account/Reset',
        dataType: 'json',
        data:
        {
            userId: userId,
            password: password,
            confirmPassword: confirmPassword,
        },
        success: function (result) {
            if (!result.isError) {
                debugger
                var url = '/Account/Login';
                successAlertWithRedirect(result.msg, url);
                $('#submit_btn').html(defaultBtnValue);
            }
            else {
                $('#submit_btn').html(defaultBtnValue);
                $('#submit_btn').attr("disabled", false);
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            $('#submit_btn').html(defaultBtnValue);
            $('#submit_btn').attr("disabled", false);
            errorAlert("Please check and try again. Contact Admin if issue persists..");
        },
    })

}