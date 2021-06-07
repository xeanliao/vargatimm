// GPS.Nd.NdAreaDialog represents a dialog used to edit the properties of
// an non-deliverable area.
GPS.Nd.NdAreaDialog = GPS.EventTrigger.extend({
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
            modal: true,
            resizable: false,
            overlay: {
                opacity: 0.5,
                background: "black"
            },
            buttons: {
                "Cancel": function() {
                    if (thisObj._OnBeforeCancel()) {
                        $(this).dialog("close");
                        thisObj._OnCancel();
                    }
                },
                "Ok": function() {
                    if (thisObj._OnBeforeOk()) {
                        $(this).dialog("close");
                        thisObj._OnOk();
                    }
                }
            }
        });
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

// GPS.Nd.NdBasicAreaDialog represents a basic dialog with three fields used to edit
// the properties of a non-deliverable area. The three fields include:
//     - a text field used to uniquely identify the area
//     - a number field used to represents the H/H of this area
//     - a text field used to add some notes to this area
GPS.Nd.NdBasicAreaDialog = GPS.Nd.NdAreaDialog.extend({
    // Initialize the state of this instance.
    //
    // This method is called when the object is constructed.
    //
    // The @options object should have the properties specified in the super class.
    init: function(options) {
        this._super(options);

        // Input controls
        this._txtCode = null;
        this._txtHh = null;
        this._txtDescription = null;
        this._labelCodeError = null;
        this._labelHhError = null;
    },

    // Override the method in the super class.
    //
    // Validate the input values. Return true if all values are valid, and false if not.
    _OnBeforeOk: function() {
        if (!this._super()) {
            return false;
        }

        // Validate Code
        var error = (this._txtCode.val() == '') ? 'Required' : '';
        this._labelCodeError.text(error);
        if (error != '') {
            return false;
        }

        // Validate H/H
        error = /^\d+(\.\d+)?$/.test(this._txtHh.val()) ? '' : 'Number required';
        this._labelHhError.text(error);
        if (error != '') {
            return false;
        }

        return true;
    },

    // Override the method in the super class.
    //
    // Clean control values.
    _OnShow: function() {
        this._super();
        this._CleanControlValues();
    },

    // Clean control values.
    _CleanControlValues: function() {
        this._txtCode.val('');
        this._txtHh.val('');
        this._txtDescription.val('');

        this._labelCodeError.text('');
        this._labelHhError.text('');
    }
});

// GPS.Nd.Nd5ZipAreaDialog represents a dialog used to edit the properties of
// an non-deliverable 5 digit zip area.
GPS.Nd.Nd5ZipAreaDialog = GPS.Nd.NdBasicAreaDialog.extend({
    // Initialize the state of this instance.
    //
    // This method is called when the object is constructed.
    //
    // The @options object should have the properties specified in the super class.
    init: function(options) {
        this._super(options);
    },

    // Override the method in the super class.
    //
    // Get input controls.
    _ConstructDialog: function() {
        // Construct the dialog
        var dialog = this._super();

        // Get input controls for later use
        this._txtCode = $('#nd-txt-5zip-code');
        this._txtHh = $('#nd-txt-5zip-hh');
        this._txtDescription = $('#nd-txt-5zip-description');
        this._labelCodeError = $('#nd-error-5zip-code');
        this._labelHhError = $('#nd-error-5zip-hh');

        return dialog;
    },

    // Override the method in the super class.
    //
    // Check if the specified 5 digit zip exists. If yes, fire an 'onendsave' event.
    _OnOk: function() {
        // Get input values
        var code = this._txtCode.val();
        var total = Number(this._txtHh.val());
        var description = this._txtDescription.val();

        // Check if the specified 5 digit zip exists
        var service = new TIMM.Website.AreaServices.AreaReaderService();
        var thisObj = this;
        service.GetFiveZipByCode(code, function(areas) {
            if (areas.length > 0) {
                thisObj.TriggerEvent("onendsave", {
                    Code: code,
                    Total: total,
                    Description: description,
                    Enabled: false,
                    Location: new VELatLong(areas[0].Location[0], areas[0].Location[1])
                });
            }
            else {
                GPSAlert("The area you specified does not exist.");
            }
        });
    }
});

