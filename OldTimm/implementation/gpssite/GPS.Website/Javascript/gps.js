/**
* This file contains the most common functions used across the system.
*/

/*
* Add commas to a number.
*/
/*
*get current login user, get its privilege by user type
 1, 'CreateCampaign'
 2, 'SubmapView'
 3, 'HistoricalData'
 4, 'PublishCampaign'
 5, 'RealTimeWalkerLocation'
 6, 'AssignNameToGTU'
 7, 'StartSuspendStopGTU'
 8, 'NotifiedByEmail'
 9, 'AssignGTUToDM'
 10, 'AssignAuditor'
 11, 'GeneratePdf'
 12, 'CreateDriverClientAuditorAccount'
 13, 'DNDManagement'
 14, 'SubmapDMView'
 15, 'AssignManager'
 16, 'GTUManagement'
 17, 'CreateDriverClientAuditorManagerAccount'
 18, 'CreateAllTypeUserAccount'
 19, 'DMView'
 20, 'ReportOfDM'
 21, 'UndoPublishCampaign'*/
 CreateCampaign = false;
 SubmapView = false;
 HistoricalData = false;
 PublishCampaign = false;
 RealTimeWalkerLocation = false;
 AssignNameToGTU = false;
 StartSuspendStopGTU = false;
 NotifiedByEmail = false;
 AssignGTUToDM = false;
 AssignAuditor = false;
 GeneratePdf = false;
 CreateDriverClientAuditorAccount = false;
 DNDManagement = false;
 SubmapDMView = false;
 AssignManager = false;
 GTUManagement = false;
 CreateDriverClientAuditorManagerAccount = false;
 CreateAllTypeUserAccount = false;
 DMView = false;
 ReportOfDM = false;
 UndoPublishCampaign = false;
 
 
