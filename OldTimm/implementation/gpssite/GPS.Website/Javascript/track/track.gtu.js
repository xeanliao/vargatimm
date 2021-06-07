var importDataDialog;

/**
* Called when the user clicks on the 'View GTUS' menu item.
*/
function ShowGtusList() {
    window.open('Gtus.aspx', '_blank', 'resizable=yes,status=yes,toolbar=no,menubar=no,location=no;');
}

/**
* Called when the user clicks on the 'Load GTUS' menu item.
*/
function ShowImportGtus() {
    if (!importDataDialog) {
        importDataDialog = $("#div_import_gtus").dialog({
            width: 400, modal: true, overlay: { opacity: 0.5 }
        });
    }
    else { $(importDataDialog).dialog("open"); }
}




function OnMenuClick(action) {

    if (action == "gtu") {

        GPSUploadFile("gtufile", function(ret) {
            if (ret.IsSuccess) {
                var filename = ret.Name;
                //var bValid = true;
                //bValid = bValid && (filename != '');
                //if (!bValid) {
                //      GTUAlert("<p style=\"color:red;\">Please select a import file.</p>"("<br />"));
                //      return;
                //}
                var gtuReader = new TIMM.Website.TrackServices.GtuReaderService();
                gtuReader.LoadGtuFromExcel(filename, function(aRet) {
                    for (var i = 0; i < aRet.length; i++) {
                        var r = aRet[i];
                        testTable.fnAddData([
                                            r.UniqueID,
                                            r.Model,
                                            r.UserName,
                                            r.IsEnabled,
                                            r.UniqueID
                                       ]);
                    }
                    GPS.Loading.hide();
                    GPSAlert(aRet.length + " GTU records have been loaded.");
                });

            }
            return true;
        });
    }
}

                                         
                                        