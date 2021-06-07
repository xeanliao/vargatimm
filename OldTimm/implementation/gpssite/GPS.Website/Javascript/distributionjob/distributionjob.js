
//************************************************************************
//jQuery Dialog 
//************************************************************************
var assigndriverDialog;
var assignauditorDialog;
var assignwalkersDialog;
var assigngtusDialog;

function ShowAssignDriverDialog(objDiv) {
    if (!assigndriverDialog) {
        assigndriverDialog = $(objDiv).dialog({
            modal: true, width: 540, height: 460, overlay: { opacity: 0.5 },
            buttons: {
                "Cancel": function() {
                    $(this).dialog("close");
                },
                "Save": function() {
                    GPS.Loading.show();
                    SaveDrivers();
                    
                }
            }

        });
    }
    $(assigndriverDialog).dialog("open");
}

function ShowAssignAuditorDialog(objDiv) {
    if (!assignauditorDialog) {
        assignauditorDialog = $(objDiv).dialog({
            modal: true, width: 540, height: 460, overlay: { opacity: 0.5 },
            buttons: {
                "Cancel": function() {
                    $(this).dialog("close");
                },
                "Save": function() {
                    GPS.Loading.show();
                    SaveAuditors();

                }
            }

        });
    }
    $(assignauditorDialog).dialog("open");
}

function ShowAssignWalkersDialog(objDiv) {
    if (!assignwalkersDialog) {
        assignwalkersDialog = $(objDiv).dialog({
            modal: true, width: 540, height: 460, overlay: { opacity: 0.5 },
            buttons: {
                "Cancel": function() {
                    $(this).dialog("close");
                },
                "Save": function() {
                    GPS.Loading.show();
                    SaveWalkers();

                }
            }
        });
    }
    $(assignwalkersDialog).dialog("open");
}

function ShowAssignGtusDialog(objDiv) {
    if (!assigngtusDialog) {
        assigngtusDialog = $(objDiv).dialog({
            modal: true, width: 700, height: 470, overlay: { opacity: 0.5 },
            buttons: {
                "Cancel": function() {
                    $(this).dialog("close");
                },
                "Save": function() {
                    GPS.Loading.show();
                    SaveGtus();

                }
            }
        });
    }
    $(assigngtusDialog).dialog("open");
}


//************************************************************************
//Functions for Assign/UnAssign Driver
//************************************************************************
var activeUser = null;
var activeDriver = null;
var objectStore = null;
var currentDistributionJobId = null;
var options = { completed1: false, completed2: false };

//Create list for the available drivers.
function CreateUserList(data) {
    var AvailableDirverListContainer = $("#users-list");
    AvailableDirverListContainer.find("ul").remove();
    //activeUser = new GPS.USER();
    var ulTitle = $('<ul id="users-list-ul"></ul>');
    if (data && data.length > 0) {
        $(AvailableDirverListContainer).append($(ulTitle));
        var ulRecord = $('<ul id="users-list-data-ul"></ul>');
        $.each(data, function(n, value) {
        var liUser = $('<li id="' + value.Id + '"></li>')
            .click(function() {
                var user = new GPS.USER();
                user._id = value.Id;
                user._fullname = value.FullName;
                user._role = value.Role;
                ActiveUser(user);
            });
            var divUserName = $('<span></span>').html(value.FullName);
            $(liUser).append($(divUserName));
            $(ulRecord).append($(liUser));
        });
        $(AvailableDirverListContainer).append($(ulRecord));
    }
    else {
        $(ulTitle).append('No user found.');
        $(AvailableDirverListContainer).append($(ulTitle));
    }
    options.completed1 = true;
    testall(options);
}