Number.addCommas = function(nStr) {
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

Array.selectByFilter = function(array, fnFilter) {
    var result = new Array();
    for (var i in array) {
        if (fnFilter(array[i])) {
            result.push(array[i]);
        }
    }
    return result;
}

Array.selectSingle = function(array, fnFilter) {
    var matched = Array.selectByFilter(array, fnFilter);
    return matched[0] || null;
}

Array.selectIndexes = function(array, fnFilter) {
    var result = new Array();
    for (var i in array) {
        if (fnFilter(array[i])) {
            result.push(i);
        }
    }
    return result;
}

Array.clone = function(array) {
    var clone = new Array();
    for (var i in array) {
        clone[i] = array[i];
    }
    return clone;
}

Date.clientFormatToServerFormat = function(date) {
    return $.datepicker.formatDate("yy-mm-dd", date);
}

Date.serverFormatToClientFormat = function(dateString) {
    return $.datepicker.parseDate("yy-mm-dd", dateString);
}

String.isNullOrEmpty = function(str) {
    return !str || str.length == 0;
}

/*
* Root namespace for GPS classes.
*/
GPS = function() { }

function GPSAlert(message, callback) {
    this._fnCallBack = callback;
    var thisObj = this;
    if ($("#dialogalert").length == 0) {
        $("body").append('<div id="dialogalert"></div>');
        $("#dialogalert").dialog({
            autoOpen: false,
            title: 'TIMM',
            modal: true,
            resizable: false,
            overlay: {
                opacity: 0.5,
                background: "black"
            },
            buttons: {
                "Ok": function() {
                    $(this).dialog("close");
                    if (thisObj._fnCallBack) {
                        thisObj._fnCallBack();
                    }
                }
            }
        });
    }
    $("#dialogalert").html(message);
    $("#dialogalert").dialog("open");
}

function GPSConfirm(message, callback) {
    this._fnCallBack = callback;
    var thisObj = this;
    if ($("#dialogconfirm").length == 0) {
        $("body").append('<div id="dialogconfirm"></div>');
        $("#dialogconfirm").dialog({
            autoOpen: false,
            title: 'Confirm',
            modal: true,
            resizable: false,
            overlay: {
                opacity: 0.5,
                background: "black"
            },
            buttons: {
                "Cancel": function() {
                    $(this).dialog("close");
                    if (thisObj._fnCallBack) {
                        thisObj._fnCallBack('cancel');
                    }
                },
                "No": function() {
                    $(this).dialog("close");
                    if (thisObj._fnCallBack) {
                        thisObj._fnCallBack('no');
                    }
                },
                "Yes": function() {
                    $(this).dialog("close");
                    if (thisObj._fnCallBack) {
                        thisObj._fnCallBack('yes');
                    }
                }
            }
        });
    }
    $("#dialogconfirm").html(message);
    $("#dialogconfirm").dialog("open");

}

function GPSUploadFile(type, callback) {
    this._fnCallBack = callback;
    this._type = type;
    var thisObj = this;
    if ($("#dialoguploadfile").length == 0) {
        $("body").append('<div id="dialoguploadfile"><p id="fileValidateTips"></p><input id="FileUploader" name="FileUploader" size="35" style="width: 350px;" type="file" /></div>');
        $("#FileUploader").bind('change', function() {
            var thisObj = this;
            var fn = $("#FileUploader").val();
            var ext = fn.slice(fn.indexOf('.')).toLowerCase();
            if (type == "campaignimage") {
                ValidateUploadExtend = (ext == '.bmp' || ext == '.gif' || ext == '.jpg' || ext == '.jpeg' || ext == '.png');
                if (!ValidateUploadExtend) {
                    $("#FileUploader:button:contains('Upload')").attr("disabled", "disabled");
                    $('#fileValidateTips').text('Only bmp, gif, jpg and png file could be uploaded!').effect("highlight", {}, 1500);
                }
                else {
                    $('#fileValidateTips').text('');
                    $("#FileUploader:button:contains('Upload')").removeAttr("disabled");
                }
            }
            if (type == "importdatafile" || type == "addressfile") {
                ValidateUploadExtend = (ext == '.txt' || ext == '.excel' || ext == '.csv');
                if (!ValidateUploadExtend) {
                    $("#FileUploader:button:contains('Upload')").attr("disabled", "disabled");
                    $('#fileValidateTips').text('Only txt, csv and excel file could be imported!').effect("highlight", {}, 1500);
                }
                else {
                    $('#fileValidateTips').text('');
                    $("#FileUploader:button:contains('Upload')").removeAttr("disabled");
                }
            }
        });


    }
    $("#dialoguploadfile").dialog({
        autoOpen: false,
        title: 'UpLoad File',
        modal: true,
        width: 450,
        height: 180,
        resizable: false,
        overlay: {
            opacity: 0.5
            //                ,
            //                background: "black"
        },
        buttons: {
            "Cancel": function() {
                $(this).dialog("close");
                $(this).dialog("destroy");
            },
            "Upload": function() {
                if (String.isNullOrEmpty($("#FileUploader").val())) {
                    $('#fileValidateTips').text('You must select a file to upload.');
                }
                else {

                    GPS.Loading.show();
                    $.ajaxFileUpload({
                        url: 'Handler/uploadfile.ashx?type=' + thisObj._type,
                        secureuri: false,
                        fileElementId: 'dialoguploadfile',
                        dataType: 'json',
                        success: function(data) {
                            $("#FileUploader").remove();
                            $("#FileUploader").remove();
                            $("#fileValidateTips").remove();
                            $("#fileValidateTips").remove();
                            if (thisObj._fnCallBack) {
                                if (!thisObj._fnCallBack(data)) {
                                    GPS.Loading.hide();
                                }
                            }
                            else {
                                GPS.Loading.hide();
                            }
                        },
                        error: function(data, status, e) {
                            GPS.Loading.hide();
                            GPSAlert('Upload file error.');
                        }
                    });
                    $(this).dialog("close");
                }
            }
        }
    });
    
    $("#dialoguploadfile").dialog("open");
}




/*
* Construct a campaign name presented to the end user.
*
* @val - the campaign object
*/
function ConstructCampaignName(val) {
    var constructDate = function(d) {
        var month = d.getMonth() + 1;
        if (month < 10) month = ['0', month].join('');

        var date = d.getDate();
        if (date < 10) date = ['0', date].join('');

        return [month, date, d.getFullYear().toString().substr(2)].join('');
    }
    return constructDate(new Date(val.Date)) + "-" + val.ClientCode + '-' + val.UserCode + "-" + val.AreaDescription + '-' + val.Sequence;
}

/*
*   Class GPS.QArray - quick access array items
*/
GPS.QArray = function() {
    this._objects = new Object();
};

GPS.QArray.prototype = {
    // set object in array
    Set: function(key, obj) {
        if (obj != null) {
            this._objects[key] = obj;
        }
        else if (typeof (this._objects[key]) != 'undefined') {
            delete this._objects[key];
        }
    },
    // get object from array
    Get: function(key) {
        var obj = null;
        if (typeof (this._objects[key]) != 'undefined') {
            obj = this._objects[key];
        }
        return obj;
    },
    //Contains key in array
    ContainsKey: function(key) {
        if (typeof (this._objects[key]) == 'undefined') {
            return false;
        }
        else {
            return true;
        }
    },
    // Each function
    Each: function(fn) {
        for (var i in this._objects) {
            if (fn) {
                if (fn(i, this._objects[i])) {
                    break;
                }
            }
        }
    }
};

GPS.QArray.prototype.constructor = GPS.QArray;

// Inspired by base2 and Prototype
(function() {
    var initializing = false, fnTest = /xyz/.test(function() { xyz; }) ? /\b_super\b/ : /.*/;

    // The base Class implementation (does nothing)
    this.Class = function() { };

    // Create a new Class that inherits from this class
    Class.extend = function(prop) {
        var _super = this.prototype;

        // Instantiate a base class (but only create the instance,
        // don't run the init constructor)
        initializing = true;
        var prototype = new this();
        initializing = false;

        // Copy the properties over onto the new prototype
        for (var name in prop) {
            // Check if we're overwriting an existing function
            prototype[name] = typeof prop[name] == "function" &&
        typeof _super[name] == "function" && fnTest.test(prop[name]) ?
        (function(name, fn) {
            return function() {
                var tmp = this._super;

                // Add a new ._super() method that is the same method
                // but on the super-class
                this._super = _super[name];

                // The method only need to be bound temporarily, so we
                // remove it when we're done executing
                var ret = fn.apply(this, arguments);
                this._super = tmp;

                return ret;
            };
        })(name, prop[name]) :
        prop[name];
        }

        // The dummy class constructor
        function Class() {
            // All construction is actually done in the init method
            if (!initializing && this.init)
                this.init.apply(this, arguments);
        }

        // Populate our constructed prototype object
        Class.prototype = prototype;

        // Enforce the constructor to be what we expect
        Class.constructor = Class;

        // And make this class extendable
        Class.extend = arguments.callee;

        return Class;
    };
})();


function DictionaryConvertToClient(items) {
    var dItems = new Object();
    for (var i in items) {
        if (typeof (items[i].Key) != 'undefined') {
            var key = items[i].Key;
            var value = items[i].Value;
            if (typeof (value[0]) != 'undefined') {
                dItems[key] = DictionaryConvertToClient(value);
            }
            else {
                dItems[key] = value;
            }
        }
    }
    return dItems;
}