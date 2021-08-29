import Backbone from 'backbone'
import Model from 'models/campaign'
import Promise from 'bluebird'
import { extend } from 'lodash'

export default Backbone.Collection.extend({
    model: Model,
    urlRoot: 'campaign',
    fetchForDistribution: function (opts) {
        var model = this,
            options = {
                url: model.urlRoot + '/distribution',
            }
        options = extend(opts, options)

        return this.fetch(options)
    },
    fetchForTask: function (opts) {
        var model = this,
            options = {
                url: model.urlRoot + '/monitor',
            }
        options = extend(opts, options)

        return this.fetch(options)
    },
    fetchForReport: function (opts) {
        var model = this,
            options = {
                url: model.urlRoot + '/report',
            }
        options = extend(opts, options)

        return this.fetch(options)
    },
})
