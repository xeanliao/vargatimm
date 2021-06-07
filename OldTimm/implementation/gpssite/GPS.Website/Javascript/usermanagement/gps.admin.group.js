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
var testTable, aGroups = [], actionNode;

function onGetAllGroups() {
    var groupReader = new TIMM.Website.GroupServices.GroupReaderService();
    groupReader.GetAllGroups(onGetAllGroupsSuccess, null, null);
}

function onGetPrivilegeList(r) {
    $('#list-privilege-name-new').empty();
    for (var i = 0; i < r.length; i++) {
        $('#list-privilege-name-new').append("<option value='" + r[i].Id + "'>" + r[i].Name + "</option>");
    }
}

function onAddGroup() {
    $('#list-group-name-new').val(null);
    $('#group_dialog').dialog('open');
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

function onEditGroup(obj, gId) {
    // Make the row selected
    fnSelectRowWithDelay(obj);

    var groupReader = new TIMM.Website.GroupServices.GroupReaderService();
    groupReader.GetGroup(gId, function(r) {
        // Assign dialog field values and show the dialog
//        $('#edit_ID').val(r.Id);
        $('#edit_groupname').val(r.Name);
        groupReader.GetAllPrivileges(function(privilegelist) {
            $('#list-privilege-name-edit').empty();
            var gplist = [];
            if (r.Privileges != null)
            for (var j = 0; j < r.Privileges.length; j++)
                gplist[j] = r.Privileges[j].Name;
            for (var i in privilegelist) {
                $('#list-privilege-name-edit').append("<option value='" + privilegelist[i].Id + "'>" + privilegelist[i].Name + "</option>");
            }
            $('#list-privilege-name-edit').val(gplist);
            }, null, null);
        $('#edit_group_dialog').dialog('open');
    }, null, null);
}

function onDeleteGroup(obj, groupName) {
    // Make the row selected
    fnSelectRowWithDelay(obj);

    // Show confirmation dialog
    $("#delete_confirm_dialog").dialog({
        autoOpen: false,
        modal: true,
        buttons: {
            'Yes': function() {
                var groupWriter = new TIMM.Website.GroupServices.GroupWriterService();
                groupWriter.DeleteGroup(groupName, function() {
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

function onGetAllGroupsSuccess(result) {
    var fnBoolRender = function(obj) {
    //var sReturn = obj.aData[obj.iDataColumn - 2];
    var sReturn = obj.aData[0];
        return sReturn ? 'Yes' : 'No';
    }
    var fnActionsRender = function(obj) {
        var format = [
                    '<a href="javascript:void(0);" onclick="javascript:onEditGroup(this, \'{3}\')">Edit</a>',
                    '&nbsp;&nbsp;',
                    '<a href="javascript:void(0);" onclick="javascript:onDeleteGroup(this, \'{4}\')">Delete</a>'
                ].join('');
        var sReturn = obj.aData[0];
        return format.replace('{3}', sReturn).replace('{4}', sReturn);
    }

    for (var i = 0; i < result.length; i++) {
        var r = result[i];
        var privilegeNameList = "";
        if (r.Privileges != null) {
            for (var j = 0; j < r.Privileges.length; j++)
                if(j%5==0){
                    privilegeNameList += r.Privileges[j].Name + ',<br/>';
                }else{
                    privilegeNameList += r.Privileges[j].Name + ',';
                }
                
        }
        privilegeNameList = privilegeNameList.substring(0, privilegeNameList.length - 1);
        aGroups.push([
                    //r.Id,
                    r.Name,
                    privilegeNameList,
                    r.Name
                ]);
    }

    testTable = $('#test_table').dataTable({
        'bJQueryUI': true,
        'sPaginationType': 'full_numbers',
        'bSort': true,
        'aaData': aGroups,
        'bAutoWidth': false,
        'aoColumns': [
//        {
//            'sTitle': 'Group ID'
//        }, 
        {
            'sTitle': 'Group Name'
        }, {
            'sTitle': 'privileges Name'
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

        function edit_checkLength(o, n, min, max) {
            if (o.val().length > max || o.val().length < min) {
                //o.addClass('ui-state-error');
                edit_updateTips("Length of " + n + " must be between " + min + " and " + max + ".");
                return false;
            } else {
                return true;
            }
        }

        $(document).ready(function() {
            // Populate all groups in the group list
            onGetAllGroups();
            var groupReader = new TIMM.Website.GroupServices.GroupReaderService();
            groupReader.GetAllPrivileges(onGetPrivilegeList, null, null);
            // Fields
            var name = $('#name');
            var allFields = $([]).add(name);

            $("#group_dialog").dialog({
                bgiframe: false,
                autoOpen: false,
                height: 460,
                width: 400,
                modal: true,
                buttons: {
                    'Create Group': function() {
                        var bValid = true;
                        allFields.removeClass('ui-state-error');

                        bValid = bValid && checkLength(name, "name", 1, 45);
                        if (!bValid) {
                            return;
                        }
                        else {
                            var group = {
                                Id: 0,
                                Name: name.val()
                            };

                            var groupReader = new TIMM.Website.GroupServices.GroupReaderService();
                            groupReader.GetGroupForValidate(group, function(r) {
                                bValid = bValid && (r == null);
                                if (!bValid) {
                                    updateTips('A same group name has existed, please use other name.');
                                    return;
                                } else {
                                    var groupWriter = new TIMM.Website.GroupServices.GroupWriterService();
                                    groupWriter.AddGroup(group,$('#list-privilege-name-new').val(),function(r) {
                                        if (r) {
                                            var privilegestr = "";
                                            $("#list-privilege-name-new option:selected").each(function() {
                                                privilegestr += $(this).text() + ',';
                                            });
                                            privilegestr = privilegestr.substring(0, privilegestr.length - 1);
                                            testTable.fnAddData([
                                                r.Id,
                                                r.Name,
                                                privilegestr,
                                                r.Id
                                            ]);
                                        }
                                    }, null, null);
                                    $("#group_dialog").dialog('close');
                                }
                            });
                        }
                    },
                    Cancel: function() {
                        $('#list-privilege-name-new').val(null);
                        $(this).dialog('close');
                    }
                },
                close: function() {
                   // $('#list-privilege-name-new').val(null);
                    allFields.val('').removeClass('ui-state-error');
                }
            });

            // Fields
            var edit_groupname = $('#edit_groupname'),
                edit_ID = $('#edit_ID');
            var edit_allFields = $([])
                .add(edit_ID)
                .add(edit_groupname);

            $("#edit_group_dialog").dialog({
                bgiframe: false,
                autoOpen: false,
                height: 460,
                width: 400,
                modal: true,
                buttons: {
                    'Save Changes': function() {
                        var bValid = true;
                        edit_allFields.removeClass('ui-state-error');

                        bValid = bValid && edit_checkLength(edit_groupname, "groupname", 1, 45);
                        if (!bValid) {
                            return;
                        }
                        else {
                            var group = {
                                Id: edit_ID.val(),
                                Name: edit_groupname.val()
                            };

                            var groupReader = new TIMM.Website.GroupServices.GroupReaderService();
                            groupReader.GetGroupForValidate(group, function(r) {
                                bValid = bValid && (r == null);
                                if (!bValid) {
                                    edit_updateTips('A same group name has existed, please use other name.');
                                    return;
                                } else {
                                    var groupWriter = new TIMM.Website.GroupServices.GroupWriterService();
                                    groupWriter.UpdateGroup(group, $('#list-privilege-name-edit').val(),function(r) {
                                        var anSelected = fnGetSelected(testTable);
                                        var iRow = testTable.fnGetPosition(anSelected[0]);
                                        var privilegestr = "";
                                        $("#list-privilege-name-edit option:selected").each(function() {
                                            privilegestr += $(this).text() + ',';
                                        });
                                        privilegestr = privilegestr.substring(0, privilegestr.length - 1);
                                        //testTable.fnUpdate(r.Id, iRow, 0);
                                        testTable.fnUpdate(r.Name, iRow, 1);
                                        testTable.fnUpdate(privilegestr, iRow, 2);
                                    }, null, null);
                                    $("#edit_group_dialog").dialog('close');
                                }
                            });
                        }
                    },
                    Cancel: function() {
                        $(this).dialog('close');
                    }
                },
                close: function() {
                    edit_allFields.val('').removeClass('ui-state-error');
                }
            });
        });