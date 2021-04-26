import Backbone from 'backbone'
import React from 'react'
import 'react.backbone'
import BaseView from 'views/base'
import L from 'leaflet'
import $ from 'jquery'
import Promise from 'bluebird'
import { map, each } from 'lodash'
var monitorMap = null
export default React.createBackboneClass({
    mixins: [BaseView],
    onInit: function (mapContainer) {
        monitorMap = L.map(mapContainer, {
            preferCanvas: true,
        })
        //google road map
        L.tileLayer('//{s}.google.com/vt/lyrs=m&x={x}&y={y}&z={z}', {
            maxZoom: 20,
            subdomains: ['mt0', 'mt1', 'mt2', 'mt3'],
        }).addTo(monitorMap)
        monitorMap.setView(
            {
                lat: 30.567027824766257,
                lng: 103.94928932189943,
            },
            13
        )
        monitorMap.on('click', (opt) => {
            console.log(opt.latlng)
        })
        // custom map
        // monitorMap = L.map(mapContainer, {
        // 	crs: L.CRS.Simple,
        // 	preferCanvas: true
        // });
        // var image = L.imageOverlay('./images/ctu.airport.svg', bounds).addTo(monitorMap);
        // monitorMap.fitBounds(bounds);
        // L.tileLayer('//{s}.google.com/vt/lyrs=s,h&x={x}&y={y}&z={z}', {
        // 	maxZoom: 20,
        // 	subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
        // }).addTo(monitorMap);
        // var imageUrl = './images/ctu.airport.jpg',
        // 	imageBounds = [
        // 		{lat: 30.59344462456968, lng: 103.93873214721681},
        // 		{lat: 30.510956048969373, lng: 103.95538330078125}
        // 	];
        // L.imageOverlay(imageUrl, imageBounds).addTo(monitorMap);
    },
    shouldComponentUpdate: function () {
        return false
    },
    componentDidMount: function () {
        //begin get submap boundary
        this.loadGtu()
    },
    loadGtu: function () {
        var address = '../api'
        var maps = []
        var campaignId = this.props.campaignId
        var submapId = this.props.submapId
        $.getJSON(address + '/print/campaign/' + campaignId)
            .then((campaign) => {
                each(campaign.SubMaps, (s) => {
                    if (s.Id != submapId) {
                        return false
                    }
                    each(s.DMaps, (d) => {
                        maps.push({
                            s: s.Id,
                            d: d.Id,
                        })
                    })
                })
                return Promise.resolve()
            })
            .then(() => {
                return $.getJSON(address + '/print/campaign/' + campaignId + '/submap/' + submapId + '/boundary').then((result) => {
                    let latlngs = map(result.boundary, (i) => {
                        return [i.lat, i.lng]
                    })
                    let polygon = L.polygon(latlngs, {
                        color: 'rgb(' + result.color.r + ',' + result.color.g + ',' + result.color.b + ')',
                    }).addTo(monitorMap)
                    monitorMap.fitBounds(polygon.getBounds())
                    return Promise.resolve()
                })
            })
            .then(() => {
                return Promise.each(maps, (i) => {
                    return $.getJSON(address + '/print/campaign/' + campaignId + '/submap/' + i.s + '/dmap/' + i.d + '/boundary').then((result) => {
                        let latlngs = map(result.boundary, (i) => {
                            return [i.lat, i.lng]
                        })
                        let polygon = L.polygon(latlngs, {
                            color: 'rgb(' + result.color.r + ',' + result.color.g + ',' + result.color.b + ')',
                        }).addTo(monitorMap)
                    })
                })
            })
            .then(() => {
                return Promise.each(maps, (i) => {
                    return $.getJSON(address + '/print/campaign/' + campaignId + '/submap/' + i.s + '/dmap/' + i.d + '/gtu').then((result) => {
                        let colors = result.pointsColors
                        each(result.points, (point, index) => {
                            let color = colors[index]
                            each(point, (p) => {
                                L.circleMarker(
                                    {
                                        lat: p.lat,
                                        lng: p.lng,
                                    },
                                    {
                                        interactive: false,
                                        radius: 3,
                                        fill: true,
                                        fillColor: color,
                                    }
                                ).addTo(monitorMap)
                            })
                        })
                    })
                })
            })
    },
    render: function () {
        return (
            <div className="row">
                <div className="small-12">
                    <div ref={this.onInit} style={{ minHeight: '640px' }}></div>
                </div>
            </div>
        )
    },
})
