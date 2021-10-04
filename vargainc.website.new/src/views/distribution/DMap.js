// import mapboxgl from '!mapbox-gl/dist/mapbox-gl-dev'
import mapboxgl from 'mapbox-gl'
import React from 'react'
import PropTypes from 'prop-types'
import { color } from 'd3-color'
import axios from 'axios'
import Logger from 'logger.mjs'
import ClassNames from 'classnames'

import Helper from 'views/base'
import DMapEdit from './DMapEdit'

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
        this.loadMapList = this.loadMapList.bind(this)
        this.loadCampaignGeojson = this.loadCampaignGeojson.bind(this)
        this.onRefresh = this.onRefresh.bind(this)

        this.onSubmapSelect = this.onSubmapSelect.bind(this)
        this.onDMapSelect = this.onDMapSelect.bind(this)
        this.onShapeSelect = this.onShapeSelect.bind(this)

        this.onNewDMap = this.onNewDMap.bind(this)
        this.onEditDMap = this.onEditDMap.bind(this)
        this.onDeleteDMap = this.onDeleteDMap.bind(this)
        this.onSaveShapesToDMap = this.onSaveShapesToDMap.bind(this)

        this.subscribe = Helper.subscribe.bind(this)
        this.doUnsubscribe = Helper.doUnsubscribe.bind(this)

        this.subscribe('DMap.Refresh', () => {
            this.onRefresh()
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

    loadMapList() {
        axios.get(`campaign/${this.props.campaign.Id}/dmap`).then((resp) => {
            this.props.campaign.SubMaps = resp.data.data.SubMaps
            this.forceUpdate()
        })
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

        this.map.loadImage('images/remove_pattern.png', (err, image) => {
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
                this.map.addLayer(
                    {
                        id: 'area-layer-fill',
                        type: 'fill',
                        source: 'map-source',
                        layout: {},
                        paint: {
                            'fill-color': 'white',
                            'fill-opacity': 0,
                        },
                        filter: ['==', ['get', 'type'], 'area'],
                    },
                    labelLayer
                )
                this.map.addLayer(
                    {
                        id: 'area-layer-selected',
                        type: 'fill',
                        source: 'map-source',
                        layout: { visibility: 'none' },
                        paint: {
                            'fill-color': '#ffffff',
                            'fill-opacity': 0.5,
                        },
                        filter: ['==', 'id', ''],
                    },
                    labelLayer
                )
                this.map.addLayer(
                    {
                        id: 'area-layer-remove',
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

                // event
                this.map.on('click', `area-layer-fill`, this.onShapeSelect)

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

    onRefresh() {
        this.loadCampaignGeojson()
        this.loadMapList()
        this.setState({ selectedShapes: new Map() }, () => {
            this.map.setFilter(`area-layer-remove`, ['==', 'vid', ''])
            this.map.setFilter(`area-layer-selected`, ['==', 'vid', ''])
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

                    return { selectedSubmapId: subMap.Id, selectedDMapId: null }
                },
                () => {
                    //set selected layer fill color as submap color
                    this.map.setLayoutProperty('submap-layer-highlight', 'visibility', 'none')
                    this.map.setFilter('submap-layer-highlight', ['all', ['==', 'sid', subMap.Id], ['==', 'type', 'submap']])
                    this.map.setPaintProperty('submap-layer-highlight', 'line-color', `#${subMap.ColorString}`)
                    this.map.setLayoutProperty('submap-layer-highlight', 'visibility', 'visible')

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
            this.setState({ selectedDMapId: dMap.Id }, () => {
                this.map.setPaintProperty(`area-layer-selected`, 'fill-color', color(`#${dMap.ColorString}`).toString())
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
        let oid = e.features?.[0]?.properties?.oid
        let vid = e.features?.[0]?.properties?.vid
        let sid = e.features?.[0]?.properties?.sid
        let classification = e.features?.[0]?.properties?.classification
        if (sid != subMapId) {
            return
        }

        // check layer is in not in other dmap
        let submaps = this.props?.campaign?.SubMaps ?? []
        let otherDMapAreas = submaps
            .filter((s) => s.Id == subMapId)
            .flatMap((s) => s.DMaps ?? [])
            .filter((d) => d.Id != dMapId)
            .flatMap((d) => d.DMapRecords ?? [])
            .map((r) => r.AreaId)

        if (new Set(otherDMapAreas).has(oid)) {
            return
        }

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
                    selectedShapes.set(vid, { Classification: classification, Id: oid, Value: !currentDMapAreas.has(oid), vid: vid })
                }
                return {
                    selectedShapes: selectedShapes,
                }
            },
            () => {
                let shapes = Array.from(this.state.selectedShapes.values())
                let addShapes = shapes.filter((i) => i.Value == true).map((i) => i.vid)
                let removeShapes = shapes.filter((i) => i.Value == false).map((i) => i.vid)

                this.map.setLayoutProperty(`area-layer-remove`, 'visibility', 'none')
                this.map.setLayoutProperty(`area-layer-selected`, 'visibility', 'none')
                this.map.setFilter(`area-layer-remove`, ['in', ['get', 'vid'], ['literal', removeShapes]])
                this.map.setFilter(`area-layer-selected`, ['in', ['get', 'vid'], ['literal', addShapes]])
                this.map.setLayoutProperty(`area-layer-remove`, 'visibility', 'visible')
                this.map.setLayoutProperty(`area-layer-selected`, 'visibility', 'visible')
            }
        )
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
        axios.post(url, data).then((resp) => {
            this.onRefresh()
        })
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
                                let subMapDesc = `Total: ${s.Total.toLocaleString('en-US', { minimumFractionDigits: 0 })} Count: ${s.CountAdjustment} Pen: ${s.Penetration}`
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
                                    let dMapDesc = `Total: ${d.Total.toLocaleString('en-US', { minimumFractionDigits: 0 })} Count: ${d.TotalAdjustment} Pen: ${d.Penetration}`
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