//Create list for the existing drivers
function CreateExistingDriverList(data) {
    var ExistingDirverListContainer = $("#drivers-list");
    ExistingDirverListContainer.find("ul").remove();
    activeDriver = new GPS.DJUSER();
    objectStore = new GPS.DJUSER.ObjectStore();
    var ulTitle = $('<ul id="drivers-list-ul"></ul>');
    var ulRecord = $('<ul id="drivers-list-data-ul"></ul>');
    if (data.DriverAssignments) {
        LoadDrivers(data.DriverAssignments);
        $(ExistingDirverListContainer).append($(ulTitle));
        $.each(data.DriverAssignments, function(n, value) {
            var liDriver = $('<li id="' + value.LoginUserId + '"></li>')
            .click(function() {
                var driver = new GPS.DJUSER();
                driver._fullname = value.FullName;
                driver._djrole = value.DjRole;
                driver._gtuuniqueid = value.UniqueID;
                driver._userid = value.LoginUserId;
                driver._distributionjobid = value.DistributionJobId;
                ActiveDriver(driver);
            });

            var divDriverName = $('<span></span>').html(value.FullName);
            $(liDriver).append($(divDriverName));
            $(ulRecord).append($(liDriver));
        });
        $(ExistingDirverListContainer).append($(ulRecord));
    }
    else {
        $(ExistingDirverListContainer).append($(ulRecord));
        $(ExistingDirverListContainer).append($(ulTitle));
    }
    options.completed2 = true;
    testall(options);
}

//Load User and the existing drivers
function LoadAll(id) {
    currentDistributionJobId = id;
    var userReader = new TIMM.Website.UserServices.UserReaderService();
    userReader.GetAllUsers(CreateUserList, null, null);
    //userReader.GetAllUsers(CreateAvailableListDiv, function() { options.completed1 = true; testall(options); }, null);

    var djReader = new TIMM.Website.DistributionMapServices.DJReaderService();
    djReader.GetDistributionJob(id, CreateExistingDriverList, null, null);

}
function testall(options) {
    if (options.completed1 && options.completed2) {
        $("#assigndrivers-progressbar_section").hide();
        $("#drivers-list").show();
        $("#users-list").show();
        $("#drivermenu-div").show();
        options.completed1 = false;
        options.completed2 = false;
    }
}

//set active user
function ActiveUser(u){
    activeUser = u;
    $("#users-list-data-ul").find("li").removeClass('itemselected');
    $("#users-list-data-ul").find("#" + u._id).addClass('itemselected');
}

//set active driver
function ActiveDriver(u) {
    activeDriver = u;
    $("#drivers-list-data-ul").find("li").removeClass('itemselected');
    $("#drivers-list-data-ul").find("#" + u._userid).addClass('itemselected');
}

//Select user and assign to driver list
function OnAssignDriver() {
    if (activeUser) {
        if (!objectStore.GetByFullName(activeUser._fullname)) {
            var newDriver = new GPS.DJUSER();
            newDriver._fullname = activeUser._fullname;
            newDriver._userid = activeUser._id;
            newDriver._distributionjobid = currentDistributionJobId;
            AppendDriver(newDriver);
        }
    }
}

//Remove a driver from the list
function OnUnAssignDriver() {
    if (activeDriver) {
        RemoveDriver(activeDriver);
    }
}

//Select a user to add into the driver list
function AppendDriver(obj) {
    var liDriver = $('<li id="' + obj._userid + '"></li>')
            .click(function() {
                var driver = new GPS.DJUSER();
                driver._fullname = obj._fullname;
                driver._userid = obj._userid;
                driver._distributionjobid = obj._distributionjobid;
                ActiveDriver(driver);
            });
    var divDriverName = $('<span></span>').html(obj._fullname);
    $(liDriver).append($(divDriverName));
    $("#drivers-list-data-ul").append($(liDriver));
    objectStore.Append(obj);
    activeUser = null;
}

//Remove Driver from Driver List
function RemoveDriver(obj) {
    $("#drivers-list-data-ul").find("li").remove('#' + obj._userid);
    objectStore.Remove(obj);
    activeDriver = null;
}

//Load the existing driver to the objectStore
function LoadDrivers(data) {
    var drivers = [];
    var i = 0;
    var length = data.length;
    while (i < length) {
        var driver = new GPS.DJUSER(data[i]);
        drivers.push(driver);
        i++;
    }
    objectStore.SetObjects(drivers);
}

//Save
function SaveDrivers() {
    var drivers = objectStore.GetObjects();
    var driverslist = [];
    
    for (var i = 0; i < drivers.length; i++) {
        var driver = {
        DistributionJobId: drivers[i]._distributionjobid,
        FullName: drivers[i]._fullname,
        LoginUserId: drivers[i]._userid
        };
        driverslist.push(driver);
    }
    var distributionjob = {
        Id: currentDistributionJobId,
        DriverAssignments: driverslist
    };
    var djWriter = new TIMM.Website.DistributionMapServices.DJWriterService();
    djWriter.SaveDrivers(distributionjob, function() {
        GPS.Loading.hide();
        objectStore = null;
        $(assigndriverDialog).dialog("close");
    });

}

