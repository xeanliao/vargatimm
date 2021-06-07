/// <reference path="jquery-1.3.2.js" />
/// <reference path="jquery-1.3.2-vsdoc2.js" />

$(document).ready(function () {
    $("#login-section").show();
    $("#login-progressbar_section").hide();
    $("#password").bind("keyup", function (e) {
        if (e.keyCode == 13 && $("#password").val().length > 0) {
            login();
        }
    });
    $("#username").bind("keyup", function (e) {
        if (e.keyCode == 13) {
            if ($("#password").val().length == 0) {
                $("#password").focus();
            } else {
                login();
            }
        }
    });
    $("#username").focus();
});

// log in to system
function login() {
    $("#login-section").hide();
    $("#login-progressbar_section").show();

    var data = [
		"username=" + $("#username").val(),
		"&password=" + $("#password").val()
        ].join(''),
        failed = function () {
            $('#error_info').removeClass('hidden');
            $("#login-section").show();
            $("#login-progressbar_section").hide();
        };


    $.ajax({
        type: "POST",
        url: "api/user/login",
        data: data,
        dataType: "JSON",
        success: function (msg) {
            var reuslt = JSON.parse(msg);
            if (reuslt && reuslt.success) {
                window.location = "newcenter/index.html";
                return;
            } else {

                failed();
            }
        },
        error: function (msg) {
            failed();
        }
    });
}

/**
* Called when the user clicks on the 'Log Out' menu item.
*/
function OnLogOutClick() {
    var data = "logout=true";
    $.ajax({
        type: "POST",
        url: "api/user/logout",
        data: data,
        success: function (msg) {
            window.location = "login.html";
        }
    });
}
