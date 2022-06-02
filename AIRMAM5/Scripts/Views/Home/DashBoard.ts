import { AjaxGet } from '../../Models/Function/Ajax';
import { API } from '../../Models/Const/API';
import { UI } from '../../Models/Templete/CompoentTemp';
import { HotkeyModel } from '../../Models/Interface/Home/HotkeyModel';
import { ChartModel, ChartBarModel, ChartDataset, BranchBooking } from '../../Models/Interface/Home/ChartModel';
import { HexToRGBA } from '../../Models/Function/Color';
import { GetDropdown } from '../../Models/Function/Element';
import { Logger } from '../../Models/Class/LoggerService';
import { GaugeWorkModel } from '../../Models/Interface/Home/GaugeViewModel';
import { gaugeChartType } from '../../Views/Shared/_windowParameter';
import { IsNULLorEmpty, IsNullorUndefined } from '../../Models/Function/Check';
import { DashBoardSetting } from '../../Models/initSetting';

const $topHotKey: JQuery<HTMLElement> = $('#topHotKey');
const $TableHotKey: JQuery<HTMLElement> = $('#ChangeHotKey');
const $HotKeyDropdown: JQuery<HTMLElement> = GetDropdown('#hotkeyscard', 'hotkey');
var stackStatus:boolean=true;
/**熱門關鍵字初始化設定 */
const hotkeyInitSetting = {
    days: 7,
    top: 10,
};
/**圖表顏色 */
const chartColors = {
    red: '#ff6384',
    orange: '#ff9f40',
    yellow: '#ffcd56',
    green: '#4bc0c0',
    blue: '#36a2eb',
    purple: '#9966ff',
    grey: '#e7e9ed',
    white: '#ffffff',
};

/**用自定義資料,填充陣列的資料數 */
const arrayPush=<T>(array:Array<T>, digital:number,supplementValue:T)=>{
    let requireLength=Math.abs(digital - array.length);
    for(let i=0;i<requireLength;i++){   array.push(supplementValue); }
    return array;
};

/**
 * 取得熱門關鍵字
 * @param days 查詢區間(幾天內)
 * @param top (前幾熱門)
 */
const HotKeyTask = (days: number, top: number) => {
    days = days <= 0 ? hotkeyInitSetting.days : days; //最少1天
    top = top <= 0 ? hotkeyInitSetting.top : top; //最少1名
    AjaxGet<{ d: number; t: number }>(API.Home.BoardHotKey, { d: days, t: top }, true)
        .then(res => {
            const hotkeys = <Array<HotkeyModel>>res.Data;
            const getTRS = ((): string => {
                let trs: string = '';
                if (hotkeys.length > 0) {
                    for (let hotkey of hotkeys) {
                        trs += `<tr><td>${hotkey.word} </td> <td>${hotkey.Counts}</td><td>${hotkey.LastTime}</td> </tr>`;
                    }
                } else {
                    trs = `<tr class="center aligned"><td colspan="3"><i class="icon pin"></i>暫時沒有資料</td></tr>`;
                }
                return trs;
            })();
            $TableHotKey.empty().html(getTRS);
        })
        .catch(error => {
            Logger.error('無法取得熱門關鍵字', error);
            $TableHotKey.empty().html(UI.Error.ErrorSegment('無法取得熱門關鍵字', '嘗試變更其他區間查詢'));
        });
};
/**
 * 近期活動圖表-入庫和調用柱狀圖
 * Notice:Chart.js目前使用2.5版本,2.6~2.9版本在交互與動態資料同步上有已知的奇怪Bug
 */
