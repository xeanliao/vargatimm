define(['handlebars'], function(Handlebars) {

return Handlebars.template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    var stack1, helper, alias1=depth0 != null ? depth0 : {}, alias2=helpers.helperMissing, alias3="function", alias4=container.escapeExpression;

  return "<div class=\"row\">\n  <div class=\"small-12 columns text-center title\"><span class=\"editable\" role=\"title\">Campaign Summary</span></div>\n</div>\n<div class=\"row list\" role=\"list\">\n  <div class=\"small-4 columns\">&nbsp;<span class=\"editable\" role=\"key\">MASTER CAMPAIGN #:</span></div>\n  <div class=\"small-8 columns\">&nbsp;<span class=\"editable\" role=\"text\">"
    + alias4(((helper = (helper = helpers.DisplayName || (depth0 != null ? depth0.DisplayName : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"DisplayName","hash":{},"data":data}) : helper)))
    + "</span></div>\n  <div class=\"small-4 columns\">&nbsp;<span class=\"editable\" role=\"key\">CLIENT NAME:</span></div>\n  <div class=\"small-8 columns\">&nbsp;<span class=\"editable\" role=\"text\">"
    + alias4(((helper = (helper = helpers.ClientName || (depth0 != null ? depth0.ClientName : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"ClientName","hash":{},"data":data}) : helper)))
    + "</span></div>\n  <div class=\"small-4 columns\">&nbsp;<span class=\"editable\" role=\"key\">CONTACT NAME:</span></div>\n  <div class=\"small-8 columns\">&nbsp;<span class=\"editable\" role=\"text\">"
    + alias4(((helper = (helper = helpers.ContactName || (depth0 != null ? depth0.ContactName : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"ContactName","hash":{},"data":data}) : helper)))
    + "</span></div>\n  <div class=\"small-4 columns\">&nbsp;<span class=\"editable\" role=\"key\">TARGETING METHOD:</span></div>\n  <div class=\"small-8 columns\">&nbsp;<span class=\"editable\" role=\"text\">"
    + alias4(((helper = (helper = helpers.targetMethod || (depth0 != null ? depth0.targetMethod : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"targetMethod","hash":{},"data":data}) : helper)))
    + "</span></div>\n  <div class=\"small-4 columns\">&nbsp;<span class=\"editable\" role=\"key\">TOTAL HOUSEHOLDS:</span></div>\n  <div class=\"small-8 columns\">&nbsp;<span class=\"editable\" role=\"text\">"
    + alias4((helpers.formatNumber || (depth0 && depth0.formatNumber) || alias2).call(alias1,(depth0 != null ? depth0.TotalHouseHold : depth0),{"name":"formatNumber","hash":{},"data":data}))
    + "</span></div>\n  <div class=\"small-4 columns\">&nbsp;<span class=\"editable\" role=\"key\">TARGET HOUSEHOLDS:</span></div>\n  <div class=\"small-8 columns\">&nbsp;<span class=\"editable\" role=\"text\">"
    + alias4((helpers.formatNumber || (depth0 && depth0.formatNumber) || alias2).call(alias1,(depth0 != null ? depth0.TargetHouseHold : depth0),{"name":"formatNumber","hash":{},"data":data}))
    + "</span></div>\n  <div class=\"small-4 columns\">&nbsp;<span class=\"editable\" role=\"key\">PENETRATION:</span></div>\n  <div class=\"small-8 columns\">&nbsp;<span class=\"editable\" role=\"text\">"
    + alias4((helpers.formatNumber || (depth0 && depth0.formatNumber) || alias2).call(alias1,(depth0 != null ? depth0.Penetration : depth0),{"name":"formatNumber","hash":{"style":"percent"},"data":data}))
    + "</span></div>\n</div>\n<div class=\"row\">\n  <div class=\"small-12 columns text-center title\"><span class=\"editable\" role=\"title\">Campaign Summary Map</span></div>\n</div>\n<div class=\"row collapse\">\n  <div class=\"small-12 columns\">\n    <div class=\"map-container\" role=\"map\" color=\"false\" legend=\"true\">\n      <div class=\"loading hexdots-loader\"></div>\n    </div>\n    <div class=\"small-12 columns\">\n      <div class=\"color-legend\">\n      </div>\n      <div class=\"direction-legend\"></div>\n    </div>\n  </div>\n</div>\n"
    + ((stack1 = container.invokePartial(partials.footer,depth0,{"name":"footer","data":data,"helpers":helpers,"partials":partials,"decorators":container.decorators})) != null ? stack1 : "");
},"usePartial":true,"useData":true})

});