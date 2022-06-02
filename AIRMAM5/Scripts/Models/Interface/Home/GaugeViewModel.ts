/**
 * 
 */
// export interface GaugeViewModel{
//    id:string;
//    data:Array<number>;
//    // needleValue:number;
//    color:string;
//    title:string;
//}
/**
 * Dashboard主機調用,入庫作業 圖表資料 */
export interface GaugeWorkModel {
    GaugeId: string;
    GaugeData: Array<number>;
    GaugeColor: string;
    GaugeTitle: string;
}