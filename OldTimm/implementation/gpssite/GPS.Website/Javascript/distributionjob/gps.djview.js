
/***************************************************
class GPS.DJFormView
***************************************************/

GPS.DJFormView = function(prefix) {
    this._view = null;
    this._djName = null;
    this._djNameValidator = null;
    this._djId = null;
    this._djCampaign = null;
    this._eventTrigger = null;
    this._currentDJ = null;
    this.__Init__(prefix);
}

GPS.DJFormView.prototype = {
    __Init__: function(prefix) {
        this.__InitSkin__(prefix);
        this.__InitEvents__();
    },
    __InitSkin__: function(prefix) {
        var formItems = [];
        formItems.push('<div>');
        formItems.push('<span>Distribution Job Name:</span>');
        formItems.push('<input type="text" id="' + prefix + 'dj-name"></input>');
        formItems.push('<br /><span id="' + prefix + 'dj-name-validator" style="display:none;color:red;" >Distribution Job Name is required.</span>');
        formItems.push('</div>');
        formItems.push('<input type="text" id="' + prefix + 'dj-id" style="display:none;"></input>');
        formItems.push('<input type="text" id="' + prefix + 'dj-campaign" style="display:none;"></input>');
        formItems.push('<br/>');
        formItems.push('<div>');
        formItems.push('<a href="#save">Save</a>&nbsp;');
        formItems.push('<a href="#cancel">Cancel</a>');
        formItems.push('</div>');
        this._view = $('<div id="' + prefix + 'djform" class="djform">' + formItems.join('') + '</div>');
        this._djName = $(this._view).find('#' + prefix + 'dj-name');
        this._djId = $(this._view).find('#' + prefix + 'dj-id');
        this._djCampaign = $(this._view).find('#' + prefix + 'dj-campaignid');
        this._djNameValidator = $(this._view).find('#' + prefix + 'dj-name-validator');
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
    },

    __Validate__: function() {
        if ($(this._djName).val() == '') {
            $(this._djNameValidator).show();
            return false;
        }
        else {
            return true;
        }
    },

    SetDJ: function(dj) { this._currentDJ = dj; },

    GetDJ: function() { return this._currentDJ; },

    DataBind: function() {
        $(this._djName).val(this._currentDJ.GetName());
        $(this._djId).val(this._currentDJ.GetId());
        this._djCampaign = this._currentDJ.GetCampaign()
        $(this._djNameValidator).hide();
    },

    DataHold: function() {
        this._currentDJ.SetName($(this._djName).val());
        this._currentDJ.SetId($(this._djId).val());
        this._currentDJ.SetCampaign(this._djCampaign);
    },

    Hide: function() { $(this._view).hide(); },

    Show: function() { $(this._view).show(); },

    AttachEvent: function(eventName, eventHandler) {
        this._eventTrigger.AttachEvent(eventName, eventHandler);
    },

    GetView: function() { return this._view; }
}

/***************************************************
class GPS.DJInfoView
***************************************************/

GPS.DJInfoView = function(prefix) {
    this._view = null;
    this.__Init__(prefix);
}

GPS.DJInfoView.prototype = {
    __Init__: function(prefix) {
        this._view = $('<div id="' + prefix + 'djinfo"></div>');
    },

    BindDJ: function(dj) {

    },

    Clear: function() {
        $(this._view).html('');
    },

    Show: function() { $(this._view).show(); },

    Hide: function() { $(this._view).hide(); },

    GetView: function() { return this._view; }
}

/***************************************************
class GPS.DJListView
***************************************************/

GPS.DJListView = function(prefix) {
    this._djitemprefix = null;
    this._dmitemprefix = null;
    this._view = null;
    this._eventTrigger = null;
    this.__Init__(prefix);
    this._layer = null;
}

