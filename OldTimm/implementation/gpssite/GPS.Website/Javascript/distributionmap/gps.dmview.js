
/***************************************************
class GPS.DMFormView
***************************************************/

GPS.DMFormView = function(prefix) {
    this._view = null;
    this._dmName = null;
    this._dmNameValidator = null;
    this._dmId = null;
    this._dmSubMapId = null;
    this._dmColor = null;
    this._colorRGB = null;
    this._colorPicker = null;
    this._eventTrigger = null;
    this._currentDM = null;
    this.__Init__(prefix);
}

GPS.DMFormView.prototype = {
    __Init__: function(prefix) {
        this.__InitSkin__(prefix);
        this.__InitEvents__();
    },
    __InitSkin__: function(prefix) {
        var formItems = [];
        formItems.push('<div id="distributionNew">');
        formItems.push('<span>Distribution Map Name:</span>');
        formItems.push('<input type="text" id="' + prefix + 'dm-name"></input>');
        formItems.push('<br /><span id="' + prefix + 'dm-name-validator" style="display:none;color:red;" >Distribution Map Name is required.</span>');
        formItems.push('</div>');
        formItems.push('<input type="text" id="' + prefix + 'dm-id" style="display:none;"></input>');
        //        formItems.push('<input type="text" id="' + prefix + 'dm-submapid" style="display:none;"></input>');
        formItems.push('<br/>');
        formItems.push('<div>');
        formItems.push('<span>Distribution Map Color:</span>');
        formItems.push('<input id="' + prefix + 'submap-color" type="text" readOnly />');
        formItems.push('</div>');
        formItems.push('<br/>');
        formItems.push('<div>');
        formItems.push('<a href="#save">Save</a>&nbsp;');
        formItems.push('<a href="#cancel">Cancel</a>');
        formItems.push('</div>');
        this._view = $('<div id="' + prefix + 'dmform" class="submapform">' + formItems.join('') + '</div>');
        this._dmName = $(this._view).find('#' + prefix + 'dm-name');
        this._dmColor = $(this._view).find('#' + prefix + 'submap-color');
        this._dmId = $(this._view).find('#' + prefix + 'dm-id');
        //this._dmSubMapId = $(this._view).find('#' + prefix + 'dm-submapid');
        this._dmSubMapId = $('#' + prefix + 'dm-submapid');
        this._dmNameValidator = $(this._view).find('#' + prefix + 'dm-name-validator');


    },

    __InitEvents__: function() {
        this._eventTrigger = new GPS.EventTrigger();
        var thisObj = this;
        $(this._view).find('A').unbind('click');
        $(this._view).find('A').click(function() {
            // Callback
            var action = $(this).attr('href').substr($(this).attr('href').lastIndexOf('#') + 1);
            if (action != 'save' || (thisObj.__Validate__())) {
                thisObj._eventTrigger.TriggerEvent('onformbuttonclick', action);
            }
            return false;
        });
        $(this._dmColor).ColorPicker({
            color: '#444444',
            onSubmit: function(hsb, hex, rgb, el) {
                thisObj._colorRGB = rgb;
                $(el).val(hex);
                $(el).css('border-right', 'solid 20px  #' + hex);
                $(el).ColorPickerHide();

            },
            onShow: function(colpkr) {
                $(colpkr).fadeIn(500);
                return false;
            },
            onHide: function(colpkr) {
                $(colpkr).fadeOut(500);
                return false;
            },
            onChange: function(hsb, hex, rgb) {
                $('#colorSelector div').css('backgroundColor', '#' + hex);
            }
        });
    },

    __Validate__: function() {
        if ($(this._dmName).val() == '') {
            $(this._dmNameValidator).show();
            return false;
        }
        else {
            return true;
        }
    },

    SetDM: function(dm) { this._currentDM = dm; },

    GetDM: function() { return this._currentDM; },

    DataBind: function() {
        $(this._dmName).val(this._currentDM.GetName());
        $(this._dmId).val(this._currentDM.GetId());
        $(this._dmColor).val(this._currentDM.GetColorString());
        this._colorRGB = this._currentDM.GetColor();
        $(this._dmColor).css('border-right', 'solid 20px  #' + this._currentDM.GetColorString());
        this._dmSubMapId = $('#sub-map-panel-dm-submapid');
        $(this._dmSubMapId).attr("disabled", "");
        var submapId = this._currentDM.GetSubMapId();
        if (submapId) {
            $(this._dmSubMapId).val(submapId);
            $(this._dmSubMapId).attr("disabled", "disabled");
        } 
        
        $(this._dmNameValidator).hide();
    },

    DataHold: function() {
        this._currentDM.SetName($(this._dmName).val());
        this._currentDM.SetId($(this._dmId).val());
        this._currentDM.SetColorString($(this._dmColor).val());
        this._currentDM.SetColor(this._colorRGB);
        //this._currentDM.SetSumMapId($(this._dmSubMapId).val());
        this._currentDM.SetSumMapId($('#sub-map-panel-dm-submapid').val());
    },

    Hide: function() { $(this._view).hide(); },

    Show: function() { $(this._view).show(); },

    AttachEvent: function(eventName, eventHandler) {
        this._eventTrigger.AttachEvent(eventName, eventHandler);
    },

    GetView: function() { return this._view; }


}

