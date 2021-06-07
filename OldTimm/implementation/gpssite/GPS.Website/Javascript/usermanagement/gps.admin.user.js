var SysBrowser = {};
SysBrowser.ie;
SysBrowser.firefox;
SysBrowser.chrome;
SysBrowser.opera;
SysBrowser.safari;
var ua = navigator.userAgent.toLowerCase();
var s;
(s = ua.match(/msie ([\d.]+)/)) ? SysBrowser.ie = s[1] :
        (s = ua.match(/firefox\/([\d.]+)/)) ? SysBrowser.firefox = s[1] :
        (s = ua.match(/chrome\/([\d.]+)/)) ? SysBrowser.chrome = s[1] :
        (s = ua.match(/opera.([\d.]+)/)) ? SysBrowser.opera = s[1] :
        (s = ua.match(/version\/([\d.]+).*safari/)) ? SysBrowser.safari = s[1] : 0;
//]]>
var currentRole;

//<!CDATA[
var testTable, aUsers = [], actionNode;

function onGetAllUsers() {
    
    var str = "<table id=\"test_table\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" class=\"display\"></table>";    
    $('#tableDiv').html(str);
    var userReader = new TIMM.Website.UserServices.UserReaderService();
    if ($('#all-group-select').val()) {
        userReader.GetAllUsersByGroup($('#all-group-select').val(), onGetAllUsersSuccess, null, null);
    }
    else {
        userReader.GetAllUsersByGroup(0, onGetAllUsersSuccess, null, null);
    }
}

function onAddUser() {
    $('#list-group-name-new').val(null);
    $('#user_dialog').dialog('open');
}

function fnRemoveSelected(tableLocal) {
    $(tableLocal.fnSettings().aoData).each(function() {
        $(this.nTr).removeClass('row_selected');
    });
}

function fnSelectRowByActionNode(tableLocal, actionNodeLocal) {
    fnRemoveSelected(tableLocal);
    $(actionNodeLocal.parentNode.parentNode).addClass('row_selected');
}

function fnSelectRowWithDelay(actionNodeLocal) {
    actionNode = actionNodeLocal;
    setTimeout('fnSelectRowByActionNode(testTable, actionNode)', 20);
}

function fnGetSelected(tableLocal) {
    var aReturn = new Array();
    var aTrs = tableLocal.fnGetNodes();

    for (var i = 0; i < aTrs.length; i++) {
        if ($(aTrs[i]).hasClass('row_selected')) {
            aReturn.push(aTrs[i]);
        }
    }
    return aReturn;
}

function onEditUser(obj, userName) {
    // Make the row selected
    fnSelectRowWithDelay(obj);

    var userReader = new TIMM.Website.UserServices.UserReaderService();
    var groupReader = new TIMM.Website.GroupServices.GroupReaderService();
    userReader.GetUser(userName, function(r) {
        // Assign dialog field values and show the dialog
        $('#edit_userid').val(r.Id);
        $('#edit_username').val(r.UserName);
        $('#edit_fullname').val(r.FullName);
        $('#edit_usercode').val(r.UserCode);
        $('#edit_password').val(r.Password);
        $('#edit_confirm_password').val(r.Password);
        $('#edit_email').val(r.Email == null ? '' : r.Email);
        //$('#edit_is_admin').attr('checked', r.Master);
        $('#edit_enabled').attr('checked', r.Enabled);
        currentRole = r.Role;
        groupReader.GetAllGroups(function(grouplist) {
            $('#list-group-name').empty();
            var uglist = [];
            if (r.Groups != null)
                for (var j = 0; j < r.Groups.length; j++)
                uglist[j] = r.Groups[j].Name;
            for (var i in grouplist) {

                $('#list-group-name').append("<option value='" + grouplist[i].Id + "'>" + grouplist[i].Name + "</option>");
            }
            $('#list-group-name').val(uglist);
        }, null, null);
        $('#edit_user_dialog').dialog('open');
    }, null, null);
//    userReader.UserEnumToList(onGetRoleListwithValue, null, null);
}


