define(['handlebars'], function(Handlebars) {

return Handlebars.template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    var stack1, helper, alias1=container.lambda, alias2=container.escapeExpression, alias3=depth0 != null ? depth0 : {}, alias4=helpers.helperMissing;

  return "<div class=\"row\">\n  <div class=\"small-12 columns text-center title\"><span class=\"editable\" role=\"title\">DM MAP "
    + alias2(alias1(((stack1 = (depth0 != null ? depth0.dmap : depth0)) != null ? stack1.Id : stack1), depth0))
    + "("
    + alias2(alias1(((stack1 = (depth0 != null ? depth0.dmap : depth0)) != null ? stack1.Name : stack1), depth0))
    + ")</span></div>\n</div>\n<div class=\"row list\" role=\"list\">\n  <div class=\"small-4 columns\">&nbsp;<span class=\"editable\" role=\"key\">DM MAP #:</span></div>\n  <div class=\"small-8 columns\">&nbsp;<span class=\"editable\" role=\"text\">"
    + alias2(alias1(((stack1 = (depth0 != null ? depth0.dmap : depth0)) != null ? stack1.Id : stack1), depth0))
    + "</span></div>\n  <div class=\"small-4 columns\">&nbsp;<span class=\"editable\" role=\"key\">DISTRIBUTION MAP NAME</span>:</div>\n  <div class=\"small-8 columns\">&nbsp;<span class=\"editable\" role=\"text\">"
    + alias2(alias1(((stack1 = (depth0 != null ? depth0.dmap : depth0)) != null ? stack1.Name : stack1), depth0))
    + "</span></div>\n  <div class=\"small-4 columns\">&nbsp;<span class=\"editable\" role=\"key\">TOTAL</span>:</div>\n  <div class=\"small-8 columns\">&nbsp;<span class=\"editable\" role=\"text\">"
    + alias2((helpers.formatNumber || (depth0 && depth0.formatNumber) || alias4).call(alias3,((stack1 = (depth0 != null ? depth0.dmap : depth0)) != null ? stack1.Total : stack1),{"name":"formatNumber","hash":{},"data":data}))
    + "</span></div>\n</div>\n<div class=\"row\">\n  <div class=\"small-12 columns text-center title mapTitle\"><span class=\"editable\" role=\"title\">"
    + alias2(((helper = (helper = helpers.DisplayName || (depth0 != null ? depth0.DisplayName : depth0)) != null ? helper : alias4),(typeof helper === "function" ? helper.call(alias3,{"name":"DisplayName","hash":{},"data":data}) : helper)))
    + " - "
    + alias2(alias1(((stack1 = (depth0 != null ? depth0.dmap : depth0)) != null ? stack1.Name : stack1), depth0))
    + "</span></div>\n</div>\n<div class=\"row collapse\">\n  <div class=\"small-12 columns\">\n    <div class=\"map-container\" role=\"map\">\n      <div class=\"loading hexdots-loader\"></div>\n    </div>\n  </div>\n</div>";
},"useData":true})

});