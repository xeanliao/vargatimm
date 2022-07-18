import Backbone from 'backbone'
import Model from 'models/user'
import { extend } from 'lodash'

export default Backbone.Collection.extend({
    model: Model,
    url: 'user',
    fetchInGroup: function (type, opts) {
        var collection = this,
            url = collection.url + '/group/' + type,
            options = {
                url: url,
                type: 'GET',
            }
        extend(options, opts)
        return this.fetch(options)
    },
    fetchForGtu: function (opts) {
        var collection = this,
            options = {
                url: collection.url + '/gtu/',
                type: 'GET',
            }
        extend(options, opts)

        return this.fetch(options)
    },
    fetchCompany: function (opts) {
        var collection = this,
            options = {
                url: collection.url + '/company/',
                type: 'GET',
            }
        extend(options, opts)

        return this.fetch(options)
    },
})