//************************************************************************
//Functions for Assign/UnAssign Auditors
//************************************************************************
var activeAuditorUser = null;
var activeAuditor = null;
var auditorStore = null;
var currentDistributionJobId = null;
var auditoroptions = { completed1: false, completed2: false };

//Create list for the available auditors.
function CreateAuditorUserList(data) {
    var AvailableAuditorListContainer = $("#auditor-users-list");
    AvailableAuditorListContainer.find("ul").remove();
    var ulTitle = $('<ul id="auditor-users-list-ul"></ul>');
    if (data && data.length > 0) {
        $(AvailableAuditorListContainer).append($(ulTitle));
        var ulRecord = $('<ul id="auditor-users-list-data-ul"></ul>');
        $.each(data, function(n, value) {
            var liUser = $('<li id="' + value.Id + '"></li>')
            .click(function() {
                var user = new GPS.USER();
                user._id = value.Id;
                user._fullname = value.FullName;
                user._role = value.Role;
                ActiveAuditorUser(user);
            });
            var divUserName = $('<span></span>').html(value.FullName);
            $(liUser).append($(divUserName));
            $(ulRecord).append($(liUser));
        });
        $(AvailableAuditorListContainer).append($(ulRecord));
    }
    else {
        $(ulTitle).append('No user found.');
        $(AvailableAuditorListContainer).append($(ulTitle));
    }
    auditoroptions.completed1 = true;
    ShowAuditorDivs(auditoroptions);
}

//Create list for the existing auditors
function CreateExistingAuditorList(data) {
    var ExistingAuditorListContainer = $("#auditors-list");
    ExistingAuditorListContainer.find("ul").remove();
    activeAuditor = new GPS.DJUSER();
    auditorStore = new GPS.DJUSER.ObjectStore();
    var ulTitle = $('<ul id="auditors-list-ul"></ul>');
    var ulRecord = $('<ul id="auditors-list-data-ul"></ul>');
    if (data.AuditorAssignment) {
        LoadAuditorToStore(data.AuditorAssignment);
        $(ExistingAuditorListContainer).append($(ulTitle));
        var liAuditor = $('<li id="' + data.AuditorAssignment.LoginUserId + '"></li>')
            .click(function() {
                var auditor = new GPS.DJUSER();
                auditor._fullname = data.AuditorAssignment.FullName;
                auditor._djrole = data.AuditorAssignment.DjRole;
                auditor._gtuuniqueid = data.AuditorAssignment.UniqueID;
                auditor._userid = data.AuditorAssignment.LoginUserId;
                auditor._distributionjobid = data.AuditorAssignment.DistributionJobId;
                ActiveAuditor(auditor);
            });

            var divAuditorName = $('<span></span>').html(data.AuditorAssignment.FullName);
        $(liAuditor).append($(divAuditorName));
        $(ulRecord).append($(liAuditor));
        $(ExistingAuditorListContainer).append($(ulRecord));
        $("#btnAssignAuditor").attr('disabled', true);
    }
    else {
        $("#btnAssignAuditor").removeAttr('disabled');
        $(ExistingAuditorListContainer).append($(ulTitle));
        $(ExistingAuditorListContainer).append($(ulRecord));
    }
    auditoroptions.completed2 = true;
    ShowAuditorDivs(auditoroptions);
}

//Load User and the existing drivers
function LoadAuditors(id) {
    currentDistributionJobId = id;
    var userReader = new TIMM.Website.UserServices.UserReaderService();
    userReader.GetAllUsers(CreateAuditorUserList, null, null);

    var djReader = new TIMM.Website.DistributionMapServices.DJReaderService();
    djReader.GetDistributionJob(id, CreateExistingAuditorList, null, null);

}
function ShowAuditorDivs(auditoroptions) {
    if (auditoroptions.completed1 && auditoroptions.completed2) {
        $("#assignauditors-progressbar_section").hide();
        $("#auditors-list").show();
        $("#auditor-users-list").show();
        $("#auditormenu-div").show();
        auditoroptions.completed1 = false;
        auditoroptions.completed2 = false;
    }
}

//set active user
function ActiveAuditorUser(u) {
    activeAuditorUser = u;
    $("#auditor-users-list-data-ul").find("li").removeClass('itemselected');
    $("#auditor-users-list-data-ul").find("#" + u._id).addClass('itemselected');
}

