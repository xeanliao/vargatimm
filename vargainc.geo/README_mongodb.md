1. conversion
ogr2ogr -f GeoJSON -t_srs crs:84 final.json Varga_Final_Q1_2017.shp -lco ENCODING=UTF-8
2. fix geojson file to array
```json
{
  "type": "FeatureCollection",
  "crs": {
    "type": "name",
    "properties": {
      "name": "urn:ogc:def:crs:OGC:1.3:CRS84"
    }
  },
  "features": [...]
}
```
=>
```json
[...]
```
3. import to mongodb
```shell
mongoimport --drop -d timm -c geolocation --file final.json --jsonArray --verbose --numInsertionWorkers 5
```
*workers is very important for import performance for large json file
then you will see something like these
```shell
2017-05-21T20:18:48.901+0800    filesize: 775240413 bytes
2017-05-21T20:18:48.902+0800    using fields: 
2017-05-21T20:18:48.906+0800    connected to: localhost
2017-05-21T20:18:48.906+0800    ns: timm.geolocation
2017-05-21T20:18:48.906+0800    connected to node type: standalone
2017-05-21T20:18:48.906+0800    using write concern: w='1', j=false, fsync=false, wtimeout=0
2017-05-21T20:18:48.906+0800    dropping: timm.geolocation
2017-05-21T20:18:48.907+0800    using write concern: w='1', j=false, fsync=false, wtimeout=0
2017-05-21T20:18:48.907+0800    using write concern: w='1', j=false, fsync=false, wtimeout=0
2017-05-21T20:18:48.907+0800    using write concern: w='1', j=false, fsync=false, wtimeout=0
2017-05-21T20:18:48.907+0800    using write concern: w='1', j=false, fsync=false, wtimeout=0
2017-05-21T20:18:48.907+0800    using write concern: w='1', j=false, fsync=false, wtimeout=0
2017-05-21T20:18:51.903+0800    [#.......................] timm.geolocation 45.6MB/739MB (6.2%)
2017-05-21T20:18:54.905+0800    [##......................] timm.geolocation 91.5MB/739MB (12.4%)
2017-05-21T20:18:57.903+0800    [####....................] timm.geolocation 134MB/739MB (18.2%)
2017-05-21T20:19:00.904+0800    [#####...................] timm.geolocation 177MB/739MB (24.0%)
2017-05-21T20:19:03.906+0800    [#######.................] timm.geolocation 219MB/739MB (29.6%)
2017-05-21T20:19:06.907+0800    [########................] timm.geolocation 265MB/739MB (35.8%)
2017-05-21T20:19:09.903+0800    [##########..............] timm.geolocation 308MB/739MB (41.7%)
2017-05-21T20:19:12.902+0800    [###########.............] timm.geolocation 351MB/739MB (47.5%)
2017-05-21T20:19:15.905+0800    [############............] timm.geolocation 395MB/739MB (53.4%)
2017-05-21T20:19:18.902+0800    [##############..........] timm.geolocation 437MB/739MB (59.1%)
2017-05-21T20:19:21.904+0800    [###############.........] timm.geolocation 480MB/739MB (64.9%)
2017-05-21T20:19:24.903+0800    [#################.......] timm.geolocation 524MB/739MB (70.9%)
2017-05-21T20:19:27.902+0800    [##################......] timm.geolocation 567MB/739MB (76.7%)
2017-05-21T20:19:30.902+0800    [###################.....] timm.geolocation 610MB/739MB (82.6%)
2017-05-21T20:19:33.902+0800    [#####################...] timm.geolocation 653MB/739MB (88.3%)
2017-05-21T20:19:36.902+0800    [######################..] timm.geolocation 697MB/739MB (94.3%)
2017-05-21T20:19:39.907+0800    [#######################.] timm.geolocation 738MB/739MB (99.8%)
2017-05-21T20:19:40.036+0800    [########################] timm.geolocation 739MB/739MB (100.0%)
2017-05-21T20:19:40.036+0800    imported 335442 documents
```
4. add index to filed
```mongo
db.geolocation.createIndex({geometry: '2dsphere'})
```
this command will take a while to create index. then you can use this command to check index
```mongo
db.geolocation.getIndexes()
```
then you will get something liek this
```json
[
  {
    "v": 1,
    "key": {
      "_id": 1
    },
    "name": "_id_",
    "ns": "timm.geolocation"
  },
  {
    "v": 1,
    "key": {
      "geometry": "2dsphere"
    },
    "name": "geometry_2dsphere",
    "ns": "timm.geolocation",
    "2dsphereIndexVersion": 3
  }
]
```
now you can use mongodb geo query to test performance
```mongo
db.geolocation.count( { "geometry" : { $near: {$geometry: {type: "Point" , coordinates: [ -87.6500523, 41.850033 ]}, $maxDistance: 100, $minDistance: 0 } } } ) 
```
This query is to get how many 5zip or croute distance less than 100 meters to center latlng [ -87.6500523, 41.850033 ]. hmm very fast

others
[esri open source (well know as AricGIS) tile server](https://koopjs.github.io/)
[mapbox tile server a bit complex](https://github.com/gravitystorm/tm2)
[how to add 3rd tile server to mapbox](https://www.mapbox.com/mapbox-gl-js/example/third-party/)
[mapzen vector tile server](https://github.com/tilezen/vector-datasource/wiki/Mapzen-Vector-Tile-Service)