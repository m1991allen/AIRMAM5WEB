import { initSetting, SignalRSetting } from '../initSetting';
import { ErrorMessage, InfoMessage, SuccessMessage, WarningMessage } from '../Function/Message';
import { IsNullorUndefined } from '../Function/Check';
import { Logger } from './LoggerService';
/**
 * signalR服務介面
 * 使用方法參考 https://docs.microsoft.com/zh-tw/aspnet/signalr/overview/guide-to-the-api/hubs-api-guide-javascript-client#dynamicproxy
 */
export interface IsignalRService {
   /**
   * 開始進行連線
   * @param clientcallback 
   * @param servercallback 
   */
     connect(clientcallback?:(connectId:string,hub:SignalR.Hub.Proxy)=>void,
             servercallback?:(connectId:string,hub:SignalR.Hub.Proxy)=>void): void;
    /**
     * 中斷連線
     * @param cusevent 中斷連線後事件
     */
    disconnect(cusevent?: Function): void;
}
/**
 * signalR服務
 */
export class signalRService implements IsignalRService {
    /**暫存連線 */
    private connection: SignalR.Hub.Connection;
    private hubProxy: SignalR.Hub.Proxy;
    private trytime: number = 0;
    private maxtrytime: number;
    private hubName: string;
    constructor(hubName: string | 'broadcastHub') {
        this.connection = $.hubConnection();
        this.connection.url = initSetting.SignalRUrl;
        this.trytime == 0;
        this.maxtrytime =  SignalRSetting.MaxTryTime;
        this.hubProxy = this.connection.createHubProxy(hubName);
        this.hubName = hubName;
        Logger.log(`signalRService[${hubName}] 初始化`);
    }

    connect(clientcallback?:(connectId:string,hub:SignalR.Hub.Proxy)=>void,
             servercallback?:(connectId:string,hub:SignalR.Hub.Proxy)=>void): void {
       
        if(!IsNullorUndefined(clientcallback)){
            Logger.log(`註冊signalR client事件`);
            clientcallback(this.connection.id, this.hubProxy);
        }
        this.connection.start()
        .done(()=>{
             Logger.log(`signalRService[${this.hubName}]開始連線(第${this.trytime}次連線)..., 連線ID=${this.connection.id}`);
             this.trytime = 0; 
              if(!IsNullorUndefined(servercallback)){
                Logger.log(`註冊signalR server事件`);
                servercallback(this.connection.id, this.hubProxy);
              }
            })
        .fail(()=>{ Logger.error('無法連線'); });

        this.connection.connectionSlow(()=>{
            WarningMessage(`SignalR連線過慢`);
            Logger.log(`SignalR連線過慢`);
        });
        this.connection.reconnected(()=>{
            SuccessMessage(`SignalR已經重新連線`);
            Logger.log(`SignalR已經重新連線`);
            this.trytime=0;
        });
        this.connection.reconnecting(()=>{
            InfoMessage(`SignalR正在嘗試連線`);
            Logger.log(`SignalR正在嘗試連線`);
        });
        this.connection.disconnected(()=>{
            Logger.log(`偵測到signalR連線斷線`);
            if (this.trytime <= this.maxtrytime) {
                const timerId=setTimeout(()=> {
                    this.trytime += 1;
                    this.connection.start();
                    Logger.log(`[${new Date().toISOString()}]signalR重新start`);
                    clearTimeout(timerId);
                    return false;
                }, SignalRSetting.ErrorTryInterval * 1000);
              
            } else {
                this.disconnect(()=>{
                    Logger.error(`嘗試多次連線,均無法與signalR連線`);
                    ErrorMessage(`嘗試多次連線,均無法與signalR進行連線`);
                });
            }
        });
    
   
     

    }
    disconnect(cusevent?: Function): void {
        try {
            this.connection.stop();
            Logger.log(`中止signalR連線`);
        } catch (error) {
            Logger.error(`無法中止signalR連線`,error);
        }finally{
            if(!IsNullorUndefined(cusevent)){
                cusevent();
            }
        }
    }
}
