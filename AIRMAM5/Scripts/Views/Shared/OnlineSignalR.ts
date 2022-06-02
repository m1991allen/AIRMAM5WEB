import { IsignalRService, signalRService } from '../../Models/Class/signalRService';
import { UserConnectedModel } from '../../Models/Interface/Shared/OnlineSignalR/UserConnectedModel';
import { UserInfoModel } from '../../Models/Interface/Shared/OnlineSignalR/UserInfoModel';
import { ErrorMessage, SuccessMessage, windowPostMessage} from '../../Models/Function/Message';
import { NotifyDataModel, UserNotifyModel } from '../../Models/Interface/Shared/OnlineSignalR/UserNotifyModel';
import { OnlineDataModel } from '../../Models/Interface/Shared/OnlineSignalR/OnlineDataModel';
import { SidebarEnum } from '../../Models/Enum/SidebarEnum';
import { INotifyController, NotifyController } from '../../Models/Controller/NotifyController';
import { AnnListModel } from '../../Models/Interface/Ann/AnnListModel'; 
import { DashboardViewModel } from '../../Models/Interface/Home/DashboardViewModel'; 
import { AnnType } from '../../Models/Enum/AnnEnum';
import { Color } from '../../Models/Enum/ColorEnum';
import { Logger } from '../../Models/Class/LoggerService';
import { DashBoadType } from '../../Models/Enum/DashBoadType';
import { IsNULL, IsNullorUndefined } from '../../Models/Function/Check';
import { DashBoardSetting, SignalRSetting } from '../../Models/initSetting';
import { GetPendingTapeModel } from '../../Models/Interface/Tsm/GetPendingTapeModel';
import { Controller } from '../../Models/Enum/Controller';
import { Action } from '../../Models/Enum/Action';
import { TsmCheckInUpdateModel } from '../../Models/Interface/DocumentViewer/DocumentPostMessageModal';
import { HttpStatusCode } from '../../Models/Enum/HttpStatusCode';
import { GaugeWorkModel } from '../../Models/Interface/Home/GaugeViewModel';
import { HexToRGBA } from '../../Models/Function/Color';
import { gaugeChartType } from '../../Views/Shared/_windowParameter';

/**初始化signalR服務 */
const signalR: IsignalRService = new signalRService('broadcastHub');
const $notifyLabel: JQuery<HTMLElement> = $('#UserNotify');
const $messgePanel: JQuery<HTMLElement>= $('#Sidebar').find(`.inside[subIndex=${SidebarEnum.Message}]`);
const $messageSideBar: JQuery<HTMLElement> =$messgePanel.children('#MessageSide');
const $onlineLabel: JQuery<HTMLElement> = $('#OnlineUsers');
const $onlineSideBar: JQuery<HTMLElement> = $('#Sidebar').find(`.inside[subIndex=${SidebarEnum.OnlinePerson}]`);
const $readallBtn: JQuery<HTMLButtonElement> = $('button[name="readall"]');
/**
 * @param tempDataList 暫存訊息資料
 * @param  batchCount 每批的訊息渲染數量
 */
let message:{tempDataList:Array<NotifyDataModel>;batchCount:number}={tempDataList:[],batchCount:50};
/**暫存上次滾動位置*/let previousScroll = 0;
/**後端broadcastHub的signalR事件名稱 */
const broadcastHubCusEvent = {
    client: {
        showmessage: 'showmessage',
        showMyNotify: 'showMyNotify',
        /**更新線上人數 */
        showOnline: 'showOnline',
        refreshAnnounce: 'refreshAnnounce',
        /**更新Dashboard入庫/調用數字, */
        refreshCounts: 'refreshCounts',
        /**更新上架磁帶資料 */
        refreshTapeWaitCheckIn: 'refreshTapeWaitCheckIn',
        /**檢查是否有上架中作業 */
        checkPendingWorks: 'checkPendingWorks',
        /**更新Dashboard主機調用作業 執行路數 */
        refreshBookArcWorkQty: 'refreshBookArcWorkQty',
    },
    server: {
        SendMessageForAll: 'SendMessageForAll',
        SendMessageForId: 'SendMessageForId',
        updateConnectionId: 'updateConnectionId',
        updateLastTime: 'updateLastTime',
        userConnected: 'userConnected',
    },
};