const BarChartTask = () => {
    const ctx: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById('myChart');
    const chartJSON: ChartModel = JSON.parse(ctx.getAttribute('data-chart'));
    ctx.style.fill = 'black';
    const chartData = {
        labels: chartJSON.Months,
        datasets: [
            {
                type: 'bar',
                label: chartJSON.BranchData[0].LabelStr, //入庫D
                // backgroundColor: pattern.draw('diamond', chartColors.red),
                backgroundColor: HexToRGBA(chartColors.red, 0.2),
                borderColor: chartColors.red,
                borderWidth: 2,
                pointBackgroundColor: chartColors.red,
                data: chartJSON.BranchData[0].Counts,
            },
            {
                type: 'bar',
                label: chartJSON.BranchData[1].LabelStr, //'調用D ',
                // backgroundColor: pattern.draw('dot', chartColors.green),
                backgroundColor: HexToRGBA(chartColors.green, 0.2),
                borderColor: chartColors.green,
                borderWidth: 2,
                pointBackgroundColor: chartColors.green,
                data: chartJSON.BranchData[1].Counts,
            },
        ],
    };
    window.top.TempChart = new Chart(ctx, {
        type: 'bar',
        data: chartData,
        options: {
            maintainAspectRatio: true,
            responsive: true,
            animation: {
                easing: 'easeInOutBack',
            },
            legend: {
                position: 'bottom',
                labels: {
                    fontColor: 'white',
                },
            },
            title: {
                display: true,
                fontSize: 26,
                fontColor: chartColors.white,
                //text: `${new Date().getFullYear() - 1}年-${new Date().getFullYear()}年統計(過去12個月活動)`,
                text: `過去12個月統計值`,
            },
            tooltips: {
                mode: 'index',
                intersect: true,
            },
            scale: {
                pointLabels: {
                    fontColor: 'white',
                },
                angleLines: {
                    color: chartColors.white,
                },
            },
            scales: {
                ticks: {
                    beginAtZero: true,
                    fontColor: 'white',
                    showLabelBackdrop: false,
                },

                xAxes: [
                    {
                        type: 'category',
                        display: true,
                        scaleLabel: {
                            display: true,
                            labelString: '日期',
                            fontColor:'#ccc',
                            fontSize: 15,
                        },
                        gridLines: {
                            color: 'rgba(255, 255, 255, 0.2)',
                        },
                        ticks: {
                            fontSize: 15,
                            fontColor: chartColors.white,
                            sampleSize: chartJSON.Months.length,
                            //color: 'rgba(255, 255, 255, 0.2)'
                        },
                    },
                ],
                yAxes: [
                    {
                        type: 'linear',
                        display: true,
                        scaleLabel: {
                            display: true,
                            labelString: '調用數量',
                            fontColor:'#ccc',
                            fontSize: 15,
                        },
                        gridLines: {
                            color: 'rgba(255, 255, 255, 0.2)',
                        },
                        ticks: {
                            fontSize: 15,
                            fontColor: chartColors.white,
                            sampleSize: 'ticks.length',
                            //sampleSize: chartJSON.BranchData[0].Counts.length,
                            // color: chartColors.white
                        },
                    },
                ],
            },
        },
    });
};

