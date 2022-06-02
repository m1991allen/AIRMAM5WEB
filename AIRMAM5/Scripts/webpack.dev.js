/* 
開發配置的 方便調試.類似debug
*/
const merge =require('webpack-merge');
const config=require('./webpack.config.js');
module.exports=merge(config,{
    devtool:'inline-source-map',
    //sourceMap:true
});