//set active driver
function ActiveAuditor(u) {
    activeAuditor = u;
    $("#auditors-list-data-ul").find("li").removeClass('itemselected');
    $("#auditors-list-data-ul").find("#" + u._userid).addClass('itemselected');
}

//Select user and assign to auditor list
function OnAssignAuditor() {
    if (activeAuditorUser) {
        var newAuditor = new GPS.DJUSER();
        newAuditor._fullname = activeAuditorUser._fullname;
        newAuditor._userid = activeAuditorUser._id;
        newAuditor._distributionjobid = currentDistributionJobId;
        AppendAuditor(newAuditor);
        
    }
}

//Remove the auditor from the distribution job
function OnUnAssignAuditor() {
    if (activeAuditor) {
        RemoveAuditor(activeAuditor);
        
    }
}

//Select a user to add into the auditor
function AppendAuditor(obj) {
    var liAuditor = $('<li id="' + obj._userid + '"></li>')
            .click(function() {
                var auditor = new GPS.DJUSER();
                auditor._fullname = obj._fullname;
                auditor._userid = obj._userid;
                auditor._distributionjobid = obj._distributionjobid;
                ActiveAuditor(auditor);
            });
    var divAuditorName = $('<span></span>').html(obj._fullname);
    $(liAuditor).append($(divAuditorName));
    $("#auditors-list-data-ul").append($(liAuditor));
    auditorStore.Append(obj);
    activeAuditorUser = null;
    $("#btnAssignAuditor").attr('disabled', true);
}

//Remove Driver from Driver List
function RemoveAuditor(obj) {
    $("#auditors-list-data-ul").find("li").remove('#' + obj._userid);
    auditorStore.Remove(obj);
    activeAuditor = null;
    if (auditorStore.GetObjects().length == 0) {
        $("#btnAssignAuditor").removeAttr('disabled');    
    }
}

//Load the existing driver to the objectStore
function LoadAuditorToStore(data) {
    var auditors = [];
    var auditor = new GPS.DJUSER(data);
    auditors.push(auditor);
    auditorStore.SetObjects(auditors);
}

//Save
function SaveAuditors() {
    var auditors = auditorStore.GetObjects();
    if (auditors.length > 0) {
        var auditor = {
        DistributionJobId: auditors[0]._distributionjobid,
        FullName: auditors[0]._fullname,
        LoginUserId: auditors[0]._userid
        };
    }
    var distributionjob = {
        Id: currentDistributionJobId,
        AuditorAssignment: auditor
    };
    var djWriter = new TIMM.Website.DistributionMapServices.DJWriterService();
    djWriter.SaveAuditor(distributionjob, function() {
        GPS.Loading.hide();
        auditorStore = null;
        $(assignauditorDialog).dialog("close");
    });

}

//************************************************************************
//Functions for Assign/UnAssign Walkers
//************************************************************************
var activeWalkerUser = null;
var activeWalker = null;
var walkerStore = null;
var currentDistributionJobId = null;
var walkeroptions = { completed1: false, completed2: false };

//Create list for all login users.
function CreateWalkerUserList(data) {
    var AvailableWalkerListContainer = $("#walkers-users-list");
    AvailableWalkerListContainer.find("ul").remove();
    //activeUser = new GPS.USER();
    var ulTitle = $('<ul id="walkers-users-list-ul"></ul>');
    if (data && data.length > 0) {
        $(AvailableWalkerListContainer).append($(ulTitle));
        var ulRecord = $('<ul id="walkers-users-list-data-ul"></ul>');
        $.each(data, function(n, value) {
            var liUser = $('<li id="' + value.Id + '"></li>')
            .click(function() {
                var user = new GPS.USER();
                user._id = value.Id;
                user._fullname = value.FullName;
                user._role = value.Role;
                ActiveWalkerUser(user);
            });
            var divUserName = $('<span></span>').html(value.FullName);
            $(liUser).append($(divUserName));
            $(ulRecord).append($(liUser));
        });
        $(AvailableWalkerListContainer).append($(ulRecord));
    }
    else {
        $(ulTitle).append('No user found.');
        $(AvailableWalkerListContainer).append($(ulTitle));
    }
    walkeroptions.completed1 = true;
    ShowWalkersDivs(walkeroptions);
}

