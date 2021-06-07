

GPS.AddressPanel = GPS.EventTrigger.extend({
    // initalization method - this function is invoked dynamic when new instance
    init: function(options) {
        this._super(options);
        this._Define();
        this._InitAddressPanelBase(options);
    },
    // define basic attributes
    _Define: function() {
        this._addresses = null;
        this._recorder = null;
        this._view = null;
    },
    // initalization method
    _InitAddressPanelBase: function(options) {
        var thisObj = this;
        this._addresses = new GPS.QArray();
        this._recorder = new GPS.CampaignMapPanel.AddressRecorder();
        this._view = new GPS.AddressPanelView(options);
        this._view.AttachEvent("onnewaddress", function(address) {
            thisObj.AppendAddress2(address);
            if (mapPanel) {
                mapPanel.RefreshColors();
            }
        });
        this._view.AttachEvent('onitemselect', function(addressId) {
            thisObj._OnSelectAddress(addressId);
        });
        this._view.AttachEvent('ondeletedaddress', function(addressId) {
            thisObj._OnDeletedAddress(addressId);
            if (mapPanel) {
                mapPanel.RefreshColors();
            }
        });
        this._view.AttachEvent('onchangedaddressradius', function(options) {
            var service = new TIMM.Website.CampaignServices.CampaignWriterService();
            service.ChangeAddressRadiusDisplay(options.RadiusId, options.Enabled);
            thisObj._recorder.SetRadiusEnabled(options);
            if (mapPanel) {
                mapPanel.RefreshColors();
            }
        });
        this._view.AttachEvent('oneditsaved', function(address) {
            thisObj._recorder.Push(address);
            if (mapPanel) {
                mapPanel.RefreshColors();
            }
        });
    },

    _OnDeletedAddress: function(addressId) {
        var service = new TIMM.Website.CampaignServices.CampaignWriterService();
        service.DeleteAddress(campaign.Id, addressId);
        this._recorder.Remove(addressId);
        this._addresses.Set(addressId, null);

    },

    _OnSelectAddress: function(addressId) {
        var address = this._addresses.Get(addressId);
        if (address != null) {
            this._view.ActiveAddress(address , this._addresses);
        }
    },

    AppendAddress: function(address) {    
        this._addresses.Set(address.Id, address);
        this._recorder.Push2(address);
                
        this._view.AppendAddress(address, this._addresses);
    },

    AppendAddress2: function(address) {
        this._addresses.Set(address.Id, address);
        this._recorder.Push(address);

        this._view.AppendAddress(address, this._addresses);
    },

    AppendAddresses: function(addresses) {
        for (var i in addresses) {
            this.AppendAddress(addresses[i]);
        }
        if (mapPanel) {
            mapPanel.RefreshColors();
        }
    },

    AppendAddresses2: function(addresses) {
        for (var i in addresses) {
            this.AppendAddress2(addresses[i]);
        }
        if (mapPanel) {
            mapPanel.RefreshColors();
        }
    },
    
    HasRecords: function(options) {
        var classification = options.Classification;
        var areaId = options.AreaId;
        return this._recorder.GetRecord(options.Classification, options.AreaId);
        //        var hasRecord = false;
        //        this._addresses.Each(function(i, address) {
        //            for (var j in address.Radiuses) {
        //                var relations = address.Radiuses[j].Relations;
        //                if (address.Radiuses[j].IsDisplay &&
        //                    relations[classification] &&
        //                    relations[classification][areaId]) {
        //                    hasRecord = true;
        //                    break;
        //                }
        //            }
        //        });
        //        return hasRecord;
    },

    GetAddresses: function() {
        var addresses = [];
        this._addresses.Each(function(i, address) {
            addresses.push(address);
        });
        return addresses;
    },

    GetAddress: function(addressId) {
        return this._addresses.Get(addressId);
    },

    GetExportString: function(addressId, radiusId) {
        return this._recorder.GetExportString(addressId, radiusId);
    }

});

