/*
代碼優化使用 
* UglifyJsPlugin做了js代碼的壓縮. 類似release
*/
const merge = require('webpack-merge');
const webpack = require("webpack");
const config = require('./webpack.config.js');
const path = require('path');
const UglifyJsPlugin = require('uglifyjs-webpack-plugin');


/* 
 optimization定義參考  https://medium.com/webpack/webpack-4-mode-and-optimization-5423a6bc597a
 UglifyJsPlugin定義參考 https://webpack.js.org/plugins/uglifyjs-webpack-plugin/
*/

module.exports = merge(config, {
  output: {
    filename: '[name].[contenthash].min.js',
  },
  optimization: {
    minimizer: [
      new UglifyJsPlugin({
        uglifyOptions: {
          cache: false,
          /* 啟用文件緩存*/
          ie8: true,
          /*啟用IE8支持 */
          parallel: true,
          /* truem或數字,使用多進程並行運行來提高構建速度。默認並發運行數：os.cpus().length - 1。*/
          UglifyJsPlugin: {
            compress: false,
            /*其他壓縮選項*/
            ecma: 5,
            /*支持的ECMAScript的版本（5，6，7或8）。影響parse，compress&& output選項 */
            mangle: true,
            /*啟用名稱管理（有關高級設置https://github.com/mishoo/UglifyJS2/tree/harmony#mangle-properties-options */
            extractComments: false,
            /*啟用/禁用提取註釋 */
          },
          output: {
            comments: false,
            beautify: false
          },
          warnings: false,
          sourceMap: true /*使用源映射將錯誤消息位置映射到模塊（這會減慢編譯速度）。如果您使用自己的minify功能，請閱讀minify正確處理源地圖的部分。 */
        }

      })
    ],
  },
  plugins: [

  ]
});