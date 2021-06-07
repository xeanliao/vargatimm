/// <reference path="jquery-1.3.2.js" />
/// <reference path="jquery-1.3.2-vsdoc2.js" />

GPS.Form = function(method, action, params, target) {
    this.from = null;
    this.__init__(method, action, params, target);
}

GPS.Form.prototype = {
    __init__: function(method, action, params, target) {
        var aform = document.createElement('form');
        aform.method = method;
        aform.action = action;
//        target = "_about";
        if (target) { aform.target = target; }
        if (params) {
            for (var i = 0; i < params.length; i++) {
                var input = document.createElement('input');
                input.type = 'hidden';
                input.name = params[i].name;
                input.value = params[i].value;
                aform.appendChild(input);
            }
        }
        this.from = aform;
        document.body.appendChild(aform);
    },
    submit_from: function() {
        this.from.submit();
    }
}