// GPS.Nd.NdCustomAreaDialog represents a dialog used to edit the properties of
// an non-deliverable custom area.
GPS.Nd.NdCustomAreaDialog = GPS.Nd.NdBasicAreaDialog.extend({
    // Initialize the state of this instance.
    //
    // This method is called when the object is constructed.
    //
    // The @options object should have the properties specified in the super class.
    init: function(options) {
        this._super(options);

        this._customPoints = options.CustomPoints;
    },

    // Override the method in the super class.
    //
    // Get input controls.
    _ConstructDialog: function() {
        // Construct the dialog
        var dialog = this._super();

        // Get input controls for later use
        this._txtCode = $('#txtcaName');
        this._txtHh = $('#txtcaTotal');
        this._txtDescription = $('#txtcaDescription');
        this._labelCodeError = $('#valcaName');
        this._labelHhError = $('#valcaTotal');

        return dialog;
    },

    // Override the method in the super class.
    //
    // Save the custom are to the server, and fire an 'onendsave' event if successful.
    _OnOk: function() {
        // Get input values
        var code = this._txtCode.val();
        var total = Number(this._txtHh.val());
        var description = this._txtDescription.val();

        // Invoke the service to save the custom area to the server
        var service = new TIMM.Website.AreaServices.AreaWriterService();
        var thisObj = this;
        service.AddCustomArea(code, total, description, this._customPoints, function(ret) {
            if (ret.IsSuccess) {
                thisObj.TriggerEvent("onendsave", {
                    Id: ret.Message,
                    Name: code,
                    Attributes: { OTotal: total },
                    Description: description,
                    Enabled: false
                });
            } else {
                GPSAlert("There's already another custom area with the same name. Consider using a different one.");
            }
        });
    }
});

// GPS.Nd.NdTractAreaDialog represents a dialog used to edit the properties of
// an non-deliverable Census Tract area.
GPS.Nd.NdTractAreaDialog = GPS.Nd.NdAreaDialog.extend({
    // Initialize the state of this instance.
    //
    // This method is called when the object is constructed.
    //
    // The @options object should have the properties specified in the super class.
    init: function(options) {
        this._super(options);

        // Input controls
        this._txtStateCode = null;
        this._txtCountyCode = null;
        this._txtTractCode = null;
        this._txtHh = null;
        this._txtDescription = null;

        this._labelStateCodeError = null;
        this._labelCountyCodeError = null;
        this._labelTractCodeError = null;
        this._labelTractHhError = null;
    },

    // Override the method in the super class.
    //
    // Validate the input values. Return true if all values are valid, and false if not.
    _OnBeforeOk: function() {
        if (!this._super()) {
            return false;
        }

        // Validate State Code
        var error = (this._txtStateCode.val() == '') ? 'Required' : '';
        this._labelStateCodeError.text(error);
        if (error != '') {
            return false;
        }

        // Validate County Code
        error = (this._txtCountyCode.val() == '') ? 'Required' : '';
        this._labelCountyCodeError.text(error);
        if (error != '') {
            return false;
        }

        // Validate Tract Code
        error = (this._txtTractCode.val() == '') ? 'Required' : '';
        this._labelTractCodeError.text(error);
        if (error != '') {
            return false;
        }

        if (!this._ValidateAfterTractCode()) {
            return false;
        }

        return true;
    },

    // Override the method in the super class.
    //
    // Clean control values.
    _OnShow: function() {
        this._super();
        this._CleanControlValues();
    },

    // Override the method in the super class.
    //
    // Get input controls.
    _ConstructDialog: function() {
        // Construct the dialog
        var dialog = this._super();

        // Get input controls for later use
        this._txtStateCode = $('#nd-txt-tract-state-code');
        this._txtCountyCode = $('#nd-txt-tract-county-code');
        this._txtTractCode = $('#nd-txt-tract-code');
        this._txtHh = $('#nd-txt-tract-hh');
        this._txtDescription = $('#nd-txt-tract-description');

        this._labelStateCodeError = $('#nd-error-tract-state-code');
        this._labelCountyCodeError = $('#nd-error-tract-county-code');
        this._labelTractCodeError = $('#nd-error-tract-code');
        this._labelTractHhError = $('#nd-error-tract-hh');

        return dialog;
    },

    // Clean control values.
    _CleanControlValues: function() {
        this._txtStateCode.val('');
        this._txtCountyCode.val('');
        this._txtTractCode.val('');
        this._txtHh.val('');
        this._txtDescription.val('');

        this._labelStateCodeError.text('');
        this._labelCountyCodeError.text('');
        this._labelTractCodeError.text('');
        this._labelTractHhError.text('');
    },

    // Validate values after the Tract Code field
    _ValidateAfterTractCode: function() {
        // Validate H/H
        var error = /^\d+(\.\d+)?$/.test(this._txtHh.val()) ? '' : 'Number required';
        this._labelTractHhError.text(error);
        return (error == '');
    },

    // Override the method in the super class.
    //
    // Check if the specified Census Tract exists. If yes, fire an 'onendsave' event.
    _OnOk: function() {
        // Get input values
        var stateCode = this._txtStateCode.val();
        var countyCode = this._txtCountyCode.val();
        var tractCode = this._txtTractCode.val();
        var total = Number(this._txtHh.val());
        var description = this._txtDescription.val();

        // Check if the specified Census Tract exist
        var thisObj = this;
        var service = new TIMM.Website.AreaServices.AreaReaderService();

        service.GetTracts({
            StateCode: stateCode,
            CountyCode: countyCode,
            TractCode: tractCode
        }, function(areas) {
            // If the specified Census Tract exists, fire an 'onendsave' event.
            if (areas.length > 0) {
                thisObj.TriggerEvent("onendsave", {
                    StateCode: stateCode,
                    CountyCode: countyCode,
                    Code: tractCode,
                    Total: Number(total),
                    Description: description,
                    Enabled: false,
                    Location: new VELatLong(areas[0].Location[0], areas[0].Location[1])
                });
            } else {
                GPSAlert("The area you specified does not exist.");
            }
        }, function(result) {
        }, null);
    }
});

