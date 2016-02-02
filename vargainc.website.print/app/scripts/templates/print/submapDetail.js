define(['handlebars'], function(Handlebars) {

return Handlebars.template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    var stack1, alias1=container.lambda, alias2=container.escapeExpression;

  return "<div class=\"row\">\n  <div class=\"small-12 columns text-center title\"><span class=\"editable\" role=\"title\">CARRIER ROUTES CONTAINED IN SUBM MAP "
    + alias2(alias1(((stack1 = (depth0 != null ? depth0.submap : depth0)) != null ? stack1.OrderId : stack1), depth0))
    + " ("
    + alias2(alias1(((stack1 = (depth0 != null ? depth0.submap : depth0)) != null ? stack1.Name : stack1), depth0))
    + ")</span></div>\n</div>\n<div class=\"row\" role=\"table\" datasource=\"submap-detail\">\n  <table>\n    <thead>\n      <tr>\n        <th><span class=\"editable\">#</span></th>\n        <th><span class=\"editable\">CARRIER ROUTE #</span></th>\n        <th><span class=\"editable\">TOTAL H/H</span></th>\n        <th><span class=\"editable\">TARGET H/H</span></th>\n        <th><span class=\"editable\">PENETRATION</span></th>\n      </tr>\n    </thead>\n    <tbody class=\"detail-body\">\n    </tbody>\n  </table>\n</div>\n"
    + ((stack1 = container.invokePartial(partials.footer,depth0,{"name":"footer","data":data,"helpers":helpers,"partials":partials,"decorators":container.decorators})) != null ? stack1 : "");
},"usePartial":true,"useData":true})

});