///<reference path="../jquery-1.3.2-vsdoc.js" />
// Define javascript objects specific to the AdminCenter.ascx web control.
var SalesG = false;
var CampaignSupervisorG = false;
var appendCam = "";
var camTitle = new Array();
var hasHighestPrivilege = false;
var distributionSupervisorG = false;
     


/////////////////////////////////////////CampaignCenter///////////////////////////////////////////////////////////////////
GPS.CampaignCenter = GPS.EventTrigger.extend({

    init: function () {
        this._super();
        this.campaignDialogId = "#div-create-campaign-dialog";
        this.view = undefined;
        this.checkedCampaignIds = [];
        this.checkedCampaignNames = [];
        this.confirmationMessage = "Are you sure you want to delete all selected Campaigns?";
        this.confirmCopyMessage = "Are you sure you want to copy the following Campaign(s): ";
        this.yesBtn = "yes";
        this.publishcampaignDialogID = "#div-publish-campaign-dialog";
        this.currentCampaignId = 0;
    },

    OnCampaignListItemClicked: function (campaignId) {
        window.open('Campaign.aspx?cid={0}'.replace("{0}", campaignId), '_blank', 'resizable=yes,status=yes,toolbar=no,menubar=no,location=no');
    },

    OnBulkDeleteCampaigns: function () {
        if (this.checkedCampaignIds.length > 0) {
            var thisObj = this;
            GPSConfirm(this.confirmationMessage, function (btn) {
                if (btn == thisObj.yesBtn) thisObj._DoBulkDeleteCampaigns();
            });
        }
    },

    OnBulkCopyCampaigns: function () {
        if (this.checkedCampaignIds.length > 0) {
            var thisObj = this;
            thisObj.GetCheckedCampaignNames();
            var confirmCopyMessage = this.confirmCopyMessage + "<br>";
            for (var i = 0; i < this.checkedCampaignNames.length; i++) {
                confirmCopyMessage = confirmCopyMessage + "<br>" + this.checkedCampaignNames[i] + "<br>";
            }
            GPSConfirm(confirmCopyMessage, function (btn) {
                if (btn == thisObj.yesBtn) thisObj._DoBulkCopyCampaigns();
            });
        }
    },

    GetCheckedCampaignNames: function () {
        var thisObj = this;
        thisObj.checkedCampaignNames = [];
        for (var i = 0; i < thisObj.checkedCampaignIds.length; i++) {
            var camid;
            var campName;
            camid = thisObj.checkedCampaignIds[i];
            camName = $('#campaign-center-campaign-list' + ' div .control-center-camp-title[timmcampaignid |= "' + camid + '"]').html();
            thisObj.checkedCampaignNames.push(camName);
        }
    },

    OnBulkPublishCampaigns: function () {
        if (this.checkedCampaignIds.length > 0) {
            var thisObj = this;
            var userService = new TIMM.Website.UserServices.UserReaderService();
            //userService.GetAllUsersByPrivilege(2, function(userlist) {
            //Distribution Supervisor = 52, Administrator:53
            var glist = new Array(3);
            glist[0] = 51;
            glist[1] = 52;
            glist[2] = 53;
            userService.GetAllUsersByGroupList(glist, function (userlist) {
                thisObj._ShowUsersDialog(userlist);
            });
        }
    },



    OnCampaignListItemChecked: function (checkState) {
        var existing = Array.selectIndexes(this.checkedCampaignIds, function (e) { return e == checkState[0]; });
        if (checkState[1] == true && existing.length == 0)
            this.checkedCampaignIds.push(checkState[0]);
        else if (checkState[1] == false && existing.length > 0)
            this.checkedCampaignIds.splice(existing[0], 1);
    },

    OnEditCampaignListItem: function (campaignId) {
        var thisObj = this;
        var campaignService = new TIMM.Website.CampaignServices.CampaignReaderService();
        campaignService.GetCampaignForEdit(campaignId, function (ret) {
            if (ret) thisObj._ShowEditCampaignDialog(ret);
        }, function () {
            alert("An error occurs when trying to fetch the campaign.");
        });
    },

    OnNewCampaignClicked: function () {
        if (!submapRole) return;
        this._ShowCreateCampaignDialog();
    },

    //    RefreshViewAfterDataPopulation: function() {
    //        setTimeout("$('#campaign-center-loading').hide()", 20);
    //        $('#campaign-center-toolbar').show();
    //    },

    SetView: function (view) {
        this.view = view;
    },

    _DoBulkDeleteCampaigns: function () {
        var thisObj = this;

        $('#campaign-center-campaign-list').html("Processing....");
        var service = new TIMM.Website.CampaignServices.CampaignWriterService();
        service.DeleteCampaigns(this.checkedCampaignIds, function () {
            //thisObj._RemoveListItemsFromView(thisObj.checkedCampaignIds);
            //thisObj.TriggerEvent('list_items_removed', thisObj.checkedCampaignIds);
            UpdateCampaignsList();

        }, function (e) {
            if (e._exceptionType == "GPS.Website.CampaignServices.MyException") {
                alert(e._message);
                location.href = "login.html";
            }
            else {
                alert("An error occurs when trying to delete the selected campaigns.");
            }
        });
        this.checkedCampaignIds = [];
    },

    _DoBulkCopyCampaigns: function () {
        var thisObj = this;

        $('#campaign-center-campaign-list').html("Processing....");
        var service = new TIMM.Website.CampaignServices.CampaignWriterService();
        service.CopyCampaigns(this.checkedCampaignIds, function () {
            UpdateCampaignsList();

        }, function (e) {
            if (e._exceptionType == "GPS.Website.CampaignServices.MyException") {
                alert(e._message);
                location.href = "login.html";
            }
            else {
                alert("An error occurs when trying to copy the selected campaigns.");
            }
        });
        this.checkedCampaignIds = [];
        this.checkedCampaignNames = [];
    },

    _RemoveListItemsFromView: function (campaignIdsToRemove) {
        this.view.RemoveListItems(campaignIdsToRemove);
    },

    // Show and hide related controls for Edit Campaign
    //
    _SetControlVisibilityForEditCampaign: function () {
        //$('#campaign-logo-container').hide();
        $('#sequence-no-container').show();
    },

    // Show and hide related controls for New Campaign
    //
    _SetControlVisibilityForNewCampaign: function () {
        $('#campaign-logo-container').show();
        $('#sequence-no-container').hide();
    },


    _ShowUsersDialog: function (userlist) {
        var thisObj = this;
        var publishDialog = $(thisObj.publishcampaignDialogID).dialog({
            title: 'Campaign Publish',
            width: 400,
            modal: true,
            overlay: { opacity: 0.5 },
            buttons: {
                'Cancel': function () {
                    thisObj._CleanForm();
                    $(publishDialog).dialog("destroy");
                    publishDialog = null;
                },
                'Save': function () {
                    //SubMap = 0,
                    //DistributionMap = 1,
                    thisObj._SavePublishCampaignToServer(thisObj.checkedCampaignIds, $('#list-user-name').val(), 1);
                    thisObj.checkedCampaignIds = [];
                    $(publishDialog).dialog("destroy");
                    $('#campaign-center-campaign-list').html("Processing....");
                    $('#dm-center-campaign-list').html("Processing....");
                    publishDialog = null;
                    UpdateCampaignsList();
                    UpdateDmCenterCampaignsList();
                    //alert("Publish successfully!");
                }
            }
        });
        publishDialog.dialog("open");
        this._BindUsersToForm(userlist);
        this._SetControlVisibilityForNewCampaign();

    },

    _ShowCreateCampaignDialog: function () {

        this._CleanForm();
        $(campaignDialog).dialog("destroy");
        campaignDialog = null;

        var thisObj = this;
        if (CampaignSupervisorG) {
            $("#select-saler-container").show();
            thisObj._GetAllSales();
        }
        var campaignDialog = $(thisObj.campaignDialogId).dialog({
            title: 'Campaign Properties',
            width: 400,
            modal: true,
            overlay: { opacity: 0.5 },
            buttons: {
                'Cancel': function () {
                    thisObj._CleanForm();
                    $(campaignDialog).dialog("destroy");
                    campaignDialog = null;
                },
                'Save': function () {
                    var campaignProperties = thisObj._CollectNewCampaignProperties();
                    if (thisObj._ValidateInputValues(campaignProperties)) {
                        thisObj._SaveNewCampaignToServer(campaignProperties);
                        thisObj._CleanForm();
                        $(campaignDialog).dialog("destroy");
                        campaignDialog = null;
                    } else {
                        alert('Please fill in all fields.');
                    }
                }
            }
        });
        campaignDialog.dialog("open");

        this._SetControlVisibilityForNewCampaign();
    },

    _GetAllSales: function () {
        var thisObj = this;
        var userService = new TIMM.Website.UserServices.UserReaderService();
        var glist = new Array(1);
        glist[0] = 46;
        userService.GetAllUsersByGroupList(glist, function (userlist) {
            thisObj._ShowAllSalesSelect(userlist);
        });
    },

    _ShowAllSalesSelect: function (r) {
        $("#all-sales-select").empty();
        $("<option value='" + "" + "'>" + "Please Select Sales" + "</option>").appendTo("#all-sales-select");
        for (var i = 0; i < r.length; i++) {
            $("<option value='" + r[i].UserName + "'>" + r[i].UserName + "</option>").appendTo("#all-sales-select");
        }
    },

    _ShowEditCampaignDialog: function (existingCampaignProps) {

        this._CleanForm();
        $(campaignDialog).dialog("destroy");
        campaignDialog = null;

        this.currentCampaignId = existingCampaignProps.Id;

        var thisObj = this;
        if (CampaignSupervisorG) {
            $("#select-saler-container").show();
            thisObj._GetEditSales(existingCampaignProps.UserName);
        }
        var campaignDialog = $(thisObj.campaignDialogId).dialog({
            title: 'Campaign Properties',
            width: 400,
            modal: true,
            overlay: { opacity: 0.5 },
            buttons: {
                'Cancel': function () {
                    thisObj._CleanForm();
                    $(campaignDialog).dialog("destroy");
                    campaignDialog = null;
                },
                'Save': function () {
                    var properties = thisObj._CollectCampaignPropertiesForUpdate();
                    //properties.Id = existingCampaignProps.Id;
                    properties.Id = thisObj.currentCampaignId;
                    if (thisObj._ValidateInputValuesForUpdate(properties)) {
                        thisObj._SaveCampaignUpdateToServer(properties);
                        thisObj._CleanForm();
                        $(campaignDialog).dialog("destroy");
                        campaignDialog = null;
                    } else {
                        alert('Please fill in all fields.');
                    }
                }
            }
        });
        campaignDialog.dialog("open");


        this._SetControlVisibilityForEditCampaign();
        this._BindCampaignPropertiesToForm(existingCampaignProps);
    },

    _GetEditSales: function (salesnm) {
        var thisObj = this;
        var userService = new TIMM.Website.UserServices.UserReaderService();
        var glist = new Array(1);
        glist[0] = 46;
        userService.GetAllUsersByGroupList(glist, function (r) {
            $("#all-sales-select").empty();
            $("<option value='" + salesnm + "'>" + salesnm + "</option>").appendTo("#all-sales-select");
            var k = -1;
            for (var i = 0; i < r.length; i++) {
                if (r[i].UserName == salesnm)
                    k = i;
                else
                    $("<option value='" + r[i].UserName + "'>" + r[i].UserName + "</option>").appendTo("#all-sales-select");
            }
            if (k == -1) {
                $("#all-sales-select").get(0).options[0].value = "";
                $("#all-sales-select").get(0).options[0].text = "Please Select Sales";
            }
        });
    },

    //    _ShowAllSalesSelect: function(r) {
    //        $("#all-sales-select").empty();
    //        $("<option value='" + "" + "'>" + "Please Select Sales" + "</option>").appendTo("#all-sales-select");
    //        for (var i = 0; i < r.length; i++) {
    //            $("<option value='" + r[i].UserName + "'>" + r[i].FullName + "</option>").appendTo("#all-sales-select");
    //        }
    //    },


    _CleanForm: function () {
        $(this.campaignDialogId + ' input').val('');
        $('#campaign-creation-date').datepicker().datepicker('setDate', new Date());
        $(this.campaignDialogId + ' img').attr('src', '');
        $(this.campaignDialogId + ' img').attr("style", "display: none; height:200px;");
        $('#btn-insert-logo').val("Upload");
    },

    _BindCampaignPropertiesToForm: function (existingCampaignProps) {
        $('#txt-client-name').val(existingCampaignProps.ClientName);
        $('#txt-contact-name').val(existingCampaignProps.ContactName);
        $('#txt-client-code').val(existingCampaignProps.ClientCode);
        $('#txt-area-description').val(existingCampaignProps.AreaDescription);
        $('#txt-sequence-no').val(existingCampaignProps.Sequence);
        $('#all-sales-select').val(existingCampaignProps.UserName);
        BindCampaignCreationDate(Date.serverFormatToClientFormat(existingCampaignProps.Date));

        if (existingCampaignProps.Logo && existingCampaignProps.Logo.length > 0) {
            $("#img-insert-logo").attr("src", "Files/Images/" + existingCampaignProps.Logo);
            $("#hidden-insert-logo").val(existingCampaignProps.Logo);
            $("#img-insert-logo").show();
        }
    },

    _BindUsersToForm: function (userlist) {
        $('#list-user-name').empty();
        for (var i in userlist) {
            $('#list-user-name').append("<option value='" + userlist[i].Id + "'>" + userlist[i].UserName + "</option>");
        }
    },

    _CollectNewCampaignProperties: function () {
        return {
            ClientName: $('#txt-client-name').val(),
            ContactName: $('#txt-contact-name').val(),
            ClientCode: $('#txt-client-code').val(),
            //AreaDescription: $('#txt-area-description').val(),
            AreaDescription: $('#select-total-type').val(),
            UserName: $('#all-sales-select').val(),
            Date: Date.clientFormatToServerFormat(GetCampaignCreationDate()),
            Logo: $('#hidden-insert-logo').val()
        }
    },

    _CollectCampaignPropertiesForUpdate: function () {
        return {
            ClientName: $('#txt-client-name').val(),
            ContactName: $('#txt-contact-name').val(),
            ClientCode: $('#txt-client-code').val(),
            AreaDescription: $('#txt-area-description').val(),
            UserName: $('#all-sales-select').val(),
            Date: Date.clientFormatToServerFormat(GetCampaignCreationDate()),
            Logo: $('#hidden-insert-logo').val(),
            Sequence: $('#txt-sequence-no').val()
        }
    },

    _SaveNewCampaignToServer: function (newCampaignProperties) {
        var thisObj = this;
        var service = new TIMM.Website.CampaignServices.CampaignWriterService();
        service.CreateNewCampaign(newCampaignProperties, function (returnedCampaign) {
            thisObj._OpenCampaignInEditWindow(returnedCampaign.Id);
            UpdateCampaignsList();
            UpdateDmCenterCampaignsList();
        }, function (e) {
            if (e._exceptionType == "GPS.Website.CampaignServices.MyException") {
                alert(e._message);
                location.href = "login.html";
            }
            else {
                alert("An error occurs when trying to create the new campaign.");
            }
        });
    },


    _SavePublishCampaignToServer: function (campaignIds, userId, status) {
        var thisObj = this;
        var service = new TIMM.Website.CampaignServices.CampaignWriterService();
        service.PublishCampaign(campaignIds, userId, status, function (returnStr) {
            if (returnStr != "") GPSAlert(returnStr);
        }, function (e) {
            if (e._exceptionType == "GPS.Website.CampaignServices.MyException") {
                alert(e._message);
                location.href = "login.html";
            }
            else {
                alert("An error occurs when trying to publish the campaign.");
            }
        });
    },

    _SaveCampaignUpdateToServer: function (campaignProperties) {
        var thisObj = this;
        var service = new TIMM.Website.CampaignServices.CampaignWriterService();
        service.UpdateCampaignProperties(campaignProperties, function (returnedCampaign) {
            thisObj._RefereshListViewItem(returnedCampaign);
            thisObj.TriggerEvent('list_item_updated', returnedCampaign);
        }, function (e) {
            if (e._exceptionType == "GPS.Website.CampaignServices.MyException") {
                alert(e._message);
                location.href = "login.html";
            }
            else {
                alert("An error occurs when trying to save the changes.");
            }
        });
    },

    _AppendNewCampaignToListView: function (newCampaignProperties) {
        this.view.AppendCampaignToList(newCampaignProperties);
    },

    _RefereshListViewItem: function (campaignProperties) {
        this.view.RefereshItem(campaignProperties);
    },


    _ValidateInputValues: function (newCampaignProperties) {
        return !(String.isNullOrEmpty(newCampaignProperties.ContactName)
            || String.isNullOrEmpty(newCampaignProperties.ClientCode)
            || String.isNullOrEmpty(newCampaignProperties.ContactName)
            || newCampaignProperties.AreaDescription.indexOf("select") > 0 //String.isNullOrEmpty(newCampaignProperties.AreaDescription)
            || String.isNullOrEmpty(newCampaignProperties.Date)
            || (String.isNullOrEmpty(newCampaignProperties.UserName) && CampaignSupervisorG));
    },

    _ValidateInputValuesForUpdate: function (campaignProperties) {
        return !(String.isNullOrEmpty(campaignProperties.ContactName)
            || String.isNullOrEmpty(campaignProperties.ClientCode)
            || String.isNullOrEmpty(campaignProperties.ContactName)
            || String.isNullOrEmpty(campaignProperties.AreaDescription)
            || String.isNullOrEmpty(campaignProperties.Date)
            || String.isNullOrEmpty(campaignProperties.Sequence)
            || (String.isNullOrEmpty(campaignProperties.UserName) && CampaignSupervisorG));
    },

    _OpenCampaignInEditWindow: function (campaignId) {
        window.open('Campaign.aspx?cid={0}'.replace("{0}", campaignId), '_blank', 'resizable=yes,status=yes,toolbar=no,menubar=no,location=no');
    }
});


