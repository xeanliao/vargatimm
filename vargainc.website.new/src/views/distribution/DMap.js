// import mapboxgl from '!mapbox-gl/dist/mapbox-gl-dev'
import mapboxgl from 'mapbox-gl'
import React from 'react'
import PropTypes from 'prop-types'
import { color } from 'd3-color'
import axios from 'axios'
import Logger from 'logger.mjs'
import ClassNames from 'classnames'
import { ceil } from 'lodash'

import Helper from 'views/base'
import DMapEdit from './DMapEdit'

const log = new Logger('views:campaign')

const layersDefine = [
    {
        layer: 'PremiumCRoute',
        zoom: [0, 24],
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
                    streets: {
                        'line-color': 'hsl(250, 76%, 67%)',
                        'line-width': {
                            base: 1.5,
                            stops: [
                                [5, 0.25],
                                [18, 6],
                            ],
                        },
                    },
                    satellite: {
                        'line-color': 'hsl(250, 76%, 90%)',
                        'line-width': {
                            base: 1.5,
                            stops: [
                                [5, 0.25],
                                [18, 6],
                            ],
                        },
                    },
                },
            },
        ],
    },
    {
        layer: 'Z5',
        zoom: [0, 24],
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
                    streets: {
                        'line-color': 'hsl(215, 36%, 59%)',
                        'line-width': {
                            base: 1.5,
                            stops: [
                                [5, 0.5],
                                [18, 26],
                            ],
                        },
                    },
                    satellite: {
                        'line-color': 'hsl(215, 36%, 90%)',
                        'line-width': {
                            base: 1.5,
                            stops: [
                                [5, 0.5],
                                [18, 26],
                            ],
                        },
                    },
                },
            },
        ],
    },
    {
        layer: 'Z3',
        zoom: [0, 24],
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
                    streets: {
                        'line-color': 'hsl(215, 20%, 42%)',
                        'line-width': {
                            base: 1.5,
                            stops: [
                                [5, 1],
                                [18, 32],
                            ],
                        },
                    },
                    satellite: {
                        'line-color': 'hsl(215, 20%, 90%)',
                        'line-width': {
                            base: 1.5,
                            stops: [
                                [5, 1],
                                [18, 32],
                            ],
                        },
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
            mapInit: false,
            mapReady: false,
            activeLayers: new Set(),
            allowedLayers: new Set(['Z3', 'Z5', 'PremiumCRoute']),
            selectedShapes: new Map(),
            selectedSubmapId: null,
            // campaignSource: { features: [] },
            mapStyle: 'streets', //'satellite'
            holes: [],
            showHole: null,
        }

        this.onInitMenuScrollbar = this.onInitMenuScrollbar.bind(this)
        this.onInitMap = this.onInitMap.bind(this)
        this.onMapLoad = this.onMapLoad.bind(this)
        this.onSwitchMapStyle = this.onSwitchMapStyle.bind(this)
        this.loadMapList = this.loadMapList.bind(this)
        this.loadCampaignGeojson = this.loadCampaignGeojson.bind(this)
        this.onRefresh = this.onRefresh.bind(this)

        this.onSubmapSelect = this.onSubmapSelect.bind(this)
        this.onDMapSelect = this.onDMapSelect.bind(this)
        this.onShapeSelect = this.onShapeSelect.bind(this)
        this.onShowShapePopup = this.onShowShapePopup.bind(this)
        this.onHideShapePopup = this.onHideShapePopup.bind(this)

        this.onNewDMap = this.onNewDMap.bind(this)
        this.onEditDMap = this.onEditDMap.bind(this)
        this.onDeleteDMap = this.onDeleteDMap.bind(this)
        this.onSaveShapesToDMap = this.onSaveShapesToDMap.bind(this)

        this.onSearchZip = this.onSearchZip.bind(this)
        this.clearPopup = this.clearPopup.bind(this)

        this.onShowHole = this.onShowHole.bind(this)
        this.onSwitchHole = this.onSwitchHole.bind(this)

        this.subscribe = Helper.subscribe.bind(this)
        this.doUnsubscribe = Helper.doUnsubscribe.bind(this)

        this.subscribe('DMap.Refresh', () => {
            this.onRefresh()
        })
        this.subscribe('Search.Zip', this.onSearchZip)
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
                if (state.mapInit == false) {
                    mapboxgl.accessToken = MapboxToken
                    let zoom = this.props.campaign.ZoomLevel ?? 8
                    let center = [this.props.campaign.Longitude ?? -73.987378, this.props.campaign.Latitude ?? 40.744556]
                    this.map = new mapboxgl.Map({
                        container: mapContainer,
                        zoom: zoom,
                        maxZoom: 20,
                        center: center,
                        // style: './street.json',
                        style: 'mapbox://styles/mapbox/outdoors-v11',
                        // style: 'mapbox://styles/mapbox/satellite-v9',
                        // style: 'mapbox://styles/mapbox/navigation-guidance-day-v4',
                        // style: 'mapbox://styles/mapbox/navigation-preview-night-v4',
                    })
                    this.map.on('load', this.onMapLoad)
                    log.info(`mapbox init`)
                }
                return { mapInit: true }
            })
        }
    }

    loadMapList() {
        axios.get(`campaign/${this.props.campaign.Id}/dmap`).then((resp) => {
            this.props.campaign.SubMaps = resp.data.data.SubMaps
            this.forceUpdate()
        })
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
                    visibility: this.state.mapStyle == 'streets' ? 'none' : 'visible',
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

        // this.map.on('zoom', () => {
        //     log.info(`map zoom: ${this.map.getZoom()}`)
        // })

        this.map.loadImage('./images/remove_pattern.png', (err, image) => {
            this.map.addImage('remove_pattern', image)

            this.setState(
                {
                    mapLabelLayer: labelLayer,
                },
                () => {
                    this.loadCampaignGeojson()
                }
            )
        })
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
                    // this.map.setLayoutProperty('google-road-tiles-layer', 'visibility', 'visible')
                    // this.map.setLayoutProperty('google-satellite-tiles-layer', 'visibility', 'none')

                    // this.map.setStyle('mapbox://styles/mapbox/outdoors-v11')

                    for (const layer of this.map.getStyle().layers) {
                        if (!layer.id.startsWith('timm')) {
                            this.map.setLayoutProperty(layer.id, 'visibility', 'visible')
                        }
                    }

                    this.map.setLayoutProperty('timm-google-satellite-tiles-layer', 'visibility', 'none')
                } else {
                    // this.map.setLayoutProperty('google-road-tiles-layer', 'visibility', 'none')
                    // this.map.setLayoutProperty('google-satellite-tiles-layer', 'visibility', 'visible')

                    // this.map.setStyle('mapbox://styles/mapbox/satellite-v9')

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

    loadCampaignGeojson() {
        let labelLayer = this.state.mapLabelLayer
        axios.get(`campaign/${this.props.campaign.Id}/dmap/geojson`).then((resp) => {
            if (this.state.mapReady == true) {
                this.map.getSource('map-source').setData(resp.data)
            } else {
                this.map.addSource('map-source', { type: 'geojson', data: resp.data })

                // dmap
                this.map.addLayer(
                    {
                        id: 'timm-dmap-layer-fill',
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

                // area selected to add
                this.map.addLayer(
                    {
                        id: 'timm-area-layer-selected',
                        type: 'fill',
                        source: 'map-source',
                        layout: { visibility: 'none' },
                        paint: {
                            'fill-color': '#000000',
                            'fill-opacity': 0.5,
                        },
                        filter: ['==', 'id', ''],
                    },
                    labelLayer
                )

                // area selected to remove
                this.map.addLayer(
                    {
                        id: 'timm-area-layer-remove',
                        type: 'fill',
                        source: 'map-source',
                        layout: { visibility: 'none' },
                        paint: {
                            'fill-pattern': 'remove_pattern',
                        },
                        filter: ['==', 'id', ''],
                    },
                    labelLayer
                )

                // area line
                let currentMapStyle = this.state.mapStyle
                const mapStyles = ['streets', 'satellite']
                layersDefine.forEach((item) => {
                    item.settings.forEach((setting) => {
                        mapStyles.forEach((style) => {
                            let layout = Object.assign({}, setting.layout, { visibility: style == currentMapStyle ? 'visible' : 'none' })
                            this.map.addLayer(
                                {
                                    id: `timm-area-layer-${item.layer}-${setting.id}-${style}`,
                                    type: setting.type,
                                    source: 'map-source',
                                    layout: layout,
                                    paint: setting.paint[style],
                                    filter: ['all', ['==', ['get', 'type'], 'area'], ['==', ['get', 'classification'], item.layer]],
                                },
                                labelLayer
                            )
                        })
                    })
                })

                // area and dmap label
                mapStyles.forEach((style) => {
                    let countRule = this.props.campaign.AreaDescription
                    let detail = ''
                    let total = ''
                    switch (countRule) {
                        case 'APT ONLY':
                            detail = ['concat', 'MFDU:', ['number-format', ['get', 'apt'], { locale: 'en-US' }]]
                            total = ''
                            break
                        case 'HOME ONLY':
                            detail = ['concat', 'SFDU:', ['number-format', ['get', 'home'], { locale: 'en-US' }]]
                            total = ''
                            break
                        case 'APT + HOME':
                            detail = [
                                'concat',
                                'SFDU:',
                                ['number-format', ['get', 'home'], { locale: 'en-US' }],
                                ' ',
                                'MFDU:',
                                ['number-format', ['get', 'apt'], { locale: 'en-US' }],
                            ]
                            total = ['concat', 'Total:', ['number-format', ['+', ['get', 'home'], ['get', 'apt']], { locale: 'en-US' }]]
                            break
                    }
                    // area label
                    this.map.addLayer({
                        id: `timm-area-layer-label-${style}`,
                        type: 'symbol',
                        source: 'map-source',
                        layout: {
                            visibility: style == currentMapStyle ? 'visible' : 'none',
                            'text-max-width': 200,
                            'text-field': [
                                'format',
                                ['get', 'name'],
                                { 'font-scale': 1 },
                                '\n',
                                detail,
                                {
                                    'font-scale': 0.75,
                                },
                                '\n',
                                total,
                                {
                                    'font-scale': 0.75,
                                },
                            ],
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
                        filter: ['all', ['==', ['geometry-type'], 'Point'], ['==', ['get', 'type'], 'area']],
                    })

                    // dmap label
                    this.map.addLayer({
                        id: `timm-dmap-layer-label-${style}`,
                        type: 'symbol',
                        source: 'map-source',
                        layout: {
                            'text-max-width': 200,
                            'text-field': [
                                'format',
                                ['get', 'name'],
                                { 'font-scale': 1.25 },
                                '\n',
                                {},
                                'Total: ',
                                { 'font-scale': 1 },
                                ['number-format', ['get', 'total'], { locale: 'en-US', 'min-fraction-digits': 0 }],
                                { 'font-scale': 1 },
                                ' Count: ',
                                { 'font-scale': 1 },
                                ['number-format', ['get', 'count'], { locale: 'en-US', 'min-fraction-digits': 0 }],
                                { 'font-scale': 1 },
                                ' Pen: ',
                                { 'font-scale': 1 },
                                ['get', 'pen'],
                                { 'font-scale': 1 },
                            ],
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
                        filter: ['all', ['==', ['geometry-type'], 'Point'], ['==', ['get', 'type'], 'dmap']],
                    })
                })

                // dmap line
                this.map.addLayer(
                    {
                        id: 'timm-dmap-layer-intersection',
                        type: 'line',
                        source: 'map-source',
                        layout: {},
                        paint: {
                            'line-color': 'black',
                            'line-width': 2,
                            'line-dasharray': [2, 1],
                        },
                        filter: ['==', ['get', 'type'], 'dmap-intersection'],
                    },
                    labelLayer
                )

                // submap line
                this.map.addLayer(
                    {
                        id: 'timm-submap-layer-line',
                        type: 'line',
                        source: 'map-source',
                        layout: {},
                        paint: {
                            'line-color': ['get', 'color'],
                            'line-width': 6,
                        },
                        filter: ['==', ['get', 'type'], 'submap'],
                    },
                    labelLayer
                )

                // submap highlight
                this.map.addLayer(
                    {
                        id: 'timm-submap-layer-highlight',
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

                // area selection transparent layer for events
                this.map.addLayer(
                    {
                        id: 'timm-area-layer-fill',
                        type: 'fill',
                        source: 'map-source',
                        paint: {
                            'fill-color': 'white',
                            'fill-opacity': 0,
                        },
                        filter: ['==', ['get', 'type'], 'area'],
                    },
                    labelLayer
                )

                // submap holes boundary
                this.map.addLayer(
                    {
                        id: 'timm-submap-layer-holes-line',
                        type: 'line',
                        source: 'map-source',
                        paint: {
                            'line-color': '#ffffff',
                            'line-width': 2,
                            'line-dasharray': [2, 2],
                            'line-offset': 2,
                        },
                        filter: ['==', ['get', 'type'], 'submap-holes'],
                    },
                    labelLayer
                )

                // dmap holes boundary
                this.map.addLayer(
                    {
                        id: 'timm-dmap-layer-holes-line',
                        type: 'line',
                        source: 'map-source',
                        layout: { layout: { visibility: 'none' } },
                        paint: {
                            'line-color': '#ffffff',
                            'line-width': 2,
                            'line-dasharray': [2, 2],
                            'line-offset': 2,
                        },
                        filter: ['==', ['get', 'type'], 'dmap-holes'],
                    },
                    labelLayer
                )

                // holes highlight
                this.map.addLayer(
                    {
                        id: 'timm-area-layer-holes',
                        type: 'fill',
                        source: 'map-source',
                        paint: {
                            'fill-color': '#ffffff',
                        },
                        filter: ['==', ['get', 'name'], ''],
                    },
                    labelLayer
                )

                // event
                this.map.on('click', `timm-area-layer-fill`, this.onShapeSelect)
                this.map.off('contextmenu', `timm-area-layer-fill`, this.onShowShapePopup)
                this.map.off('mousemove', `timm-area-layer-fill`, this.onHideShapePopup)
                this.map.on('contextmenu', `timm-area-layer-fill`, this.onShowShapePopup)
                this.map.on('mousemove', `timm-area-layer-fill`, this.onHideShapePopup)

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
            this.setState(
                {
                    mapSource: resp.data,
                    mapReady: true,
                },
                () => {
                    // this.onSwitchMapStyle(this.state.mapStyle ?? 'satellite')
                }
            )
        })
    }

    onRefresh() {
        this.loadCampaignGeojson()
        this.loadMapList()
        this.setState({ selectedShapes: new Map() }, () => {
            this.map.setFilter(`timm-area-layer-remove`, ['==', 'vid', ''])
            this.map.setFilter(`timm-area-layer-selected`, ['==', 'vid', ''])
            this.map.setFilter(`timm-area-layer-holes`, ['==', 'name', ''])
        })
    }

    onSubmapSelect(subMap) {
        if (!this.state.mapReady) {
            return Promise.resolve()
        }
        if (this.state.selectedSubmapId == subMap.Id) {
            return Promise.resolve()
        }
        return new Promise((resolve) => {
            this.setState(
                (state) => {
                    let submapGeo = state.mapSource.features.filter((i) => i.id == `submap-${subMap.Id}`)?.[0]

                    if (submapGeo) {
                        this.map.fitBounds([
                            [submapGeo.bbox[0], submapGeo.bbox[1]],
                            [submapGeo.bbox[2], submapGeo.bbox[3]],
                        ])
                    }

                    return { selectedSubmapId: subMap.Id, selectedDMapId: null, selectedShapes: new Map() }
                },
                () => {
                    //set selected layer fill color as submap color
                    let highlightColor = color(`#${subMap.ColorString}`).darker(2).toString()
                    this.map.setLayoutProperty('timm-submap-layer-highlight', 'visibility', 'none')
                    this.map.setFilter('timm-submap-layer-highlight', ['all', ['==', 'sid', subMap.Id], ['==', 'type', 'submap']])
                    this.map.setPaintProperty('timm-submap-layer-highlight', 'line-color', highlightColor)
                    this.map.setLayoutProperty('timm-submap-layer-highlight', 'visibility', 'visible')

                    this.map.setLayoutProperty(`timm-area-layer-remove`, 'visibility', 'none')
                    this.map.setLayoutProperty(`timm-area-layer-selected`, 'visibility', 'none')
                    this.map.setFilter(`timm-area-layer-remove`, ['in', ['get', 'vid'], ['literal', []]])
                    this.map.setFilter(`timm-area-layer-selected`, ['in', ['get', 'vid'], ['literal', []]])
                    this.map.setLayoutProperty(`timm-area-layer-remove`, 'visibility', 'visible')
                    this.map.setLayoutProperty(`timm-area-layer-selected`, 'visibility', 'visible')

                    return resolve()
                }
            )
        })
    }

    onDMapSelect(dMap) {
        if (!this.state.mapReady) {
            return Promise.resolve()
        }
        if (this.state.selectedDMapId == dMap.Id) {
            return Promise.resolve()
        }
        let subMap = this.props.campaign.SubMaps.filter((s) => s.Id == dMap.SubMapId)?.[0]
        this.onSubmapSelect(subMap).then(() => {
            this.setState({ selectedDMapId: dMap.Id, selectedShapes: new Map(), holes: dMap.Holes }, () => {
                this.map.setLayoutProperty(`timm-area-layer-remove`, 'visibility', 'none')
                this.map.setLayoutProperty(`timm-area-layer-selected`, 'visibility', 'none')
                this.map.setPaintProperty(`timm-area-layer-selected`, 'fill-color', color(`#${dMap.ColorString}`).toString())
                this.map.setFilter(`timm-area-layer-remove`, ['in', ['get', 'vid'], ['literal', []]])
                this.map.setFilter(`timm-area-layer-selected`, ['in', ['get', 'vid'], ['literal', []]])
                this.map.setLayoutProperty(`timm-area-layer-remove`, 'visibility', 'visible')
                this.map.setLayoutProperty(`timm-area-layer-selected`, 'visibility', 'visible')
            })
        })
    }

    onShapeSelect(e) {
        if (!this.state.selectedSubmapId || !this.state.selectedDMapId) {
            return
        }
        let subMapId = this.state.selectedSubmapId
        let dMapId = this.state.selectedDMapId
        // geojson is no id
        // oid: shape primary key in database
        // vid: classification + oid
        // sid: submap id
        let properties = e.features?.[0]?.properties
        let oid = properties?.oid
        let vid = properties?.vid
        let sid = properties?.sid
        let name = properties?.name
        let classification = properties?.classification
        let apt = properties?.apt ?? 0
        let home = properties?.home ?? 0
        if (sid != subMapId) {
            return
        }

        let submaps = this.props?.campaign?.SubMaps ?? []

        let currentDMapAreas = submaps
            .filter((s) => s.Id == subMapId)
            .flatMap((s) => s.DMaps ?? [])
            .filter((d) => d.Id == dMapId)
            .flatMap((d) => d.DMapRecords ?? [])
            .map((r) => r.AreaId)

        currentDMapAreas = new Set(currentDMapAreas)

        this.setState(
            (state) => {
                let selectedShapes = new Map(state.selectedShapes)
                if (selectedShapes.has(vid)) {
                    selectedShapes.delete(vid)
                } else {
                    selectedShapes.set(vid, { Classification: classification, Id: oid, Value: !currentDMapAreas.has(oid), Name: name, vid: vid, apt: apt, home: home })
                }
                return {
                    selectedShapes: selectedShapes,
                }
            },
            () => {
                let shapes = Array.from(this.state.selectedShapes.values())
                let addShapes = shapes.filter((i) => i.Value == true).map((i) => i.vid)
                let removeShapes = shapes.filter((i) => i.Value == false).map((i) => i.vid)

                this.map.setLayoutProperty(`timm-area-layer-remove`, 'visibility', 'none')
                this.map.setLayoutProperty(`timm-area-layer-selected`, 'visibility', 'none')
                this.map.setFilter(`timm-area-layer-remove`, ['in', ['get', 'vid'], ['literal', removeShapes]])
                this.map.setFilter(`timm-area-layer-selected`, ['in', ['get', 'vid'], ['literal', addShapes]])
                this.map.setLayoutProperty(`timm-area-layer-remove`, 'visibility', 'visible')
                this.map.setLayoutProperty(`timm-area-layer-selected`, 'visibility', 'visible')
            }
        )
    }

    onShowShapePopup(e) {
        this.clearPopup()
        // this.onHideShapePopup()
        const coordinates = e.features[0].geometry.coordinates.slice()
        // Ensure that if the map is zoomed out such that multiple
        // copies of the feature are visible, the popup appears
        // over the copy being pointed to.
        while (Math.abs(e.lngLat.lng - coordinates[0]) > 180) {
            coordinates[0] += e.lngLat.lng > coordinates[0] ? 360 : -360
        }

        let home = e.features[0].properties.home
        let apt = e.features[0].properties.apt
        let name = e.features[0].properties.name
        let lnglat = e.lngLat

        let popup = new mapboxgl.Popup({
            closeButton: false,
            closeOnClick: false,
        })

        let countRule = this.props.campaign.AreaDescription
        let content = ''
        switch (countRule) {
            case 'APT ONLY':
                content = `MFDU:${new Intl.NumberFormat('en-US').format(apt)}`
                break
            case 'HOME ONLY':
                content = `SFDU:${new Intl.NumberFormat('en-US').format(home)}`
                break
            case 'APT + HOME':
                content = `MFDU:${new Intl.NumberFormat('en-US').format(apt)} SFDU:${new Intl.NumberFormat('en-US').format(home)}<br />Total:${new Intl.NumberFormat(
                    'en-US'
                ).format(apt + home)}`
                break
        }

        popup.setLngLat(lnglat).setHTML(`<h6>${name}</h6><p>${content}</p>`).addTo(this.map)

        this.setState({ popup: popup })
    }

    onHideShapePopup() {
        if (this.state.popup) {
            this.state.popup.remove()
            this.setState({ popup: null })
        }
    }

    onNewDMap() {
        let subMaps = this.props?.campaign.SubMaps.map((s) => {
            return {
                Id: s.Id,
                Name: s.Name,
            }
        })
        let dMapCount = this.props?.campaign?.SubMaps.flatMap((i) => i.DMaps).length
        let colorIndex = (this.props?.campaign?.Id ?? 0) + dMapCount ?? 0
        let color = Helper.randomColor(colorIndex).replace('#', '')
        let model = { CampaignId: this.props?.campaign?.Id, ColorString: color, SubMaps: subMaps }
        Helper.publish('showDialog', {
            view: <DMapEdit model={model} registeredTopic={{}} registeredEvents={[]} />,
            options: {
                size: 'tiny',
            },
        })
    }

    onEditDMap() {
        if (!this.state.selectedDMapId) {
            return
        }
        let subMaps = this.props?.campaign.SubMaps.map((s) => {
            return {
                Id: s.Id,
                Name: s.Name,
            }
        })
        let dMap = this.props.campaign.SubMaps.flatMap((i) => i.DMaps).filter((i) => i.Id == this.state.selectedDMapId)?.[0]
        let model = {
            SubMaps: subMaps,
            CampaignId: this.props?.campaign?.Id,
            Name: dMap.Name,
            ColorString: dMap.ColorString,
            SubMapId: dMap.SubMapId,
            Id: this.state.selectedDMapId,
        }
        Helper.publish('showDialog', {
            view: <DMapEdit model={model} registeredTopic={{}} registeredEvents={[]} />,
            options: {
                size: 'tiny',
            },
        })
    }

    onDeleteDMap() {
        if (!this.state.selectedDMapId) {
            return
        }

        let dMap = this.props.campaign.SubMaps.flatMap((i) => i.DMaps).filter((i) => i.Id == this.state.selectedDMapId)?.[0]

        axios.delete(`campaign/${this.props.campaign.Id}/dmap/${dMap.Id}/delete`).then(() => {
            this.onRefresh()
        })
    }

    onSaveShapesToDMap() {
        if (!this.state.selectedDMapId) {
            return
        }
        if (this.state.selectedShapes.size == 0) {
            return
        }
        let data = Array.from(this.state.selectedShapes.values()).map((i) => {
            return {
                Classification: i.Classification,
                Id: i.Id,
                Value: i.Value,
            }
        })
        let url = `campaign/${this.props.campaign.Id}/dmap/${this.state.selectedDMapId}/merge`
        axios.post(url, data).then((rep) => {
            if (rep.data.success) {
                this.setState({ holes: rep.data.holes }, this.onRefresh)
            }
        })
    }

    onShowHole(code) {
        if (code == this.state.showHole) {
            this.setState({ showHole: null })
            this.map.setFilter(`timm-area-layer-holes`, ['==', 'name', ''])
        } else {
            this.setState({ showHole: code })
            this.map.setFilter(`timm-area-layer-holes`, ['==', 'name', code])
        }
    }

    onSwitchHole(hole) {
        var dMap = this.props.campaign.SubMaps.flatMap((s) => s.DMaps).filter((d) => d.Id == this.state.selectedDMapId)?.[0]
        let classification = dMap.DMapRecords?.[0]?.Classification
        let classificationName = Classification.get(classification)
        let id = `${classificationName}-${hole.AreaId}`
        let selectedShapes = new Map(this.state.selectedShapes)
        if (selectedShapes.has(id)) {
            selectedShapes.delete(id)
        } else {
            selectedShapes.set(id, {
                Classification: classification,
                Id: hole.AreaId,
                Value: true,
                Name: hole.Code,
                vid: id,
                apt: hole.Apt,
                home: hole.Home,
            })
        }

        this.setState({ selectedShapes: selectedShapes }, () => {
            let shapes = Array.from(this.state.selectedShapes.values())
            let addShapes = shapes.filter((i) => i.Value == true).map((i) => i.vid)

            this.map.setFilter(`timm-area-layer-selected`, ['in', ['get', 'vid'], ['literal', addShapes]])
        })
    }

    onSearchZip(searchKey) {
        if (searchKey == null || searchKey.trim().length != 5) {
            return
        }
        axios.get(`area/zip/${searchKey}`).then((resp) => {
            if (resp.data.success) {
                let lnglat = [resp.data.result.lng, resp.data.result.lat]
                this.map.setCenter(lnglat)
                let name = resp.data.result.name
                let apt = resp.data.result.apt
                let home = resp.data.result.home
                let countRule = this.props.campaign.AreaDescription
                let content = ''
                switch (countRule) {
                    case 'APT ONLY':
                        content = `MFDU:${new Intl.NumberFormat('en-US').format(apt)}`
                        break
                    case 'HOME ONLY':
                        content = `SFDU:${new Intl.NumberFormat('en-US').format(home)}`
                        break
                    case 'APT + HOME':
                        content = `MFDU:${new Intl.NumberFormat('en-US').format(apt)} SFDU:${new Intl.NumberFormat('en-US').format(home)}<br />Total:${new Intl.NumberFormat(
                            'en-US'
                        ).format(apt + home)}`
                        break
                }
                this.clearPopup()
                const popup = new mapboxgl.Popup({ closeOnClick: false }).setLngLat(lnglat).setHTML(`<h6>${name}</h6><p>${content}</p>`).addTo(this.map)
                if (this.map.getZoom() < 10) {
                    this.map.setZoom(10)
                    this.map.setCenter(lnglat)
                }
                this.setState({ searchResultPopup: popup })
            }
        })
    }

    clearPopup() {
        if (this.state.searchResultPopup) {
            this.state.searchResultPopup.remove()
        }
        if (this.state.popup) {
            this.state.popup.remove()
        }
    }

    render() {
        return (
            <div className="dmap full-container">
                <div className="grid-x grid-margin-x medium-margin-collapse" style={{ height: '100%' }}>
                    <div className="small-10 cell">{this.renderMapbox()}</div>
                    <div className="small-2 cell bordered">{this.renderRightMenu()}</div>
                </div>
            </div>
        )
    }

    renderRightMenu() {
        let areaDescription = this.props?.campaign?.AreaDescription
        let submaps = this.props?.campaign?.SubMaps ?? []
        return (
            <div className="grid-y" style={{ height: '100%' }}>
                <div className="small-11 cell max-width-100">
                    <div className="row small-collapse">
                        <div className="columns small-12">
                            <button className="button expanded margin-0">Distribution Maps</button>
                        </div>
                        <div className="columns small-12">
                            <div className="button-group no-gaps clear small tiny-button-group margin-top-1">
                                <div className="button padding-horizontal-1 padding-vertical-0" onClick={this.onNewDMap}>
                                    New
                                </div>
                                <div className="button padding-horizontal-1 padding-vertical-0" onClick={this.onEditDMap}>
                                    Edit
                                </div>
                                <div className="button padding-horizontal-1 padding-vertical-0" onClick={this.onDeleteDMap}>
                                    Delete
                                </div>
                                <div className="button padding-horizontal-1 padding-vertical-0" onClick={this.onSaveShapesToDMap}>
                                    Save Shapes
                                </div>
                            </div>
                        </div>
                    </div>
                    <div ref={this.onInitMenuScrollbar} style={{ overflow: 'hidden scroll', height: '640px' }}>
                        <div className="row">
                            {submaps.flatMap((s, index) => {
                                let subMapBoxStyle = {
                                    width: '28px',
                                    height: '28px',
                                    backgroundColor: `#${s.ColorString}`,
                                }
                                let fixTotal = (s.Total ?? 0) + (s.TotalAdjustment ?? 0)
                                let fixTarget = (s.Penetration ?? 0) + (s.CountAdjustment ?? 0)
                                let fixPercent = fixTotal > 0 ? ceil(fixTarget / fixTotal, 4) : 0
                                let fixTotalWithFormat = fixTotal.toLocaleString('en-US', { minimumFractionDigits: 0 })
                                let fixTargetWithFormat = fixTarget.toLocaleString('en-US', { minimumFractionDigits: 0 })
                                let fixPercentWithFormat = fixPercent.toLocaleString('en-US', { style: 'percent', minimumFractionDigits: 2 })
                                let subMapDesc = `Total: ${fixTotalWithFormat} Count: ${fixTargetWithFormat} Pen: ${fixPercentWithFormat}`
                                let subMapButtonClassName = ClassNames('button expanded border-none padding-0 margin-0 text-left', {
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
                                        <div className={subMapButtonClassName}>
                                            <div className="grid-x align-middle">
                                                <div className="cell shrink">
                                                    <div style={subMapBoxStyle}></div>
                                                </div>
                                                <div className="cell auto padding-left-1">
                                                    <div className="margin-0 padding-0 font-medium">{`${index + 1}. ${s.Name}`}</div>
                                                    <div className="margin-0 padding-0 font-small">{subMapDesc}</div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>,
                                ]
                                s.DMaps.forEach((d) => {
                                    let dMapBoxStyle = {
                                        width: '28px',
                                        height: '28px',
                                        backgroundColor: `#${d.ColorString}`,
                                    }

                                    let fixTotal = (d.Total ?? 0) + (d.TotalAdjustment ?? 0)
                                    let fixTarget = (d.Penetration ?? 0) + (d.CountAdjustment ?? 0)

                                    if (this.state.selectedDMapId == d.Id) {
                                        // calc select shapes
                                        Array.from(this.state.selectedShapes.values()).forEach((i) => {
                                            switch (areaDescription) {
                                                case 'APT ONLY':
                                                    fixTotal = i.Value === true ? fixTotal + i.apt : fixTotal - i.apt
                                                    break
                                                case 'HOME ONLY':
                                                    fixTotal = i.Value === true ? fixTotal + i.home : fixTotal - i.home
                                                    break
                                                case 'APT + HOME':
                                                    fixTotal = i.Value === true ? fixTotal + i.apt + i.home : fixTotal - i.apt - i.home
                                                    break
                                            }
                                        })
                                    }

                                    let fixPercent = fixTotal > 0 ? ceil(fixTarget / fixTotal, 4) : 0
                                    let fixTotalWithFormat = fixTotal.toLocaleString('en-US', { minimumFractionDigits: 0 })
                                    let fixTargetWithFormat = fixTarget.toLocaleString('en-US', { minimumFractionDigits: 0 })
                                    let fixPercentWithFormat = fixPercent.toLocaleString('en-US', { style: 'percent', minimumFractionDigits: 2 })
                                    let dMapDesc = `Total: ${fixTotalWithFormat} Count: ${fixTargetWithFormat} Pen: ${fixPercentWithFormat}`
                                    let dMapButtonClassName = ClassNames('button secondary expanded border-none padding-0 margin-0 text-left', {
                                        hollow: this.state.selectedDMapId != d.Id,
                                    })
                                    result.push(
                                        <div
                                            style={{ cursor: 'pointer', marginBottom: '5px' }}
                                            className="columns small-12 padding-left-2"
                                            key={`${s.Id}-${d.Id}`}
                                            title={d.Id}
                                            onClick={() => this.onDMapSelect(d)}
                                        >
                                            <div className={dMapButtonClassName}>
                                                <div className="grid-x align-middle">
                                                    <div className="cell shrink">
                                                        <div style={dMapBoxStyle}></div>
                                                    </div>
                                                    <div className="cell auto padding-left-1">
                                                        <div className="margin-0 padding-0 font-medium">{d.Name}</div>
                                                        <div className="margin-0 padding-0 font-small">{dMapDesc}</div>
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
                {this.renderHolesMenu()}
            </div>
        )
    }

    renderHolesMenu() {
        if (this.state.holes.length == 0) {
            return null
        }
        let selectedShapes = new Set(Array.from(this.state.selectedShapes.values()).map((i) => i.Name))
        return (
            <div
                className="map-right-center bg-white padding-1 bordered"
                style={{ right: '-1px', borderRight: '#f8f8f8', backgroundColor: '#f8f8f8', maxHeight: '400px', overflowY: 'auto' }}
            >
                <p className="h6 text-center">HOLES</p>
                <ul className="vertical menu">
                    {this.state.holes.map((i) => {
                        return (
                            <li key={i.AreaId} className="flex-container align-middle" style={{ marginBottom: '2px' }}>
                                <div className="switch tiny-small margin-0 display-inline">
                                    <input
                                        className="switch-input"
                                        id={`hole-${i.AreaId}`}
                                        type="checkbox"
                                        name="hole"
                                        checked={selectedShapes.has(i.Code)}
                                        value={i.AreaId}
                                        onChange={() => {
                                            this.onSwitchHole(i)
                                        }}
                                    />
                                    <label className="switch-paddle" htmlFor={`hole-${i.AreaId}`}></label>
                                </div>
                                <label htmlFor={`hole-${i.AreaId}`} style={{ paddingLeft: '5px', cursor: 'pointer' }} title={`APT: ${i.Apt} Home: ${i.Home}`}>
                                    {i.Code}
                                </label>
                            </li>
                        )
                    })}
                </ul>
            </div>
        )
    }
}