/**創建通知訊息的fragment */
const messgaeFragment=(list:Array<NotifyDataModel>):DocumentFragment=>{
    const fragment = document.createDocumentFragment();
    for (let data of list) {
        const item: HTMLDivElement = document.createElement('div');
        let headerClass='',readbtn='';
        item.setAttribute('data-MessageId', data.NOTIFY_ID.toString());
        item.className = 'ui raised card inverted';
        item.setAttribute('data-IsRead', !data.IsRead ? 'false' : 'true');
        const status:'success'|'warning'|'error'=data.CONTENT.indexOf('成功') > -1?'success':data.CONTENT.indexOf('失敗') > -1?'error':'warning';
        switch(true){
            case data.IsRead:
                headerClass='grey';
                readbtn='<label class="ui mini label">已讀</label>';
                break;
            case status==='success':
                headerClass='green inverted';
                data.CONTENT=data.CONTENT.replace(/成功/gm, '<span class="ui small green inverted header">成功</span>');
                readbtn='<button type="button" name="unread" class="ui basic inverted mini button" data-inverted="" data-tooltip="設置為已讀" data-position="bottom center">未讀</button>';
                break;
            case  status==='error':
                headerClass='red inverted';
                data.CONTENT= data.CONTENT.replace(/失敗/gm, '<span class="ui small red inverted header">失敗</span>');
                readbtn='<button type="button" name="unread" class="ui basic inverted mini button" data-inverted="" data-tooltip="設置為已讀" data-position="bottom center">未讀</button>';
                break;
            case  status==='warning':
                headerClass='yellow inverted';
                readbtn='<button type="button" name="unread" class="ui basic inverted mini button" data-inverted="" data-tooltip="設置為已讀" data-position="bottom center">未讀</button>';
                break;
            
        }
          item.innerHTML = ` <div class="content">
                            <div class="header" style="line-height: 1.6;">
                               <div class="ui ${headerClass} tiny header">${data.TITLE}</div>
                            </div>
                            <div class="description">${data.CONTENT}</div>
                        </div>
                        <div class="extra content">${readbtn}<div class="right floated">${data.CREATED_DATE}</div></div>`;
        fragment.appendChild(item);
    }
    return fragment;
};
/**signalR事件處理 */
const userInfo = <UserInfoModel>JSON.parse($('#UserInfoJson').attr('data-JSON'));
/*連線呼叫*/
var callsignalR=(function() {

    signalR.connect((connectionId,broadcastHub)=> {
             /*----------------------client接收server訊息------------------------------------------------- */
            /*client接收到server訊息 */
            broadcastHub.on(broadcastHubCusEvent.client.showmessage, (msg: string) => {
                Logger.log(msg);
            });
            /**更新在線人數與在線清單 */
            broadcastHub.on(broadcastHubCusEvent.client.showOnline, (json: OnlineDataModel) => {
                const onlineinfo = json;
                const fakediv = document.createElement('div');
                const fragment = document.createDocumentFragment();
                onlineinfo.Number > 0
                    ? $onlineLabel.siblings().addClass('open')
                    : $onlineLabel.siblings().removeClass('open');
                onlineinfo.Number > 0
                    ? $onlineLabel.html(json.Number.toString()).show()
                    : $onlineLabel.html('0').hide();
                $onlineSideBar.empty();
                for (let data of onlineinfo.DataList) {
                    const item = document.createElement('div');
                    item.innerHTML = `<div class='item' title='最後更新時間:${data.UpdateDTime}'>
                                 <i class='user icon'></i>
                                 ${data.UserName},${data.Note}
                              </div> `;
                    fragment.appendChild(item);
                }
                fakediv.append(fragment);
                $onlineSideBar.html(fakediv.innerHTML);
            });
            /*更新使用者未讀訊息與數字*/
            broadcastHub.on(broadcastHubCusEvent.client.showMyNotify, (json: UserNotifyModel) => {
                const remainlist = json.DataList.splice(message.batchCount);   
                const list =json.DataList.splice(0,message.batchCount);
                if(message.tempDataList.length===0){
                    message.tempDataList=remainlist;
                }else{
                    message.tempDataList=message.tempDataList.concat(json.DataList);
                }
                if (json.UnRead > 0) {
                    $notifyLabel.siblings().addClass('open');
                    $notifyLabel.html(json.UnRead.toString()).show();
                    $readallBtn.removeClass('disabled');
                } else {
                    $notifyLabel.siblings().removeClass('open');
                    $notifyLabel.html('0').hide();
                    $readallBtn.addClass('disabled');
                }
                const fragment = messgaeFragment(list);
                $messageSideBar.prepend(fragment);
            });
            /*
             * 收到server通知:公告更新 -> 2.向server取得最新公告資訊 -> 3.更新DashBoard 區塊
             */
            broadcastHub.on(broadcastHubCusEvent.client.refreshAnnounce, (json: Array<AnnListModel>) => {
                const dashiframe = <HTMLIFrameElement>document.getElementById('DashBoardIframe');
                const dashbody = dashiframe.contentWindow.document.body;
                const $anncards = dashbody.querySelector('#AnnCards');
                const $annrow   = dashbody.querySelector('#AnnRows');
                const fragment  = document.createDocumentFragment();
                for (let data of json) {
                    const card = document.createElement('div');
                    card.className = 'ui card';
                    const cardColor: Color =
                        data.AnnType == AnnType.O ? Color.水鴨藍 : data.AnnType == AnnType.Y ? Color.紅 : Color.橙;
                    card.innerHTML = `<div class="content">
                                        <span class="right floated mini ui ${cardColor} label">${data.AnnTypeName}</span>
                                        <div class="header"> ${data.AnnTitle} </div>
                                        <div class="meta">${data.AnnPublishDept} ${data.AnnSdate}</div>
                                        <div class="description">${data.AnnContent}</div>
                                    </div>
                                    <div class="extra content"> </div>`;
                    fragment.appendChild(card);
                }
                $anncards.innerHTML = '';
                $anncards.appendChild(fragment);
                json.length>0?$annrow.setAttribute('style','display:flex'):$annrow.setAttribute('style','display:none');
            });
            /* 更新入庫/調用統計值、圖表 */
            broadcastHub.on(broadcastHubCusEvent.client.refreshCounts, (json: DashboardViewModel) => {
                Logger.log('signalR更新儀錶板', json);
                const dashiframe = <HTMLIFrameElement>document.getElementById('DashBoardIframe');
                const dashbody = dashiframe.contentWindow.document.body;
                for (let r of json.StatisticsData) {
                    switch (r.Category) {
                        case DashBoadType.TodayUpload:
                            dashbody.querySelector('#todayupload').innerHTML = r.Counts.toString();
                            break;
                        case DashBoadType.TodayBooking:
                            dashbody.querySelector('#todaybooking').innerHTML = r.Counts.toString();
                            break;
                        case DashBoadType.MonthUpload:
                            dashbody.querySelector('#monthupload').innerHTML = r.Counts.toString();
                            break;
                        case DashBoadType.MonthBooking:
                            dashbody.querySelector('#monthbooking').innerHTML = r.Counts.toString();
                            break;
                        case DashBoadType.YesterdayUpload:
                            dashbody.querySelector('#yesterdayupload').innerHTML = r.Counts.toString();
                            break;
                        case DashBoadType.YesterdayBooking:
                            dashbody.querySelector('#yesterdaybooking').innerHTML = r.Counts.toString();
                            break;
                    }
                }
                if (!IsNULL(window.top.TempChart)) {
                    const input = json.Charts;
                    window.top.TempChart.data.labels = input.Months;
                    for (let i = 0; i < input.BranchData.length; i++) {
                        window.top.TempChart.data.datasets[i].data = input.BranchData[i].Counts;
                        window.top.TempChart.data.datasets[i].label = input.BranchData[i].LabelStr;
                    }
                    window.top.TempChart.update();
                }
            });
            /**更新上架磁帶資料 */
            broadcastHub.on(broadcastHubCusEvent.client.refreshTapeWaitCheckIn, (list: Array<GetPendingTapeModel>) => {
                windowPostMessage<TsmCheckInUpdateModel>({ eventid: 'UpdateTsmCheckInList', list: list });
            });
            /**萬一有上架中作業時,禁用上架 */
            broadcastHub.on(broadcastHubCusEvent.client.checkPendingWorks, (IsInWork: 'false' | 'true') => {
                /*Notice:這裡後端的驗證、回傳路徑與資料表都是綁TSM,所以用正確的Tsm會找不到iframe,必須使用大寫TSM,需要確認後端是否要修改*/
                const tsmiframes = document.querySelectorAll(
                    `iframe[src$="${[Controller.Tsm.toUpperCase(), Action.IndexCheckIn].join('/')}"]`
                );
                tsmiframes.forEach(iframe => {
                    const body = (<HTMLIFrameElement>iframe).contentWindow.document.body;
                    const $CheckInBtn = body.querySelector('#CheckInBtn');
                    IsInWork === 'true'
                        ? $CheckInBtn.classList.add('disabled')
                        : $CheckInBtn.classList.remove('disabled');
                });
            });
                 /**更新Dashboard主機調用,入庫作業執行路數 */
            broadcastHub.on(broadcastHubCusEvent.client.refreshBookArcWorkQty, (json: GaugeWorkModel) => {
                if(DashBoardSetting.ShowGuageChart){
                    let _chart_ = json.GaugeId === gaugeChartType.BOOK ? window.top.TempGaugeBOOK : (json.GaugeId === gaugeChartType.ARC ? window.top.TempGaugeARC : null);
                    if(!IsNullorUndefined(_chart_)){
                        _chart_.data.datasets[0]= {
                                data:json.GaugeData,
                                borderColor: [HexToRGBA(json.GaugeColor, 1), HexToRGBA('#333333', 1)],
                                hoverBackgroundColor: [HexToRGBA(json.GaugeColor, 0.6), HexToRGBA('#333333', 0.6)],
                                backgroundColor: [HexToRGBA(json.GaugeColor, 0.2), HexToRGBA('#333333', 0.2)],
                        };            
                         _chart_.options.title.text=json.GaugeTitle;
                         _chart_.options.animation.animateRotate=true;
                         _chart_.options.animation.animateScale=false;
                        _chart_.update();
                        // if(!IsNullorUndefined(_chart_.canvas)){
                        //     _chart_.canvas.dispatchEvent(new Event('resize'));
                        // }

                    }
                }
               
            });
           
        },
        (connectionId,broadcastHub)=>{
        /*----------------------推訊息給server------------------------------------------------- */
            /*推連線資訊給server */
            broadcastHub.invoke(broadcastHubCusEvent.server.userConnected, <UserConnectedModel>{
                UserId: userInfo.fsUSER_ID,
                SignalrConnectionId:connectionId,
                GroupId: userInfo.UserRoles !== undefined ? userInfo.UserRoles.join(',') : '',
                LoginId: userInfo.fsLOGIN_ID,
                LoginLogId: userInfo.LoginLogid,
            });
            /*每分鐘推連線資訊給server,告訴server還在線*/
           const timerID=setInterval(()=> {
               if(broadcastHub.state!==4){
                broadcastHub.invoke(
                    broadcastHubCusEvent.server.updateLastTime,
                    userInfo.fsUSER_ID,
                   connectionId,
                    userInfo.LoginLogid
                );
               }else{
                   clearInterval( timerID);
                   return false;
               }
               
            }, 60 * 1000);
        }
       
    );
});
callsignalR();

