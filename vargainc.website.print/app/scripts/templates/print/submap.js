define(['handlebars'], function(Handlebars) {

return Handlebars.template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    var stack1, alias1=container.lambda, alias2=container.escapeExpression, alias3=depth0 != null ? depth0 : {}, alias4=helpers.helperMissing;

  return "<div class=\"row\">\n  <div class=\"small-12 columns text-center title\"><span class=\"editable\" role=\"title\">SUB MAP "
    + alias2(alias1(((stack1 = (depth0 != null ? depth0.submap : depth0)) != null ? stack1.OrderId : stack1), depth0))
    + "("
    + alias2(alias1(((stack1 = (depth0 != null ? depth0.submap : depth0)) != null ? stack1.Name : stack1), depth0))
    + ")</span></div>\n</div>\n<div class=\"row list\" role=\"list\">\n  <div class=\"small-4 columns\">&nbsp;<span class=\"editable\" role=\"key\">SUB MAP #</span>:</div>\n  <div class=\"small-8 columns\">&nbsp;<span class=\"editable\" role=\"text\">"
    + alias2(alias1(((stack1 = (depth0 != null ? depth0.submap : depth0)) != null ? stack1.OrderId : stack1), depth0))
    + "</span></div>\n  <div class=\"small-4 columns\">&nbsp;<span class=\"editable\" role=\"key\">SUB MAP NAME</span>:</div>\n  <div class=\"small-8 columns\">&nbsp;<span class=\"editable\" role=\"text\">"
    + alias2(alias1(((stack1 = (depth0 != null ? depth0.submap : depth0)) != null ? stack1.Name : stack1), depth0))
    + "</span></div>\n  <div class=\"small-4 columns\">&nbsp;<span class=\"editable\" role=\"key\">TARGETING METHOD</span>:</div>\n  <div class=\"small-8 columns\">&nbsp;<span class=\"editable\" role=\"text\">"
    + alias2(alias1(((stack1 = (depth0 != null ? depth0.campaign : depth0)) != null ? stack1.targetMethod : stack1), depth0))
    + "</span></div>\n  <div class=\"small-4 columns\">&nbsp;<span class=\"editable\" role=\"key\">TOTAL HOUSEHOLDS</span>:</div>\n  <div class=\"small-8 columns\">&nbsp;<span class=\"editable\" role=\"text\">"
    + alias2((helpers.formatNumber || (depth0 && depth0.formatNumber) || alias4).call(alias3,((stack1 = (depth0 != null ? depth0.submap : depth0)) != null ? stack1.TotalHouseHold : stack1),{"name":"formatNumber","hash":{},"data":data}))
    + "</span></div>\n  <div class=\"small-4 columns\">&nbsp;<span class=\"editable\" role=\"key\">TARGET HOUSEHOLDS</span>:</div>\n  <div class=\"small-8 columns\">&nbsp;<span class=\"editable\" role=\"text\">"
    + alias2((helpers.formatNumber || (depth0 && depth0.formatNumber) || alias4).call(alias3,((stack1 = (depth0 != null ? depth0.submap : depth0)) != null ? stack1.TargetHouseHold : stack1),{"name":"formatNumber","hash":{},"data":data}))
    + "</span></div>\n  <div class=\"small-4 columns\">&nbsp;<span class=\"editable\" role=\"key\">PENETRATION</span>:</div>\n  <div class=\"small-8 columns\">&nbsp;<span class=\"editable\" role=\"text\">"
    + alias2((helpers.formatNumber || (depth0 && depth0.formatNumber) || alias4).call(alias3,((stack1 = (depth0 != null ? depth0.submap : depth0)) != null ? stack1.Penetration : stack1),{"name":"formatNumber","hash":{"style":"percent"},"data":data}))
    + "</span></div>\n</div>\n<div class=\"row\">\n  <div class=\"small-12 columns text-center title\"><span class=\"editable\" role=\"title\">Map</span></div>\n</div>\n<div class=\"row collapse\">\n  <div class=\"small-12 columns\">\n    <div class=\"map-container\" role=\"map\" color=\"false\" legend=\"true\">\n      <div class=\"loading hexdots-loader\"></div>\n    </div>\n  </div>\n  <div class=\"small-12 columns\">\n      <div class=\"color-legend\">\n      </div>\n      <div class=\"direction-legend\"></div>\n    </div>\n</div>\n"
    + ((stack1 = container.invokePartial(partials.footer,depth0,{"name":"footer","data":data,"helpers":helpers,"partials":partials,"decorators":container.decorators})) != null ? stack1 : "");
},"usePartial":true,"useData":true})

});