import mapboxgl from '!mapbox-gl/dist/mapbox-gl-dev'
// import mapboxgl from 'mapbox-gl'
import * as Turf from '@turf/turf'
import React from 'react'
import PropTypes from 'prop-types'
import { color } from 'd3-color'
import { scaleOrdinal } from 'd3-scale'
import { interpolateSinebow } from 'd3-scale-chromatic'
import axios from 'axios'
import Logger from 'logger.mjs'
import ClassNames from 'classnames'

import Helper from 'views/base'
import SubmapEdit from './SubmapEdit'

const log = new Logger('views:campaign')
//streets
//satellite
const layers = [
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

export default class Campaign extends React.Component {
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
            campaignSource: { features: [] },
            mapStyle: 'streets',
        }

        this.onInitSubMapScrollbar = this.onInitSubMapScrollbar.bind(this)
        this.onInitMap = this.onInitMap.bind(this)
        this.onMapLoad = this.onMapLoad.bind(this)
        this.LoadBackgroundLayer = this.LoadBackgroundLayer.bind(this)
        this.loadCampaignGeojson = this.loadCampaignGeojson.bind(this)

        this.onRefresh = this.onRefresh.bind(this)

        this.onSubmapSelect = this.onSubmapSelect.bind(this)
        this.onClassificationsChange = this.onClassificationsChange.bind(this)
        this.onShapeSelect = this.onShapeSelect.bind(this)
        this.onShowShapePopup = this.onShowShapePopup.bind(this)
        this.onHideShapePopup = this.onHideShapePopup.bind(this)

        this.loadSubmapList = this.loadSubmapList.bind(this)
        this.onNewSubmap = this.onNewSubmap.bind(this)
        this.onEditSubmap = this.onEditSubmap.bind(this)
        this.onDeleteSubmap = this.onDeleteSubmap.bind(this)
        this.onSaveShapesToSubmap = this.onSaveShapesToSubmap.bind(this)
        this.onEmptySubmap = this.onEmptySubmap.bind(this)
        this.clearSelectedShapes = this.clearSelectedShapes.bind(this)
        this.onSelectAllScreenShapes = this.onSelectAllScreenShapes.bind(this)

        this.onSwitchMapStyle = this.onSwitchMapStyle.bind(this)
        this.onStyleChange = this.onStyleChange.bind(this)
        this.initCampaignLayers = this.initCampaignLayers.bind(this)
        this.restoreMapSelection = this.restoreMapSelection.bind(this)

        this.getActiveLayer = this.getActiveLayer.bind(this)

        this.map = null

        this.subscribe = Helper.subscribe.bind(this)
        this.doUnsubscribe = Helper.doUnsubscribe.bind(this)

        this.subscribe('CampaignMap.Refresh', this.onRefresh)

        this.saveCampaignLocation = this.saveCampaignLocation.bind(this)

        window.setInterval(this.saveCampaignLocation, 2 * 60 * 1000)
    }

    componentWillUnmount() {
        this.doUnsubscribe()
    }

    saveCampaignLocation() {
        let zoom = this.map.getZoom()
        let latlng = this.map.getCenter()
        axios.put(`campaign/${this.props.campaign.Id}/location/${zoom}/${latlng.lat}/${latlng.lng}")`)
    }

    onInitSubMapScrollbar(el) {
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
                    this.map.once('load', this.onMapLoad)
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
        // this.map.on('styledata', this.onStyleChange)
        this.map.loadImage('images/remove_pattern.png', (err, image) => {
            if (image) {
                this.map.addImage('remove_pattern', image)
                this.setState(
                    {
                        mapLabelLayer: labelLayer,
                    },
                    () => {
                        Promise.all([this.LoadBackgroundLayer(), this.loadCampaignGeojson()]).then(() => {
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

    LoadBackgroundLayer() {
        let testSourceExists = null
        let testLayerExist = null
        let labelLayer = this.state.mapLabelLayer
        let tileUrl = new URL('./api/area/tiles', `${location.protocol}//${location.host}${location.pathname}`)
        let mapStyle = this.state.mapStyle
        layers.forEach((item) => {
            let tileSource = `${tileUrl}/${item.layer}/{z}/{x}/{y}`
            let sourceName = `area-source-${item.layer}`
            testSourceExists = this.map.getSource(sourceName)
            if (!testSourceExists) {
                this.map.addSource(sourceName, {
                    type: 'vector',
                    tiles: [tileSource],
                    minzoom: 24,
                    maxzoom: 24,
                })
            }

            item.settings.forEach((setting) => {
                setting.layout['visibility'] = 'none'
                testLayerExist = this.map.getLayer(`timm-area-layer-${item.layer}-${setting.id}`)
                testLayerExist && this.map.removeLayer(`timm-area-layer-${item.layer}-${setting.id}`)
                this.map.addLayer(
                    {
                        id: `timm-area-layer-${item.layer}-${setting.id}`,
                        type: setting.type,
                        source: sourceName,
                        'source-layer': item.layer,
                        minzoom: item.zoom[0],
                        maxzoom: item.zoom[1],
                        layout: setting.layout,
                        paint: setting.paint[mapStyle],
                    },
                    labelLayer
                )
            })
            testLayerExist = this.map.getLayer(`timm-area-layer-${item.layer}-fill`)
            testLayerExist && this.map.removeLayer(`timm-area-layer-${item.layer}-fill`)
            this.map.addLayer(
                {
                    id: `timm-area-layer-${item.layer}-fill`,
                    type: 'fill',
                    source: `area-source-${item.layer}`,
                    'source-layer': item.layer,
                    minzoom: item.zoom[0],
                    maxzoom: item.zoom[1],
                    layout: { visibility: 'none' },
                    paint: {
                        'fill-color': 'white',
                        'fill-opacity': 0,
                    },
                },
                labelLayer
            )

            testLayerExist = this.map.getLayer(`timm-area-layer-${item.layer}-selected`)
            testLayerExist && this.map.removeLayer(`timm-area-layer-${item.layer}-selected`)
            this.map.addLayer(
                {
                    id: `timm-area-layer-${item.layer}-selected`,
                    type: 'fill',
                    source: `area-source-${item.layer}`,
                    'source-layer': item.layer,
                    minzoom: item.zoom[0],
                    maxzoom: item.zoom[1],
                    layout: { visibility: 'none' },
                    paint: {
                        'fill-color': '#000000',
                        'fill-opacity': 0.5,
                    },
                    filter: ['==', 'id', ''],
                },
                labelLayer
            )

            testLayerExist = this.map.getLayer(`timm-area-layer-${item.layer}-remove`)
            testLayerExist && this.map.removeLayer(`timm-area-layer-${item.layer}-remove`)
            this.map.addLayer(
                {
                    id: `timm-area-layer-${item.layer}-remove`,
                    type: 'fill',
                    source: `area-source-${item.layer}`,
                    'source-layer': item.layer,
                    minzoom: item.zoom[0],
                    maxzoom: item.zoom[1],
                    // layout: { visibility: 'none' },
                    paint: {
                        'fill-pattern': 'remove_pattern',
                    },
                    filter: ['==', 'id', ''],
                },
                labelLayer
            )

            testLayerExist = this.map.getLayer(`timm-area-layer-${item.layer}-label`)
            testLayerExist && this.map.removeLayer(`timm-area-layer-${item.layer}-label`)
            let countRule = this.props.campaign.AreaDescription
            let detail = ''
            let total = ''
            switch (countRule) {
                case 'APT ONLY':
                    detail = ['concat', 'MFDU:', ['number-format', ['get', 'apt'], { locale: 'en-Latn-US' }]]
                    total = ''
                    break
                case 'HOME ONLY':
                    detail = ['concat', 'SFDU:', ['number-format', ['get', 'home'], { locale: 'en-Latn-US' }]]
                    total = ''
                    break
                case 'APT + HOME':
                    detail = [
                        'concat',
                        'SFDU:',
                        ['number-format', ['get', 'home'], { locale: 'en-Latn-US' }],
                        ' ',
                        'MFDU:',
                        ['number-format', ['get', 'apt'], { locale: 'en-Latn-US' }],
                    ]
                    total = ['concat', 'Total:', ['number-format', ['+', ['get', 'home'], ['get', 'apt']], { locale: 'en-Latn-US' }]]
                    break
            }
            this.map.addLayer({
                id: `timm-area-layer-${item.layer}-label`,
                type: 'symbol',
                source: sourceName,
                'source-layer': item.layer,
                minzoom: item.zoom[0],
                maxzoom: item.zoom[1],
                layout: {
                    visibility: 'none',
                    'text-max-width': 200,
                    'text-field': [
                        'format',
                        ['get', 'name'],
                        { 'font-scale': item.fontScale[0] },
                        '\n',
                        detail,
                        {
                            'font-scale': item.fontScale[1],
                        },
                        '\n',
                        total,
                        {
                            'font-scale': item.fontScale[1],
                        },
                    ],
                },
                paint: {
                    'text-color': mapStyle == 'streets' ? 'black' : 'white', //'hsl(0, 0%, 0%)',
                    'text-halo-color': ['interpolate', ['linear'], ['zoom'], 2, 'rgba(255, 255, 255, 0.75)', 3, 'rgb(255, 255, 255)'],
                    'text-halo-width': item.fontScale[1],
                },
                filter: ['==', ['geometry-type'], 'Point'],
            })

            this.map.off('click', `timm-area-layer-${item.layer}-fill`, this.onShapeSelect)
            this.map.off('contextmenu', `timm-area-layer-${item.layer}-fill`, this.onShowShapePopup)
            this.map.off('mousemove', `timm-area-layer-${item.layer}-fill`, this.onHideShapePopup)
            this.map.on('click', `timm-area-layer-${item.layer}-fill`, this.onShapeSelect)
            this.map.on('contextmenu', `timm-area-layer-${item.layer}-fill`, this.onShowShapePopup)
            this.map.on('mousemove', `timm-area-layer-${item.layer}-fill`, this.onHideShapePopup)
        })
        return Promise.resolve()
    }

    loadCampaignGeojson() {
        return axios.get(`campaign/${this.props.campaign.Id}/submap/geojson`).then((resp) => {
            return new Promise((resolve) => {
                this.setState(
                    {
                        campaignSource: resp.data,
                    },
                    () => {
                        if (this.state.mapReady == true) {
                            this.map.getSource('campaign-source').setData(resp.data)
                        } else {
                            this.initCampaignLayers()
                        }

                        return resolve()
                    }
                )
            })
        })
    }

    initCampaignLayers() {
        let testSourceExists = null
        let testLayerExist = null
        let labelLayer = this.state.mapLabelLayer

        testSourceExists = this.map.getSource('campaign-source')
        if (!testSourceExists) {
            this.map.addSource('campaign-source', { type: 'geojson', data: this.state.campaignSource })
        }

        testLayerExist = this.map.getLayer('timm-campaign-layer-fill')
        testLayerExist && this.map.removeLayer('timm-campaign-layer-fill')
        this.map.addLayer(
            {
                id: 'timm-campaign-layer-fill',
                type: 'fill',
                source: 'campaign-source',
                layout: {},
                paint: {
                    'fill-color': ['get', 'color'],
                    'fill-opacity': 0.5,
                },
            },
            labelLayer
        )

        testLayerExist = this.map.getLayer('timm-campaign-layer-line')
        testLayerExist && this.map.removeLayer('timm-campaign-layer-line')
        this.map.addLayer(
            {
                id: 'timm-campaign-layer-line',
                type: 'line',
                source: 'campaign-source',
                layout: {},
                paint: {
                    'line-color': ['get', 'color'],
                    'line-width': 2,
                },
            },
            labelLayer
        )

        testLayerExist = this.map.getLayer('timm-campaign-layer-line')
        testLayerExist && this.map.removeLayer('timm-campaign-layer-line')
        this.map.addLayer(
            {
                id: 'timm-campaign-layer-highlight',
                type: 'line',
                source: 'campaign-source',
                layout: {},
                paint: {
                    'line-color': '#000000',
                    'line-width': 6,
                },
                filter: ['==', ['get', 'sid'], ''],
            },
            labelLayer
        )

        return Promise.resolve()
    }

    restoreMapSelection() {
        // restore classfication
        const otherLayers = ['fill', 'selected', 'label']
        layers.forEach((item) => {
            item.settings.forEach((setting) => {
                let testLayer = this.map.getLayer(`timm-area-layer-${item.layer}-${setting.id}`)
                this.map.setLayoutProperty(`timm-area-layer-${item.layer}-${setting.id}`, 'visibility', this.state.activeLayers.has(item.layer) ? 'visible' : 'none')
            })
            otherLayers.forEach((name) => {
                this.map.setLayoutProperty(`timm-area-layer-${item.layer}-${name}`, 'visibility', this.state.activeLayers.has(item.layer) ? 'visible' : 'none')
            })

            let source = this.map.getSource(`area-source-${item.layer}`)
            if (this.state.activeLayers.has(item.layer)) {
                source.minzoom = 0
            } else {
                source.minzoom = 24
            }
        })

        // restore Selected Shapes
        let shapes = Array.from(this.state.selectedShapes.values())
        let addShapes = shapes.filter((i) => i.Value == true).map((i) => i.Id)
        let removeShapes = shapes.filter((i) => i.Value == false).map((i) => i.Id)

        let activeLayer = this.getActiveLayer()
        this.map.setFilter(`timm-area-layer-${activeLayer}-remove`, ['in', ['get', 'id'], ['literal', removeShapes]])
        this.map.setFilter(`timm-area-layer-${activeLayer}-selected`, ['in', ['get', 'id'], ['literal', addShapes]])

        // restore submap selected
        let submaps = this.props?.campaign?.SubMaps ?? []
        let submap = submaps.filter((i) => i.Id == this.state.selectedSubmapId)?.[0]
        if (submap) {
            let highlightColor = color(`#${submap.ColorString}`).darker(2).toString()
            this.map.setLayoutProperty('timm-campaign-layer-highlight', 'visibility', 'none')
            this.map.setFilter('timm-campaign-layer-highlight', ['==', ['get', 'sid'], submap.Id])
            this.map.setPaintProperty('timm-campaign-layer-highlight', 'line-color', highlightColor)
            this.map.setLayoutProperty('timm-campaign-layer-highlight', 'visibility', 'visible')

            layers.forEach((item) => {
                this.map.setPaintProperty(`timm-area-layer-${item.layer}-selected`, 'fill-color', color(`#${submap.ColorString}`).toString())
            })
        }
    }

    onRefresh() {
        this.loadSubmapList()
        this.loadCampaignGeojson()
        this.clearSelectedShapes()
    }

    onClassificationsChange(evt) {
        if (!this.state.mapReady) {
            return
        }
        let layerId = evt.currentTarget.id
        this.setState(
            (state) => {
                let activeLayers = new Set(state.activeLayers)
                if (activeLayers.has(layerId)) {
                    activeLayers.delete(layerId)
                } else {
                    activeLayers.add(layerId)
                }
                return {
                    activeLayers: activeLayers,
                    // selectedShapes: new Map(),
                }
            },
            () => {
                const otherLayers = ['fill', 'selected', 'label']
                layers.forEach((item) => {
                    item.settings.forEach((setting) => {
                        this.map.setLayoutProperty(`timm-area-layer-${item.layer}-${setting.id}`, 'visibility', this.state.activeLayers.has(item.layer) ? 'visible' : 'none')
                    })
                    otherLayers.forEach((name) => {
                        this.map.setLayoutProperty(`timm-area-layer-${item.layer}-${name}`, 'visibility', this.state.activeLayers.has(item.layer) ? 'visible' : 'none')
                    })

                    let source = this.map.getSource(`area-source-${item.layer}`)
                    if (this.state.activeLayers.has(item.layer)) {
                        source.minzoom = 0
                    } else {
                        source.minzoom = 24
                    }
                })
            }
        )
    }

    onSubmapSelect(submap) {
        if (!this.state.mapReady) {
            return
        }
        if (this.state.selectedSubmapId == submap.Id) {
            return
        }
        this.setState(
            (state) => {
                let allowedLayers = Array.from(Classification.values())
                let fixClassification = Classification.get(submap.SubMapRecords?.[0]?.Classification)
                if (fixClassification) {
                    allowedLayers = [fixClassification]
                }

                let submapGeo = state.campaignSource.features.filter((i) => i.id == submap.Id)?.[0]

                if (submapGeo) {
                    this.map.fitBounds([
                        [submapGeo.bbox[0], submapGeo.bbox[1]],
                        [submapGeo.bbox[2], submapGeo.bbox[3]],
                    ])
                }

                return { selectedSubmapId: submap.Id, allowedLayers: new Set(allowedLayers), selectedShapes: new Map() }
            },
            () => {
                //set selected layer fill color as submap color
                let highlightColor = color(`#${submap.ColorString}`).darker(2).toString()
                this.map.setLayoutProperty('timm-campaign-layer-highlight', 'visibility', 'none')
                this.map.setFilter('timm-campaign-layer-highlight', ['==', ['get', 'sid'], submap.Id])
                this.map.setPaintProperty('timm-campaign-layer-highlight', 'line-color', highlightColor)
                this.map.setLayoutProperty('timm-campaign-layer-highlight', 'visibility', 'visible')

                layers.forEach((item) => {
                    this.map.setFilter(`timm-area-layer-${item.layer}-remove`, ['==', ['get', 'sid'], ''])
                    this.map.setFilter(`timm-area-layer-${item.layer}-selected`, ['==', ['get', 'sid'], ''])
                    this.map.setPaintProperty(`timm-area-layer-${item.layer}-selected`, 'fill-color', color(`#${submap.ColorString}`).toString())
                })
            }
        )
    }

    getActiveLayer() {
        let allowedLayers = this.state.allowedLayers
        let activeLayers = this.state.activeLayers
        return activeLayers.has('PremiumCRoute') && allowedLayers.has('PremiumCRoute')
            ? 'PremiumCRoute'
            : activeLayers.has('Z5') && allowedLayers.has('Z5')
            ? 'Z5'
            : activeLayers.has('Z3') && allowedLayers.has('Z3')
            ? 'Z3'
            : null
    }

    onShapeSelect(e) {
        if (!this.state.selectedSubmapId) {
            return
        }

        let id = e.features?.[0]?.properties?.id
        let name = e.features?.[0]?.properties?.name

        // check is allowed layer
        let activeLayer = this.getActiveLayer()
        if (!id || !name || id.indexOf(activeLayer) == -1) {
            return
        }
        // let id = `${layer}-${name}`
        // check layer is in another submap
        let submaps = this.props?.campaign?.SubMaps ?? []
        let blockLayers = submaps
            .filter((s) => s.Id != this.state.selectedSubmapId)
            .flatMap((s) => (s.SubMapRecords ?? []).map((r) => `${Classification.get(r.Classification)}-${r.AreaId}`))
        if (new Set(blockLayers).has(id)) {
            return
        }
        let subMapRecords = submaps.filter((i) => i.Id == this.state.selectedSubmapId)?.[0]?.SubMapRecords ?? []
        let selectedShapes = subMapRecords.map((r) => `${Classification.get(r.Classification)}-${r.AreaId}`)
        let currentShapes = new Set(selectedShapes)
        this.setState(
            (state) => {
                let selectedShapes = new Map(state.selectedShapes)
                if (selectedShapes.has(id)) {
                    selectedShapes.delete(id)
                } else {
                    selectedShapes.set(id, { Classification: activeLayer, Id: id, Value: !currentShapes.has(id), Name: name })
                }

                let allowedLayers = new Set([activeLayer])
                //if submap is empty allow all layers
                if (selectedShapes.size == 0 && currentShapes.size == 0) {
                    allowedLayers = new Set(layers.map((i) => i.layer))
                }
                return {
                    selectedShapes: selectedShapes,
                    allowedLayers: allowedLayers,
                }
            },
            () => {
                let shapes = Array.from(this.state.selectedShapes.values())
                let addShapes = shapes.filter((i) => i.Value == true).map((i) => i.Id)
                let removeShapes = shapes.filter((i) => i.Value == false).map((i) => i.Id)

                this.map.setFilter(`timm-area-layer-${activeLayer}-remove`, ['in', ['get', 'id'], ['literal', removeShapes]])
                this.map.setFilter(`timm-area-layer-${activeLayer}-selected`, ['in', ['get', 'id'], ['literal', addShapes]])
            }
        )
    }

    onShowShapePopup(e) {
        let id = e.features?.[0]?.properties?.id
        let activeLayer = this.getActiveLayer()
        if (!id || id.indexOf(activeLayer) == -1) {
            return
        }

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

    loadSubmapList() {
        axios.get(`campaign/${this.props.campaign.Id}/submap`).then((resp) => {
            this.props.campaign.SubMaps = resp.data.data.SubMaps
            this.forceUpdate()
        })
    }

    onNewSubmap() {
        let colorIndex = (this.props?.campaign?.Id ?? 0) + (this.props?.campaign?.SubMaps?.length ?? 0)
        let color = Helper.randomColor(colorIndex).replace('#', '')
        let model = { ColorString: color, CampaignId: this.props.campaign.Id }
        Helper.publish('showDialog', {
            view: <SubmapEdit model={model} registeredTopic={{}} registeredEvents={[]} />,
            options: {
                size: 'tiny',
            },
        })
    }

    onEditSubmap() {
        if (!this.state.selectedSubmapId) {
            return
        }
        var submap = this.props.campaign.SubMaps.filter((i) => i.Id == this.state.selectedSubmapId)?.[0]
        Helper.publish('showDialog', {
            view: <SubmapEdit model={submap} registeredTopic={{}} registeredEvents={[]} />,
            options: {
                size: 'tiny',
            },
        })
    }

    onDeleteSubmap() {
        if (!this.state.selectedSubmapId) {
            return
        }
        var submap = this.props.campaign.SubMaps.filter((i) => i.Id == this.state.selectedSubmapId)?.[0]

        axios.delete(`campaign/${submap.CampaignId}/submap/${submap.Id}/delete`).then(() => {
            this.setState(
                {
                    activeLayers: new Set(),
                    allowedLayers: new Set(['Z3', 'Z5', 'PremiumCRoute']),
                    selectedShapes: new Map(),
                    selectedSubmapId: null,
                },
                () => {
                    layers.forEach((item) => {
                        this.map.setFilter(`timm-area-layer-${item.layer}-remove`, ['in', ['get', 'id'], ['literal', []]])
                        this.map.setFilter(`timm-area-layer-${item.layer}-selected`, ['in', ['get', 'id'], ['literal', []]])
                    })

                    this.loadSubmapList()
                    this.loadCampaignGeojson()
                    this.clearSelectedShapes()
                }
            )
        })
    }

    onSaveShapesToSubmap() {
        if (!this.state.selectedSubmapId) {
            return
        }
        let data = Array.from(this.state.selectedShapes.values()).map((i) => {
            return {
                Classification: i.Classification,
                Name: i.Name,
                Value: i.Value,
            }
        })
        let url = `campaign/${this.props.campaign.Id}/submap/${this.state.selectedSubmapId}/merge`
        axios.post(url, data).then(() => {
            this.loadSubmapList()
            this.loadCampaignGeojson()
            this.clearSelectedShapes()
        })
    }

    onEmptySubmap() {
        if (!this.state.selectedSubmapId) {
            return
        }
        let url = `campaign/${this.props.campaign.Id}/submap/${this.state.selectedSubmapId}/empty`
        axios.delete(url).then(() => {
            this.setState(
                {
                    allowedLayers: new Set(['Z3', 'Z5', 'PremiumCRoute']),
                },
                () => {
                    this.loadCampaignGeojson()
                    this.loadSubmapList()
                    this.clearSelectedShapes()
                }
            )
        })
    }

    clearSelectedShapes() {
        this.setState(
            (state) => {
                let submaps = this.props?.campaign?.SubMaps ?? []
                let subMapRecords = submaps.filter((i) => i.Id == state.selectedSubmapId)?.[0]?.SubMapRecords ?? []
                let allowedLayers = new Set(layers.map((i) => i.layer))
                if (subMapRecords.length > 0) {
                    allowedLayers = new Set([Classification.get(subMapRecords[0].Classification)])
                }
                return { selectedShapes: new Map(), allowedLayers: allowedLayers }
            },
            () => {
                layers.forEach((item) => {
                    this.map.setFilter(`timm-area-layer-${item.layer}-remove`, ['in', ['get', 'id'], ['literal', []]])
                    this.map.setFilter(`timm-area-layer-${item.layer}-selected`, ['in', ['get', 'id'], ['literal', []]])
                })
            }
        )
    }

    onSelectAllScreenShapes() {
        if (!this.state.selectedSubmapId) {
            return
        }

        let activeLayer = this.getActiveLayer()
        let features = this.map.queryRenderedFeatures({
            layers: [`timm-area-layer-${activeLayer}-fill`],
        })
        let submaps = this.props?.campaign?.SubMaps ?? []
        let blockLayers = submaps
            .filter((s) => s.Id != this.state.selectedSubmapId)
            .flatMap((s) => (s.SubMapRecords ?? []).map((r) => `${Classification.get(r.Classification)}-${r.AreaId}`))
        blockLayers = new Set(blockLayers)
        this.setState(
            (state) => {
                let selectedShapes = new Map(state.selectedShapes)
                features.forEach((f) => {
                    let id = f.properties?.id
                    let name = f.properties?.name

                    if (id.indexOf('label') > -1) {
                        return
                    }

                    if (blockLayers.has(id)) {
                        return
                    }

                    if (selectedShapes.has(id)) {
                        return
                    }

                    selectedShapes.set(id, { Classification: activeLayer, Id: id, Name: name, Value: true })
                })

                let allowedLayers = new Set([activeLayer])
                return {
                    selectedShapes: selectedShapes,
                    allowedLayers: allowedLayers,
                }
            },
            () => {
                let shapes = Array.from(this.state.selectedShapes.values())
                let addShapes = shapes.filter((i) => i.Value == true).map((i) => i.Id)
                let removeShapes = shapes.filter((i) => i.Value == false).map((i) => i.Id)

                this.map.setFilter(`timm-area-layer-${activeLayer}-remove`, ['in', ['get', 'id'], ['literal', removeShapes]])
                this.map.setFilter(`timm-area-layer-${activeLayer}-selected`, ['in', ['get', 'id'], ['literal', addShapes]])
            }
        )
    }

    onSwitchMapStyle(style) {
        if (!this.state.mapReady) {
            return
        }

        this.map.once('styledata', this.onStyleChange)
        this.setState({ mapStyle: style }, () => {
            if (style == 'streets') {
                // this.map.setLayoutProperty('google-road-tiles-layer', 'visibility', 'visible')
                // this.map.setLayoutProperty('google-satellite-tiles-layer', 'visibility', 'none')

                this.map.setStyle('mapbox://styles/mapbox/outdoors-v11')
                // for (const layer of this.map.getStyle().layers) {
                //     if (!layer.id.startsWith('timm')) {
                //         this.map.setLayoutProperty(layer.id, 'visibility', 'visible')
                //     }
                // }
                // this.map.setLayoutProperty('timm-google-satellite-tiles-layer', 'visibility', 'none')
            } else {
                // this.map.setLayoutProperty('google-road-tiles-layer', 'visibility', 'none')
                // this.map.setLayoutProperty('google-satellite-tiles-layer', 'visibility', 'visible')

                this.map.setStyle('mapbox://styles/mapbox/satellite-streets-v11')

                // for (const layer of this.map.getStyle().layers) {
                //     if (!layer.id.startsWith('timm')) {
                //         this.map.setLayoutProperty(layer.id, 'visibility', 'none')
                //     }
                // }
                // this.map.setLayoutProperty('timm-google-satellite-tiles-layer', 'visibility', 'visible')
            }
        })
    }

    onStyleChange() {
        const layers = this.map.getStyle().layers
        let labelLayer = null

        for (const layer of layers) {
            if (labelLayer == null && layer.id.indexOf('label') > -1) {
                labelLayer = layer.id
            }
        }

        this.setState({ mapLabelLayer: labelLayer }, () => {
            this.setState(
                {
                    mapLabelLayer: labelLayer,
                },
                () => {
                    return new Promise((resolve) => {
                        this.map.loadImage('images/remove_pattern.png', (err, image) => {
                            if (image) {
                                this.map.addImage('remove_pattern', image)
                                return resolve()
                            }
                        })
                    }).then(() => {
                        // this.LoadBackgroundLayer()
                        // this.initCampaignLayers()
                        // this.restoreMapSelection()
                        // return Promise.resolve()
                        return Promise.all([this.LoadBackgroundLayer(), this.initCampaignLayers()]).then(() => {
                            setTimeout(this.restoreMapSelection, 500)
                        })
                    })
                }
            )
        })
    }

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
        let submaps = this.props?.campaign?.SubMaps ?? []
        return (
            <div className="grid-y" style={{ height: '100%' }}>
                <div className="small-1 cell max-width-100 padding-horizontal-1">
                    <legend>Classifications</legend>

                    <div className="button-group stacked-for-small clear">
                        <div>
                            <input type="checkbox" name="Z3" id="Z3" onChange={this.onClassificationsChange} checked={this.state.activeLayers.has('Z3')} />
                            <label
                                htmlFor="Z3"
                                className={ClassNames('margin-right-1', { 'font-bold': this.state.allowedLayers.has('Z3'), 'text-gray': !this.state.allowedLayers.has('Z3') })}
                            >
                                3 ZIP
                            </label>
                        </div>
                        <div>
                            <input type="checkbox" name="Z5" id="Z5" onChange={this.onClassificationsChange} checked={this.state.activeLayers.has('Z5')} />
                            <label
                                htmlFor="Z5"
                                className={ClassNames('margin-right-1', {
                                    'font-bold': this.state.allowedLayers.has('Z5'),
                                    'text-gray': !this.state.allowedLayers.has('Z5'),
                                })}
                            >
                                5 ZIP
                            </label>
                        </div>
                        <div>
                            <input
                                type="checkbox"
                                name="PremiumCRoute"
                                id="PremiumCRoute"
                                onChange={this.onClassificationsChange}
                                checked={this.state.activeLayers.has('PremiumCRoute')}
                            />
                            <label
                                htmlFor="PremiumCRoute"
                                className={ClassNames('margin-right-1', {
                                    'font-bold': this.state.allowedLayers.has('PremiumCRoute'),
                                    'text-gray': !this.state.allowedLayers.has('PremiumCRoute'),
                                })}
                            >
                                CRoute
                            </label>
                        </div>
                    </div>
                </div>
                <div className="small-10 cell max-width-100 padding-top-1">
                    <div className="row small-collapse">
                        <div className="columns small-6">
                            <button className="button expanded margin-0">SubMaps</button>
                        </div>
                        <div className="columns small-6">
                            <button className="button expanded secondary margin-0">Address</button>
                        </div>
                        <div className="columns small-12">
                            <div className="button-group no-gaps clear small tiny-button-group">
                                <div className="button padding-horizontal-1 padding-vertical-0 " onClick={this.onNewSubmap}>
                                    New
                                </div>
                                <div className="button padding-horizontal-1 padding-vertical-0 " onClick={this.onEditSubmap}>
                                    Edit
                                </div>
                                <div className="button padding-horizontal-1 padding-vertical-0 " onClick={this.onDeleteSubmap}>
                                    Delete
                                </div>
                                <div className="button padding-horizontal-1 padding-vertical-0 " onClick={this.onSaveShapesToSubmap}>
                                    Save Shapes
                                </div>
                                <div className="button padding-horizontal-1 padding-vertical-0 " onClick={this.onSelectAllScreenShapes}>
                                    Select All Shapes
                                </div>
                                <div className="button padding-horizontal-1 padding-vertical-0 " onClick={this.clearSelectedShapes}>
                                    Deselect All Shapes
                                </div>
                            </div>
                        </div>
                    </div>
                    <div ref={this.onInitSubMapScrollbar} style={{ overflow: 'hidden scroll', height: '640px' }}>
                        <div className="row">
                            {submaps.map((s, index) => {
                                let boxStyle = {
                                    width: '28px',
                                    height: '28px',
                                    backgroundColor: `#${s.ColorString}`,
                                }
                                let desc = `Total: ${s.Total.toLocaleString('en-US', { minimumFractionDigits: 0 })} Count: ${s.CountAdjustment} Pen: ${s.Penetration}`
                                let buttonClassName = ClassNames('button expanded border-none padding-0 margin-0 text-left', {
                                    hollow: this.state.selectedSubmapId != s.Id,
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
                                                    <div className="margin-0 padding-0 font-medium">{`${index + 1}. ${s.Name}`}</div>
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
                            <button
                                className="mapboxgl-ctrl-icon mapboxgl-ctrl-img mapboxgl-ctrl-stree"
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
