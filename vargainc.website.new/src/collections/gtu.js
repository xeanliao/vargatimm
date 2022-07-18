import Backbone from 'backbone'
import Model from 'models/gtu'
import { extend, each } from 'lodash'

export default Backbone.Collection.extend({
    model: Model,
    urlRoot: 'gtu',
    fetchByTask: function (taskId, opts) {
        var model = this,
            options = {
                url: 'task/' + taskId + '/gtu',
            }
        options = extend(opts, options)

        return this.fetch(options)
    },
    fetchGtuWithStatusByTask: function (taskId, opts) {
        var model = this,
            options = {
                url: model.urlRoot + '/task/' + taskId,
            }
        options = extend(opts, options)
        return this.fetch(options)
    },
    fetchGtuLocation: function (taskId, opts) {
        var collection = this,
            options = {
                url: collection.urlRoot + '/task/' + taskId + '/online/',
                method: 'GET',
                success: (result) => {
                    if (result) {
                        each(result, (item) => {
                            var gtu = collection.get(item.Id)
                            if (gtu) {
                                var location = null
                                if (item.Location && item.Location.lat && item.Location.lng) {
                                    location = {
                                        lat: parseFloat(item.Location.lat),
                                        lng: parseFloat(item.Location.lng),
                                    }
                                }

                                gtu.set({
                                    IsOnline: item.IsOnline,
                                    Location: location,
                                    OutOfBoundary: item.OutOfBoundary,
                                })
                            }
                        })
                    }
                },
            }
        extend(options, opts)
        return (this.sync || Backbone.sync).call(this, '', this, options)
    },
})
