import Backbone from 'backbone'
import $ from 'jquery'
import Promise from 'bluebird'
import { extend } from 'lodash'

export default Backbone.Model.extend({
    urlRoot: 'task',
    idAttribute: 'Id',
    defaults: {
        Id: null,
        Name: null,
        Date: null,
        Status: null,
        Telephone: null,
        Email: null,
        DistributionMapId: null,
        AuditorId: null,
        AuditorName: null,
    },
    markFinished: function (opts) {
        var model = this,
            options = {
                url: model.urlRoot + '/' + model.get('Id') + '/finish/',
                method: 'PUT',
            }
        options = extend(opts, options)

        return (this.sync || Backbone.sync).call(this, 'update', this, options)
    },
    reOpen: function (opts) {
        var model = this,
            options = {
                url: model.urlRoot + '/' + model.get('Id') + '/reopen/',
                method: 'PUT',
            }
        options = extend(opts, options)

        return (this.sync || Backbone.sync).call(this, 'update', this, options)
    },
    importGtu: function (file, opts) {
        var model = this,
            options = {
                url: '../api/' + model.urlRoot + '/' + model.get('Id') + '/import/',
                method: 'PUT',
            }
        options = extend(opts, options)
        var xhr = new XMLHttpRequest()
        var fd = new FormData()

        xhr.open(options.method, options.url, true)
        xhr.onreadystatechange = () => {
            if (xhr.readyState == 4 && xhr.status == 200) {
                options.success && options.success.call(model, JSON.parse(xhr.responseText))
            }
        }
        fd.append('gtuFile', file)
        xhr.send(fd)
    },
    addGtuDots: function (dots, opts) {
        var model = this
        var options = {
            url: model.urlRoot + '/' + model.get('Id') + '/dots/',
            method: 'POST',
            data: $.param({
                '': dots,
            }),
        }
        options = extend(opts, options)
        return (this.sync || Backbone.sync).call(this, '', this, options).then((result) => {
            if (result && result.success) {
                return Promise.resolve()
            }
            return Promise.reject()
        })
    },
    removeGtuDots: function (dots, opts) {
        var model = this,
            options = {
                url: model.urlRoot + '/' + model.get('Id') + '/dots/',
                method: 'PUT',
                data: $.param({
                    '': dots,
                }),
            }
        options = extend(opts, options)

        return (this.sync || Backbone.sync).call(this, '', this, options).then((result) => {
            if (result && result.success) {
                return Promise.resolve()
            }
            return Promise.reject()
        })
    },
    setStart: function (opts) {
        var model = this,
            options = {
                url: model.urlRoot + '/' + model.get('Id') + '/start/',
                method: 'PUT',
                success: function (result) {
                    if (result && result.success) {
                        model.set({
                            Status: result.status,
                        })
                    }
                },
            }
        options = extend(opts, options)

        return (this.sync || Backbone.sync).call(this, 'update', this, options)
    },
    setPause: function (opts) {
        var model = this,
            options = {
                url: model.urlRoot + '/' + model.get('Id') + '/pause/',
                method: 'PUT',
                success: (result) => {
                    if (result && result.success) {
                        model.set({
                            Status: result.status,
                        })
                    }
                },
            }
        options = extend(opts, options)

        return (this.sync || Backbone.sync).call(this, 'update', this, options)
    },
    setStop: function (opts) {
        var model = this,
            options = {
                url: model.urlRoot + '/' + model.get('Id') + '/stop/',
                method: 'PUT',
                success: function (result) {
                    if (result && result.success) {
                        model.set({
                            Status: result.status,
                        })
                    }
                },
            }
        options = extend(opts, options)

        return (this.sync || Backbone.sync).call(this, 'update', this, options)
    },
    mergeFrom: function (taskId) {
        var model = this,
            options = {
                url: `${model.urlRoot}/merge/${taskId}/to/${model.get('Id')}`,
                method: 'GET',
            }

        return (this.sync || Backbone.sync).call(this, '', this, options).then((result) => {
            if (result && result.success) {
                return Promise.resolve()
            }
            return Promise.reject()
        })
    },
})