////////////////////////////////////////////DistributionMapCenter///////////////////////////////////////////////////////////
GPS.DmCenter = Class.extend({

    init: function() {
        this.checkedCampaignIds = [];
        this.nopublishcampaignDialogID = "#div-no-publish-campaign-dialog";
        this.view = undefined;
        this.checkedCampaignNames = [];
        this.yesBtn = "yes";
    },

    SetView: function(view) {
        this.view = view;
    },

    OnCampaignListItemClicked: function(campaignId) {
        window.open('DistributionMap.aspx?cid={0}'.replace("{0}", campaignId), '_blank', 'resizable=yes,status=yes,toolbar=no,menubar=no,location=no');
    },

    OnCampaignsRemoved: function(campaignIdsToRemove) {
        this.view.RemoveListItems(campaignIdsToRemove);
    },

    OnCampaignUpdated: function(updatedCampaign) {
        this.view.RefereshItem(updatedCampaign);
    },

    OnCampaignListItemChecked: function(checkState) {
        var existing = Array.selectIndexes(this.checkedCampaignIds, function(e) { return e == checkState[0]; });
        if (checkState[1] == true && existing.length == 0)
            this.checkedCampaignIds.push(checkState[0]);
        else if (checkState[1] == false && existing.length > 0)
            this.checkedCampaignIds.splice(existing[0], 1);
    },

    OnBulkPublishDMs: function() {
        if (this.checkedCampaignIds.length > 0) {
            var thisObj = this;
            var userService = new TIMM.Website.UserServices.UserReaderService();
            //Sales:46, Driver:48, Client:49, Auditor:50, DistributionManager:51, DistributionSupervisor: 52, Administrator:53
            //privilege:realtime:3, history:5
            var glist = new Array(7);
            glist[0] = 48;
            glist[1] = 49;
            glist[2] = 50;
            glist[3] = 51;
            glist[4] = 52;
            glist[5] = 53;
            glist[6] = 46;
            userService.GetAllUsersByGroupList(glist, function(userlist) {
                thisObj._ShowMonitorUsersDialog(userlist);
            });
        }
    },

    //    OnBulkNoPublishCampaigns: function() {
    //        if (this.checkedTaskIds.length > 0) {
    //            var thisObj = this;
    //            var userService = new TIMM.Website.UserServices.UserReaderService();
    //            GPSConfirm(this.confirmationMessage, function(btn) {
    //                if (btn == thisObj.yesBtn) {
    //                    var glist = new Array(3);
    //                    glist[0] = 46;
    //                    glist[1] = 47;
    //                    glist[2] = 53;
    //                    userService.GetAllUsersByGroupList(glist, function(userlist) {
    //                        thisObj._ShowSubmapsUsersDialog(userlist);
    //                    });
    //                }
    //                //thisObj._DoBulkDeleteTasks();
    //            });
    //        }
    //    },

    OnBulkNoPublishCampaigns: function() {
        if (this.checkedCampaignIds.length > 0) {
            var thisObj = this;
            thisObj.GetCheckedCampaignNames();
            var confirmCopyMessage = 'Are you sure you would like to move  {0} to Campaigns? Any changes that were made will be lost.';
            var cams = '<br/><br/>';
            for (var i = 0; i < thisObj.checkedCampaignNames.length; i++) {
                cams = cams + thisObj.checkedCampaignNames[i] + '<br/><br/>';
            }
            confirmCopyMessage = confirmCopyMessage.replace("{0}",cams);
            var userService = new TIMM.Website.UserServices.UserReaderService();
            //userService.GetAllUsersByPrivilege(1, function(userlist) {
            //Sales :46, CampaignSupervisor:47, Administrator:53
            GPSConfirm(confirmCopyMessage, function(btn) {
                if (btn == thisObj.yesBtn) {
                    var glist = new Array(3);
                    glist[0] = 46;
                    glist[1] = 47;
                    glist[2] = 53;
                    userService.GetAllUsersByGroupList(glist, function(userlist) {
                        thisObj._ShowSubmapsUsersDialog(userlist);
                    });
                }
            });


        }
    },

    GetCheckedCampaignNames: function() {
        var thisObj = this;
        thisObj.checkedCampaignNames = [];
        for (var i = 0; i < thisObj.checkedCampaignIds.length; i++) {
            var camid;
            var campName;
            camid = thisObj.checkedCampaignIds[i];
            camName = $('#dm-center-campaign-list' + ' div .control-center-camp-title[timmcampaignid |= "' + camid + '"]').html();
            thisObj.checkedCampaignNames.push(camName);
        }
    },

    _SetControlVisibilityForNewCampaign: function() {
        $('#campaign-logo-container').show();
        $('#sequence-no-container').hide();
    },

    _ShowSubmapsUsersDialog: function(userlist) {
        var thisObj = this;
        var nopublishDialog = $(thisObj.nopublishcampaignDialogID).dialog({
            title: 'Campaign Dismiss',
            width: 400,
            modal: true,
            overlay: { opacity: 0.5 },
            buttons: {
                'Cancel': function() {
                    $(nopublishDialog).dialog("destroy");
                    nopublishDialog = null;
                },
                'Save': function() {
                    //SubMap = 0,
                    //DistributionMap = 1,
                    thisObj._SavePublishCampaignToServer(thisObj.checkedCampaignIds, $('#list-user-name-submap').val(), 0);
                    $(nopublishDialog).dialog("destroy");
                    $('#dm-center-campaign-list').html("Processing....");
                    $('#campaign-center-campaign-list').html("Processing....");
                    nopublishDialog = null;
                    UpdateCampaignsList();
                    UpdateDmCenterCampaignsList();
                    //alert("Dismiss successfully!");
                }
            }
        });
        nopublishDialog.dialog("open");
        this._BindUsersToForm(userlist);
        this._SetControlVisibilityForNewCampaign();

    },

    _ShowMonitorUsersDialog: function(userlist) {
        var thisObj = this;
        var publishMonitorDialog = $(thisObj.nopublishcampaignDialogID).dialog({
            title: 'Campaign Monitor',
            width: 400,
            modal: true,
            overlay: { opacity: 0.5 },
            buttons: {
                'Cancel': function() {
                    $(publishMonitorDialog).dialog("destroy");
                    publishMonitorDialog = null;
                },
                'Save': function() {
                    //SubMap = 0,
                    //DistributionMap = 1,
                    //Monitor = 2
                    thisObj._SavePublishMonitorToServer(thisObj.checkedCampaignIds, $('#list-user-name-submap').val(), 2);
                    thisObj.checkedCampaignIds = [];
                    $(publishMonitorDialog).dialog("destroy");
                    $('#dm-center-campaign-list').html("Processing....");
                    $('#productivity-center-campaign-list').html("Processing....");
                    publishMonitorDialog = null;
                    UpdateDmCenterCampaignsList();
                    UpdateTasksList();
                }
            }
        });
        publishMonitorDialog.dialog("open");
        this._BindUsersToForm(userlist);
        this._SetControlVisibilityForNewCampaign();

    },

    _BindUsersToForm: function(userlist) {
        $('#list-user-name-submap').empty();
        for (var i in userlist) {
            $('#list-user-name-submap').append("<option value='" + userlist[i].Id + "'>" + userlist[i].UserName + "</option>");
        }
    },

    _SavePublishCampaignToServer: function(campaignIds, userId, status) {
        var thisObj = this;
        var service = new TIMM.Website.CampaignServices.CampaignWriterService();
        service.PublishCampaign(campaignIds, userId, status, function() {
        }, function() {
            alert("An error occurs when trying to not publish the campaign.");
        });
    },

    _SavePublishMonitorToServer: function(campaignIds, userId, status) {
        var thisObj = this;
        var service = new TIMM.Website.CampaignServices.CampaignWriterService();
        service.PublishCampaign(campaignIds, userId, status, function(returnStr) {
            if (returnStr != "") GPSAlert(returnStr);
        }, function() {
            alert("An error occurs when trying to not publish the campaign.");
        });
    }
});

