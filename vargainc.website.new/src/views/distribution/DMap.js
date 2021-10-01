// import mapboxgl from '!mapbox-gl/dist/mapbox-gl-dev'
import mapboxgl from 'mapbox-gl'
import React from 'react'
import PropTypes from 'prop-types'
import { color } from 'd3-color'
import axios from 'axios'
import Logger from 'logger.mjs'
import ClassNames from 'classnames'

import Helper from 'views/base'

const log = new Logger('views:campaign')

const layers = [
    {
        layer: 'PremiumCRoute',
        zoom: [11, 24],
        fontScale: [0.75, 0.75],
        settings: [
            {
                id: 'boundaries',
                type: 'line',
                layout: {
                    'line-join': 'round',
                    'line-cap': 'round',
                },
                paint: {
                    'line-color': 'hsl(250, 76%, 67%)',
                    'line-width': {
                        base: 1.5,
                        stops: [
                            [5, 0.25],
                            [18, 6],
                        ],
                    },
                },
            },
        ],
    },
    {
        layer: 'Z5',
        zoom: [11, 24],
        fontScale: [1, 0.75],
        settings: [
            {
                id: 'boundaries',
                type: 'line',
                layout: {
                    'line-join': 'round',
                    'line-cap': 'round',
                },
                paint: {
                    'line-color': 'hsl(215, 36%, 59%)',
                    'line-width': {
                        base: 1.5,
                        stops: [
                            [5, 0.5],
                            [18, 26],
                        ],
                    },
                },
            },
        ],
    },
    {
        layer: 'Z3',
        zoom: [11, 24],
        fontScale: [1.25, 1],
        settings: [
            {
                id: 'boundaries',
                type: 'line',
                layout: {
                    'line-cap': 'round',
                    'line-join': 'round',
                },
                paint: {
                    'line-color': 'hsl(215, 20%, 42%)',
                    'line-width': {
                        base: 1.5,
                        stops: [
                            [5, 1],
                            [18, 32],
                        ],
                    },
                },
            },
        ],
    },
]

const Classification = new Map([
    [0, 'Z3'],
    [1, 'Z5'],
    [15, 'PremiumCRoute'],
])

export default class DMap extends React.Component {
    static propTypes = {
        campaigin: PropTypes.object,
    }

    constructor(props) {
        super(props)
        this.state = {
            mapInited: false,
            mapReady: false,
            activeLayers: new Set(),
            allowedLayers: new Set(['Z3', 'Z5', 'PremiumCRoute']),
            selectedShapes: new Map(),
            selectedSubmapId: null,
            campaginSource: { features: [] },
        }

        this.onInitMenuScrollbar = this.onInitMenuScrollbar.bind(this)
        this.onInitMap = this.onInitMap.bind(this)
        this.onMapLoad = this.onMapLoad.bind(this)
        this.loadCampaignGeojson = this.loadCampaignGeojson.bind(this)

        this.onSubmapSelect = this.onSubmapSelect.bind(this)

        this.subscribe = Helper.subscribe.bind(this)
        this.doUnsubscribe = Helper.doUnsubscribe.bind(this)

        this.subscribe('DMap.Refresh', () => {
            this.loadSubmapList()
            this.initCampaignLayer()
        })
    }

    componentWillUnmount() {
        this.doUnsubscribe()
    }

    onInitMenuScrollbar(el) {
        if (el) {
            let containerHeight = $(el).parent().height()
            let menuHeight = $(el).prev().height()
            $(el).css({ height: `${containerHeight - menuHeight}px` })
        }
    }

    onInitMap(mapContainer) {
        if (mapContainer) {
            this.setState((state) => {
                if (state.mapInited == false) {
                    mapboxgl.accessToken = MapboxToken
                    let zoom = this.props.campaign.ZoomLevel ?? 8
                    let center = [this.props.campaign.Longitude ?? -73.987378, this.props.campaign.Latitude ?? 40.744556]
                    this.map = new mapboxgl.Map({
                        container: mapContainer,
                        zoom: zoom,
                        maxZoom: 20,
                        center: center,
                        // style: '/map/street.json',
                        style: 'mapbox://styles/mapbox/outdoors-v11',
                        // style: 'mapbox://styles/mapbox/navigation-guidance-day-v4',
                        // style: 'mapbox://styles/mapbox/navigation-preview-night-v4',
                    })
                    this.map.on('load', this.onMapLoad)
                    log.info(`mapbox inited`)
                }
                return { mapInited: true }
            })
        }
    }

