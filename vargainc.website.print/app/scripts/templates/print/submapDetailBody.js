define(['handlebars'], function(Handlebars) {

return Handlebars.template({"1":function(container,depth0,helpers,partials,data) {
    var stack1, helper, alias1=depth0 != null ? depth0 : {}, alias2=helpers.helperMissing, alias3=container.escapeExpression;

  return "<tr>\n  <td>"
    + alias3((helpers.math || (depth0 && depth0.math) || alias2).call(alias1,(data && data.index),"+",1,{"name":"math","hash":{},"data":data}))
    + "</td>\n  <td>"
    + alias3(((helper = (helper = helpers.Name || (depth0 != null ? depth0.Name : depth0)) != null ? helper : alias2),(typeof helper === "function" ? helper.call(alias1,{"name":"Name","hash":{},"data":data}) : helper)))
    + "</td>\n  <td>\n"
    + ((stack1 = helpers["if"].call(alias1,(depth0 != null ? depth0.TotalHouseHold : depth0),{"name":"if","hash":{},"fn":container.program(2, data, 0),"inverse":container.program(4, data, 0),"data":data})) != null ? stack1 : "")
    + "  </td>\n  <td>\n"
    + ((stack1 = helpers["if"].call(alias1,(depth0 != null ? depth0.TargetHouseHold : depth0),{"name":"if","hash":{},"fn":container.program(6, data, 0),"inverse":container.program(4, data, 0),"data":data})) != null ? stack1 : "")
    + "  </td>\n  <td>\n"
    + ((stack1 = helpers["if"].call(alias1,(depth0 != null ? depth0.Penetration : depth0),{"name":"if","hash":{},"fn":container.program(8, data, 0),"inverse":container.program(10, data, 0),"data":data})) != null ? stack1 : "")
    + "  </td>\n</tr>\n";
},"2":function(container,depth0,helpers,partials,data) {
    return "		"
    + container.escapeExpression((helpers.formatNumber || (depth0 && depth0.formatNumber) || helpers.helperMissing).call(depth0 != null ? depth0 : {},(depth0 != null ? depth0.TotalHouseHold : depth0),{"name":"formatNumber","hash":{},"data":data}))
    + "\n";
},"4":function(container,depth0,helpers,partials,data) {
    return "		0\n";
},"6":function(container,depth0,helpers,partials,data) {
    return "  		"
    + container.escapeExpression((helpers.formatNumber || (depth0 && depth0.formatNumber) || helpers.helperMissing).call(depth0 != null ? depth0 : {},(depth0 != null ? depth0.TargetHouseHold : depth0),{"name":"formatNumber","hash":{},"data":data}))
    + "\n";
},"8":function(container,depth0,helpers,partials,data) {
    return "  		"
    + container.escapeExpression((helpers.formatNumber || (depth0 && depth0.formatNumber) || helpers.helperMissing).call(depth0 != null ? depth0 : {},(depth0 != null ? depth0.Penetration : depth0),{"name":"formatNumber","hash":{"style":"percent"},"data":data}))
    + "\n";
},"10":function(container,depth0,helpers,partials,data) {
    return "		0%\n";
},"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    var stack1;

  return ((stack1 = helpers.each.call(depth0 != null ? depth0 : {},depth0,{"name":"each","hash":{},"fn":container.program(1, data, 0),"inverse":container.noop,"data":data})) != null ? stack1 : "");
},"useData":true})

});