//    RefreshViewAfterDataPopulation: function() {
//        setTimeout("$('#dm-center-loading').hide()", 20);
//    }



//function onGetSubmaps() {
//    var camReader = new TIMM.Website.CampaignServices.CampaignReaderService();
//    if ($('#sel-task-campaigns').val()) {
//        camReader.GetCampaignById($('#sel-task-campaigns').val(), function(cam) {
//            $('#sel-task-submaps').empty();
//            $('#sel-task-submaps').append("<option value=''>----------------------------Select----------------------------</option>");    
//            if (cam.SubMaps!=undefined) {
//                for (var i = 0; i < cam.SubMaps.length; i++) {
//                    $('#sel-task-submaps').append("<option value='" + cam.SubMaps[i].Id + "'>" + cam.SubMaps[i].Name + "</option>");
//                }
//            }
//        });
//    }

//    
//}

//function onGetDMs() {
//    var dmsReader = new TIMM.Website.DistributionMapServices.DMReaderService();
//    if ($('#sel-task-submaps').val()) {
//        dmsReader.GetSubMap($('#sel-task-submaps').val(), function(sub) {
//            $('#sel-task-dms').empty();
//            $('#sel-task-dms').append("<option value=''>----------------------------Select----------------------------</option>");  
//            if (sub.DistributionMaps != undefined) {
//                for (var i = 0; i < sub.DistributionMaps.length; i++) {
//                    $('#sel-task-dms').append("<option value='" + sub.DistributionMaps[i].Id + "'>" + sub.DistributionMaps[i].Name + "</option>");
//                }
//            }
//        });
//    }
//}

