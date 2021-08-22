// import mapboxgl from '!mapbox-gl/dist/mapbox-gl-dev'
import mapboxgl from 'mapbox-gl'
import React from 'react'
import PropTypes from 'prop-types'
import { color } from 'd3-color'
import axios from 'axios'
import Logger from 'logger.mjs'
import ClassNames from 'classnames'

const log = new Logger('views:campaign')

const layers = [
    { layer: '3zip', zoom: [7, 18, 7], color: '#323232' },
    { layer: '5zip', zoom: [9, 18, 11], color: '#00FF00' },
    { layer: 'croute', zoom: [11, 18, 14], color: '#00FF00' },
]

export default class Campaign extends React.Component {
    static propTypes = {
        campaigin: PropTypes.object,
    }

    constructor(props) {
        super(props)
        this.state = {
            mapInited: false,
            mapReady: false,
            activeLayer: null,
            highlightLayerName: null,
            selectedLayer: [],
            selectedSubmap: null,
            campaginSource: { features: [] },
        }

        this.onInitSubMapScrollbar = this.onInitSubMapScrollbar.bind(this)
        this.onInitMap = this.onInitMap.bind(this)
        this.onMapLoad = this.onMapLoad.bind(this)
        this.initBackgroundLayer = this.initBackgroundLayer.bind(this)
        this.initCampaignLayer = this.initCampaignLayer.bind(this)
        this.initPopup = this.initPopup.bind(this)

        this.onSubmapSelect = this.onSubmapSelect.bind(this)
        this.onClassificationsChange = this.onClassificationsChange.bind(this)
        this.onLayerHightlight = this.onLayerHightlight.bind(this)
        this.onLayerSelect = this.onLayerSelect.bind(this)

        this.map = null
    }

    onInitMap(mapContainer) {
        if (mapContainer) {
            this.setState((state) => {
                if (state.mapInited == false) {
                    mapboxgl.accessToken = MapboxToken
                    let zoom = this.props.campaign.get('ZoomLevel', 8)
                    let center = [this.props.campaign.get('Longitude', -73.987378), this.props.campaign.get('Latitude', 40.744556)]
                    this.map = new mapboxgl.Map({
                        container: mapContainer,
                        zoom: zoom,
                        maxZoom: 20,
                        center: center,
                        // style: '/map/street.json',
                        style: 'mapbox://styles/mapbox/outdoors-v11',
                    })
                    this.map.on('load', this.onMapLoad)
                    log.info(`mapbox inited`)
                }
                return { mapInited: true }
            })
        }
    }

    onInitSubMapScrollbar(el) {
        if (el) {
            let containerHeight = $(el).parent().height()
            let menuHeight = $(el).prev().height()
            $(el).css({ height: `${containerHeight - menuHeight}px` })
        }
    }

    onMapLoad() {
        const layers = this.map.getStyle().layers
        // Find the index of the first symbol layer in the map style
        let firstSymbolId = 'aerialway'
        for (const layer of layers) {
            log.info(layer.id)
            if (layer.id.indexOf('label') > -1 && !firstSymbolId) {
                firstSymbolId = layer.id
            }
        }

        // this.map.on('zoom', () => {
        //     log.info(`map zoom: ${this.map.getZoom()}`)
        // })

        this.initBackgroundLayer(firstSymbolId)
        this.initCampaignLayer(firstSymbolId)
        this.initPopup()
    }

