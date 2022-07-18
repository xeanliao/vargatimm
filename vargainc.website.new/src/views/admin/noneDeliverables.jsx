import mapboxgl from 'mapbox-gl'
import MapboxDraw from '@mapbox/mapbox-gl-draw'
import MapboxGeocoder from '@mapbox/mapbox-gl-geocoder'
import React from 'react'
import axios from 'axios'
import classnames from 'classnames'

import Helper from 'views/base'
import Logger from 'logger.mjs'

import AreaEditor from './editNdArea'
import AddressEditor from './editNdAddress'

const log = new Logger('views:dnd')

export default class NoneDeliverables extends React.Component {
    constructor(props) {
        super(props)

        this.state = {
            mapInit: false,
            mapReady: false,
            mapStyle: 'streets',
            ndSource: [],
        }

        this.doUnsubscribe = Helper.doUnsubscribe.bind(this)
        this.subscribe = Helper.subscribe.bind(this)
        this.onInitMap = this.onInitMap.bind(this)
        this.onMapLoad = this.onMapLoad.bind(this)
        this.loadGeojson = this.loadGeojson.bind(this)
        this.initNdLayers = this.initNdLayers.bind(this)
        this.onSwitchMapStyle = this.onSwitchMapStyle.bind(this)
        this.onInitLeftMenuScrollbar = this.onInitLeftMenuScrollbar.bind(this)
        this.onCreateNewCustomArea = this.onCreateNewCustomArea.bind(this)
        this.onCreateNewAddress = this.onCreateNewAddress.bind(this)

        this.onActiveNd = this.onActiveNd.bind(this)
    }

    componentDidMount() {
        this.subscribe('ndRefresh', this.loadGeojson)
    }

    componentWillUnmount() {
        this.doUnsubscribe()
    }

    onInitMap(mapContainer) {
        if (mapContainer) {
            this.setState((state) => {
                if (state.mapInit == false) {
                    mapboxgl.accessToken = MapboxToken
                    let zoom = 8
                    let center = [-73.987378, 40.744556]
                    this.map = new mapboxgl.Map({
                        container: mapContainer,
                        zoom: zoom,
                        maxZoom: 20,
                        center: center,
                        style: 'mapbox://styles/mapbox/outdoors-v11',
                    })

                    this.draw = new MapboxDraw({
                        displayControlsDefault: false,
                        controls: {
                            polygon: true,
                            trash: true,
                        },
                        defaultMode: 'simple_select',
                    })

                    this.geocoder = new MapboxGeocoder({
                        accessToken: mapboxgl.accessToken,
                        mapboxgl: mapboxgl,
                        placeholder: 'street, zip code',
                        language: 'en',
                    })

                    this.map.addControl(this.geocoder)
                    this.map.addControl(this.draw)

                    this.draw.deleteAll()

                    this.map.once('load', this.onMapLoad)
                    log.info(`mapbox init`)
                }
                return { mapInit: true }
            })
        }
    }

    onMapLoad() {
        this.map.addSource('google-road-tiles', {
            type: 'raster',
            tiles: [
                '//mt0.google.com/vt/lyrs=m&x={x}&y={y}&z={z}',
                '//mt1.google.com/vt/lyrs=m&x={x}&y={y}&z={z}',
                '//mt2.google.com/vt/lyrs=m&x={x}&y={y}&z={z}',
                '//mt3.google.com/vt/lyrs=m&x={x}&y={y}&z={z}',
            ],
            tileSize: 256,
        })
        this.map.addSource('google-satellite-tiles', {
            type: 'raster',
            tiles: [
                '//mt0.google.com/vt/lyrs=y&x={x}&y={y}&z={z}',
                '//mt1.google.com/vt/lyrs=y&x={x}&y={y}&z={z}',
                '//mt2.google.com/vt/lyrs=y&x={x}&y={y}&z={z}',
                '//mt3.google.com/vt/lyrs=y&x={x}&y={y}&z={z}',
            ],
            tileSize: 256,
        })
        this.map.addLayer(
            {
                id: 'timm-google-road-tiles-layer',
                type: 'raster',
                source: 'google-road-tiles',
                minzoom: 0,
                maxzoom: 21,
                layout: {
                    visibility: 'none',
                },
            },
            'land'
        )
        this.map.addLayer(
            {
                id: 'timm-google-satellite-tiles-layer',
                type: 'raster',
                source: 'google-satellite-tiles',
                minzoom: 0,
                maxzoom: 21,
                layout: {
                    visibility: 'none',
                },
            },
            'land'
        )

        const layers = this.map.getStyle().layers
        // Find the index of the first symbol layer in the map style
        let labelLayer = null
        for (const layer of layers) {
            log.info(layer.id)
            if (labelLayer == null && layer.id.indexOf('label') > -1) {
                labelLayer = layer.id
            }
        }

        this.map.loadImage('images/remove_pattern.png', (err, image) => {
            if (image) {
                this.map.addImage('remove_pattern', image)
                this.setState(
                    {
                        mapLabelLayer: labelLayer,
                    },
                    () => {
                        Promise.all([this.loadGeojson()]).then(() => {
                            this.setState({
                                mapReady: true,
                            })
                            return Promise.resolve()
                        })
                    }
                )
            }
        })
    }

