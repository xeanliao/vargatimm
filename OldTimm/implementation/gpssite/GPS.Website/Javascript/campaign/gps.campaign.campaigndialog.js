/*
* This file is the implementation of dialogs used to create or modify campaign properties.
*/

// GPS.Campaign.CampaignDialog is the base class of dialogs used to create or edit the 
// properties of a campaign.
GPS.Campaign.CampaignDialog = GPS.EventTrigger.extend({
    // Initialize the state of this instance.
    //
    // This method is called when the object is constructed.
    //
    // The @options object should have the following properties:
    // @DialogElement - the element used as the container of the dialog.
    // @DialogTitle - the title of the dialog.
    init: function(options) {
        this._super(options);

        this._dialogElement = options.DialogElement;
        this._dialogTitle = options.DialogTitle || '';

        // The jQuery dialog object
        this._dialog = null;
    },

    // Show the dialog.
    Show: function() {
        if (!this._dialog) {
            this._dialog = this._ConstructDialog();
        }

        this._dialog.dialog("open");
        this._OnShow();
    },

    // Construct and return the dialog object.
    _ConstructDialog: function() {
        var thisObj = this;

        return $("#" + this._dialogElement).dialog({
            autoOpen: false,
            title: thisObj._dialogTitle,
            width: 400,
            modal: true,
            resizable: false,
            close: function() {
                thisObj._OnDialogClose();
            },
            overlay: {
                opacity: 0.5,
                background: "black"
            },
            buttons: {
                "Cancel": function() {
                    if (thisObj._OnBeforeCancel()) {
                        $(this).dialog("destroy");
                        thisObj._OnCancel();
                    }
                },
                "Save": function() {
                    if (thisObj._OnBeforeOk()) {
                        $(this).dialog("destroy");
                        thisObj._OnOk();
                    }
                }
            }
        });
    },

    // Called when the user clicks on the 'x' button on the dialog.
    _OnDialogClose: function() {
    },

    // Return a boolean value indicating whether the dialog Cancel action should continue.
    _OnBeforeCancel: function() {
        return true;
    },

    // Called after the Cancel button is clicked and the dialog is closed.
    _OnCancel: function() {
    },

    // Return a boolean value indicating whether the dialog Ok action should continue.
    _OnBeforeOk: function() {
        return true;
    },

    // Called after the Ok button is clicked and the dialog is closed.
    _OnOk: function() {
    },

    // Called after the dialog is shown.
    _OnShow: function() {
    }
});

// GPS.Campaign.EditCampaignDialog is used for the user to modify the 
// properties of a campaign.
GPS.Campaign.EditCampaignDialog = GPS.Campaign.CampaignDialog.extend({
    // Initialize the state of this instance.
    //
    // This method is called when the object is constructed.
    //
    // The @options object should have the properties specified in the super
    // class and the following extended properties:
    //     @Campaign - an object containning the following properties:
    //         @Id - the identity of this campaign
    //         @ClientName - the client name
    //         @ContactName - the contact name
    //         @ClientCode - the client code
    //         @AreaDescription - the area description
    //         @Date - the campaign creation date
    //         @Sequence - the sequence of this campaign
    init: function(options) {
        this._super(options);

        // The internal campaign object being edited
        this._campaign = {
            Id: options.Campaign.Id,
            ClientName: options.Campaign.ClientName,
            ContactName: options.Campaign.ContactName,
            ClientCode: options.Campaign.ClientCode,
            AreaDescription: options.Campaign.AreaDescription,
            Date: options.Campaign.Date,
            Sequence: options.Campaign.Sequence
        }

        // Fields
        this._txtClientName = null;
        this._txtContactName = null;
        this._txtClientCode = null;
        this._campaignLogoContainer = null;
        this._txtAreaDescription = null;
        this._datePicker = null;
        this._sequenceNoContainer = null;
        this._txtSequenceNo = null;
    },

    // Override the method in the super class.
    //
    // Get input controls.
    _ConstructDialog: function() {
        // Construct the dialog
        var dialog = this._super();

        // Get input controls for later use
        this._txtClientName = $('#txt-client-name');
        this._txtContactName = $('#txt-contact-name');
        this._txtClientCode = $('#txt-client-code');
        this._campaignLogoContainer = $('#campaign-logo-container');
        this._campaignLogoContainer.hide();
        this._txtAreaDescription = $('#txt-area-description');
        this._datePicker = $('#campaign-creation-date').datepicker();
        this._sequenceNoContainer = $('#sequence-no-container');
        this._sequenceNoContainer.show();
        this._txtSequenceNo = $('#txt-sequence-no');

        return dialog;
    },

    // Override the method in the super class.
    //
    // Clear field values.
    _OnDialogClose: function() {
        this._super();
        this._ClearControlValues();
    },

    // Override the method in the super class.
    //
    // Show campaign properties on the dialog.
    _OnShow: function() {
        this._super();
        this._ShowCampaignProperties();
    },

    // Show campaign properties on the dialog.
    _ShowCampaignProperties: function() {
        this._txtClientName.val(this._campaign.ClientName);
        this._txtContactName.val(this._campaign.ContactName);
        this._txtClientCode.val(this._campaign.ClientCode);
        this._txtAreaDescription.val(this._campaign.AreaDescription);
        this._datePicker.datepicker('setDate', this._campaign.Date);
        this._txtSequenceNo.val(this._campaign.Sequence);
    },

    // Override the method in the super class.
    //
    // Clear field values.
    _OnCancel: function() {
        this._super();
        this._ClearControlValues();
    },

    // Override the method in the super class.
    //
    // Clear field values.
    _OnOk: function() {
        this._super();

        // Get control values
        var id = this._campaign.Id;
        var clientName = this._txtClientName.val();
        var contactName = this._txtContactName.val();
        var clientCode = this._txtClientCode.val();
        var areaDescription = this._txtAreaDescription.val();
        var creationDate = this._datePicker.datepicker('getDate');
        var sequenceNo = Number(this._txtSequenceNo.val());

        // If the new sequence number is valid, then save the new properties to the server.
        var options = {
            Id: id,
            ClientName: clientName,
            ContactName: contactName,
            ClientCode: clientCode,
            AreaDescription: areaDescription,
            Date: Date.clientFormatToServerFormat(creationDate),
            Sequence: sequenceNo
        }

        var thisObj = this;
        var service = new TIMM.Website.CampaignServices.CampaignWriterService();
        service.IsValidSequence(options, function(result) {
            if (result) {
                service.UpdateCampaignProperties(options, function() {
                    // Fire an 'onendsave' event
                    options.Date = creationDate;
                    thisObj.TriggerEvent("onendsave", options);
                }, function() {
                    GPSAlert('Invalid sequence number, please try another.');
                }, null);
            } else {
                GPSAlert('Invalid sequence number, please try another.');
            }
        }, function() {
            GPSAlert('Internal error.');
        }, null);

        // Clear control values
        this._ClearControlValues();
    },

    // Clear field values and hide related controls.
    _ClearControlValues: function() {
        this._txtClientName.val('');
        this._txtContactName.val('');
        this._txtClientCode.val('');
        this._campaignLogoContainer.hide();
        this._txtAreaDescription.val('');
        this._datePicker.datepicker('setDate', new Date());
        this._sequenceNoContainer.hide();
        this._txtSequenceNo.val('');
    }
});
