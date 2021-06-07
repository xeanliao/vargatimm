//************************************************************************
// Click functions
//************************************************************************

function OnSaveDMsClick() {
    if (dmPanel) { SaveDMs(); }
    else{
        GPSAlert("No distribution maps to save.");
    }
}

function SaveDMs() {
    SaveExitsDMs();
}

function SaveExitsDMs() {
    UpdateDMChanged();
    var dms = GetDMsList();

    var submaplist = [];
    for (var i = 0; i < dms.length; i++) {
        var j = 0;
        var dmlist = [];
        while (j < dms[i]._dms._dmrecords.length) {
            var dm = {
                Id: dms[i]._dms._dmrecords[j]._id,
                Name:dms[i]._dms._dmrecords[j]._name,
                SubMapId:dms[i]._dms._dmrecords[j]._submapid
            };
            dmlist.push(dm);
            j++;
        }
        var submap = {
            CampaignId: dms[i]._campaignid,
            Id: dms[i]._id,
            Name: dms[i]._name,
            ColorString:dms[i]._colorString,
            OrderId:dms[i]._orderid,
            Penetration: dms[i]._penetration,
            Percentage: dms[i]._percentage,
            Total: dms[i]._total,
            DistributionMaps: dmlist
        };
        submaplist.push(submap);
    }
    var dmWriter = new TIMM.Website.DistributionMapServices.DMWriterService();
    dmWriter.SaveDMs(submaplist);
   
}