    initBackgroundLayer(firstSymbolId) {
        layers.forEach((item) => {
            let tileSource = `${location.protocol}//${location.host}/api/area/tiles/${item.layer}/{z}/{x}/{y}`
            this.map.addSource(`area-source-${item.layer}`, {
                type: 'vector',
                tiles: [tileSource],
                minzoom: item.zoom[0],
                maxzoom: item.zoom[1],
            })
            this.map.addLayer(
                {
                    id: `area-layer-${item.layer}-fill`,
                    type: 'fill',
                    source: `area-source-${item.layer}`,
                    'source-layer': item.layer,
                    minzoom: item.zoom[0],
                    maxzoom: item.zoom[1],
                    layout: {
                        visibility: 'none',
                    },
                    paint: {
                        'fill-color': 'hsl(156, 20%, 95%)',
                        'fill-opacity': 0.5,
                    },
                },
                firstSymbolId
            )
            this.map.addLayer(
                {
                    id: `area-layer-${item.layer}-selected`,
                    type: 'fill',
                    source: `area-source-${item.layer}`,
                    'source-layer': item.layer,
                    minzoom: item.zoom[0],
                    maxzoom: item.zoom[1],
                    layout: {},
                    paint: {
                        'fill-color': 'hsl(156, 20%, 75%)',
                        'fill-opacity': 0.75,
                    },
                    filter: ['==', 'name', ''],
                },
                firstSymbolId
            )
            this.map.addLayer(
                {
                    id: `area-layer-${item.layer}-line-bg`,
                    type: 'line',
                    source: `area-source-${item.layer}`,
                    'source-layer': item.layer,
                    minzoom: item.zoom[0],
                    maxzoom: item.zoom[1],
                    layout: {
                        visibility: 'none',
                    },
                    paint: {
                        'line-width': ['interpolate', ['linear'], ['zoom'], 4, 3.5, 18, 16],
                        'line-color': 'hsl(0, 0%, 84%)',
                        'line-opacity': ['interpolate', ['linear'], ['zoom'], 4, 0, 18, 0.5],
                        'line-translate': [0, 0],
                        'line-blur': ['interpolate', ['linear'], ['zoom'], 4, 0, 18, 4],
                    },
                },
                firstSymbolId
            )
            this.map.addLayer(
                {
                    id: `area-layer-${item.layer}-line`,
                    type: 'line',
                    source: `area-source-${item.layer}`,
                    'source-layer': item.layer,
                    minzoom: item.zoom[0],
                    maxzoom: item.zoom[1],
                    layout: {
                        visibility: 'none',
                        'line-join': 'round',
                        'line-cap': 'round',
                    },
                    paint: {
                        'line-color': 'hsl(0, 0%, 62%)',
                        'line-width': ['interpolate', ['linear'], ['zoom'], 4, 0.5, 18, 4],
                    },
                },
                firstSymbolId
            )
            this.map.addLayer({
                id: `area-layer-${item.layer}-label`,
                type: 'symbol',
                source: `area-source-${item.layer}`,
                'source-layer': item.layer,
                minzoom: item.zoom[0],
                maxzoom: item.zoom[1],
                layout: {
                    visibility: 'none',
                    'text-max-width': 200,
                    'text-field': [
                        'format',
                        ['get', 'name'],
                        { 'font-scale': 1.2 },
                        '\n',
                        [
                            'concat',
                            'SFDU:',
                            ['number-format', ['get', 'home'], { locale: 'en-Latn-US' }],
                            ' ',
                            'MFDU:',
                            ['number-format', ['get', 'apt'], { locale: 'en-Latn-US' }],
                        ],
                        {
                            'font-scale': 0.75,
                        },
                        '\n',
                        ['concat', 'Total:', ['number-format', ['+', ['get', 'home'], ['get', 'apt']], { locale: 'en-Latn-US' }]],
                        {
                            'font-scale': 0.75,
                        },
                    ],
                },
                paint: {
                    'text-halo-color': 'hsl(0, 0%, 100%)',
                    'text-halo-width': 0.5,
                    'text-halo-blur': 0.5,
                    'text-color': [
                        'step',
                        ['zoom'],
                        ['step', ['get', 'sizerank'], 'hsl(0, 0%, 66%)', 5, 'hsl(230, 0%, 56%)'],
                        17,
                        ['step', ['get', 'sizerank'], 'hsl(0, 0%, 66%)', 13, 'hsl(0, 0%, 56%)'],
                    ],
                },
                filter: ['==', ['geometry-type'], 'Point'],
            })

            this.map.on('mousemove', `area-layer-${item.layer}-fill`, this.onLayerHightlight)
        })
    }

    initCampaignLayer(firstSymbolId) {
        axios.get(`${location.protocol}//${location.host}/api/campaign/${this.props.campaign.get('Id')}/submap/geojson`).then((response) => {
            this.setState(
                {
                    campaginSource: response.data,
                    mapReady: true,
                },
                () => {
                    this.map.addSource('campaign-source', { type: 'geojson', data: response.data })

                    this.map.addLayer(
                        {
                            id: 'campaign-layer-fill',
                            type: 'fill',
                            source: 'campaign-source',
                            layout: {},
                            paint: {
                                'fill-color': ['get', 'color'],
                                'fill-opacity': 0.3,
                            },
                        },
                        firstSymbolId
                    )

                    this.map.addLayer(
                        {
                            id: 'campaign-layer-line',
                            type: 'line',
                            source: 'campaign-source',
                            layout: {},
                            paint: {
                                'line-color': ['get', 'color'],
                                'line-width': 3,
                            },
                        },
                        firstSymbolId
                    )

                    this.map.addLayer(
                        {
                            id: 'campaign-layer-highlighted',
                            type: 'line',
                            source: 'campaign-source',
                            layout: {},
                            paint: {
                                'line-color': '#000000',
                                'line-width': 6,
                            },
                            filter: ['==', ['get', 'sid'], ''],
                        },
                        firstSymbolId
                    )
                }
            )
        })
    }

