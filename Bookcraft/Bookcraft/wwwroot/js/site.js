
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

//Genres
function addGenre() {
    var defaultBtnValue = $('#submit_btn').html();
    $('#submit_btn').html("Please wait...");
    $('#submit_btn').attr("disabled", true);

    var name = $('#genre_Name').val();

    if (name == "" || name == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please add the genre name");
        return;
    }
    
    $.ajax({
        type: 'Post',
        url: '/Admin/CreateGnere',
        dataType: 'json',
        data:
        {
            name: name,
        },
        success: function (result) {

            if (!result.isError) {
                var url = '/Admin/BookGenre';
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
        }
    });

}

function genreToBeEdited(id) {

    $.ajax({
        type: 'Get',
        dataType: 'Json',
        url: '/Admin/EditGenre',
        data: {
            id: id
        },
        success: function (result) {

            if (!result.isError) {
                $('#genre_id').val(result.id);
                $('#edit_genre_Name').val(result.name);
                $('#edit_genre').modal('show');
            }
            else {
                errorAlert(result.msg)
            }
        },
        error: function (ex) {
            errorAlert("An error occured, please check and try again. Please contact admin if issue persists..");
        }
    })
}

function SaveEditedGenre() {

    var defaultBtnValue = $('#submit_Btn').html();
    $('#submit_Btn').html("Please wait...");
    $('#submit_Btn').attr("disabled", true);

    var data = {};
    var id = $("#genre_id").val();
    var name = $('#edit_genre_Name').val();
   
    $.ajax({
        type: 'POST',
        url: '/Admin/EditedGenre',
        dataType: 'json',
        data:
        {
            id: id,
            name: name,
        },
        success: function (result) {
            if (!result.isError) {
                debugger
                var url = '/Admin/Genre'
                successAlertWithRedirect(result.msg, url)
                $('#submit_Btn').html(defaultBtnValue);
            }
            else {
                $('#submit_Btn').html(defaultBtnValue);
                $('#submit_Btn').attr("disabled", false);
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            $('#submit_Btn').html(defaultBtnValue);
            $('#submit_Btn').attr("disabled", false);
            errorAlert(result.msg);
        }
    });
    
}

function genreToDelete(id) {

    $('#genre_id').val(id);
    $('#delete_genre').modal('show');
}
function DeleteGenre() {
    var id = $('#genre_id').val();
    $.ajax({
        type: 'Post',
        dataType: 'Json',
        url: '/Admin/DeleteGenre',
        data: {
            id: id
        },
        success: function (result) {
            if (!result.isError) {
                var url = '/Admin/Genre'
                successAlertWithRedirect(result.msg, url)
                $('#submit_Btn').html(defaultBtnValue);
            }
            else {
                errorAlert(result.msg)
            }
        },
        error: function (ex) {
            errorAlert("An error occured, please check and try again. Please contact admin if issue persists..");
        }
    })
}


//Target Audience
function addAud() {
    var defaultBtnValue = $('#submit_btn').html();
    $('#submit_btn').html("Please wait...");
    $('#submit_btn').attr("disabled", true);

    var name = $('#aud_Name').val();

    if (name == "" || name == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please add the name");
        return;
    }

    $.ajax({
        type: 'Post',
        url: '/Admin/CreateTargetAudience',
        dataType: 'json',
        data:
        {
            name: name,
        },
        success: function (result) {

            if (!result.isError) {
                var url = '/Admin/Audience';
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
        }
    });

}

function audToBeEdited(id) {

    $.ajax({
        type: 'Get',
        dataType: 'Json',
        url: '/Admin/EditAudience',
        data: {
            id: id
        },
        success: function (result) {

            if (!result.isError) {
                $('#aud_id').val(result.id);
                $('#edit_Aud_Name').val(result.name);
                $('#edit_aud').modal('show');
            }
            else {
                errorAlert(result.msg)
            }
        },
        error: function (ex) {
            errorAlert("An error occured, please check and try again. Please contact admin if issue persists..");
        }
    })
}

function SaveEditedAudience() {

    var defaultBtnValue = $('#submit_Btn').html();
    $('#submit_Btn').html("Please wait...");
    $('#submit_Btn').attr("disabled", true);

    var data = {};
    var id = $("#aud_id").val();
    var name = $('#edit_Aud_Name').val();

    $.ajax({
        type: 'POST',
        url: '/Admin/EditedAudience',
        dataType: 'json',
        data:
        {
            id: id,
            name: name,
        },
        success: function (result) {
            if (!result.isError) {
                debugger
                var url = '/Admin/Audience'
                successAlertWithRedirect(result.msg, url)
                $('#submit_Btn').html(defaultBtnValue);
            }
            else {
                $('#submit_Btn').html(defaultBtnValue);
                $('#submit_Btn').attr("disabled", false);
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            $('#submit_Btn').html(defaultBtnValue);
            $('#submit_Btn').attr("disabled", false);
            errorAlert(result.msg);
        }
    });

}

function audToDelete(id) {

    $('#aud_id').val(id);
    $('#delete_aud').modal('show');
}
function DeleteAud() {
    var id = $('#aud_id').val();
    $.ajax({
        type: 'Post',
        dataType: 'Json',
        url: '/Admin/DeleteAudience',
        data: {
            id: id
        },
        success: function (result) {
            if (!result.isError) {
                var url = '/Admin/Audience'
                successAlertWithRedirect(result.msg, url)
                $('#submit_Btn').html(defaultBtnValue);
            }
            else {
                errorAlert(result.msg)
            }
        },
        error: function (ex) {
            errorAlert("An error occured, please check and try again. Please contact admin if issue persists..");
        }
    })
}


//Writing Styles
function addStyle() {
    var defaultBtnValue = $('#submit_btn').html();
    $('#submit_btn').html("Please wait...");
    $('#submit_btn').attr("disabled", true);

    var name = $('#Wstyle_Name').val();

    if (name == "" || name == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please add the style name");
        return;
    }

    $.ajax({
        type: 'Post',
        url: '/Admin/CreateStyle',
        dataType: 'json',
        data:
        {
            name: name,
        },
        success: function (result) {

            if (!result.isError) {
                var url = '/Admin/WritingStyles';
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
        }
    });

}

function StyleToBeEdited(id) {

    $.ajax({
        type: 'Get',
        dataType: 'Json',
        url: '/Admin/EditStyle',
        data: {
            id: id
        },
        success: function (result) {

            if (!result.isError) {
                $('#wStyle_id').val(result.id);
                $('#edit_wStyle_Name').val(result.name);
                $('#edit_wStyle').modal('show');
            }
            else {
                errorAlert(result.msg)
            }
        },
        error: function (ex) {
            errorAlert("An error occured, please check and try again. Please contact admin if issue persists..");
        }
    })
}

function SaveEditedStyle() {

    var defaultBtnValue = $('#submit_Btn').html();
    $('#submit_Btn').html("Please wait...");
    $('#submit_Btn').attr("disabled", true);

    var data = {};
    var id = $("#wStyle_id").val();
    var name = $('#edit_wStyle_Name').val();

    $.ajax({
        type: 'POST',
        url: '/Admin/EditedStyle',
        dataType: 'json',
        data:
        {
            id: id,
            name: name,
        },
        success: function (result) {
            if (!result.isError) {
                debugger
                var url = '/Admin/WritingStyle'
                successAlertWithRedirect(result.msg, url)
                $('#submit_Btn').html(defaultBtnValue);
            }
            else {
                $('#submit_Btn').html(defaultBtnValue);
                $('#submit_Btn').attr("disabled", false);
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            $('#submit_Btn').html(defaultBtnValue);
            $('#submit_Btn').attr("disabled", false);
            errorAlert(result.msg);
        }
    });

}

function StyleToDelete(id) {
    $('#wStyle_id').val(id);
    $('#delete_style').modal('show');
}
function DeleteStyle() {
    var id = $('#wStyle_id').val();
    $.ajax({
        type: 'Post',
        dataType: 'Json',
        url: '/Admin/DeleteStyle',
        data: {
            id: id
        },
        success: function (result) {
            if (!result.isError) {
                var url = '/Admin/WritingStyle'
                successAlertWithRedirect(result.msg, url)
                $('#submit_Btn').html(defaultBtnValue);
            }
            else {
                errorAlert(result.msg)
            }
        },
        error: function (ex) {
            errorAlert("An error occured, please check and try again. Please contact admin if issue persists..");
        }
    })
}