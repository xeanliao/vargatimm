//

function OnPenetrationColorEnabledChange(obj) {
    var index = Number(obj.id.substring(obj.id.length - 1)) - 1;
    var id = index + 1;
    if (obj.checked) {
        //        $('#txtColorMin' + id).val(PenetrationColorSettings.GetColors(true)[index].Min);
        //        $('#txtColorMax' + id).val(PenetrationColorSettings.GetColors(true)[index].Max);
        $('#txtColorMin' + id).attr('disabled', false);
        $('#txtColorMax' + id).attr('disabled', false);
    }
    else {
        $('#txtColorMin' + id).val('');
        $('#txtColorMax' + id).val('');
        $('#txtColorMin' + id).attr('disabled', true);
        $('#txtColorMax' + id).attr('disabled', true);
        PenetrationColorSettings.GetColors(true)[index].Min = -1;
        PenetrationColorSettings.GetColors(true)[index].Max = -1;
    }
    if (PrintSettings.ShowPenetrationColors) {
        ChangeShowPenetrationColor(true);
    }
}

function OnPenetrationColorMin(obj) {
    if (ValidatePenetrationColorMin(obj)) {
        if (PrintSettings.ShowPenetrationColors) {
            ChangeShowPenetrationColor(true);
        }
    }
}

function ValidatePenetrationColorMin(obj) {
    var valid = false;
    var strP = /^\d+(\.\d+)?$/;
    var index = Number(obj.id.substring(obj.id.length - 1)) - 1;
    var omax = Math.round(PenetrationColorSettings.GetColors(true)[index].Max * 100);
    var omin = Math.round(PenetrationColorSettings.GetColors(true)[index].Min * 100);

    if (strP.test(obj.value) && Number(obj.value) >= 0 && Number(obj.value) <= 100) {
        var nmin = Number(obj.value);
        var colors = PenetrationColorSettings.GetColors(true);
        var i = 0;
        var clen = colors.length;
        var innerValid = true;
        while (i < clen) {
            if (i == index) { i++; continue; }
            if (omax >= 0) {
                if ((Math.round(colors[i].Min * 100) <= nmin && nmin < Math.round(colors[i].Max * 100))
                || (Math.round(colors[i].Min * 100) < omax && nmin < Math.round(colors[i].Max * 100))) {
                    innerValid = false;
                    break;
                }

            }
            else {
                if (Math.round(colors[i].Min * 100) <= nmin && nmin < Math.round(colors[i].Max * 100)) {
                    innerValid = false;
                    break;
                }
            }
            i++;
        }
        if (innerValid) {
            if (omax >= 0 && omin >= 0) {
                if (nmin < omax) { valid = true; }
            }
            else {
                valid = true;
            }
        }
    }

    if (valid) { PenetrationColorSettings.GetColors(true)[index].Min = Number(obj.value) / 100; }
    else if (omin >= 0) { $(obj).val(GetPenetrationNumber(omin)); }
    else { $(obj).val(''); }
    return valid;
}

function OnPenetrationColorMax(obj) {
    if (ValidatePenetrationColorMax(obj)) {
        if (PrintSettings.ShowPenetrationColors) {
            ChangeShowPenetrationColor(true);
        }
    }
}

function ValidatePenetrationColorMax(obj) {
    var valid = false;
    var strP = /^\d+(\.\d+)?$/;
    var index = Number(obj.id.substring(obj.id.length - 1)) - 1;
    var omax = Math.round(PenetrationColorSettings.GetColors(true)[index].Max * 100);
    var omin = Math.round(PenetrationColorSettings.GetColors(true)[index].Min * 100);

    if (strP.test(obj.value) && Number(obj.value) >= 0 && Number(obj.value) <= 100) {
        var nmax = Number(obj.value);
        var colors = PenetrationColorSettings.GetColors(true);
        var i = 0;
        var clen = colors.length;
        var innerValid = true;
        while (i < clen) {
            if (i == index) { i++; continue; }
            if (omin >= 0) {
                if ((Math.round(colors[i].Min * 100) < nmax && nmax <= Math.round(colors[i].Max * 100))
                    || (Math.round(colors[i].Min * 100) < nmax && omin < Math.round(colors[i].Max * 100))) {
                    innerValid = false;
                    break;
                }
            }
            else {
                if (Math.round(colors[i].Min * 100) < nmax && nmax <= Math.round(colors[i].Max * 100)) {
                    innerValid = false;
                    break;
                }
            }

            i++;
        }
        if (innerValid) {
            if (nmax >= 0 && omin >= 0) {
                if (nmax > omin) { valid = true; }
            }
            else {
                valid = true;
            }
        }
    }

    if (valid) { PenetrationColorSettings.GetColors(true)[index].Max = Number(obj.value) / 100; }
    else if (omax >= 0) { $(obj).val(GetPenetrationNumber(omax)); }
    else { $(obj).val(''); }
    return valid;
}


function GetPenetrationNumber(number) {
    return number > 100 ? 100 : (number < 0 ? 0 : number);
}


PenetrationColor = function(name, htmlValue, min, max, r, g, b, a) {
    this.Min = null;
    this.Max = null;
    this.Name = null;
    this.HtmlValue = null;
    this.R = null;
    this.G = null;
    this.B = null;
    this.A = null;
    this.__Init__(name, htmlValue, min, max, r, g, b, a);
}