    onMapLoad() {
        const layers = this.map.getStyle().layers
        // Find the index of the first symbol layer in the map style
        let labelLayer = null
        for (const layer of layers) {
            log.info(layer.id)
            if (labelLayer == null && layer.id.indexOf('label') > -1) {
                labelLayer = layer.id
            }
        }

        // this.map.on('zoom', () => {
        //     log.info(`map zoom: ${this.map.getZoom()}`)
        // })

        this.setState(
            {
                mapLabelLayer: labelLayer,
            },
            () => {
                this.loadCampaignGeojson()
            }
        )
    }

    loadCampaignGeojson() {
        let labelLayer = this.state.mapLabelLayer
        axios.get(`campaign/${this.props.campaign.Id}/dmap/geojson`).then((resp) => {
            if (this.state.mapReady == true) {
                this.map.getSource('map-source').setData(resp.data)
            } else {
                this.map.addSource('map-source', { type: 'geojson', data: resp.data })
                // submap
                this.map.addLayer(
                    {
                        id: 'submap-layer-line',
                        type: 'line',
                        source: 'map-source',
                        layout: {},
                        paint: {
                            'line-color': ['get', 'color'],
                            'line-width': 4,
                        },
                        filter: ['==', ['get', 'type'], 'submap'],
                    },
                    labelLayer
                )

                // dmap
                this.map.addLayer(
                    {
                        id: 'submap-layer-line',
                        type: 'line',
                        source: 'map-source',
                        layout: {},
                        paint: {
                            'line-color': ['get', 'color'],
                            'line-width': 4,
                        },
                        filter: ['==', ['get', 'type'], 'submap'],
                    },
                    labelLayer
                )
                this.map.addLayer(
                    {
                        id: 'dmap-layer-fill',
                        type: 'fill',
                        source: 'map-source',
                        layout: {},
                        paint: {
                            'fill-color': ['get', 'color'],
                            'fill-opacity': 0.5,
                        },
                        filter: ['==', ['get', 'type'], 'dmap'],
                    },
                    labelLayer
                )
                this.map.addLayer(
                    {
                        id: 'dmap-layer-line',
                        type: 'line',
                        source: 'map-source',
                        layout: {},
                        paint: {
                            'line-color': ['get', 'color'],
                            'line-width': 2,
                        },
                        filter: ['==', ['get', 'type'], 'dmap'],
                    },
                    labelLayer
                )

                // area
                this.map.addLayer(
                    {
                        id: 'area-layer-line',
                        type: 'line',
                        source: 'map-source',
                        layout: {},
                        paint: {
                            'line-color': 'hsl(250, 76%, 67%)',
                            'line-width': {
                                base: 1.5,
                                stops: [
                                    [5, 0.25],
                                    [18, 6],
                                ],
                            },
                        },
                        filter: ['==', ['get', 'type'], 'area'],
                    },
                    labelLayer
                )

                // submap highlight
                this.map.addLayer(
                    {
                        id: 'submap-layer-highlight',
                        type: 'line',
                        source: 'map-source',
                        layout: {},
                        paint: {
                            'line-width': 8,
                        },
                        filter: ['==', ['get', 'sid'], ''],
                    },
                    labelLayer
                )

                // fit all submap
                let mapBbox = (resp.data.features ?? [])
                    .filter((i) => i.properties?.type == 'submap')
                    .reduce(
                        (bbox, item) => {
                            return [
                                bbox[0] ? Math.min(bbox[0], item.bbox[0]) : item.bbox[0],
                                bbox[1] ? Math.min(bbox[1], item.bbox[1]) : item.bbox[1],
                                bbox[2] ? Math.max(bbox[2], item.bbox[2]) : item.bbox[2],
                                bbox[3] ? Math.max(bbox[3], item.bbox[3]) : item.bbox[3],
                            ]
                        },
                        [null, null, null, null]
                    )
                this.map.fitBounds([
                    [mapBbox[0], mapBbox[1]],
                    [mapBbox[2], mapBbox[3]],
                ])
            }
            this.setState({
                mapSource: resp.data,
                mapReady: true,
            })
        })
    }

