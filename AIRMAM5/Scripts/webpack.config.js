/*
    Notice曾發生過的問題解決:https://github.com/DefinitelyTyped/DefinitelyTyped/issues/23649
    HashedModuleIdsPlugin: 幫助創建穩定的模塊ID（與Webpack 1和2兼容）
    CleanWebpackPlugin:用於刪除/清理構建文件夾
    HtmlWebpackPlugin:簡化HTML文件的創建,生成HTML內容
    SplitChunksPlugin:用來避免重複的依賴,將綑綁包切割
    Babel 版本 < v7.4.0:建議使用npm install @babel/polyfill/Babel 版本 >= v7.4.0：npm install core-js regenerator-runtime/runtime
*/
const path = require('path');
const HashedModuleIdsPlugin = require('webpack-hashed-module-id-plugin');
const {
    CleanWebpackPlugin
} = require('clean-webpack-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const glob = require('glob');

const webpack = require('webpack');
const JsTemplate = '../Views/Bundle/_JsTemplate.cshtml'; /*JS樣板路徑*/
const CssTemplate = '../Views/Bundle/_CssTemplate.cshtml'; /*CSS樣板路徑*/
/**
 * @class setPartialView
 * @description 動態生成 Partial View，並引入該頁相關 Bundle 檔案
 * @returns Array<HtmlWebpackPlugin>
 */
var setPartialView = function() {
    const plugins=[];
    const TargetPage = glob.sync(path.resolve(__dirname, './dist/*.js'));
    const GetPath = FileName => path.resolve(__dirname, `../Views/Bundle/${FileName}.cshtml`);
    TargetPage.forEach(filePath => {
        var BundleName = filePath.split('/')[filePath.split('/').length - 1].split('.')[0];
        if (/^vendor/i.test(BundleName) || /^runtime/i.test(BundleName)){
            return false;
        }
        const JsFileName = GetPath(BundleName + '_Js');
        // const CssFileName = GetPath(BundleName + '_Css');
        plugins.push(
            new HtmlWebpackPlugin({
                minify: false,
                template: JsTemplate,
                filename: JsFileName,
               chunks: [BundleName],
               inject: true,
            //    excludeChunks:function (params) {
            //     const chucks=[];
            //     params.htmlWebpackPlugin.files.js.map(
            //         file => {
            //           const [filename, hash] = file.split('?');
            //           const modifiedFileName = filename.replace('.min.js',  `${hash}.min.js`).replace('.js',`${hash}.js`);
            //            chucks.push(modifiedFileName);
            //     });
            //         return BundleName==='Layout'?[]:chucks.filter(chuckname=>/^vendorCommon/i.test(chuckname));
            //   },
            })
        );

    });
    return plugins;
};
const config = {
    entry: {
        // 'babel-polyfill': ['babel-polyfill'],
        /*入口文件 及要編譯的文件，可以配置多個*/
        Layout: './Views/Shared/Layout.ts',
        BlankLayout: './Views/Shared/BlankLayout.ts',
        AnnMatain: './Views/Ann/Matain.ts',
        LLoginIndex: './Views/L_Login/Index.ts',
        SynonymIndex: './Views/Synonym/Index.ts',
        LSearchIndex: './Views/L_Search/Index.ts',
        LLogIndex: './Views/L_Log/Index.ts',
        LTranIndex: './Views/L_Tran/Index.ts',
        LUploadIndex: './Views/L_Upload/Index.ts',
        GroupIndex: './Views/Group/Index.ts',
        UserIndex: './Views/User/Index.ts',
        SysCodeIndex: './Views/SysCode/Index.ts',
        UserCodeIndex: './Views/UserCode/Index.ts',
        ColTemplateIndex: './Views/ColTemplate/Index.ts',
        AccountResetPassword: './Views/Account/ResetPassword.ts',
        DeleteIndex: './Views/Delete/Index.ts',
        DirIndex: './Views/Dir/Index.ts',
        ArePreIndex: './Views/ArcPre/Index.ts',
        SubjectIndex: './Views/Subject/Index.ts',
        SubjectTreePlugin:'./Views/Subject/TreePlugin.ts',
        SubjectFormPlugin:'./Views/Subject/FormPlugin.ts',
        SuhjectDocSystem:'./Views/Subject/DocSystem.ts',
        SearchIndex: './Views/Search/Index.ts',
        MateriaIndex: './Views/Materia/Index.ts',
        MyBookingIndex: './Views/MyBooking/Index.ts',
        BookingIndex: './Views/Booking/Index.ts',
        OnlineSignalR: './Views/Shared/OnlineSignalR.ts',
        ReportIndex: './Views/Report/Index.ts',
        VerifyBookingIndex: './Views/VerifyBooking/Index.ts',
        DocumentViewerIndex: './Views/DocumentViewer/Index.ts',
        RuleIndex: './Views/Rule/Index.ts',
        HomeDashBoard: './Views/Home/DashBoard.ts',
        ArchiveMoveIndex: './Views/ArchiveMove/Index.ts',
        TsmCheckIn: './Views/Tsm/CheckIn.ts',
        TsmCheckOut: './Views/Tsm/CheckOut.ts',
        BatchBookingIndex: './Views/BatchBooking/Index.ts',
        LicenseIndex: './Views/License/Index.ts'
    },
    output: {
        /*輸出至dist */
        //  filename: '[name].js',
        publicPath: '../Scripts/dist',
        filename: '[name].[contenthash].js',
        path: path.resolve(__dirname, './dist'),
    },
    resolve: {
        alias: {
            // "@Class":path.resolve(__dirname,'./Models/Class'),
            // "@Enum":path.resolve(__dirname,'./Models/Enum'),
            // "@Function":path.resolve(__dirname,'./Models/Function'),
            // "@Interface":path.resolve(__dirname,'./Models/Interface'),
            // "@Const":path.resolve(__dirname,'./Models/Const'),
            // "@Controller":path.resolve(__dirname,'./Models/Controller'),
        },
        extensions: ['.ts', '.js', '.json'],
    },
    plugins: [
        new webpack.HashedModuleIdsPlugin(),
        new CleanWebpackPlugin({
            cleanAfterEveryBuildPatterns: ['dist'],
            /*訊息在 terminal 上列出*/
            verbose: true,
        }),
        new webpack.DefinePlugin({
            'process.env.NODE_ENV': JSON.stringify(process.env.NODE_ENV),
        }),
    ].concat(setPartialView()),
    module: {
        rules: [{
                test: /\.css$/,
                use: ['style-loader', 'css-loader'],
            },
            {
                test: /\.(png|svg|jpg|gif)$/,
                use: ['file-loader'],
            },
            {
                test: /\.ts$/,
                loader: 'ts-loader',
                exclude: /node_modules/,
            },
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
                                corejs: 3,
                            },
                            '@babel/transform-runtime',
                            {
                                corejs: 3,
                            },
                        ],
                    ],
                },
            },
        ],
    },
    optimization: {
        // runtimeChunk: {
        //     name: entrypoint => `runtime~${entrypoint.name}`,
        // },
        splitChunks: {
            cacheGroups: {
                // 抽離 node_modules和各頁面使用的元件
                // vendorBabelPollyfill: {
                //     enforce: true,
                //     chunks: 'initial',
                //     test: /[\\/]node_modules[\\/](babel-polyfill|)[\\/]/,
                //     name: 'vendorBabelPollyfill'
                // },
                default: false,
                vendorCommon: {
                  test: /[\\/]Views[\\/]Shared[\\/]/,
                   name: 'vendorCommon',
                    enforce: true,
                    reuseExistingChunk:true,
                },
                vendorInspiretree: {
                    enforce: true,
                    chunks: 'all',
                    test: /[\\/]node_modules[\\/](lodash|inspire-tree|inspire-tree-dom)[\\/]/,
                    name: 'vendorInspiretree',
                },
                vendorTime: {
                    enforce: true,
                    chunks: 'all',
                    test: /[\\/]node_modules[\\/](dayjs|)[\\/]/,
                    name: 'vendorTime',
                },
                vendor: {
                    enforce: true,
                    chunks: 'all',
                    test: /[\\/]node_modules[\\/](!inspire-tree)(!inspire-tree-dom)(!lodash)(!dayjs)[\\/]/,
                    name(module) {
                        const packageName = module.context.match(/[\\/]node_modules[\\/](.*?)([\\/]|$)/)[1];
                        return `vendor.${packageName.replace('@', '')}`;
                    },
                },
                //     vendors: {
                //         test: /[\\/]node_modules[\\/]/,
                //         // chunks: 'initial',
                //         //name: 'vendors',
                //         // priority: 20,
                //         // enforce: true,
                //         name(module) {
                //             // get the name. E.g. node_modules/packageName/not/this/part.js
                //             // or node_modules/packageName
                //             const packageName = module.context.match(/[\\/]node_modules[\\/](.*?)([\\/]|$)/)[1];

                //             // npm package names are URL-safe, but some servers don't like @ symbols
                //             return `npm.${packageName.replace('@', '')}`;
                //         },
                //     },
            },
        },
    },
};


module.exports = config;