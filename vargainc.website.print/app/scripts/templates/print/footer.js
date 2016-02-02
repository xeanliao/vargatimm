define(['handlebars'], function(Handlebars) {

return Handlebars.template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    var helper, alias1=depth0 != null ? depth0 : {}, alias2=helpers.helperMissing, alias3="function", alias4=container.escapeExpression;

  return "<div class=\"footer\">\n  <div class=\"left\">\n    <img src=\"images/vargainc-logo.png\" alt=\"\">\n  </div>\n  <div class=\"center\">\n    <ul class=\"no-bullet\">\n      <li>\n        <span>MC#:"
    + alias4(((helper = (helper = helpers.DisplayName || (depth0 != null ? depth0.DisplayName : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"DisplayName","hash":{},"data":data}) : helper)))
    + "</span>\n        <span>www.vargainc.com</span>\n      </li>\n      <li>\n        <span>Created on:"
    + alias4(((helper = (helper = helpers.Date || (depth0 != null ? depth0.Date : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"Date","hash":{},"data":data}) : helper)))
    + "</span>\n        <span>PH:949-768-1500</span>\n      </li>\n      <li>\n        <span>Created for:"
    + alias4(((helper = (helper = helpers.ContactName || (depth0 != null ? depth0.ContactName : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"ContactName","hash":{},"data":data}) : helper)))
    + "</span>\n        <span>FX:949-768-1501</span>\n      </li>\n      <li>\n        <span>Created by:"
    + alias4(((helper = (helper = helpers.CreatorName || (depth0 != null ? depth0.CreatorName : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"CreatorName","hash":{},"data":data}) : helper)))
    + "</span>\n        <span>&copyright;2010 Varga Media Solutions,Inc.All rights reserved.</span>\n      </li>\n    </ul>\n  </div>\n  <div class=\"right\">\n    <img class=\"timm-log\" src=\"images/timm-logo-print.jpg\" alt=\"\">\n  </div>\n</div>";
},"useData":true})

});