/***************************************************
class GPS.DMInfoView
***************************************************/

GPS.DMInfoView = function(prefix) {
    this._view = null;
    this.__Init__(prefix);
}

GPS.DMInfoView.prototype = {
    __Init__: function(prefix) {
        this._view = $('<div id="' + prefix + 'dminfo"></div>');
    },

    BindDM: function(dm) {
       
    },

    Clear: function() {
        $(this._view).html('');
    },

    Show: function() { $(this._view).show(); },

    Hide: function() { $(this._view).hide(); },

    GetView: function() { return this._view; }
}

/***************************************************
class GPS.DMListView
***************************************************/

GPS.DMListView = function(prefix) {
    this._smitemprefix = null;
    this._dmitemprefix = null;
    this._view = null;
    this._eventTrigger = null;
    this.__Init__(prefix);
}

GPS.DMListView.prototype = {
    __Init__: function(prefix) {
        this._view = $('<div id="' + prefix + 'submapdmlist" class="submapDMlist"></div>');
        this._smitemprefix = prefix + 'submaplist-';
        this._dmitemprefix = prefix + 'mdlist-';
        this.__InitEvents__();
    },

    __InitEvents__: function() {
        this._eventTrigger = new GPS.EventTrigger();
    },

    Hide: function() { $(this._view).hide(); },

    Show: function() { $(this._view).show(); },

    //Show existing submaps and their distribution maps
    Append: function(dm) {
        var submapdiv = $('<div id="' + dm.GetId() + '"></div>');
        var item = $('<div id="' + this._smitemprefix + dm.GetId() + '" ></div>');
        var htmString = '<strong>{4}.&nbsp;{0}</strong><span >&nbsp;&nbsp;Total:&nbsp;{1}&nbsp;&nbsp;Count:&nbsp;{2}&nbsp;&nbsp;Pen:&nbsp;{3}%</span>';
        htmString = htmString.replace("{0}", dm.GetName().substring(0, 25));
        htmString = htmString.replace("{1}", Number.addCommas(dm.GetTotal()));
        htmString = htmString.replace("{2}", Number.addCommas(dm.GetPenetration()));
        htmString = htmString.replace("{3}", (dm.GetPercentage()*100).toFixed(2));
        htmString = htmString.replace("{4}", dm.GetOrderId());

        $(item).html(htmString);
        $(item).css('border-left', 'solid 20px  #' + dm.GetColorString());



        var thisObj = this;
        var OnItemClick = function() {
                        $(this._view).find('div.selected').removeClass('selected');
                        $(item).addClass('selected');
                        thisObj._eventTrigger.TriggerEvent('onsubmapitemselect', $(item).attr('id').substr(thisObj._smitemprefix.length));
        };
        $(item).dblclick(OnItemClick);
        $(this._view).find('div.selected').removeClass('selected');
        $(item).addClass('selected');
        $(submapdiv).append($(item));
        $(this._view).append($(submapdiv));

        if (dm._dms._dmrecords.length > 0) {
            for (i = 0; i < dm._dms._dmrecords.length; i++) {
                this.AppendDM(submapdiv, dm._dms._dmrecords[i]);
            }

        }
    },

    //function to show one submap's distribution maps
    AppendDM: function(div, dmitem) {
    var dmapitem = $('<div id="' + dmitem._id + '+' + dmitem._submapid + '" class="DMlist"></div>');
        //var htmString = '&nbsp;&nbsp;&nbsp;' + dmitem._name;

        var htmString = '<strong>&nbsp;{0}</strong><br /><span>&nbsp;Total:&nbsp;{1}&nbsp;&nbsp;Count:&nbsp;{2}&nbsp;&nbsp;Pen:&nbsp;{3}%</span>';
        htmString = htmString.replace("{0}", dmitem.GetName().substring(0, 25));
        htmString = htmString.replace("{1}", Number.addCommas(dmitem.GetTotal()));
        htmString = htmString.replace("{2}", Number.addCommas(dmitem.GetPenetration()));
        htmString = htmString.replace("{3}", (dmitem.GetPercentage()*100).toFixed(2));


        $(dmapitem).html(htmString);
        $(dmapitem).css('border-left', 'solid 20px  #' + dmitem.GetColorString());

        var thisObj = this;
        var OnItemDoubleClick = function() {
            $(this._view).find('div.selected').removeClass('selected');
            $(dmapitem).addClass('selected');
            thisObj._eventTrigger.TriggerEvent('onitemdoubleselect', $(dmapitem).attr('id'));
        };
        
        var OnItemClick = function() {
            $(this._view).find('div.selected').removeClass('selected');
            $(dmapitem).addClass('selected');
            thisObj._eventTrigger.TriggerEvent('onitemselect', $(dmapitem).attr('id'));
        };
        $(dmapitem).dblclick(OnItemDoubleClick);
        $(dmapitem).click(OnItemClick);
        $(this._view).find('div.selected').removeClass('selected');
        $(dmapitem).addClass('selected');
        $(div).append($(dmapitem));
    },

    //function to add new distribution map and show it on the page
    AppendNewDM: function(dmitem) {
        var newdmitem = $('<div id="' + dmitem._id + '+' + dmitem._submapid + '" class="DMlist"></div>');
        //var htmString = '&nbsp;&nbsp;&nbsp;' + dmitem.GetName();

        var htmString = '<strong>&nbsp;{0}</strong><br /><span>&nbsp;Total:&nbsp;{1}&nbsp;&nbsp;Count:&nbsp;{2}&nbsp;&nbsp;Pen:&nbsp;{3}%</span>';
        htmString = htmString.replace("{0}", dmitem.GetName().substring(0, 25));
        htmString = htmString.replace("{1}", Number.addCommas(dmitem.GetTotal()));
        htmString = htmString.replace("{2}", Number.addCommas(dmitem.GetPenetration()));
        htmString = htmString.replace("{3}", (dmitem.GetPercentage()*100).toFixed(2));

        $(newdmitem).html(htmString);
        $(newdmitem).css('border-left', 'solid 20px  #' + dmitem.GetColorString());

        var thisObj = this;
        var OnItemDoubleClick = function() {
            $(this._view).find('div.selected').removeClass('selected');
            $(newdmitem).addClass('selected');
            thisObj._eventTrigger.TriggerEvent('onitemdoubleselect', $(newdmitem).attr('id'));
        };
        var OnItemClick = function() {
            $(this._view).find('div.selected').removeClass('selected');
            $(newdmitem).addClass('selected');
            thisObj._eventTrigger.TriggerEvent('onitemselect', $(newdmitem).attr('id'));
        };
        $(newdmitem).dblclick(OnItemDoubleClick);
        $(newdmitem).click(OnItemClick);
        $(this._view).find('div.selected').removeClass('selected');
        $(newdmitem).addClass('selected');
        $('#' + dmitem.GetSubMapId()).append($(newdmitem));
    },

    //function to udpate the display on the page
    UpdateDM: function(dm) {
        var htmString = '<strong>&nbsp;{0}</strong><br /><span>&nbsp;Total:&nbsp;{1}&nbsp;&nbsp;Count:&nbsp;{2}&nbsp;&nbsp;Pen:&nbsp;{3}%</span>';
        htmString = htmString.replace("{0}", dm.GetName().substring(0, 25));
        htmString = htmString.replace("{1}", Number.addCommas(dm.GetTotal()));
        htmString = htmString.replace("{2}", Number.addCommas(dm.GetPenetration()));
        htmString = htmString.replace("{3}", (dm.GetPercentage()*100).toFixed(2));

        $('#' + dm.GetId() + '\\+' + dm.GetSubMapId()).html(htmString);
        $('#' + dm.GetId() + '\\+' + dm.GetSubMapId()).css('border-left', 'solid 20px  #' + dm.GetColorString());
    },

    //function to remove the distribution map on the page
    Remove: function(dm) {
        $('#' + dm.GetId() + '\\+' + dm.GetSubMapId()).remove();
    },

    AttachEvent: function(eventName, eventHandler) {
        this._eventTrigger.AttachEvent(eventName, eventHandler);
    },

    BindDMs: function(dms) {
        $(this._view).html('');
        var i = 0;
        var length = dms.length;
        while (i < length) {
            this.Append(dms[i]);
            i++;
        }
        $(this._view).find('div.selected').removeClass('selected');
    },

    GetView: function() { return this._view; }
}