GPS.DJListView.prototype = {
    __Init__: function(prefix) {
        this._view = $('<div id="' + prefix + 'djlist" ></div>');
        this._djitemprefix = prefix + 'djlist-';
        this._dmitemprefix = prefix + 'dmlist-';
        this.__InitEvents__();
    },

    __InitEvents__: function() {
        this._eventTrigger = new GPS.EventTrigger();
    },

    Hide: function() { $(this._view).hide(); },

    Show: function() { $(this._view).show(); },

    //Show existing distribution jobs and their distribution maps
    Append: function(dj, i) {
        var thisObj = this;
        var currentlayer = null;
        if (null != i) {
            currentlayer = 100000 + i;
            this.SetLayer(100000 + i);
        }
        else if (null == i && this.GetLayer() == null) {
            currentlayer = 100000;
            this.SetLayer(currentlayer);
        }
        else {
            currentlayer = this.GetLayer() - 1;
            this.SetLayer(currentlayer);
        }

        var distributionjobdiv = $('<div id="' + dj.GetId() + '" class="djlist" style="z-index:' + currentlayer + ';"></div>');
        var menu = $('<ul id="navmenu"><li><a href="#">&nbsp;+&nbsp;</a><ul></ul></li></ul>')
        $(distributionjobdiv).append($(menu));

        //Assign Dirvers//
        var assigndrivers = $('<li id="' + dj.GetId() + '"><a href="#">Assign Dirvers</a></li>');
        var AssignDriversOnClick = function() {
            thisObj._eventTrigger.TriggerEvent('onassigndriversclick', $(assigndrivers).attr('id'));
        }
        $(assigndrivers).click(AssignDriversOnClick);
        $(distributionjobdiv).find('ul li ul').append($(assigndrivers));

        //Assign Auditors//
        var assignauditors = $('<li id="' + dj.GetId() + '"><a href="#">Assign Auditors</a></li>');
        var AssignAuditorsOnClick = function() {
            thisObj._eventTrigger.TriggerEvent('onassignauditorsclick', $(assignauditors).attr('id'));
        }
        $(assignauditors).click(AssignAuditorsOnClick);
        $(distributionjobdiv).find('ul li ul').append($(assignauditors));

        //Assign Walkers//
        var assignwalkers = $('<li id="' + dj.GetId() + '"><a href="#">Assign Walkers</a></li>');
        var AssignWalkersOnClick = function() {
            thisObj._eventTrigger.TriggerEvent('onassignwalkersclick', $(assignwalkers).attr('id'));
        }
        $(assignwalkers).click(AssignWalkersOnClick);
        $(distributionjobdiv).find('ul li ul').append($(assignwalkers));

        //Assign Gtus//
        /*
        var assigngtus = $('<li id="' + dj.GetId() + '"><a href="#">Assign GTUs</a></li>');
        var AssignGtusOnClick = function() {
            thisObj._eventTrigger.TriggerEvent('onassigngtusclick', $(assigngtus).attr('id'));
        }
        $(assigngtus).click(AssignGtusOnClick);
        $(distributionjobdiv).find('ul li ul').append($(assigngtus));
        */
        var item = $('<div id="' + this._djitemprefix + dj.GetId() + '" class="djitem" ></div>');
        var htmString = '<strong>&nbsp;{0}</strong>';
        htmString = htmString.replace("{0}", dj.GetName());
        $(item).html(htmString);

        var OnItemClick = function() {
            $(this._view).find('div.selected').removeClass('selected');
            $(item).addClass('selected');
            thisObj._eventTrigger.TriggerEvent('ondjitemselect', $(item).attr('id').substr(thisObj._djitemprefix.length));
        };
        $(item).click(OnItemClick);
        $(this._view).find('div.selected').removeClass('selected');
        $(item).addClass('selected');
        $(distributionjobdiv).append($(item));
        $(this._view).append($(distributionjobdiv));

        if (dj._dms._dmrecords.length > 0) {
            for (i = 0; i < dj._dms._dmrecords.length; i++) {
                this.AppendDM(distributionjobdiv, dj._dms._dmrecords[i], dj.GetId());
            }

        }
    },

    //function to show one submap's distribution maps
    AppendDM: function(div, dmitem, djid) {
        var dmapitem = $('<div id="' + djid + '' + dmitem._id + '" class="dmlist"></div>');
        var htmString = '&nbsp;&nbsp;&nbsp;' + dmitem._name;

        $(dmapitem).html(htmString);

        var thisObj = this;
        var OnItemClick = function() {
            $(this._view).find('div.selected').removeClass('selected');
            $(dmapitem).addClass('selected');
            thisObj._eventTrigger.TriggerEvent('ondmitemselect', $(dmapitem).attr('id'));
        };
        $(dmapitem).click(OnItemClick);
        $(this._view).find('div.selected').removeClass('selected');
        $(dmapitem).addClass('selected');
        $(div).append($(dmapitem));
    },

    //function to udpate the display on the page
    UpdateDJ: function(dj) {
        var htmString = '<strong>&nbsp;{0}</strong>';
        htmString = htmString.replace("{0}", dj.GetName());
        $('#' + this._djitemprefix + dj.GetId()).html(htmString);
    },

    //function to remove the distribution map on the page
    Remove: function(dj) {
        $('#' + dj.GetId()).remove();

    },

    AttachEvent: function(eventName, eventHandler) {
        this._eventTrigger.AttachEvent(eventName, eventHandler);
    },

    BindDJs: function(djs) {
        $(this._view).html('');
        var i = 0;
        var length = djs.length;
        while (i < length) {
            this.Append(djs[i], -i);
            i++;
        }
        $(this._view).find('div.selected').removeClass('selected');
    },

    GetView: function() { return this._view; },

    GetLayer: function() { return this._layer; },

    SetLayer: function(layer) { this._layer = layer }
}



/***************************************************
class GPS.DJMenuBarView
***************************************************/

GPS.DJMenuBarView = function(prefix) {
    this._view = null;
    this._eventTrigger = null;
    this.__Init__(prefix);
}

GPS.DJMenuBarView.prototype = {
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