//Create list for the existing walkers
function CreateExistingWalkersList(data) {
    var ExistingWalkerListContainer = $("#walkers-list");
    ExistingWalkerListContainer.find("ul").remove();
    activeWalker = new GPS.DJUSER();
    walkerStore = new GPS.DJUSER.ObjectStore();
    var ulTitle = $('<ul id="walkers-list-ul"></ul>');
    var ulRecord = $('<ul id="walkers-list-data-ul"></ul>');
    if (data.WalkerAssignments) {
        LoadWalkersToStore(data.WalkerAssignments);
        $(ExistingWalkerListContainer).append($(ulTitle));
        $.each(data.WalkerAssignments, function(n, value) {
            var liWalker = $('<li id="' + value.LoginUserId + '"></li>')
            .click(function() {
                var walker = new GPS.DJUSER();
                walker._fullname = value.FullName;
                walker._djrole = value.DjRole;
                walker._gtuuniqueid = value.UniqueID;
                walker._userid = value.LoginUserId;
                walker._distributionjobid = value.DistributionJobId;
                ActiveWalker(walker);
            });
            var divWalkerName = $('<span></span>').html(value.FullName);
            $(liWalker).append($(divWalkerName));
            $(ulRecord).append($(liWalker));
        });
        $(ExistingWalkerListContainer).append($(ulRecord));
    }
    else {
        $(ExistingWalkerListContainer).append($(ulRecord));
        $(ExistingWalkerListContainer).append($(ulTitle));
    }
    walkeroptions.completed2 = true;
    ShowWalkersDivs(walkeroptions);
}

//Load User and the existing walkers
function LoadWalkers(id) {
    currentDistributionJobId = id;
    var userReader = new TIMM.Website.UserServices.UserReaderService();
    userReader.GetAllUsers(CreateWalkerUserList, null, null);
    var djReader = new TIMM.Website.DistributionMapServices.DJReaderService();
    djReader.GetDistributionJob(id, CreateExistingWalkersList, null, null);

}
function ShowWalkersDivs(walkeroptions) {
    if (walkeroptions.completed1 && walkeroptions.completed2) {
        $("#assignwalkers-progressbar_section").hide();
        $("#walkers-list").show();
        $("#walkers-users-list").show();
        $("#walkermenu-div").show();
        walkeroptions.completed1 = false;
        walkeroptions.completed2 = false;
    }
}

//set active user
function ActiveWalkerUser(u) {
    activeWalkerUser = u;
    $("#walkers-users-list-data-ul").find("li").removeClass('itemselected');
    $("#walkers-users-list-data-ul").find("#" + u._id).addClass('itemselected');
}

//set active driver
function ActiveWalker(u) {
    activeWalker = u;
    $("#walkers-list-data-ul").find("li").removeClass('itemselected');
    $("#walkers-list-data-ul").find("#" + u._userid).addClass('itemselected');
}

//Select user and assign to driver list
function OnAssignWalker() {
    if (activeWalkerUser) {
        if (!walkerStore.GetByFullName(activeWalkerUser._fullname)) {
            var newWalker = new GPS.DJUSER();
            newWalker._fullname = activeWalkerUser._fullname;
            newWalker._userid = activeWalkerUser._id;
            newWalker._distributionjobid = currentDistributionJobId;
            AppendWalker(newWalker);
        }
    }
}

//Remove a driver from the list
function OnUnAssignWalker() {
    if (activeWalker) {
        RemoveWalker(activeWalker);
    }
}

//Select a user to add into the driver list
function AppendWalker(obj) {
    var liWalker = $('<li id="' + obj._userid + '"></li>')
            .click(function() {
                var walker = new GPS.DJUSER();
                walker._fullname = obj._fullname;
                walker._userid = obj._userid;
                walker._distributionjobid = obj._distributionjobid;
                ActiveWalker(walker);
            });
    var divWalkerName = $('<span></span>').html(obj._fullname);
    $(liWalker).append($(divWalkerName));
    $("#walkers-list-data-ul").append($(liWalker));
    walkerStore.Append(obj);
    activeWalkerUser = null;
}

//Remove Driver from Driver List
function RemoveWalker(obj) {
    $("#walkers-list-data-ul").find("li").remove('#' + obj._userid);
    walkerStore.Remove(obj);
    activeWalker = null;
}

