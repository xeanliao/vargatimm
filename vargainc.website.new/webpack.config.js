const path = require('path')

const webpack = require('webpack')
const HtmlWebpackPlugin = require('html-webpack-plugin')
const FileManagerPlugin = require('filemanager-webpack-plugin')
const ProxyAgent = require('proxy-agent')
const output = path.resolve(__dirname, '../publish/website')
const config = {
    entry: ['./src/main.js', './scss/app.scss', './scss/library.scss'],
    output: {
        filename: '[name].[contenthash].js',
        path: output,
        clean: true,
        devtoolModuleFilenameTemplate: 'file:///[absolute-resource-path]',
    },
    resolve: {
        preferRelative: true,
        modules: ['node_modules', path.resolve(__dirname, 'src')],
        extensions: ['*', '.js', '.jsx'],
        alias: {
            'foundation-datepicker$': path.resolve(__dirname, 'node_modules/foundation-datepicker/js/foundation-datepicker.js'),
        },
    },
    externals: {
        jquery: 'jQuery',
        'jquery-ui': '',
        Foundation: 'Foundation',
    },
    module: {
        rules: [
            {
                test: /\.css$/i,
                use: ['style-loader', 'css-loader'],
            },
            {
                test: /\.s[ac]ss$/i,
                use: ['style-loader', 'css-loader', 'sass-loader'],
            },
            {
                test: /\.(png|svg|jpg|jpeg|gif)$/i,
                type: 'asset/resource',
            },
            {
                test: /\.(woff|woff2|eot|ttf|otf)$/i,
                type: 'asset/resource',
            },
            {
                test: /(\.jsx$)|(views\/base.js$)/i,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        presets: ['@babel/preset-env', '@babel/preset-react'],
                        plugins: ['react-hot-loader/babel'],
                    },
                },
            },
            {
                test: /views\/.*\.js$/i,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        presets: ['@babel/preset-env', '@babel/preset-react'],
                        plugins: ['react-hot-loader/babel'],
                    },
                },
            },
        ],
    },
}

module.exports = (env, args) => {
    config.plugins = [
        new HtmlWebpackPlugin({
            title: args.mode === 'development' ? 'Development' : 'Vargainc Timm Application',
            template: 'src/index.html',
        }),
        new FileManagerPlugin({
            events: {
                onEnd: {
                    copy: [
                        {
                            source: path.resolve(__dirname, 'public', 'images'),
                            destination: path.resolve(output, 'images'),
                        },
                        {
                            source: path.resolve(__dirname, 'public', 'style'),
                            destination: path.resolve(output, 'style'),
                        },
                        {
                            source: path.resolve(__dirname, 'public', 'street.json'),
                            destination: path.resolve(output, 'street.json'),
                        },
                        { source: path.resolve(__dirname, 'public', 'login.html'), destination: path.resolve(output, 'login.html') },
                    ],
                },
            },
        }),
        new webpack.DefinePlugin({
            DEBUG: JSON.stringify(args.mode === 'development' ? true : false),
            MapboxToken: JSON.stringify('pk.eyJ1IjoiZ2hvc3R1eiIsImEiOiJjaXczc2tmc3cwMDEyMm9tb29pdDRwOXUzIn0.KPSiOO6DWTY59x1zHdvYSA'),
        }),
    ]
    switch (args.mode) {
        case 'development':
            config.devtool = 'eval-source-map'
            config.devServer = {
                contentBase: './public/',
                host: '0.0.0.0',
                port: 8090,
                serveIndex: true,
                hot: true,
                proxy: {
                    // '/api/**': {
                    //     target: 'https://timm.vargainc.com/',
                    //     pathRewrite: { '^/api': '/preview/api' },
                    //     secure: false,
                    //     changeOrigin: true,
                    // },
                    // '/api/**': {
                    //     target: 'http://ec2-18-144-35-101.us-west-1.compute.amazonaws.com/',
                    //     pathRewrite: { '^/api': '/202202/api' },
                    //     secure: false,
                    //     changeOrigin: true,
                    // },
                    '/api/**': {
                        target: 'http://localhost:8091',
                        pathRewrite: { '/api': '' },
                        agent: new ProxyAgent(),
                        changeOrigin: true,
                    },
                    '/map/street.json': {
                        target: 'https://timm.vargainc.com',
                        changeOrigin: true,
                    },
                },
            }
            break
        case 'production':
            config.optimization = {
                minimize: true,
            }
            break
        default:
            break
    }

    return config
}
