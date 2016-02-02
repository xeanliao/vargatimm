define(['handlebars'], function(Handlebars) {

return Handlebars.template({"1":function(container,depth0,helpers,partials,data) {
    var helper, alias1=depth0 != null ? depth0 : {}, alias2=helpers.helperMissing, alias3="function", alias4=container.escapeExpression;

  return "        <tr>\n          <td>"
    + alias4(((helper = (helper = helpers.OrderId || (depth0 != null ? depth0.OrderId : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"OrderId","hash":{},"data":data}) : helper)))
    + "</td>\n          <td>"
    + alias4(((helper = (helper = helpers.Name || (depth0 != null ? depth0.Name : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"Name","hash":{},"data":data}) : helper)))
    + "</td>\n          <td>"
    + alias4((helpers.formatNumber || (depth0 && depth0.formatNumber) || alias2).call(alias1,(depth0 != null ? depth0.TotalHouseHold : depth0),{"name":"formatNumber","hash":{},"data":data}))
    + "</td>\n          <td>"
    + alias4((helpers.formatNumber || (depth0 && depth0.formatNumber) || alias2).call(alias1,(depth0 != null ? depth0.TargetHouseHold : depth0),{"name":"formatNumber","hash":{},"data":data}))
    + "</td>\n          <td>"
    + alias4((helpers.formatNumber || (depth0 && depth0.formatNumber) || alias2).call(alias1,(depth0 != null ? depth0.Penetration : depth0),{"name":"formatNumber","hash":{"style":"percent"},"data":data}))
    + "</td>\n        </tr>\n";
},"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    var stack1;

  return "<div class=\"row\">\n  <div class=\"small-12 columns text-center title\"><span class=\"editable\" role=\"title\">Summary of Sub Maps</span></div>\n</div>\n<div class=\"row collapse\" role=\"table\" datasource=\"submap-list\">\n  <div class=\"small-12 columns\">\n    <table>\n      <thead>\n        <tr>\n          <th><span class=\"editable\">#</span></th>\n          <th><span class=\"editable\">SUB MAP NAME</span></th>\n          <th><span class=\"editable\">TOTAL H/H</span></th>\n          <th><span class=\"editable\">TARGET H/H</span></th>\n          <th><span class=\"editable\">PENETRATION</span></th>\n        </tr>\n      </thead>\n      <tbody>\n"
    + ((stack1 = helpers.each.call(depth0 != null ? depth0 : {},(depth0 != null ? depth0.SubMaps : depth0),{"name":"each","hash":{},"fn":container.program(1, data, 0),"inverse":container.noop,"data":data})) != null ? stack1 : "")
    + "      </tbody>\n    </table>\n  </div>\n</div>\n"
    + ((stack1 = container.invokePartial(partials.footer,depth0,{"name":"footer","data":data,"helpers":helpers,"partials":partials,"decorators":container.decorators})) != null ? stack1 : "");
},"usePartial":true,"useData":true})

});