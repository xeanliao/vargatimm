import Backbone from 'backbone';
import BaseModel from 'models/print/base';

export default BaseModel.extend({
    urlRoot: 'map',
    defaults: {
        suppressCover: true,

        suppressCampaign: true,
        suppressCampaignSummary: true,
        suppressNDAInCampaign: true,

        suppressSubMap: true,
        suppressSubMapCountDetail: true,
        suppressNDAInSubMap: true,

        suppressDMap: false,
        suppressGTU: true,
        suppressNDAInDMap: true,

        customSubMapPenetrationColors: false,
        suppressLocations: true,
        suppressRadii: true
    }
});