/////////////////////////////////////////////TaskCenter & MonitorCenter/////////////////////////////////////////////////////////////////////////
GPS.ProductivityCenter = GPS.EventTrigger.extend({

    init: function () {
        this._super();
        this.taskDialogId = "#div-create-task-dialog";
        this.assignDialogId = "#div-assign-GTUs-task-dialog";
        this.delTaskDialogID = "#div-del-Task-Dialog";
        this.view = undefined;
        this.checkedTaskIds = [];
        this.confirmationMessage = "Are you sure you want to delete all selected Tasks?";
        this.confirmationMessageFinish = "Are you sure you want to mark finish all selected Tasks?";
        this.yesBtn = "yes";

    },

    OnTaskListItemClicked: function (taskId) {
        //window.open('Task.aspx?cid={0}'.replace("{0}", taskId), '_blank', 'resizable=yes,status=yes,toolbar=no,menubar=no,location=no');
    },

    OnBulkDeleteTasks: function () {
        if (this.checkedTaskIds.length > 0) {
            var thisObj = this;
            var userService = new TIMM.Website.UserServices.UserReaderService();
            GPSConfirm(this.confirmationMessage, function (btn) {
                if (btn == thisObj.yesBtn) {
                    var glist = new Array(2);
                    glist[0] = 52;
                    glist[1] = 53;
                    userService.GetAllUsersByGroupList(glist, function (userlist) {
                        thisObj._ShowDMmapsUsersDialog(userlist);
                    });
                }
                //thisObj._DoBulkDeleteTasks();
            });
        }
    },

    _ShowDMmapsUsersDialog: function (userlist) {
        var thisObj = this;
        var delTaskDmDialog = $(thisObj.delTaskDialogID).dialog({
            title: 'Campaign Dismiss',
            width: 400,
            modal: true,
            overlay: { opacity: 0.5 },
            buttons: {
                'Cancel': function () {
                    $(delTaskDmDialog).dialog("destroy");
                    delTaskDmDialog = null;
                },
                'Save': function () {
                    thisObj._SavePublishCampaignToServer(thisObj.checkedTaskIds, $('#list-user-name-task').val(), 1);
                    $(delTaskDmDialog).dialog("destroy");
                    delTaskDmDialog = null;
                    $('#productivity-center-campaign-list').html("Processing....");
                    $('#dm-center-campaign-list').html("Processing....");
                    UpdateDmCenterCampaignsList();
                    UpdateTasksList();
                }
            }
        });
        delTaskDmDialog.dialog("open");
        this._BindUsersToTaskForm(userlist);
        this._SetControlVisibilityForNewCampaign();
    },

    _SavePublishCampaignToServer: function (taskIds, userId, status) {
        var thisObj = this;
        var service = new TIMM.Website.TaskServices.TaskWriterService();
        service.DismissTasks(this.checkedTaskIds, userId, function () {
        }, function () {
            alert("An error occurs when trying to delete task.");
        });
    },

    _BindUsersToTaskForm: function (userlist) {
        $('#list-user-name-task').empty();
        for (var i in userlist) {
            $('#list-user-name-task').append("<option value='" + userlist[i].Id + "'>" + userlist[i].UserName + "</option>");
        }
    },
    _SetControlVisibilityForNewCampaign: function () {
        $('#campaign-logo-container').show();
        $('#sequence-no-container').hide();
    },

    OnBulkAssignGTUsToTask: function () {
        if (this.checkedTaskIds.length > 0) {
            var thisObj = this;

            //var taskService = new TIMM.Website.TaskServices.TaskReaderService();
            //taskService.IsTaskDateDuplicate(thisObj.checkedTaskIds, function(flag) {
            //if (flag == true) {
            //    alert("Can't assign GTUs to the tasks on the same day! ")
            //}
            //else 
            //{

            var gtuService = new TIMM.Website.TrackServices.GtuReaderService();
            gtuService.GetAllGtus(function (gtuList) {
                thisObj._ShowAssignGTUsToTaskDialog(gtuList);
            });
            //}

            //});
        }
        else {
            alert("You need select tasks below first!");
        }
    },


    OnTaskListItemChecked: function (checkState) {
        var existing = Array.selectIndexes(this.checkedTaskIds, function (e) { return e == checkState[0]; });
        if (checkState[1] == true && existing.length == 0)
            this.checkedTaskIds.push(checkState[0]);
        else if (checkState[1] == false && existing.length > 0)
            this.checkedTaskIds.splice(existing[0], 1);
    },

    OnEditTaskListItem: function (taskId) {
        var thisObj = this;
        var taskService = new TIMM.Website.TaskServices.TaskReaderService();
        var dmService = new TIMM.Website.DistributionMapServices.DMReaderService();
        taskService.GetTask(taskId, function (ret) {
            if (ret) {
                //get submapid,campaignid by task
                //dmService.GetSubmapCamp(ret.DmId, function(results) {
                thisObj._ShowEditTaskDialog(ret);
                //});
            }
        }, function () {
            alert("An error occurs when trying to fetch the task.");
        });
    },


    OnTaskUpdated: function (updatedTask) {
        this.view.RefereshItem(updatedTask);
    },

    OnTasksRemoved: function (taskIdsToRemove) {
        this.view.RemoveListItems(taskIdsToRemove);
    },

    //    OnNewTaskClicked: function() {
    //        this._ShowEditTaskDialog();
    //    },

    OnMarkFinishClicked: function () {
        if (this.checkedTaskIds.length > 0) {
            var thisObjOut = this;
            var serviceOut = new TIMM.Website.TaskServices.TaskReaderService();
            serviceOut.GetStartOrStopByTaskId(thisObjOut.checkedTaskIds, function (timeType) {
                if (timeType == 1 || timeType==2) {
                    var thisObj = thisObjOut;
                    GPSConfirm("Are you sure you want to mark finish all selected Tasks?", function (btn) {
                        if (btn == thisObj.yesBtn) {
                            $('#campaign-center-report-list').html("Processing....");
                            var service = new TIMM.Website.TaskServices.TaskWriterService();
                            service.MarkFinish(thisObj.checkedTaskIds, function () {
                                UpdateTasksList();
                                UpdateReportsList();
                            });
                        }
                    });
                } else {
                    alert("You could not mark finish for this task because you have not stop monitor(s) yet!");
                }

            });
        }
    },

    SetView: function (view) {
        this.view = view;
    },

    _DoBulkDeleteTasks: function () {
        var thisObj = this;
        $('#productivity-center-campaign-list').html("Processing....");
        var service = new TIMM.Website.TaskServices.TaskWriterService();
        service.DeleteTasks(this.checkedTaskIds, function () {
            thisObj._RemoveListItemsFromView(thisObj.checkedTaskIds);
            thisObj.TriggerEvent('list_items_removed', thisObj.checkedTaskIds);
            UpdateTasksList();
        }, function () {
            alert("An error occurs when trying to delete the selected tasks.");
        });
    },

    _RemoveListItemsFromView: function (taskIdsToRemove) {
        this.view.RemoveListItems(taskIdsToRemove);
    },


    _ShowAssignGTUsToTaskDialog: function (gtuList) {
        var thisObj = this;
        var assignDialog = $(thisObj.assignDialogId).dialog({
            title: 'Assign GTUs',
            width: 500,
            modal: true,
            overlay: { opacity: 0.5 },
            buttons: {
                'Cancel': function () {
                    thisObj._CleanForm();
                    $(assignDialog).dialog("destroy");
                    assignDialog = null;
                },
                'Save': function () {
                    var taskService = new TIMM.Website.TaskServices.TaskWriterService();
                    taskService.IsValidSelectedGTUs(thisObj.checkedTaskIds, $('#list-gtus').val(), function (str) {
                        if (str != "") {
                            alert(str);
                        }
                        else {
                            taskService.UpdateTasksWithGTUs(thisObj.checkedTaskIds, $('#list-gtus').val(), function () {
                                thisObj._CleanForm();
                                $(assignDialog).dialog("destroy");
                                assignDialog = null;
                                UpdateTasksList();

                            }, function (e) {
                                alert(e._message);
                            });
                        }
                    });
                }
            }
        });
        assignDialog.dialog("open");
        this._BindGTUsToForm(gtuList);
    },


    _BindGTUsToForm: function (gtuList) {
        $('#list-gtus').empty();
        for (var i in gtuList) {
            var appendStr = (gtuList[i].UserName && gtuList[i].UserName != "") ? " (" + gtuList[i].UserName + ")" : "";
            var UId;
            UId = gtuList[i].UniqueID;
            if (UId.length > 6) {
                var len = UId.length;
                UId = UId.substr(len - 6, 6);
            }
            $('#list-gtus').append("<option value='" + gtuList[i].Id + "'>" + UId + appendStr + "</option>");
        }
    },

    //    _ShowUsersDialog: function(userlist) {
    //        var thisObj = this;
    //        var publishDialog = $(thisObj.publishcampaignDialogID).dialog({
    //            modal: true,
    //            overlay: { opacity: 0.5 },
    //            buttons: {
    //                'Cancel': function() {
    //                    thisObj._CleanForm();
    //                    $(publishDialog).dialog("destroy");
    //                    publishDialog = null;
    //                },
    //                'Save': function() {
    //                    //SubMap = 0,
    //                    //DistributionMap = 1,
    //                    thisObj._SavePublishCampaignToServer(thisObj.checkedCampaignIds, $('#list-user-name').val(), 1);
    //                    $(publishDialog).dialog("destroy");
    //                    publishDialog = null;
    //                    UpdateCampaignsList();
    //                    UpdateDmCenterCampaignsList();
    //                }
    //            }
    //        });
    //        publishDialog.dialog("open");
    //        this._BindUsersToForm(userlist);
    //        this._SetControlVisibilityForNewCampaign();

    //    },


    _ShowEditTaskDialog: function (existingTaskProps) {
        var thisObj = this;
        var taskDialog = $(thisObj.taskDialogId).dialog({
            title: 'Task Properties',
            width: 800,
            modal: true,
            overlay: { opacity: 0.5 },
            buttons: {
                'Cancel': function () {
                    thisObj._CleanForm();
                    $(taskDialog).dialog("destroy");
                    taskDialog = null;
                },
                'Save': function () {
                    var taskProperties = thisObj._CollectNewTaskProperties();
                    if (thisObj._ValidateInputValues(taskProperties)) {
                        if (thisObj._ValidateEmail(taskProperties)) {
                            thisObj._SaveTaskUpdateToServer(taskProperties);
                            thisObj._CleanForm();
                            $(taskDialog).dialog("destroy");
                            taskDialog = null;
                        } else {
                            alert("The email format is not correct!");
                        }
                    } else {
                        alert('Please fill in all fields.');
                    }
                }
            }
        });
        taskDialog.dialog("open");
        this._BindTaskPropertiesToForm(existingTaskProps);
        //this._SetControlVisibilityForNewTask();
    },

    //    _ShowEditTaskDialog: function(existingTaskProps, results) {
    //        var thisObj = this;
    //        var taskDialog = $(thisObj.taskDialogId).dialog({
    //            title: 'Task Properties',
    //            width: 400,
    //            modal: true,
    //            overlay: { opacity: 0.5 },
    //            buttons: {
    //                'Cancel': function() {
    //                    thisObj._CleanForm();
    //                    $(taskDialog).dialog("destroy");
    //                    taskDialog = null;
    //                },
    //                'Save': function() {
    //                    var properties = thisObj._CollectTaskPropertiesForUpdate();
    //                    properties.Id = existingTaskProps.Id;
    //                    if (thisObj._ValidateInputValuesForUpdate(properties)) {
    //                        thisObj._SaveTaskUpdateToServer(properties);
    //                        thisObj._CleanForm();
    //                        $(taskDialog).dialog("destroy");
    //                        taskDialog = null;
    //                    } else {
    //                        alert('Please fill in all fields.');
    //                    }
    //                }
    //            }
    //        });
    //        taskDialog.dialog("open");

    //        this._SetControlVisibilityForEditTask();
    //        this._BindTaskPropertiesToForm(existingTaskProps, results);
    //    },

    _CleanForm: function () {
        $(this.taskDialogId + ' input').val('');
    },

    _BindTaskPropertiesToForm: function (existingTaskProps) {
        $('#txt-task-name').val(existingTaskProps.Name);
        $('#select-task-auditor').val(existingTaskProps.AuditorId);
        BindTaskCreationDate(Date.serverFormatToClientFormat(existingTaskProps.Date));
        $('#sel-task-dms').val(existingTaskProps.DmId);
        $('#sel-task-id').val(existingTaskProps.Id);
        $('#tex-task-email').val(existingTaskProps.Email);
        if (existingTaskProps.Telephone != null) {
            var telarrys = existingTaskProps.Telephone.split("@");
            $('#tex-task-tel').val(telarrys[0]);
            $('#tex-task-tel-post').val("@" + telarrys[1]);
        }

        if (existingTaskProps.Taskgtuinfomappings.length > 0)
            document.getElementById('select-task-auditor').disabled = "disabled";
    },


    _CollectNewTaskProperties: function () {
        return {
            Name: $('#txt-task-name').val(),
            Date: Date.clientFormatToServerFormat(GetTaskCreationDate()),
            AuditorId: $('#select-task-auditor').val(),
            DmId: $('#sel-task-dms').val(),
            Id: $('#sel-task-id').val(),
            Taskgtuinfomappings: null,
            Tasktimes: null,
            Email: $('#tex-task-email').val(),
            Telephone: $('#tex-task-tel').val() + $('#tex-task-tel-post').val()
        }
    },

    _CollectTaskPropertiesForUpdate: function () {
        return {
            Name: $('#txt-task-name').val(),
            Date: Date.clientFormatToServerFormat(GetTaskCreationDate()),
            AuditorId: $('#select-task-auditor').val(),
            DmId: $('#sel-task-dms').val(),
            Taskgtuinfomappings: null,
            Tasktimes: null,
            Email: $('#tex-task-email').val(),
            Telephone: $('#tex-task-tel').val() + $('#tex-task-tel-post').val()
        }
    },

    _SaveNewTaskToServer: function (newTaskProperties) {
        var thisObj = this;
        var service = new TIMM.Website.TaskServices.TaskWriterService();
        service.AddTask(newTaskProperties, function (taskret) {
            UpdateTasksList();
        }, function (e) {
            alert("An error occurs when trying to create the new task.");
        });
    },

    _SaveTaskUpdateToServer: function (taskProperties) {
        var thisObj = this;
        var service = new TIMM.Website.TaskServices.TaskWriterService();
        service.UpdateTask(taskProperties, function (returnedTask) {
            //            thisObj._RefereshListViewItem(returnedTask);
            //            thisObj.TriggerEvent('list_item_updated', returnedTask);
            UpdateTasksList();
        }, function () {
            alert("An error occurs when trying to save the changes.");
        });
    },

    _AppendNewTaskToListView: function (newTaskProperties) {
        this.view.AppendTaskToList(newTaskProperties);
    },

    _RefereshListViewItem: function (taskProperties) {
        this.view.RefereshItem(taskProperties);
    },

    _ValidateEmail: function (taskProperties) {
        var myarray;
        var patterns = /^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$/;
        var flagForEmail = true;
        myarray = taskProperties.Email.split(";");
        if ((myarray[0] != "") && (myarray.length >= 1)) {
            for (i = 0; i < myarray.length; i++) {
                if (!patterns.test(myarray[i])) {
                    flagForEmail = false;
                    break;
                }
            }
        } else {
            flagForEmail = false;
        }

        return flagForEmail;
    },

    _ValidateInputValues: function (newTaskProperties) {
        return !(String.isNullOrEmpty(newTaskProperties.Name)
            || String.isNullOrEmpty(newTaskProperties.Date)
            || String.isNullOrEmpty(newTaskProperties.AuditorId));
    }
});

