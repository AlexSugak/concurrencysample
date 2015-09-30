"use strict";

var Api = require('../TicketsApi');

module.exports = {
    name: 'ticketsApiPlugin',
    
    plugContext: function (options) {
        var apiHost = options.config && options.config.TICKETS_API_HOST;

        return {
            plugActionContext: function (actionContext){
                actionContext.ticketsApi = new Api({
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