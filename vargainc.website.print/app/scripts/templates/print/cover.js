define(['handlebars'], function(Handlebars) {

return Handlebars.template({"1":function(container,depth0,helpers,partials,data) {
    var helper;

  return "      <img class=\"client-logo\" src=\""
    + container.escapeExpression(((helper = (helper = helpers.Logo || (depth0 != null ? depth0.Logo : depth0)) != null ? helper : helpers.helperMissing),(typeof helper === "function" ? helper.call(depth0 != null ? depth0 : {},{"name":"Logo","hash":{},"data":data}) : helper)))
    + "\" alt=\"\" role=\"image\" p-height=\"80\" p-align=\"center\" p-top=\"40\" p-bottom=\"40\">\n";
},"3":function(container,depth0,helpers,partials,data) {
    return "      <div class=\"client-logo\" role=\"image\" src=\"\" role=\"image\" p-height=\"80\" p-align=\"center\" p-top=\"40\" p-bottom=\"40\" ></div>\n";
},"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    var stack1, helper, alias1=depth0 != null ? depth0 : {}, alias2=helpers.helperMissing, alias3="function", alias4=container.escapeExpression;

  return "<div class=\"row\">\n  <div class=\"small-12 columns text-center title\" ><span class=\"editable\" role=\"title\">This Custom Campaign is Presented to:</span></div>\n  </div>\n<div class=\"row\">\n  <div class=\"small-12 columns\">\n    <span class=\"hide\" role=\"key\"></span>\n    <div class=\"text-center\">\n"
    + ((stack1 = helpers["if"].call(alias1,(depth0 != null ? depth0.Logo : depth0),{"name":"if","hash":{},"fn":container.program(1, data, 0),"inverse":container.program(3, data, 0),"data":data})) != null ? stack1 : "")
    + "    </div>\n  </div>\n</div>\n<div class=\"row\">\n  <div class=\"small-12 columns text-center\"><label class=\"editable\" role=\"key\">Client Name:</label></div>\n  <div class=\"small-12 columns text-center\"><span class=\"editable\" role=\"text\">"
    + alias4(((helper = (helper = helpers.ClientName || (depth0 != null ? depth0.ClientName : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"ClientName","hash":{},"data":data}) : helper)))
    + "</span></div>\n</div>\n<div class=\"row\">\n  <div class=\"small-12 columns text-center\"><label class=\"editable\" role=\"key\">Created for:</label></div>\n  <div class=\"small-12 columns text-center\"><span class=\"editable\" role=\"text\">"
    + alias4(((helper = (helper = helpers.ContactName || (depth0 != null ? depth0.ContactName : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"ContactName","hash":{},"data":data}) : helper)))
    + "</span></div>\n</div>\n<div class=\"row\">\n  <div class=\"small-12 columns text-center\"><label class=\"editable\" role=\"key\">Created on:</label></div>\n  <div class=\"small-12 columns text-center\"><span class=\"editable\" role=\"text\">"
    + alias4(((helper = (helper = helpers.Date || (depth0 != null ? depth0.Date : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"Date","hash":{},"data":data}) : helper)))
    + "</span></div>\n</div>\n<div class=\"row\">\n  <div class=\"small-12 columns text-center\"><label class=\"\" role=\"key\">&nbsp;</label></div>\n  <div class=\"small-12 columns text-center\"><span class=\"\" role=\"text\">&nbsp;</span></div>\n</div>\n<div class=\"row\">\n  <div class=\"small-12 columns text-center\"><label class=\"editable\" role=\"key\">Presented by:</label></div>\n  <div class=\"small-12 columns text-center\"><img class=\"vargainc-log\" src=\"images/vargainc-logo.png\" role=\"image\" p-height=\"40\" p-align=\"center\" p-top=\"10\" p-bottom=\"10\" /></div>\n</div>\n<div class=\"row\">\n  <div class=\"small-12 columns text-center\"><label class=\"editable\" role=\"key\">Master Campaign #:</label></div>\n  <div class=\"small-12 columns text-center\"><span class=\"editable\" role=\"text\">"
    + alias4(((helper = (helper = helpers.DisplayName || (depth0 != null ? depth0.DisplayName : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"DisplayName","hash":{},"data":data}) : helper)))
    + "</span></div>\n</div>\n<div class=\"row\">\n  <div class=\"small-12 columns text-center\"><label class=\"editable\" role=\"key\">Created by:</label></div>\n  <div class=\"small-12 columns text-center\"><span class=\"editable\" role=\"text\">"
    + alias4(((helper = (helper = helpers.CreatorName || (depth0 != null ? depth0.CreatorName : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"CreatorName","hash":{},"data":data}) : helper)))
    + "</span></div>\n</div>\n"
    + ((stack1 = container.invokePartial(partials.footer,depth0,{"name":"footer","data":data,"helpers":helpers,"partials":partials,"decorators":container.decorators})) != null ? stack1 : "");
},"usePartial":true,"useData":true})

});