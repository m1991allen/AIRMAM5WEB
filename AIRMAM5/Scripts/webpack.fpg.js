const merge = require('webpack-merge');
const config = require('./webpack.prod.js');
const webpack = require("webpack");

module.exports = merge(config, {
  // output: {
  //   filename: '[name].[contenthash].fpg.min.js',
  // },
});