/////////////////////////////////////////AdminCenter///////////////////////////////////////////////////////////////////////       
GPS.AdminCenter = {
    /**
    * TIMM Control Center page navigation link - User
    */
    OnUsersClick: function() {
        if (!userRole) return;
        window.open('Users.aspx', '_blank', 'resizable=yes,status=yes,toolbar=no,menubar=no,location=no');
    },

    /**
    * TIMM Control Center page navigation link - User
    */
    OnGtusClick: function() {
        if (!userRole) return;
        window.open('GtuAdmin.aspx?AssignNameToGTUFlag={0}'.replace("{0}", AssignNameToGTU), '_blank', 'resizable=yes,status=yes,toolbar=no,menubar=no,location=no');
    },

    OnNonDeliverablesClick: function() {
        if (!userRole) return;
        window.open('NonDeliverables.aspx', '_blank', 'resizable=yes,status=yes,toolbar=no,menubar=no,location=no');
    },

    OnAvailableGTUListClick: function() {
        if (!userRole) return;
        window.open('AvailableGTUList.aspx', '_blank', 'resizable=yes,status=yes,toolbar=no,menubar=no,location=no');
    },

    /**
    * TIMM Control Center page navigation link - Group
    */
    OnGroupsClick: function() {
        if (!userRole) return;
        window.open('Groups.aspx', '_blank', 'resizable=yes,status=yes,toolbar=no,menubar=no,location=no');
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////






GPS.ControlCenter = { }

// Show a list of Campaigns and related UI elements for the user to manipulate
// the Campaigns.
//-------------------------------------------------CampaignView & DistritubionMapView------------------------------------------------------------------------

GPS.ControlCenter.CampaignListView = GPS.EventTrigger.extend({
    init: function(options) {
        this._super();
        this._campaigns = options.Campaigns;
        this._campaignListContainer = '#' + options.CampaignListContainer;
        this._itemFormat = [
            '<div timmcampaignid="{4}">',
            '<a href="javascript:void(0)" timmcampaignid="{3}" class="camp-center-item-edit-btn">Edit</a>',
            '<input type="checkbox" timmcampaignid="{2}" />',
            '<span timmcampaignid="{1}" class="control-center-camp-title">{0}</span>',
            '</div>'].join('');
        $(this._campaignListContainer).html('');
    },

    Show: function() {
        for (var i in this._campaigns) {
            this.AppendCampaignToList(this._campaigns[i]);

        }
    },

    EnableMultipleSelection: function(enabled) {
        if (enabled)
            $(this._campaignListContainer + ' div input[type |= "checkbox"]').show();
        else
            $(this._campaignListContainer + ' div input[type |= "checkbox"]').hide();
    },

    EnableEdit: function(enabled) {
        if (enabled)
            $(this._campaignListContainer + ' div .camp-center-item-edit-btn').show();
        else
            $(this._campaignListContainer + ' div .camp-center-item-edit-btn').hide();
    },

    AppendCampaignToList: function(camp) {
        var itemHtml = this._itemFormat
                .replace('{0}', camp.CompositeName)
                .replace('{1}', camp.Id)
                .replace('{2}', camp.Id)
                .replace('{3}', camp.Id)
                .replace('{4}', camp.Id);
        var e = $(this._campaignListContainer).append(itemHtml);

        var thisObj = this;

        // Trigger the 'list_item_check' event
        //
        $(this._campaignListContainer + ' div input[type |= "checkbox"][timmcampaignid |= "' + camp.Id + '"]').click(function() {
            var checked = $(this).attr('checked');
            thisObj.TriggerEvent('list_item_check', [camp.Id, checked]);
        });

        // Trigger the 'list_item_click' event
        //
        $(this._campaignListContainer + ' div .control-center-camp-title[timmcampaignid |= "' + camp.Id + '"]').click(function() {
            thisObj.TriggerEvent('list_item_click', camp.Id);
        });

        // Trigger the 'list_item_edit' event
        //
        $(this._campaignListContainer + ' div .camp-center-item-edit-btn[timmcampaignid |= "' + camp.Id + '"]').click(function() {
            thisObj.TriggerEvent('list_item_edit', camp.Id);
        });
    },

    RefereshItem: function(camp) {
        $(this._campaignListContainer + ' div .control-center-camp-title[timmcampaignid |= "' + camp.Id + '"]').html(camp.CompositeName);
    },

    RemoveListItems: function(campaignIdsToRemove) {
        for (var i in campaignIdsToRemove) {
            $(this._campaignListContainer + ' div[timmcampaignid |= "' + campaignIdsToRemove[i] + '"]').remove();
        }
    }
});

//---------------------------------------------TaskListView-----------------------------------------------------------------------
GPS.ControlCenter.TaskListView = GPS.EventTrigger.extend({
    init: function(options) {
        this._super();
        this._tasks = options.Tasks;
        this._taskListContainer = '#' + options.TaskListContainer;
        if (AssignAuditor || AssignGTUToDM) {
            this._itemFormat = ['{6}',
            '<div timmtaskid="{4}">',
            '<a href="javascript:void(0)" timmtaskid="{3}" class="camp-center-item-edit-btn">Edit</a>',
            '&nbsp;&nbsp;<input type="radio" name="tasklist" timmtaskid="{2}" />',
            '<span timmtaskid="{1}" class="control-center-camp-title">{0}</span>',
            '</div>'].join('');

        } else {
            this._itemFormat = ['{6}',
            '<div timmtaskid="{4}">',
            '&nbsp;&nbsp;<span timmtaskid="{1}" class="control-center-camp-title">{0}</span>',
            '</div>'].join('');
        }

        $(this._taskListContainer).html('');
    },

    Show: function() {
        var taskids = new Array();
        for (var i in this._tasks) {
            taskids[i] = this._tasks[i].Id;
        }
        var service = new TIMM.Website.CampaignServices.CampaignReaderService();
        var thisObj = this;
        service.GetCamNameByTasks(taskids, function(returnedName) {
            service.GetCurrentDate(function(dateArray) {
                for (var i in thisObj._tasks) {
                    tasktem = thisObj._tasks[i];
                    thisObj.AppendTaskToList(tasktem, returnedName[i], dateArray);
                };
            });
        });

    },

    EnableMultipleSelection: function(enabled) {
        if (enabled)
            $(this._taskListContainer + ' div input[type |= "radio"]').show();
        else
            $(this._taskListContainer + ' div input[type |= "radio"]').hide();
    },

    EnableEdit: function(enabled) {
        if (enabled)
            $(this._taskListContainer + ' div .camp-center-item-edit-btn').show();
        else
            $(this._taskListContainer + ' div .camp-center-item-edit-btn').hide();
    },

    AppendTaskToList: function(task, camTitle, date) {
        var append = "";
        //already assign gtus && monitor date should be before the date now
        //if (task.Taskgtuinfomappings != null && task.Taskgtuinfomappings.length > 0) {
            //var now = new Date();
            var tasktimes = (task.Date).split("-");
            var year = date[0];
            var month = date[1];
            var day = date[2];
            var flag = 0;
            if ((tasktimes[0] <= year) && (tasktimes[1] <= month) && (tasktimes[2] <= day)) {
                //append = "&nbsp;<a href='Monitor.aspx?id=" + task.Id + "' target='_blank'>&nbsp;&nbsp;Monitor</a>";
                //append += "&nbsp;<a onclick='openImportGtuInfo(" + task.Id + ")' href='#' onclick=')'>&nbsp;&nbsp;Import</a>";
                append = "&nbsp;<a href='taskMonitor.aspx?taskid=" + task.Id + "' target='_blank'>&nbsp;&nbsp;Monitor</a>";
                append += "&nbsp;<a onclick='openImportGtuInfo(" + task.Id + ")' href='#' onclick=')'>&nbsp;&nbsp;Import</a>";
            }
            if (tasktimes[0] < year) {
                flag = 1;
            }
            else if (tasktimes[0] == year) {
                if (tasktimes[1] < month) {
                    flag = 1;
                }
                else if (tasktimes[1] == month) {
                    if (tasktimes[2] <= day) {
                        flag = 1;
                    }
                }
            }


            if (flag==1) {
                //append = "&nbsp;<a href='Monitor.aspx?id=" + task.Id + "' target='_blank'>&nbsp;&nbsp;Monitor</a>";
                //append += "&nbsp;<a onclick='openImportGtuInfo(" + task.Id + ")' href='#' onclick=')'>&nbsp;&nbsp;Import</a>";
                append = "&nbsp;<a href='taskMonitor.aspx?taskid=" + task.Id + "' target='_blank'>&nbsp;&nbsp;Monitor</a>";
                append += "&nbsp;<a onclick='openImportGtuInfo(" + task.Id + ")' href='#' onclick=')'>&nbsp;&nbsp;Import</a>";
            }

        //}
        var itemHtml;
        if (AssignAuditor || AssignGTUToDM) {
            itemHtml = this._itemFormat
                .replace('{0}', task.Date + "-" + task.Name + append)
                .replace('{1}', task.Id)
                .replace('{2}', task.Id)
                .replace('{3}', task.Id)
                .replace('{4}', task.Id)
                .replace('{5}', task.Id)
                .replace('{6}', camTitle);
        } else {
            itemHtml = this._itemFormat
                .replace('{0}', task.Date + "-" + task.Name + append)
                .replace('{1}', task.Id)
                .replace('{3}', task.Id)
                .replace('{4}', task.Id)
                .replace('{5}', task.Id)
                .replace('{6}', camTitle);
        }
        //        var itemHtml = this._itemFormat
        //                .replace('{0}', task.Date + "-" + task.Name + append)
        //                .replace('{1}', task.Id)
        //                .replace('{2}', task.Id)
        //                .replace('{3}', task.Id)
        //                .replace('{4}', task.Id);


        var e = $(this._taskListContainer).append(itemHtml);

        var thisObj = this;

        // Trigger the 'list_item_check' event
        //
        $(this._taskListContainer + ' div input[type |= "radio"][timmtaskid |= "' + task.Id + '"]').click(function() {
            var checked = $(this).attr('checked');
            thisObj.TriggerEvent('list_item_check', [task.Id, checked]);
        });

        // Trigger the 'list_item_click' event
        //
        //        $(this._taskListContainer + ' div .control-center-camp-title[timmtaskid |= "' + task.Id + '"]').click(function() {
        //            thisObj.TriggerEvent('list_item_click', task.Id);
        //        });

        // Trigger the 'list_item_edit' event
        //
        $(this._taskListContainer + ' div .camp-center-item-edit-btn[timmtaskid |= "' + task.Id + '"]').click(function() {
            thisObj.TriggerEvent('list_item_edit', task.Id);
        });
    },

    RefereshItem: function(task) {
        $(this._taskListContainer + ' div .control-center-camp-title[timmtaskid |= "' + task.Id + '"]').html(task.Name);
    },

    RemoveListItems: function(taskIdsToRemove) {
        for (var i in taskIdsToRemove) {
            $(this._taskListContainer + ' div[timmtaskid |= "' + taskIdsToRemove[i] + '"]').remove();
        }
    }
});




//-------------------------------------------------ReportView------------------------------------------------------------------------
GPS.ControlCenter.ReportListView = GPS.EventTrigger.extend({
    init: function(options) {
        this._super();
        this._tasks = options.Tasks;
        this._taskListContainer = '#' + options.TaskListContainer;
//        if (AssignAuditor || AssignGTUToDM) {
            this._itemFormat = ['{6}',
            '<div timmtaskid="{4}">',
            '<input type="radio" id="tasklist" name="tasklist" timmtaskid="{2}" />',
            '<span timmtaskid="{1}" class="control-center-camp-title">{0}</span>',
            '&nbsp;&nbsp;<a href="ReportsTask.aspx?tid={7}" target="_blank" class="camp-center-item-edit-btn">Report</a>',
            '&nbsp;&nbsp;<a href="EditGTU.aspx?id={5}" target="_blank" class="camp-center-item-edit-btn">Review</a>',
            '</div>'].join('');

//        } else {
//            this._itemFormat = ['{6}',
//            '<div timmtaskid="{4}">',
//            '&nbsp;&nbsp;&nbsp;&nbsp;<span timmtaskid="{1}" class="control-center-camp-title">{0}</span>',
//            '&nbsp;&nbsp;<a href="EditGTU.aspx?id={5}" target="_blank" class="camp-center-item-edit-btn">Edit Points</a>',
//            '</div>'].join('');
//        }

        $(this._taskListContainer).html('');
    },

    Show: function() {
        var taskids = new Array();
        for (var i in this._tasks) {
            taskids[i] = this._tasks[i].Id;
        }
        var service = new TIMM.Website.CampaignServices.CampaignReaderService();
        var thisObj = this;
        service.GetCamNameByReports(taskids, function(returnedName) {

            for (var i in thisObj._tasks) {
                tasktem = thisObj._tasks[i];
                thisObj.AppendTaskToList(tasktem, returnedName[i]);
            }
        });

    },

    EnableMultipleSelection: function(enabled) {
        if (enabled)
            $(this._taskListContainer + ' div input[type |= "checkbox"]').show();
        else
            $(this._taskListContainer + ' div input[type |= "checkbox"]').hide();
    },

    EnableEdit: function(enabled) {
        if (enabled)
            $(this._taskListContainer + ' div .camp-center-item-edit-btn').show();
        else
            $(this._taskListContainer + ' div .camp-center-item-edit-btn').hide();
    },

    AppendTaskToList: function(task, camTitle) {
        var append = "";
        var itemHtml;
        if (AssignAuditor || AssignGTUToDM) {
            itemHtml = this._itemFormat
                .replace('{0}', task.Date + "-" + task.Name + append)
                .replace('{1}', task.Id)
                .replace('{2}', task.Id)
                .replace('{3}', task.Id)
                .replace('{4}', task.Id)
                .replace('{5}', task.Id)
                .replace('{7}', task.Id)
                .replace('{6}', camTitle);
        } else {
            itemHtml = this._itemFormat
                .replace('{0}', task.Date + "-" + task.Name + append)
                .replace('{1}', task.Id)
                .replace('{3}', task.Id)
                .replace('{4}', task.Id)
                .replace('{5}', task.Id)
                .replace('{7}', task.Id)
                .replace('{6}', camTitle);
        }
        //        var itemHtml = this._itemFormat
        //                .replace('{0}', task.Date + "-" + task.Name + append)
        //                .replace('{1}', task.Id)
        //                .replace('{2}', task.Id)
        //                .replace('{3}', task.Id)
        //                .replace('{4}', task.Id);


        var e = $(this._taskListContainer).append(itemHtml);

        var thisObj = this;

        // Trigger the 'list_item_check' event
        //
        $(this._taskListContainer + ' div input[type |= "checkbox"][timmtaskid |= "' + task.Id + '"]').click(function() {
            var checked = $(this).attr('checked');
            thisObj.TriggerEvent('list_item_check', [task.Id, checked]);
        });

        // Trigger the 'list_item_click' event
        //
        //        $(this._taskListContainer + ' div .control-center-camp-title[timmtaskid |= "' + task.Id + '"]').click(function() {
        //            thisObj.TriggerEvent('list_item_click', task.Id);
        //        });

        // Trigger the 'list_item_edit' event
        //
        //        $(this._taskListContainer + ' div .camp-center-item-edit-btn[timmtaskid |= "' + task.Id + '"]').click(function() {
        //            thisObj.TriggerEvent('list_item_edit', task.Id);
        //        });
    },

    RefereshItem: function(task) {
        $(this._taskListContainer + ' div .control-center-camp-title[timmtaskid |= "' + task.Id + '"]').html(task.Name);
    },

    RemoveListItems: function(taskIdsToRemove) {
        for (var i in taskIdsToRemove) {
            $(this._taskListContainer + ' div[timmtaskid |= "' + taskIdsToRemove[i] + '"]').remove();
        }
    }
});

//------------------------------------------------------------------------------------------------------------------------------------




//*************************************************************************************************************************************

var campaignCenter = new GPS.CampaignCenter();
var dmCenter = new GPS.DmCenter();
var productivityCenter = new GPS.ProductivityCenter();

function UpdateCampaignsList() {
    var campaignService = new TIMM.Website.CampaignServices.CampaignReaderService();

    // Get data from the server and populate the Campaigns panel
    //
    //campaignService.GetCurrentUserCampaigns(function(campaigns) {
    if (!CampaignSupervisorG && SalesG) {
        $("#publishDiv").hide();
        $("#delcampaignsepdiv").hide();
    }
    if (hasHighestPrivilege || CampaignSupervisorG) {
        campaignService.GetAllBySubmapStatusWithoutUser(function(campaigns) {
            var view = new GPS.ControlCenter.CampaignListView({
                Campaigns: campaigns,
                CampaignListContainer: 'campaign-center-campaign-list'
            });
            if (submapRole) {
            view.AttachEvent('list_item_click', function(campaignId) { campaignCenter.OnCampaignListItemClicked(campaignId); });
            view.AttachEvent('list_item_edit', function(campaignId) { campaignCenter.OnEditCampaignListItem(campaignId); });
            view.AttachEvent('list_item_check', function(checkState) { campaignCenter.OnCampaignListItemChecked(checkState); });
        }
        
        //campaignCenter.RefreshViewAfterDataPopulation();
        campaignCenter.AttachEvent('list_items_removed', function(campaignIdsToRemove) { dmCenter.OnCampaignsRemoved(campaignIdsToRemove); });
        campaignCenter.AttachEvent('list_item_updated', function(updatedCampaign) { dmCenter.OnCampaignUpdated(updatedCampaign); });
        //view.EnableEdit(true);
        // Show the list
        //
        view.Show();

        campaignCenter.SetView(view);
    });
        
    }else{
        campaignService.GetAllBySubmapStatus(function(campaigns) {
           var view = new GPS.ControlCenter.CampaignListView({
                Campaigns: campaigns,
                CampaignListContainer: 'campaign-center-campaign-list'
            });
            if (submapRole) {
                view.AttachEvent('list_item_click', function(campaignId) { campaignCenter.OnCampaignListItemClicked(campaignId); });
                view.AttachEvent('list_item_edit', function(campaignId) { campaignCenter.OnEditCampaignListItem(campaignId); });
                view.AttachEvent('list_item_check', function(checkState) { campaignCenter.OnCampaignListItemChecked(checkState); });
            }

            //campaignCenter.RefreshViewAfterDataPopulation();
            campaignCenter.AttachEvent('list_items_removed', function(campaignIdsToRemove) { dmCenter.OnCampaignsRemoved(campaignIdsToRemove); });
            campaignCenter.AttachEvent('list_item_updated', function(updatedCampaign) { dmCenter.OnCampaignUpdated(updatedCampaign); });
            //view.EnableEdit(true);
            // Show the list
            //
            view.Show();

            campaignCenter.SetView(view);
        });
        
    }        

    $('#campaign-center-loading').show();
    if (SubmapView || SubmapDMView) {
        $('#campaign-center-campaign-list').show();
    }
}


function UpdateDmCenterCampaignsList() {
    var campaignService = new TIMM.Website.CampaignServices.CampaignReaderService();

    // Get data from the server and populate the Distribution Maps panel
    //
    //campaignService.GetCurrentUserCampaignsForDistribution(function(campaigns) {
    if (hasHighestPrivilege || distributionSupervisorG) {
        campaignService.GetAllCampByDMStatusWithoutUser(function(campaigns) {
            var view = new GPS.ControlCenter.CampaignListView({
                Campaigns: campaigns,
                CampaignListContainer: 'dm-center-campaign-list'
            });

            if (dmapRole) {
                view.AttachEvent('list_item_click', function(campaignId) { dmCenter.OnCampaignListItemClicked(campaignId); });
                view.AttachEvent('list_item_check', function(checkState) { dmCenter.OnCampaignListItemChecked(checkState); });
            }

            //dmCenter.RefreshViewAfterDataPopulation();

            // Show the list
            //
            view.Show();

            // Hide check boxes
            //
            //view.EnableMultipleSelection(false);

            // Hide the 'Edit' buttons
            //
            view.EnableEdit(false);

            dmCenter.SetView(view);
        });

        $('#dm-center-loading').show();
    
    }else{
        campaignService.GetAllByDMStatus(function(campaigns) {
            var view = new GPS.ControlCenter.CampaignListView({
                Campaigns: campaigns,
                CampaignListContainer: 'dm-center-campaign-list'
            });

            if (dmapRole) {
                view.AttachEvent('list_item_click', function(campaignId) { dmCenter.OnCampaignListItemClicked(campaignId); });
                view.AttachEvent('list_item_check', function(checkState) { dmCenter.OnCampaignListItemChecked(checkState); });
            }

            //dmCenter.RefreshViewAfterDataPopulation();

            // Show the list
            //
            view.Show();

            // Hide check boxes
            //
            //view.EnableMultipleSelection(false);

            // Hide the 'Edit' buttons
            //
            view.EnableEdit(false);

            dmCenter.SetView(view);
        });

        $('#dm-center-loading').show();
    }

}


function UpdateTasksList() {
    var taskService = new TIMM.Website.TaskServices.TaskReaderService();
    taskService.GetAllTasks(function(tasks) {
        var view = new GPS.ControlCenter.TaskListView({
            Tasks: tasks,
            TaskListContainer: 'productivity-center-campaign-list'
        });

        //view.AttachEvent('list_item_click', function(campaignId) { campaignCenter.OnCampaignListItemClicked(campaignId); });
        view.AttachEvent('list_item_edit', function(taskId) { productivityCenter.OnEditTaskListItem(taskId); });
        view.AttachEvent('list_item_check', function(checkState) { productivityCenter.OnTaskListItemChecked(checkState); });

        productivityCenter.AttachEvent('list_items_removed', function(campaignIdsToRemove) { productivityCenter.OnTasksRemoved(campaignIdsToRemove); });
        productivityCenter.AttachEvent('list_item_updated', function(updatedCampaign) { productivityCenter.OnTaskUpdated(updatedCampaign); });
        view.Show();

        productivityCenter.SetView(view);
    });
}



function UpdateReportsList() {
    var taskService = new TIMM.Website.TaskServices.TaskReaderService();
    taskService.GetAllReports(function(tasks) {
        var view = new GPS.ControlCenter.ReportListView({
            Tasks: tasks,
            TaskListContainer: 'campaign-center-report-list'
        });

        //view.AttachEvent('list_item_click', function(campaignId) { campaignCenter.OnCampaignListItemClicked(campaignId); });
        //view.AttachEvent('list_item_edit', function(campaignId) { campaignCenter.OnEditCampaignListItem(campaignId); });
        view.AttachEvent('list_item_check', function(checkState) { productivityCenter.OnTaskListItemChecked(checkState); });

        productivityCenter.AttachEvent('list_items_removed', function(campaignIdsToRemove) { productivityCenter.OnTasksRemoved(campaignIdsToRemove); });
        productivityCenter.AttachEvent('list_item_updated', function(updatedCampaign) { productivityCenter.OnTaskUpdated(updatedCampaign); });
        view.Show();

        productivityCenter.SetView(view);

        // set visible of 
        if (document.getElementById("divDismissToMonitor") != null) {
            if (tasks.length > 0)
                document.getElementById("divDismissToMonitor").style.display = "";
            else
                document.getElementById("divDismissToMonitor").style.display = "none";
        }
        
    });
}


//*************************************************************************************************************************************************

var submapRole, dmapRole, userRole;
submapRole = dmapRole = userRole = false;

$(document).ready(function() {
    //$("#newcampaignsepdiv").show();
    //    $("#div-campaign").attr("disabled", "disabled");
    //    $("#div-distribution-map-center").attr("disabled", "disabled");
    //    $("#div-Admin").attr("disabled", "disabled");

    //get the privilege for login user
    var userService = new TIMM.Website.UserServices.UserReaderService();
    userService.GetCurrentUser(function(currentUser) {
    $("#div-cclogo-title").html(currentUser.FullName + ",&nbsp;&nbsp;" +  $("#div-cclogo-title").html());

        if (currentUser.Groups) {
            var length = currentUser.Groups.length;
            for (var i = 0; i < length; i++) {
                if (currentUser.Groups[i].Privileges) {
                    var len = currentUser.Groups[i].Privileges.length;
                    for (var j = 0; j < len; j++) {
                        if (currentUser.Groups[i].Privileges[j].Value == 1) {
                            CreateCampaign = true;
                            //$("#div-campaign").show();
                            $("#newcampaignDiv").show();
                            $("#newcampaignsepdiv").show();
                            $("#deletecampaignDiv").show();
                            $("#delcampaignsepdiv").show();
                            $("#cpycampaignDiv").show();
                            $("#cpycampaignsepdiv").show();
                            $("#campaign-center-campaign-list").show();
                        }
                        if (currentUser.Groups[i].Privileges[j].Value == 2) {
                            SubmapView = true;
                            //$("#div-campaign").show();
                            $("#campaign-center-campaign-list").show();
                        }
                        if (currentUser.Groups[i].Privileges[j].Value == 3) {
                            HistoricalData = true;
                            $("#productivity-center-campaign-list").show();
                            $("#campaign-center-report-list").show();
                        }
                        if (currentUser.Groups[i].Privileges[j].Value == 4) {
                            PublishCampaign = true;
                            $("#publishDiv").show();
                        }
                        if (currentUser.Groups[i].Privileges[j].Value == 5) {
                            RealTimeWalkerLocation = true;
                            $("#productivity-center-campaign-list").show();
                            //$("#campaign-center-toolbar-dm").show();
                        }
                        if (currentUser.Groups[i].Privileges[j].Value == 6) {
                            AssignNameToGTU = true;
                            //$("#div-Admin").show();
                            $("#GTUManagementDiv").show();
                        }
                        if (currentUser.Groups[i].Privileges[j].Value == 7) {
                            StartSuspendStopGTU = true;
                        }
                        if (currentUser.Groups[i].Privileges[j].Value == 8) {
                            NotifiedByEmail = true;
                        }
                        if (currentUser.Groups[i].Privileges[j].Value == 9) {
                            AssignGTUToDM = true;
                            $("#assigngtuA").show();
                        }
                        if (currentUser.Groups[i].Privileges[j].Value == 10) {
                            AssignAuditor = true;
                            $("#assigngtuSep").show();
                            //$("#tasknewSep").show();
                            //$("#tasknewA").show();
                            $("#taskdeleteA").show();
                            $("#markfinishsepdiv").show();
                            $("#markfinish").show();
                        }
                        if (currentUser.Groups[i].Privileges[j].Value == 11) {
                            GeneratePdf = true;
                        }
                        if (currentUser.Groups[i].Privileges[j].Value == 12) {
                            CreateDriverClientAuditorAccount = true;
                            //$("#div-Admin").show();
                            $("#UserManagementDiv").show();
                        }
                        if (currentUser.Groups[i].Privileges[j].Value == 13) {
                            DNDManagement = true;
                            //$("#div-Admin").show();
                            $("#NonDeliverablesDiv").show();
                        }
                        if (currentUser.Groups[i].Privileges[j].Value == 14) {
                            SubmapDMView = true;
                            //$("#div-campaign").attr("disabled", "");
                            //$("#div-campaign").show();
                            //$("#div-distribution-map-center").show();
                            $("#dm-center-campaign-list").show();
                            $("#campaign-center-campaign-list").show();
                        }
                        if (currentUser.Groups[i].Privileges[j].Value == 15) {
                            AssignManager = true;
                        }
                        if (currentUser.Groups[i].Privileges[j].Value == 16) {
                            GTUManagement = true;
                            //$("#div-Admin").show();
                            $("#GTUManagementDiv").show();
                        }
                        if (currentUser.Groups[i].Privileges[j].Value == 17) {
                            CreateDriverClientAuditorManagerAccount = true;
                            //$("#div-Admin").show();
                            $("#UserManagementDiv").show();
                        }
                        if (currentUser.Groups[i].Privileges[j].Value == 18) {
                            CreateAllTypeUserAccount = true;
                            //$("#div-Admin").show();
                            $("#UserManagementDiv").show();
                        }
                        if (currentUser.Groups[i].Privileges[j].Value == 19) {
                            DMView = true;
                            //$("#div-distribution-map-center").show();
                            $("#dm-center-campaign-list").show();
                        }
                        if (currentUser.Groups[i].Privileges[j].Value == 20) {
                            ReportOfDM = true;
                        }
                        if (currentUser.Groups[i].Privileges[j].Value == 21) {
                            UndoPublishCampaign = true;
                            $("#campaign-center-toolbar-dm").show();
                            //                            $("dismissDiv").show();
                            //                            $("publishdmDiv").show();
                            //                            $("publishmonitorsepdiv").show();
                        }
                    }
                }
            }
            if (length > 0) {
                for (var g = 0; g < length; g++) {
                    //Administrator-----53
                    if (currentUser.Groups[g].Id == 53) {
                        hasHighestPrivilege = true;
                        $("#GTUAvailableDiv").show();
                        break;
                    }
                }
                for (var g = 0; g < length; g++) {
                    //CampaignSupervisor-----47
                    if (currentUser.Groups[g].Id == 47) {
                        CampaignSupervisorG = true;
                        break;
                    }
                }
                for (var g = 0; g < length; g++) {
                    //Sales-----46
                    if (currentUser.Groups[g].Id == 46) {
                        //$("#publishDiv").hide();
                        //$("#delcampaignsepdiv").hide();
                        SalesG = true;
                        break;
                    }
                }
                for (var g = 0; g < length; g++) {
                    //Distribution Supervisor-----52
                    if (currentUser.Groups[g].Id == 52) {
                        //$("#publishDiv").hide();
                        //$("#delcampaignsepdiv").hide();
                        distributionSupervisorG = true;
                        break;
                    }
                }
            }
        }

        if (SubmapView || SubmapDMView || CreateCampaign || PublishCampaign || GeneratePdf) {
            submapRole = true;
        }
        if (DMView || SubmapDMView || GeneratePdf || UndoPublishCampaign || GeneratePdf || AssignManager) {
            dmapRole = true;
        }
        if (CreateDriverClientAuditorAccount || CreateDriverClientAuditorManagerAccount || CreateAllTypeUserAccount || AssignNameToGTU || DNDManagement || GTUManagement) {
            userRole = true;
        }
        //for test----
        //        $("#div-Admin").attr("disabled", "");
        //        $("#div-distribution-map-center").attr("disabled", "");
        //        $("#div-campaign").attr("disabled", "");
        //        $("#GTUManagementDiv").attr("disabled", "");
        //        $("#NonDeliverablesDiv").attr("disabled", "");
        //        $("#UserManagementDiv").attr("disabled", "");
        //        userRole = true;
        //        dmapRole = true;
        //        submapRole = true;

        //---

        // Original entrance;
        UpdateCampaignsList();
        UpdateDmCenterCampaignsList();
        UpdateTasksList();
        UpdateReportsList();
    });

    //show campaign list
    var campaignsService = new TIMM.Website.CampaignServices.CampaignReaderService();
    //    campaignsService.GetAllCampByDMStatusWithoutUser(function(camlist) {
    //        $('#sel-task-campaigns').empty();
    //        $('#sel-task-campaigns').append("<option value=''>----------------------------Select----------------------------</option>");
    //        $('#sel-task-submaps').empty();
    //        $('#sel-task-submaps').append("<option value=''>----------------------------Select----------------------------</option>");
    //        $('#sel-task-dms').empty();
    //        $('#sel-task-dms').append("<option value=''>----------------------------Select----------------------------</option>");
    //        for (var i in camlist) {
    //            $('#sel-task-campaigns').append("<option value='" + camlist[i].Id + "'>" + camlist[i].CompositeName + "</option>");
    //        }

    //    });

    //show auditor list
    var usersService = new TIMM.Website.UserServices.UserReaderService();
    usersService.GetAllUsers(function(userlist) {
        $('#select-task-auditor').empty();
        $('#select-task-auditor').append("<option value=''>----------------------------Select----------------------------</option>");
        for (var i in userlist) {
            $('#select-task-auditor').append("<option value='" + userlist[i].Id + "'>" + userlist[i].UserName + "</option>");
        }

    });
});


function openImportGtuInfo(taskID) {
    var btnImport = getSubElement("divImport", "btnImport");
    var btnImportID = new String(btnImport.id);

    var modalID = btnImportID.replace("btnImport", "ajaxModalEx");
    var txtGtuNumberID = btnImportID.replace("btnImport", "txtTaskID");
    document.getElementById(txtGtuNumberID).value = taskID;
    $find(modalID).show();
}

function onOk() {
    var btnImport = getSubElement("divImport", "btnImport");
    var btnImportID = new String(btnImport.id);

    var fileUploadID = btnImportID.replace("btnImport", "FileUpload1");
    var inputFile = new String(document.getElementById(fileUploadID).value);

    if (inputFile.indexOf(".txt") < 0
                && inputFile.indexOf(".csv") < 0
                && inputFile.indexOf(".xls") < 0
            ) {
        alert("Please choose a valid text file");
        return false;
    }

    btnImport.click();
}

function getSubElement(parentId, ChildId) {
    var parentElement = document.getElementById(parentId);
    for (var i = 0; i < parentElement.childNodes.length; i++) {
        var sElementId = new String(parentElement.childNodes[i].id);
        if (sElementId.indexOf(ChildId) >= 0)
            return parentElement.childNodes[i];
    }
    return null;
}