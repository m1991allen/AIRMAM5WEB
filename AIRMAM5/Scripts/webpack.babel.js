/*
代碼優化 使用UglifyJsPlugin做了js代碼的壓縮. 類似release
*/
const merge = require('webpack-merge');
const webpack = require('webpack');
const config = require('./webpack.config.js');

config.output.filename = '[name].min.js';
module.exports = merge(config, {
    module: {
        rules: [
            {
                test: /\.js$/,
                loader: 'babel-loader',
                exclude: /node_modules/,
                options: {
                    plugins: ['@babel/proposal-class-properties', '@babel/proposal-object-rest-spread'],
                    presets: [
                        [
                            '@babel/preset-env',
                            {
                                targets: {
                                    edge: '17',
                                    firefox: '60',
                                    chrome: '67',
                                    safari: '11.1',
                                    ie: '11',
                                },
                                useBuiltIns: 'usage',
                            },
                        ],
                    ],
                },
            },
        ],
    },
});