GPS.AddressPanelView = GPS.EventTrigger.extend({
    // initalization method - this function is invoked dynamic when new instance
    init: function(options) {
        this._super(options);
        this._Define();
        this._InitAddressPanelViewBase(options);

    },
    // define basic attributes
    _Define: function() {
        this._state = null;
        this._view = null;
        this._listView = null;
        this._formView = null;
        this._mapView = null;
        this._newAddressDialog = null;
        this._activeAddress = null;
        this._activeAddressShape = null;

        this._addresses = null;

    },
    // initalization method
    _InitAddressPanelViewBase: function(options) {
        var thisObj = this;
        this._InitToolBarView();
        this._InitNewDialog();
        this._listView = new GPS.AddressListView();
        this._listView.AttachEvent('onitemselect', function(addressId) {
            thisObj.TriggerEvent("onitemselect", addressId);
        });
        this._formView = new GPS.AddressFormView();
        this._formView.AttachEvent('oneditcancel', function() {
            thisObj._listView.Show();
            thisObj._formView.Hide();
        });
        this._formView.AttachEvent('oneditsaved', function() {
            thisObj._listView.Show();
            thisObj._formView.Hide();
            thisObj.TriggerEvent("oneditsaved", thisObj._activeAddress);
        });
        this._mapView = new GPS.AddressMapView(options);
    },

    ActiveAddress: function(address, addresses) {
        this._activeAddress = address;
        this._activeAddressShape = this._mapView.GetAddressShape(address.Id);
        this._activeAddress.Radiuses.sort(function(a, b) { return a.Length - b.Length });

        if(this._activeAddress.Radiuses) {
            if(this._activeAddress.Radiuses[0]) {
              $("#cb_smallest").attr('checked', this._activeAddress.Radiuses[0].IsDisplay);    
            }

            if(this._activeAddress.Radiuses[1]) {
              $("#cb_middle").attr('checked', this._activeAddress.Radiuses[1].IsDisplay);    
            }

            if(this._activeAddress.Radiuses[2]) {
              $("#cb_largest").attr('checked', this._activeAddress.Radiuses[2].IsDisplay);    
            }        
        }
        
        this._addresses = addresses;
        this._mapView.SetCenter(address);
    },

    _NewAddressDialog: function(isShow) {
        if (isShow) {
            if (!this._newAddressDialog) {
                this._newAddressDialog = $('#new-address-dialog').dialog({
                    width: 400, modal: true, overlay: { opacity: 0.5 }
                });
            }
            $(this._newAddressDialog).dialog('open');
        }
        else if (this._newAddressDialog) {
            $(this._newAddressDialog).dialog('close');
        }
    },

    _OnEditAddress: function() {
        if (this._activeAddress != null) {
            this._formView.BindAddress(this._activeAddress, this._activeAddressShape);
            this._listView.Hide();
            this._formView.Show();
        }
    },

    _OnDeleteAddress: function() {
        if (this._activeAddress != null) {
            this._mapView.DeleteAddressShape(this._activeAddress.Id);
            this._listView.Delete(this._activeAddress.Id);
            this.TriggerEvent("ondeletedaddress", this._activeAddress.Id);
            this._activeAddress = null;
            this._activeAddressShape = null;
        }
    },

    _InitToolBarView: function() {
        // int tool bar
        var thisObj = this;
        $("#address_toolbar").find('A').attr("onclick", "");

        $("#address-toolbar-new").click(function() {
            thisObj._NewAddressDialog(true);
        });
        $("#address-toolbar-edit").click(function() {
            thisObj._OnEditAddress();
        });
        $("#address-toolbar-delete").click(function() {
            thisObj._OnDeleteAddress();
        });
        $("#boundary_toolbar_2 input").attr("onclick", "");
        $("#cb_largest").click(function() {
            if (thisObj._activeAddress != null) {
                thisObj._activeAddress.Radiuses.sort(function(a, b) { return a.Length - b.Length });
                var checked;
                if ($("#cb_largest").attr('checked')) {
                    thisObj._activeAddress.Radiuses[2].IsDisplay = true;
                    thisObj._activeAddressShape.SetCircleVisable(thisObj._activeAddress.Radiuses[2].Id, true);
                    checked = true;
                }
                else {
                    thisObj._activeAddress.Radiuses[2].IsDisplay = false;
                    thisObj._activeAddressShape.SetCircleVisable(thisObj._activeAddress.Radiuses[2].Id, false);
                    checked = false;
                }
                thisObj.TriggerEvent("onchangedaddressradius", {
                    AddressId: thisObj._activeAddress.Id,
                    RadiusId: thisObj._activeAddress.Radiuses[2].Id,
                    Enabled: checked
                });
            }
        });
        $("#cb_middle").click(function() {
            if (thisObj._activeAddress != null) {
                thisObj._activeAddress.Radiuses.sort(function(a, b) { return a.Length - b.Length });
                var checked;
                if ($("#cb_middle").attr('checked')) {
                    thisObj._activeAddress.Radiuses[1].IsDisplay = true;
                    thisObj._activeAddressShape.SetCircleVisable(thisObj._activeAddress.Radiuses[1].Id, true);
                    checked = true;
                }
                else {
                    thisObj._activeAddress.Radiuses[1].IsDisplay = false;
                    thisObj._activeAddressShape.SetCircleVisable(thisObj._activeAddress.Radiuses[1].Id, false);
                    checked = false;
                }
                thisObj.TriggerEvent("onchangedaddressradius", {
                    AddressId: thisObj._activeAddress.Id,
                    RadiusId: thisObj._activeAddress.Radiuses[1].Id,
                    Enabled: checked
                });
            }
        });
        $("#cb_smallest").click(function() {
            if (thisObj._activeAddress != null) {
                thisObj._activeAddress.Radiuses.sort(function(a, b) { return a.Length - b.Length });
                var checked;
                if ($("#cb_smallest").attr('checked')) {
                    thisObj._activeAddress.Radiuses[0].IsDisplay = true;
                    thisObj._activeAddressShape.SetCircleVisable(thisObj._activeAddress.Radiuses[0].Id, true);
                    checked = true;
                }
                else {
                    thisObj._activeAddress.Radiuses[0].IsDisplay = false;
                    thisObj._activeAddressShape.SetCircleVisable(thisObj._activeAddress.Radiuses[0].Id, false);
                    checked = false;
                }
                thisObj.TriggerEvent("onchangedaddressradius", {
                    AddressId: thisObj._activeAddress.Id,
                    RadiusId: thisObj._activeAddress.Radiuses[0].Id,
                    Enabled: checked
                });
            }
        });

        $("#btnApplyAll").click(function() {

            var largestFlag = $("#cb_largest").attr('checked');
            var middleFlag = $("#cb_middle").attr('checked');
            var smallestFlag = $("#cb_smallest").attr('checked');

            if (thisObj._addresses) {
                thisObj._addresses.Each(function(i, address) {
                    address.Radiuses[0].IsDisplay = smallestFlag;
                    address.Radiuses[1].IsDisplay = middleFlag;
                    address.Radiuses[2].IsDisplay = largestFlag;
                    var shape = thisObj._mapView.GetAddressShape(address.Id);
                    shape.SetCircleVisable(address.Radiuses[0].Id, smallestFlag);
                    shape.SetCircleVisable(address.Radiuses[1].Id, middleFlag);
                    shape.SetCircleVisable(address.Radiuses[2].Id, largestFlag);

                    thisObj.TriggerEvent("onchangedaddressradius", {
                        AddressId: address.Id,
                        RadiusId: address.Radiuses[0].Id,
                        Enabled: smallestFlag
                    });

                    thisObj.TriggerEvent("onchangedaddressradius", {
                        AddressId: address.Id,
                        RadiusId: address.Radiuses[1].Id,
                        Enabled: middleFlag
                    });

                    thisObj.TriggerEvent("onchangedaddressradius", {
                        AddressId: address.Id,
                        RadiusId: address.Radiuses[2].Id,
                        Enabled: largestFlag
                    });

                });
            }
        });



    },

    _InitNewDialog: function() {
        var thisObj = this;
        $("#new-addresses-button").find('INPUT').attr("onclick", "");
        $("#submit-new-address").click(function() {
            thisObj._NewAddressDialog(false);
            GPS.Loading.show();
            var street = jQuery.trim($('#new-address-line').val());
            var postalCode = jQuery.trim($('#new-address-line-postalcode').val());
            var color = $('#new-address-color').val();
            var pic = $('#hidden-insert-logo').val();
            var service = new TIMM.Website.CampaignServices.CampaignWriterService();
            service.NewAddress(campaign.Id, street, postalCode, color, pic, function(data) {
                if (data != null) {
                    thisObj.TriggerEvent("onnewaddress", data);
                    GPS.Loading.hide();
                }
                else {
                    GPSAlert("The address won't be added,please check your input!");
                    GPS.Loading.hide();
                }
            });
        });
        $("#cancel-new-address").click(function() {
            thisObj._NewAddressDialog(false);
        });
    },
    AppendAddress: function(address, addresses) {
        this._listView.Append(address);
        this._mapView.Append(address);
        this.ActiveAddress(address, addresses);
    }
});




/***************************************************
enum ViewState
***************************************************/

AddressViewState = {
    List: 0,
    New: 1,
    Edit: 2
}