/**調用量測圖表 */
const GuageBoookingChartTask = (input: GaugeWorkModel) => {//(input: GaugeViewModel) => {
    //https://stackoverflow.com/questions/57218877/how-to-draw-a-needle-on-a-custom-donut-chart-and-add-datapoints-to-the-needle
    const element: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById(input.GaugeId);
    let _guageChart = new Chart(element, {
        type: 'doughnut',
        plugins: [{
            beforeDraw: chart => {
                let count =chart.data.datasets[0].data[0];//執行路數
                let cw = element.offsetWidth;
                let ch = element.offsetHeight;
                let ctx = element.getContext('2d');
                ctx.fillStyle = input.GaugeColor;
                ctx.textAlign = "center";
                ctx.textBaseline = 'top'; // important!
                ctx.font = `${ch / 3}px Arial`;
                ctx.fillText(count.toString(), cw / 2, ch / 2 - ch / 12);
            },
    
        }],
        data: {
            labels: ['執行路數', '未執行路數'],
            datasets: <Array<any>>[{
                data: input.GaugeData,
                borderWidth: 2,
                borderColor: [HexToRGBA(input.GaugeColor, 1), HexToRGBA('#333333', 1)],
                hoverBackgroundColor: [HexToRGBA(input.GaugeColor, 0.6), HexToRGBA('#333333', 0.6)],
                backgroundColor: [HexToRGBA(input.GaugeColor, 0.2), HexToRGBA('#333333', 0.2)],
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            title: {
                display: true,
                fontSize: 18,
                fontColor: '#ccc',
                text: input.GaugeTitle
            },
            rotation: -Math.PI * 1.25,
            circumference: Math.PI * 1.5,
            //   rotation: -Math.PI,
            //   circumference: Math.PI,
            cutoutPercentage: 70,
            legend: {
                display: true,
                position: 'bottom',
                labels:{
                    fontColor:'#ccc'
                }
            },
            animation: {
                animateRotate: false,
                animateScale: true,
            },
          
        }
    });

    if (input.GaugeId == gaugeChartType.BOOK) { window.top.TempGaugeBOOK = _guageChart; }
    else if (input.GaugeId == gaugeChartType.ARC) { window.top.TempGaugeARC = _guageChart; }
};
/**水平堆疊圖表 */
const HorizontalStackChartTask = (id:string) => {
    const element = <HTMLCanvasElement>document.getElementById(id);
   let originJSON:ChartBarModel=IsNullorUndefined(element)||IsNullorUndefined(element.getAttribute('data-chart'))?<ChartBarModel>{}: JSON.parse(element.getAttribute('data-chart'));
   const deepCloneJSON:ChartBarModel=JSON.parse(JSON.stringify(originJSON)); 
   const maxValueInArray=  Math.max.apply(null, deepCloneJSON.BookWorkVals.map(i=>Math.max.apply(Math, i.Values)));
   const showSize=deepCloneJSON.UserLabels.length<=9?10:deepCloneJSON.UserLabels.length;
   const barJSON: ChartBarModel ={
     UserLabels:IsNullorUndefined(deepCloneJSON.UserLabels)?arrayPush([],showSize,''):arrayPush(deepCloneJSON.UserLabels,showSize,''),
     BookWorkVals:IsNullorUndefined(deepCloneJSON.BookWorkVals)?<Array<BranchBooking>>[]:deepCloneJSON.BookWorkVals.map((item)=>{
         return <BranchBooking>{
             Type:item.Type,
             TypeStr:item.TypeStr,
             Values:arrayPush(item.Values,showSize,0),
             BarColorHex:item.BarColorHex
         };
     })
    
   };
   
    window.top.StackedChart= new Chart(element, {
        type: 'horizontalBar',
        options:{
            responsive: true,
            tooltips: {
                enabled: true,
                intersect: true,
            },
            hover :{
                animationDuration:0
            },
            legend:{
                display:true,
                position:'bottom',
            },
            title: {
                display: true,
                fontSize: 22,
                fontColor: chartColors.white,
                text: `今日前10大調用者`,
            },
            animation: {
                onComplete: function() {
                    let chartInstance = this.chart;
                    if(!IsNullorUndefined(chartInstance)){
                        let ctx = chartInstance.ctx;
                        ctx.textAlign = "left";
                        const _data_=IsNullorUndefined(this.data)? <{labels:Array<string>,datasets:Array<ChartDataset>}>{}: <{labels:Array<string>,datasets:Array<ChartDataset>}>this.data;
                        //渲染總計標籤
                        if(!IsNullorUndefined(_data_.datasets) && _data_.datasets.length>0){
                            _data_.labels.forEach((label,index,array)=>{
                                const dataSum=_data_.datasets.map(i=>i.data[index]);
                                let total= dataSum.length===0?0: dataSum.reduce((a,b)=>a+b);
                                let meta = chartInstance.controller.getDatasetMeta(_data_.datasets.length-1);
                                let posX = meta.data[index]._model.x;
                                let posY = meta.data[index]._model.y;
                                ctx.fillStyle = chartColors.white;
                                ctx.fillText((!(IsNULLorEmpty(label) && total===0 ))?'總計:'+total:'',posX+4, posY-4);
                            });               
                        }
                    }  
                  }
            },
            scales: {
                ticks: {
                    beginAtZero: true,
                    showLabelBackdrop: false,
                    precision: 0
                },
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: '調用數量',     
                        fontColor:'#ccc', 
                        fontSize: 12,  
                    },
                    gridLines: {
                        color: 'rgba(255, 255, 255, 0.2)',
                    },
                    ticks: {
                        display: true,
                        beginAtZero:true,
                        fontSize: 15,
                        suggestedMax: maxValueInArray + 5,//如果最大是所有陣列中的最大數(最大那一筆會看不到後面的統計標籤)
                        fontColor:chartColors.white,
                        callback: function(val) {
                            return typeof val !=='string' && Number.isInteger(val)  ? val : null;
                        }
                    },
     
                    stacked: stackStatus
                }],
                yAxes: [{
                    gridLines: {
                        display:false,
                        color: 'rgba(255, 255, 255, 0.2)',
                        zeroLineColor: 'rgba(255, 255, 255, 0.2)',
                        zeroLineWidth: 2
                    },
                    
                    ticks: {
                        fontSize:15,
                        fontColor:chartColors.white,
                    },
                    stacked: stackStatus
                }]
            }
        },
        data: {
            labels:barJSON.UserLabels,
            datasets:barJSON.BookWorkVals.map(item=>{
                return <ChartDataset>{
                    barPercentage:0.5,
                    barThickness:4,
                    maxBarThickness:8,
                    data:item.Values,
                    label:item.TypeStr,
                    backgroundColor: HexToRGBA(item.BarColorHex, 0.3),
                    hoverBackgroundColor: HexToRGBA(item.BarColorHex, 0.6),
                    borderColor: HexToRGBA(item.BarColorHex, 1),
                    borderWidth: 2
                };
            })
        }
    });

}