/**訊息創建與已讀 */
var notifyroute: INotifyController = new NotifyController();
/*單筆已讀 */
$messageSideBar.on('click', "button[name='unread']", function(event) {
    event.preventDefault();
    const $notifyLabel: JQuery<HTMLElement> = $('#UserNotify');
    const card: JQuery<HTMLDivElement> = $(this).closest('.card');
    const cardId: number = Number(card.attr('data-MessageId'));
    if (typeof cardId !== undefined) {
        notifyroute.ReadNotify([cardId], false).then(res => {
            if (res.IsSuccess) {
                const orginCount = Number($notifyLabel.html());
                if (orginCount > 0) {
                    $notifyLabel.siblings().addClass('open');
                    $notifyLabel.html((orginCount - 1).toString()).show();
                } else {
                    $notifyLabel.siblings().removeClass('open');
                    $notifyLabel.html('0').hide();
                }
                $(this).replaceWith('<label class="ui mini label">已讀</label>');
                card.find('.header')
                    .addClass('grey')
                    .removeClass(['inverted', 'red', 'green', 'yellow']);
            } else {
                ErrorMessage(`${res.Message}<br>------------<br>${card.children('.content').html()}`);
            }
        });
    }
    return false;
});
/**全部已讀 */
$readallBtn.click(function(event) {
    $(this).addClass('disabled loading');
    event.preventDefault();
    notifyroute.ReadNotify([], true).then(res => {
        if (res.IsSuccess) {
            SuccessMessage(res.Message);
            $notifyLabel.siblings().removeClass('open');
            $notifyLabel.html('0').hide();
            const cards = $messageSideBar.find(".card[data-IsRead='false']");
            cards
                .find('.header')
                .addClass('grey')
                .removeClass(['inverted', 'red', 'green', 'yellow']);
            cards.find("button[name='unread']").replaceWith('<label class="ui mini label">已讀</label>');
            $(this).addClass('disabled');
        } else {
            ErrorMessage(res.Message);
        }
        $(this).removeClass('disabled loading');
    });
    return false;
});
$messageSideBar.on('click','a[download]',function(){
    const link=$(this).attr('href');
   $.ajax({
       method:'GET',
       headers:{
           'Access-Control-Allow-Origin':document.location.href,
       },
       url:link,
       error:function (this,xhr, ajaxOptions, thrownError){
        Logger.log(`檔案狀態回應[${ xhr.status}]`,xhr);
            switch( xhr.status){
           case HttpStatusCode.NotFound:
               ErrorMessage('檔案不存在');
               break;
            case HttpStatusCode.Forbidden:
                ErrorMessage('檔案權限不足,無法下載');
                break;
            case 0:
                ErrorMessage('安全政策不符，瀏覽器拒絕執行');
                break;
            case HttpStatusCode.InternalServerError:
            default:
                ErrorMessage('系統錯誤,無法下載');
                break;
       }
      } 
   });
});

/**向下卷軸載入繼續載入新一批的訊息 */
$messgePanel.on('scroll',function(e){
    let currentScroll = $(this).scrollTop();
     //向下卷軸
     if(currentScroll>previousScroll){
         if(($(this).scrollTop() +$(this).height() + 360 > $messageSideBar.height()) && 0 < message.tempDataList.length){
             const loadingList= message.tempDataList.splice(0,message.batchCount);
             const fragment=messgaeFragment(loadingList);
              $messageSideBar.append(fragment);
         }
     }
     previousScroll=currentScroll;
});

  