    initPopup() {
        let popup = new mapboxgl.Popup({
            closeButton: false,
            closeOnClick: false,
        })

        this.map.on('mouseenter', 'area-layer-fill', (e) => {
            // Change the cursor style as a UI indicator.
            // this.map.getCanvas().style.cursor = 'pointer'
            let id = e.features[0].properties.id
            let lnglat = e.features[0].properties.center.split(',').map((c) => parseFloat(c))
            let name = e.features[0].properties.name

            // Ensure that if the map is zoomed out such that multiple
            // copies of the feature are visible, the popup appears
            // over the copy being pointed to.
            while (Math.abs(e.lngLat.lng - lnglat[0]) > 180) {
                lnglat[0] += e.lngLat.lng > lnglat[0] ? 360 : -360
            }

            // Populate the popup and set its coordinates
            // based on the feature found.
            popup.setLngLat(lnglat).setHTML(name).addTo(this.map)
            popup.getElement().id = id
        })

        this.map.on('mousemove', 'area-layer-fill', (e) => {
            let el = popup.getElement()
            let id = e.features[0].properties.id
            if (el.id != id) {
                let lnglat = e.features[0].properties.center.split(',').map((c) => parseFloat(c))
                let name = e.features[0].properties.name

                // Ensure that if the map is zoomed out such that multiple
                // copies of the feature are visible, the popup appears
                // over the copy being pointed to.
                while (Math.abs(e.lngLat.lng - lnglat[0]) > 180) {
                    lnglat[0] += e.lngLat.lng > lnglat[0] ? 360 : -360
                }

                popup.setLngLat(lnglat).setHTML(name)
                el.id = id
            }
        })

        this.map.on('mouseleave', 'area-layer-fill', () => {
            popup.remove()
        })
    }

    onClassificationsChange(evt) {
        if (!this.state.mapReady) {
            return
        }
        let activeLayer = evt.currentTarget.id == 'none' ? null : evt.currentTarget.id
        this.setState(
            {
                activeLayer: activeLayer,
            },
            () => {
                const type = ['fill', 'selected', 'line-bg', 'line', 'label']
                layers.forEach((item) => {
                    type.forEach((type) => {
                        this.map.setLayoutProperty(`area-layer-${item.layer}-${type}`, 'visibility', item.layer == activeLayer ? 'visible' : 'none')
                    })
                })

                if (activeLayer) {
                    let minZoom = layers.filter((i) => i.layer == activeLayer)?.[0]?.zoom?.[2]
                    if (this.map.getZoom() < minZoom) {
                        this.map.setZoom(minZoom)
                    }
                }
            }
        )
    }

    onSubmapSelect(submap) {
        if (!this.state.mapReady) {
            return
        }
        this.setState((state) => {
            if (state.selectedSubmap != submap.Id) {
                let highlightColor = color(`#${submap.ColorString}`).darker(2).toString()
                this.map.setLayoutProperty('campaign-layer-highlighted', 'visibility', 'none')
                this.map.setFilter('campaign-layer-highlighted', ['==', ['get', 'sid'], submap.Id])
                this.map.setPaintProperty('campaign-layer-highlighted', 'line-color', highlightColor)
                this.map.setLayoutProperty('campaign-layer-highlighted', 'visibility', 'visible')

                let selectedLayer = this.state.campaginSource.features.filter((i) => i.id == submap.Id)?.[0]
                this.map.fitBounds([
                    [selectedLayer.bbox[0], selectedLayer.bbox[1]],
                    [selectedLayer.bbox[2], selectedLayer.bbox[3]],
                ])
            }
            return { selectedSubmap: submap.Id }
        })
    }

    onLayerHightlight(e) {
        let name = e.features?.[0]?.properties?.name
        let layer = this.state.activeLayer
        if (!name || !layer) {
            return
        }

        let newState = `${layer}-${name}`
        this.setState((state) => {
            if (state.highlightLayerName != newState) {
                log.info(`highlight ${layer} name ${name}`)
                this.map.setFilter(`area-layer-${layer}-selected`, ['==', 'name', name])
            }
            return { highlightLayerName: newState }
        })
    }

