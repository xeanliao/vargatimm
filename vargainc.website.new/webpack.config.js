const path = require('path')

const webpack = require('webpack')
const HtmlWebpackPlugin = require('html-webpack-plugin')
const CopyWebpackPlugin = require('copy-webpack-plugin')
const ProxyAgent = require('proxy-agent')

module.exports = {
    entry: ['./src/main.js', './scss/app.scss', './scss/library.scss'],
    output: {
        filename: '[name].[contenthash].js',
        path: path.resolve(__dirname, 'wwwroot'),
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
    },
    devtool: 'eval-source-map',
    devServer: {
        contentBase: './wwwroot',
        host: '0.0.0.0',
        port: 8090,
        serveIndex: true,
        hot: true,
        proxy: {
            // '/api/**': {
            //     target: 'http://ec2-52-8-230-211.us-west-1.compute.amazonaws.com/',
            //     pathRewrite: { '^/api': '/201907/api' },
            // },
            '/api/**': {
                target: 'http://localhost:8091',
                pathRewrite: { '^/api': '' },
                agent: new ProxyAgent(),
                changeOrigin: true,
            },
            '/map/street.json': {
                target: 'https://timm.vargainc.com',
                changeOrigin: true,
            },
        },
    },
    plugins: [
        new HtmlWebpackPlugin({
            title: 'Development',
            template: 'src/index.html',
        }),
        new CopyWebpackPlugin({
            patterns: [
                { from: 'images', to: 'images' },
                { from: 'style', to: 'style' },
                { from: 'src/login.html', to: '' },
                { from: 'street.json', to: '' },
            ],
        }),
        new webpack.DefinePlugin({
            DEBUG: JSON.stringify(true),
            RELEASE_VERSION: 'preview',
            MapboxToken: JSON.stringify('pk.eyJ1IjoiZ2hvc3R1eiIsImEiOiJjaXczc2tmc3cwMDEyMm9tb29pdDRwOXUzIn0.KPSiOO6DWTY59x1zHdvYSA'),
        }),
    ],
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
