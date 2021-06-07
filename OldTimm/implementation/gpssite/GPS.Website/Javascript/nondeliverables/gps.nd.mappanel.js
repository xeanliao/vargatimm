
//
GPS.Nd.MapPanel = GPS.Map.MapBase.extend({
    init: function(options) {
        options.Symbol = "ndmap";
        this._super(options);
        this.SetClassificationVisible(13, true);
        this.SetClassificationVisible(14, true);
//        this.TestShape();
        //        this.SetClassificationVisible(15, true);
    },
    //VEMap onchangeview event method
    _OnClick: function(e) {
        this._super(e);
        if (e.rightMouseButton && e.elementID) {
            var area = this._GetAreaByElementId([1, 2, 3, 13, 14], e.elementID);
            if (area && (!area.GetEnabled())) {
                var inSpeed = 150;
                if ($("#area-context-memu").length > 0) {
                    $("#area-context-memu").remove();
                }
                var menus = [];
                if (area.GetEnabled()) {
                    menus.push('<li><a href="#add">Add to Non-Deliverables</a></li>');
                }
                else {
                    menus.push('<li><a href="#remove">Remove from Non-Deliverables</a></li>');
                }

                $("body").append('<ul id="area-context-memu" class="contextMenu">' + menus.join('') + '</ul>');
                var menu = $('#area-context-memu');

                $(menu).find('A').mouseover(function() {
                    $(menu).find('LI.hover').removeClass('hover');
                    $(this).parent().addClass('hover');
                }).mouseout(function() {
                    $(menu).find('LI.hover').removeClass('hover');
                });
                var thisObj = this;
                $(menu).find('A').unbind('click');
                $(menu).find('LI:not(.disabled) A').click(function() {
                    $(document).unbind('click').unbind('keypress');
                    $(".contextMenu").hide();
                    // Callback
                    thisObj._OnClickPopMenu(area, $(this).attr('href').substr($(this).attr('href').lastIndexOf('#') + 1));
                    return false;
                });

                $("#area-context-memu").css({ top: e.clientY, left: e.clientX }).fadeIn(inSpeed);
            }
        }
        else {
            $(".contextMenu").hide();
        }
    },
    _OnClickPopMenu: function(area, action) {
        if (action == "remove") {
            // custom area
            if (area.GetClassification() == 13) {
                var service = new TIMM.Website.AreaServices.AreaWriterService();
                var thisObj = this;
                service.RemoveCustomArea(area.GetName(), function(ret) {
                    if (ret.IsSuccess) {
                        thisObj._areaLayers[13].RemoveAreaById(area.GetId());
                    }
                    else {
                        GPSAlert(ret.Message);
                    }
                });
            }
            else if (area.GetClassification() == 14) {
                var service = new TIMM.Website.AreaServices.AreaWriterService();
                var thisObj = this;
                service.RemoveNonDeliverableAddress(Number(area.GetId()), function(ret) {
                    if (ret.IsSuccess) {
                        thisObj._areaLayers[14].RemoveAreaById(area.GetId());
                    }
                    else {
                        GPSAlert(ret.Message);
                    }
                });
            }
            else if (area.GetClassification() == 1) {
                this.SetFiveZipAreaEnabled({
                    Code: area.GetName(),
                    Total: 0,
                    Description: '',
                    Enabled: true
                });
            }
            else if (area.GetClassification() == 2) {
                this.SetTractAreaEnabled({
                    StateCode: area.GetAttributes().State,
                    CountyCode: area.GetAttributes().County,
                    Code: area.GetAttributes().Tract,
                    Total: 0,
                    Description: '',
                    Enabled: true
                });
            }
            else if (area.GetClassification() == 3) {
                this.SetBlockGroupAreaEnabled({
                    StateCode: area.GetAttributes().State,
                    CountyCode: area.GetAttributes().County,
                    TractCode: area.GetAttributes().Tract,
                    Code: area.GetAttributes().BlockGroup,
                    Total: 0,
                    Description: '',
                    Enabled: true
                });
            }
        }
    },
    //add custom area
    AddCustomArea: function(options) {
        this._areaLayers[13].AddArea(options);
    },
    //add custom area
    AddNonDeliverableAddress: function(options) {
        options.IsEnabled = false;
        if (options.Location) {
            this._map.SetCenterAndZoom(options.Location, 13);
        }
        this._areaLayers[14].AddArea(options);
    },
    //add custom area
    AddNonDeliverableAddresses: function(options) {
        var locations = [];
        if (options.length > 0) {
            this._areaLayers[14].ReLoad();
            this._map.SetCenterAndZoom(options[0].Location, 12);
        }
    },
    /* set five zip area enabled
    *   @options    Code: zip code
    *               Total: number
    *               Description: string
    *               Enabled: bool
    *               Location: VELatLong
    */
    SetFiveZipAreaEnabled: function(options) {
        var thisObj = this;
        var service = new TIMM.Website.AreaServices.AreaWriterService()
        service.SetFiveZipEnable(options.Code, options.Total, options.Description, options.Enabled, function(ret) {
            if (ret.IsSuccess) {
                thisObj._areaLayers[1].ReLoad();
                thisObj._areaLayers[2].ReLoad();
                thisObj._areaLayers[3].ReLoad();
                if (options.Location) {
                    thisObj._map.SetCenterAndZoom(options.Location, 12);
                }
            }
        });
    },
    /* set tract area enabled
    *   @options    StateCode: string
    *               CountyCode: string
    *               Code: string
    *               Total: number
    *               Description: string
    *               Enabled: bool
    *               Location: VELatLong
    */
    SetTractAreaEnabled: function(options) {
        var thisObj = this;
        var service = new TIMM.Website.AreaServices.AreaWriterService()
        service.SetTractEnable(options.StateCode, options.CountyCode, options.Code, options.Total, options.Description, options.Enabled, function(ret) {
            if (ret.IsSuccess) {
                thisObj._areaLayers[2].ReLoad();
                thisObj._areaLayers[3].ReLoad();
                if (options.Location) {
                    thisObj._map.SetCenterAndZoom(options.Location, 13);
                }
            }

        });
    },
    /* set tract area enabled
    *   @options    StateCode: string
    *               CountyCode: string
    *               TractCode: string
    *               Code: string
    *               Total: number
    *               Description: string
    *               Enabled: bool
    *               Location: VELatLong
    */
    SetBlockGroupAreaEnabled: function(options) {
        var thisObj = this;
        var service = new TIMM.Website.AreaServices.AreaWriterService()
        service.SetBlockGroupEnable(options.StateCode, options.CountyCode, options.TractCode, options.Code, options.Total, options.Description, options.Enabled, function(ret) {
            if (ret.IsSuccess) {
                thisObj._areaLayers[3].ReLoad();
                if (options.Location) {
                    thisObj._map.SetCenterAndZoom(options.Location, 14);
                }
            }

        });
    }
});

GPS.Nd.MapPanel.AreaLayer = GPS.Map.AreaLayerBase.extend({
    init: function(options) {
        options.Symbol = "ndarealayer";
        this._super(options);
    }
});

GPS.Nd.MapPanel.Area = GPS.Map.AreaBase.extend({
    init: function(options) {
        options.Symbol = "ndarea";
        this._super(options);
    }
});