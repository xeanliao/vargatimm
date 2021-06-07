
/***************************************************
class GPS.SubMapFormView
***************************************************/

GPS.SubMapFormView = function(prefix) {
    this._view = null;
    this._submapName = null;
    this._submapNameValidator = null;
    this._submapColor = null;
    this._colorRGB = null;
    this._eventTrigger = null;
    this._colorPicker = null;
    this._currentSubMap = null;
    this.__Init__(prefix);
}

GPS.SubMapFormView.prototype = {
    __Init__: function(prefix) {
        this.__InitSkin__(prefix);
        this.__InitEvents__();
    },
    __InitSkin__: function(prefix) {
        var formItems = [];
        formItems.push('<div>');
        formItems.push('<span>Sub Map Designator:</span>');
        formItems.push('<input type="text" id="' + prefix + 'submap-name"></input>');
        formItems.push('<br /><span id="' + prefix + 'submap-name-validator" style="display:none;color:red;" >Required</span>');
        formItems.push('</div>');
        formItems.push('<div>');
        formItems.push('<span>Sub Map Color:</span>');
        formItems.push('<input id="' + prefix + 'submap-color" type="text" readOnly />');
        formItems.push('</div>');
        formItems.push('<div>');
        formItems.push('<span>Adjust Total:</span>');
        formItems.push('<input type="text" id="' + prefix + 'submap-total"></input>');
        formItems.push('<br /><span id="' + prefix + 'submap-total-validator" style="display:none;color:red;" >Required</span>');
        formItems.push('</div>');
        formItems.push('<div>');
        formItems.push('<span>Adjust Count:</span>');
        formItems.push('<input type="text" id="' + prefix + 'submap-count"></input>');
        formItems.push('<br /><span id="' + prefix + 'submap-count-validator" style="display:none;color:red;" >Required</span>');
        formItems.push('</div>');
        formItems.push('<div>');
        formItems.push('<a href="#save">Save</a>"');
        formItems.push('<a href="#cancel">Cancel</a>');
        formItems.push('</div>');
        this._view = $('<div id="' + prefix + 'submapform" class="submapform">' + formItems.join('') + '</div>');
        this._submapName = $(this._view).find('#' + prefix + 'submap-name');
        this._submapColor = $(this._view).find('#' + prefix + 'submap-color');
        this._submapNameValidator = $(this._view).find('#' + prefix + 'submap-name-validator');
        this._adjustTotal = $(this._view).find('#' + prefix + 'submap-total');
        this._adjustCount = $(this._view).find('#' + prefix + 'submap-count');
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

        $(this._submapColor).ColorPicker({
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
        if ($(this._submapName).val() == '') {
            $(this._submapNameValidator).show();
            return false;
        }
        else {
            return true;
        }
    },

    SetSubMap: function(submap) { this._currentSubMap = submap; },

    GetSubMap: function() { return this._currentSubMap; },

    DataBind: function() {
        $(this._submapName).val(this._currentSubMap.GetName());
        $(this._submapColor).val(this._currentSubMap.GetColorString());
        this._colorRGB = this._currentSubMap.GetColor();
        $(this._submapColor).css('border-right', 'solid 20px  #' + this._currentSubMap.GetColorString());
        $(this._submapNameValidator).hide();
        $(this._adjustTotal).val(this._currentSubMap.GetAdjustTotal());
        $(this._adjustCount).val(this._currentSubMap.GetAdjustCount());
    },

    DataHold: function() {
        this._currentSubMap.SetName($(this._submapName).val());
        this._currentSubMap.SetColorString($(this._submapColor).val());
        this._currentSubMap.SetColor(this._colorRGB);
        this._currentSubMap.SetAdjustTotal($(this._adjustTotal).val());
        this._currentSubMap.SetAdjustCount($(this._adjustCount).val());
        this._currentSubMap.RefreshPercentage();
    },

    Hide: function() { $(this._view).hide(); },

    Show: function() { $(this._view).show(); },

    AttachEvent: function(eventName, eventHandler) {
        this._eventTrigger.AttachEvent(eventName, eventHandler);
    },

    GetView: function() { return this._view; }
}

/***************************************************
class GPS.SubMapInfoView
***************************************************/

GPS.SubMapInfoView = function(prefix) {
    this._view = null;
    this.__Init__(prefix);
}

GPS.SubMapInfoView.prototype = {
    __Init__: function(prefix) {
        this._view = $('<div id="' + prefix + 'submapinfo"></div>');
    },

    BindSubMap: function(submap) {
        var total = submap.GetTotal();
        var pen = submap.GetPenetration();
        var html = "Total:&nbsp;{0}&nbsp;&nbsp;Pen:&nbsp;{1}";
        html = html.replace("{0}", total);
        html = html.replace("{1}", pen);
        $(this._view).html(html);
    },

    Clear: function() {
        $(this._view).html('');
    },

    Show: function() { $(this._view).show(); },

    Hide: function() { $(this._view).hide(); },

    GetView: function() { return this._view; }
}

/***************************************************
class GPS.SubMapListView
***************************************************/

GPS.SubMapListView = function(prefix) {
    this._itemprefix = null;
    this._view = null;
    this._eventTrigger = null;
    this.__Init__(prefix);
}

GPS.SubMapListView.prototype = {
    __Init__: function(prefix) {
        this._view = $('<div id="' + prefix + 'submaplist" class="submaplist"></div>');
        this._itemprefix = prefix + 'submaplist-';
        this.__InitEvents__();
    },

    __InitEvents__: function() {
        this._eventTrigger = new GPS.EventTrigger();
    },

    Hide: function() { $(this._view).hide(); },

    Show: function() { $(this._view).show(); },

    Append: function(submap) {
        var item = $('<div id="' + this._itemprefix + submap.GetId() + '"></div>');
        var htmString = '<strong>{4}.&nbsp;{0}</strong><br /><span>Total:&nbsp;{1}&nbsp;&nbsp;Count:&nbsp;{2}&nbsp;&nbsp;Pen:&nbsp;{3}%</span>';
        htmString = htmString.replace("{0}", submap.GetName().substring(0, 25));
        htmString = htmString.replace("{1}", Number.addCommas(submap.GetTotal()));
        htmString = htmString.replace("{2}", Number.addCommas(submap.GetPenetration()));
        htmString = htmString.replace("{3}", (submap.GetPercentage() * 100).toFixed(2));
        htmString = htmString.replace("{4}", submap.GetId());
        $(item).html(htmString);
        $(item).css('border-left', 'solid 20px  #' + submap.GetColorString());


        var thisObj = this;
        var OnItemClick = function() {
            $(this._view).find('div.selected').removeClass('selected');
            $(item).addClass('selected');
            thisObj._eventTrigger.TriggerEvent('onitemselect', $(item).attr('id').substr(thisObj._itemprefix.length));
        };
        var OnItemDoubleClick = function() {
            $(this._view).find('div.selected').removeClass('selected');
            $(item).addClass('selected');           
            thisObj._eventTrigger.TriggerEvent('onitemdoubleselect', $(item).attr('id').substr(thisObj._itemprefix.length));
        }
        $(item).dblclick(OnItemDoubleClick);
        $(item).click(OnItemClick);
        
        $(this._view).find('div.selected').removeClass('selected');
        $(item).addClass('selected');
        $(this._view).append($(item));
    },

    UpdateSubMap: function(submap) {
        var htmString = '<strong>{4}.&nbsp;{0}</strong><br /><span>Total:&nbsp;{1}&nbsp;&nbsp;Count:&nbsp;{2}&nbsp;&nbsp;Pen:&nbsp;{3}%</span>';
        htmString = htmString.replace("{0}", submap.GetName().substring(0, 25));
        htmString = htmString.replace("{1}", Number.addCommas(submap.GetTotal()));
        htmString = htmString.replace("{2}", Number.addCommas(submap.GetPenetration()));
        htmString = htmString.replace("{3}", (submap.GetPercentage() * 100).toFixed(2));
        htmString = htmString.replace("{4}", submap.GetId());

        $('#' + this._itemprefix + submap.GetId()).html(htmString);
        $('#' + this._itemprefix + submap.GetId()).css('border-left', 'solid 20px  #' + submap.GetColorString());
    },

    Remove: function(submap) {
        $('#' + this._itemprefix + submap.GetId()).remove();
    },

    AttachEvent: function(eventName, eventHandler) {
        this._eventTrigger.AttachEvent(eventName, eventHandler);
    },

    BindSubMaps: function(submaps) {
        $(this._view).html('');
        var i = 0;
        var length = submaps.length;
        while (i < length) {
            this.Append(submaps[i]);
            i++;
        }
        $(this._view).find('div.selected').removeClass('selected');
    },

    GetView: function() { return this._view; }
}



/***************************************************
class GPS.SubMapMenuBarView
***************************************************/

GPS.SubMapMenuBarView = function(prefix) {
    this._view = null;
    this._eventTrigger = null;
    this.__Init__(prefix);
}

GPS.SubMapMenuBarView.prototype = {
    __Init__: function(prefix) {
        this._eventTrigger = new GPS.EventTrigger();
        var menus = [];
        menus.push('<a href="#new">New</a>');
        menus.push('<a href="#edit">Edit</a>');
        menus.push('<a href="#delete">Delete</a>');
        menus.push('<a href="#addAllShapes">Add All Shapes</a>');
        menus.push('<a href="#removeAllShapes">Remove All Shapes</a>');
        menus.push('<a href="#deselectAllShapes">Deselect All Shapes</a>');
        
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