/***************************************************
class GPS.DMMenuBarView
***************************************************/

GPS.DMMenuBarView = function(prefix) {
    this._view = null;
    this._eventTrigger = null;
    this.__Init__(prefix);
}

GPS.DMMenuBarView.prototype = {
    __Init__: function(prefix) {
        this._eventTrigger = new GPS.EventTrigger();
        var menus = [];
        menus.push('<a href="#new">New</a>');
        menus.push('<a href="#edit">Edit</a>');
        menus.push('<a href="#delete">Delete</a>');
        var menuBar = $('<div id="' + prefix + 'toolbar" class="menubar">' + menus.join('|') + '</div>');

        $(menuBar).find('A').mouseover(function() {
            $(menuBar).find('A.hover').removeClass('hover');
            $(this).parent().addClass('hover');
        }).mouseout(function() {
            $(menuBar).find('A.hover').removeClass('hover');
        });

        var thisObj = this;
        $(menuBar).find('A').unbind('click');
        $(menuBar).find('A').click(function() {
            // Callback
            thisObj._eventTrigger.TriggerEvent('onmenuclick', $(this).attr('href').substr($(this).attr('href').lastIndexOf('#') + 1));
            return false;
        });
        this._view = menuBar;
    },


    AttachEvent: function(eventName, eventHandler) {
        this._eventTrigger.AttachEvent(eventName, eventHandler);
    },

    GetView: function() { return this._view; }
}
