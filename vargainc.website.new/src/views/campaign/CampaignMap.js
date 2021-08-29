// import mapboxgl from '!mapbox-gl/dist/mapbox-gl-dev'
import mapboxgl from 'mapbox-gl'
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

const layers = [
    { layer: 'Z3', zoom: [7, 18, 7], color: '#323232' },
    { layer: 'Z5', zoom: [9, 18, 11], color: '#00FF00' },
    { layer: 'PremiumCRoute', zoom: [11, 18, 14], color: '#00FF00' },
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
            activeLayer: null,
            allowedLayer: new Set(['Z3', 'Z5', 'PremiumCRoute']),
            selectedShapes: new Map(),
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
        this.onShapeSelect = this.onShapeSelect.bind(this)

        this.onLoadSubmapList = this.onLoadSubmapList.bind(this)
        this.onNewSubmap = this.onNewSubmap.bind(this)
        this.onEditSubmap = this.onEditSubmap.bind(this)
        this.onSaveShapesToSubmap = this.onSaveShapesToSubmap.bind(this)
        this.onEmptySubmap = this.onEmptySubmap.bind(this)
        this.onClearSelectedShapes = this.onClearSelectedShapes.bind(this)

        this.map = null

        this.subscribe = Helper.subscribe.bind(this)
        this.doUnsubscribe = Helper.doUnsubscribe.bind(this)

        this.subscribe('CampaignMap.Refresh', this.onLoadSubmapList)
    }

    componentWillUnmount() {
        this.doUnsubscribe()
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

        this.setState(
            {
                mapTextLayerId: firstSymbolId,
            },
            () => {
                this.initBackgroundLayer()
                this.initCampaignLayer()
                this.initPopup()
            }
        )
    }

    initBackgroundLayer() {
        let textLayerId = this.state.mapTextLayerId
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
                textLayerId
            )
            let selectedColor = color(`hsl(156, 20%, 95%)`).darker(1)
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
                        'fill-color': selectedColor.formatHsl(),
                        'fill-opacity': 0.3,
                    },
                    filter: ['==', 'id', ''],
                },
                textLayerId
            )
            let highlight = color(`hsl(156, 20%, 95%)`).darker(2)
            this.map.addLayer(
                {
                    id: `area-layer-${item.layer}-highlight`,
                    type: 'fill',
                    source: `area-source-${item.layer}`,
                    'source-layer': item.layer,
                    minzoom: item.zoom[0],
                    maxzoom: item.zoom[1],
                    layout: {},
                    paint: {
                        'fill-color': highlight.formatHsl(),
                        'fill-opacity': 0.75,
                    },
                    filter: ['==', 'name', ''],
                },
                textLayerId
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
                textLayerId
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
                textLayerId
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

            this.map.on('click', `area-layer-${item.layer}-fill`, this.onShapeSelect)
        })
    }

    initCampaignLayer() {
        let textLayerId = this.state.mapTextLayerId
        axios.get(`campaign/${this.props.campaign.Id}/submap/geojson`).then((resp) => {
            if (this.state.mapReady == true) {
                this.map.getSource('campaign-source').setData(resp.data)
            } else {
                this.map.addSource('campaign-source', { type: 'geojson', data: resp.data })
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
                    textLayerId
                )
                this.map.addLayer(
                    {
                        id: 'campaign-layer-line',
                        type: 'line',
                        source: 'campaign-source',
                        layout: {},
                        paint: {
                            'line-color': ['get', 'color'],
                            'line-width': 2,
                        },
                    },
                    textLayerId
                )
                this.map.addLayer(
                    {
                        id: 'campaign-layer-highlight',
                        type: 'line',
                        source: 'campaign-source',
                        layout: {},
                        paint: {
                            'line-color': '#000000',
                            'line-width': 6,
                        },
                        filter: ['==', ['get', 'sid'], ''],
                    },
                    textLayerId
                )
            }
            this.setState({
                campaginSource: resp.data,
                mapReady: true,
            })
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
                const type = ['fill', 'highlight', 'line-bg', 'line', 'label']
                layers.forEach((item) => {
                    type.forEach((type) => {
                        this.map.setLayoutProperty(`area-layer-${item.layer}-${type}`, 'visibility', item.layer == activeLayer ? 'visible' : 'none')
                    })
                    this.map.setFilter(`area-layer-${item.layer}-highlight`, ['==', 'name', ''])
                })

                if (activeLayer) {
                    let minZoom = layers.filter((i) => i.layer == activeLayer)?.[0]?.zoom?.[2]
                    if (this.map.getZoom() < minZoom) {
                        // this.map.setZoom(minZoom)
                    }
                }
            }
        )
    }

    onSubmapSelect(submap) {
        if (!this.state.mapReady) {
            return
        }
        let needTrigerClassificationsChange
        this.setState(
            (state) => {
                let allowedLayer = Array.from(Classification.values())
                let fixClassification = Classification.get(submap.SubMapRecords?.[0]?.Classification)
                if (fixClassification) {
                    allowedLayer = [fixClassification]
                }

                let selectedShapes = new Map(state.selectedShapes)

                if (state.selectedSubmap != submap.Id) {
                    selectedShapes = new Map()

                    needTrigerClassificationsChange = true

                    let highlightColor = color(`#${submap.ColorString}`).darker(2).toString()
                    this.map.setLayoutProperty('campaign-layer-highlight', 'visibility', 'none')
                    this.map.setFilter('campaign-layer-highlight', ['==', ['get', 'sid'], submap.Id])
                    this.map.setPaintProperty('campaign-layer-highlight', 'line-color', highlightColor)
                    this.map.setLayoutProperty('campaign-layer-highlight', 'visibility', 'visible')

                    let submapGeo = this.state.campaginSource.features.filter((i) => i.id == submap.Id)?.[0]

                    if (submapGeo) {
                        this.map.fitBounds([
                            [submapGeo.bbox[0], submapGeo.bbox[1]],
                            [submapGeo.bbox[2], submapGeo.bbox[3]],
                        ])
                    }
                }

                return { selectedSubmap: submap.Id, allowedLayer: new Set(allowedLayer), selectedShapes: selectedShapes }
            },
            () => {
                if (needTrigerClassificationsChange && !this.state.allowedLayer.has(this.state.activeLayer)) {
                    this.onClassificationsChange({ currentTarget: { id: Array.from(this.state.allowedLayer.values())[0] } })
                }

                //set selected layer fill color as submap color
                layers.forEach((item) => {
                    this.map.setPaintProperty(`area-layer-${item.layer}-selected`, 'fill-color', `#${submap.ColorString}`)
                    this.map.setFilter(`area-layer-${item.layer}-selected`, ['in', ['get', 'id'], ['literal', Array.from(this.state.selectedShapes.keys())]])
                })
            }
        )
    }

    onShapeSelect(e) {
        let id = e.features?.[0]?.properties?.id
        let name = e.features?.[0]?.properties?.name
        let layer = this.state.activeLayer
        if (!id || !layer) {
            return
        }
        // let id = `${layer}-${name}`
        // check layer is in another submap
        let submaps = this.props?.campaign?.SubMaps ?? []
        let blockLayers = submaps
            .filter((s) => s.Id != this.state.selectedSubmap)
            .flatMap((s) => (s.SubMapRecords ?? []).map((r) => `${Classification.get(r.Classification)}-${r.AreaId}`))
        if (new Set(blockLayers).has(id)) {
            return
        }
        let currentShapes = new Set(
            (submaps.filter((i) => i.Id == this.state.selectedSubmap)?.[0]?.SubMapRecords ?? []).map((r) => `${Classification.get(r.Classification)}-${r.AreaId}`)
        )
        this.setState(
            (state) => {
                let selectedShapes = new Map(state.selectedShapes)
                if (selectedShapes.has(id)) {
                    selectedShapes.delete(id)
                } else {
                    selectedShapes.set(id, { Classification: layer, Id: id, Value: !currentShapes.has(id), Name: name })
                }
                return {
                    selectedShapes: selectedShapes,
                }
            },
            () => {
                let selectedShapes = Array.from(this.state.selectedShapes.keys())
                this.map.setFilter(`area-layer-${layer}-selected`, ['in', ['get', 'id'], ['literal', selectedShapes]])
            }
        )
    }

    onLoadSubmapList() {
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
        if (!this.state.selectedSubmap) {
            return
        }
        var submap = this.props.campaign.SubMaps.filter((i) => i.Id == this.state.selectedSubmap)?.[0]
        Helper.publish('showDialog', {
            view: <SubmapEdit model={submap} registeredTopic={{}} registeredEvents={[]} />,
            options: {
                size: 'tiny',
            },
        })
    }

    onSaveShapesToSubmap() {
        if (!this.state.selectedSubmap) {
            return
        }
        let data = Array.from(this.state.selectedShapes.values()).map((i) => {
            return {
                Classification: i.Classification,
                Name: i.Name,
                Value: i.Value,
            }
        })
        let url = `campaign/${this.props.campaign.Id}/submap/${this.state.selectedSubmap}/merge`
        axios.post(url, data).then((resp) => {
            this.onLoadSubmapList()
            this.initCampaignLayer()
            this.onClearSelectedShapes()
        })
    }

    onEmptySubmap() {
        if (!this.state.selectedSubmap) {
            return
        }
        let url = `campaign/${this.props.campaign.Id}/submap/${this.state.selectedSubmap}`
        axios.delete(url).then(() => {
            this.initCampaignLayer()
        })
    }

    onClearSelectedShapes() {
        let layer = this.state.activeLayer
        this.setState({ selectedShapes: new Map() }, () => {
            this.map.setFilter(`area-layer-${layer}-selected`, ['==', 'id', ''])
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
                    <div className="grid-x grid-margin-x margin-collapse small-up-3">
                        <div className="cell">
                            <input type="radio" name="mapType" id="none" onChange={this.onClassificationsChange} checked={this.state.activeLayer == null} />
                            <label htmlFor="none">None</label>
                        </div>
                        <div className="cell">
                            <input
                                type="radio"
                                name="mapType"
                                id="Z3"
                                onChange={this.onClassificationsChange}
                                disabled={!this.state.allowedLayer.has('Z3')}
                                checked={this.state.activeLayer == 'Z3'}
                            />
                            <label htmlFor="Z3">3 ZIP</label>
                        </div>
                        <div className="cell">
                            <input
                                type="radio"
                                name="mapType"
                                id="Z5"
                                onChange={this.onClassificationsChange}
                                disabled={!this.state.allowedLayer.has('Z5')}
                                checked={this.state.activeLayer == 'Z5'}
                            />
                            <label htmlFor="Z5">5 ZIP</label>
                        </div>
                        <div className="cell auto">
                            <input
                                type="radio"
                                name="mapType"
                                id="PremiumCRoute"
                                onChange={this.onClassificationsChange}
                                disabled={!this.state.allowedLayer.has('PremiumCRoute')}
                                checked={this.state.activeLayer == 'PremiumCRoute'}
                            />
                            <label htmlFor="PremiumCRoute">CRoute</label>
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
                            <div className="button-group no-gaps clear small">
                                <div className="button" onClick={this.onNewSubmap}>
                                    New
                                </div>
                                <div className="button" onClick={this.onEditSubmap}>
                                    Edit
                                </div>
                                <div className="button">Delete</div>
                                <div className="button" onClick={this.onSaveShapesToSubmap}>
                                    Save Shapes
                                </div>
                                <div className="button" onClick={this.onEmptySubmap}>
                                    Remove All Shapes
                                </div>
                                <div className="button" onClick={this.onClearSelectedShapes}>
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