// GPS.Nd.NdBgAreaDialog represents a dialog used to edit the properties of
// an non-deliverable Census Block Group area.
GPS.Nd.NdBgAreaDialog = GPS.Nd.NdTractAreaDialog.extend({
    // Initialize the state of this instance.
    //
    // This method is called when the object is constructed.
    //
    // The @options object should have the properties specified in the super class.
    init: function(options) {
        this._super(options);

        this._txtBgCode = null;
        this._labelBgCodeError = null;
    },

    // Override the method in the super class.
    //
    // Get input controls.
    _ConstructDialog: function() {
        // Construct the dialog
        var dialog = this._super();

        // Get input controls for later use
        this._txtStateCode = $('#nd-txt-bg-state-code');
        this._txtCountyCode = $('#nd-txt-bg-county-code');
        this._txtTractCode = $('#nd-txt-bg-tract-code');
        this._txtBgCode = $('#nd-txt-bg-code');
        this._txtHh = $('#nd-txt-bg-hh');
        this._txtDescription = $('#nd-txt-bg-description');

        this._labelStateCodeError = $('#nd-error-bg-state-code');
        this._labelCountyCodeError = $('#nd-error-bg-county-code');
        this._labelTractCodeError = $('#nd-error-bg-tract-code');
        this._labelBgCodeError = $('#nd-error-bg-code');
        this._labelTractHhError = $('#nd-error-bg-hh');

        return dialog;
    },

    // Clean control values.
    _CleanControlValues: function() {
        this._super();

        this._txtBgCode.val('');
        this._labelBgCodeError.val('');
    },

    // Override the method in the super class.
    //
    // Validate Bg Code just after Tract Code is validated.
    _ValidateAfterTractCode: function() {
        // Validate Bg Code
        var error = (this._txtBgCode.val() == '') ? 'Required' : '';
        this._labelBgCodeError.text(error);
        if (error != '') {
            return false;
        }

        if (!this._super()) {
            return false;
        }

        return true;
    },

    // Override the method in the super class.
    //
    // Check if the specified Census Block Group exists. If yes, fire an 'onendsave' event.
    _OnOk: function() {
        // Get input values
        var stateCode = this._txtStateCode.val();
        var countyCode = this._txtCountyCode.val();
        var tractCode = this._txtTractCode.val();
        var bgCode = this._txtBgCode.val();
        var total = Number(this._txtHh.val());
        var description = this._txtDescription.val();

        // Check if the specified Census Block Group exist        
        var thisObj = this;
        var service = new TIMM.Website.AreaServices.AreaReaderService();

        service.GetBlockGroups({
            StateCode: stateCode,
            CountyCode: countyCode,
            TractCode: tractCode,
            BgCode: bgCode
        }, function(areas) {
            // If the specified Census Block Group exists, fire an 'onendsave' event.
            if (areas.length > 0) {
                thisObj.TriggerEvent("onendsave", {
                    StateCode: stateCode,
                    CountyCode: countyCode,
                    TractCode: tractCode,
                    Code: bgCode,
                    Total: Number(total),
                    Description: description,
                    Enabled: false,
                    Location: new VELatLong(areas[0].Location[0], areas[0].Location[1])
                });
            } else {
                GPSAlert("The area you specified does not exist.");
            }
        }, function(result) {
        }, null);
    }
});

