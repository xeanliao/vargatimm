import Backbone from 'backbone';
import $ from 'jquery';
import Promise from 'bluebird';
import {
    extend
} from 'lodash';

export default Backbone.Model.extend({
    urlRoot: 'user',
    idAttribute: 'Id',
    defaults: {
        Id: null,
        UserCode: null,
        UserName: null,
        FullName: null,
        Emai: null,
        Token: null
    },
    fetchCurrentUser: function (opts) {
        var model = this,
            url = model.urlRoot + '/info',
            options = {
                url: url,
                type: 'GET'
            };
        extend(options, opts);

        return (this.sync || Backbone.sync).call(this, 'read', this, options)
            .then((result) => {
                if (result && result.success) {
                    model.set(result.data);
                    return Promise.resolve();
                }
                return Promise.reject();
            });
    },
    addEmployee: function (file, opts) {
        var model = this,
            options = {
                url: '../api/' + model.urlRoot + '/employee/',
                method: 'POST'
            };
        options = extend(opts, options);
        return new Promise((resolve, reject) => {
            var xhr = new XMLHttpRequest();
            var fd = new FormData();

            xhr.open(options.method, options.url, true);
            xhr.onreadystatechange = () => {
                if (xhr.readyState == 4) {
                    if (xhr.status == 200) {
                        var result = JSON.parse(xhr.responseText);
                        model.set(result);
                        options.success && options.success.call(model, result);
                        return resolve();
                    }
                    return reject(new Error('xhr states not OK'));
                }
            };
            fd.append('Picture', file);
            fd.append('CompanyId', model.get('CompanyId'));
            fd.append('Role', model.get('Role'));
            fd.append('FullName', model.get('FullName'));
            fd.append('CellPhone', model.get('CellPhone'));
            fd.append('DateOfBirth', model.get('DateOfBirth'));
            fd.append('Notes', model.get('Notes'));
            xhr.send(fd);
        });
    },
    logout: function (opts) {
        var model = this,
            url = model.urlRoot + '/logout',
            options = {
                url: url,
                type: 'POST'
            };
        extend(options, opts);

        return (this.sync || Backbone.sync).call(this, null, this, options);
    }
});