function onAssignUser(obj, userName) {
    // Make the row selected
    fnSelectRowWithDelay(obj);

    var userReader = new TIMM.Website.UserServices.UserReaderService();
    userReader.GetUser(userName, function(r) {
        // Assign dialog field values and show the dialog
        $('#edit_userid').val(r.Id);
        $('#edit_username').val(r.UserName);
        $('#edit_fullname').val(r.FullName);
        $('#edit_usercode').val(r.UserCode);
        $('#edit_password').val(r.Password);
        $('#edit_confirm_password').val(r.Password);
        //$('#edit_is_admin').attr('checked', r.Master);
        $('#edit_email').val(r.Email == null ? '' : r.Email);
        $('#edit_enabled').attr('checked', r.Enabled);
        currentRole = r.Role;
        $('#edit_user_dialog').dialog('open');
    }, null, null);

//    userReader.UserEnumToList(onGetRoleListwithValue, null, null);
}

function onGetRoleListwithValue(r) {
    $("#edit_userrole").empty();
    //            $("<option value='" + "0" + "'>" + "Please Select User" + "</option>").appendTo("#edit_userrole");
    for (var i = 0; i < r.length; i++) {
        if (currentRole == r[i].RoleValue) {

            $("<option value='" + r[i].RoleValue + "' selected>" + r[i].RoleName + "</option>").appendTo("#edit_userrole");
        }
        else {
            $("<option value='" + r[i].RoleValue + "'>" + r[i].RoleName + "</option>").appendTo("#edit_userrole");
        }
    }
}

function onGetRoleList(r) {
    $("#userrole").empty();
    //            $("<option value='" + "0" + "' selected>" + "Please Select User" + "</option>").appendTo("#userrole");
    for (var i = 0; i < r.length; i++) {
        $("<option value='" + r[i].RoleValue + "'>" + r[i].RoleName + "</option>").appendTo("#userrole");
    }
}

function onGetGroupList(r) {
    $('#list-group-name-new').empty();
    $('#all-group-select').empty();
    $('#all-group-select').append("<option value='0'>----------- All ------------</option>");
    for (var i = 0; i < r.length; i++) {
        $('#list-group-name-new').append("<option value='" + r[i].Id + "'>" + r[i].Name + "</option>");
        $('#all-group-select').append("<option value='" + r[i].Id + "'>" + r[i].Name + "</option>");
    }

   
}
function onDeleteUser(obj, userName) {
    // Make the row selected
    fnSelectRowWithDelay(obj);

    // Show confirmation dialog
    $("#delete_confirm_dialog").dialog({
        autoOpen: false,
        modal: true,
        buttons: {
            'Yes': function() {
                var userWriter = new TIMM.Website.UserServices.UserWriterService();
                userWriter.DeleteUser(userName, function() {
                    var anSelected = fnGetSelected(testTable);
                    var iRow = testTable.fnGetPosition(anSelected[0]);
                    testTable.fnDeleteRow(iRow);
                }, null, null);

                $(this).dialog('destroy');
            },
            'No': function() {
                $(this).dialog('close');
            }
        }
    });
    $("#delete_confirm_dialog").dialog('open');
}

