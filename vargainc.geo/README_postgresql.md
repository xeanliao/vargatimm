1. make sure have extend postgresql with postgis
```sql
CREATE EXTENSION postgis;
```
then you can check the postgis version
```sql
SELECT postgis_full_version();
```

2. import shape file to postsql
```shell
shp2pgsql -I -s 4326 Varga_Final_Q1_2017.shp geolocation | psql -U postgres -d timm
```

3. setup tiles server
use [tilestrata](https://github.com/naturalatlas/tilestrata) base [tilestrata-postgismvt
](https://github.com/Stezii/tilestrata-postgismvt) plugin as our croutes tiles server

3.1 we need extend postgis sql functions by [postgis-vt-util](https://github.com/mapbox/postgis-vt-util)