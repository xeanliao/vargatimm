define(['handlebars'], function(Handlebars) {

return Handlebars.template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    var stack1;

  return "<div class=\"row\">\n  <div class=\"small-12 columns text-center title\">Summary of Sub Maps</div>\n</div>\n<table>\n  <thead>\n    <tr>\n      <th>#</th>\n      <th>ZIP CODE</th>\n      <th>TOTAL H/H</th>\n      <th>TARGET H/H</th>\n      <th>PENETRATION</th>\n    </tr>\n  </thead>\n</table>\n"
    + ((stack1 = container.invokePartial(partials.footer,depth0,{"name":"footer","data":data,"helpers":helpers,"partials":partials,"decorators":container.decorators})) != null ? stack1 : "");
},"usePartial":true,"useData":true})

});