//Load the existing driver to the walkerStore
function LoadWalkersToStore(data) {
    var walkers = [];
    var i = 0;
    var length = data.length;
    while (i < length) {
        var walker = new GPS.DJUSER(data[i]);
        walkers.push(walker);
        i++;
    }
    walkerStore.SetObjects(walkers);
}

//Save
function SaveWalkers() {
    var walkers = walkerStore.GetObjects();
    var walkerslist = [];

    for (var i = 0; i < walkers.length; i++) {
        var walker = {
            DistributionJobId: walkers[i]._distributionjobid,
            FullName: walkers[i]._fullname,
            LoginUserId: walkers[i]._userid
        };
        walkerslist.push(walker);
    }
    var distributionjob = {
        Id: currentDistributionJobId,
        WalkerAssignments: walkerslist
    };
    var djWriter = new TIMM.Website.DistributionMapServices.DJWriterService();
    djWriter.SaveWalkers(distributionjob, function() {
        GPS.Loading.hide();
        walkerStore = null;
        $(assignwalkersDialog).dialog("close");
    });

}

//************************************************************************
//Functions for Assign/UnAssign Gtus
//************************************************************************
var activeWalkerUser = null;
var activeWalker = null;
var gtuStore = null;
var gtuslist = null;
var gtuoptions = { completed1: false, completed2: false };

function GetGtusList(data) {
    gtuslist = data;
    gtuoptions.completed1 = true;
    ShowGtusDivs(gtuoptions);
}

//Create list for the existing walkers
function CreateExistingMembersList(data) {
    var ExistingMembersListContainer = $("#gtus-users-list");
    ExistingMembersListContainer.find("#gtus-users-list-data-ul").remove();
    //activeWalker = new GPS.DJUSER();
    gtuStore = new GPS.DJ.ObjectStore();
    LoadMembersToGtuStore(data);
    //var ulTitle = $('<ul id="gtus-users-list-ul"><li>aaaabbcc</li></ul>');
    var ulRecord = $('<div id="gtus-users-list-data-ul"></div>');
    //$(ExistingMembersListContainer).append($(ulTitle));
    if (data.AuditorAssignment) {
        var liUser = $('<div id="' + data.AuditorAssignment.LoginUserId + '"><div style="clear:both;height:1px;font-size:1px;margin:0px;padding:0px;"></div></div>');
        var divUserName = $('<span></span>').html("&nbsp;" + data.AuditorAssignment.DjRoleName + "&nbsp;:&nbsp;" + data.AuditorAssignment.FullName + "&nbsp;&nbsp;&nbsp;&nbsp;");
        var divGtu = $('<select id="' + data.AuditorAssignment.LoginUserId + '"class="text ui-widget-content ui-corner-all"></select>')
                    .change(function() {
                        var user = new GPS.DJUSER();
                        user._fullname = data.AuditorAssignment.FullName;
                        user._djrole = data.AuditorAssignment.DjRole;
                        user._gtuuniqueid = $(divGtu).val();
                        user._userid = data.AuditorAssignment.LoginUserId;
                        user._distributionjobid = data.AuditorAssignment.DistributionJobId;
                        UpdateAuditorGtu(divGtu, user, data.AuditorAssignment.UniqueID);
                    });
        BindGtuOptions(divGtu, data.AuditorAssignment);
        $(liUser).append($(divGtu));
        $(liUser).append($(divUserName));
        $(ulRecord).append($(liUser));
        $(ExistingMembersListContainer).append($(ulRecord));
    }
    if (data.DriverAssignments) {
        $.each(data.DriverAssignments, function(n, value) {
            var liUser = $('<div id="' + value.LoginUserId + '"></div>');
            var divUserName = $('<span></span>').html("&nbsp;" + value.DjRoleName + "&nbsp;:&nbsp;" + value.FullName + "&nbsp;&nbsp;&nbsp;&nbsp;");
            var divGtu = $('<select id="' + value.LoginUserId + '"class="text ui-widget-content ui-corner-all"></select>')
                        .change(function() {
                            var user = new GPS.DJUSER();
                            user._fullname = value.FullName;
                            user._djrole = value.DjRole;
                            user._gtuuniqueid = $(divGtu).val();
                            user._userid = value.LoginUserId;
                            user._distributionjobid = value.DistributionJobId;
                            UpdateDriverGtu(divGtu, user, value.UniqueID);
                        });
            BindGtuOptions(divGtu, value);
            $(liUser).append($(divGtu));
            $(liUser).append($(divUserName));
            $(ulRecord).append($(liUser));
        });
            $(ExistingMembersListContainer).append($(ulRecord));
    }
    if (data.WalkerAssignments) {
        $.each(data.WalkerAssignments, function(n, value) {
            var liUser = $('<div id="' + value.LoginUserId + '"></div>');
            var divUserName = $('<span></span>').html("&nbsp;" + value.DjRoleName + "&nbsp;:&nbsp;" + value.FullName + "&nbsp;&nbsp;&nbsp;&nbsp;");
            var divGtu = $('<select id="' + value.LoginUserId + '"class="text ui-widget-content ui-corner-all"></select>')
                        .change(function() {
                            var user = new GPS.DJUSER();
                            user._fullname = value.FullName;
                            user._djrole = value.DjRole;
                            user._gtuuniqueid = $(divGtu).val();
                            user._userid = value.LoginUserId;
                            user._distributionjobid = value.DistributionJobId;
                            UpdateWalkerGtu(divGtu, user, value.UniqueID);
                        });
            BindGtuOptions(divGtu, value);
            $(liUser).append($(divGtu));
            $(liUser).append($(divUserName));
            $(ulRecord).append($(liUser));
        });
        $(ExistingMembersListContainer).append($(ulRecord));
    }
    gtuoptions.completed2 = true;
    ShowGtusDivs(gtuoptions);
}

