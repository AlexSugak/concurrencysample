"use strict";

var Api = require('../DocumentsApi');

module.exports = {
    name: 'documentsApiPlugin',
    
    plugContext: function (options) {
        var apiHost = options.config && options.config.DOCUMENTS_API_HOST;

        return {
            plugActionContext: function (actionContext){
                actionContext.documentsApi = new Api({
                    getHost: function () { 
                        return apiHost;
                    }
                });
            },
            dehydrate: function () {
                return {
                    apiHost: apiHost
                };
            },
            rehydrate: function (state) { 
                apiHost = state.apiHost;
            }
        };
    }
};