// GPS.Nd.NdAddressDialog represents a dialog used to edit the properties of
// an non-deliverable address.
GPS.Nd.NdAddressDialog = GPS.Nd.NdAreaDialog.extend({
    // Initialize the state of this instance.
    //
    // This method is called when the object is constructed.
    //
    // The @options object should have the properties specified in the super class.
    init: function(options) {
        this._super(options);
        this._txtStreet = null;
        this._txtZipCode = null;
        this._txtGeofence = null;
        this._txtDescription = null;
        this._labelStreetError = null;
        this._labelZipCodeError = null;
        this._labelGeofenceError = null;
    },
    // Override the method in the super class.
    //
    // Get input controls.
    _ConstructDialog: function() {
        // Construct the dialog
        var dialog = this._super();

        // Get input controls for later use
        this._txtStreet = $('#nd-txt-address-street');
        this._txtZipCode = $('#nd-txt-address-zipcode');
        this._txtGeofence = $('#nd-txt-address-geofence');
        this._txtDescription = $('#nd-txt-address-description');

        this._labelStreetError = $('#nd-error-address-street');
        this._labelZipCodeError = $('#nd-error-address-zipcode');
        this._labelGeofenceError = $('#nd-error-address-geofence');

        return dialog;
    },
    // Clean control values.
    _OnShow: function() {
        this._txtStreet.val('');
        this._txtZipCode.val('');
        this._txtGeofence.val('500');
        this._txtDescription.val('');
        this._labelStreetError.val('');
        this._labelZipCodeError.val('');
        this._labelGeofenceError.val('');
    },
    // Override the method in the super class.
    //
    // Validate Address
    _OnBeforeOk: function() {
        // Validate Street
        var error = (this._txtStreet.val() == '') ? 'Required' : '';
        this._labelStreetError.text(error);
        if (error != '') {
            return false;
        }

        // Validate Zip code
        var error = (this._txtZipCode.val() == '') ? 'Required' : '';
        this._labelZipCodeError.text(error);
        if (error != '') {
            return false;
        }

        // Validate Geofence
        var error = // Validate 
        error = /^\d+(\.\d+)?$/.test(this._txtGeofence.val()) ? '' : 'Number required';
        this._labelGeofenceError.text(error);
        if (error != '') {
            return false;
        }

        error = Number(this._txtGeofence.val()) > 0 && Number(this._txtGeofence.val()) <= 2000 ? '' : 'Must be between 1 and 2000';
        this._labelGeofenceError.text(error);
        if (error != '') {
            return false;
        }

        return true;
    },
    // Override the method in the super class.
    //
    // Fire an 'onendsave' event.
    _OnOk: function() {
        // Invoke the service to save the address to the server
        var street = this._txtStreet.val();
        var zipCode = this._txtZipCode.val();
        var geofence = Number(this._txtGeofence.val());
        var description = this._txtDescription.val();
        var service = new TIMM.Website.AreaServices.AreaWriterService();
        var thisObj = this;
        service.AddNonDeliverableAddress(street, zipCode, geofence, description, function(ret) {
            if (ret.IsSuccess) {
                ret.Name = street + ", " + zipCode;
                ret.Location = new VELatLong(ret.Latitude, ret.Longitude);
                ret.Attributes = { Geofence: ret.Geofence };
                var locations = [];
                var i = 0;
                var llen = ret.Locations.length;
                while (i < llen) {
                    locations.push(new VELatLong(ret.Locations[i][0], ret.Locations[i][1]));
                    i++;
                }
                ret.Locations = locations;
                thisObj.TriggerEvent("onendsave", ret);
            } else {
                GPSAlert("Can't find this address.");
            }
        });

    },

    // Called after the Cancel button is clicked and the dialog is closed.
    _OnCancel: function() {
    }
});