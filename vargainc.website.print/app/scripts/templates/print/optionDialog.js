define(['handlebars'], function(Handlebars) {

return Handlebars.template({"1":function(container,depth0,helpers,partials,data) {
    var helper, alias1=depth0 != null ? depth0 : {}, alias2=helpers.helperMissing, alias3="function", alias4=container.escapeExpression;

  return "                <li><input id=\"optionDmap-"
    + alias4(((helper = (helper = helpers.Id || (depth0 != null ? depth0.Id : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"Id","hash":{},"data":data}) : helper)))
    + "\" type=\"checkbox\"><label for=\"optionDmap-"
    + alias4(((helper = (helper = helpers.Id || (depth0 != null ? depth0.Id : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"Id","hash":{},"data":data}) : helper)))
    + "\">"
    + alias4(((helper = (helper = helpers.Name || (depth0 != null ? depth0.Name : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"Name","hash":{},"data":data}) : helper)))
    + "</label> </li>\n";
},"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    var stack1;

  return "<h4>Print Options</h4>\n<form>\n    <div class=\"row\">\n        <div class=\"small-2 columns\">\n            <label>Map Type</label>\n        </div>\n        <div class=\"small-10 columns mapTypeContainer\">\n            <ul class=\"button-group\">\n                <li>\n                    <input id=\"mapType-ROADMAP\" type=\"radio\" name='mapType' /><label for=\"mapType-ROADMAP\">RoadMap</label>\n                </li>\n                <li>\n                    <input id=\"mapType-HYBRID\" type=\"radio\" name='mapType' /><label for=\"mapType-HYBRID\">Aerial</label>\n                </li>\n            </ul>\n        </div>\n    </div>\n    <div class=\"row hideForDistribute\">\n        <div class=\"small-12 columns\">\n            <label>\n                Target Method\n                <input id=\"targetMethod\" type=\"text\" />\n            </label>\n        </div>\n    </div>\n    <div class=\"panel hideForDistribute\">\n        <h6>Campaign Maps</h6>\n        <ul class=\"small-block-grid-2\">\n            <li>\n                <input id=\"suppressCover\" type=\"checkbox\" /><label for=\"suppressCover\">Suppress Cover</label>\n            </li>\n            <li>\n                <input id=\"suppressCampaign\" type=\"checkbox\" /><label for=\"suppressCampaign\">Suppress Campaign Page</label>\n            </li>\n            <li>\n                <input id=\"suppressSubMap\" type=\"checkbox\" /><label for=\"suppressSubMap\">Suppress Sub Maps</label>\n            </li>\n            <li>\n                <input id=\"suppressCampaignSummary\" type=\"checkbox\" /><label for=\"suppressCampaignSummary\">Suppress Sub Map Summary</label>\n            </li>\n            <li>\n                <input id=\"suppressSubMapCountDetail\" type=\"checkbox\" /><label for=\"suppressSubMapCountDetail\">Suppress Sub Map Croute Counts</label>\n            </li>\n            <li>\n                <input id=\"suppressNDAInCampaign\" type=\"checkbox\"><label for=\"suppressNDAInCampaign\">Suppress non-deliverables for campaign map</label>  \n            </li>\n            <li>\n                <input id=\"suppressNDAInSubMap\" type=\"checkbox\"><label for=\"suppressNDAInSubMap\">Suppress non-deliverables for sub map</label>  \n            </li>\n            <li>\n                <input id=\"suppressLocations\" type=\"checkbox\" /><label for=\"suppressLocations\">Suppress Locations</label>\n            </li>\n            <li>\n                <input id=\"suppressRadii\" type=\"checkbox\" /><label for=\"suppressRadii\">Suppress Radii</label>\n            </li>\n        </ul>\n        <ul class=\"small-block-grid-1\">\n            <li>\n                <input id=\"showPenetrationColors\" type=\"checkbox\"><label for=\"showPenetrationColors\">Show Penetration Colors:</label> \n            </li>\n            <li>\n                <div class=\"row\">\n                    <div class=\"custom-colors small-10 columns\">\n                        <label>\n                            <span>Blue (0% - 20%) Green (20% - 40%) Yellow (40% - 60%) Orange (60% - 80%) Red (80% - 100%) </span>\n                            <div id=\"colorSlider\" class=\"slider\"></div>\n                        </label>\n                    </div>\n                    <div class=\"small-2 columns\">\n                        <button id=\"btnRestPenetration\" class=\"tiny alert\">Reset</button>\n                    </div>\n                </div>            \n            </li>\n        </ul>\n    </div>\n    <div class=\"panel hideForDistribute hideForCampaign\">\n        <h6>GTU Reports</h6>\n        <ul class=\"small-block-grid-2\">\n            <li>\n                <input id=\"suppressGTU\" type=\"checkbox\" /><label for=\"suppressGTU\">Suppress GTU Tracking</label>\n            </li>\n        </ul>\n    </div>\n    <div class=\"panel hideForCampaign\">\n        <h6>Distribution Maps</h6>\n        <ul class=\"small-block-grid-1\">\n            <li>\n                <input id=\"suppressDMap\" type=\"checkbox\" /><label for=\"suppressDMap\">Suppress Distribution Maps</label>\n            </li>\n            <li>\n                <input id=\"suppressNDAInDMap\" type=\"checkbox\"><label for=\"suppressNDAInDMap\">Suppress non-deliverables for distribution map</label>  \n            </li>\n            \n        </ul>\n    </div>\n    <div class=\"panel callout radius dMapContainer hideForCampaign\">\n        <input id=\"btnCheckAllDMap\" class='btnCheckAllDMap' type=\"checkbox\"><label for=\"btnCheckAllDMap\">Suppress All Distribute Maps</label>\n        <ul class=\"small-block-grid-2 medium-block-grid-4 large-block-grid-6\">\n"
    + ((stack1 = helpers.each.call(depth0 != null ? depth0 : {},(depth0 != null ? depth0.DMaps : depth0),{"name":"each","hash":{},"fn":container.program(1, data, 0),"inverse":container.noop,"data":data})) != null ? stack1 : "")
    + "        </ul>\n    </div>\n</form>\n<a class=\"close-reveal-modal\" aria-label=\"Close\">&#215;</a>\n\n<button class=\"button tiny secondary btnCloseOption right\">Cancel</button>\n<button class=\"button tiny btnApplyOption right\">Apply</button>";
},"useData":true})

});