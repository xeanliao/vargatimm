
//GPS.Colors

GPS.Color = function(id, name, htmlValue, r, g, b, a) {
    this.Id = null;
    this.Name = null;
    this.HtmlValue = null;
    this.R = null;
    this.G = null;
    this.B = null;
    this.A = null;
    this.MapColor = null;
    this.__Init__(id, name, htmlValue, r, g, b, a);
};

GPS.Color.prototype = {
    __Init__: function(id, name, htmlValue, r, g, b, a) {
        this.Id = id;
        this.Name = name;
        this.HtmlValue = htmlValue;
        this.R = r;
        this.G = g;
        this.B = b;
        this.A = a;
        this.MapColor = new VEColor(r, g, b, a);
    }
};

GPS.Colors = {
    _colors: [
        new GPS.Color(1, 'Blue', '0000FF', 0, 0, 255, 0.5),
        new GPS.Color(2, 'Green', '008000', 0, 128, 0, 0.5),
        new GPS.Color(3, 'Yellow', 'FFFF00', 255, 255, 0, 0.5),
        new GPS.Color(4, 'Orange', 'F75600', 247, 86, 0, 0.5),
        new GPS.Color(5, 'Red', 'BB0000', 187, 0, 0, 0.5)],

    GetColorById: function(id) {
        var color = null;
        var i = 0;
        var length = this._colors.length;
        while (i < length) {
            if (id == this._colors[i].Id) {
                color = this._colors[i];
                break;
            }
            i++;
        }
        return color;
    },

    GetColorByName: function(name) {
        var color = null;
        var i = 0;
        var length = this._colors.length;
        while (i < length) {
            if (name == this._colors[i].Name) {
                color = this._colors[i];
                break;
            }
            i++;
        }
        return color;
    }
};



GPS.PenetrationColor = function(min, max, colorId) {
    this.Min = null;
    this.Max = null;
    this.ColorId = null;
    this.Color = null;
    this.__Init__(min, max, colorId);
};


GPS.PenetrationColor.prototype = {
    __Init__: function(min, max, colorId) {
        this.Min = min;
        this.Max = max;
        this.ColorId = colorId;
        this.Color = GPS.Colors.GetColorById(colorId);
    }
};

GPS.PenetrationColorManager = {
    _colors: [new GPS.PenetrationColor(0.0, 0.2, 1),
            new GPS.PenetrationColor(0.2, 0.4, 2),
            new GPS.PenetrationColor(0.4, 0.6, 3),
            new GPS.PenetrationColor(0.6, 0.8, 4),
            new GPS.PenetrationColor(0.8, 1.0, 5)],
    DefaultColors: function() {
        return [new GPS.PenetrationColor(0.0, 0.2, 1),
            new GPS.PenetrationColor(0.2, 0.4, 2),
            new GPS.PenetrationColor(0.4, 0.6, 3),
            new GPS.PenetrationColor(0.6, 0.8, 4),
            new GPS.PenetrationColor(0.8, 1.0, 5)];
    },

    Changed: false,

    GetColors: function() { return this._colors; },

    SetColors: function(colors) {
        GPS.Loading.show("Saving...");
        this._colors = colors;
        var i = 0;
        var length = this._colors.length;
        while (i < length) {
            if (this._colors[i].Max < 0 || this._colors[i].Min < 0) {
                this._colors[i].Max = -1;
                this._colors[i].Min = -1;
            }
            i++;
        }
        this.Changed = true;
        var toColors = this.GetPenetrationColors();
        var service = new TIMM.Website.CampaignServices.CampaignWriterService();
        service.SaveCampaignPercentageColors(campaign.Id, toColors, function() {
            if (mapPanel) { mapPanel.RefreshColors(); }
            GPS.Loading.hide();
        });
    },

    GetColorPoints: function() {
        var settings = this._colors;
        var points = [Math.round(settings[0].Max * 100),
         Math.round(settings[1].Max * 100),
         Math.round(settings[2].Max * 100),
         Math.round(settings[3].Max * 100)];
        return points;
    },

    SetColorPoints: function(points) {
        var settings = this._colors;
        settings[0].Max = points[0] / 100;
        settings[1].Max = points[1] / 100;
        settings[2].Max = points[2] / 100;
        settings[3].Max = points[3] / 100;
        settings[1].Min = points[0] / 100;
        settings[2].Min = points[1] / 100;
        settings[3].Min = points[2] / 100;
        settings[4].Min = points[3] / 100;
        if (map) { map.Refresh(); }
        this.Changed = true;
    },

    CopyColors: function() {
        var newColors = [];
        var i = 0;
        var length = this._colors.length;
        while (i < length) {
            var color = new GPS.PenetrationColor(this._colors[i].Min, this._colors[i].Max, this._colors[i].ColorId);
            newColors.push(color);
            i++;
        }
        return newColors;
    },

    BindCampaign: function(campaign) {
        if (campaign && campaign.CampaignPercentageColors && campaign.CampaignPercentageColors.length > 0) {
            this._colors = [];
            var i = 0;
            campaign.CampaignPercentageColors.sort(function(a, b) { return a.ColorId - b.ColorId; });
            var length = campaign.CampaignPercentageColors.length;
            while (i < length) {
                this._colors.push(new GPS.PenetrationColor(campaign.CampaignPercentageColors[i].Min, campaign.CampaignPercentageColors[i].Max, campaign.CampaignPercentageColors[i].ColorId));
                i++;
            }
        }
        else {
            this._colors = this.DefaultColors();
        }
        this.Changed = false;
    },

    GetPenetrationColorString: function() {
        var items = [];
        var settings = this._colors;
        var i = 0;
        var length = settings.length;
        while (i < length) {
            items.push(settings[i].Min + "," + settings[i].Max + "," + settings[i].ColorId);
            i++;
        }
        return items.join(";");
    },

    GetPenetrationColors: function() {
        var items = [];
        var settings = this._colors;
        var i = 0;
        var length = settings.length;
        while (i < length) {
            items.push({ Min: settings[i].Min, Max: settings[i].Max, ColorId: settings[i].ColorId });
            i++;
        }
        return items;
    },

    GetMapColor: function(per) {
        var color = null;
        var settings = this._colors;
        var i = 0;
        var length = settings.length;
        while (i < length) {
            if ((settings[i].Min <= per && settings[i].Max > per)
             || (settings[i].Max == 1 && per == 1)) {
                color = settings[i].Color;
                break;
            }
            i++;
        }
        return color;
    }
};