    onSubmapSelect(subMap) {
        if (!this.state.mapReady) {
            return
        }
        if (this.state.selectedSubmapId == subMap.Id) {
            return
        }

        this.setState(
            (state) => {
                let submapGeo = state.mapSource.features.filter((i) => i.id == `submap-${subMap.Id}`)?.[0]

                if (submapGeo) {
                    this.map.fitBounds([
                        [submapGeo.bbox[0], submapGeo.bbox[1]],
                        [submapGeo.bbox[2], submapGeo.bbox[3]],
                    ])
                }

                return { selectedSubmapId: subMap.Id }
            },
            () => {
                //set selected layer fill color as submap color
                this.map.setLayoutProperty('submap-layer-highlight', 'visibility', 'none')
                this.map.setFilter('submap-layer-highlight', ['all', ['==', 'sid', subMap.Id], ['==', 'type', 'submap']])
                this.map.setPaintProperty('submap-layer-highlight', 'line-color', `#${subMap.ColorString}`)
                this.map.setLayoutProperty('submap-layer-highlight', 'visibility', 'visible')
            }
        )
    }

    render() {
        return (
            <div className="dmap full-container">
                <div className="grid-x grid-margin-x medium-margin-collapse" style={{ height: '100%' }}>
                    <div className="small-10 cell">{this.renderMapbox()}</div>
                    <div className="small-2 cell">{this.renderRightMenu()}</div>
                </div>
            </div>
        )
    }

    renderRightMenu() {
        let submaps = this.props?.campaign?.SubMaps ?? []
        return (
            <div className="grid-y" style={{ height: '100%' }}>
                <div className="small-11 cell max-width-100">
                    <div className="row small-collapse">
                        <div className="columns small-12">
                            <button className="button expanded margin-0">Distribution Maps</button>
                        </div>
                        <div className="columns small-12">
                            <div className="button-group no-gaps clear small tiny-button-group">
                                <div className="button padding-horizontal-1 padding-vertical-0">New</div>
                                <div className="button padding-horizontal-1 padding-vertical-0">Edit</div>
                                <div className="button padding-horizontal-1 padding-vertical-0">Delete</div>
                            </div>
                        </div>
                    </div>
                    <div ref={this.onInitMenuScrollbar} style={{ overflow: 'hidden scroll', height: '640px' }}>
                        <div className="row">
                            {submaps.flatMap((s, index) => {
                                let boxStyle = {
                                    width: '28px',
                                    height: '28px',
                                    backgroundColor: `#${s.ColorString}`,
                                }
                                let desc = `Total: ${s.Total.toLocaleString('en-US', { minimumFractionDigits: 0 })} Count: ${s.CountAdjustment} Pen: ${s.Penetration}`
                                let buttonClassName = ClassNames('button expanded border-none padding-0 margin-0 text-left', {
                                    hollow: this.state.selectedSubmapId != s.Id,
                                })
                                let result = [
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
                                                    <div className="margin-0 padding-0 font-medium">{`${index + 1}. ${s.Name}`}</div>
                                                    <div className="margin-0 padding-0 font-small">{desc}</div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>,
                                ]
                                s.DMaps.forEach((d) => {
                                    boxStyle.backgroundColor = `#${d.ColorString}`
                                    desc = `Total: ${d.Total.toLocaleString('en-US', { minimumFractionDigits: 0 })} Count: ${d.TotalAdjustment} Pen: ${d.Penetration}`
                                    result.push(
                                        <div
                                            style={{ cursor: 'pointer', marginBottom: '5px' }}
                                            className="columns small-12 padding-left-2"
                                            key={`${s.Id}-${d.Id}`}
                                            title={d.Id}
                                            onClick={() => this.onSubmapSelect(s)}
                                        >
                                            <div className={buttonClassName}>
                                                <div className="grid-x align-middle">
                                                    <div className="cell shrink">
                                                        <div style={boxStyle}></div>
                                                    </div>
                                                    <div className="cell auto padding-left-1">
                                                        <div className="margin-0 padding-0 font-medium">{d.Name}</div>
                                                        <div className="margin-0 padding-0 font-small">{desc}</div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    )
                                })

                                return result
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
