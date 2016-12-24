import Backbone from 'backbone';
import BaseModel from 'models/print/base';

export default BaseModel.extend({
    idAttribute: 'key',
    defaults: {
        'Id': null,
        'ClientName': null,
        'ContactName': null,
        'DisplayName': null,
        'CreatorName': null,
        'Date': null,
        'Logo': null,
    }
});