    loadGeojson() {
        return axios.get(`nd/geojson`).then((resp) => {
            return new Promise((resolve) => {
                this.setState(
                    {
                        ndSource: resp.data,
                    },
                    () => {
                        if (this.state.mapReady == true) {
                            this.map.getSource('map-source').setData(resp.data)
                        } else {
                            this.initNdLayers()
                        }

                        return resolve()
                    }
                )
            })
        })
    }

    initNdLayers() {
        let labelLayer = this.state.mapLabelLayer

        this.map.addSource('map-source', { type: 'geojson', data: this.state.ndSource })
        // polygon fill
        this.map.addLayer(
            {
                id: 'timm-nd-layer-fill',
                type: 'fill',
                source: 'map-source',
                layout: {},
                paint: {
                    'fill-color': '#ff0000',
                    'fill-opacity': 0.5,
                },
            },
            labelLayer
        )
        // boundary
        this.map.addLayer(
            {
                id: 'timm-nd-layer-line',
                type: 'line',
                source: 'map-source',
                layout: {},
                paint: {
                    'line-color': '#ededed',
                    'line-width': 3,
                },
            },
            labelLayer
        )
        // highlight
        this.map.addLayer(
            {
                id: 'timm-nd-layer-highlight',
                type: 'line',
                source: 'map-source',
                layout: {},
                paint: {
                    'line-color': '#000000',
                    'line-width': 6,
                },
                filter: ['==', ['get', 'oid'], ''],
            },
            labelLayer
        )
        // label
        const mapStyles = ['streets', 'satellite']
        mapStyles.forEach((style) => {
            this.map.addLayer({
                id: `timm-nd-layer-label-${style}`,
                type: 'symbol',
                source: 'map-source',
                layout: {
                    'text-max-width': 200,
                    'text-field': ['format', ['get', 'name'], { 'font-scale': 1.25 }, '\n', ['get', 'desc'], { 'font-scale': 1 }],
                },
                paint:
                    style == 'streets'
                        ? {
                              'text-color': 'black',
                              'text-halo-color': ['interpolate', ['linear'], ['zoom'], 2, 'rgba(255, 255, 255, 0.75)', 3, 'rgb(255, 255, 255)'],
                              'text-halo-width': 1,
                          }
                        : {
                              'text-color': 'white',
                              'text-halo-color': ['interpolate', ['linear'], ['zoom'], 2, 'rgba(0, 0, 0, 0.75)', 3, 'rgb(0, 0, 0)'],
                              'text-halo-width': 1,
                          },
                filter: ['==', ['geometry-type'], 'Point'],
            })
        })

        return Promise.resolve()
    }

    onCreateNewCustomArea(evt) {
        evt.preventDefault()
        const data = this.draw.getAll()
        if (data.features.length == 0) {
            Helper.alert('you have not draw any polygon. plz check map top right button to draw')
        } else {
            Helper.publish('showDialog', {
                view: <AreaEditor geom={data} />,
            })
            this.draw.deleteAll()
        }
    }

    onCreateNewAddress(evt) {
        evt.preventDefault()
        const center = this.geocoder?.mapMarker?._lngLat
        const address = this.geocoder?.inputString ?? ''
        const addressArray = address.split(',')
        const street = addressArray?.[0]
        const zipCode = addressArray?.[1]

        if (center && street && zipCode) {
            Helper.publish('showDialog', {
                view: <AddressEditor geom={center} street={street} zipCode={zipCode} />,
            })
        } else {
            Helper.alert('plz search street, zip then add again')
        }
    }

    onSwitchMapStyle(style) {
        if (!this.state.mapReady) {
            return
        }
        this.setState(
            {
                mapStyle: style,
            },
            () => {
                if (style == 'streets') {
                    for (const layer of this.map.getStyle().layers) {
                        if (!layer.id.startsWith('timm')) {
                            this.map.setLayoutProperty(layer.id, 'visibility', 'visible')
                        }
                    }

                    this.map.setLayoutProperty('timm-google-satellite-tiles-layer', 'visibility', 'none')
                } else {
                    for (const layer of this.map.getStyle().layers) {
                        if (!layer.id.startsWith('timm')) {
                            this.map.setLayoutProperty(layer.id, 'visibility', 'none')
                        }
                    }

                    this.map.setLayoutProperty('timm-google-satellite-tiles-layer', 'visibility', 'visible')
                }

                // hide/show area by map style
                let hideStyle = this.state.mapStyle == 'streets' ? 'satellite' : 'streets'
                let showStyle = this.state.mapStyle == 'streets' ? 'streets' : 'satellite'

                for (const layer of this.map.getStyle().layers) {
                    if (layer.id.endsWith(hideStyle)) {
                        this.map.setLayoutProperty(layer.id, 'visibility', 'none')
                    } else if (layer.id.endsWith(showStyle)) {
                        this.map.setLayoutProperty(layer.id, 'visibility', 'visible')
                    }
                }
            }
        )
    }

