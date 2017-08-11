const tilestrata = require('tilestrata');
const postgismvt = require('tilestrata-postgismvt');
var server = tilestrata();

server.layer('croute')
    .route('tile.mvt')
    .use(postgismvt({
        lyr: {
            table: 'public.croutes',
            geometry: 'geom',
            type: 'circle',
            srid: 4326,
            minZoom: 3,
            maxZoom: 21,
            buffer: 10,
            fields: 'gid',
            resolution: 256,
        },
        pgConfig: {
            host: 'localhost',
            user: 'postgres',
            password: 'postgress',
            database: 'timm',
            port: '5432'
        }
    })
);

server.listen(8080, function () {
    console.log('Listening on 8080...');
});

process.on('SIGTERM', function () {
    server.close(function () {
        process.exit(0);
    });
});