PenetrationColor.prototype = {
    __Init__: function(name, htmlValue, min, max, r, g, b, a) {
        this.Name = name;
        this.HtmlValue = htmlValue;
        this.Min = min;
        this.Max = max;
        this.R = r;
        this.G = g;
        this.B = b;
        this.A = a;
    },

    Contains: function(penetration) {

        if ((this.Min >= 0 && this.Max >= 0) && (this.Min <= penetration && penetration < this.Max) || (penetration == 1 && this.Max == 1)) { return true; }
        else { return false; }
    },

    Serialize: function() {
        var min = this.Min < 0 ? 0 : this.Min;
        var max = this.Max > 1 ? 1 : this.Max;
        var items = [];
        items.push(this.Name);
        items.push(this.HtmlValue);
        items.push(min);
        items.push(max);
        items.push(this.R);
        items.push(this.G);
        items.push(this.B);
        items.push(this.A);
        return items.join(',');
    }
}


PenetrationColorSettings = {
    _settings: [new PenetrationColor('Blue', 'Blue', 0.0, 0.2, 0, 0, 255, 0.6),
                new PenetrationColor('Green', 'Green', 0.2, 0.4, 0, 255, 0, 0.6),
                new PenetrationColor('Yellow', 'Yellow', 0.4, 0.6, 255, 255, 0, 0.6),
                new PenetrationColor('Orange', '#ff9500', 0.6, 0.8, 255, 150, 0, 0.7),
                new PenetrationColor('Red', '#bb0000', 0.8, 1.0, 187, 0, 0, 0.6)],
 _newsettings: [new PenetrationColor('Blue', 'Blue', 0.0, 0.2, 0, 0, 255, 0.6),
                new PenetrationColor('Green', 'Green', 0.2, 0.4, 0, 255, 0, 0.6),
                new PenetrationColor('Yellow', 'Yellow', 0.4, 0.6, 255, 255, 0, 0.6),
                new PenetrationColor('Orange', '#ff9500', 0.6, 0.8, 255, 150, 0, 0.7),
                new PenetrationColor('Red', '#bb0000', 0.8, 1.0, 187, 0, 0, 0.6)],
    GetColor: function(penetration, isNew, isDisabled) {
        var color = null;
        var settings = null;
        if (isNew && this._newsettings) { settings = this._newsettings; }
        else { settings = this._settings; }
        var i = 0;
        var length = settings.length;
        while (i < length) {
            if (settings[i].Contains(penetration)) {
                color = settings[i];
                break;
            }
            i++;
        }
        if (!color) {
            if (isDisabled) {
                color = new PenetrationColor("Disabled", '', -1, 0, 0, 0, 0, 0.3);
            }
            else {
                color = new PenetrationColor("Default", '', -1, 0, 66, 89, 204, 0.1);
            }
            if (isNew) { color.A = 0.1; }
        }
        return color;
    },

    GetColorPoints: function(isNew) {
        var settings = null;
        if (isNew && this._newsettings) { settings = this._newsettings; }
        else { settings = this._settings; }
        return [settings[0].Max, settings[1].Max, settings[2].Max, settings[3].Max];
    },

    SetColorPoint: function(index, p, isNew) {
        var settings = null;
        if (isNew && this._newsettings) { settings = this._newsettings; }
        else { settings = this._settings; }

        var distance = Math.round((p - settings[index].Min) * 100) / 100;


        if (distance > 0) {
            for (var i = index; i < 5; i++) {
                settings[i].Max += distance;
                settings[i].Min += distance;
            }
        }
        //        else {
        //            for (var i = index - 1; i >= 0; i--) {
        //                settings[i].Max += distance;
        //                settings[i].Min += distance;
        //            }
        //        }

        settings[index - 1].Max = p;
        settings[index].Min = p;
    },

    GetColorPoints: function(isNew) {
        var settings = null;
        if (isNew && this._newsettings) { settings = this._newsettings; }
        else { settings = this._settings; }
        var points = [];
        for (var i in settings) {
            points.push(settings[i].Max);
        }
        return points;
    },

    GetColors: function(isNew) {
        var color = null;
        var settings = null;
        if (isNew && this._newsettings) { settings = this._newsettings; }
        else { settings = this._settings; }
        return settings;
    },

    Serialize: function(isNew) {
        var settings = null;
        if (isNew && this._newsettings) { settings = this._newsettings; }
        else { settings = this._settings; }
        var items = [];
        var i = 0;
        var length = settings.length;
        while (i < length) {
            if (settings[i].Max > 0 && settings[i].Min < 1) {
                items.push(settings[i].Serialize());
            }
            i++;
        }
        return items.join(';');
    },

    GetCollegendStr: function(isNew) {
        var settings = null;
        var retStr;
        var tempStr = "<td width=\"30px\"><div style=\"height: 12px; width: 25px; background-color:{1};\"></div></td><td width=\"155px\">{2}({3}%-{4}%)</td>";
        var colStr;
        var endStr = "</tr></table>";
        if (isNew && this._newsettings) { settings = this._newsettings; }
        else { settings = this._settings; }
        var items = [];
        var i = 0;
        var len = 0;
        var length = settings.length;
        var tabWidth;
        retStr = "<table cellpadding='0' cellspacing='0'><tr><td>COLOR LEGEND</td></tr></table><div class='spaceline'></div><table style=\"width:{0}px\" cellpadding=\"0\" ><tr>";
        while (i < length) {
            if (settings[i].Max > 0 && settings[i].Min < 1) {
                var temp = tempStr;
                colStr = temp.replace("{1}", settings[i].HtmlValue).replace("{2}", settings[i].Name).replace("{3}", (settings[i].Min * 100).toFixed(0)).replace("{4}", (settings[i].Max * 100).toFixed(0));
                retStr = retStr + colStr;
                len++;
            }
            i++;
        }
        if (len == 0)
            retStr = "<div></div>";
        else {
            retStr = retStr.replace("{0}", 180 * len);
            retStr = retStr + endStr;
        }
        return retStr;
    }
}