function onGetAllUsersSuccess(result) {
    var fnBoolRender = function(obj) {
        var sReturn = obj.aData[obj.iDataColumn];
        return sReturn ? 'Yes' : 'No';
    }
    var fnActionsRender = function(obj) {
        var format = [
                    '<a href="javascript:void(0);" onclick="javascript:onEditUser(this, \'{3}\')">Edit</a>',
                    '&nbsp;&nbsp;',
                    '<a href="javascript:void(0);" onclick="javascript:onDeleteUser(this, \'{4}\')">Delete</a>'
                ].join('');
        var sReturn = obj.aData[obj.iDataColumn];
        return format.replace('{3}', sReturn).replace('{4}', sReturn);
    }

    aUsers = [];
    for (var i = 0; i < result.length; i++) {
        var r = result[i];
        var GroupNameList = "";
        if (r.Groups != null) {
            for (var j = 0; j < r.Groups.length; j++)
                if ((j % 3 == 0) && (j != 0) && (j != r.Groups.length-1)) {
                    GroupNameList += r.Groups[j].Name + ',<br/>';
                } else {
                    GroupNameList += r.Groups[j].Name + ',';
                }
        }
        GroupNameList = GroupNameList.substring(0, GroupNameList.length - 1);

        aUsers.push([
                    r.UserName,
                    r.FullName,
                    r.UserCode,
                    r.Enabled,
                    // r.RoleName,
                    GroupNameList,
                    r.UserName
                ]);
    }

    testTable = $('#test_table').dataTable({
        'bJQueryUI': true,
        'sPaginationType': 'full_numbers',
        'bSort': true,
        'aaData': aUsers,
        'bAutoWidth': false,
        'aoColumns': [{
            'sTitle': 'User Name'
        }, {
            'sTitle': 'Full Name'
        }, {
            'sTitle': 'User Code'
        }, {
            'sTitle': 'Enabled',
            "fnRender": fnBoolRender
        }, {
           // 'sTitle': 'Role'
            //                    "fnRender": fnBoolRender
       // }, {
            'sTitle': 'Groups Name'
        }, {
            'sTitle': 'Actions',
            'sWidth': '100px',
            'fnRender': fnActionsRender
}]
});


        // Round input fields
        $('.fg-toolbar input').addClass('ui-corner-all');
        $('.fg-toolbar input').addClass('ui-widget-content');

        // Fix the display bug in MS IE
        $('#test_table thead tr th').each(function() {
            if ($.browser.msie) {
                $(this).css('position', 'relative');
                var icon = $('.css_right', this)[0];
                $(icon).css('position', 'absolute');
                $(icon).css('top', '2px');
                $(icon).css('right', '0px');
            }
        });

        // Highlight selected row
        $("#test_table tbody").click(function(event) {
            $(testTable.fnSettings().aoData).each(function() {
                $(this.nTr).removeClass('row_selected');
            });
            $(event.target.parentNode).addClass('row_selected');
        });
    }
    //]]>

    function updateTips(t) {
        $('#validateTips').text(t).effect("highlight", {}, 1500);
    }

    function edit_updateTips(t) {
        $('#edit_validateTips').text(t).effect("highlight", {}, 1500);
    }

    function checkLength(o, n, min, max) {
        if (o.val().length > max || o.val().length < min) {
            //o.addClass('ui-state-error');
            updateTips("Length of " + n + " must be between " + min + " and " + max + ".");
            return false;
        } else {
            return true;
        }
    }

    function checkEmail(email) {
        if (email.length > 0) {
            var reg = /^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$/;
            return reg.test(email);
        }
        return true;
    }

    function edit_checkLength(o, n, min, max) {
        if (o.val().length > max || o.val().length < min) {
            //o.addClass('ui-state-error');
            edit_updateTips("Length of " + n + " must be between " + min + " and " + max + ".");
            return false;
        } else {
            return true;
        }
    }

    $(document).ready(function () {
        // Populate all users in the user list
        onGetAllUsers();
        var userReader = new TIMM.Website.UserServices.UserReaderService();
        //userReader.UserEnumToList(onGetRoleList, null, null);
        var groupReader = new TIMM.Website.GroupServices.GroupReaderService();
        groupReader.GetAllGroups(onGetGroupList, null, null);

        // Fields
        var username = $('#username'),
                fullname = $('#fullname'),
                usercode = $('#usercode'),
                password = $('#password'),
                confirm_password = $('#confirm_password'),
        // userrole = $('#userrole'),
                email = $('#email');
        enabled = $('#enabled');
        var allFields = $([])
                .add(username)
                .add(fullname)
                .add(usercode)
                .add(password)
                .add(confirm_password)
                .add(email)
        // .add(userrole)
                .add(enabled);

        $("#user_dialog").dialog({
            bgiframe: false,
            autoOpen: false,
            height: 560,
            width: 400,
            modal: true,
            buttons: {
                'Create Account': function () {
                    var bValid = true;
                    allFields.removeClass('ui-state-error');

                    bValid = bValid && checkLength(username, "username", 3, 64);
                    if (!bValid) {
                        //updateTips("Length of username must be between 3 and 64.");
                        return;
                    }
                    bValid = bValid && checkLength(fullname, "fullname", 3, 128);
                    if (!bValid) {
                        //updateTips("Length of fullname must be between 2 and 128.");
                        return;
                    }
                    bValid = bValid && checkLength(usercode, "usercode", 1, 64);
                    if (!bValid) {
                        //updateTips("Length of usercode must be between 1 and 64.");
                        return;
                    }
                    bValid = bValid && checkLength(password, "password", 3, 64);
                    if (!bValid) {
                        //updateTips("Length of password must be between 3 and 64.");
                        return;
                    }
                    bValid = bValid && password.val() == confirm_password.val();
                    if (!bValid) {
                        updateTips('Password does not match with the confirmed password.');
                        return;
                    }
                    bValid = bValid && checkEmail(email.val());
                    if (!bValid) {
                        updateTips('Please enter a valid e-mail address in this format: yourname@domain.com.');
                        return;
                    }
                    //                    bValid = bValid && (userrole.val() != '0');
                    //                    if (!bValid) {
                    //                        updateTips('Please assign a role before you create the user.');
                    //                        return;
                    //                    }

                    var userReader = new TIMM.Website.UserServices.UserReaderService();
                    userReader.GetUser(username.val(), function (r) {
                        bValid = bValid && (r == null);
                        if (!bValid) {
                            updateTips('A same user name has existed, please use other name.');
                            return;
                        }
                        if (bValid) {
                            //                            if ($('#list-group-name').val()!=null)
                            var user = {
                                Id: 0,
                                UserName: username.val(),
                                FullName: fullname.val(),
                                UserCode: usercode.val(),
                                Password: password.val(),
                                Email: email.val(),
                                Enabled: $('#enabled:checked').val() != null
                                //                                Role: $("#userrole").val()
                            };

                            var userWriter = new TIMM.Website.UserServices.UserWriterService();
                            userWriter.AddUser(user, $('#list-group-name-new').val(), function (r) {
                                if (r) {
                                    var groupstr = "";
                                    var countNum = 0;
                                    $("#list-group-name-new option:selected").each(function () {
                                        groupstr += $(this).text() + ',';
                                        if ((countNum != 0) && (countNum % 3 == 0)) {
                                            groupstr += '<br/>';
                                        }
                                        countNum += 1;
                                    });
                                    groupstr = groupstr.substring(0, groupstr.length - 1);
                                    testTable.fnAddData([
                                                r.UserName,
                                                r.FullName,
                                                r.UserCode,
                                                r.Enabled,
                                    //r.RoleName,
                                                groupstr,
                                                r.UserName
                                            ]);
                                }
                            }, null, null);
                            $("#user_dialog").dialog('close');
                        }
                    });

                    //bValid = bValid && checkRegexp(name, /^[a-z]([0-9a-z_])+$/i, "Username may consist of a-z, 0-9, underscores, begin with a letter.");
                    //From jquery.validate.js (by joern), contributed by Scott Gonzalez: http://projects.scottsplayground.com/email_address_validation/
                    //bValid = bValid && checkRegexp(email, /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i, "eg. ui@jquery.com");
                    //bValid = bValid && checkRegexp(password, /^([0-9a-zA-Z])+$/, "Password field only allow : a-z 0-9");

                },
                Cancel: function () {
                    $('#list-group-name-new').val(null);
                    $(this).dialog('close');
                }
            },
            close: function () {
                allFields.val('').removeClass('ui-state-error');
            }
        });

        // Fields
        var edit_username = $('#edit_username'),
                edit_userid = $('#edit_userid'),
                edit_fullname = $('#edit_fullname'),
                edit_usercode = $('#edit_usercode'),
                edit_password = $('#edit_password'),
                edit_confirm_password = $('#edit_confirm_password'),
        //edit_is_admin = $('#edit_is_admin'),
        //edit_userrole = $('#edit_userrole'),
                edit_email = $('#edit_email'),
                edit_enabled = $('#edit_enabled');
        var edit_allFields = $([])
                .add(edit_username)
                .add(edit_userid)
                .add(edit_fullname)
                .add(edit_usercode)
                .add(edit_password)
                .add(edit_confirm_password)
                .add(edit_email)
        //.add(edit_is_admin)
        //.add(edit_userrole)
                .add(edit_enabled);

        $("#edit_user_dialog").dialog({
            bgiframe: false,
            autoOpen: false,
            height: 560,
            width: 400,
            modal: true,
            buttons: {
                'Save Changes': function () {
                    var bValid = true;
                    edit_allFields.removeClass('ui-state-error');

                    bValid = bValid && edit_checkLength(edit_username, "username", 3, 64);
                    if (!bValid) {
                        //updateTips("Length of username must be between 3 and 64.");
                        return;
                    }
                    bValid = bValid && edit_checkLength(edit_fullname, "fullname", 3, 128);
                    if (!bValid) {
                        //updateTips("Length of fullname must be between 3 and 128.");
                        return;
                    }
                    bValid = bValid && edit_checkLength(edit_usercode, "usercode", 1, 64);
                    if (!bValid) {
                        //updateTips("Length of usercode must be between 1 and 64.");
                        return;
                    }
                    bValid = bValid && edit_checkLength(edit_password, "password", 3, 64);
                    if (!bValid) {
                        //updateTips("Length of password must be between 3 and 64.");
                        return;
                    }
                    bValid = bValid && edit_password.val() == edit_confirm_password.val();
                    if (!bValid) {
                        edit_updateTips('Password does not match with the confirmed password.');
                        return;
                    }
                    bValid = bValid && checkEmail(edit_email.val());
                    if (!bValid) {
                        edit_updateTips('Please enter a valid e-mail address in this format: yourname@domain.com.');
                        return;
                    }
                   

                    //                    bValid = bValid && (edit_userrole.val() != '0');
                    //                    if (!bValid) {
                    //                        edit_updateTips('Please select a role before you submit the change.');
                    //                        return;
                    //                    }
                    if (bValid) {
                        var user = {
                            Id: edit_userid.val(),
                            UserName: edit_username.val(),
                            FullName: edit_fullname.val(),
                            UserCode: edit_usercode.val(),
                            Password: edit_password.val(),
                            Email: edit_email.val(),
                            //Role: $("#edit_userrole").val(),
                            Role: 0,
                            RoleName: '',
                            Enabled: $('#edit_enabled:checked').val() != null
                        };
                        var userWriter = new TIMM.Website.UserServices.UserWriterService();
                        //userWriter.AssignUserToGroups($('#list-group-name').val(), user.Id);
                        userWriter.UpdateUser(user, $('#list-group-name').val(), function (r) {
                            if (r) {
                                var anSelected = fnGetSelected(testTable);
                                var iRow = testTable.fnGetPosition(anSelected[0]);
                                var groupstr = "";
                                var countNum = 0;
                                $("#list-group-name option:selected").each(function () {
                                    groupstr += $(this).text() + ',';
                                    if ((countNum != 0) && (countNum % 3 == 0)) {
                                        groupstr += '<br/>';
                                    }
                                    countNum += 1;
                                });
                                groupstr = groupstr.substring(0, groupstr.length - 1);
                                testTable.fnUpdate(r.FullName, iRow, 1);
                                testTable.fnUpdate(r.UserCode, iRow, 2);
                                testTable.fnUpdate(r.Enabled, iRow, 3);
                                //testTable.fnUpdate(r.RoleName, iRow, 4);
                                testTable.fnUpdate(groupstr, iRow, 4);
                            }
                        });
                        $(this).dialog('close');
                    }
                },
                Cancel: function () {
                    $(this).dialog('close');
                }
            },
            close: function () {
                edit_allFields.val('').removeClass('ui-state-error');
            }
        });
    });