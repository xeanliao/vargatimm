import Backbone from 'backbone'
import $ from 'jquery'
import { extend } from 'lodash'

import TaskCollection from 'collections/task'

export default Backbone.Model.extend({
    urlRoot: 'campaign',
    idAttribute: 'Id',
    defaults: {
        AreaDescription: null,
        ClientCode: null,
        ClientName: null,
        ContactName: null,
        CreatorName: null,
        CustemerName: null,
        Date: null,
        Description: null,
        Id: null,
        Name: null,
        Sequence: 0,
        UserName: null,
    },
    getDisplayName: function () {
        return this.get('ClientCode') + '-' + this.get('CreatorName') + '-' + this.get('AreaDescription')
    },
    getWithSubmap: function () {
        let model = this
        let url = `${this.urlRoot}/${this.get('Id')}/submap`
        return (this.sync || Backbone.sync).call(this, 'read', this, {
            method: 'GET',
            url: url,
            processData: true,
            success: function (result) {
                if (result && result.success) {
                    model.set(result.data)
                }
            },
        })
    },
    copy: function (opts) {
        var model = this,
            options = {
                method: 'POST',
                url: model.urlRoot + '/copy/' + model.get('Id'),
            }
        extend(options, opts)

        return (this.sync || Backbone.sync).call(this, 'create', this, options)
    },
    publishToDMap: function (user, opts) {
        var model = this,
            options = {
                url: model.urlRoot + '/' + model.get('Id') + '/publish/',
                method: 'PUT',
                processData: true,
                data: $.param({
                    '': user.get('Id'),
                }),
            }
        options = extend(opts, options)

        return (this.sync || Backbone.sync).call(this, 'update', this, options)
    },
    dismissToCampaign: function (user, opts) {
        var model = this,
            options = {
                url: model.urlRoot + '/' + model.get('Id') + '/dismiss/',
                method: 'PUT',
                processData: true,
                data: $.param({
                    '': user.get('Id'),
                }),
            }
        options = extend(opts, options)

        return (this.sync || Backbone.sync).call(this, 'update', this, options)
    },
    publishToMonitor: function (user, opts) {
        var model = this,
            options = {
                url: model.urlRoot + '/' + model.get('Id') + '/monitor/',
                method: 'PUT',
                processData: true,
                data: $.param({
                    '': user.get('Id'),
                }),
            }
        options = extend(opts, options)

        return (this.sync || Backbone.sync).call(this, 'update', this, options)
    },
    dismissToDMap: function (user, opts) {
        var model = this,
            options = {
                url: model.urlRoot + '/' + model.get('Id') + '/dismiss/',
                method: 'PUT',
                processData: true,
                data: $.param({
                    '': user.get('Id'),
                }),
            }
        options = extend(opts, options)

        return (this.sync || Backbone.sync).call(this, 'update', this, options)
    },
    loadWithAllTask: function (campaignId) {
        var self = this
        return (this.sync || Backbone.sync)
            .call(this, 'read', this, {
                url: `campaign/${campaignId}/tasks/all`,
            })
            .then((result) => {
                var tasks = new TaskCollection()
                tasks.set(result.Tasks)
                result.Tasks = tasks
                self.set(result)
            })
    },
})