    onLayerSelect() {}

    render() {
        return (
            <div className="campaign full-container">
                <div className="grid-x grid-margin-x medium-margin-collapse" style={{ height: '100%' }}>
                    <div className="small-10 cell">{this.renderMapbox()}</div>
                    <div className="small-2 cell">{this.renderRightMenu()}</div>
                </div>
            </div>
        )
    }

    renderRightMenu() {
        let campaign = this.props.campaign
        let submaps = campaign.get('SubMaps', [])
        return (
            <div className="grid-y" style={{ height: '100%' }}>
                <div className="small-1 cell max-width-100 padding-horizontal-1">
                    <legend>Classifications</legend>
                    <div className="grid-x grid-margin-x margin-collapse small-up-3">
                        <div className="cell">
                            <input type="radio" name="mapType" id="none" onChange={this.onClassificationsChange} checked={this.state.activeLayer == null} />
                            <label htmlFor="none">None</label>
                        </div>
                        <div className="cell">
                            <input type="radio" name="mapType" id="3zip" onChange={this.onClassificationsChange} checked={this.state.activeLayer == '3zip'} />
                            <label htmlFor="3zip">3 ZIP</label>
                        </div>
                        <div className="cell">
                            <input type="radio" name="mapType" id="5zip" onChange={this.onClassificationsChange} checked={this.state.activeLayer == '5zip'} />
                            <label htmlFor="5zip">5 ZIP</label>
                        </div>
                        {/* <div className="cell">
                            <input type="radio" name="mapType" id="trk" onChange={this.onClassificationsChange} />
                            <label htmlFor="trk">TRK</label>
                        </div> */}
                        {/* <div className="cell">
                            <input type="radio" name="mapType" id="bg" onChange={this.onClassificationsChange} />
                            <label htmlFor="bg">{"BG's"}</label>
                        </div> */}
                        <div className="cell auto">
                            <input type="radio" name="mapType" id="croute" onChange={this.onClassificationsChange} checked={this.state.activeLayer == 'croute'} />
                            <label htmlFor="croute">CRoute</label>
                        </div>
                    </div>
                </div>
                <div className="small-10 cell max-width-100 padding-top-1">
                    <div className="row small-collapse">
                        <div className="columns small-6">
                            <button className="button expanded">SubMaps</button>
                        </div>
                        <div className="columns small-6">
                            <button className="button expanded secondary">Address</button>
                        </div>
                        {/* <div className="columns small-12">New|Edit|Delete|Add All Shapes|Remove All Shapes|Deselect All Shapes</div> */}
                    </div>
                    <div ref={this.onInitSubMapScrollbar} style={{ overflow: 'hidden scroll', height: '640px' }}>
                        <div className="row">
                            {submaps.map((s) => {
                                let boxStyle = {
                                    width: '28px',
                                    height: '28px',
                                    backgroundColor: `#${s.ColorString}`,
                                }
                                let desc = `Total: ${s.Total.toLocaleString('en-US', { minimumFractionDigits: 0 })} Count: ${s.CountAdjustment} Pen: ${s.Penetration}`
                                let buttonClassName = ClassNames('button expanded border-none padding-0 margin-0 text-left', {
                                    hollow: this.state.selectedSubmap != s.Id,
                                })
                                return (
                                    <div
                                        style={{ cursor: 'pointer', marginBottom: '5px' }}
                                        className="columns small-12"
                                        key={s.Id}
                                        title={s.Id}
                                        onClick={() => this.onSubmapSelect(s)}
                                    >
                                        <div className={buttonClassName}>
                                            <div className="grid-x align-middle">
                                                <div className="cell shrink">
                                                    <div style={boxStyle}></div>
                                                </div>
                                                <div className="cell auto padding-left-1">
                                                    <div className="margin-0 padding-0 font-medium">{`${s.OrderId}. ${s.Name}`}</div>
                                                    <div className="margin-0 padding-0 font-small">{desc}</div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                )
                            })}
                        </div>
                    </div>
                </div>
                <div className="small-1 cell max-width-100"></div>
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
                            <button className="mapboxgl-ctrl-icon mapboxgl-ctrl-img mapboxgl-ctrl-stree"></button>
                            <button className="mapboxgl-ctrl-icon mapboxgl-ctrl-img mapboxgl-ctrl-satellite"></button>
                        </div>
                    </div>
                </div>
                <div className="map-top-center"></div>
            </div>
        )
    }
}