/*---------------流程---------------------------------*/
/**預設查詢與設定值 */
$topHotKey.val(10);
HotKeyTask(30, 10);

/**關鍵字時間範圍變更 */
$HotKeyDropdown.dropdown({
    onChange: function(value, text, $selectedItem) {
        const top: number = Number($topHotKey.val()) || 10;
        HotKeyTask(Number(value), top);
    },
});
/**TOP 輸入變更 */
$topHotKey
    .keyup(function() {
        const top: number = Number($(this).val()) || hotkeyInitSetting.top;
        const days = Number($HotKeyDropdown.dropdown('get value')) || hotkeyInitSetting.days;
        HotKeyTask(days, top);
    })
    .change(function() {
        const top: number = Number($(this).val()) || hotkeyInitSetting.top;
        const days = Number($HotKeyDropdown.dropdown('get value')) || hotkeyInitSetting.days;
        HotKeyTask(days, top);
    });

/**文檔就緒後再執行 */
$(document).ready(function() {
    BarChartTask();
   if(DashBoardSetting.ShowGuageChart){
        const elementBook: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById('guageBoooking');
        const elementARC: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById('guageUpload');
        const guageBookModel: GaugeWorkModel = JSON.parse(elementBook.getAttribute('data-chart'));
        const guageArcModel: GaugeWorkModel = JSON.parse(elementARC.getAttribute('data-chart'));
        HorizontalStackChartTask("stackBar");
        GuageBoookingChartTask(guageBookModel); 
        GuageBoookingChartTask(guageArcModel);
        $('#stackchangeBtn').click(function(){
            stackStatus=!stackStatus;
            if(window.top.StackedChart!=null){window.top.StackedChart.destroy();}
            HorizontalStackChartTask("stackBar");
        });
    }else{
        document.getElementById('guagechartRow').style.display='none';
    }
});
