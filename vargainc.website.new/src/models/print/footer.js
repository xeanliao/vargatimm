import Backbone from 'backbone';
import BaseModel from 'models/print/base';

export default BaseModel.extend({
    defaults: {
        'DisplayName': null,
        'Date': null,
        'ContactName': null,
        'CreatorName': null
    }
});