    onInitLeftMenuScrollbar(el) {
        if (el) {
            let containerHeight = $(el).parent().height()
            let menuHeight = $(el).prev().height()
            $(el).css({ height: `${containerHeight - menuHeight}px` })
        }
    }

    onActiveNd(id) {
        this.setState({ activeId: id }, () => {
            let targetLayer = this.state.ndSource.features.filter((i) => i.properties.oid == id && i.geometry.type != 'Point')?.[0]
            if (targetLayer) {
                this.map.fitBounds([
                    [targetLayer.bbox[0], targetLayer.bbox[1]],
                    [targetLayer.bbox[2], targetLayer.bbox[3]],
                ])

                this.map.setFilter('timm-nd-layer-highlight', ['==', ['get', 'oid'], id])
            }
        })
    }

    onSwitchMenu(menu) {
        this.setState({ activeMenu: menu })
    }

    render() {
        return (
            <div className="none-deliverables full-container">
                <div className="grid-x grid-margin-x medium-margin-collapse" style={{ height: '100%' }}>
                    <div className="small-8 medium-9 large-10 cell">{this.renderMapbox()}</div>
                    <div className="small-4 medium-3 large-2 cell">{this.renderRightMenu()}</div>
                </div>
            </div>
        )
    }

    renderRightMenu() {
        let activeMenu = this.state.activeMenu ?? 'area'
        let data = (this.state.ndSource?.features ?? [])
            .filter((i) => i?.properties?.type == activeMenu && i?.geometry?.type == 'Point')
            .map((i) => {
                return {
                    id: i?.properties?.oid,
                    name: i?.properties?.name,
                    desc: i?.properties?.desc,
                }
            })
        return (
            <div className="grid-y grid-frame">
                <div className="cell shrink">
                    <div className="row small-collapse">
                        <div className="columns small-12">
                            <ul className="menu icons icon-left align-spaced">
                                <li>
                                    <a className="button clear" onClick={this.onCreateNewCustomArea} title="New">
                                        <i className="fa fa-file-text-o fi-list"></i>
                                        <span>Custom Area</span>
                                    </a>
                                </li>
                                <li>
                                    <a className="button clear" href="javascript:void" onClick={this.onCreateNewAddress} title="New">
                                        <i className="fa fa-file-text-o fi-list"></i>
                                        <span>Zip Code Area</span>
                                    </a>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
                <div className="cell shrink">
                    <div className="row small-collapse">
                        <div className="columns small-6">
                            <button
                                className={classnames('button small expanded margin-0', { hollow: activeMenu != 'area' })}
                                onClick={() => {
                                    this.onSwitchMenu('area')
                                }}
                            >
                                Area
                            </button>
                        </div>
                        <div className="columns small-6">
                            <button
                                className={classnames('button small expanded margin-0', { hollow: activeMenu != 'address' })}
                                onClick={() => {
                                    this.onSwitchMenu('address')
                                }}
                            >
                                Address
                            </button>
                        </div>
                    </div>
                </div>
                <div className="cell auto">
                    <div ref={this.onInitLeftMenuScrollbar} style={{ overflow: 'hidden scroll', height: '640px' }}>
                        <table className="stack">
                            <tbody>
                                {data.map((item) => {
                                    let activeClass = classnames({ 'is-active': item.id == this.state.activeId })
                                    return (
                                        <tr
                                            className={activeClass}
                                            key={item.id}
                                            onClick={() => {
                                                this.onActiveNd(item.id)
                                            }}
                                        >
                                            <td>
                                                {item.name} ({item.desc})
                                            </td>
                                        </tr>
                                    )
                                })}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        )
    }

    renderMapbox() {
        return (
            <div className="mapbox-wrapper" style={{ width: '100%', height: '100%' }}>
                <div className="mapbox-container" ref={this.onInitMap} style={{ width: '100%', height: '100%' }}></div>
                <div className="map-overlay">
                    <div className="mapboxgl-ctrl-top-left">
                        <div className="mapboxgl-ctrl mapboxgl-ctrl-group">
                            <button
                                className="mapboxgl-ctrl-icon mapboxgl-ctrl-img mapboxgl-ctrl-street"
                                onClick={() => {
                                    this.onSwitchMapStyle('streets')
                                }}
                            ></button>
                            <button
                                className="mapboxgl-ctrl-icon mapboxgl-ctrl-img mapboxgl-ctrl-satellite"
                                onClick={() => {
                                    this.onSwitchMapStyle('satellite')
                                }}
                            ></button>
                        </div>
                    </div>
                </div>
                <div className="map-top-center"></div>
            </div>
        )
    }
}