//Load User and the existing walkers
function LoadGtus(id) {
    var gtuReader = new TIMM.Website.TrackServices.GtuReaderService();
    gtuReader.GetAllGtus(GetGtusList, null, null);

    var djReader = new TIMM.Website.DistributionMapServices.DJReaderService();
    djReader.GetDistributionJob(id, CreateExistingMembersList, null, null);
    
}
function ShowGtusDivs(gtuoptions) {
    if (gtuoptions.completed1 && gtuoptions.completed2) {
        $("#assigngtus-progressbar_section").hide();
        $("#gtus-users-list").show();
        gtuoptions.completed1 = false;
        gtuoptions.completed2 = false;
    }
}

//Load the existing driver to the gtuStore
function LoadMembersToGtuStore(data) {
    if (data) {
        var dj = new GPS.DJ(data);
        gtuStore.SetObjects(dj);
    }
}

function BindGtuOptions(div, dataitem) {
    $("<option value='" + "0" + "'>" + "None" + "</option>").appendTo($(div));
    for (var i = 0; i < gtuslist.length; i++) {
        if (dataitem.UniqueID == gtuslist[i].UniqueID) {
            $("<option value='" + gtuslist[i].UniqueID + "' selected>" + gtuslist[i].UniqueID + "</option>").appendTo($(div));
        }
        else {
            $("<option value='" + gtuslist[i].UniqueID + "'>" + gtuslist[i].UniqueID + "</option>").appendTo($(div));
        }
    }
}

function UpdateAuditorGtu(div, obj, olduniqueid) {
    if (gtuStore.CheckGTUIsUsed(obj)) {
        GPSAlert('This GTU has been assigned to other member.');
        $(div).val(olduniqueid);
    }
    else {
        gtuStore.UpdateAuditorGtu(obj);
    }
}

function UpdateWalkerGtu(div, obj, olduniqueid) {
    if (gtuStore.CheckGTUIsUsed(obj)) {
        GPSAlert('This GTU has been assigned to other member.');
        $(div).val(olduniqueid);
    }
    else {
        gtuStore.UpdateWalkerGtu(obj);
    }
}

function UpdateDriverGtu(div, obj, olduniqueid) {
    if (gtuStore.CheckGTUIsUsed(obj)) {
        GPSAlert('This GTU has been assigned to other member.');
        $(div).val(olduniqueid);
    }
    else {
        gtuStore.UpdateDriverGtu(obj);
    }
}

//Save Gtus
function SaveGtus() {
    var dj = gtuStore.GetObjects();
    var distributionjob = {
        Id: dj._id,
        WalkerAssignments: dj._walkers,
        AuditorAssignment: dj._auditor,
        DriverAssignments: dj._drivers
    };
    var djWriter = new TIMM.Website.DistributionMapServices.DJWriterService();
    djWriter.SaveGtus(distributionjob, function() {
        GPS.Loading.hide();
        gtuStore = null;
        $(assigngtusDialog).dialog("close");
    });
}